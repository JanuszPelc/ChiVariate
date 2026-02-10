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
  Job-BHHXHU : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD

MinIterationTime=500ms  IterationCount=15  

| Method      | Param           | Mean        | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------ |---------------- |------------:|----------:|----------:|------:|--------:|----------:|------------:|
| Bernoulli   | ForLoop_Float   |    15.84 μs |  0.018 μs |  0.015 μs |  1.04 |    0.00 |         - |          NA |
| Categorical | ForLoop_Float   |    44.03 μs |  0.058 μs |  0.054 μs |  2.88 |    0.00 |         - |          NA |
| Cauchy      | ForLoop_Float   |    38.61 μs |  0.406 μs |  0.380 μs |  2.53 |    0.02 |         - |          NA |
| Exponential | ForLoop_Float   |    47.30 μs |  0.026 μs |  0.022 μs |  3.10 |    0.00 |         - |          NA |
| Gumbel      | ForLoop_Float   |    51.32 μs |  0.250 μs |  0.234 μs |  3.36 |    0.01 |         - |          NA |
| Laplace     | ForLoop_Float   |    93.62 μs |  0.044 μs |  0.041 μs |  6.13 |    0.00 |         - |          NA |
| Logistic    | ForLoop_Float   |    18.67 μs |  0.008 μs |  0.007 μs |  1.22 |    0.00 |         - |          NA |
| Normal      | ForLoop_Float   |    54.07 μs |  0.028 μs |  0.024 μs |  3.54 |    0.00 |         - |          NA |
| Pareto      | ForLoop_Float   |    20.80 μs |  0.015 μs |  0.012 μs |  1.36 |    0.00 |         - |          NA |
| Rayleigh    | ForLoop_Float   |    47.62 μs |  0.020 μs |  0.016 μs |  3.12 |    0.00 |         - |          NA |
| Triangular  | ForLoop_Float   |    34.61 μs |  0.030 μs |  0.028 μs |  2.27 |    0.00 |         - |          NA |
| Uniform     | ForLoop_Float   |    15.26 μs |  0.011 μs |  0.010 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | ForLoop_Float   |    52.52 μs |  0.036 μs |  0.034 μs |  3.44 |    0.00 |         - |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | ForLoop_Double  |    31.58 μs |  0.018 μs |  0.015 μs |  1.02 |    0.00 |         - |          NA |
| Categorical | ForLoop_Double  |    59.17 μs |  0.532 μs |  0.498 μs |  1.91 |    0.02 |         - |          NA |
| Cauchy      | ForLoop_Double  |    53.29 μs |  0.076 μs |  0.071 μs |  1.72 |    0.00 |         - |          NA |
| Exponential | ForLoop_Double  |    63.70 μs |  0.108 μs |  0.101 μs |  2.06 |    0.00 |         - |          NA |
| Gumbel      | ForLoop_Double  |    77.36 μs |  0.074 μs |  0.069 μs |  2.50 |    0.00 |         - |          NA |
| Laplace     | ForLoop_Double  |   111.20 μs |  0.790 μs |  0.701 μs |  3.59 |    0.02 |         - |          NA |
| Logistic    | ForLoop_Double  |    41.70 μs |  0.066 μs |  0.062 μs |  1.35 |    0.00 |         - |          NA |
| Normal      | ForLoop_Double  |    70.82 μs |  0.125 μs |  0.116 μs |  2.29 |    0.00 |         - |          NA |
| Pareto      | ForLoop_Double  |    40.71 μs |  0.071 μs |  0.066 μs |  1.31 |    0.00 |         - |          NA |
| Rayleigh    | ForLoop_Double  |    64.14 μs |  0.110 μs |  0.103 μs |  2.07 |    0.00 |         - |          NA |
| Triangular  | ForLoop_Double  |    48.72 μs |  0.078 μs |  0.073 μs |  1.57 |    0.00 |         - |          NA |
| Uniform     | ForLoop_Double  |    30.98 μs |  0.024 μs |  0.019 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | ForLoop_Double  |    74.17 μs |  0.150 μs |  0.141 μs |  2.39 |    0.00 |         - |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | ForLoop_Decimal |   149.81 μs |  0.620 μs |  0.549 μs |  0.65 |    0.00 |         - |          NA |
| Categorical | ForLoop_Decimal |   172.66 μs |  0.372 μs |  0.348 μs |  0.75 |    0.00 |         - |          NA |
| Cauchy      | ForLoop_Decimal | 2,991.24 μs |  6.233 μs |  5.831 μs | 12.97 |    0.03 |       3 B |          NA |
| Exponential | ForLoop_Decimal |   307.56 μs |  0.642 μs |  0.600 μs |  1.33 |    0.00 |         - |          NA |
| Gumbel      | ForLoop_Decimal | 4,474.96 μs | 11.469 μs | 10.728 μs | 19.41 |    0.05 |       6 B |          NA |
| Laplace     | ForLoop_Decimal |   336.95 μs |  0.616 μs |  0.576 μs |  1.46 |    0.00 |         - |          NA |
| Logistic    | ForLoop_Decimal | 4,625.45 μs | 12.436 μs | 11.632 μs | 20.06 |    0.06 |       6 B |          NA |
| Normal      | ForLoop_Decimal |   297.58 μs |  0.775 μs |  0.725 μs |  1.29 |    0.00 |         - |          NA |
| Pareto      | ForLoop_Decimal | 6,731.33 μs | 13.909 μs | 13.011 μs | 29.20 |    0.07 |       6 B |          NA |
| Rayleigh    | ForLoop_Decimal | 1,311.32 μs |  2.607 μs |  2.439 μs |  5.69 |    0.01 |       1 B |          NA |
| Triangular  | ForLoop_Decimal | 1,335.22 μs | 10.631 μs |  9.944 μs |  5.79 |    0.04 |       1 B |          NA |
| Uniform     | ForLoop_Decimal |   230.54 μs |  0.393 μs |  0.367 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | ForLoop_Decimal | 6,509.36 μs | 14.766 μs | 13.812 μs | 28.24 |    0.07 |       6 B |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | ForLoop_Fixed   |    16.05 μs |  0.047 μs |  0.044 μs |  0.71 |    0.00 |         - |          NA |
| Categorical | ForLoop_Fixed   |    41.09 μs |  0.069 μs |  0.065 μs |  1.83 |    0.00 |         - |          NA |
| Cauchy      | ForLoop_Fixed   |    44.93 μs |  0.091 μs |  0.085 μs |  2.00 |    0.01 |         - |          NA |
| Exponential | ForLoop_Fixed   |    55.49 μs |  0.090 μs |  0.084 μs |  2.47 |    0.01 |         - |          NA |
| Gumbel      | ForLoop_Fixed   |    79.24 μs |  0.610 μs |  0.571 μs |  3.53 |    0.03 |         - |          NA |
| Laplace     | ForLoop_Fixed   |    97.41 μs |  0.170 μs |  0.159 μs |  4.33 |    0.01 |         - |          NA |
| Logistic    | ForLoop_Fixed   |    61.69 μs |  0.526 μs |  0.492 μs |  2.74 |    0.02 |         - |          NA |
| Normal      | ForLoop_Fixed   |    63.68 μs |  0.096 μs |  0.085 μs |  2.83 |    0.01 |         - |          NA |
| Pareto      | ForLoop_Fixed   |    64.75 μs |  0.122 μs |  0.114 μs |  2.88 |    0.01 |         - |          NA |
| Rayleigh    | ForLoop_Fixed   |    76.55 μs |  0.119 μs |  0.112 μs |  3.41 |    0.01 |         - |          NA |
| Triangular  | ForLoop_Fixed   |    46.62 μs |  0.348 μs |  0.325 μs |  2.07 |    0.01 |         - |          NA |
| Uniform     | ForLoop_Fixed   |    22.48 μs |  0.045 μs |  0.042 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | ForLoop_Fixed   |    91.52 μs |  0.152 μs |  0.142 μs |  4.07 |    0.01 |         - |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | ForEach_Float   |    16.23 μs |  0.010 μs |  0.008 μs |  0.86 |    0.00 |         - |          NA |
| Categorical | ForEach_Float   |    49.92 μs |  0.115 μs |  0.108 μs |  2.65 |    0.01 |         - |          NA |
| Cauchy      | ForEach_Float   |    42.51 μs |  0.093 μs |  0.087 μs |  2.26 |    0.01 |         - |          NA |
| Exponential | ForEach_Float   |    50.98 μs |  0.115 μs |  0.108 μs |  2.71 |    0.01 |         - |          NA |
| Gumbel      | ForEach_Float   |    54.73 μs |  0.088 μs |  0.082 μs |  2.91 |    0.01 |         - |          NA |
| Laplace     | ForEach_Float   |   101.49 μs |  0.245 μs |  0.218 μs |  5.40 |    0.02 |         - |          NA |
| Logistic    | ForEach_Float   |    23.05 μs |  0.046 μs |  0.043 μs |  1.23 |    0.00 |         - |          NA |
| Normal      | ForEach_Float   |    62.25 μs |  0.178 μs |  0.167 μs |  3.31 |    0.01 |         - |          NA |
| Pareto      | ForEach_Float   |    24.96 μs |  0.052 μs |  0.049 μs |  1.33 |    0.00 |         - |          NA |
| Rayleigh    | ForEach_Float   |    51.42 μs |  0.360 μs |  0.319 μs |  2.73 |    0.02 |         - |          NA |
| Triangular  | ForEach_Float   |    38.76 μs |  0.098 μs |  0.092 μs |  2.06 |    0.01 |         - |          NA |
| Uniform     | ForEach_Float   |    18.81 μs |  0.055 μs |  0.051 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | ForEach_Float   |    56.42 μs |  0.104 μs |  0.098 μs |  3.00 |    0.01 |         - |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | ForEach_Double  |    31.95 μs |  0.081 μs |  0.071 μs |  0.92 |    0.00 |         - |          NA |
| Categorical | ForEach_Double  |    66.81 μs |  0.202 μs |  0.189 μs |  1.93 |    0.01 |         - |          NA |
| Cauchy      | ForEach_Double  |    56.73 μs |  0.091 μs |  0.085 μs |  1.64 |    0.01 |         - |          NA |
| Exponential | ForEach_Double  |    72.23 μs |  0.686 μs |  0.642 μs |  2.08 |    0.02 |         - |          NA |
| Gumbel      | ForEach_Double  |    87.13 μs |  0.854 μs |  0.757 μs |  2.51 |    0.02 |         - |          NA |
| Laplace     | ForEach_Double  |   122.32 μs |  0.183 μs |  0.171 μs |  3.53 |    0.01 |         - |          NA |
| Logistic    | ForEach_Double  |    45.65 μs |  0.087 μs |  0.082 μs |  1.32 |    0.00 |         - |          NA |
| Normal      | ForEach_Double  |    72.44 μs |  0.148 μs |  0.138 μs |  2.09 |    0.01 |         - |          NA |
| Pareto      | ForEach_Double  |    44.99 μs |  0.281 μs |  0.249 μs |  1.30 |    0.01 |         - |          NA |
| Rayleigh    | ForEach_Double  |    73.42 μs |  0.242 μs |  0.226 μs |  2.12 |    0.01 |         - |          NA |
| Triangular  | ForEach_Double  |    52.93 μs |  0.142 μs |  0.111 μs |  1.53 |    0.01 |         - |          NA |
| Uniform     | ForEach_Double  |    34.69 μs |  0.111 μs |  0.104 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | ForEach_Double  |    82.40 μs |  0.241 μs |  0.214 μs |  2.38 |    0.01 |         - |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | ForEach_Decimal |   155.36 μs |  0.514 μs |  0.481 μs |  0.60 |    0.00 |         - |          NA |
| Categorical | ForEach_Decimal |   185.98 μs |  1.958 μs |  1.635 μs |  0.72 |    0.01 |         - |          NA |
| Cauchy      | ForEach_Decimal | 3,009.10 μs |  8.440 μs |  7.895 μs | 11.67 |    0.05 |       3 B |          NA |
| Exponential | ForEach_Decimal |   310.46 μs |  0.842 μs |  0.788 μs |  1.20 |    0.00 |         - |          NA |
| Gumbel      | ForEach_Decimal | 4,481.25 μs | 14.015 μs | 13.110 μs | 17.37 |    0.07 |       6 B |          NA |
| Laplace     | ForEach_Decimal |   335.67 μs |  0.883 μs |  0.826 μs |  1.30 |    0.01 |         - |          NA |
| Logistic    | ForEach_Decimal | 4,546.51 μs | 14.879 μs | 12.425 μs | 17.63 |    0.07 |       6 B |          NA |
| Normal      | ForEach_Decimal |   299.26 μs |  1.069 μs |  1.000 μs |  1.16 |    0.01 |         - |          NA |
| Pareto      | ForEach_Decimal | 6,739.81 μs | 19.483 μs | 18.225 μs | 26.13 |    0.11 |       6 B |          NA |
| Rayleigh    | ForEach_Decimal | 1,318.67 μs |  3.832 μs |  3.585 μs |  5.11 |    0.02 |       1 B |          NA |
| Triangular  | ForEach_Decimal | 1,328.94 μs |  4.162 μs |  3.893 μs |  5.15 |    0.02 |       1 B |          NA |
| Uniform     | ForEach_Decimal |   257.96 μs |  0.928 μs |  0.868 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | ForEach_Decimal | 6,510.48 μs | 23.269 μs | 21.766 μs | 25.24 |    0.12 |       6 B |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | ForEach_Fixed   |    16.50 μs |  0.053 μs |  0.047 μs |  0.74 |    0.00 |         - |          NA |
| Categorical | ForEach_Fixed   |    54.40 μs |  0.138 μs |  0.130 μs |  2.42 |    0.01 |         - |          NA |
| Cauchy      | ForEach_Fixed   |    45.07 μs |  0.144 μs |  0.120 μs |  2.01 |    0.01 |         - |          NA |
| Exponential | ForEach_Fixed   |    61.12 μs |  0.059 μs |  0.046 μs |  2.72 |    0.01 |         - |          NA |
| Gumbel      | ForEach_Fixed   |    92.54 μs |  0.237 μs |  0.221 μs |  4.12 |    0.01 |         - |          NA |
| Laplace     | ForEach_Fixed   |   104.57 μs |  0.253 μs |  0.224 μs |  4.66 |    0.01 |         - |          NA |
| Logistic    | ForEach_Fixed   |    63.06 μs |  0.389 μs |  0.345 μs |  2.81 |    0.02 |         - |          NA |
| Normal      | ForEach_Fixed   |    64.73 μs |  0.235 μs |  0.220 μs |  2.88 |    0.01 |         - |          NA |
| Pareto      | ForEach_Fixed   |    65.72 μs |  0.180 μs |  0.168 μs |  2.93 |    0.01 |         - |          NA |
| Rayleigh    | ForEach_Fixed   |    81.30 μs |  0.247 μs |  0.231 μs |  3.62 |    0.01 |         - |          NA |
| Triangular  | ForEach_Fixed   |    48.84 μs |  0.158 μs |  0.148 μs |  2.18 |    0.01 |         - |          NA |
| Uniform     | ForEach_Fixed   |    22.44 μs |  0.058 μs |  0.054 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | ForEach_Fixed   |   101.27 μs |  0.307 μs |  0.287 μs |  4.51 |    0.02 |         - |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | Linq_Float      |    18.00 μs |  0.042 μs |  0.037 μs |  0.95 |    0.00 |         - |          NA |
| Categorical | Linq_Float      |    46.18 μs |  0.079 μs |  0.074 μs |  2.45 |    0.01 |         - |          NA |
| Cauchy      | Linq_Float      |    43.10 μs |  0.056 μs |  0.044 μs |  2.28 |    0.01 |         - |          NA |
| Exponential | Linq_Float      |    55.88 μs |  0.355 μs |  0.332 μs |  2.96 |    0.02 |         - |          NA |
| Gumbel      | Linq_Float      |    58.79 μs |  0.159 μs |  0.149 μs |  3.11 |    0.01 |         - |          NA |
| Laplace     | Linq_Float      |    97.18 μs |  0.258 μs |  0.241 μs |  5.15 |    0.02 |         - |          NA |
| Logistic    | Linq_Float      |    23.13 μs |  0.062 μs |  0.058 μs |  1.23 |    0.00 |         - |          NA |
| Normal      | Linq_Float      |    58.25 μs |  0.140 μs |  0.131 μs |  3.08 |    0.01 |         - |          NA |
| Pareto      | Linq_Float      |    25.04 μs |  0.061 μs |  0.057 μs |  1.33 |    0.00 |         - |          NA |
| Rayleigh    | Linq_Float      |    54.79 μs |  0.115 μs |  0.108 μs |  2.90 |    0.01 |         - |          NA |
| Triangular  | Linq_Float      |    38.93 μs |  0.112 μs |  0.105 μs |  2.06 |    0.01 |         - |          NA |
| Uniform     | Linq_Float      |    18.88 μs |  0.056 μs |  0.053 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | Linq_Float      |    62.12 μs |  0.198 μs |  0.176 μs |  3.29 |    0.01 |         - |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | Linq_Double     |    33.43 μs |  0.100 μs |  0.089 μs |  0.96 |    0.00 |         - |          NA |
| Categorical | Linq_Double     |    62.06 μs |  0.232 μs |  0.217 μs |  1.79 |    0.01 |         - |          NA |
| Cauchy      | Linq_Double     |    56.83 μs |  0.152 μs |  0.142 μs |  1.64 |    0.00 |         - |          NA |
| Exponential | Linq_Double     |    66.77 μs |  0.201 μs |  0.188 μs |  1.92 |    0.01 |         - |          NA |
| Gumbel      | Linq_Double     |    79.98 μs |  0.252 μs |  0.236 μs |  2.30 |    0.01 |         - |          NA |
| Laplace     | Linq_Double     |   112.71 μs |  0.257 μs |  0.241 μs |  3.24 |    0.01 |         - |          NA |
| Logistic    | Linq_Double     |    45.30 μs |  0.286 μs |  0.253 μs |  1.30 |    0.01 |         - |          NA |
| Normal      | Linq_Double     |    74.57 μs |  1.273 μs |  1.191 μs |  2.15 |    0.03 |         - |          NA |
| Pareto      | Linq_Double     |    45.14 μs |  0.301 μs |  0.235 μs |  1.30 |    0.01 |         - |          NA |
| Rayleigh    | Linq_Double     |    67.91 μs |  0.129 μs |  0.115 μs |  1.95 |    0.00 |         - |          NA |
| Triangular  | Linq_Double     |    59.29 μs |  0.352 μs |  0.330 μs |  1.71 |    0.01 |         - |          NA |
| Uniform     | Linq_Double     |    34.74 μs |  0.057 μs |  0.053 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | Linq_Double     |    77.37 μs |  0.103 μs |  0.097 μs |  2.23 |    0.00 |         - |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | Linq_Decimal    |   164.14 μs |  0.229 μs |  0.214 μs |  0.66 |    0.00 |         - |          NA |
| Categorical | Linq_Decimal    |   175.13 μs |  0.276 μs |  0.258 μs |  0.70 |    0.00 |         - |          NA |
| Cauchy      | Linq_Decimal    | 3,059.57 μs |  2.931 μs |  2.598 μs | 12.22 |    0.01 |       3 B |          NA |
| Exponential | Linq_Decimal    |   312.92 μs |  0.453 μs |  0.424 μs |  1.25 |    0.00 |         - |          NA |
| Gumbel      | Linq_Decimal    | 4,539.79 μs |  9.679 μs |  9.054 μs | 18.13 |    0.04 |       6 B |          NA |
| Laplace     | Linq_Decimal    |   337.96 μs |  2.200 μs |  2.058 μs |  1.35 |    0.01 |         - |          NA |
| Logistic    | Linq_Decimal    | 4,620.99 μs |  6.699 μs |  6.266 μs | 18.45 |    0.03 |       6 B |          NA |
| Normal      | Linq_Decimal    |   298.10 μs |  0.813 μs |  0.760 μs |  1.19 |    0.00 |         - |          NA |
| Pareto      | Linq_Decimal    | 6,799.19 μs | 12.136 μs | 10.758 μs | 27.15 |    0.04 |       6 B |          NA |
| Rayleigh    | Linq_Decimal    | 1,316.20 μs |  2.235 μs |  2.091 μs |  5.26 |    0.01 |       1 B |          NA |
| Triangular  | Linq_Decimal    | 1,353.75 μs |  3.130 μs |  2.444 μs |  5.41 |    0.01 |       1 B |          NA |
| Uniform     | Linq_Decimal    |   250.41 μs |  0.144 μs |  0.112 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | Linq_Decimal    | 6,474.47 μs | 13.372 μs | 12.508 μs | 25.86 |    0.05 |       6 B |          NA |
|             |                 |             |           |           |       |         |           |             |
| Bernoulli   | Linq_Fixed      |    17.94 μs |  0.046 μs |  0.041 μs |  0.70 |    0.00 |         - |          NA |
| Categorical | Linq_Fixed      |    43.82 μs |  0.066 μs |  0.062 μs |  1.71 |    0.00 |         - |          NA |
| Cauchy      | Linq_Fixed      |    50.19 μs |  0.073 μs |  0.068 μs |  1.96 |    0.00 |         - |          NA |
| Exponential | Linq_Fixed      |    55.99 μs |  0.077 μs |  0.072 μs |  2.19 |    0.00 |         - |          NA |
| Gumbel      | Linq_Fixed      |    81.38 μs |  0.128 μs |  0.120 μs |  3.18 |    0.01 |         - |          NA |
| Laplace     | Linq_Fixed      |    99.13 μs |  0.155 μs |  0.145 μs |  3.87 |    0.01 |         - |          NA |
| Logistic    | Linq_Fixed      |    70.28 μs |  0.209 μs |  0.195 μs |  2.75 |    0.01 |         - |          NA |
| Normal      | Linq_Fixed      |    63.67 μs |  0.164 μs |  0.145 μs |  2.49 |    0.01 |         - |          NA |
| Pareto      | Linq_Fixed      |    68.28 μs |  0.111 μs |  0.098 μs |  2.67 |    0.01 |         - |          NA |
| Rayleigh    | Linq_Fixed      |    76.75 μs |  0.122 μs |  0.114 μs |  3.00 |    0.01 |         - |          NA |
| Triangular  | Linq_Fixed      |    47.58 μs |  0.078 μs |  0.073 μs |  1.86 |    0.00 |         - |          NA |
| Uniform     | Linq_Fixed      |    25.59 μs |  0.046 μs |  0.043 μs |  1.00 |    0.00 |         - |          NA |
| Weibull     | Linq_Fixed      |    92.35 μs |  0.098 μs |  0.091 μs |  3.61 |    0.01 |         - |          NA |

