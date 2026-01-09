using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Benchmarks;

/// <summary>
///     Benchmarks for discrete distributions that return integer values.
///     Includes: Binomial, Chance, Geometric, Hypergeometric, Multinomial, NegativeBinomial,
///     Poisson, Prime, Uniform (discrete), Zipf.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
[MinIterationTime(500)]
[IterationCount(15)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams)]
public class SamplerDiscreteBenchmarks
{
    #region Benchmarks

    [Benchmark(Description = "Binomial")]
    public bool ChiVariateBinomial()
    {
        return ValueTypeParam switch
        {
            ValueType.Int32 => RunBenchmark<int>(),
            ValueType.Int64 => RunBenchmark<long>(),
            ValueType.Int128 => RunBenchmark<Int128>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IBinaryInteger<T>
        {
            var sampler = _rng.Binomial(T.CreateChecked(50), 0.25);
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

    [Benchmark(Description = "Chance.PickBetween")]
    public bool ChiVariateChance()
    {
        return ValueTypeParam switch
        {
            ValueType.Int32 => RunBenchmark<int>(),
            ValueType.Int64 => RunBenchmark<long>(),
            ValueType.Int128 => RunBenchmark<Int128>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
        {
            var sampler = _rng.Chance();
            var min = T.MinValue / T.CreateChecked(100);
            var max = T.MaxValue / T.CreateChecked(100);
            var sum = T.Zero;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                case IterationType.ForEach:
                case IterationType.Linq:
                {
                    for (var i = 0; i < SampleCount; i++)
                        sum += sampler.PickBetween(min, max);
                    break;
                }
                default: throw new UnreachableException();
            }

            return Consume(sum);
        }
    }

    [Benchmark(Description = "Geometric")]
    public bool ChiVariateGeometric()
    {
        return ValueTypeParam switch
        {
            ValueType.Int32 => RunBenchmark<int>(),
            ValueType.Int64 => RunBenchmark<long>(),
            ValueType.Int128 => RunBenchmark<Int128>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IBinaryInteger<T>
        {
            var sampler = _rng.Geometric(0.25);
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

    [Benchmark(Description = "Hypergeometric")]
    public bool ChiVariateHypergeometric()
    {
        return ValueTypeParam switch
        {
            ValueType.Int32 => RunBenchmark<int>(),
            ValueType.Int64 => RunBenchmark<long>(),
            ValueType.Int128 => RunBenchmark<Int128>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IBinaryInteger<T>
        {
            var sampler = _rng.Hypergeometric(T.CreateChecked(100), T.CreateChecked(50), T.CreateChecked(20));
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

    [Benchmark(Description = "Multinomial")]
    public bool ChiVariateMultinomial()
    {
        return ValueTypeParam switch
        {
            ValueType.Int32 => RunBenchmark<int>(),
            ValueType.Int64 => RunBenchmark<long>(),
            ValueType.Int128 => RunBenchmark<Int128>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IBinaryInteger<T>
        {
            var probabilities = TypedWeight<double>.Data;
            var sampler = _rng.Multinomial(T.CreateChecked(50), (ReadOnlySpan<double>)probabilities);
            var sum = T.Zero;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                    {
                        using var vector = sampler.Sample();
                        sum += vector[0];
                    }

                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var vector in sampler.Sample(SampleCount))
                    {
                        sum += vector[0];
                        vector.Dispose();
                    }

                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount)
                        .Select(vector =>
                        {
                            using (vector)
                            {
                                return vector[0];
                            }
                        });
                    sum = SumValues(enumerable);
                    break;
                }
                default: throw new UnreachableException();
            }

            return Consume(sum);
        }
    }

    [Benchmark(Description = "Negative Binomial")]
    public bool ChiVariateNegativeBinomial()
    {
        return ValueTypeParam switch
        {
            ValueType.Int32 => RunBenchmark<int>(),
            ValueType.Int64 => RunBenchmark<long>(),
            ValueType.Int128 => RunBenchmark<Int128>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IBinaryInteger<T>
        {
            var sampler = _rng.NegativeBinomial(T.CreateChecked(5), 0.5);
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

    [Benchmark(Description = "Poisson")]
    public bool ChiVariatePoisson()
    {
        return ValueTypeParam switch
        {
            ValueType.Int32 => RunBenchmark<int>(),
            ValueType.Int64 => RunBenchmark<long>(),
            ValueType.Int128 => RunBenchmark<Int128>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IBinaryInteger<T>
        {
            var sampler = _rng.Poisson(10.0);
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

    [Benchmark(Description = "Prime")]
    public bool ChiVariatePrime()
    {
        return ValueTypeParam switch
        {
            ValueType.Int32 => RunBenchmark<int>(),
            ValueType.Int64 => RunBenchmark<long>(),
            ValueType.Int128 => RunBenchmark<Int128>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>()
            where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
        {
            var sampler = _rng.Prime(T.Zero, T.MaxValue);
            var sum = T.Zero;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                        sum += (sampler.Sample() & T.Zero) | T.One;

                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var sample in sampler.Sample(SampleCount))
                        sum += (sample & T.Zero) | T.One;
                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount)
                        .Select(sample => (sample & T.Zero) | T.One);
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
            ValueType.Int32 => RunBenchmark<int>(),
            ValueType.Int64 => RunBenchmark<long>(),
            ValueType.Int128 => RunBenchmark<Int128>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
        {
            var sampler = _rng.Uniform(T.MinValue / T.CreateChecked(100), T.MaxValue / T.CreateChecked(100));
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

    [Benchmark(Description = "Zipf")]
    public bool ChiVariateZipf()
    {
        return ValueTypeParam switch
        {
            ValueType.Int32 => RunBenchmark<int>(),
            ValueType.Int64 => RunBenchmark<long>(),
            ValueType.Int128 => RunBenchmark<Int128>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IBinaryInteger<T>
        {
            var sampler = _rng.Zipf(T.CreateChecked(100), 1.07);
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
        Int32,
        Int64,
        Int128,
        BitMask = 0xFFFF
    }

    public enum ParamType
    {
        // ReSharper disable InconsistentNaming
        ForLoop_Int32 = IterationType.ForLoop | ValueType.Int32,
        ForLoop_Int64 = IterationType.ForLoop | ValueType.Int64,
        ForLoop_Int128 = IterationType.ForLoop | ValueType.Int128,
        ForEach_Int32 = IterationType.ForEach | ValueType.Int32,
        ForEach_Int64 = IterationType.ForEach | ValueType.Int64,
        ForEach_Int128 = IterationType.ForEach | ValueType.Int128,
        Linq_Int32 = IterationType.Linq | ValueType.Int32,
        Linq_Int64 = IterationType.Linq | ValueType.Int64,

        Linq_Int128 = IterationType.Linq | ValueType.Int128
        // ReSharper restore InconsistentNaming
    }

    [Params(
        ParamType.ForLoop_Int32, ParamType.ForLoop_Int64, ParamType.ForLoop_Int128,
        ParamType.ForEach_Int32, ParamType.ForEach_Int64, ParamType.ForEach_Int128,
        ParamType.Linq_Int32, ParamType.Linq_Int64, ParamType.Linq_Int128)]
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
        if (typeof(T) == typeof(int))
            return T.CreateTruncating(enumerable.Sum(int.CreateTruncating));

        if (typeof(T) == typeof(long))
            return T.CreateTruncating(enumerable.Sum(long.CreateTruncating));

        if (typeof(T) == typeof(Int128))
            return enumerable.Aggregate(T.Zero, (sum, value) => sum + value);

        throw new UnreachableException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T SumValues<T>(IEnumerable<T> enumerable)
        where T : INumberBase<T>
    {
        if (typeof(T) == typeof(int))
            return T.CreateTruncating(enumerable.Sum(int.CreateTruncating));

        if (typeof(T) == typeof(long))
            return T.CreateTruncating(enumerable.Sum(long.CreateTruncating));

        if (typeof(T) == typeof(Int128))
            return enumerable.Aggregate(T.Zero, (sum, value) => sum + value);

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