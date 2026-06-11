using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Benchmarks;

[MemoryDiagnoser]
[SimpleJob]
[MinIterationTime(500)]
[IterationCount(15)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class FixedBenchmarks
{
    private const int Ops = 1_000;
    private static readonly ChiFixed Ten = (ChiFixed)10m;
    private decimal[] _decimalValues = null!;
    private decimal[] _decimalValues2 = null!;
    private double[] _doubleValues = null!;
    private double[] _doubleValues2 = null!;
    private ChiFixed[] _fixedValues = null!;
    private ChiFixed[] _fixedValues2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);

        _fixedValues = new ChiFixed[Ops];
        _fixedValues2 = new ChiFixed[Ops];
        _doubleValues = new double[Ops];
        _doubleValues2 = new double[Ops];
        _decimalValues = new decimal[Ops];
        _decimalValues2 = new decimal[Ops];

        for (var i = 0; i < Ops; i++)
        {
            var v1 = random.NextDouble() * 100 - 50;
            var v2 = random.NextDouble() * 100 - 50;
            if (Math.Abs(v2) < 0.001) v2 = 1.0;

            _doubleValues[i] = v1;
            _doubleValues2[i] = v2;
            _fixedValues[i] = (ChiFixed)v1;
            _fixedValues2[i] = (ChiFixed)v2;
            _decimalValues[i] = (decimal)v1;
            _decimalValues2[i] = (decimal)v2;
        }
    }

    #region Arithmetic

    [Benchmark]
    [BenchmarkCategory("Arithmetic")]
    public ChiFixed Fixed_Add()
    {
        var sum = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            sum += _fixedValues[i];
        return sum;
    }

    [Benchmark]
    [BenchmarkCategory("Arithmetic")]
    public double Double_Add()
    {
        var sum = 0.0;
        for (var i = 0; i < Ops; i++)
            sum += _doubleValues[i];
        return sum;
    }

    [Benchmark]
    [BenchmarkCategory("Arithmetic")]
    public decimal Decimal_Add()
    {
        var sum = 0m;
        for (var i = 0; i < Ops; i++)
            sum += _decimalValues[i];
        return sum;
    }

    [Benchmark]
    [BenchmarkCategory("Arithmetic")]
    public ChiFixed Fixed_Sub()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result -= _fixedValues[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Arithmetic")]
    public double Double_Sub()
    {
        var result = 0.0;
        for (var i = 0; i < Ops; i++)
            result -= _doubleValues[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Arithmetic")]
    public decimal Decimal_Sub()
    {
        var result = 0m;
        for (var i = 0; i < Ops; i++)
            result -= _decimalValues[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Arithmetic")]
    public ChiFixed Fixed_Mul()
    {
        var result = ChiFixed.One;
        for (var i = 0; i < Ops; i++)
            result = _fixedValues[i] * _fixedValues2[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Arithmetic")]
    public double Double_Mul()
    {
        var result = 1.0;
        for (var i = 0; i < Ops; i++)
            result = _doubleValues[i] * _doubleValues2[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Arithmetic")]
    public decimal Decimal_Mul()
    {
        var result = 1m;
        for (var i = 0; i < Ops; i++)
            result = _decimalValues[i] * _decimalValues2[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Arithmetic")]
    public ChiFixed Fixed_Div()
    {
        var result = ChiFixed.One;
        for (var i = 0; i < Ops; i++)
            result = _fixedValues[i] / _fixedValues2[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Arithmetic")]
    public double Double_Div()
    {
        var result = 1.0;
        for (var i = 0; i < Ops; i++)
            result = _doubleValues[i] / _doubleValues2[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Arithmetic")]
    public decimal Decimal_Div()
    {
        var result = 1m;
        for (var i = 0; i < Ops; i++)
            result = _decimalValues[i] / _decimalValues2[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Arithmetic")]
    public ChiFixed Fixed_Mod()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = _fixedValues[i] % _fixedValues2[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Arithmetic")]
    public double Double_Mod()
    {
        var result = 0.0;
        for (var i = 0; i < Ops; i++)
            result = _doubleValues[i] % _doubleValues2[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Arithmetic")]
    public decimal Decimal_Mod()
    {
        var result = 0m;
        for (var i = 0; i < Ops; i++)
            result = _decimalValues[i] % _decimalValues2[i];
        return result;
    }

    #endregion

    #region Rounding

    [Benchmark]
    [BenchmarkCategory("Rounding")]
    public ChiFixed Fixed_Floor()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.Floor(_fixedValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Rounding")]
    public double Double_Floor()
    {
        var result = 0.0;
        for (var i = 0; i < Ops; i++)
            result = Math.Floor(_doubleValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Rounding")]
    public ChiFixed Fixed_Ceiling()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.Ceiling(_fixedValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Rounding")]
    public double Double_Ceiling()
    {
        var result = 0.0;
        for (var i = 0; i < Ops; i++)
            result = Math.Ceiling(_doubleValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Rounding")]
    public ChiFixed Fixed_Truncate()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.Truncate(_fixedValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Rounding")]
    public double Double_Truncate()
    {
        var result = 0.0;
        for (var i = 0; i < Ops; i++)
            result = Math.Truncate(_doubleValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Rounding")]
    public ChiFixed Fixed_Round()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.Round(_fixedValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Rounding")]
    public double Double_Round()
    {
        var result = 0.0;
        for (var i = 0; i < Ops; i++)
            result = Math.Round(_doubleValues[i], MidpointRounding.AwayFromZero);
        return result;
    }

    #endregion

    #region Roots

    [Benchmark]
    [BenchmarkCategory("Roots")]
    public ChiFixed Fixed_Sqrt()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.Sqrt(ChiFixed.Abs(_fixedValues[i]) + ChiFixed.One);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Roots")]
    public double Double_Sqrt()
    {
        var result = 0.0;
        for (var i = 0; i < Ops; i++)
            result = Math.Sqrt(Math.Abs(_doubleValues[i]) + 1.0);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Roots")]
    public ChiFixed Fixed_Cbrt()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.Cbrt(_fixedValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Roots")]
    public double Double_Cbrt()
    {
        var result = 0.0;
        for (var i = 0; i < Ops; i++)
            result = Math.Cbrt(_doubleValues[i]);
        return result;
    }

    #endregion

    #region Power

    [Benchmark]
    [BenchmarkCategory("Power")]
    public ChiFixed Fixed_Pow()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.Pow(ChiFixed.Abs(_fixedValues[i]) + ChiFixed.One, _fixedValues2[i] / Ten);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Power")]
    public double Double_Pow()
    {
        var result = 0.0;
        for (var i = 0; i < Ops; i++)
            result = Math.Pow(Math.Abs(_doubleValues[i]) + 1.0, _doubleValues2[i] / 10.0);
        return result;
    }

    #endregion

    #region Trigonometry

    [Benchmark]
    [BenchmarkCategory("Trig")]
    public ChiFixed Fixed_Sin()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.Sin(_fixedValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Trig")]
    public double Double_Sin()
    {
        var result = 0.0;
        for (var i = 0; i < Ops; i++)
            result = Math.Sin(_doubleValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Trig")]
    public ChiFixed Fixed_Cos()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.Cos(_fixedValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Trig")]
    public double Double_Cos()
    {
        var result = 0.0;
        for (var i = 0; i < Ops; i++)
            result = Math.Cos(_doubleValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Trig")]
    public ChiFixed Fixed_Tan()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.Tan(_fixedValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Trig")]
    public double Double_Tan()
    {
        var result = 0.0;
        for (var i = 0; i < Ops; i++)
            result = Math.Tan(_doubleValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Trig")]
    public (ChiFixed, ChiFixed) Fixed_SinCos()
    {
        var result = (ChiFixed.Zero, ChiFixed.Zero);
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.SinCos(_fixedValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Trig")]
    public (double, double) Double_SinCos()
    {
        var result = (0.0, 0.0);
        for (var i = 0; i < Ops; i++)
            result = Math.SinCos(_doubleValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Trig")]
    public ChiFixed Fixed_Atan()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.Atan(_fixedValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Trig")]
    public double Double_Atan()
    {
        var result = 0.0;
        for (var i = 0; i < Ops; i++)
            result = Math.Atan(_doubleValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Trig")]
    public ChiFixed Fixed_Atan2()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.Atan2(_fixedValues[i], _fixedValues2[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Trig")]
    public double Double_Atan2()
    {
        var result = 0.0;
        for (var i = 0; i < Ops; i++)
            result = Math.Atan2(_doubleValues[i], _doubleValues2[i]);
        return result;
    }

    #endregion

    #region ExpLog

    [Benchmark]
    [BenchmarkCategory("ExpLog")]
    public ChiFixed Fixed_Exp()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.Exp(_fixedValues[i] / Ten);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("ExpLog")]
    public double Double_Exp()
    {
        var result = 0.0;
        for (var i = 0; i < Ops; i++)
            result = Math.Exp(_doubleValues[i] / 10.0);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("ExpLog")]
    public ChiFixed Fixed_Log()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.Log(ChiFixed.Abs(_fixedValues[i]) + ChiFixed.One);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("ExpLog")]
    public double Double_Log()
    {
        var result = 0.0;
        for (var i = 0; i < Ops; i++)
            result = Math.Log(Math.Abs(_doubleValues[i]) + 1.0);
        return result;
    }

    #endregion
}

/*

// * Summary *

BenchmarkDotNet v0.14.0, macOS Sequoia 15.7.7 (24G720) [Darwin 24.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD
  Job-PHPQWI : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD

MinIterationTime=500ms  IterationCount=15

| Method          | Mean        | Error     | StdDev    | Allocated |
|---------------- |------------:|----------:|----------:|----------:|
| Fixed_Add       |    635.6 ns |   0.30 ns |   0.28 ns |         - |
| Double_Add      |    895.6 ns |   0.61 ns |   0.51 ns |         - |
| Decimal_Add     | 12,009.4 ns |   9.37 ns |   8.30 ns |         - |
| Fixed_Sub       |    715.5 ns |   0.48 ns |   0.43 ns |         - |
| Double_Sub      |    895.5 ns |   0.25 ns |   0.21 ns |         - |
| Decimal_Sub     | 12,008.6 ns |   5.14 ns |   4.55 ns |         - |
| Fixed_Mul       |    895.4 ns |   0.20 ns |   0.18 ns |         - |
| Double_Mul      |    669.3 ns |   0.25 ns |   0.23 ns |         - |
| Decimal_Mul     |  8,247.4 ns |  19.10 ns |  17.87 ns |         - |
| Fixed_Div       |  3,701.8 ns |   2.21 ns |   1.85 ns |         - |
| Double_Div      |    669.2 ns |   0.37 ns |   0.35 ns |         - |
| Decimal_Div     | 49,403.0 ns | 292.80 ns | 244.51 ns |         - |
| Fixed_Mod       |    787.7 ns |   0.18 ns |   0.16 ns |         - |
| Double_Mod      |  3,717.0 ns |   7.43 ns |   6.21 ns |         - |
| Decimal_Mod     |  5,830.3 ns |  47.48 ns |  42.09 ns |         - |
|                 |             |           |           |           |
| Fixed_Exp       |  7,099.7 ns |   2.82 ns |   2.36 ns |         - |
| Double_Exp      |  2,908.0 ns |  15.32 ns |  14.33 ns |         - |
| Fixed_Log       |  4,179.5 ns |   2.38 ns |   2.11 ns |         - |
| Double_Log      |  3,179.4 ns |   0.98 ns |   0.92 ns |         - |
|                 |             |           |           |           |
| Fixed_Pow       | 11,798.2 ns |   6.67 ns |   5.91 ns |         - |
| Double_Pow      |  8,919.2 ns |  15.58 ns |  14.57 ns |         - |
|                 |             |           |           |           |
| Fixed_Sqrt      |  1,998.1 ns |   0.69 ns |   0.61 ns |         - |
| Double_Sqrt     |    621.1 ns |   0.26 ns |   0.23 ns |         - |
| Fixed_Cbrt      |  3,168.2 ns |   0.83 ns |   0.74 ns |         - |
| Double_Cbrt     |  2,500.4 ns |   0.98 ns |   0.92 ns |         - |
|                 |             |           |           |           |
| Fixed_Floor     |    559.9 ns |   0.30 ns |   0.27 ns |         - |
| Double_Floor    |    395.2 ns |   0.12 ns |   0.09 ns |         - |
| Fixed_Ceiling   |    550.7 ns |   0.15 ns |   0.14 ns |         - |
| Double_Ceiling  |    395.2 ns |   0.11 ns |   0.10 ns |         - |
| Fixed_Truncate  |    907.9 ns |   0.22 ns |   0.21 ns |         - |
| Double_Truncate |    395.3 ns |   0.19 ns |   0.17 ns |         - |
| Fixed_Round     |    951.4 ns |   0.46 ns |   0.41 ns |         - |
| Double_Round    |    395.4 ns |   0.24 ns |   0.22 ns |         - |
|                 |             |           |           |           |
| Fixed_Sin       |  2,265.1 ns |   1.86 ns |   1.74 ns |         - |
| Double_Sin      |  3,666.6 ns |  17.20 ns |  16.09 ns |         - |
| Fixed_Cos       |  2,300.0 ns |  51.58 ns |  48.24 ns |         - |
| Double_Cos      |  3,661.0 ns |  20.06 ns |  18.76 ns |         - |
| Fixed_Tan       |  4,384.6 ns |   3.55 ns |   3.15 ns |         - |
| Double_Tan      |  4,279.6 ns |  37.82 ns |  35.38 ns |         - |
| Fixed_SinCos    |  2,279.0 ns |   4.06 ns |   3.80 ns |         - |
| Double_SinCos   |  8,335.4 ns |  21.99 ns |  20.57 ns |         - |
| Fixed_Atan      |  4,889.0 ns |   5.89 ns |   4.92 ns |         - |
| Double_Atan     |  4,723.6 ns |  24.15 ns |  20.16 ns |         - |
| Fixed_Atan2     |  5,217.9 ns |  71.79 ns |  67.16 ns |         - |
| Double_Atan2    |  6,322.0 ns |  55.10 ns |  46.01 ns |         - |

// * Hints *
Outliers
  FixedBenchmarks.Fixed_Add: MinIterationTime=500ms, IterationCount=15       -> 1 outlier  was  detected (636.89 ns)
  FixedBenchmarks.Double_Add: MinIterationTime=500ms, IterationCount=15      -> 2 outliers were removed (900.31 ns, 901.04 ns)
  FixedBenchmarks.Decimal_Add: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (12.04 us)
  FixedBenchmarks.Fixed_Sub: MinIterationTime=500ms, IterationCount=15       -> 1 outlier  was  removed (722.20 ns)
  FixedBenchmarks.Double_Sub: MinIterationTime=500ms, IterationCount=15      -> 2 outliers were removed (899.12 ns, 901.12 ns)
  FixedBenchmarks.Fixed_Mul: MinIterationTime=500ms, IterationCount=15       -> 1 outlier  was  removed (903.20 ns)
  FixedBenchmarks.Fixed_Div: MinIterationTime=500ms, IterationCount=15       -> 2 outliers were removed (3.72 us, 3.73 us)
  FixedBenchmarks.Decimal_Div: MinIterationTime=500ms, IterationCount=15     -> 2 outliers were removed (50.32 us, 51.73 us)
  FixedBenchmarks.Fixed_Mod: MinIterationTime=500ms, IterationCount=15       -> 1 outlier  was  removed, 2 outliers were detected (789.31 ns, 790.24 ns)
  FixedBenchmarks.Double_Mod: MinIterationTime=500ms, IterationCount=15      -> 2 outliers were removed (3.75 us, 3.82 us)
  FixedBenchmarks.Decimal_Mod: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (5.95 us)
  FixedBenchmarks.Fixed_Exp: MinIterationTime=500ms, IterationCount=15       -> 2 outliers were removed, 3 outliers were detected (7.10 us, 7.12 us, 7.13 us)
  FixedBenchmarks.Fixed_Log: MinIterationTime=500ms, IterationCount=15       -> 1 outlier  was  removed (4.19 us)
  FixedBenchmarks.Fixed_Pow: MinIterationTime=500ms, IterationCount=15       -> 1 outlier  was  removed (11.82 us)
  FixedBenchmarks.Fixed_Sqrt: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (2.00 us)
  FixedBenchmarks.Double_Sqrt: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (623.95 ns)
  FixedBenchmarks.Fixed_Cbrt: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (3.19 us)
  FixedBenchmarks.Fixed_Floor: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (562.60 ns)
  FixedBenchmarks.Double_Floor: MinIterationTime=500ms, IterationCount=15    -> 3 outliers were removed, 5 outliers were detected (397.08 ns, 397.09 ns, 397.54 ns..398.16 ns)
  FixedBenchmarks.Fixed_Ceiling: MinIterationTime=500ms, IterationCount=15   -> 1 outlier  was  removed (556.81 ns)
  FixedBenchmarks.Double_Ceiling: MinIterationTime=500ms, IterationCount=15  -> 1 outlier  was  removed, 2 outliers were detected (396.98 ns, 397.41 ns)
  FixedBenchmarks.Double_Truncate: MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  removed (398.85 ns)
  FixedBenchmarks.Fixed_Round: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (954.74 ns)
  FixedBenchmarks.Double_Round: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  removed (398.18 ns)
  FixedBenchmarks.Double_Sin: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  detected (3.63 us)
  FixedBenchmarks.Fixed_Tan: MinIterationTime=500ms, IterationCount=15       -> 1 outlier  was  removed (4.40 us)
  FixedBenchmarks.Fixed_Atan: MinIterationTime=500ms, IterationCount=15      -> 2 outliers were removed (4.95 us, 4.95 us)
  FixedBenchmarks.Double_Atan: MinIterationTime=500ms, IterationCount=15     -> 2 outliers were removed (4.86 us, 4.93 us)
  FixedBenchmarks.Double_Atan2: MinIterationTime=500ms, IterationCount=15    -> 2 outliers were removed (6.58 us, 6.64 us)

// * Legends *
  Mean      : Arithmetic mean of all measurements
  Error     : Half of 99.9% confidence interval
  StdDev    : Standard deviation of all measurements
  Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 ns      : 1 Nanosecond (0.000000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
Run time: 00:15:03 (903.96 sec), executed benchmarks: 45

*/