// * Hints *
Outliers
  SamplerBasicBenchmarks.Bernoulli: MinIterationTime=500ms, IterationCount=15   -> 2 outliers were removed (15.90 μs, 16.00 μs)
  SamplerBasicBenchmarks.Exponential: MinIterationTime=500ms, IterationCount=15 -> 2 outliers were removed (47.43 μs, 47.45 μs)
  SamplerBasicBenchmarks.Logistic: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  removed (18.70 μs)
  SamplerBasicBenchmarks.Normal: MinIterationTime=500ms, IterationCount=15      -> 2 outliers were removed (54.30 μs, 54.36 μs)
  SamplerBasicBenchmarks.Pareto: MinIterationTime=500ms, IterationCount=15      -> 2 outliers were removed (20.86 μs, 20.87 μs)
  SamplerBasicBenchmarks.Rayleigh: MinIterationTime=500ms, IterationCount=15    -> 2 outliers were removed (47.68 μs, 47.76 μs)
  SamplerBasicBenchmarks.Uniform: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (15.40 μs)
  SamplerBasicBenchmarks.Weibull: MinIterationTime=500ms, IterationCount=15     -> 2 outliers were detected (52.44 μs, 52.46 μs)
  SamplerBasicBenchmarks.Bernoulli: MinIterationTime=500ms, IterationCount=15   -> 2 outliers were removed (31.68 μs, 31.72 μs)
  SamplerBasicBenchmarks.Laplace: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (114.90 μs)
  SamplerBasicBenchmarks.Pareto: MinIterationTime=500ms, IterationCount=15      -> 2 outliers were detected (40.58 μs, 40.61 μs)
  SamplerBasicBenchmarks.Uniform: MinIterationTime=500ms, IterationCount=15     -> 3 outliers were removed (31.10 μs..31.12 μs)
  SamplerBasicBenchmarks.Bernoulli: MinIterationTime=500ms, IterationCount=15   -> 1 outlier  was  removed (151.72 μs)
  SamplerBasicBenchmarks.Normal: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (63.98 μs)
  SamplerBasicBenchmarks.Bernoulli: MinIterationTime=500ms, IterationCount=15   -> 3 outliers were removed (16.32 μs..16.32 μs)
  SamplerBasicBenchmarks.Laplace: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (102.37 μs)
  SamplerBasicBenchmarks.Rayleigh: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  removed (52.50 μs)
  SamplerBasicBenchmarks.Bernoulli: MinIterationTime=500ms, IterationCount=15   -> 1 outlier  was  removed (32.40 μs)
  SamplerBasicBenchmarks.Gumbel: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (90.40 μs)
  SamplerBasicBenchmarks.Logistic: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  detected (45.50 μs)
  SamplerBasicBenchmarks.Pareto: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed, 3 outliers were detected (44.49 μs, 44.52 μs, 45.45 μs)
  SamplerBasicBenchmarks.Triangular: MinIterationTime=500ms, IterationCount=15  -> 3 outliers were removed (53.48 μs..53.96 μs)
  SamplerBasicBenchmarks.Weibull: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (83.57 μs)
  SamplerBasicBenchmarks.Categorical: MinIterationTime=500ms, IterationCount=15 -> 2 outliers were removed (193.93 μs, 196.02 μs)
  SamplerBasicBenchmarks.Logistic: MinIterationTime=500ms, IterationCount=15    -> 2 outliers were removed (4.58 ms, 4.58 ms)
  SamplerBasicBenchmarks.Bernoulli: MinIterationTime=500ms, IterationCount=15   -> 1 outlier  was  removed (16.75 μs)
  SamplerBasicBenchmarks.Cauchy: MinIterationTime=500ms, IterationCount=15      -> 2 outliers were removed (45.68 μs, 45.83 μs)
  SamplerBasicBenchmarks.Exponential: MinIterationTime=500ms, IterationCount=15 -> 3 outliers were removed (61.53 μs..61.65 μs)
  SamplerBasicBenchmarks.Laplace: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (105.78 μs)
  SamplerBasicBenchmarks.Logistic: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  removed (64.28 μs)
  SamplerBasicBenchmarks.Bernoulli: MinIterationTime=500ms, IterationCount=15   -> 1 outlier  was  removed (18.11 μs)
  SamplerBasicBenchmarks.Cauchy: MinIterationTime=500ms, IterationCount=15      -> 3 outliers were removed (43.32 μs..43.66 μs)
  SamplerBasicBenchmarks.Weibull: MinIterationTime=500ms, IterationCount=15     -> 1 outlier  was  removed (62.61 μs)
  SamplerBasicBenchmarks.Bernoulli: MinIterationTime=500ms, IterationCount=15   -> 1 outlier  was  removed (33.68 μs)
  SamplerBasicBenchmarks.Logistic: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  removed (46.93 μs)
  SamplerBasicBenchmarks.Pareto: MinIterationTime=500ms, IterationCount=15      -> 3 outliers were removed, 5 outliers were detected (44.57 μs, 44.78 μs, 45.71 μs..48.33 μs)
  SamplerBasicBenchmarks.Rayleigh: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  removed (68.31 μs)
  SamplerBasicBenchmarks.Cauchy: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed, 2 outliers were detected (3.05 ms, 3.07 ms)
  SamplerBasicBenchmarks.Pareto: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (6.87 ms)
  SamplerBasicBenchmarks.Triangular: MinIterationTime=500ms, IterationCount=15  -> 3 outliers were removed (1.37 ms..1.39 ms)
  SamplerBasicBenchmarks.Uniform: MinIterationTime=500ms, IterationCount=15     -> 3 outliers were removed (251.24 μs..251.53 μs)
  SamplerBasicBenchmarks.Normal: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (64.74 μs)
  SamplerBasicBenchmarks.Pareto: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed, 2 outliers were detected (68.03 μs, 68.54 μs)

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
Run time: 00:46:36 (2796.82 sec), executed benchmarks: 156

*/