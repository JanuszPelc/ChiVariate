using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Benchmarks;

/// <summary>
///     Benchmarks for basic continuous distributions that use direct transforms of uniform samples.
///     Includes: Bernoulli, Categorical, Cauchy, Exponential, Gumbel, Laplace, Logistic,
///     Normal, Pareto, Rayleigh, Triangular, Uniform, Weibull.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
[MinIterationTime(500)]
[IterationCount(15)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams)]
public class SamplerBasicBenchmarks
{
    #region Benchmarks

    [Benchmark(Description = "Bernoulli")]
    public bool ChiVariateBernoulli()
    {
        return ValueTypeParam switch
        {
            ValueType.Float => RunBenchmark<float>(),
            ValueType.Double => RunBenchmark<double>(),
            ValueType.Decimal => RunBenchmark<decimal>(),
            ValueType.Fixed => RunBenchmark<ChiFixed>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : IFloatingPoint<T>
        {
            var sampler = _rng.Bernoulli(T.CreateChecked(0.5));
            var sum = 0;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                        sum += sampler.Sample();
                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var sample in sampler.Sample(SampleCount))
                        sum += sample;
                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount);
                    sum = SumValues(enumerable);
                    break;
                }
                default: throw new UnreachableException();
            }

            return Consume(sum);
        }
    }

    [Benchmark(Description = "Categorical")]
    public bool ChiVariateCategorical()
    {
        return ValueTypeParam switch
        {
            ValueType.Float => RunBenchmark<float>(),
            ValueType.Double => RunBenchmark<double>(),
            ValueType.Decimal => RunBenchmark<decimal>(),
            ValueType.Fixed => RunBenchmark<ChiFixed>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
        {
            var sampler = _rng.Categorical((ReadOnlySpan<T>)TypedWeight<T>.Data);
            var sum = 0L;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                        sum += sampler.Sample();
                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var sample in sampler.Sample(SampleCount))
                        sum += sample;
                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount);
                    sum = SumValues(enumerable);
                    break;
                }
                default: throw new UnreachableException();
            }

            return Consume(sum);
        }
    }

    [Benchmark(Description = "Cauchy")]
    public bool ChiVariateCauchy()
    {
        return ValueTypeParam switch
        {
            ValueType.Float => RunBenchmark<float>(),
            ValueType.Double => RunBenchmark<double>(),
            ValueType.Decimal => RunBenchmark<decimal>(),
            ValueType.Fixed => RunBenchmark<ChiFixed>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : IFloatingPoint<T>
        {
            var sampler = _rng.Cauchy(T.Zero, T.One);
            var sum = T.Zero;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                        sum += sampler.Sample();
                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var sample in sampler.Sample(SampleCount))
                        sum += sample;
                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount);
                    sum = SumValues(enumerable);
                    break;
                }
                default: throw new UnreachableException();
            }

            return Consume(sum);
        }
    }

    [Benchmark(Description = "Exponential")]
    public bool ChiVariateExponential()
    {
        return ValueTypeParam switch
        {
            ValueType.Float => RunBenchmark<float>(),
            ValueType.Double => RunBenchmark<double>(),
            ValueType.Decimal => RunBenchmark<decimal>(),
            ValueType.Fixed => RunBenchmark<ChiFixed>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : IFloatingPoint<T>
        {
            var sampler = _rng.Exponential(T.One);
            var sum = T.Zero;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                        sum += sampler.Sample();
                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var sample in sampler.Sample(SampleCount))
                        sum += sample;
                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount);
                    sum = SumValues(enumerable);
                    break;
                }
                default: throw new UnreachableException();
            }

            return Consume(sum);
        }
    }

    [Benchmark(Description = "Gumbel")]
    public bool ChiVariateGumbel()
    {
        return ValueTypeParam switch
        {
            ValueType.Float => RunBenchmark<float>(),
            ValueType.Double => RunBenchmark<double>(),
            ValueType.Decimal => RunBenchmark<decimal>(),
            ValueType.Fixed => RunBenchmark<ChiFixed>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : IFloatingPoint<T>
        {
            var sampler = _rng.Gumbel(T.Zero, T.One);
            var sum = T.Zero;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                        sum += sampler.Sample();
                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var sample in sampler.Sample(SampleCount))
                        sum += sample;
                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount);
                    sum = SumValues(enumerable);
                    break;
                }
                default: throw new UnreachableException();
            }

            return Consume(sum);
        }
    }

    [Benchmark(Description = "Laplace")]
    public bool ChiVariateLaplace()
    {
        return ValueTypeParam switch
        {
            ValueType.Float => RunBenchmark<float>(),
            ValueType.Double => RunBenchmark<double>(),
            ValueType.Decimal => RunBenchmark<decimal>(),
            ValueType.Fixed => RunBenchmark<ChiFixed>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : IFloatingPoint<T>
        {
            var sampler = _rng.Laplace(T.Zero, T.One);
            var sum = T.Zero;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                        sum += sampler.Sample();
                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var sample in sampler.Sample(SampleCount))
                        sum += sample;
                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount);
                    sum = SumValues(enumerable);
                    break;
                }
                default: throw new UnreachableException();
            }

            return Consume(sum);
        }
    }

    [Benchmark(Description = "Logistic")]
    public bool ChiVariateLogistic()
    {
        return ValueTypeParam switch
        {
            ValueType.Float => RunBenchmark<float>(),
            ValueType.Double => RunBenchmark<double>(),
            ValueType.Decimal => RunBenchmark<decimal>(),
            ValueType.Fixed => RunBenchmark<ChiFixed>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : IFloatingPoint<T>
        {
            var sampler = _rng.Logistic(T.Zero, T.One);
            var sum = T.Zero;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                        sum += sampler.Sample();
                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var sample in sampler.Sample(SampleCount))
                        sum += sample;
                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount);
                    sum = SumValues(enumerable);
                    break;
                }
                default: throw new UnreachableException();
            }

            return Consume(sum);
        }
    }

    [Benchmark(Description = "Normal")]
    public bool ChiVariateNormal()
    {
        return ValueTypeParam switch
        {
            ValueType.Float => RunBenchmark<float>(),
            ValueType.Double => RunBenchmark<double>(),
            ValueType.Decimal => RunBenchmark<decimal>(),
            ValueType.Fixed => RunBenchmark<ChiFixed>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : IFloatingPoint<T>
        {
            var sampler = _rng.Normal(T.Zero, T.One);
            var sum = T.Zero;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                        sum += sampler.Sample();
                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var sample in sampler.Sample(SampleCount))
                        sum += sample;
                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount);
                    sum = SumValues(enumerable);
                    break;
                }
                default: throw new UnreachableException();
            }

            return Consume(sum);
        }
    }

    [Benchmark(Description = "Pareto")]
    public bool ChiVariatePareto()
    {
        return ValueTypeParam switch
        {
            ValueType.Float => RunBenchmark<float>(),
            ValueType.Double => RunBenchmark<double>(),
            ValueType.Decimal => RunBenchmark<decimal>(),
            ValueType.Fixed => RunBenchmark<ChiFixed>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : IFloatingPoint<T>
        {
            var sampler = _rng.Pareto(T.One, T.CreateChecked(1.16));
            var sum = T.Zero;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                        sum += sampler.Sample();
                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var sample in sampler.Sample(SampleCount))
                        sum += sample;
                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount);
                    sum = SumValues(enumerable);
                    break;
                }
                default: throw new UnreachableException();
            }

            return Consume(sum);
        }
    }

    [Benchmark(Description = "Rayleigh")]
    public bool ChiVariateRayleigh()
    {
        return ValueTypeParam switch
        {
            ValueType.Float => RunBenchmark<float>(),
            ValueType.Double => RunBenchmark<double>(),
            ValueType.Decimal => RunBenchmark<decimal>(),
            ValueType.Fixed => RunBenchmark<ChiFixed>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : IFloatingPoint<T>
        {
            var sampler = _rng.Rayleigh(T.One);
            var sum = T.Zero;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                        sum += sampler.Sample();
                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var sample in sampler.Sample(SampleCount))
                        sum += sample;
                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount);
                    sum = SumValues(enumerable);
                    break;
                }
                default: throw new UnreachableException();
            }

            return Consume(sum);
        }
    }

    [Benchmark(Description = "Triangular")]
    public bool ChiVariateTriangular()
    {
        return ValueTypeParam switch
        {
            ValueType.Float => RunBenchmark<float>(),
            ValueType.Double => RunBenchmark<double>(),
            ValueType.Decimal => RunBenchmark<decimal>(),
            ValueType.Fixed => RunBenchmark<ChiFixed>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : IFloatingPoint<T>
        {
            var sampler = _rng.Triangular(T.CreateChecked(-1.0), T.One, T.Zero);
            var sum = T.Zero;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                        sum += sampler.Sample();
                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var sample in sampler.Sample(SampleCount))
                        sum += sample;
                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount);
                    sum = SumValues(enumerable);
                    break;
                }
                default: throw new UnreachableException();
            }

            return Consume(sum);
        }
    }

    [Benchmark(Description = "Uniform", Baseline = true)]
    public bool ChiVariateUniform()
    {
        return ValueTypeParam switch
        {
            ValueType.Float => RunBenchmark<float>(),
            ValueType.Double => RunBenchmark<double>(),
            ValueType.Decimal => RunBenchmark<decimal>(),
            ValueType.Fixed => RunBenchmark<ChiFixed>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : IFloatingPoint<T>
        {
            var sampler = _rng.Uniform(T.CreateChecked(-100.0), T.CreateChecked(100.0));
            var sum = T.Zero;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                        sum += sampler.Sample();
                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var sample in sampler.Sample(SampleCount))
                        sum += sample;
                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount);
                    sum = SumValues(enumerable);
                    break;
                }
                default: throw new UnreachableException();
            }

            return Consume(sum);
        }
    }

    [Benchmark(Description = "Weibull")]
    public bool ChiVariateWeibull()
    {
        return ValueTypeParam switch
        {
            ValueType.Float => RunBenchmark<float>(),
            ValueType.Double => RunBenchmark<double>(),
            ValueType.Decimal => RunBenchmark<decimal>(),
            ValueType.Fixed => RunBenchmark<ChiFixed>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : IFloatingPoint<T>
        {
            var sampler = _rng.Weibull(T.CreateChecked(2.0), T.One);
            var sum = T.Zero;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                        sum += sampler.Sample();
                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var sample in sampler.Sample(SampleCount))
                        sum += sample;
                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount);
                    sum = SumValues(enumerable);
                    break;
                }
                default: throw new UnreachableException();
            }

            return Consume(sum);
        }
    }

    #endregion

    #region Enums and Parameters

    private enum IterationType
    {
        ForLoop = 1 << 16,
        ForEach = 2 << 16,
        Linq = 3 << 16,
        BitMask = 0xFFFF << 16
    }

    private enum ValueType
    {
        Float,
        Double,
        Decimal,
        Fixed,
        BitMask = 0xFFFF
    }

    public enum ParamType
    {
        // ReSharper disable InconsistentNaming
        ForLoop_Float = IterationType.ForLoop | ValueType.Float,
        ForLoop_Double = IterationType.ForLoop | ValueType.Double,
        ForLoop_Decimal = IterationType.ForLoop | ValueType.Decimal,
        ForLoop_Fixed = IterationType.ForLoop | ValueType.Fixed,
        ForEach_Float = IterationType.ForEach | ValueType.Float,
        ForEach_Double = IterationType.ForEach | ValueType.Double,
        ForEach_Decimal = IterationType.ForEach | ValueType.Decimal,
        ForEach_Fixed = IterationType.ForEach | ValueType.Fixed,
        Linq_Float = IterationType.Linq | ValueType.Float,
        Linq_Double = IterationType.Linq | ValueType.Double,
        Linq_Decimal = IterationType.Linq | ValueType.Decimal,

        Linq_Fixed = IterationType.Linq | ValueType.Fixed
        // ReSharper restore InconsistentNaming
    }

    [Params(
        ParamType.ForLoop_Float, ParamType.ForLoop_Double, ParamType.ForLoop_Decimal, ParamType.ForLoop_Fixed,
        ParamType.ForEach_Float, ParamType.ForEach_Double, ParamType.ForEach_Decimal, ParamType.ForEach_Fixed,
        ParamType.Linq_Float, ParamType.Linq_Double, ParamType.Linq_Decimal, ParamType.Linq_Fixed)]
    public ParamType Param { get; set; }

    private IterationType IterationTypeParam => (IterationType)((int)Param & (int)IterationType.BitMask);

    private ValueType ValueTypeParam => (ValueType)((int)Param & (int)ValueType.BitMask);

    #endregion

    #region Fields and Setup

    private const int SampleCount = 1_000;

    private ChiRng _rng;

    [GlobalSetup]
    public void Setup()
    {
        _rng = new ChiRng("benchmark-seed");
    }

    #endregion

    #region Helper & Consume Methods

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T SumValues<T>(ChiEnumerable<T> enumerable)
        where T : INumberBase<T>
    {
        if (typeof(T) == typeof(float))
            return T.CreateTruncating(enumerable.Sum(float.CreateTruncating));

        if (typeof(T) == typeof(double))
            return T.CreateTruncating(enumerable.Sum(double.CreateTruncating));

        if (typeof(T) == typeof(decimal))
            return T.CreateTruncating(enumerable.Sum(decimal.CreateTruncating));

        if (typeof(T) == typeof(ChiFixed))
            return enumerable.Aggregate(T.Zero, (sum, value) => sum + value);

        if (typeof(T) == typeof(int))
            return T.CreateTruncating(enumerable.Sum(int.CreateTruncating));

        if (typeof(T) == typeof(long))
            return T.CreateTruncating(enumerable.Sum(long.CreateTruncating));

        throw new UnreachableException();
    }

    private static bool Consume<T>(T value) where T : INumberBase<T>
    {
        if (value.GetHashCode() == Environment.TickCount)
            Console.WriteLine(value);
        return true;
    }

    private static class TypedWeight<T>
        where T : unmanaged, IFloatingPoint<T>
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly double[] Template = [0.05, 0.1, 0.15, 0.2, 0.25, 0.15, 0.1];

        public static T[] Data { get; } = Initialize();

        private static T[] Initialize()
        {
            var data = new T[Template.Length];
            for (var i = 0; i < Template.Length; i++)
                data[i] = T.CreateChecked(Template[i]);
            return data;
        }
    }

    #endregion
}

