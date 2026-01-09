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