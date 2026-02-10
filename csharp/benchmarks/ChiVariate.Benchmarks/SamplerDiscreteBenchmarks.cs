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
  Job-BHHXHU : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD

MinIterationTime=500ms  IterationCount=15  

| Method              | Param          | Mean            | Error         | StdDev        | Ratio     | RatioSD | Allocated | Alloc Ratio |
|-------------------- |--------------- |----------------:|--------------:|--------------:|----------:|--------:|----------:|------------:|
| Binomial            | ForLoop_Int32  |     2,023.57 μs |      5.401 μs |      5.052 μs |    107.80 |    0.81 |       3 B |          NA |
| Chance.PickBetween  | ForLoop_Int32  |        18.35 μs |      0.039 μs |      0.036 μs |      0.98 |    0.01 |         - |          NA |
| Geometric           | ForLoop_Int32  |       162.81 μs |      0.390 μs |      0.304 μs |      8.67 |    0.06 |         - |          NA |
| Hypergeometric      | ForLoop_Int32  |     1,035.42 μs |      2.315 μs |      2.166 μs |     55.16 |    0.41 |       1 B |          NA |
| Multinomial         | ForLoop_Int32  |     3,086.71 μs |      5.615 μs |      4.689 μs |    164.44 |    1.19 |       3 B |          NA |
| 'Negative Binomial' | ForLoop_Int32  |       498.13 μs |      1.114 μs |      1.042 μs |     26.54 |    0.20 |       1 B |          NA |
| Poisson             | ForLoop_Int32  |       391.87 μs |      3.169 μs |      2.964 μs |     20.88 |    0.21 |         - |          NA |
| Prime               | ForLoop_Int32  |    82,820.67 μs |    618.341 μs |    548.143 μs |  4,412.03 |   42.14 |     105 B |          NA |
| Uniform             | ForLoop_Int32  |        18.77 μs |      0.147 μs |      0.138 μs |      1.00 |    0.01 |         - |          NA |
| Zipf                | ForLoop_Int32  |       114.40 μs |      0.194 μs |      0.181 μs |      6.09 |    0.04 |         - |          NA |
|                     |                |                 |               |               |           |         |           |             |
| Binomial            | ForLoop_Int64  |     2,022.69 μs |      4.662 μs |      4.361 μs |     57.71 |    0.16 |       3 B |          NA |
| Chance.PickBetween  | ForLoop_Int64  |        34.82 μs |      0.095 μs |      0.089 μs |      0.99 |    0.00 |         - |          NA |
| Geometric           | ForLoop_Int64  |       162.95 μs |      0.498 μs |      0.466 μs |      4.65 |    0.02 |         - |          NA |
| Hypergeometric      | ForLoop_Int64  |     1,041.84 μs |      2.432 μs |      2.275 μs |     29.73 |    0.08 |       1 B |          NA |
| Multinomial         | ForLoop_Int64  |     3,086.25 μs |      6.272 μs |      5.867 μs |     88.06 |    0.23 |       3 B |          NA |
| 'Negative Binomial' | ForLoop_Int64  |       498.11 μs |      1.099 μs |      1.028 μs |     14.21 |    0.04 |       1 B |          NA |
| Poisson             | ForLoop_Int64  |       390.17 μs |      0.730 μs |      0.683 μs |     11.13 |    0.03 |         - |          NA |
| Prime               | ForLoop_Int64  |   416,891.82 μs |  2,800.281 μs |  2,482.376 μs | 11,894.84 |   72.04 |     736 B |          NA |
| Uniform             | ForLoop_Int64  |        35.05 μs |      0.074 μs |      0.069 μs |      1.00 |    0.00 |         - |          NA |
| Zipf                | ForLoop_Int64  |       114.16 μs |      0.174 μs |      0.163 μs |      3.26 |    0.01 |         - |          NA |
|                     |                |                 |               |               |           |         |           |             |
| Binomial            | ForLoop_Int128 |     2,057.00 μs |      5.344 μs |      4.999 μs |     16.62 |    0.05 |       3 B |          NA |
| Chance.PickBetween  | ForLoop_Int128 |       120.94 μs |      0.179 μs |      0.167 μs |      0.98 |    0.00 |         - |          NA |
| Geometric           | ForLoop_Int128 |       162.92 μs |      0.381 μs |      0.356 μs |      1.32 |    0.00 |         - |          NA |
| Hypergeometric      | ForLoop_Int128 |     1,108.05 μs |      2.795 μs |      2.615 μs |      8.95 |    0.03 |       1 B |          NA |
| Multinomial         | ForLoop_Int128 |     3,108.79 μs |      6.947 μs |      6.499 μs |     25.12 |    0.08 |       3 B |          NA |
| 'Negative Binomial' | ForLoop_Int128 |       508.67 μs |      3.553 μs |      3.323 μs |      4.11 |    0.03 |       1 B |          NA |
| Poisson             | ForLoop_Int128 |       389.99 μs |      0.646 μs |      0.573 μs |      3.15 |    0.01 |         - |          NA |
| Prime               | ForLoop_Int128 | 3,251,752.83 μs | 23,594.965 μs | 19,702.864 μs | 26,279.81 |  164.38 |     736 B |          NA |
| Uniform             | ForLoop_Int128 |       123.74 μs |      0.325 μs |      0.288 μs |      1.00 |    0.00 |         - |          NA |
| Zipf                | ForLoop_Int128 |       119.52 μs |      0.213 μs |      0.189 μs |      0.97 |    0.00 |         - |          NA |
|                     |                |                 |               |               |           |         |           |             |
| Binomial            | ForEach_Int32  |     2,048.79 μs |      2.368 μs |      2.215 μs |    102.18 |    0.16 |       3 B |          NA |
| Chance.PickBetween  | ForEach_Int32  |        18.59 μs |      0.032 μs |      0.030 μs |      0.93 |    0.00 |         - |          NA |
| Geometric           | ForEach_Int32  |       167.14 μs |      0.339 μs |      0.317 μs |      8.34 |    0.02 |         - |          NA |
| Hypergeometric      | ForEach_Int32  |     1,036.42 μs |      2.372 μs |      2.219 μs |     51.69 |    0.12 |       1 B |          NA |
| Multinomial         | ForEach_Int32  |     3,176.04 μs |      5.943 μs |      5.559 μs |    158.40 |    0.33 |       3 B |          NA |
| 'Negative Binomial' | ForEach_Int32  |       497.88 μs |      0.468 μs |      0.365 μs |     24.83 |    0.03 |       1 B |          NA |
| Poisson             | ForEach_Int32  |       410.46 μs |      1.299 μs |      1.215 μs |     20.47 |    0.06 |         - |          NA |
| Prime               | ForEach_Int32  |    80,438.31 μs |    203.015 μs |    189.901 μs |  4,011.81 |   10.38 |     105 B |          NA |
| Uniform             | ForEach_Int32  |        20.05 μs |      0.027 μs |      0.025 μs |      1.00 |    0.00 |         - |          NA |
| Zipf                | ForEach_Int32  |       131.45 μs |      0.084 μs |      0.066 μs |      6.56 |    0.01 |         - |          NA |
|                     |                |                 |               |               |           |         |           |             |
| Binomial            | ForEach_Int64  |     2,019.42 μs |      1.397 μs |      1.091 μs |     54.99 |    0.09 |       3 B |          NA |
| Chance.PickBetween  | ForEach_Int64  |        34.81 μs |      0.045 μs |      0.042 μs |      0.95 |    0.00 |         - |          NA |
| Geometric           | ForEach_Int64  |       164.84 μs |      0.242 μs |      0.226 μs |      4.49 |    0.01 |         - |          NA |
| Hypergeometric      | ForEach_Int64  |     1,041.72 μs |      1.577 μs |      1.475 μs |     28.37 |    0.06 |       1 B |          NA |
| Multinomial         | ForEach_Int64  |     3,172.46 μs |      3.185 μs |      2.979 μs |     86.39 |    0.15 |       3 B |          NA |
| 'Negative Binomial' | ForEach_Int64  |       498.24 μs |      0.582 μs |      0.516 μs |     13.57 |    0.02 |       1 B |          NA |
| Poisson             | ForEach_Int64  |       409.87 μs |      0.395 μs |      0.370 μs |     11.16 |    0.02 |         - |          NA |
| Prime               | ForEach_Int64  |   421,576.30 μs |  5,907.623 μs |  5,525.994 μs | 11,479.71 |  146.66 |     736 B |          NA |
| Uniform             | ForEach_Int64  |        36.72 μs |      0.059 μs |      0.055 μs |      1.00 |    0.00 |         - |          NA |
| Zipf                | ForEach_Int64  |       131.63 μs |      0.222 μs |      0.208 μs |      3.58 |    0.01 |         - |          NA |
|                     |                |                 |               |               |           |         |           |             |
| Binomial            | ForEach_Int128 |     2,059.48 μs |      3.006 μs |      2.812 μs |     16.16 |    0.02 |       3 B |          NA |
| Chance.PickBetween  | ForEach_Int128 |       120.83 μs |      0.111 μs |      0.104 μs |      0.95 |    0.00 |         - |          NA |
| Geometric           | ForEach_Int128 |       164.87 μs |      0.238 μs |      0.211 μs |      1.29 |    0.00 |         - |          NA |
| Hypergeometric      | ForEach_Int128 |     1,120.23 μs |      1.901 μs |      1.778 μs |      8.79 |    0.01 |       1 B |          NA |
| Multinomial         | ForEach_Int128 |     3,192.80 μs |      4.454 μs |      4.167 μs |     25.06 |    0.03 |       3 B |          NA |
| 'Negative Binomial' | ForEach_Int128 |       509.09 μs |      1.113 μs |      1.042 μs |      4.00 |    0.01 |       1 B |          NA |
| Poisson             | ForEach_Int128 |       410.00 μs |      0.492 μs |      0.460 μs |      3.22 |    0.00 |         - |          NA |
| Prime               | ForEach_Int128 | 3,251,642.85 μs | 20,357.456 μs | 18,046.355 μs | 25,519.69 |  137.31 |     736 B |          NA |
| Uniform             | ForEach_Int128 |       127.42 μs |      0.065 μs |      0.060 μs |      1.00 |    0.00 |         - |          NA |
| Zipf                | ForEach_Int128 |       128.05 μs |      0.097 μs |      0.090 μs |      1.00 |    0.00 |         - |          NA |
|                     |                |                 |               |               |           |         |           |             |
| Binomial            | Linq_Int32     |     2,023.82 μs |      2.211 μs |      1.847 μs |    101.39 |    0.18 |       3 B |          NA |
| Chance.PickBetween  | Linq_Int32     |        18.33 μs |      0.009 μs |      0.007 μs |      0.92 |    0.00 |         - |          NA |
| Geometric           | Linq_Int32     |       166.41 μs |      0.061 μs |      0.054 μs |      8.34 |    0.01 |         - |          NA |
| Hypergeometric      | Linq_Int32     |     1,147.06 μs |      0.804 μs |      0.752 μs |     57.47 |    0.10 |       1 B |          NA |
| Multinomial         | Linq_Int32     |     3,168.76 μs |      2.971 μs |      2.634 μs |    158.76 |    0.28 |     123 B |          NA |
| 'Negative Binomial' | Linq_Int32     |       498.90 μs |      0.454 μs |      0.425 μs |     25.00 |    0.04 |       1 B |          NA |
| Poisson             | Linq_Int32     |       390.27 μs |      0.323 μs |      0.287 μs |     19.55 |    0.03 |         - |          NA |
| Prime               | Linq_Int32     |    82,730.46 μs |    248.677 μs |    220.446 μs |  4,144.85 |   12.51 |     225 B |          NA |
| Uniform             | Linq_Int32     |        19.96 μs |      0.035 μs |      0.032 μs |      1.00 |    0.00 |         - |          NA |
| Zipf                | Linq_Int32     |       119.36 μs |      0.139 μs |      0.124 μs |      5.98 |    0.01 |         - |          NA |
|                     |                |                 |               |               |           |         |           |             |
| Binomial            | Linq_Int64     |     2,048.52 μs |      9.083 μs |      8.052 μs |     55.72 |    0.47 |       3 B |          NA |
| Chance.PickBetween  | Linq_Int64     |        35.06 μs |      0.333 μs |      0.312 μs |      0.95 |    0.01 |         - |          NA |
| Geometric           | Linq_Int64     |       166.34 μs |      0.263 μs |      0.234 μs |      4.52 |    0.03 |         - |          NA |
| Hypergeometric      | Linq_Int64     |     1,146.91 μs |      3.010 μs |      2.816 μs |     31.20 |    0.24 |       1 B |          NA |
| Multinomial         | Linq_Int64     |     3,170.12 μs |      5.473 μs |      4.851 μs |     86.23 |    0.66 |     123 B |          NA |
| 'Negative Binomial' | Linq_Int64     |       499.58 μs |      1.008 μs |      0.943 μs |     13.59 |    0.10 |       1 B |          NA |
| Poisson             | Linq_Int64     |       390.29 μs |      0.681 μs |      0.637 μs |     10.62 |    0.08 |         - |          NA |
| Prime               | Linq_Int64     |   418,898.73 μs |  3,261.212 μs |  2,890.979 μs | 11,394.43 |  114.21 |     856 B |          NA |
| Uniform             | Linq_Int64     |        36.77 μs |      0.303 μs |      0.284 μs |      1.00 |    0.01 |         - |          NA |
| Zipf                | Linq_Int64     |       117.22 μs |      0.198 μs |      0.186 μs |      3.19 |    0.02 |         - |          NA |
|                     |                |                 |               |               |           |         |           |             |
| Binomial            | Linq_Int128    |     2,179.23 μs |      5.915 μs |      5.533 μs |     14.76 |    0.06 |       3 B |          NA |
| Chance.PickBetween  | Linq_Int128    |       120.72 μs |      0.177 μs |      0.165 μs |      0.82 |    0.00 |         - |          NA |
| Geometric           | Linq_Int128    |       166.40 μs |      0.290 μs |      0.271 μs |      1.13 |    0.00 |         - |          NA |
| Hypergeometric      | Linq_Int128    |     1,254.35 μs |      2.785 μs |      2.605 μs |      8.49 |    0.03 |       1 B |          NA |
| Multinomial         | Linq_Int128    |     3,200.97 μs |      5.665 μs |      5.299 μs |     21.67 |    0.08 |     139 B |          NA |
| 'Negative Binomial' | Linq_Int128    |       529.29 μs |      0.444 μs |      0.347 μs |      3.58 |    0.01 |       1 B |          NA |
| Poisson             | Linq_Int128    |       390.20 μs |      0.612 μs |      0.573 μs |      2.64 |    0.01 |         - |          NA |
| Prime               | Linq_Int128    | 3,253,009.42 μs | 22,890.789 μs | 19,114.846 μs | 22,025.50 |  142.53 |     872 B |          NA |
| Uniform             | Linq_Int128    |       147.69 μs |      0.510 μs |      0.477 μs |      1.00 |    0.00 |         - |          NA |
| Zipf                | Linq_Int128    |       123.15 μs |      0.246 μs |      0.230 μs |      0.83 |    0.00 |         - |          NA |

