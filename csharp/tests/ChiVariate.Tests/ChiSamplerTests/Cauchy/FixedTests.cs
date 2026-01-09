using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Cauchy;

/// <summary>
///     Tests for Cauchy distribution using ChiFixed type.
///     Cauchy has no mean or variance (heavy tails), so we test the median.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(0.0, 1.0)] // Standard Cauchy
    [InlineData(10.0, 5.0)] // Shifted and scaled
    [InlineData(0.0, 0.5)] // Narrower peak
    public void Sample_WithVariousParameters_ProducesCauchyDistribution(double location, double scale)
    {
        var loc = (ChiFixed)(decimal)location;
        var sc = (ChiFixed)(decimal)scale;

        var rng = new ChiRng(ChiSeed.Scramble("CauchyFixed", location * 100 + scale));
        var histogramRange = 10 * scale;
        var histogram = new Histogram(location - histogramRange, location + histogramRange, 201);
        var sampler = new ChiFixedCauchySampler(loc, sc);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedCauchySampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, $"Cauchy(x0={location}, gamma={scale}) using ChiFixed");
        histogram.AssertIsCauchy(location, 0.15);
    }

    [Fact]
    public void Sample_StandardCauchy_HasMedianAtZero()
    {
        var location = ChiFixed.Zero;
        var scale = ChiFixed.One;

        var rng = new ChiRng("CauchyStandardFixed");
        var histogram = new Histogram(-10, 10, 201);
        var sampler = new ChiFixedCauchySampler(location, scale);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedCauchySampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, "Cauchy(0, 1) using ChiFixed");

        var median = histogram.CalculateMedian();
        Assert.InRange(median, -0.1, 0.1);
    }

    private readonly struct ChiFixedCauchySampler(ChiFixed location, ChiFixed scale) :
        IHistogramSamplerWithRange<ChiFixed, ChiRng>
    {
        public ChiFixed NextSample(ref ChiRng rng)
        {
            return rng.Cauchy(location, scale).Sample();
        }

        public double Normalize(ChiFixed value)
        {
            return double.CreateChecked(value);
        }
    }
}