using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.ChiSquared;

/// <summary>
///     Tests for Chi-squared distribution using ChiFixed type.
///     Chi-squared is the sum of k squared standard normals, directly exercising the Ziggurat algorithm.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(2)] // Exponential-like (most common in hypothesis testing)
    [InlineData(5)] // Skewed right
    [InlineData(10)] // More symmetric, approaching normal
    public void Sample_WithIntegerDegreesOfFreedom_ProducesChiSquaredDistribution(int degreesOfFreedom)
    {
        var k = (ChiFixed)degreesOfFreedom;

        var rng = new ChiRng(ChiSeed.Scramble("ChiSquaredFixed", degreesOfFreedom));
        var expectedMean = (double)degreesOfFreedom;
        var expectedStdDev = Math.Sqrt(2.0 * degreesOfFreedom);
        var maxBound = expectedMean + 5 * expectedStdDev;
        var histogram = new Histogram(0, maxBound, 150);
        var sampler = new ChiFixedChiSquaredSampler(k);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedChiSquaredSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, $"ChiSquared(k={degreesOfFreedom}) using ChiFixed");
        histogram.AssertIsChiSquared(degreesOfFreedom, 0.1, 0.15, 0.005);
    }

    [Fact]
    public void Sample_WithLargeDegreesOfFreedom_ApproachesNormal()
    {
        // For large k, Chi-squared(k) approaches Normal(k, sqrt(2k))
        var k = (ChiFixed)30;

        var rng = new ChiRng("ChiSquaredLargeK");
        var expectedMean = 30.0;
        var expectedStdDev = Math.Sqrt(60.0);
        var histogram = new Histogram(0, expectedMean + 5 * expectedStdDev, 150);
        var sampler = new ChiFixedChiSquaredSampler(k);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedChiSquaredSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, "ChiSquared(k=30) using ChiFixed - should approach Normal");

        var actualMean = histogram.CalculateMean();
        var actualStdDev = histogram.CalculateStdDev(actualMean);

        // For large k, mean and stddev should match theoretical values
        Assert.InRange(actualMean, expectedMean * 0.95, expectedMean * 1.05);
        Assert.InRange(actualStdDev, expectedStdDev * 0.90, expectedStdDev * 1.10);
    }

    private readonly struct ChiFixedChiSquaredSampler(ChiFixed degreesOfFreedom) :
        IHistogramSamplerWithRange<ChiFixed, ChiRng>
    {
        public ChiFixed NextSample(ref ChiRng rng)
        {
            return rng.ChiSquared(degreesOfFreedom).Sample();
        }

        public double Normalize(ChiFixed value)
        {
            return double.CreateChecked(value);
        }
    }
}