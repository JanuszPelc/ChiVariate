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

        for (var i = 0; i < Ops; i++)
        {
            var v1 = random.NextDouble() * 100 - 50;
            var v2 = random.NextDouble() * 100 - 50;
            if (Math.Abs(v2) < 0.001) v2 = 1.0;

            _doubleValues[i] = v1;
            _doubleValues2[i] = v2;
            _fixedValues[i] = (ChiFixed)v1;
            _fixedValues2[i] = (ChiFixed)v2;
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

BenchmarkDotNet v0.14.0, macOS Sequoia 15.7.3 (24G419) [Darwin 24.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD
  Job-QMXRNN : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD

MinIterationTime=500ms  IterationCount=15

| Method          | Mean        | Error    | StdDev   | Allocated |
|---------------- |------------:|---------:|---------:|----------:|
| Fixed_Add       |    638.5 ns |  0.32 ns |  0.29 ns |         - |
| Double_Add      |    899.9 ns |  0.84 ns |  0.79 ns |         - |
| Fixed_Sub       |    719.4 ns |  0.60 ns |  0.50 ns |         - |
| Double_Sub      |    900.0 ns |  0.47 ns |  0.44 ns |         - |
| Fixed_Mul       |    902.5 ns |  0.69 ns |  0.61 ns |         - |
| Double_Mul      |    673.4 ns |  0.40 ns |  0.37 ns |         - |
| Fixed_Div       |  4,026.4 ns |  3.46 ns |  3.23 ns |         - |
| Double_Div      |    674.7 ns |  1.72 ns |  1.61 ns |         - |
| Fixed_Mod       |    792.8 ns |  1.46 ns |  1.29 ns |         - |
| Double_Mod      |  3,792.0 ns | 33.95 ns | 31.76 ns |         - |
|                 |             |          |          |           |
| Fixed_Exp       |  6,419.3 ns |  5.53 ns |  4.62 ns |         - |
| Double_Exp      |  2,965.6 ns | 34.71 ns | 32.46 ns |         - |
| Fixed_Log       |  4,211.3 ns |  8.18 ns |  6.39 ns |         - |
| Double_Log      |  3,216.5 ns |  7.02 ns |  6.56 ns |         - |
|                 |             |          |          |           |
| Fixed_Pow       | 11,801.1 ns | 40.96 ns | 38.31 ns |         - |
| Double_Pow      |  9,325.9 ns | 24.66 ns | 23.06 ns |         - |
|                 |             |          |          |           |
| Fixed_Sqrt      |  2,011.1 ns |  3.32 ns |  2.77 ns |         - |
| Double_Sqrt     |    626.7 ns |  1.40 ns |  1.31 ns |         - |
| Fixed_Cbrt      |  3,191.3 ns |  3.40 ns |  3.02 ns |         - |
| Double_Cbrt     |  2,525.2 ns | 21.14 ns | 19.77 ns |         - |
|                 |             |          |          |           |
| Fixed_Floor     |    537.5 ns |  1.10 ns |  0.97 ns |         - |
| Double_Floor    |    396.3 ns |  0.72 ns |  0.60 ns |         - |
| Fixed_Ceiling   |    551.8 ns |  0.44 ns |  0.39 ns |         - |
| Double_Ceiling  |    396.0 ns |  0.45 ns |  0.42 ns |         - |
| Fixed_Truncate  |    910.8 ns |  2.03 ns |  1.80 ns |         - |
| Double_Truncate |    396.7 ns |  0.90 ns |  0.85 ns |         - |
| Fixed_Round     |    956.1 ns |  6.05 ns |  4.72 ns |         - |
| Double_Round    |    396.5 ns |  0.45 ns |  0.38 ns |         - |
|                 |             |          |          |           |
| Fixed_Sin       |  2,274.0 ns |  3.83 ns |  3.39 ns |         - |
| Double_Sin      |  3,775.5 ns | 26.79 ns | 25.06 ns |         - |
| Fixed_Cos       |  2,267.8 ns |  4.63 ns |  4.10 ns |         - |
| Double_Cos      |  3,728.9 ns | 15.84 ns | 14.82 ns |         - |
| Fixed_Tan       |  4,051.8 ns |  4.12 ns |  3.65 ns |         - |
| Double_Tan      |  4,260.1 ns |  3.09 ns |  2.41 ns |         - |
| Fixed_SinCos    |  2,286.9 ns |  5.25 ns |  4.91 ns |         - |
| Double_SinCos   |  8,468.0 ns | 63.70 ns | 59.58 ns |         - |
| Fixed_Atan      |  4,258.4 ns |  6.30 ns |  5.90 ns |         - |
| Double_Atan     |  4,722.3 ns | 14.04 ns | 12.45 ns |         - |
| Fixed_Atan2     |  5,552.6 ns |  7.78 ns |  7.28 ns |         - |
| Double_Atan2    |  6,303.4 ns | 60.58 ns | 50.58 ns |         - |

// * Hints *
Outliers
  FixedBenchmarks.Fixed_Add: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (642.03 ns)
  FixedBenchmarks.Fixed_Sub: MinIterationTime=500ms, IterationCount=15      -> 2 outliers were removed (733.16 ns, 744.67 ns)
  FixedBenchmarks.Double_Sub: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  detected (901.08 ns)
  FixedBenchmarks.Fixed_Mul: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (907.55 ns)
  FixedBenchmarks.Fixed_Mod: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed, 2 outliers were detected (792.54 ns, 798.19 ns)
  FixedBenchmarks.Fixed_Exp: MinIterationTime=500ms, IterationCount=15      -> 2 outliers were removed (6.44 us, 6.46 us)
  FixedBenchmarks.Fixed_Log: MinIterationTime=500ms, IterationCount=15      -> 3 outliers were removed (4.26 us..4.32 us)
  FixedBenchmarks.Fixed_Sqrt: MinIterationTime=500ms, IterationCount=15     -> 2 outliers were removed (2.03 us, 2.03 us)
  FixedBenchmarks.Fixed_Cbrt: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (3.21 us)
  FixedBenchmarks.Fixed_Floor: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  removed (542.36 ns)
  FixedBenchmarks.Double_Floor: MinIterationTime=500ms, IterationCount=15   -> 2 outliers were removed (404.04 ns, 473.90 ns)
  FixedBenchmarks.Fixed_Ceiling: MinIterationTime=500ms, IterationCount=15  -> 1 outlier  was  removed (555.47 ns)
  FixedBenchmarks.Fixed_Truncate: MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  removed (919.00 ns)
  FixedBenchmarks.Fixed_Round: MinIterationTime=500ms, IterationCount=15    -> 3 outliers were removed (983.44 ns..1.13 us)
  FixedBenchmarks.Double_Round: MinIterationTime=500ms, IterationCount=15   -> 2 outliers were removed (400.11 ns, 400.42 ns)
  FixedBenchmarks.Fixed_Sin: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (2.29 us)
  FixedBenchmarks.Fixed_Cos: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (2.30 us)
  FixedBenchmarks.Fixed_Tan: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (4.07 us)
  FixedBenchmarks.Double_Tan: MinIterationTime=500ms, IterationCount=15     -> 3 outliers were removed (4.28 us..4.36 us)
  FixedBenchmarks.Double_Atan: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  removed (4.77 us)
  FixedBenchmarks.Double_Atan2: MinIterationTime=500ms, IterationCount=15   -> 2 outliers were removed (6.47 us, 6.49 us)

// * Legends *
  Mean      : Arithmetic mean of all measurements
  Error     : Half of 99.9% confidence interval
  StdDev    : Standard deviation of all measurements
  Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 ns      : 1 Nanosecond (0.000000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
Run time: 00:12:57 (777.02 sec), executed benchmarks: 40

Global total time: 00:13:03 (783.49 sec), executed benchmarks: 40
// * Artifacts cleanup *
Artifacts cleanup is finished

Process finished with exit code 0.

*/