using System.Numerics;
using BenchmarkDotNet.Attributes;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Benchmarks;

[MemoryDiagnoser]
[SimpleJob]
[MinIterationTime(500)]
[IterationCount(15)]
public class DivExperimentBenchmarks
{
    private const int Ops = 1_000;
    private const int Seed = 42;

    private double[] _doubleValues = null!;
    private double[] _doubleValues2 = null!;
    private ChiFixed[] _fixedValues = null!;
    private ChiFixed[] _fixedValues2 = null!;
    private Int128[] _int128Values = null!;
    private Int128[] _int128Values2 = null!;
    private long[] _longValues = null!;
    private long[] _longValues2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        var rng = new ChiRng(Seed);

        _doubleValues = new double[Ops];
        _doubleValues2 = new double[Ops];
        _longValues = new long[Ops];
        _longValues2 = new long[Ops];
        _int128Values = new Int128[Ops];
        _int128Values2 = new Int128[Ops];
        _fixedValues = new ChiFixed[Ops];
        _fixedValues2 = new ChiFixed[Ops];

        // Operands: log-normal magnitude with a random sign, so the data spans the whole
        // ChiFixed domain instead of a single band — many sub-1 values (fast path), many
        // large ones (Knuth path), and the occasional large-dividend / small-divisor pair
        // that trips the saturation early-out. a and b are independent draws, so they
        // decorrelate for free. Sigma 7 makes +/-3 sigma cover ChiFixed's full ~19-decade
        // dynamic range (2^-32 .. 2^31); median 1 splits the data evenly below/above 1.
        const decimal logMean = 0.0m; // mu of the underlying normal: median magnitude = 1
        const decimal logSigma = 7.0m; // sigma: magnitudes span ~1e-9 .. 1e9
        const decimal minAbsDivisor = 1e-9m; // keep the divisor clear of a round-to-zero ChiFixed
        const decimal maxLogMagnitude = 21.4m; // ≈ ln(ChiFixed.MaxValue): clamp so the tail can't overflow decimal Exp

        for (var i = 0; i < Ops; i++)
        {
            var v1 = NextSignedMagnitude(ref rng);
            var v2 = NextSignedMagnitude(ref rng);
            if (Math.Abs(v2) < minAbsDivisor) v2 = 1.0m;

            _doubleValues[i] = (double)v1;
            _doubleValues2[i] = (double)v2;
            _fixedValues[i] = (ChiFixed)v1;
            _fixedValues2[i] = (ChiFixed)v2;
            _longValues[i] = _fixedValues[i].Raw;
            _longValues2[i] = _fixedValues2[i].Raw;
            _int128Values[i] = _longValues[i];
            _int128Values2[i] = _longValues2[i];
        }

        return;

