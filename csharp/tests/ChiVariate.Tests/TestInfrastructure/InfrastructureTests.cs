using System.Runtime.CompilerServices;
using AwesomeAssertions;
using Xunit;
using Xunit.Sdk;

namespace ChiVariate.Tests.TestInfrastructure;

#pragma warning disable CS1591

public class InfrastructureTests
{
    private const int SampleCount = 100_000;

    [Fact]
    public void AssertIsUniform_WithSkewedDistribution_ThrowsXunitException()
    {
        var rng = new ChiRng(42);
        var histogram = new Histogram(0.0, 1.0, 10);
        var sampler = new SkewedSampler();

        var act = () =>
        {
            histogram.Generate<double, ChiRng, SkewedSampler>(ref rng, SampleCount, sampler);
            histogram.AssertIsUniform(0.05);
        };

        act.Should().Throw<XunitException>()
            .WithMessage("*because bin*should be within*of the expected average*");
    }

    private readonly struct SkewedSampler : IHistogramSamplerWithRange<double, ChiRng>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double NextSample(ref ChiRng rng)
        {
            return rng.Chance().NextDouble() * rng.Chance().NextDouble();
        }

        public double Normalize(double value)
        {
            return value;
        }
    }
}