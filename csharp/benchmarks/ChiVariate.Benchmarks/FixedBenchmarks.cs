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
  Job-BHHXHU : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD

MinIterationTime=500ms  IterationCount=15  

| Method          | Mean        | Error     | StdDev    | Allocated |
|---------------- |------------:|----------:|----------:|----------:|
| Fixed_Add       |    638.4 ns |   0.66 ns |   0.55 ns |         - |
| Double_Add      |    895.3 ns |   1.37 ns |   1.28 ns |         - |
| Fixed_Sub       |    714.5 ns |   1.54 ns |   1.44 ns |         - |
| Double_Sub      |    894.4 ns |   1.24 ns |   1.16 ns |         - |
| Fixed_Mul       |    894.2 ns |   1.09 ns |   1.02 ns |         - |
| Double_Mul      |    668.2 ns |   1.36 ns |   1.27 ns |         - |
| Fixed_Div       |  3,697.3 ns |   5.90 ns |   5.23 ns |         - |
| Double_Div      |    668.1 ns |   1.26 ns |   1.18 ns |         - |
| Fixed_Mod       |    786.2 ns |   1.07 ns |   1.00 ns |         - |
| Double_Mod      |  3,750.0 ns |  21.76 ns |  20.35 ns |         - |
|                 |             |           |           |           |
| Fixed_Exp       |  7,099.2 ns |  11.41 ns |  10.67 ns |         - |
| Double_Exp      |  2,914.4 ns |  19.63 ns |  18.36 ns |         - |
| Fixed_Log       |  4,184.4 ns |  11.95 ns |  11.18 ns |         - |
| Double_Log      |  3,179.6 ns |   5.12 ns |   4.28 ns |         - |
|                 |             |           |           |           |
| Fixed_Pow       | 11,807.3 ns |  34.71 ns |  32.47 ns |         - |
| Double_Pow      |  8,892.1 ns |  31.24 ns |  29.22 ns |         - |
|                 |             |           |           |           |
| Fixed_Sqrt      |  1,998.1 ns |   3.84 ns |   3.59 ns |         - |
| Double_Sqrt     |    620.9 ns |   1.26 ns |   1.18 ns |         - |
| Fixed_Cbrt      |  3,169.1 ns |   5.73 ns |   5.36 ns |         - |
| Double_Cbrt     |  2,499.1 ns |   4.45 ns |   4.16 ns |         - |
|                 |             |           |           |           |
| Fixed_Floor     |    534.8 ns |   1.34 ns |   1.12 ns |         - |
| Double_Floor    |    395.3 ns |   0.77 ns |   0.72 ns |         - |
| Fixed_Ceiling   |    550.9 ns |   1.03 ns |   0.97 ns |         - |
| Double_Ceiling  |    395.3 ns |   0.64 ns |   0.60 ns |         - |
| Fixed_Truncate  |    921.2 ns |  20.81 ns |  17.38 ns |         - |
| Double_Truncate |    395.3 ns |   0.66 ns |   0.61 ns |         - |
| Fixed_Round     |    950.5 ns |   1.71 ns |   1.60 ns |         - |
| Double_Round    |    395.7 ns |   0.95 ns |   0.84 ns |         - |
|                 |             |           |           |           |
| Fixed_Sin       |  2,263.9 ns |   5.06 ns |   4.73 ns |         - |
| Double_Sin      |  3,661.5 ns |  38.51 ns |  36.03 ns |         - |
| Fixed_Cos       |  2,275.3 ns |  21.08 ns |  19.72 ns |         - |
| Double_Cos      |  3,622.6 ns |  23.81 ns |  22.27 ns |         - |
| Fixed_Tan       |  4,387.9 ns |  11.58 ns |  10.84 ns |         - |
| Double_Tan      |  4,255.2 ns |  11.51 ns |  10.77 ns |         - |
| Fixed_SinCos    |  2,299.3 ns |  20.97 ns |  19.61 ns |         - |
| Double_SinCos   |  8,309.5 ns |  15.30 ns |  13.57 ns |         - |
| Fixed_Atan      |  4,867.3 ns |   9.81 ns |   9.18 ns |         - |
| Double_Atan     |  4,722.7 ns |  14.44 ns |  12.80 ns |         - |
| Fixed_Atan2     |  5,163.6 ns |  12.49 ns |  11.69 ns |         - |
| Double_Atan2    |  6,422.2 ns | 125.10 ns | 117.02 ns |         - |

// * Hints *
Outliers
  FixedBenchmarks.Fixed_Add: MinIterationTime=500ms, IterationCount=15      -> 2 outliers were removed (642.84 ns, 657.38 ns)
  FixedBenchmarks.Fixed_Div: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (3.71 μs)
  FixedBenchmarks.Double_Log: MinIterationTime=500ms, IterationCount=15     -> 2 outliers were removed (3.20 μs, 3.22 μs)
  FixedBenchmarks.Fixed_Floor: MinIterationTime=500ms, IterationCount=15    -> 2 outliers were removed (542.30 ns, 549.42 ns)
  FixedBenchmarks.Fixed_Truncate: MinIterationTime=500ms, IterationCount=15 -> 2 outliers were removed (1.04 μs, 1.14 μs)
  FixedBenchmarks.Double_Round: MinIterationTime=500ms, IterationCount=15   -> 1 outlier  was  removed (401.65 ns)
  FixedBenchmarks.Double_SinCos: MinIterationTime=500ms, IterationCount=15  -> 1 outlier  was  removed (8.36 μs)
  FixedBenchmarks.Double_Atan: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  removed (4.78 μs)

// * Legends *
  Mean      : Arithmetic mean of all measurements
  Error     : Half of 99.9% confidence interval
  StdDev    : Standard deviation of all measurements
  Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 ns      : 1 Nanosecond (0.000000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
Run time: 00:12:57 (777.01 sec), executed benchmarks: 40

*/