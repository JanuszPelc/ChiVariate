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

    /*
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
    */

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

    /*
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
    */

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

    /*
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
    */

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