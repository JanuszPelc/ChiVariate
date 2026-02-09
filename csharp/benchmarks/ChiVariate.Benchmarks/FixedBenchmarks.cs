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