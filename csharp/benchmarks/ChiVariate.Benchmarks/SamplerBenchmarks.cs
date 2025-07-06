using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams)]
public class SamplerBenchmarks
{
    #region Benchmarks

    [Benchmark(Description = "Bernoulli")]
    public bool ChiVariateBernoulli()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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

    [Benchmark(Description = "Beta")]
    public bool ChiVariateBeta()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
        {
            var sampler = _rng.Beta(T.CreateChecked(2.0), T.CreateChecked(5.0)); // alpha=2.0, beta=5.0
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

    [Benchmark(Description = "Binomial")]
    public bool ChiVariateBinomial()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<int>(),
            ValueType._64Bit => RunBenchmark<long>(),
            ValueType._128Bit => RunBenchmark<Int128>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IBinaryInteger<T>
        {
            var sampler = _rng.Binomial(T.CreateChecked(50), 0.25); // n=50, p=0.25
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

    [Benchmark(Description = "Categorical")]
    public bool ChiVariateCategorical()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
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
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
        {
            var sampler = _rng.Cauchy(T.Zero, T.One); // location=0, scale=1
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
            ValueType._32Bit => RunBenchmark<int>(),
            ValueType._64Bit => RunBenchmark<long>(),
            ValueType._128Bit => RunBenchmark<Int128>(),
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

    [Benchmark(Description = "Chi")]
    public bool ChiVariateChi()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
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

    [Benchmark(Description = "Exponential")]
    public bool ChiVariateExponential()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
        {
            var sampler = _rng.Exponential(T.One); // lambda = 1.0
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

    [Benchmark(Description = "F")]
    public bool ChiVariateF()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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

    [Benchmark(Description = "Gamma")]
    public bool ChiVariateGamma()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
        {
            var sampler = _rng.Gamma(T.CreateChecked(2.5), T.CreateChecked(1.5)); // shape=2.5, scale=1.5
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
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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

    [Benchmark(Description = "Geometric")]
    public bool ChiVariateGeometric()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<int>(),
            ValueType._64Bit => RunBenchmark<long>(),
            ValueType._128Bit => RunBenchmark<Int128>(),
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

    [Benchmark(Description = "Halton")]
    public bool ChiVariateHalton()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
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

    [Benchmark(Description = "Hypergeometric")]
    public bool ChiVariateHypergeometric()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<int>(),
            ValueType._64Bit => RunBenchmark<long>(),
            ValueType._128Bit => RunBenchmark<Int128>(),
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

    [Benchmark(Description = "Inverse-Gamma")]
    public bool ChiVariateInverseGamma()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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

    [Benchmark(Description = "Laplace")]
    public bool ChiVariateLaplace()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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

    [Benchmark(Description = "Log-Normal")]
    public bool ChiVariateLogNormal()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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

    [Benchmark(Description = "Multinomial")]
    public bool ChiVariateMultinomial()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<int>(),
            ValueType._64Bit => RunBenchmark<long>(),
            ValueType._128Bit => RunBenchmark<Int128>(),
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

    [Benchmark(Description = "Multivariate Normal")]
    public bool ChiVariateMultivariateNormal()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
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

    [Benchmark(Description = "Negative Binomial")]
    public bool ChiVariateNegativeBinomial()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<int>(),
            ValueType._64Bit => RunBenchmark<long>(),
            ValueType._128Bit => RunBenchmark<Int128>(),
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

    [Benchmark(Description = "Normal")]
    public bool ChiVariateNormal()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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

    [Benchmark(Description = "Poisson")]
    public bool ChiVariatePoisson()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<int>(),
            ValueType._64Bit => RunBenchmark<long>(),
            ValueType._128Bit => RunBenchmark<Int128>(),
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
            ValueType._32Bit => RunBenchmark<int>(),
            ValueType._64Bit => RunBenchmark<long>(),
            ValueType._128Bit => RunBenchmark<Int128>(),
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
                        sum += (sampler.Sample() & T.Zero) | T.One;
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

    [Benchmark(Description = "Rayleigh")]
    public bool ChiVariateRayleigh()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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

    [Benchmark(Description = "Sobol")]
    public bool ChiVariateSobol()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
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
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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

    [Benchmark(Description = "Triangular")]
    public bool ChiVariateTriangular()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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

    [Benchmark(Description = "Uniform (Continuous)", Baseline = true)]
    public bool ChiVariateUniformContinuous()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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

    [Benchmark(Description = "Uniform (Discrete)")]
    public bool ChiVariateUniformDiscrete()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<int>(),
            ValueType._64Bit => RunBenchmark<long>(),
            ValueType._128Bit => RunBenchmark<Int128>(),
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

    [Benchmark(Description = "Weibull")]
    public bool ChiVariateWeibull()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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

    [Benchmark(Description = "Wishart")]
    public bool ChiVariateWishart()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<float>(),
            ValueType._64Bit => RunBenchmark<double>(),
            ValueType._128Bit => RunBenchmark<decimal>(),
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

    [Benchmark(Description = "Zipf")]
    public bool ChiVariateZipf()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBenchmark<int>(),
            ValueType._64Bit => RunBenchmark<long>(),
            ValueType._128Bit => RunBenchmark<Int128>(),
            _ => throw new UnreachableException()
        };

        bool RunBenchmark<T>() where T : unmanaged, IBinaryInteger<T>
        {
            var sampler = _rng.Zipf(T.CreateChecked(100), 1.07); // N=100, s=1.07
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
        _32Bit,
        _64Bit,
        _128Bit,
        BitMask = 0xFFFF
    }

    public enum ParamType
    {
        // ReSharper disable InconsistentNaming
        ForLoop_32Bit = IterationType.ForLoop | ValueType._32Bit,
        ForLoop_64Bit = IterationType.ForLoop | ValueType._64Bit,
        ForLoop_128Bit = IterationType.ForLoop | ValueType._128Bit,
        ForEach_32Bit = IterationType.ForEach | ValueType._32Bit,
        ForEach_64Bit = IterationType.ForEach | ValueType._64Bit,
        ForEach_128Bit = IterationType.ForEach | ValueType._128Bit,
        Linq_32Bit = IterationType.Linq | ValueType._32Bit,
        Linq_64Bit = IterationType.Linq | ValueType._64Bit,

        Linq_128Bit = IterationType.Linq | ValueType._128Bit
        // ReSharper restore InconsistentNaming
    }

    // ReSharper disable UnusedAutoPropertyAccessor.Global
    [Params(
        ParamType.ForLoop_32Bit, ParamType.ForLoop_64Bit, ParamType.ForLoop_128Bit,
        ParamType.ForEach_32Bit, ParamType.ForEach_64Bit, ParamType.ForEach_128Bit,
        ParamType.Linq_32Bit, ParamType.Linq_64Bit, ParamType.Linq_128Bit)]
    public ParamType Param { get; set; }
    // ReSharper restore UnusedAutoPropertyAccessor.Global

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
        where T : unmanaged, INumberBase<T>
    {
        if (typeof(T) == typeof(float))
            return T.CreateTruncating(enumerable.Sum(float.CreateTruncating));

        if (typeof(T) == typeof(double))
            return T.CreateTruncating(enumerable.Sum(double.CreateTruncating));

        if (typeof(T) == typeof(decimal))
            return T.CreateTruncating(enumerable.Sum(decimal.CreateTruncating));

        if (typeof(T) == typeof(int))
            return T.CreateTruncating(enumerable.Sum(int.CreateTruncating));

        if (typeof(T) == typeof(long))
            return T.CreateTruncating(enumerable.Sum(long.CreateTruncating));

        if (typeof(T) == typeof(Int128))
            return T.CreateTruncating(enumerable.Aggregate(T.Zero, (sum, value) => sum + value));

        throw new UnreachableException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T SumValues<T>(IEnumerable<T> enumerable)
        where T : unmanaged, INumberBase<T>
    {
        if (typeof(T) == typeof(float))
            return T.CreateTruncating(enumerable.Sum(float.CreateTruncating));

        if (typeof(T) == typeof(double))
            return T.CreateTruncating(enumerable.Sum(double.CreateTruncating));

        if (typeof(T) == typeof(decimal))
            return T.CreateTruncating(enumerable.Sum(decimal.CreateTruncating));

        if (typeof(T) == typeof(int))
            return T.CreateTruncating(enumerable.Sum(int.CreateTruncating));

        if (typeof(T) == typeof(long))
            return T.CreateTruncating(enumerable.Sum(long.CreateTruncating));

        if (typeof(T) == typeof(Int128))
            return T.CreateTruncating(enumerable.Sum(long.CreateTruncating));

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
        private static readonly double[,] Template = new[,]
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