// * Hints *
Outliers
  SamplerDiscreteBenchmarks.Geometric: MinIterationTime=500ms, IterationCount=15           -> 3 outliers were removed (164.60 μs..166.38 μs)
  SamplerDiscreteBenchmarks.Multinomial: MinIterationTime=500ms, IterationCount=15         -> 2 outliers were removed (3.11 ms, 3.13 ms)
  SamplerDiscreteBenchmarks.Prime: MinIterationTime=500ms, IterationCount=15               -> 1 outlier  was  removed (101.81 ms)
  SamplerDiscreteBenchmarks.Prime: MinIterationTime=500ms, IterationCount=15               -> 1 outlier  was  removed (427.48 ms)
  SamplerDiscreteBenchmarks.Poisson: MinIterationTime=500ms, IterationCount=15             -> 1 outlier  was  removed (393.33 μs)
  SamplerDiscreteBenchmarks.Prime: MinIterationTime=500ms, IterationCount=15               -> 2 outliers were removed, 4 outliers were detected (3.21 s, 3.22 s, 3.30 s, 3.34 s)
  SamplerDiscreteBenchmarks.Uniform: MinIterationTime=500ms, IterationCount=15             -> 1 outlier  was  removed (125.19 μs)
  SamplerDiscreteBenchmarks.Zipf: MinIterationTime=500ms, IterationCount=15                -> 1 outlier  was  removed (120.06 μs)
  SamplerDiscreteBenchmarks.'Negative Binomial': MinIterationTime=500ms, IterationCount=15 -> 3 outliers were removed (500.15 μs..500.95 μs)
  SamplerDiscreteBenchmarks.Zipf: MinIterationTime=500ms, IterationCount=15                -> 3 outliers were removed (131.89 μs..131.94 μs)
  SamplerDiscreteBenchmarks.Binomial: MinIterationTime=500ms, IterationCount=15            -> 3 outliers were removed (2.03 ms..2.03 ms)
  SamplerDiscreteBenchmarks.'Negative Binomial': MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  removed (499.48 μs)
  SamplerDiscreteBenchmarks.Geometric: MinIterationTime=500ms, IterationCount=15           -> 1 outlier  was  removed (165.89 μs)
  SamplerDiscreteBenchmarks.Prime: MinIterationTime=500ms, IterationCount=15               -> 1 outlier  was  removed, 3 outliers were detected (3.21 s, 3.22 s, 3.34 s)
  SamplerDiscreteBenchmarks.Binomial: MinIterationTime=500ms, IterationCount=15            -> 2 outliers were removed (2.03 ms, 2.04 ms)
  SamplerDiscreteBenchmarks.Chance.PickBetween: MinIterationTime=500ms, IterationCount=15  -> 3 outliers were removed (18.38 μs..18.54 μs)
  SamplerDiscreteBenchmarks.Geometric: MinIterationTime=500ms, IterationCount=15           -> 1 outlier  was  removed (166.62 μs)
  SamplerDiscreteBenchmarks.Multinomial: MinIterationTime=500ms, IterationCount=15         -> 1 outlier  was  removed (3.19 ms)
  SamplerDiscreteBenchmarks.Poisson: MinIterationTime=500ms, IterationCount=15             -> 1 outlier  was  removed (398.36 μs)
  SamplerDiscreteBenchmarks.Prime: MinIterationTime=500ms, IterationCount=15               -> 1 outlier  was  removed (83.39 ms)
  SamplerDiscreteBenchmarks.Zipf: MinIterationTime=500ms, IterationCount=15                -> 1 outlier  was  removed (120.00 μs)
  SamplerDiscreteBenchmarks.Binomial: MinIterationTime=500ms, IterationCount=15            -> 1 outlier  was  removed, 3 outliers were detected (2.02 ms, 2.04 ms, 2.06 ms)
  SamplerDiscreteBenchmarks.Geometric: MinIterationTime=500ms, IterationCount=15           -> 1 outlier  was  removed (167.13 μs)
  SamplerDiscreteBenchmarks.Multinomial: MinIterationTime=500ms, IterationCount=15         -> 1 outlier  was  removed (3.22 ms)
  SamplerDiscreteBenchmarks.Prime: MinIterationTime=500ms, IterationCount=15               -> 1 outlier  was  removed (429.72 ms)
  SamplerDiscreteBenchmarks.'Negative Binomial': MinIterationTime=500ms, IterationCount=15 -> 3 outliers were removed (531.17 μs..532.28 μs)
  SamplerDiscreteBenchmarks.Prime: MinIterationTime=500ms, IterationCount=15               -> 2 outliers were removed, 4 outliers were detected (3.21 s, 3.22 s, 3.31 s, 3.34 s)
  SamplerDiscreteBenchmarks.Uniform: MinIterationTime=500ms, IterationCount=15             -> 1 outlier  was  detected (146.57 μs)

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
Run time: 00:28:27 (1707.23 sec), executed benchmarks: 90

*/