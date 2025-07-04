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
public class DistributionBenchmarks
{
    #region Benchmarks

    [Benchmark(Description = "Bernoulli")]
    public bool ChiVariateBernoulli()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunBernoulliBenchmark<float>(),
            ValueType._64Bit => RunBernoulliBenchmark<double>(),
            ValueType._128Bit => RunBernoulliBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBernoulliBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunBetaBenchmark<float>(),
            ValueType._64Bit => RunBetaBenchmark<double>(),
            ValueType._128Bit => RunBetaBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunBetaBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunBinomialBenchmark<int>(),
            ValueType._64Bit => RunBinomialBenchmark<long>(),
            ValueType._128Bit => RunBinomialBenchmark<Int128>(),
            _ => throw new UnreachableException()
        };

        bool RunBinomialBenchmark<T>() where T : unmanaged, IBinaryInteger<T>
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
            ValueType._32Bit => RunCategoricalBenchmark<float>(),
            ValueType._64Bit => RunCategoricalBenchmark<double>(),
            ValueType._128Bit => RunCategoricalBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunCategoricalBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunCauchyBenchmark<float>(),
            ValueType._64Bit => RunCauchyBenchmark<double>(),
            ValueType._128Bit => RunCauchyBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunCauchyBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunChanceBenchmark<int>(),
            ValueType._64Bit => RunChanceBenchmark<long>(),
            ValueType._128Bit => RunChanceBenchmark<Int128>(),
            _ => throw new UnreachableException()
        };

        bool RunChanceBenchmark<T>() where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
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
            ValueType._32Bit => RunChiBenchmark<float>(),
            ValueType._64Bit => RunChiBenchmark<double>(),
            ValueType._128Bit => RunChiBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunChiBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunChiSquaredBenchmark<float>(),
            ValueType._64Bit => RunChiSquaredBenchmark<double>(),
            ValueType._128Bit => RunChiSquaredBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunChiSquaredBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunDirichletBenchmark<float>(),
            ValueType._64Bit => RunDirichletBenchmark<double>(),
            ValueType._128Bit => RunDirichletBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunDirichletBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunExponentialBenchmark<float>(),
            ValueType._64Bit => RunExponentialBenchmark<double>(),
            ValueType._128Bit => RunExponentialBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunExponentialBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunFBenchmark<float>(),
            ValueType._64Bit => RunFBenchmark<double>(),
            ValueType._128Bit => RunFBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunFBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunGammaBenchmark<float>(),
            ValueType._64Bit => RunGammaBenchmark<double>(),
            ValueType._128Bit => RunGammaBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunGammaBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunGumbelBenchmark<float>(),
            ValueType._64Bit => RunGumbelBenchmark<double>(),
            ValueType._128Bit => RunGumbelBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunGumbelBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunGeometricBenchmark<int>(),
            ValueType._64Bit => RunGeometricBenchmark<long>(),
            ValueType._128Bit => RunGeometricBenchmark<Int128>(),
            _ => throw new UnreachableException()
        };

        bool RunGeometricBenchmark<T>() where T : unmanaged, IBinaryInteger<T>
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
            ValueType._32Bit => RunHaltonBenchmark<float>(),
            ValueType._64Bit => RunHaltonBenchmark<double>(),
            ValueType._128Bit => RunHaltonBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunHaltonBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunHypergeometricBenchmark<int>(),
            ValueType._64Bit => RunHypergeometricBenchmark<long>(),
            ValueType._128Bit => RunHypergeometricBenchmark<Int128>(),
            _ => throw new UnreachableException()
        };

        bool RunHypergeometricBenchmark<T>() where T : unmanaged, IBinaryInteger<T>
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
            ValueType._32Bit => RunInverseGammaBenchmark<float>(),
            ValueType._64Bit => RunInverseGammaBenchmark<double>(),
            ValueType._128Bit => RunInverseGammaBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunInverseGammaBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunLaplaceBenchmark<float>(),
            ValueType._64Bit => RunLaplaceBenchmark<double>(),
            ValueType._128Bit => RunLaplaceBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunLaplaceBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunLogisticBenchmark<float>(),
            ValueType._64Bit => RunLogisticBenchmark<double>(),
            ValueType._128Bit => RunLogisticBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunLogisticBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunLogNormalBenchmark<float>(),
            ValueType._64Bit => RunLogNormalBenchmark<double>(),
            ValueType._128Bit => RunLogNormalBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunLogNormalBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunMultinomialBenchmark<int>(),
            ValueType._64Bit => RunMultinomialBenchmark<long>(),
            ValueType._128Bit => RunMultinomialBenchmark<Int128>(),
            _ => throw new UnreachableException()
        };

        bool RunMultinomialBenchmark<T>() where T : unmanaged, IBinaryInteger<T>
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
            ValueType._32Bit => RunMultivariateNormalBenchmark<float>(),
            ValueType._64Bit => RunMultivariateNormalBenchmark<double>(),
            ValueType._128Bit => RunMultivariateNormalBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunMultivariateNormalBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunNegativeBinomialBenchmark<int>(),
            ValueType._64Bit => RunNegativeBinomialBenchmark<long>(),
            ValueType._128Bit => RunNegativeBinomialBenchmark<Int128>(),
            _ => throw new UnreachableException()
        };

        bool RunNegativeBinomialBenchmark<T>() where T : unmanaged, IBinaryInteger<T>
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
            ValueType._32Bit => RunNormalBenchmark<float>(),
            ValueType._64Bit => RunNormalBenchmark<double>(),
            ValueType._128Bit => RunNormalBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunNormalBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunParetoBenchmark<float>(),
            ValueType._64Bit => RunParetoBenchmark<double>(),
            ValueType._128Bit => RunParetoBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunParetoBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunPoissonBenchmark<int>(),
            ValueType._64Bit => RunPoissonBenchmark<long>(),
            ValueType._128Bit => RunPoissonBenchmark<Int128>(),
            _ => throw new UnreachableException()
        };

        bool RunPoissonBenchmark<T>() where T : unmanaged, IBinaryInteger<T>
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

    [Benchmark(Description = "Primes")]
    public bool ChiVariatePrimes()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunUniformDiscreteBenchmark(int.MaxValue),
            ValueType._64Bit => RunUniformDiscreteBenchmark(long.MaxValue),
            ValueType._128Bit => RunUniformDiscreteBenchmark<Int128>(ulong.MaxValue),
            _ => throw new UnreachableException()
        };

        bool RunUniformDiscreteBenchmark<T>(T max)
            where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
        {
            var sampler = _rng.Primes(T.Zero, max);
            var sum = T.Zero;

            switch (IterationTypeParam)
            {
                case IterationType.ForLoop:
                {
                    for (var i = 0; i < SampleCount; i++)
                        sum += sampler.Sample() / T.CreateChecked(10_000);

                    break;
                }
                case IterationType.ForEach:
                {
                    foreach (var sample in sampler.Sample(SampleCount))
                        sum += sample / T.CreateChecked(10_000);
                    break;
                }
                case IterationType.Linq:
                {
                    var enumerable = sampler.Sample(SampleCount).Select(value => value / T.CreateChecked(10_000));
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
            ValueType._32Bit => RunRayleighBenchmark<float>(),
            ValueType._64Bit => RunRayleighBenchmark<double>(),
            ValueType._128Bit => RunRayleighBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunRayleighBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunSobolBenchmark<float>(),
            ValueType._64Bit => RunSobolBenchmark<double>(),
            ValueType._128Bit => RunSobolBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunSobolBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunSpatialBenchmark<float>(),
            ValueType._64Bit => RunSpatialBenchmark<double>(),
            ValueType._128Bit => RunSpatialBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunSpatialBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunStudentTBenchmark<float>(),
            ValueType._64Bit => RunStudentTBenchmark<double>(),
            ValueType._128Bit => RunStudentTBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunStudentTBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunTriangularBenchmark<float>(),
            ValueType._64Bit => RunTriangularBenchmark<double>(),
            ValueType._128Bit => RunTriangularBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunTriangularBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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

    [Benchmark(Baseline = true, Description = "Uniform (Continuous)")]
    public bool ChiVariateUniformContinuous()
    {
        return ValueTypeParam switch
        {
            ValueType._32Bit => RunUniformContinuousBenchmark<float>(),
            ValueType._64Bit => RunUniformContinuousBenchmark<double>(),
            ValueType._128Bit => RunUniformContinuousBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunUniformContinuousBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunUniformDiscreteBenchmark<int>(),
            ValueType._64Bit => RunUniformDiscreteBenchmark<long>(),
            ValueType._128Bit => RunUniformDiscreteBenchmark<Int128>(),
            _ => throw new UnreachableException()
        };

        bool RunUniformDiscreteBenchmark<T>() where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
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
            ValueType._32Bit => RunWeibullBenchmark<float>(),
            ValueType._64Bit => RunWeibullBenchmark<double>(),
            ValueType._128Bit => RunWeibullBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunWeibullBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunWishartBenchmark<float>(),
            ValueType._64Bit => RunWishartBenchmark<double>(),
            ValueType._128Bit => RunWishartBenchmark<decimal>(),
            _ => throw new UnreachableException()
        };

        bool RunWishartBenchmark<T>() where T : unmanaged, IFloatingPoint<T>
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
            ValueType._32Bit => RunZipfBenchmark<int>(),
            ValueType._64Bit => RunZipfBenchmark<long>(),
            ValueType._128Bit => RunZipfBenchmark<Int128>(),
            _ => throw new UnreachableException()
        };

        bool RunZipfBenchmark<T>() where T : unmanaged, IBinaryInteger<T>
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