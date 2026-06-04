using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Pareto;

/// <summary>
///     Tests for Pareto distribution using ChiFixed type.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(1.0, 2.0)] // Finite mean, infinite variance
    [InlineData(10.0, 3.0)] // Finite mean and variance
    [InlineData(1.0, 3.0)] // Classic power-law
    public void Sample_AcrossScaleAndShape_MatchesParetoDistribution(double scale, double shape)
    {
        var sc = (ChiFixed)(decimal)scale;
        var sh = (ChiFixed)(decimal)shape;

        var rng = new ChiRng(ChiSeed.Scramble("ParetoFixed", scale * 100 + shape));
        var maxBound = scale * 10;
        var histogram = new Histogram(scale, maxBound, 150);
        var sampler = new ChiFixedParetoSampler(sc, sh);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedParetoSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, $"Pareto(xm={scale}, alpha={shape}) using ChiFixed");
        histogram.AssertIsPareto(scale, shape, 0.25);
    }

    [Fact]
    public void Sample_AcrossManySamples_ReturnsValuesAtLeastScale()
    {
        var scale = (ChiFixed)5m;
        var shape = (ChiFixed)2m;

        var rng = new ChiRng("ParetoBoundsFixed");

        for (var i = 0; i < 10000; i++)
        {
            var sample = rng.Pareto(scale, shape).Sample();
            Assert.True(sample >= scale, "Pareto samples must be >= scale");
        }
    }

    private readonly struct ChiFixedParetoSampler(ChiFixed scale, ChiFixed shape) :
        IHistogramSamplerWithRange<ChiFixed, ChiRng>
    {
        public ChiFixed NextSample(ref ChiRng rng)
        {
            return rng.Pareto(scale, shape).Sample();
        }

        public double Normalize(ChiFixed value)
        {
            return double.CreateChecked(value);
        }
    }
}