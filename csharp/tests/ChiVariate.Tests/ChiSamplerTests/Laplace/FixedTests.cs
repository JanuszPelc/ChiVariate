using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Laplace;

/// <summary>
///     Tests for Laplace (double exponential) distribution using ChiFixed type.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(0.0, 1.0)] // Standard Laplace
    [InlineData(10.0, 5.0)] // Shifted and scaled
    [InlineData(-5.0, 0.5)] // Shifted and narrow
    public void Sample_AcrossLocationAndScale_MatchesLaplaceDistribution(double location, double scale)
    {
        var loc = (ChiFixed)(decimal)location;
        var sc = (ChiFixed)(decimal)scale;

        var rng = new ChiRng(ChiSeed.Scramble("LaplaceFixed", location * 100 + scale));
        var maxBound = location + 10 * scale;
        var minBound = location - 10 * scale;
        var histogram = new Histogram(minBound, maxBound, 100);
        var sampler = new ChiFixedLaplaceSampler(loc, sc);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedLaplaceSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, $"Laplace(mu={location}, b={scale}) using ChiFixed");
        histogram.AssertIsLaplace(location, scale, 0.15);
    }

    [Fact]
    public void Sample_StandardLaplace_HasMeanNearZero()
    {
        var location = ChiFixed.Zero;
        var scale = ChiFixed.One;

        var rng = new ChiRng("LaplaceStandardFixed");
        var histogram = new Histogram(-10, 10, 100);
        var sampler = new ChiFixedLaplaceSampler(location, scale);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedLaplaceSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, "Laplace(0, 1) using ChiFixed");

        var mean = histogram.CalculateMean();
        Assert.InRange(mean, -0.05, 0.05);
    }

    private readonly struct ChiFixedLaplaceSampler(ChiFixed location, ChiFixed scale) :
        IHistogramSamplerWithRange<ChiFixed, ChiRng>
    {
        public ChiFixed NextSample(ref ChiRng rng)
        {
            return rng.Laplace(location, scale).Sample();
        }

        public double Normalize(ChiFixed value)
        {
            return double.CreateChecked(value);
        }
    }
}