using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Chi;

/// <summary>
///     Tests for Chi distribution using ChiFixed type.
///     Chi is the square root of Chi-squared, directly testing the Normal/Ziggurat chain.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(2)] // Equivalent to Rayleigh(1)
    [InlineData(5)] // Skewed right
    [InlineData(10)] // More symmetric
    public void Sample_Squared_MatchesChiSquaredDistribution(int degreesOfFreedom)
    {
        var k = (ChiFixed)degreesOfFreedom;

        var rng = new ChiRng(ChiSeed.Scramble("ChiFixed", degreesOfFreedom));
        var expectedMean = (double)degreesOfFreedom;
        var expectedVariance = 2.0 * degreesOfFreedom;
        var expectedStdDev = Math.Sqrt(expectedVariance);
        var maxBound = expectedMean + 8 * expectedStdDev;
        var histogram = new Histogram(0, maxBound, 100);
        var sampler = new ChiFixedChiSquaredSampler(k);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedChiSquaredSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, $"Chi(k={degreesOfFreedom})^2 using ChiFixed");
        histogram.AssertIsChiSquared(degreesOfFreedom, 0.1, 0.2, 0.005);
    }

    [Fact]
    public void Sample_With2DoF_ProducesRayleighLikeDistribution()
    {
        // Chi(2) is equivalent to Rayleigh(1)
        var k = (ChiFixed)2;

        var rng = new ChiRng("ChiRayleighFixed");
        var histogram = new Histogram(0, 6, 100);
        var sampler = new ChiFixedDirectSampler(k);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedDirectSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, "Chi(k=2) using ChiFixed - should be Rayleigh(1)");

        // Rayleigh(1) has mean = sqrt(pi/2) ≈ 1.253
        var expectedMean = Math.Sqrt(Math.PI / 2);
        var actualMean = histogram.CalculateMean();
        Assert.InRange(actualMean, expectedMean * 0.95, expectedMean * 1.05);
    }

    /// <summary>Sampler that squares the Chi sample to verify Chi-squared relationship.</summary>
    private readonly struct ChiFixedChiSquaredSampler(ChiFixed degreesOfFreedom) :
        IHistogramSamplerWithRange<ChiFixed, ChiRng>
    {
        public ChiFixed NextSample(ref ChiRng rng)
        {
            var chi = rng.Chi(degreesOfFreedom).Sample();
            return chi * chi; // Chi^2 = Chi-squared
        }

        public double Normalize(ChiFixed value)
        {
            return double.CreateChecked(value);
        }
    }

    /// <summary>Sampler that returns Chi samples directly.</summary>
    private readonly struct ChiFixedDirectSampler(ChiFixed degreesOfFreedom) :
        IHistogramSamplerWithRange<ChiFixed, ChiRng>
    {
        public ChiFixed NextSample(ref ChiRng rng)
        {
            return rng.Chi(degreesOfFreedom).Sample();
        }

        public double Normalize(ChiFixed value)
        {
            return double.CreateChecked(value);
        }
    }
}