/*

// * Summary *

BenchmarkDotNet v0.14.0, macOS Sequoia 15.7.3 (24G419) [Darwin 24.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD
  Job-AMWZTP : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD

MinIterationTime=500ms  IterationCount=15

| Method      | Param           | Mean        | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------ |---------------- |------------:|----------:|----------:|------:|--------:|----------:|------------:|
| Bernoulli   | ForLoop_Float   |    15.87 μs |  0.014 μs |  0.012 μs |  1.04 |    0.00 |         - |          NA |
| Categorical | ForLoop_Float   |    44.18 μs |  0.029 μs |  0.027 μs |  2.89 |    0.00 |         - |          NA |
| Cauchy      | ForLoop_Float   |    38.42 μs |  0.028 μs |  0.026 μs |  2.51 |    0.00 |         - |          NA |
| Exponential | ForLoop_Float   |    47.46 μs |  0.027 μs |  0.024 μs |  3.10 |    0.00 |         - |          NA |
| Gumbel      | ForLoop_Float   |    51.18 μs |  0.018 μs |  0.016 μs |  3.34 |    0.00 |         - |          NA |
| Laplace     | ForLoop_Float   |    93.91 μs |  0.067 μs |  0.062 μs |  6.14 |    0.01 |         - |          NA |
| Logistic    | ForLoop_Float   |    18.72 μs |  0.010 μs |  0.008 μs |  1.22 |    0.00 |         - |          NA |
| Normal      | ForLoop_Float   |    54.22 μs |  0.018 μs |  0.016 μs |  3.54 |    0.00 |         - |          NA |
| Pareto      | ForLoop_Float   |    20.87 μs |  0.022 μs |  0.021 μs |  1.36 |    0.00 |         - |          NA |
| Rayleigh    | ForLoop_Float   |    47.77 μs |  0.022 μs |  0.018 μs |  3.12 |    0.00 |         - |          NA |
| Triangular  | ForLoop_Float   |    34.74 μs |  0.025 μs |  0.022 μs |  2.27 |    0.00 |         - |          NA |
| Uniform     | ForLoop_Float   |    15.30 μs |  0.009 μs |  0.008 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | ForLoop_Float   |    52.69 μs |  0.026 μs |  0.024 μs |  3.44 |    0.00 |         - |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | ForLoop_Double  |    31.67 μs |  0.015 μs |  0.013 μs |  1.02 |    0.00 |         - |          NA |
| Categorical | ForLoop_Double  |    58.81 μs |  0.032 μs |  0.028 μs |  1.89 |    0.00 |         - |          NA |
| Cauchy      | ForLoop_Double  |    52.82 μs |  0.015 μs |  0.014 μs |  1.70 |    0.00 |         - |          NA |
| Exponential | ForLoop_Double  |    63.00 μs |  0.049 μs |  0.046 μs |  2.03 |    0.00 |         - |          NA |
| Gumbel      | ForLoop_Double  |    76.58 μs |  0.030 μs |  0.028 μs |  2.46 |    0.00 |         - |          NA |
| Laplace     | ForLoop_Double  |   110.42 μs |  0.034 μs |  0.032 μs |  3.55 |    0.00 |         - |          NA |
| Logistic    | ForLoop_Double  |    41.25 μs |  0.026 μs |  0.024 μs |  1.33 |    0.00 |         - |          NA |
| Normal      | ForLoop_Double  |    70.21 μs |  0.028 μs |  0.025 μs |  2.26 |    0.00 |         - |          NA |
| Pareto      | ForLoop_Double  |    40.42 μs |  0.021 μs |  0.018 μs |  1.30 |    0.00 |         - |          NA |
| Rayleigh    | ForLoop_Double  |    63.47 μs |  0.055 μs |  0.052 μs |  2.04 |    0.00 |         - |          NA |
| Triangular  | ForLoop_Double  |    48.81 μs |  0.019 μs |  0.018 μs |  1.57 |    0.00 |         - |          NA |
| Uniform     | ForLoop_Double  |    31.07 μs |  0.020 μs |  0.019 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | ForLoop_Double  |    74.35 μs |  0.035 μs |  0.030 μs |  2.39 |    0.00 |         - |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | ForLoop_Decimal |   149.25 μs |  0.132 μs |  0.117 μs |  0.65 |    0.00 |         - |          NA |
| Categorical | ForLoop_Decimal |   173.38 μs |  0.075 μs |  0.066 μs |  0.75 |    0.00 |         - |          NA |
| Cauchy      | ForLoop_Decimal | 2,996.56 μs |  2.648 μs |  2.067 μs | 12.97 |    0.01 |       3 B |          NA |
| Exponential | ForLoop_Decimal |   308.69 μs |  0.151 μs |  0.134 μs |  1.34 |    0.00 |         - |          NA |
| Gumbel      | ForLoop_Decimal | 4,488.48 μs |  6.933 μs |  6.485 μs | 19.42 |    0.03 |       6 B |          NA |
| Laplace     | ForLoop_Decimal |   337.94 μs |  0.137 μs |  0.129 μs |  1.46 |    0.00 |         - |          NA |
| Logistic    | ForLoop_Decimal | 4,608.49 μs |  6.828 μs |  6.387 μs | 19.94 |    0.03 |       6 B |          NA |
| Normal      | ForLoop_Decimal |   299.74 μs |  0.284 μs |  0.252 μs |  1.30 |    0.00 |         - |          NA |
| Pareto      | ForLoop_Decimal | 6,729.15 μs |  7.148 μs |  6.686 μs | 29.12 |    0.03 |       6 B |          NA |
| Rayleigh    | ForLoop_Decimal | 1,315.72 μs |  0.789 μs |  0.738 μs |  5.69 |    0.00 |       1 B |          NA |
| Triangular  | ForLoop_Decimal | 1,336.77 μs |  0.750 μs |  0.665 μs |  5.78 |    0.00 |       1 B |          NA |
| Uniform     | ForLoop_Decimal |   231.10 μs |  0.105 μs |  0.088 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | ForLoop_Decimal | 6,482.81 μs |  9.567 μs |  8.949 μs | 28.05 |    0.04 |       6 B |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | ForLoop_Fixed   |    16.10 μs |  0.011 μs |  0.009 μs |  0.82 |    0.00 |         - |          NA |
| Categorical | ForLoop_Fixed   |    41.23 μs |  0.036 μs |  0.034 μs |  2.10 |    0.00 |         - |          NA |
| Cauchy      | ForLoop_Fixed   |    42.47 μs |  0.019 μs |  0.018 μs |  2.17 |    0.00 |         - |          NA |
| Exponential | ForLoop_Fixed   |    64.32 μs |  0.050 μs |  0.047 μs |  3.28 |    0.00 |         - |          NA |
| Gumbel      | ForLoop_Fixed   |    78.24 μs |  0.042 μs |  0.035 μs |  3.99 |    0.00 |         - |          NA |
| Laplace     | ForLoop_Fixed   |    99.00 μs |  0.088 μs |  0.083 μs |  5.05 |    0.00 |         - |          NA |
| Logistic    | ForLoop_Fixed   |    56.26 μs |  0.026 μs |  0.024 μs |  2.87 |    0.00 |         - |          NA |
| Normal      | ForLoop_Fixed   |    62.18 μs |  0.039 μs |  0.037 μs |  3.17 |    0.00 |         - |          NA |
| Pareto      | ForLoop_Fixed   |    65.54 μs |  0.064 μs |  0.060 μs |  3.35 |    0.00 |         - |          NA |
| Rayleigh    | ForLoop_Fixed   |    74.70 μs |  0.035 μs |  0.031 μs |  3.81 |    0.00 |         - |          NA |
| Triangular  | ForLoop_Fixed   |    44.90 μs |  0.058 μs |  0.052 μs |  2.29 |    0.00 |         - |          NA |
| Uniform     | ForLoop_Fixed   |    19.59 μs |  0.007 μs |  0.006 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | ForLoop_Fixed   |    89.69 μs |  0.057 μs |  0.051 μs |  4.58 |    0.00 |         - |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | ForEach_Float   |    16.27 μs |  0.013 μs |  0.012 μs |  0.86 |    0.00 |         - |          NA |
| Categorical | ForEach_Float   |    50.12 μs |  0.074 μs |  0.069 μs |  2.66 |    0.00 |         - |          NA |
| Cauchy      | ForEach_Float   |    43.16 μs |  0.020 μs |  0.019 μs |  2.29 |    0.00 |         - |          NA |
| Exponential | ForEach_Float   |    51.06 μs |  0.023 μs |  0.020 μs |  2.71 |    0.00 |         - |          NA |
| Gumbel      | ForEach_Float   |    54.86 μs |  0.024 μs |  0.022 μs |  2.91 |    0.00 |         - |          NA |
| Laplace     | ForEach_Float   |   102.04 μs |  0.050 μs |  0.047 μs |  5.42 |    0.00 |         - |          NA |
| Logistic    | ForEach_Float   |    23.08 μs |  0.012 μs |  0.011 μs |  1.23 |    0.00 |         - |          NA |
| Normal      | ForEach_Float   |    62.38 μs |  0.050 μs |  0.046 μs |  3.31 |    0.00 |         - |          NA |
| Pareto      | ForEach_Float   |    25.00 μs |  0.007 μs |  0.007 μs |  1.33 |    0.00 |         - |          NA |
| Rayleigh    | ForEach_Float   |    51.38 μs |  0.021 μs |  0.020 μs |  2.73 |    0.00 |         - |          NA |
| Triangular  | ForEach_Float   |    39.09 μs |  0.013 μs |  0.011 μs |  2.08 |    0.00 |         - |          NA |
| Uniform     | ForEach_Float   |    18.83 μs |  0.009 μs |  0.008 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | ForEach_Float   |    56.56 μs |  0.030 μs |  0.027 μs |  3.00 |    0.00 |         - |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | ForEach_Double  |    31.99 μs |  0.032 μs |  0.028 μs |  0.92 |    0.00 |         - |          NA |
| Categorical | ForEach_Double  |    66.97 μs |  0.066 μs |  0.059 μs |  1.93 |    0.00 |         - |          NA |
| Cauchy      | ForEach_Double  |    56.93 μs |  0.020 μs |  0.018 μs |  1.64 |    0.00 |         - |          NA |
| Exponential | ForEach_Double  |    71.79 μs |  0.052 μs |  0.046 μs |  2.07 |    0.00 |         - |          NA |
| Gumbel      | ForEach_Double  |    86.48 μs |  0.044 μs |  0.037 μs |  2.49 |    0.00 |         - |          NA |
| Laplace     | ForEach_Double  |   121.05 μs |  0.047 μs |  0.041 μs |  3.48 |    0.00 |         - |          NA |
| Logistic    | ForEach_Double  |    45.11 μs |  0.039 μs |  0.035 μs |  1.30 |    0.00 |         - |          NA |
| Normal      | ForEach_Double  |    71.70 μs |  0.037 μs |  0.035 μs |  2.06 |    0.00 |         - |          NA |
| Pareto      | ForEach_Double  |    44.65 μs |  0.021 μs |  0.020 μs |  1.29 |    0.00 |         - |          NA |
| Rayleigh    | ForEach_Double  |    73.28 μs |  0.043 μs |  0.040 μs |  2.11 |    0.00 |         - |          NA |
| Triangular  | ForEach_Double  |    52.97 μs |  0.025 μs |  0.022 μs |  1.52 |    0.00 |         - |          NA |
| Uniform     | ForEach_Double  |    34.74 μs |  0.014 μs |  0.012 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | ForEach_Double  |    82.61 μs |  0.034 μs |  0.031 μs |  2.38 |    0.00 |         - |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | ForEach_Decimal |   155.67 μs |  0.119 μs |  0.106 μs |  0.60 |    0.00 |         - |          NA |
| Categorical | ForEach_Decimal |   185.14 μs |  0.073 μs |  0.068 μs |  0.72 |    0.00 |         - |          NA |
| Cauchy      | ForEach_Decimal | 3,019.57 μs |  2.148 μs |  1.904 μs | 11.71 |    0.01 |       3 B |          NA |
| Exponential | ForEach_Decimal |   311.65 μs |  0.217 μs |  0.192 μs |  1.21 |    0.00 |         - |          NA |
| Gumbel      | ForEach_Decimal | 4,497.48 μs |  7.483 μs |  6.634 μs | 17.43 |    0.03 |       6 B |          NA |
| Laplace     | ForEach_Decimal |   336.70 μs |  0.221 μs |  0.206 μs |  1.31 |    0.00 |         - |          NA |
| Logistic    | ForEach_Decimal | 4,543.77 μs |  5.963 μs |  5.286 μs | 17.61 |    0.02 |       6 B |          NA |
| Normal      | ForEach_Decimal |   299.92 μs |  0.303 μs |  0.269 μs |  1.16 |    0.00 |         - |          NA |
| Pareto      | ForEach_Decimal | 6,751.04 μs | 95.914 μs | 85.025 μs | 26.17 |    0.32 |       6 B |          NA |
| Rayleigh    | ForEach_Decimal | 1,320.98 μs |  0.852 μs |  0.797 μs |  5.12 |    0.00 |       1 B |          NA |
| Triangular  | ForEach_Decimal | 1,332.52 μs |  0.804 μs |  0.713 μs |  5.17 |    0.00 |       1 B |          NA |
| Uniform     | ForEach_Decimal |   257.97 μs |  0.150 μs |  0.133 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | ForEach_Decimal | 6,527.93 μs |  7.813 μs |  7.309 μs | 25.31 |    0.03 |       6 B |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | ForEach_Fixed   |    16.51 μs |  0.008 μs |  0.008 μs |  0.82 |    0.00 |         - |          NA |
| Categorical | ForEach_Fixed   |    54.49 μs |  0.062 μs |  0.051 μs |  2.70 |    0.00 |         - |          NA |
| Cauchy      | ForEach_Fixed   |    43.54 μs |  0.028 μs |  0.027 μs |  2.16 |    0.00 |         - |          NA |
| Exponential | ForEach_Fixed   |    74.70 μs |  0.128 μs |  0.120 μs |  3.70 |    0.01 |         - |          NA |
| Gumbel      | ForEach_Fixed   |    91.01 μs |  0.060 μs |  0.053 μs |  4.51 |    0.00 |         - |          NA |
| Laplace     | ForEach_Fixed   |   101.76 μs |  0.123 μs |  0.115 μs |  5.05 |    0.01 |         - |          NA |
| Logistic    | ForEach_Fixed   |    57.83 μs |  0.040 μs |  0.035 μs |  2.87 |    0.00 |         - |          NA |
| Normal      | ForEach_Fixed   |    64.61 μs |  0.052 μs |  0.048 μs |  3.20 |    0.00 |         - |          NA |
| Pareto      | ForEach_Fixed   |    66.93 μs |  0.053 μs |  0.049 μs |  3.32 |    0.00 |         - |          NA |
| Rayleigh    | ForEach_Fixed   |    81.20 μs |  0.052 μs |  0.049 μs |  4.03 |    0.00 |         - |          NA |
| Triangular  | ForEach_Fixed   |    48.77 μs |  0.075 μs |  0.063 μs |  2.42 |    0.00 |         - |          NA |
| Uniform     | ForEach_Fixed   |    20.16 μs |  0.008 μs |  0.008 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | ForEach_Fixed   |    98.71 μs |  0.077 μs |  0.072 μs |  4.90 |    0.00 |         - |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | Linq_Float      |    17.78 μs |  0.011 μs |  0.009 μs |  0.94 |    0.00 |         - |          NA |
| Categorical | Linq_Float      |    45.70 μs |  0.035 μs |  0.033 μs |  2.42 |    0.00 |         - |          NA |
| Cauchy      | Linq_Float      |    42.67 μs |  0.022 μs |  0.019 μs |  2.26 |    0.00 |         - |          NA |
| Exponential | Linq_Float      |    55.71 μs |  0.022 μs |  0.019 μs |  2.95 |    0.00 |         - |          NA |
| Gumbel      | Linq_Float      |    58.81 μs |  0.017 μs |  0.015 μs |  3.11 |    0.00 |         - |          NA |
| Laplace     | Linq_Float      |    97.39 μs |  0.072 μs |  0.064 μs |  5.16 |    0.01 |         - |          NA |
| Logistic    | Linq_Float      |    23.24 μs |  0.015 μs |  0.013 μs |  1.23 |    0.00 |         - |          NA |
| Normal      | Linq_Float      |    58.29 μs |  0.017 μs |  0.015 μs |  3.09 |    0.00 |         - |          NA |
| Pareto      | Linq_Float      |    25.08 μs |  0.015 μs |  0.013 μs |  1.33 |    0.00 |         - |          NA |
| Rayleigh    | Linq_Float      |    54.88 μs |  0.021 μs |  0.018 μs |  2.91 |    0.00 |         - |          NA |
| Triangular  | Linq_Float      |    38.92 μs |  0.062 μs |  0.049 μs |  2.06 |    0.00 |         - |          NA |
| Uniform     | Linq_Float      |    18.88 μs |  0.019 μs |  0.017 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | Linq_Float      |    62.03 μs |  0.040 μs |  0.035 μs |  3.29 |    0.00 |         - |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | Linq_Double     |    33.46 μs |  0.021 μs |  0.020 μs |  0.96 |    0.00 |         - |          NA |
| Categorical | Linq_Double     |    62.12 μs |  0.052 μs |  0.048 μs |  1.79 |    0.00 |         - |          NA |
| Cauchy      | Linq_Double     |    56.91 μs |  0.041 μs |  0.038 μs |  1.64 |    0.00 |         - |          NA |
| Exponential | Linq_Double     |    66.91 μs |  0.052 μs |  0.046 μs |  1.92 |    0.00 |         - |          NA |
| Gumbel      | Linq_Double     |    80.18 μs |  0.063 μs |  0.058 μs |  2.30 |    0.00 |         - |          NA |
| Laplace     | Linq_Double     |   112.95 μs |  0.022 μs |  0.020 μs |  3.25 |    0.00 |         - |          NA |
| Logistic    | Linq_Double     |    45.23 μs |  0.030 μs |  0.026 μs |  1.30 |    0.00 |         - |          NA |
| Normal      | Linq_Double     |    72.81 μs |  0.031 μs |  0.029 μs |  2.09 |    0.00 |         - |          NA |
| Pareto      | Linq_Double     |    44.76 μs |  0.041 μs |  0.037 μs |  1.29 |    0.00 |         - |          NA |
| Rayleigh    | Linq_Double     |    67.26 μs |  0.045 μs |  0.042 μs |  1.93 |    0.00 |         - |          NA |
| Triangular  | Linq_Double     |    58.54 μs |  0.022 μs |  0.020 μs |  1.68 |    0.00 |         - |          NA |
| Uniform     | Linq_Double     |    34.79 μs |  0.025 μs |  0.022 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | Linq_Double     |    77.65 μs |  0.043 μs |  0.040 μs |  2.23 |    0.00 |         - |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | Linq_Decimal    |   166.35 μs |  0.162 μs |  0.151 μs |  0.66 |    0.00 |         - |          NA |
| Categorical | Linq_Decimal    |   176.10 μs |  0.076 μs |  0.071 μs |  0.70 |    0.00 |         - |          NA |
| Cauchy      | Linq_Decimal    | 3,034.31 μs |  2.886 μs |  2.700 μs | 12.02 |    0.02 |       3 B |          NA |
| Exponential | Linq_Decimal    |   310.52 μs |  0.500 μs |  0.418 μs |  1.23 |    0.00 |         - |          NA |
| Gumbel      | Linq_Decimal    | 4,497.43 μs | 16.778 μs | 14.011 μs | 17.82 |    0.06 |       6 B |          NA |
| Laplace     | Linq_Decimal    |   338.83 μs |  6.044 μs |  5.047 μs |  1.34 |    0.02 |         - |          NA |
| Logistic    | Linq_Decimal    | 4,636.58 μs |  4.463 μs |  3.726 μs | 18.37 |    0.02 |       6 B |          NA |
| Normal      | Linq_Decimal    |   299.07 μs |  0.369 μs |  0.345 μs |  1.18 |    0.00 |         - |          NA |
| Pareto      | Linq_Decimal    | 6,800.58 μs |  7.687 μs |  7.191 μs | 26.94 |    0.04 |       6 B |          NA |
| Rayleigh    | Linq_Decimal    | 1,320.72 μs |  0.892 μs |  0.745 μs |  5.23 |    0.01 |       1 B |          NA |
| Triangular  | Linq_Decimal    | 1,361.74 μs |  1.064 μs |  0.943 μs |  5.39 |    0.01 |       1 B |          NA |
| Uniform     | Linq_Decimal    |   252.41 μs |  0.283 μs |  0.265 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | Linq_Decimal    | 6,529.19 μs |  8.155 μs |  7.628 μs | 25.87 |    0.04 |       6 B |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | Linq_Fixed      |    18.92 μs |  0.010 μs |  0.009 μs |  0.81 |    0.00 |         - |          NA |
| Categorical | Linq_Fixed      |    43.94 μs |  0.027 μs |  0.023 μs |  1.89 |    0.00 |         - |          NA |
| Cauchy      | Linq_Fixed      |    47.07 μs |  0.026 μs |  0.024 μs |  2.02 |    0.00 |         - |          NA |
| Exponential | Linq_Fixed      |    64.73 μs |  0.072 μs |  0.064 μs |  2.78 |    0.00 |         - |          NA |
| Gumbel      | Linq_Fixed      |    80.76 μs |  0.068 μs |  0.064 μs |  3.47 |    0.00 |         - |          NA |
| Laplace     | Linq_Fixed      |   100.47 μs |  0.056 μs |  0.053 μs |  4.32 |    0.00 |         - |          NA |
| Logistic    | Linq_Fixed      |    66.65 μs |  0.098 μs |  0.087 μs |  2.87 |    0.00 |         - |          NA |
| Normal      | Linq_Fixed      |    63.52 μs |  0.049 μs |  0.046 μs |  2.73 |    0.00 |         - |          NA |
| Pareto      | Linq_Fixed      |    71.67 μs |  0.085 μs |  0.079 μs |  3.08 |    0.00 |         - |          NA |
| Rayleigh    | Linq_Fixed      |    76.79 μs |  0.044 μs |  0.041 μs |  3.30 |    0.00 |         - |          NA |
| Triangular  | Linq_Fixed      |    46.96 μs |  0.040 μs |  0.034 μs |  2.02 |    0.00 |         - |          NA |
| Uniform     | Linq_Fixed      |    23.25 μs |  0.010 μs |  0.009 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | Linq_Fixed      |    91.89 μs |  0.052 μs |  0.048 μs |  3.95 |    0.00 |         - |          NA |

// * Hints *
Outliers
  SamplerBasicBenchmarks.Bernoulli: MinIterationTime=500ms, IterationCount=15   -> 1 outlier  was  removed (16.07 μs)
  SamplerBasicBenchmarks.Exponential: MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  removed (47.70 μs)
  SamplerBasicBenchmarks.Gumbel: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (51.24 μs)
  SamplerBasicBenchmarks.Logistic: MinIterationTime=500ms, IterationCount=15    -> 2 outliers were removed (18.80 μs, 18.91 μs)
  SamplerBasicBenchmarks.Normal: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed, 2 outliers were detected (54.18 μs, 54.28 μs)
  SamplerBasicBenchmarks.Pareto: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  detected (20.83 μs)
  SamplerBasicBenchmarks.Rayleigh: MinIterationTime=500ms, IterationCount=15    -> 2 outliers were removed (47.93 μs, 47.97 μs)
  SamplerBasicBenchmarks.Triangular: MinIterationTime=500ms, IterationCount=15  -> 1 outlier  was  removed (34.92 μs)
  SamplerBasicBenchmarks.Bernoulli: MinIterationTime=500ms, IterationCount=15   -> 1 outlier  was  removed (31.72 μs)
  SamplerBasicBenchmarks.Categorical: MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  removed (58.96 μs)
  SamplerBasicBenchmarks.Normal: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed, 2 outliers were detected (70.15 μs, 70.29 μs)
  SamplerBasicBenchmarks.Pareto: MinIterationTime=500ms, IterationCount=15      -> 2 outliers were removed (40.48 μs, 40.48 μs)
  SamplerBasicBenchmarks.Weibull: MinIterationTime=500ms, IterationCount=15     -> 2 outliers were removed (74.48 μs, 75.06 μs)
  SamplerBasicBenchmarks.Bernoulli: MinIterationTime=500ms, IterationCount=15   -> 1 outlier  was  removed (150.02 μs)
  SamplerBasicBenchmarks.Categorical: MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  removed (173.62 μs)
  SamplerBasicBenchmarks.Cauchy: MinIterationTime=500ms, IterationCount=15      -> 3 outliers were removed (3.01 ms..3.04 ms)
  SamplerBasicBenchmarks.Exponential: MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  removed, 2 outliers were detected (308.42 μs, 312.75 μs)
  SamplerBasicBenchmarks.Gumbel: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  detected (4.47 ms)
  SamplerBasicBenchmarks.Logistic: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  detected (4.59 ms)
  SamplerBasicBenchmarks.Normal: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (301.23 μs)
  SamplerBasicBenchmarks.Triangular: MinIterationTime=500ms, IterationCount=15  -> 1 outlier  was  removed (1.34 ms)
  SamplerBasicBenchmarks.Uniform: MinIterationTime=500ms, IterationCount=15     -> 2 outliers were removed (231.45 μs, 231.56 μs)
  SamplerBasicBenchmarks.Bernoulli: MinIterationTime=500ms, IterationCount=15   -> 2 outliers were removed (16.14 μs, 16.26 μs)
  SamplerBasicBenchmarks.Gumbel: MinIterationTime=500ms, IterationCount=15      -> 2 outliers were removed (78.38 μs, 78.46 μs)
  SamplerBasicBenchmarks.Laplace: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  detected (98.79 μs)
  SamplerBasicBenchmarks.Rayleigh: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  removed (75.15 μs)
  SamplerBasicBenchmarks.Triangular: MinIterationTime=500ms, IterationCount=15  -> 1 outlier  was  removed (45.08 μs)
  SamplerBasicBenchmarks.Weibull: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (89.97 μs)
  SamplerBasicBenchmarks.Exponential: MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  removed, 2 outliers were detected (51.01 μs, 51.12 μs)
  SamplerBasicBenchmarks.Gumbel: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  detected (54.81 μs)
  SamplerBasicBenchmarks.Logistic: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  removed, 2 outliers were detected (23.06 μs, 23.25 μs)
  SamplerBasicBenchmarks.Triangular: MinIterationTime=500ms, IterationCount=15  -> 2 outliers were removed, 3 outliers were detected (39.07 μs, 39.13 μs, 39.72 μs)
  SamplerBasicBenchmarks.Uniform: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (18.85 μs)
  SamplerBasicBenchmarks.Weibull: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (56.76 μs)
  SamplerBasicBenchmarks.Bernoulli: MinIterationTime=500ms, IterationCount=15   -> 1 outlier  was  removed (32.33 μs)
  SamplerBasicBenchmarks.Categorical: MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  removed, 3 outliers were detected (66.85 μs, 66.85 μs, 67.18 μs)
  SamplerBasicBenchmarks.Exponential: MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  removed (72.20 μs)
  SamplerBasicBenchmarks.Gumbel: MinIterationTime=500ms, IterationCount=15      -> 2 outliers were removed (86.62 μs, 87.33 μs)
  SamplerBasicBenchmarks.Laplace: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (121.18 μs)
  SamplerBasicBenchmarks.Logistic: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  removed, 2 outliers were detected (45.03 μs, 45.33 μs)
  SamplerBasicBenchmarks.Triangular: MinIterationTime=500ms, IterationCount=15  -> 1 outlier  was  removed, 2 outliers were detected (52.92 μs, 53.03 μs)
  SamplerBasicBenchmarks.Uniform: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (34.77 μs)
  SamplerBasicBenchmarks.Bernoulli: MinIterationTime=500ms, IterationCount=15   -> 1 outlier  was  removed (156.80 μs)
  SamplerBasicBenchmarks.Cauchy: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (3.03 ms)
  SamplerBasicBenchmarks.Exponential: MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  removed, 2 outliers were detected (311.17 μs, 313.47 μs)
  SamplerBasicBenchmarks.Gumbel: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed, 2 outliers were detected (4.48 ms, 4.51 ms)
  SamplerBasicBenchmarks.Logistic: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  removed (4.63 ms)
  SamplerBasicBenchmarks.Normal: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (300.95 μs)
  SamplerBasicBenchmarks.Pareto: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (7.12 ms)
  SamplerBasicBenchmarks.Uniform: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (258.64 μs)
  SamplerBasicBenchmarks.Weibull: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  detected (6.51 ms)
  SamplerBasicBenchmarks.Categorical: MinIterationTime=500ms, IterationCount=15 -> 2 outliers were removed (54.93 μs, 54.98 μs)
  SamplerBasicBenchmarks.Gumbel: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (91.13 μs)
  SamplerBasicBenchmarks.Laplace: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  detected (101.49 μs)
  SamplerBasicBenchmarks.Logistic: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  removed (58.07 μs)
  SamplerBasicBenchmarks.Triangular: MinIterationTime=500ms, IterationCount=15  -> 2 outliers were removed (48.97 μs, 48.98 μs)
  SamplerBasicBenchmarks.Bernoulli: MinIterationTime=500ms, IterationCount=15   -> 1 outlier  was  removed (17.93 μs)
  SamplerBasicBenchmarks.Categorical: MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  detected (45.64 μs)
  SamplerBasicBenchmarks.Cauchy: MinIterationTime=500ms, IterationCount=15      -> 2 outliers were removed (42.95 μs, 43.04 μs)
  SamplerBasicBenchmarks.Exponential: MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  removed (55.97 μs)
  SamplerBasicBenchmarks.Gumbel: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (58.95 μs)
  SamplerBasicBenchmarks.Laplace: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed, 2 outliers were detected (97.23 μs, 97.60 μs)
  SamplerBasicBenchmarks.Logistic: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  removed (23.38 μs)
  SamplerBasicBenchmarks.Normal: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (58.35 μs)
  SamplerBasicBenchmarks.Pareto: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (25.13 μs)
  SamplerBasicBenchmarks.Rayleigh: MinIterationTime=500ms, IterationCount=15    -> 2 outliers were removed (54.94 μs, 54.94 μs)
  SamplerBasicBenchmarks.Triangular: MinIterationTime=500ms, IterationCount=15  -> 3 outliers were removed (39.14 μs..39.41 μs)
  SamplerBasicBenchmarks.Uniform: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (19.03 μs)
  SamplerBasicBenchmarks.Weibull: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed, 2 outliers were detected (61.95 μs, 62.49 μs)
  SamplerBasicBenchmarks.Exponential: MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  removed (67.41 μs)
  SamplerBasicBenchmarks.Laplace: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (113.36 μs)
  SamplerBasicBenchmarks.Logistic: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  removed (45.55 μs)
  SamplerBasicBenchmarks.Pareto: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed, 2 outliers were detected (44.66 μs, 44.85 μs)
  SamplerBasicBenchmarks.Uniform: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (35.30 μs)
  SamplerBasicBenchmarks.Weibull: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  detected (77.57 μs)
  SamplerBasicBenchmarks.Exponential: MinIterationTime=500ms, IterationCount=15 -> 2 outliers were removed, 3 outliers were detected (309.46 μs, 315.42 μs, 317.19 μs)
  SamplerBasicBenchmarks.Gumbel: MinIterationTime=500ms, IterationCount=15      -> 2 outliers were removed (4.58 ms, 4.62 ms)
  SamplerBasicBenchmarks.Laplace: MinIterationTime=500ms, IterationCount=15     -> 2 outliers were removed (380.40 μs, 396.32 μs)
  SamplerBasicBenchmarks.Logistic: MinIterationTime=500ms, IterationCount=15    -> 2 outliers were removed (4.65 ms, 4.68 ms)
  SamplerBasicBenchmarks.Pareto: MinIterationTime=500ms, IterationCount=15      -> 2 outliers were detected (6.79 ms, 6.79 ms)
  SamplerBasicBenchmarks.Rayleigh: MinIterationTime=500ms, IterationCount=15    -> 2 outliers were removed, 3 outliers were detected (1.32 ms, 1.33 ms, 1.34 ms)
  SamplerBasicBenchmarks.Triangular: MinIterationTime=500ms, IterationCount=15  -> 1 outlier  was  removed (1.36 ms)
  SamplerBasicBenchmarks.Bernoulli: MinIterationTime=500ms, IterationCount=15   -> 1 outlier  was  removed (19.00 μs)
  SamplerBasicBenchmarks.Categorical: MinIterationTime=500ms, IterationCount=15 -> 2 outliers were removed (44.00 μs, 44.01 μs)
  SamplerBasicBenchmarks.Exponential: MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  removed (65.24 μs)
  SamplerBasicBenchmarks.Logistic: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  removed (66.89 μs)
  SamplerBasicBenchmarks.Triangular: MinIterationTime=500ms, IterationCount=15  -> 2 outliers were removed (47.11 μs, 47.25 μs)
  SamplerBasicBenchmarks.Uniform: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (23.34 μs)

// * Legends *
  Param       : Value of the 'Param' parameter
  Mean        : Arithmetic mean of all measurements
  Error       : Half of 99.9% confidence interval
  StdDev      : Standard deviation of all measurements
  Ratio       : Mean of the ratio distribution ([Current]/[Baseline])
  RatioSD     : Standard deviation of the ratio distribution ([Current]/[Baseline])
  Allocated   : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  Alloc Ratio : Allocated memory ratio distribution ([Current]/[Baseline])
  1 μs        : 1 Microsecond (0.000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
Run time: 00:47:49 (2869.98 sec), executed benchmarks: 156

*/