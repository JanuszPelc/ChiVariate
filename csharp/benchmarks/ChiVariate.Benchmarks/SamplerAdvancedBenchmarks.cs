using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Benchmarks;

/// <summary>
///     Benchmarks for advanced continuous distributions that use composition of simpler distributions,
///     multivariate outputs, or quasi-random sequences.
///     Includes: Beta, Chi, ChiSquared, Dirichlet, F, Gamma, Halton, InverseGamma, LogNormal,
///     MultivariateNormal, Sobol, Spatial, StudentT, Wishart.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
[MinIterationTime(500)]
[IterationCount(15)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams)]
public class SamplerAdvancedBenchmarks
{
    #region Benchmarks

    [Benchmark(Description = "Beta")]
    public bool ChiVariateBeta()
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
            var sampler = _rng.Beta(T.CreateChecked(2.0), T.CreateChecked(5.0));
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

    [Benchmark(Description = "Chi")]
    public bool ChiVariateChi()
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
            var sampler = _rng.Chi(T.CreateChecked(5.0));
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

    [Benchmark(Description = "Chi-Squared")]
    public bool ChiVariateChiSquared()
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
            var sampler = _rng.ChiSquared(T.CreateChecked(5.0));
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

    [Benchmark(Description = "Dirichlet")]
    public bool ChiVariateDirichlet()
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
            var alpha = (ReadOnlySpan<T>)TypedAlpha<T>.Data;
            var sampler = _rng.Dirichlet(alpha);
            var sum = T.Zero;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                    {
                        using var sample = sampler.Sample();
                        sum += sample[0];
                    }

                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var sample in sampler.Sample(SampleCount))
                    {
                        sum += sample[0];
                        sample.Dispose();
                    }

                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount)
                        .Select(sample =>
                        {
                            using (sample)
                            {
                                return sample[0];
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

    [Benchmark(Description = "F")]
    public bool ChiVariateF()
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
            var sampler = _rng.F(T.CreateChecked(5.0), T.CreateChecked(10.0));
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

    [Benchmark(Description = "Gamma", Baseline = true)]
    public bool ChiVariateGamma()
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
            var sampler = _rng.Gamma(T.CreateChecked(2.5), T.CreateChecked(1.5));
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

    [Benchmark(Description = "Halton")]
    public bool ChiVariateHalton()
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
            var sampler = _rng.Halton(4).OfType<T>();
            var sum = T.Zero;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                    {
                        using var point = sampler.Sample();
                        sum += point[0];
                    }

                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var point in sampler.Sample(SampleCount))
                        using (point)
                        {
                            sum += point[0];
                        }

                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount)
                        .Select(point =>
                        {
                            using (point)
                            {
                                return point[0];
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

    [Benchmark(Description = "Inverse-Gamma")]
    public bool ChiVariateInverseGamma()
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
            var sampler = _rng.InverseGamma(T.CreateChecked(3.0), T.CreateChecked(2.0));
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

    [Benchmark(Description = "Log-Normal")]
    public bool ChiVariateLogNormal()
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
            var sampler = _rng.LogNormal(T.Zero, T.One);
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

    [Benchmark(Description = "Multivariate Normal")]
    public bool ChiVariateMultivariateNormal()
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
            var meanVector = ChiMatrix.Zeros<T>(3, 1);
            var covarianceMatrix = ChiMatrix.With(ScaleMatrix<T>.Data);
            var sampler = _rng.MultivariateNormal(meanVector, covarianceMatrix);
            var sum = T.Zero;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                    {
                        using var sample = sampler.Sample();
                        sum += sample[0];
                    }

                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var sample in sampler.Sample(SampleCount))
                    {
                        sum += sample[0];
                        sample.Dispose();
                    }

                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount)
                        .Select(sample =>
                        {
                            using (sample)
                            {
                                return sample[0];
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

    [Benchmark(Description = "Sobol")]
    public bool ChiVariateSobol()
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
            var sampler = _rng.Sobol(4).OfType<T>();
            var sum = T.Zero;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                    {
                        using var point = sampler.Sample();
                        sum += point[0];
                    }

                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var point in sampler.Sample(SampleCount))
                        using (point)
                        {
                            sum += point[0];
                        }

                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount)
                        .Select(point =>
                        {
                            using (point)
                            {
                                return point[0];
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

    [Benchmark(Description = "Spatial.OnSphere")]
    public bool ChiVariateSpatial()
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
            var sampler = _rng.Spatial().OnSphere(T.One);
            var sum = T.Zero;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                        sum += sampler.Sample().X;
                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var sample in sampler.Sample(SampleCount))
                        sum += sample.X;
                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount).Select(sample => sample.X);
                    sum = SumValues(enumerable);
                    break;
                }
                default: throw new UnreachableException();
            }

            return Consume(sum);
        }
    }

    [Benchmark(Description = "StudentT")]
    public bool ChiVariateStudentT()
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
            var sampler = _rng.StudentT(T.CreateChecked(5.0));
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

    [Benchmark(Description = "Wishart")]
    public bool ChiVariateWishart()
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
            var scaleMatrix = ChiMatrix.With(ScaleMatrix<T>.Data);
            var dimension = scaleMatrix.RowCount;
            var sampler = _rng.Wishart(dimension, scaleMatrix);
            var sum = T.Zero;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                        sum += sampler.Sample()[0, 0];
                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var matrix in sampler.Sample(SampleCount))
                        sum += matrix[0, 0];
                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount);
                    sum = enumerable.Aggregate(T.Zero, (current, matrix) => current + matrix[0, 0]);
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

        throw new UnreachableException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T SumValues<T>(IEnumerable<T> enumerable)
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

        throw new UnreachableException();
    }

    private static bool Consume<T>(T value) where T : INumberBase<T>
    {
        if (value.GetHashCode() == Environment.TickCount)
            Console.WriteLine(value);
        return true;
    }

    private static class ScaleMatrix<T>
        where T : unmanaged, IFloatingPoint<T>
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly double[,] Template =
        {
            { 0.6, 0.2, 0.2 },
            { 0.2, 0.6, 0.2 },
            { 0.2, 0.2, 0.6 }
        };

        public static T[,] Data { get; } = Initialize();

        private static T[,] Initialize()
        {
            var dimension = Template.GetLength(0);
            var data = new T[dimension, dimension];
            for (var i = 0; i < dimension; i++)
            for (var j = 0; j < dimension; j++)
                data[i, j] = T.CreateChecked(Template[i, j]);
            return data;
        }
    }

    private static class TypedAlpha<T>
        where T : unmanaged, IFloatingPoint<T>
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly double[] Template = [0.1, 1.0, 10.0, 5.0, 0.5];

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

