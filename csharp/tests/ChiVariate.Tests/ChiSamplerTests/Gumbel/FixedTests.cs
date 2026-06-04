using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Gumbel;

/// <summary>
///     Tests for Gumbel (extreme value) distribution using ChiFixed type.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;
    private const double EulerMascheroni = 0.57721566490153286;

    [Theory]
    [InlineData(0.0, 1.0)] // Standard Gumbel
    [InlineData(5.0, 2.0)] // Shifted and scaled
    [InlineData(-10.0, 0.5)] // Shifted and narrow
    public void Sample_AcrossLocationAndScale_MatchesGumbelMean(double location, double scale)
    {
        var loc = (ChiFixed)(decimal)location;
        var sc = (ChiFixed)(decimal)scale;

        var rng = new ChiRng(ChiSeed.Scramble("GumbelFixed", location * 100 + scale));
        var expectedMean = location + scale * EulerMascheroni;
        var expectedVariance = Math.PI * Math.PI / 6.0 * (scale * scale);
        var expectedStdDev = Math.Sqrt(expectedVariance);
        var histogram = new Histogram(expectedMean - 8 * expectedStdDev, expectedMean + 8 * expectedStdDev, 200);
        var sampler = new ChiFixedGumbelSampler(loc, sc);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedGumbelSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, $"Gumbel(mu={location}, beta={scale}) using ChiFixed");

        var actualMean = histogram.CalculateMean();
        actualMean.Should().BeApproximately(expectedMean, Math.Abs(expectedMean * 0.15) + 0.1,
            "because the mean should match mu + beta*gamma.");
    }

    [Fact]
    public void Sample_StandardGumbel_HasMeanEqualToEulerMascheroni()
    {
        var location = ChiFixed.Zero;
        var scale = ChiFixed.One;

        var rng = new ChiRng("GumbelStandardFixed");
        var expectedMean = EulerMascheroni;
        var histogram = new Histogram(-5, 10, 150);
        var sampler = new ChiFixedGumbelSampler(location, scale);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedGumbelSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, "Gumbel(0, 1) using ChiFixed");

        var actualMean = histogram.CalculateMean();
        actualMean.Should().BeApproximately(expectedMean, 0.05,
            "because standard Gumbel has mean = Euler-Mascheroni constant.");
    }

    private readonly struct ChiFixedGumbelSampler(ChiFixed location, ChiFixed scale) :
        IHistogramSamplerWithRange<ChiFixed, ChiRng>
    {
        public ChiFixed NextSample(ref ChiRng rng)
        {
            return rng.Gumbel(location, scale).Sample();
        }

        public double Normalize(ChiFixed value)
        {
            return double.CreateChecked(value);
        }
    }
}