        static decimal NextSignedMagnitude(ref ChiRng rng)
        {
            // Draw the log-magnitude (the underlying normal) and clamp it to ChiFixed's range
            // before exponentiating. On the decimal path the sigma-7 tail can reach ~±11, and
            // Exp of that overflows decimal (~Exp(66)); clamping bounds it — and also avoids
            // wasting draws on magnitudes that would just saturate on conversion anyway.
            var sample = rng.Normal(logMean, logSigma).Sample();
            var logMagnitude = Math.Clamp(sample, -maxLogMagnitude, maxLogMagnitude);
            var magnitude = ChiMath.Exp(logMagnitude);
            return rng.Chance().FlipCoin() ? magnitude : -magnitude;
        }
    }

    [Benchmark(Baseline = true)]
    public double Div_Double()
    {
        var result = 1.0;
        for (var i = 0; i < Ops; i++)
            result = _doubleValues[i] / _doubleValues2[i];
        return result;
    }

    [Benchmark]
    public long Div_Long()
    {
        var result = 1L;
        for (var i = 0; i < Ops; i++)
            result = _longValues[i] / _longValues2[i];
        return result;
    }

    [Benchmark]
    public Int128 Div_Int128()
    {
        var result = (Int128)1;
        for (var i = 0; i < Ops; i++)
            result = _int128Values[i] / _int128Values2[i];
        return result;
    }

    // Current ChiFixed division, inlined from FixedMath.Div.
    [Benchmark]
    public ChiFixed Div_Schoolbook()
    {
        var result = ChiFixed.One;
        for (var i = 0; i < Ops; i++)
        {
            var a = _fixedValues[i].Raw;
            var b = _fixedValues2[i].Raw;

            if (b == 0) throw new DivideByZeroException();

            var negative = (a ^ b) < 0;
            var uA = (ulong)(a < 0 ? -a : a);
            var uB = (ulong)(b < 0 ? -b : b);

            // Decompose: (uA << 32) / uB = (uA/uB) << 32 + ((uA%uB) << 32) / uB
            var q1 = uA / uB;
            var r1 = uA - q1 * uB;

            long resultRaw;
            if (q1 > uint.MaxValue)
            {
                resultRaw = negative ? long.MinValue : long.MaxValue;
            }
            else
            {
                ulong q2;
                var r1Hi = r1 >> 32;
                if (r1Hi == 0)
                {
                    // r1 << 32 fits in 64 bits — plain division
                    q2 = (r1 << 32) / uB;
                }
                else
                {
                    // Single-digit Knuth: quotient fits in 32 bits
                    var shift = BitOperations.LeadingZeroCount(uB);
                    var d = uB << shift;
                    var nHi = (r1Hi << shift) | (shift == 0 ? 0 : (r1 << 32) >> (64 - shift));
                    var nLo = r1 << (32 + shift);

                    var dHi = d >> 32;
                    var dLo = d & 0xFFFF_FFFF;
                    var rem = (nHi << 32) | (nLo >> 32);

                    q2 = rem / dHi;
                    var rr = rem - q2 * dHi;
                    while (q2 >= 0x1_0000_0000UL || q2 * dLo > ((rr << 32) | (nLo & 0xFFFF_FFFF)))
                    {
                        q2--;
                        rr += dHi;
                        if (rr >= 0x1_0000_0000UL) break;
                    }
                }

                var quotient = (q1 << 32) | q2;
                if (quotient > long.MaxValue)
                    resultRaw = negative ? long.MinValue : long.MaxValue;
                else
                    resultRaw = negative ? -(long)quotient : (long)quotient;
            }

            result = new ChiFixed(resultRaw);
        }

        return result;
    }
}

/*

// * Summary *

BenchmarkDotNet v0.14.0, macOS Sequoia 15.7.7 (24G720) [Darwin 24.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD
  Job-VULMAL : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD

MinIterationTime=500ms  IterationCount=15

| Method         | Mean       | Error   | StdDev  | Ratio | Allocated | Alloc Ratio |
|--------------- |-----------:|--------:|--------:|------:|----------:|------------:|
| Div_Double     |   684.3 ns | 1.88 ns | 1.67 ns |  1.00 |         - |          NA |
| Div_Long       |   962.9 ns | 2.95 ns | 2.76 ns |  1.41 |         - |          NA |
| Div_Int128     | 2,736.1 ns | 7.51 ns | 6.27 ns |  4.00 |         - |          NA |
| Div_Schoolbook | 3,341.1 ns | 3.43 ns | 3.04 ns |  4.88 |         - |          NA |

// * Hints *
Outliers
  DivExperimentBenchmarks.Div_Double: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (705.99 ns)
  DivExperimentBenchmarks.Div_Int128: MinIterationTime=500ms, IterationCount=15     -> 2 outliers were removed (2.81 us, 2.81 us)
  DivExperimentBenchmarks.Div_Schoolbook: MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  removed, 3 outliers were detected (3.34 us, 3.34 us, 3.39 us)

// * Legends *
  Mean        : Arithmetic mean of all measurements
  Error       : Half of 99.9% confidence interval
  StdDev      : Standard deviation of all measurements
  Ratio       : Mean of the ratio distribution ([Current]/[Baseline])
  Allocated   : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  Alloc Ratio : Allocated memory ratio distribution ([Current]/[Baseline])
  1 ns        : 1 Nanosecond (0.000000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
Run time: 00:01:17 (77.27 sec), executed benchmarks: 4

Global total time: 00:01:24 (84.03 sec), executed benchmarks: 4
// * Artifacts cleanup *
Artifacts cleanup is finished

*/