| Method                | Param           | Mean         | Error      | StdDev     | Median       | Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------- |---------------- |-------------:|-----------:|-----------:|-------------:|------:|--------:|----------:|------------:|
| Beta                  | ForLoop_Float   |    179.47 μs |   0.302 μs |   0.283 μs |    179.34 μs |  1.98 |    0.01 |         - |          NA |
| Chi                   | ForLoop_Float   |     90.03 μs |   0.154 μs |   0.144 μs |     89.95 μs |  1.00 |    0.01 |         - |          NA |
| Chi-Squared           | ForLoop_Float   |     89.79 μs |   0.162 μs |   0.152 μs |     89.75 μs |  0.99 |    0.01 |         - |          NA |
| Dirichlet             | ForLoop_Float   |    623.52 μs |   1.082 μs |   1.012 μs |    623.18 μs |  6.90 |    0.05 |       1 B |          NA |
| F                     | ForLoop_Float   |    179.55 μs |   0.277 μs |   0.260 μs |    179.42 μs |  1.99 |    0.01 |         - |          NA |
| Gamma                 | ForLoop_Float   |     90.42 μs |   0.738 μs |   0.691 μs |     90.78 μs |  1.00 |    0.01 |         - |          NA |
| Halton                | ForLoop_Float   |     77.79 μs |   0.146 μs |   0.136 μs |     77.72 μs |  0.86 |    0.01 |         - |          NA |
| Inverse-Gamma         | ForLoop_Float   |     89.50 μs |   0.170 μs |   0.159 μs |     89.43 μs |  0.99 |    0.01 |         - |          NA |
| Log-Normal            | ForLoop_Float   |     55.81 μs |   0.090 μs |   0.070 μs |     55.78 μs |  0.62 |    0.00 |         - |          NA |
| 'Multivariate Normal' | ForLoop_Float   |    325.76 μs |   0.790 μs |   0.660 μs |    325.64 μs |  3.60 |    0.03 |         - |          NA |
| Sobol                 | ForLoop_Float   |    120.08 μs |   3.238 μs |   3.029 μs |    120.94 μs |  1.33 |    0.03 |         - |          NA |
| Spatial.OnSphere      | ForLoop_Float   |     53.28 μs |   0.069 μs |   0.065 μs |     53.27 μs |  0.59 |    0.00 |         - |          NA |
| StudentT              | ForLoop_Float   |    144.24 μs |   0.324 μs |   0.303 μs |    144.12 μs |  1.60 |    0.01 |         - |          NA |
| Wishart               | ForLoop_Float   |    940.68 μs |   1.475 μs |   1.380 μs |    940.90 μs | 10.40 |    0.08 |       1 B |          NA |
|                       |                 |              |            |            |              |       |         |           |             |
| Beta                  | ForLoop_Double  |    236.88 μs |   0.407 μs |   0.381 μs |    236.83 μs |  2.01 |    0.00 |         - |          NA |
| Chi                   | ForLoop_Double  |    118.92 μs |   0.864 μs |   0.808 μs |    118.45 μs |  1.01 |    0.01 |         - |          NA |
| Chi-Squared           | ForLoop_Double  |    120.00 μs |   0.162 μs |   0.151 μs |    120.03 μs |  1.02 |    0.00 |         - |          NA |
| Dirichlet             | ForLoop_Double  |    849.66 μs |   1.252 μs |   1.171 μs |    849.41 μs |  7.19 |    0.01 |         - |          NA |
| F                     | ForLoop_Double  |    239.04 μs |   0.298 μs |   0.264 μs |    239.08 μs |  2.02 |    0.00 |         - |          NA |
| Gamma                 | ForLoop_Double  |    118.12 μs |   0.203 μs |   0.180 μs |    118.10 μs |  1.00 |    0.00 |         - |          NA |
| Halton                | ForLoop_Double  |     76.34 μs |   0.175 μs |   0.164 μs |     76.29 μs |  0.65 |    0.00 |         - |          NA |
| Inverse-Gamma         | ForLoop_Double  |    118.11 μs |   0.210 μs |   0.197 μs |    118.14 μs |  1.00 |    0.00 |         - |          NA |
| Log-Normal            | ForLoop_Double  |     73.70 μs |   0.616 μs |   0.576 μs |     73.61 μs |  0.62 |    0.00 |         - |          NA |
| 'Multivariate Normal' | ForLoop_Double  |    427.79 μs |   0.474 μs |   0.420 μs |    427.82 μs |  3.62 |    0.01 |         - |          NA |
| Sobol                 | ForLoop_Double  |    105.95 μs |   1.053 μs |   0.985 μs |    106.07 μs |  0.90 |    0.01 |         - |          NA |
| Spatial.OnSphere      | ForLoop_Double  |     94.62 μs |   0.644 μs |   0.602 μs |     94.80 μs |  0.80 |    0.01 |         - |          NA |
| StudentT              | ForLoop_Double  |    188.68 μs |   0.365 μs |   0.341 μs |    188.65 μs |  1.60 |    0.00 |         - |          NA |
| Wishart               | ForLoop_Double  |  1,231.95 μs |   3.658 μs |   3.422 μs |  1,230.58 μs | 10.43 |    0.03 |       1 B |          NA |
|                       |                 |              |            |            |              |       |         |           |             |
| Beta                  | ForLoop_Decimal |  5,139.35 μs |  19.016 μs |  17.787 μs |  5,137.39 μs |  1.92 |    0.01 |       6 B |        2.00 |
| Chi                   | ForLoop_Decimal |  3,584.80 μs |  26.386 μs |  24.681 μs |  3,574.16 μs |  1.34 |    0.01 |       3 B |        1.00 |
| Chi-Squared           | ForLoop_Decimal |  2,634.68 μs |  10.456 μs |   9.781 μs |  2,631.20 μs |  0.98 |    0.00 |       3 B |        1.00 |
| Dirichlet             | ForLoop_Decimal | 15,979.87 μs |  38.490 μs |  32.141 μs | 15,983.63 μs |  5.97 |    0.02 |      23 B |        7.67 |
| F                     | ForLoop_Decimal |  5,356.42 μs |  14.099 μs |  13.189 μs |  5,357.08 μs |  2.00 |    0.01 |       6 B |        2.00 |
| Gamma                 | ForLoop_Decimal |  2,678.13 μs |   8.575 μs |   8.021 μs |  2,676.82 μs |  1.00 |    0.00 |       3 B |        1.00 |
| Halton                | ForLoop_Decimal |  1,088.37 μs |   1.480 μs |   1.384 μs |  1,088.56 μs |  0.41 |    0.00 |       1 B |        0.33 |
| Inverse-Gamma         | ForLoop_Decimal |  2,760.58 μs |   7.249 μs |   6.426 μs |  2,760.48 μs |  1.03 |    0.00 |       3 B |        1.00 |
| Log-Normal            | ForLoop_Decimal |  2,519.89 μs |   5.092 μs |   4.514 μs |  2,518.46 μs |  0.94 |    0.00 |       3 B |        1.00 |
| 'Multivariate Normal' | ForLoop_Decimal |  1,620.33 μs |   3.465 μs |   3.242 μs |  1,619.41 μs |  0.61 |    0.00 |       1 B |        0.33 |
| Sobol                 | ForLoop_Decimal |    110.46 μs |   0.241 μs |   0.225 μs |    110.52 μs |  0.04 |    0.00 |         - |        0.00 |
| Spatial.OnSphere      | ForLoop_Decimal |  2,021.35 μs |   6.472 μs |   6.054 μs |  2,018.29 μs |  0.75 |    0.00 |       3 B |        1.00 |
| StudentT              | ForLoop_Decimal |  4,118.39 μs |  15.993 μs |  14.960 μs |  4,113.92 μs |  1.54 |    0.01 |       6 B |        2.00 |
| Wishart               | ForLoop_Decimal | 19,051.66 μs |  61.758 μs |  57.769 μs | 19,031.11 μs |  7.11 |    0.03 |      23 B |        7.67 |
|                       |                 |              |            |            |              |       |         |           |             |
| Beta                  | ForLoop_Fixed   |    235.04 μs |   0.519 μs |   0.485 μs |    235.26 μs |  2.06 |    0.01 |         - |          NA |
| Chi                   | ForLoop_Fixed   |    117.63 μs |   0.222 μs |   0.208 μs |    117.63 μs |  1.03 |    0.00 |         - |          NA |
| Chi-Squared           | ForLoop_Fixed   |    115.18 μs |   0.913 μs |   0.854 μs |    114.77 μs |  1.01 |    0.01 |         - |          NA |
| Dirichlet             | ForLoop_Fixed   |    759.26 μs |   1.828 μs |   1.710 μs |    758.26 μs |  6.66 |    0.02 |       1 B |          NA |
| F                     | ForLoop_Fixed   |    237.17 μs |   0.510 μs |   0.477 μs |    237.14 μs |  2.08 |    0.01 |         - |          NA |
| Gamma                 | ForLoop_Fixed   |    113.99 μs |   0.263 μs |   0.246 μs |    114.09 μs |  1.00 |    0.00 |         - |          NA |
| Halton                | ForLoop_Fixed   |    205.41 μs |   0.531 μs |   0.497 μs |    205.20 μs |  1.80 |    0.01 |         - |          NA |
| Inverse-Gamma         | ForLoop_Fixed   |    119.47 μs |   0.237 μs |   0.222 μs |    119.58 μs |  1.05 |    0.00 |         - |          NA |
| Log-Normal            | ForLoop_Fixed   |     73.19 μs |   0.646 μs |   0.604 μs |     73.38 μs |  0.64 |    0.01 |         - |          NA |
| 'Multivariate Normal' | ForLoop_Fixed   |    399.65 μs |   0.885 μs |   0.828 μs |    399.34 μs |  3.51 |    0.01 |         - |          NA |
| Sobol                 | ForLoop_Fixed   |    238.28 μs |   0.498 μs |   0.466 μs |    238.39 μs |  2.09 |    0.01 |         - |          NA |
| Spatial.OnSphere      | ForLoop_Fixed   |     83.14 μs |   0.401 μs |   0.356 μs |     82.92 μs |  0.73 |    0.00 |         - |          NA |
| StudentT              | ForLoop_Fixed   |    186.19 μs |   0.494 μs |   0.462 μs |    186.11 μs |  1.63 |    0.01 |         - |          NA |
| Wishart               | ForLoop_Fixed   |  1,172.47 μs |   5.233 μs |   4.895 μs |  1,172.84 μs | 10.29 |    0.05 |       1 B |          NA |
|                       |                 |              |            |            |              |       |         |           |             |
| Beta                  | ForEach_Float   |    183.68 μs |   0.550 μs |   0.515 μs |    183.59 μs |  1.96 |    0.01 |         - |          NA |
| Chi                   | ForEach_Float   |     94.01 μs |   0.306 μs |   0.286 μs |     93.79 μs |  1.00 |    0.00 |         - |          NA |
| Chi-Squared           | ForEach_Float   |     94.14 μs |   0.237 μs |   0.222 μs |     94.05 μs |  1.00 |    0.00 |         - |          NA |
| Dirichlet             | ForEach_Float   |    712.51 μs |   2.173 μs |   2.032 μs |    711.26 μs |  7.59 |    0.03 |       1 B |          NA |
| F                     | ForEach_Float   |    183.66 μs |   0.568 μs |   0.443 μs |    183.39 μs |  1.96 |    0.01 |         - |          NA |
| Gamma                 | ForEach_Float   |     93.88 μs |   0.244 μs |   0.229 μs |     93.76 μs |  1.00 |    0.00 |         - |          NA |
| Halton                | ForEach_Float   |    168.13 μs |   0.604 μs |   0.565 μs |    167.88 μs |  1.79 |    0.01 |         - |          NA |
| Inverse-Gamma         | ForEach_Float   |     93.83 μs |   0.251 μs |   0.235 μs |     93.70 μs |  1.00 |    0.00 |         - |          NA |
| Log-Normal            | ForEach_Float   |     64.48 μs |   0.229 μs |   0.215 μs |     64.38 μs |  0.69 |    0.00 |         - |          NA |
| 'Multivariate Normal' | ForEach_Float   |    330.84 μs |   1.627 μs |   1.522 μs |    329.77 μs |  3.52 |    0.02 |         - |          NA |
| Sobol                 | ForEach_Float   |    208.32 μs |   1.049 μs |   0.981 μs |    208.23 μs |  2.22 |    0.01 |         - |          NA |
| Spatial.OnSphere      | ForEach_Float   |     57.00 μs |   0.169 μs |   0.150 μs |     57.03 μs |  0.61 |    0.00 |         - |          NA |
| StudentT              | ForEach_Float   |    150.54 μs |   0.529 μs |   0.495 μs |    150.31 μs |  1.60 |    0.01 |         - |          NA |
| Wishart               | ForEach_Float   |    954.61 μs |   2.429 μs |   2.273 μs |    954.84 μs | 10.17 |    0.03 |       1 B |          NA |
|                       |                 |              |            |            |              |       |         |           |             |
| Beta                  | ForEach_Double  |    242.17 μs |   0.680 μs |   0.636 μs |    242.27 μs |  1.98 |    0.01 |         - |          NA |
| Chi                   | ForEach_Double  |    121.95 μs |   0.268 μs |   0.251 μs |    122.12 μs |  1.00 |    0.00 |         - |          NA |
| Chi-Squared           | ForEach_Double  |    122.19 μs |   0.471 μs |   0.441 μs |    121.99 μs |  1.00 |    0.01 |         - |          NA |
| Dirichlet             | ForEach_Double  |    924.49 μs |   2.086 μs |   1.952 μs |    925.15 μs |  7.55 |    0.03 |       1 B |          NA |
| F                     | ForEach_Double  |    241.58 μs |   1.832 μs |   1.713 μs |    240.91 μs |  1.97 |    0.02 |         - |          NA |
| Gamma                 | ForEach_Double  |    122.39 μs |   0.516 μs |   0.482 μs |    122.15 μs |  1.00 |    0.01 |         - |          NA |
| Halton                | ForEach_Double  |    166.11 μs |   0.553 μs |   0.518 μs |    165.76 μs |  1.36 |    0.01 |         - |          NA |
| Inverse-Gamma         | ForEach_Double  |    121.42 μs |   0.294 μs |   0.275 μs |    121.49 μs |  0.99 |    0.00 |         - |          NA |
| Log-Normal            | ForEach_Double  |     76.23 μs |   0.201 μs |   0.188 μs |     76.13 μs |  0.62 |    0.00 |         - |          NA |
| 'Multivariate Normal' | ForEach_Double  |    442.72 μs |   1.079 μs |   1.009 μs |    442.66 μs |  3.62 |    0.02 |         - |          NA |
| Sobol                 | ForEach_Double  |    201.50 μs |   1.262 μs |   1.180 μs |    201.67 μs |  1.65 |    0.01 |         - |          NA |
| Spatial.OnSphere      | ForEach_Double  |     99.01 μs |   0.907 μs |   0.849 μs |     98.92 μs |  0.81 |    0.01 |         - |          NA |
| StudentT              | ForEach_Double  |    197.58 μs |   2.853 μs |   2.669 μs |    197.29 μs |  1.61 |    0.02 |         - |          NA |
| Wishart               | ForEach_Double  |  1,290.12 μs |  17.554 μs |  16.420 μs |  1,294.69 μs | 10.54 |    0.14 |       1 B |          NA |
|                       |                 |              |            |            |              |       |         |           |             |
| Beta                  | ForEach_Decimal |  5,232.94 μs |  26.206 μs |  24.513 μs |  5,222.91 μs |  1.98 |    0.01 |       6 B |        2.00 |
| Chi                   | ForEach_Decimal |  3,627.35 μs |  17.049 μs |  15.947 μs |  3,623.90 μs |  1.37 |    0.01 |       3 B |        1.00 |
| Chi-Squared           | ForEach_Decimal |  2,632.06 μs |  10.572 μs |   9.889 μs |  2,632.04 μs |  1.00 |    0.01 |       3 B |        1.00 |
| Dirichlet             | ForEach_Decimal | 16,129.02 μs |  81.525 μs |  76.258 μs | 16,108.12 μs |  6.11 |    0.04 |      23 B |        7.67 |
| F                     | ForEach_Decimal |  5,373.64 μs |  27.127 μs |  25.374 μs |  5,370.03 μs |  2.04 |    0.01 |       6 B |        2.00 |
| Gamma                 | ForEach_Decimal |  2,639.94 μs |  11.273 μs |  10.545 μs |  2,636.31 μs |  1.00 |    0.01 |       3 B |        1.00 |
| Halton                | ForEach_Decimal |  1,151.46 μs |   4.684 μs |   4.382 μs |  1,152.16 μs |  0.44 |    0.00 |       1 B |        0.33 |
| Inverse-Gamma         | ForEach_Decimal |  2,729.86 μs |  13.438 μs |  12.570 μs |  2,730.08 μs |  1.03 |    0.01 |       3 B |        1.00 |
| Log-Normal            | ForEach_Decimal |  2,491.81 μs |  12.415 μs |  11.613 μs |  2,487.94 μs |  0.94 |    0.01 |       3 B |        1.00 |
| 'Multivariate Normal' | ForEach_Decimal |  1,646.64 μs |   5.677 μs |   5.311 μs |  1,643.89 μs |  0.62 |    0.00 |       1 B |        0.33 |
| Sobol                 | ForEach_Decimal |    201.33 μs |   0.658 μs |   0.616 μs |    201.32 μs |  0.08 |    0.00 |         - |        0.00 |
| Spatial.OnSphere      | ForEach_Decimal |  2,024.68 μs |   7.256 μs |   6.788 μs |  2,020.92 μs |  0.77 |    0.00 |       3 B |        1.00 |
| StudentT              | ForEach_Decimal |  4,128.67 μs |  22.302 μs |  20.861 μs |  4,118.25 μs |  1.56 |    0.01 |       6 B |        2.00 |
| Wishart               | ForEach_Decimal | 19,150.04 μs | 128.890 μs | 120.564 μs | 19,141.28 μs |  7.25 |    0.05 |      23 B |        7.67 |
|                       |                 |              |            |            |              |       |         |           |             |
| Beta                  | ForEach_Fixed   |    237.81 μs |   2.576 μs |   2.410 μs |    236.35 μs |  2.05 |    0.02 |         - |          NA |
| Chi                   | ForEach_Fixed   |    118.88 μs |   0.341 μs |   0.302 μs |    118.83 μs |  1.02 |    0.00 |         - |          NA |
| Chi-Squared           | ForEach_Fixed   |    115.07 μs |   0.352 μs |   0.329 μs |    114.96 μs |  0.99 |    0.00 |         - |          NA |
| Dirichlet             | ForEach_Fixed   |    856.98 μs |   5.369 μs |   5.022 μs |    855.85 μs |  7.37 |    0.04 |       1 B |          NA |
| F                     | ForEach_Fixed   |    240.78 μs |   0.446 μs |   0.395 μs |    240.71 μs |  2.07 |    0.01 |         - |          NA |
| Gamma                 | ForEach_Fixed   |    116.24 μs |   0.261 μs |   0.244 μs |    116.32 μs |  1.00 |    0.00 |         - |          NA |
| Halton                | ForEach_Fixed   |    274.66 μs |   1.053 μs |   0.985 μs |    274.84 μs |  2.36 |    0.01 |         - |          NA |
| Inverse-Gamma         | ForEach_Fixed   |    122.21 μs |   0.437 μs |   0.409 μs |    121.99 μs |  1.05 |    0.00 |         - |          NA |
| Log-Normal            | ForEach_Fixed   |     76.08 μs |   0.491 μs |   0.436 μs |     75.88 μs |  0.65 |    0.00 |         - |          NA |
| 'Multivariate Normal' | ForEach_Fixed   |    424.52 μs |   1.331 μs |   1.245 μs |    424.14 μs |  3.65 |    0.01 |         - |          NA |
| Sobol                 | ForEach_Fixed   |    328.23 μs |   0.944 μs |   0.788 μs |    328.26 μs |  2.82 |    0.01 |         - |          NA |
| Spatial.OnSphere      | ForEach_Fixed   |     84.53 μs |   0.091 μs |   0.080 μs |     84.53 μs |  0.73 |    0.00 |         - |          NA |
| StudentT              | ForEach_Fixed   |    193.11 μs |   0.409 μs |   0.363 μs |    193.24 μs |  1.66 |    0.00 |         - |          NA |
| Wishart               | ForEach_Fixed   |  1,206.46 μs |   3.326 μs |   2.949 μs |  1,206.06 μs | 10.38 |    0.03 |       1 B |          NA |
|                       |                 |              |            |            |              |       |         |           |             |
| Beta                  | Linq_Float      |    185.81 μs |   0.429 μs |   0.381 μs |    185.87 μs |  1.97 |    0.00 |         - |          NA |
| Chi                   | Linq_Float      |     95.04 μs |   0.417 μs |   0.390 μs |     95.11 μs |  1.01 |    0.00 |         - |          NA |
| Chi-Squared           | Linq_Float      |     93.96 μs |   0.143 μs |   0.127 μs |     93.98 μs |  1.00 |    0.00 |         - |          NA |
| Dirichlet             | Linq_Float      |    761.99 μs |   1.202 μs |   1.066 μs |    762.00 μs |  8.07 |    0.02 |     121 B |          NA |
| F                     | Linq_Float      |    184.98 μs |   0.310 μs |   0.275 μs |    184.97 μs |  1.96 |    0.00 |         - |          NA |
| Gamma                 | Linq_Float      |     94.42 μs |   0.148 μs |   0.139 μs |     94.43 μs |  1.00 |    0.00 |         - |          NA |
| Halton                | Linq_Float      |    171.14 μs |   0.346 μs |   0.324 μs |    171.14 μs |  1.81 |    0.00 |     120 B |          NA |
| Inverse-Gamma         | Linq_Float      |     94.39 μs |   0.206 μs |   0.183 μs |     94.35 μs |  1.00 |    0.00 |         - |          NA |
| Log-Normal            | Linq_Float      |     60.25 μs |   0.103 μs |   0.096 μs |     60.26 μs |  0.64 |    0.00 |         - |          NA |
| 'Multivariate Normal' | Linq_Float      |    339.52 μs |   0.410 μs |   0.364 μs |    339.49 μs |  3.60 |    0.01 |     120 B |          NA |
| Sobol                 | Linq_Float      |    195.37 μs |   1.302 μs |   1.218 μs |    195.23 μs |  2.07 |    0.01 |     120 B |          NA |
| Spatial.OnSphere      | Linq_Float      |     57.52 μs |   0.129 μs |   0.108 μs |     57.52 μs |  0.61 |    0.00 |     120 B |          NA |
| StudentT              | Linq_Float      |    148.92 μs |   0.234 μs |   0.207 μs |    148.93 μs |  1.58 |    0.00 |         - |          NA |
| Wishart               | Linq_Float      |    959.23 μs |   3.661 μs |   3.425 μs |    959.25 μs | 10.16 |    0.04 |      65 B |          NA |
|                       |                 |              |            |            |              |       |         |           |             |
| Beta                  | Linq_Double     |    242.29 μs |   0.408 μs |   0.381 μs |    242.23 μs |  1.98 |    0.01 |         - |          NA |
| Chi                   | Linq_Double     |    122.91 μs |   0.264 μs |   0.247 μs |    122.88 μs |  1.00 |    0.00 |         - |          NA |
| Chi-Squared           | Linq_Double     |    122.35 μs |   0.335 μs |   0.313 μs |    122.19 μs |  1.00 |    0.00 |         - |          NA |
| Dirichlet             | Linq_Double     |    958.34 μs |   1.282 μs |   1.136 μs |    958.06 μs |  7.82 |    0.02 |     121 B |          NA |
| F                     | Linq_Double     |    241.35 μs |   0.514 μs |   0.481 μs |    241.35 μs |  1.97 |    0.01 |         - |          NA |
| Gamma                 | Linq_Double     |    122.47 μs |   0.363 μs |   0.339 μs |    122.40 μs |  1.00 |    0.00 |         - |          NA |
| Halton                | Linq_Double     |    170.61 μs |   0.651 μs |   0.609 μs |    170.37 μs |  1.39 |    0.01 |     120 B |          NA |
| Inverse-Gamma         | Linq_Double     |    122.51 μs |   1.111 μs |   0.928 μs |    122.24 μs |  1.00 |    0.01 |         - |          NA |
| Log-Normal            | Linq_Double     |     77.77 μs |   0.414 μs |   0.367 μs |     77.59 μs |  0.63 |    0.00 |         - |          NA |
| 'Multivariate Normal' | Linq_Double     |    460.05 μs |   4.326 μs |   3.613 μs |    459.45 μs |  3.76 |    0.03 |     120 B |          NA |
| Sobol                 | Linq_Double     |    177.48 μs |   2.912 μs |   2.724 μs |    177.94 μs |  1.45 |    0.02 |     120 B |          NA |
| Spatial.OnSphere      | Linq_Double     |     97.51 μs |   0.177 μs |   0.165 μs |     97.48 μs |  0.80 |    0.00 |     120 B |          NA |
| StudentT              | Linq_Double     |    192.12 μs |   0.425 μs |   0.398 μs |    191.91 μs |  1.57 |    0.01 |         - |          NA |
| Wishart               | Linq_Double     |  1,255.44 μs |   3.503 μs |   3.277 μs |  1,254.66 μs | 10.25 |    0.04 |      65 B |          NA |
|                       |                 |              |            |            |              |       |         |           |             |
| Beta                  | Linq_Decimal    |  5,137.69 μs |  12.259 μs |  11.467 μs |  5,135.46 μs |  1.95 |    0.01 |       6 B |        2.00 |
| Chi                   | Linq_Decimal    |  3,574.03 μs |   9.193 μs |   8.599 μs |  3,573.52 μs |  1.35 |    0.00 |       3 B |        1.00 |
| Chi-Squared           | Linq_Decimal    |  2,619.87 μs |   8.276 μs |   7.336 μs |  2,619.73 μs |  0.99 |    0.00 |       3 B |        1.00 |
| Dirichlet             | Linq_Decimal    | 16,223.89 μs | 129.712 μs | 121.333 μs | 16,221.21 μs |  6.14 |    0.05 |     151 B |       50.33 |
| F                     | Linq_Decimal    |  5,371.93 μs |   7.305 μs |   6.833 μs |  5,372.07 μs |  2.03 |    0.00 |       6 B |        2.00 |
| Gamma                 | Linq_Decimal    |  2,640.87 μs |   4.513 μs |   4.000 μs |  2,641.12 μs |  1.00 |    0.00 |       3 B |        1.00 |
| Halton                | Linq_Decimal    |  1,165.97 μs |   1.591 μs |   1.329 μs |  1,165.82 μs |  0.44 |    0.00 |     129 B |       43.00 |
| Inverse-Gamma         | Linq_Decimal    |  2,727.65 μs |   9.163 μs |   8.571 μs |  2,728.46 μs |  1.03 |    0.00 |       3 B |        1.00 |
| Log-Normal            | Linq_Decimal    |  2,498.71 μs |   8.647 μs |   8.088 μs |  2,499.22 μs |  0.95 |    0.00 |       3 B |        1.00 |
| 'Multivariate Normal' | Linq_Decimal    |  1,662.11 μs |  13.767 μs |  12.878 μs |  1,654.84 μs |  0.63 |    0.00 |     129 B |       43.00 |
| Sobol                 | Linq_Decimal    |    210.12 μs |   2.244 μs |   2.099 μs |    209.71 μs |  0.08 |    0.00 |     128 B |       42.67 |
| Spatial.OnSphere      | Linq_Decimal    |  2,032.61 μs |   2.909 μs |   2.271 μs |  2,032.19 μs |  0.77 |    0.00 |     131 B |       43.67 |
| StudentT              | Linq_Decimal    |  4,153.07 μs |   6.186 μs |   5.484 μs |  4,151.83 μs |  1.57 |    0.00 |       6 B |        2.00 |
| Wishart               | Linq_Decimal    | 19,179.71 μs |  91.243 μs |  85.349 μs | 19,152.05 μs |  7.26 |    0.03 |      87 B |       29.00 |
|                       |                 |              |            |            |              |       |         |           |             |
| Beta                  | Linq_Fixed      |    215.65 μs |   0.549 μs |   0.514 μs |    215.67 μs |  2.03 |    0.02 |         - |          NA |
| Chi                   | Linq_Fixed      |    109.17 μs |   0.766 μs |   0.716 μs |    109.45 μs |  1.03 |    0.01 |         - |          NA |
| Chi-Squared           | Linq_Fixed      |    106.69 μs |   0.265 μs |   0.248 μs |    106.71 μs |  1.01 |    0.01 |         - |          NA |
| Dirichlet             | Linq_Fixed      |    867.58 μs |   1.745 μs |   1.632 μs |    867.56 μs |  8.18 |    0.06 |     121 B |          NA |
| F                     | Linq_Fixed      |    220.00 μs |   1.454 μs |   1.360 μs |    219.34 μs |  2.07 |    0.02 |         - |          NA |
| Gamma                 | Linq_Fixed      |    106.06 μs |   0.892 μs |   0.835 μs |    105.59 μs |  1.00 |    0.01 |         - |          NA |
| Halton                | Linq_Fixed      |    297.45 μs |  32.570 μs |  30.466 μs |    276.94 μs |  2.80 |    0.28 |     120 B |          NA |
| Inverse-Gamma         | Linq_Fixed      |    110.33 μs |   0.342 μs |   0.320 μs |    110.28 μs |  1.04 |    0.01 |         - |          NA |
| Log-Normal            | Linq_Fixed      |     86.60 μs |   0.768 μs |   0.718 μs |     86.74 μs |  0.82 |    0.01 |         - |          NA |
| 'Multivariate Normal' | Linq_Fixed      |    436.12 μs |   4.133 μs |   3.866 μs |    434.93 μs |  4.11 |    0.05 |     120 B |          NA |
| Sobol                 | Linq_Fixed      |    354.66 μs |   0.561 μs |   0.524 μs |    354.65 μs |  3.34 |    0.03 |     120 B |          NA |
| Spatial.OnSphere      | Linq_Fixed      |     86.07 μs |   0.083 μs |   0.078 μs |     86.06 μs |  0.81 |    0.01 |     120 B |          NA |
| StudentT              | Linq_Fixed      |    178.42 μs |   0.476 μs |   0.397 μs |    178.22 μs |  1.68 |    0.01 |         - |          NA |
| Wishart               | Linq_Fixed      |  1,198.30 μs |   3.851 μs |   3.602 μs |  1,198.81 μs | 11.30 |    0.09 |      65 B |          NA |

