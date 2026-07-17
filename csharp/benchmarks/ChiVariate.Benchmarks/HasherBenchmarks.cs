using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Benchmarks;

[MemoryDiagnoser]
[SimpleJob]
[MinIterationTime(500)]
[IterationCount(15)]
public class HasherBenchmarks
{
    private const int IterationCount = 1_000;
    private const string Text = "Apollo 440";
    private const double Value = Math.PI * Math.E * Math.Tau;

    [Benchmark(Description = "HashCode.Hash32", Baseline = true)]
    public bool HashCode_Build()
    {
        var sum = 0;
        for (var i = 0; i < IterationCount; i++)
        {
            var hash = new HashCode();
            hash.Add(Text);
            hash.Add(Value);
            sum += hash.ToHashCode();
        }

        return Consume(sum);
    }

    [Benchmark(Description = "ChiHash.Hash32")]
    public bool ChiHash_Build()
    {
        var sum = 0;
        for (var i = 0; i < IterationCount; i++)
            sum += new ChiHash().Add(Text).Add(Value).Hash;
        return Consume(sum);
    }

    [Benchmark(Description = "ChiSeed.Hash64")]
    public bool ChiSeed_Scramble()
    {
        var sum = 0L;
        for (var i = 0; i < IterationCount; i++)
            sum += ChiSeed.Scramble(Text, Value);
        return Consume(sum);
    }

    [Benchmark(Description = "ChiSeed.Entropy64")]
    public bool ChiSeed_GetEntropy()
    {
        var sum = 0L;
        for (var i = 0; i < IterationCount; i++)
            sum += ChiSeed.GetEntropy();
        return Consume(sum);
    }

    [Benchmark(Description = "CryptoRng.Generate64")]
    public bool RandomNumberGenerator_FillBytes()
    {
        var sum = 0L;
        Span<byte> buffer = stackalloc byte[8];
        for (var i = 0; i < IterationCount; i++)
        {
            RandomNumberGenerator.Fill(buffer);
            sum += Unsafe.ReadUnaligned<long>(ref buffer[0]);
        }

        return Consume(sum);
    }

    private static bool Consume<T>(T value) where T : INumberBase<T>
    {
        if (value.GetHashCode() == Environment.TickCount)
            Console.WriteLine(value);
        return true;
    }
}

/*

// * Summary *

BenchmarkDotNet v0.14.0, macOS 26.5.2 (25F84) [Darwin 25.5.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD
  Job-SWUTRJ : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD

MinIterationTime=500ms  IterationCount=15

| Method               | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
|--------------------- |---------:|---------:|---------:|------:|----------:|------------:|
| HashCode.Hash32      | 11.67 us | 0.008 us | 0.006 us |  1.00 |         - |          NA |
| ChiHash.Hash32       | 31.81 us | 0.075 us | 0.070 us |  2.73 |         - |          NA |
| ChiSeed.Hash64       | 38.05 us | 0.054 us | 0.051 us |  3.26 |         - |          NA |
| ChiSeed.Entropy64    | 82.25 us | 0.054 us | 0.051 us |  7.05 |         - |          NA |
| CryptoRng.Generate64 | 81.78 us | 0.068 us | 0.061 us |  7.01 |         - |          NA |

// * Hints *
Outliers
  HasherBenchmarks.HashCode.Hash32: MinIterationTime=500ms, IterationCount=15      -> 3 outliers were removed (11.70 us..11.87 us)
  HasherBenchmarks.CryptoRng.Generate64: MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  removed (82.00 us)

// * Legends *
  Mean        : Arithmetic mean of all measurements
  Error       : Half of 99.9% confidence interval
  StdDev      : Standard deviation of all measurements
  Ratio       : Mean of the ratio distribution ([Current]/[Baseline])
  Allocated   : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  Alloc Ratio : Allocated memory ratio distribution ([Current]/[Baseline])
  1 us        : 1 Microsecond (0.000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
Run time: 00:01:24 (84.98 sec), executed benchmarks: 5

Global total time: 00:01:31 (91.56 sec), executed benchmarks: 5
// * Artifacts cleanup *
Artifacts cleanup is finished

Process finished with exit code 0.

*/