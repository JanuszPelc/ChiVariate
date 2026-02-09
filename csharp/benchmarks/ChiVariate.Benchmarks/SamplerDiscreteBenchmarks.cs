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

/*

// * Summary *

BenchmarkDotNet v0.14.0, macOS Sequoia 15.7.3 (24G419) [Darwin 24.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD
  Job-AMWZTP : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD

MinIterationTime=500ms  IterationCount=15

| Method              | Param          | Mean            | Error         | StdDev        | Ratio     | RatioSD | Allocated | Alloc Ratio |
|-------------------- |--------------- |----------------:|--------------:|--------------:|----------:|--------:|----------:|------------:|
| Binomial            | ForLoop_Int32  |     2,021.23 μs |      2.405 μs |      2.132 μs |    108.71 |    0.15 |       3 B |          NA |
| Chance.PickBetween  | ForLoop_Int32  |        18.44 μs |      0.108 μs |      0.101 μs |      0.99 |    0.01 |         - |          NA |
| Geometric           | ForLoop_Int32  |       162.75 μs |      0.235 μs |      0.220 μs |      8.75 |    0.01 |         - |          NA |
| Hypergeometric      | ForLoop_Int32  |     1,035.83 μs |      1.840 μs |      1.721 μs |     55.71 |    0.10 |       1 B |          NA |
| Multinomial         | ForLoop_Int32  |     3,086.51 μs |      4.568 μs |      4.273 μs |    166.01 |    0.27 |       3 B |          NA |
| 'Negative Binomial' | ForLoop_Int32  |       498.32 μs |      0.934 μs |      0.873 μs |     26.80 |    0.05 |       1 B |          NA |
| Poisson             | ForLoop_Int32  |       392.28 μs |      3.543 μs |      3.315 μs |     21.10 |    0.17 |         - |          NA |
| Prime               | ForLoop_Int32  |    81,091.55 μs |    109.763 μs |    102.672 μs |  4,361.45 |    6.62 |     105 B |          NA |
| Uniform             | ForLoop_Int32  |        18.59 μs |      0.021 μs |      0.017 μs |      1.00 |    0.00 |         - |          NA |
| Zipf                | ForLoop_Int32  |       114.29 μs |      0.146 μs |      0.137 μs |      6.15 |    0.01 |         - |          NA |
|                     |                |                 |               |               |           |         |           |             |
| Binomial            | ForLoop_Int64  |     2,024.13 μs |      4.260 μs |      3.985 μs |     57.73 |    0.14 |       3 B |          NA |
| Chance.PickBetween  | ForLoop_Int64  |        35.22 μs |      0.311 μs |      0.291 μs |      1.00 |    0.01 |         - |          NA |
| Geometric           | ForLoop_Int64  |       165.20 μs |      0.287 μs |      0.268 μs |      4.71 |    0.01 |         - |          NA |
| Hypergeometric      | ForLoop_Int64  |     1,056.77 μs |      1.960 μs |      1.833 μs |     30.14 |    0.07 |       1 B |          NA |
| Multinomial         | ForLoop_Int64  |     3,132.53 μs |      7.335 μs |      6.861 μs |     89.34 |    0.24 |       3 B |          NA |
| 'Negative Binomial' | ForLoop_Int64  |       498.73 μs |      1.186 μs |      1.110 μs |     14.22 |    0.04 |       1 B |          NA |
| Poisson             | ForLoop_Int64  |       392.98 μs |      3.740 μs |      3.498 μs |     11.21 |    0.10 |         - |          NA |
| Prime               | ForLoop_Int64  |   424,851.07 μs |  5,653.644 μs |  5,288.422 μs | 12,117.24 |  147.35 |     736 B |          NA |
| Uniform             | ForLoop_Int64  |        35.06 μs |      0.063 μs |      0.059 μs |      1.00 |    0.00 |         - |          NA |
| Zipf                | ForLoop_Int64  |       114.24 μs |      0.166 μs |      0.155 μs |      3.26 |    0.01 |         - |          NA |
|                     |                |                 |               |               |           |         |           |             |
| Binomial            | ForLoop_Int128 |     2,057.20 μs |      4.525 μs |      4.232 μs |     16.64 |    0.05 |       3 B |          NA |
| Chance.PickBetween  | ForLoop_Int128 |       120.87 μs |      0.111 μs |      0.099 μs |      0.98 |    0.00 |         - |          NA |
| Geometric           | ForLoop_Int128 |       162.91 μs |      0.261 μs |      0.244 μs |      1.32 |    0.00 |         - |          NA |
| Hypergeometric      | ForLoop_Int128 |     1,108.25 μs |      2.232 μs |      2.088 μs |      8.96 |    0.02 |       1 B |          NA |
| Multinomial         | ForLoop_Int128 |     3,108.27 μs |      6.352 μs |      5.942 μs |     25.14 |    0.07 |       3 B |          NA |
| 'Negative Binomial' | ForLoop_Int128 |       505.64 μs |      1.409 μs |      1.318 μs |      4.09 |    0.01 |       1 B |          NA |
| Poisson             | ForLoop_Int128 |       390.16 μs |      0.718 μs |      0.672 μs |      3.16 |    0.01 |         - |          NA |
| Prime               | ForLoop_Int128 | 3,251,431.35 μs | 20,885.352 μs | 18,514.321 μs | 26,299.03 |  153.27 |     736 B |          NA |
| Uniform             | ForLoop_Int128 |       123.63 μs |      0.263 μs |      0.246 μs |      1.00 |    0.00 |         - |          NA |
| Zipf                | ForLoop_Int128 |       118.57 μs |      0.483 μs |      0.403 μs |      0.96 |    0.00 |         - |          NA |
|                     |                |                 |               |               |           |         |           |             |
| Binomial            | ForEach_Int32  |     2,079.61 μs |     49.392 μs |     46.202 μs |    102.66 |    2.22 |       3 B |          NA |
| Chance.PickBetween  | ForEach_Int32  |        18.70 μs |      0.409 μs |      0.383 μs |      0.92 |    0.02 |         - |          NA |
| Geometric           | ForEach_Int32  |       165.08 μs |      0.365 μs |      0.323 μs |      8.15 |    0.02 |         - |          NA |
| Hypergeometric      | ForEach_Int32  |     1,037.29 μs |      2.615 μs |      2.446 μs |     51.21 |    0.16 |       1 B |          NA |
| Multinomial         | ForEach_Int32  |     3,176.73 μs |      5.903 μs |      5.522 μs |    156.82 |    0.41 |       3 B |          NA |
| 'Negative Binomial' | ForEach_Int32  |       498.79 μs |      1.355 μs |      1.268 μs |     24.62 |    0.08 |       1 B |          NA |
| Poisson             | ForEach_Int32  |       410.31 μs |      0.727 μs |      0.645 μs |     20.26 |    0.05 |         - |          NA |
| Prime               | ForEach_Int32  |    83,563.85 μs |  1,031.505 μs |    964.870 μs |  4,125.13 |   46.85 |     105 B |          NA |
| Uniform             | ForEach_Int32  |        20.26 μs |      0.047 μs |      0.042 μs |      1.00 |    0.00 |         - |          NA |
| Zipf                | ForEach_Int32  |       132.95 μs |      1.086 μs |      0.907 μs |      6.56 |    0.05 |         - |          NA |
|                     |                |                 |               |               |           |         |           |             |
| Binomial            | ForEach_Int64  |     2,051.00 μs |      4.681 μs |      4.379 μs |     55.76 |    0.14 |       3 B |          NA |
| Chance.PickBetween  | ForEach_Int64  |        35.25 μs |      0.056 μs |      0.047 μs |      0.96 |    0.00 |         - |          NA |
| Geometric           | ForEach_Int64  |       165.02 μs |      0.319 μs |      0.283 μs |      4.49 |    0.01 |         - |          NA |
| Hypergeometric      | ForEach_Int64  |     1,043.56 μs |      2.417 μs |      2.261 μs |     28.37 |    0.07 |       1 B |          NA |
| Multinomial         | ForEach_Int64  |     3,177.00 μs |      4.988 μs |      4.665 μs |     86.38 |    0.17 |       3 B |          NA |
| 'Negative Binomial' | ForEach_Int64  |       499.42 μs |      1.803 μs |      1.687 μs |     13.58 |    0.05 |       1 B |          NA |
| Poisson             | ForEach_Int64  |       410.64 μs |      0.794 μs |      0.743 μs |     11.16 |    0.03 |         - |          NA |
| Prime               | ForEach_Int64  |   416,241.15 μs |  2,362.755 μs |  1,973.008 μs | 11,317.07 |   54.15 |     736 B |          NA |
| Uniform             | ForEach_Int64  |        36.78 μs |      0.061 μs |      0.054 μs |      1.00 |    0.00 |         - |          NA |
| Zipf                | ForEach_Int64  |       132.78 μs |      1.694 μs |      1.501 μs |      3.61 |    0.04 |         - |          NA |
|                     |                |                 |               |               |           |         |           |             |
| Binomial            | ForEach_Int128 |     2,059.00 μs |      2.721 μs |      2.412 μs |     15.22 |    0.14 |       3 B |          NA |
| Chance.PickBetween  | ForEach_Int128 |       120.75 μs |      0.279 μs |      0.233 μs |      0.89 |    0.01 |         - |          NA |
| Geometric           | ForEach_Int128 |       166.14 μs |      1.341 μs |      1.255 μs |      1.23 |    0.01 |         - |          NA |
| Hypergeometric      | ForEach_Int128 |     1,120.37 μs |      1.977 μs |      1.850 μs |      8.28 |    0.08 |       1 B |          NA |
| Multinomial         | ForEach_Int128 |     3,194.67 μs |      4.436 μs |      4.149 μs |     23.61 |    0.21 |       3 B |          NA |
| 'Negative Binomial' | ForEach_Int128 |       508.92 μs |      0.944 μs |      0.883 μs |      3.76 |    0.03 |       1 B |          NA |
| Poisson             | ForEach_Int128 |       410.75 μs |      1.373 μs |      1.217 μs |      3.04 |    0.03 |         - |          NA |
| Prime               | ForEach_Int128 | 3,248,225.72 μs | 20,300.610 μs | 17,995.962 μs | 24,009.33 |  251.58 |     736 B |          NA |
| Uniform             | ForEach_Int128 |       135.30 μs |      1.344 μs |      1.257 μs |      1.00 |    0.01 |         - |          NA |
| Zipf                | ForEach_Int128 |       130.02 μs |      0.256 μs |      0.239 μs |      0.96 |    0.01 |         - |          NA |
|                     |                |                 |               |               |           |         |           |             |
| Binomial            | Linq_Int32     |     2,053.74 μs |      2.895 μs |      2.708 μs |    104.33 |    0.24 |       3 B |          NA |
| Chance.PickBetween  | Linq_Int32     |        18.62 μs |      0.030 μs |      0.028 μs |      0.95 |    0.00 |         - |          NA |
| Geometric           | Linq_Int32     |       168.88 μs |      0.229 μs |      0.215 μs |      8.58 |    0.02 |         - |          NA |
| Hypergeometric      | Linq_Int32     |     1,141.59 μs |      2.719 μs |      2.543 μs |     57.99 |    0.17 |       1 B |          NA |
| Multinomial         | Linq_Int32     |     3,173.11 μs |      5.025 μs |      4.700 μs |    161.19 |    0.39 |     123 B |          NA |
| 'Negative Binomial' | Linq_Int32     |       499.12 μs |      1.754 μs |      1.555 μs |     25.36 |    0.09 |       1 B |          NA |
| Poisson             | Linq_Int32     |       390.83 μs |      0.897 μs |      0.749 μs |     19.85 |    0.05 |         - |          NA |
| Prime               | Linq_Int32     |    82,591.84 μs |    310.058 μs |    274.858 μs |  4,195.58 |   15.77 |     243 B |          NA |
| Uniform             | Linq_Int32     |        19.69 μs |      0.042 μs |      0.040 μs |      1.00 |    0.00 |         - |          NA |
| Zipf                | Linq_Int32     |       117.85 μs |      0.177 μs |      0.165 μs |      5.99 |    0.01 |         - |          NA |
|                     |                |                 |               |               |           |         |           |             |
| Binomial            | Linq_Int64     |     2,047.70 μs |     18.927 μs |     17.704 μs |     56.22 |    0.48 |       3 B |          NA |
| Chance.PickBetween  | Linq_Int64     |        34.81 μs |      0.059 μs |      0.055 μs |      0.96 |    0.00 |         - |          NA |
| Geometric           | Linq_Int64     |       166.52 μs |      0.323 μs |      0.302 μs |      4.57 |    0.01 |         - |          NA |
| Hypergeometric      | Linq_Int64     |     1,147.42 μs |      2.645 μs |      2.474 μs |     31.50 |    0.09 |       1 B |          NA |
| Multinomial         | Linq_Int64     |     3,170.70 μs |      5.249 μs |      4.654 μs |     87.05 |    0.20 |     123 B |          NA |
| 'Negative Binomial' | Linq_Int64     |       500.05 μs |      1.168 μs |      1.092 μs |     13.73 |    0.04 |       1 B |          NA |
| Poisson             | Linq_Int64     |       390.30 μs |      0.659 μs |      0.616 μs |     10.72 |    0.03 |         - |          NA |
| Prime               | Linq_Int64     |   415,754.30 μs |  3,358.424 μs |  2,804.436 μs | 11,414.55 |   77.10 |     856 B |          NA |
| Uniform             | Linq_Int64     |        36.42 μs |      0.078 μs |      0.070 μs |      1.00 |    0.00 |         - |          NA |
| Zipf                | Linq_Int64     |       117.36 μs |      0.157 μs |      0.147 μs |      3.22 |    0.01 |         - |          NA |
|                     |                |                 |               |               |           |         |           |             |
| Binomial            | Linq_Int128    |     2,180.58 μs |      6.495 μs |      6.075 μs |     14.97 |    0.07 |       3 B |          NA |
| Chance.PickBetween  | Linq_Int128    |       120.85 μs |      0.156 μs |      0.146 μs |      0.83 |    0.00 |         - |          NA |
| Geometric           | Linq_Int128    |       166.51 μs |      0.314 μs |      0.294 μs |      1.14 |    0.00 |         - |          NA |
| Hypergeometric      | Linq_Int128    |     1,284.09 μs |     10.490 μs |      9.812 μs |      8.81 |    0.07 |       1 B |          NA |
| Multinomial         | Linq_Int128    |     3,203.98 μs |      6.112 μs |      5.717 μs |     21.99 |    0.09 |     139 B |          NA |
| 'Negative Binomial' | Linq_Int128    |       530.62 μs |      1.100 μs |      1.029 μs |      3.64 |    0.01 |       1 B |          NA |
| Poisson             | Linq_Int128    |       393.14 μs |      4.150 μs |      3.882 μs |      2.70 |    0.03 |         - |          NA |
| Prime               | Linq_Int128    | 3,281,973.44 μs | 23,854.946 μs | 21,146.788 μs | 22,527.14 |  161.82 |     872 B |          NA |
| Uniform             | Linq_Int128    |       145.69 μs |      0.576 μs |      0.539 μs |      1.00 |    0.01 |         - |          NA |
| Zipf                | Linq_Int128    |       121.69 μs |      0.195 μs |      0.182 μs |      0.84 |    0.00 |         - |          NA |

// * Hints *
Outliers
  SamplerDiscreteBenchmarks.Binomial: MinIterationTime=500ms, IterationCount=15            -> 1 outlier  was  removed (2.04 ms)
  SamplerDiscreteBenchmarks.Prime: MinIterationTime=500ms, IterationCount=15               -> 2 outliers were detected (80.87 ms, 80.92 ms)
  SamplerDiscreteBenchmarks.Uniform: MinIterationTime=500ms, IterationCount=15             -> 2 outliers were removed (18.64 μs, 18.66 μs)
  SamplerDiscreteBenchmarks.Geometric: MinIterationTime=500ms, IterationCount=15           -> 1 outlier  was  detected (164.65 μs)
  SamplerDiscreteBenchmarks.Chance.PickBetween: MinIterationTime=500ms, IterationCount=15  -> 1 outlier  was  removed (121.96 μs)
  SamplerDiscreteBenchmarks.Prime: MinIterationTime=500ms, IterationCount=15               -> 1 outlier  was  removed, 3 outliers were detected (3.21 s, 3.22 s, 3.34 s)
  SamplerDiscreteBenchmarks.Zipf: MinIterationTime=500ms, IterationCount=15                -> 2 outliers were removed (120.51 μs, 121.11 μs)
  SamplerDiscreteBenchmarks.Geometric: MinIterationTime=500ms, IterationCount=15           -> 1 outlier  was  removed (166.01 μs)
  SamplerDiscreteBenchmarks.Poisson: MinIterationTime=500ms, IterationCount=15             -> 1 outlier  was  removed (414.23 μs)
  SamplerDiscreteBenchmarks.Uniform: MinIterationTime=500ms, IterationCount=15             -> 1 outlier  was  removed (20.61 μs)
  SamplerDiscreteBenchmarks.Zipf: MinIterationTime=500ms, IterationCount=15                -> 2 outliers were removed (148.24 μs, 148.87 μs)
  SamplerDiscreteBenchmarks.Chance.PickBetween: MinIterationTime=500ms, IterationCount=15  -> 2 outliers were removed, 3 outliers were detected (35.16 μs, 35.40 μs, 35.40 μs)
  SamplerDiscreteBenchmarks.Geometric: MinIterationTime=500ms, IterationCount=15           -> 1 outlier  was  removed (167.28 μs)
  SamplerDiscreteBenchmarks.Prime: MinIterationTime=500ms, IterationCount=15               -> 2 outliers were removed (422.67 ms, 428.53 ms)
  SamplerDiscreteBenchmarks.Uniform: MinIterationTime=500ms, IterationCount=15             -> 1 outlier  was  removed (37.39 μs)
  SamplerDiscreteBenchmarks.Zipf: MinIterationTime=500ms, IterationCount=15                -> 1 outlier  was  removed (139.05 μs)
  SamplerDiscreteBenchmarks.Binomial: MinIterationTime=500ms, IterationCount=15            -> 1 outlier  was  removed (2.08 ms)
  SamplerDiscreteBenchmarks.Chance.PickBetween: MinIterationTime=500ms, IterationCount=15  -> 2 outliers were removed (123.17 μs, 123.26 μs)
  SamplerDiscreteBenchmarks.Poisson: MinIterationTime=500ms, IterationCount=15             -> 1 outlier  was  removed (414.75 μs)
  SamplerDiscreteBenchmarks.Prime: MinIterationTime=500ms, IterationCount=15               -> 1 outlier  was  removed, 3 outliers were detected (3.21 s, 3.21 s, 3.34 s)
  SamplerDiscreteBenchmarks.Zipf: MinIterationTime=500ms, IterationCount=15                -> 1 outlier  was  detected (129.52 μs)
  SamplerDiscreteBenchmarks.Geometric: MinIterationTime=500ms, IterationCount=15           -> 1 outlier  was  detected (168.41 μs)
  SamplerDiscreteBenchmarks.'Negative Binomial': MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  removed (505.58 μs)
  SamplerDiscreteBenchmarks.Poisson: MinIterationTime=500ms, IterationCount=15             -> 2 outliers were removed (395.66 μs, 396.33 μs)
  SamplerDiscreteBenchmarks.Prime: MinIterationTime=500ms, IterationCount=15               -> 1 outlier  was  removed (83.67 ms)
  SamplerDiscreteBenchmarks.Multinomial: MinIterationTime=500ms, IterationCount=15         -> 1 outlier  was  removed (3.24 ms)
  SamplerDiscreteBenchmarks.Prime: MinIterationTime=500ms, IterationCount=15               -> 2 outliers were removed (423.09 ms, 427.73 ms)
  SamplerDiscreteBenchmarks.Uniform: MinIterationTime=500ms, IterationCount=15             -> 1 outlier  was  removed (36.94 μs)
  SamplerDiscreteBenchmarks.Prime: MinIterationTime=500ms, IterationCount=15               -> 1 outlier  was  removed (3.38 s)

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
Run time: 00:28:37 (1717.9 sec), executed benchmarks: 90

*/