// * Hints *
Outliers
  SamplerAdvancedBenchmarks.Log-Normal: MinIterationTime=500ms, IterationCount=15            -> 3 outliers were removed (56.65 μs..57.42 μs)
  SamplerAdvancedBenchmarks.'Multivariate Normal': MinIterationTime=500ms, IterationCount=15 -> 2 outliers were removed, 3 outliers were detected (324.34 μs, 327.97 μs, 329.02 μs)
  SamplerAdvancedBenchmarks.F: MinIterationTime=500ms, IterationCount=15                     -> 1 outlier  was  removed (241.55 μs)
  SamplerAdvancedBenchmarks.Gamma: MinIterationTime=500ms, IterationCount=15                 -> 1 outlier  was  removed (118.78 μs)
  SamplerAdvancedBenchmarks.'Multivariate Normal': MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  removed (429.39 μs)
  SamplerAdvancedBenchmarks.Dirichlet: MinIterationTime=500ms, IterationCount=15             -> 2 outliers were removed (16.07 ms, 16.10 ms)
  SamplerAdvancedBenchmarks.Inverse-Gamma: MinIterationTime=500ms, IterationCount=15         -> 1 outlier  was  removed (2.79 ms)
  SamplerAdvancedBenchmarks.Log-Normal: MinIterationTime=500ms, IterationCount=15            -> 1 outlier  was  removed (2.56 ms)
  SamplerAdvancedBenchmarks.Spatial.OnSphere: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (84.77 μs)
  SamplerAdvancedBenchmarks.F: MinIterationTime=500ms, IterationCount=15                     -> 3 outliers were removed (190.12 μs..193.40 μs)
  SamplerAdvancedBenchmarks.Spatial.OnSphere: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (57.96 μs)
  SamplerAdvancedBenchmarks.Chi: MinIterationTime=500ms, IterationCount=15                   -> 1 outlier  was  removed (119.85 μs)
  SamplerAdvancedBenchmarks.F: MinIterationTime=500ms, IterationCount=15                     -> 1 outlier  was  removed (242.70 μs)
  SamplerAdvancedBenchmarks.Halton: MinIterationTime=500ms, IterationCount=15                -> 1 outlier  was  detected (271.55 μs)
  SamplerAdvancedBenchmarks.Log-Normal: MinIterationTime=500ms, IterationCount=15            -> 1 outlier  was  removed (78.00 μs)
  SamplerAdvancedBenchmarks.Sobol: MinIterationTime=500ms, IterationCount=15                 -> 2 outliers were removed (330.39 μs, 330.84 μs)
  SamplerAdvancedBenchmarks.Spatial.OnSphere: MinIterationTime=500ms, IterationCount=15      -> 1 outlier  was  removed (84.82 μs)
  SamplerAdvancedBenchmarks.StudentT: MinIterationTime=500ms, IterationCount=15              -> 1 outlier  was  removed, 3 outliers were detected (192.41 μs, 192.45 μs, 193.82 μs)
  SamplerAdvancedBenchmarks.Wishart: MinIterationTime=500ms, IterationCount=15               -> 1 outlier  was  removed (1.22 ms)
  SamplerAdvancedBenchmarks.Beta: MinIterationTime=500ms, IterationCount=15                  -> 1 outlier  was  removed, 2 outliers were detected (184.98 μs, 186.86 μs)
  SamplerAdvancedBenchmarks.Chi-Squared: MinIterationTime=500ms, IterationCount=15           -> 1 outlier  was  removed (94.69 μs)
  SamplerAdvancedBenchmarks.Dirichlet: MinIterationTime=500ms, IterationCount=15             -> 1 outlier  was  removed (764.98 μs)
  SamplerAdvancedBenchmarks.F: MinIterationTime=500ms, IterationCount=15                     -> 1 outlier  was  removed (186.12 μs)
  SamplerAdvancedBenchmarks.Inverse-Gamma: MinIterationTime=500ms, IterationCount=15         -> 1 outlier  was  removed (95.15 μs)
  SamplerAdvancedBenchmarks.'Multivariate Normal': MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  removed (354.48 μs)
  SamplerAdvancedBenchmarks.Spatial.OnSphere: MinIterationTime=500ms, IterationCount=15      -> 2 outliers were removed (57.92 μs, 58.12 μs)
  SamplerAdvancedBenchmarks.StudentT: MinIterationTime=500ms, IterationCount=15              -> 1 outlier  was  removed (149.72 μs)
  SamplerAdvancedBenchmarks.Dirichlet: MinIterationTime=500ms, IterationCount=15             -> 1 outlier  was  removed (962.01 μs)
  SamplerAdvancedBenchmarks.Inverse-Gamma: MinIterationTime=500ms, IterationCount=15         -> 2 outliers were removed (129.66 μs, 130.09 μs)
  SamplerAdvancedBenchmarks.Log-Normal: MinIterationTime=500ms, IterationCount=15            -> 1 outlier  was  removed (78.75 μs)
  SamplerAdvancedBenchmarks.'Multivariate Normal': MinIterationTime=500ms, IterationCount=15 -> 2 outliers were removed (507.63 μs, 512.50 μs)
  SamplerAdvancedBenchmarks.Chi-Squared: MinIterationTime=500ms, IterationCount=15           -> 1 outlier  was  removed (2.65 ms)
  SamplerAdvancedBenchmarks.Gamma: MinIterationTime=500ms, IterationCount=15                 -> 1 outlier  was  removed, 2 outliers were detected (2.63 ms, 2.65 ms)
  SamplerAdvancedBenchmarks.Halton: MinIterationTime=500ms, IterationCount=15                -> 2 outliers were removed (1.17 ms, 1.17 ms)
  SamplerAdvancedBenchmarks.Spatial.OnSphere: MinIterationTime=500ms, IterationCount=15      -> 3 outliers were removed (2.05 ms..2.09 ms)
  SamplerAdvancedBenchmarks.StudentT: MinIterationTime=500ms, IterationCount=15              -> 1 outlier  was  removed (4.17 ms)
  SamplerAdvancedBenchmarks.StudentT: MinIterationTime=500ms, IterationCount=15              -> 2 outliers were removed (180.16 μs, 180.96 μs)

// * Legends *
  Param       : Value of the 'Param' parameter
  Mean        : Arithmetic mean of all measurements
  Error       : Half of 99.9% confidence interval
  StdDev      : Standard deviation of all measurements
  Median      : Value separating the higher half of all measurements (50th percentile)
  Ratio       : Mean of the ratio distribution ([Current]/[Baseline])
  RatioSD     : Standard deviation of the ratio distribution ([Current]/[Baseline])
  Allocated   : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  Alloc Ratio : Allocated memory ratio distribution ([Current]/[Baseline])
  1 μs        : 1 Microsecond (0.000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
Run time: 00:57:02 (3422.49 sec), executed benchmarks: 168

*/