using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.F;

/// <summary>
///     Tests for F distribution using ChiFixed type.
///     F is the ratio of two Chi-squared distributions, testing the full Ziggurat chain.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(5, 10)] // Test variance for d2 >= 10
    [InlineData(10, 20)] // Well-behaved case
    [InlineData(20, 30)] // Less skewed
    public void Sample_AcrossDoF_MatchesFDistribution(int d1, int d2)
    {
        var df1 = (ChiFixed)d1;
        var df2 = (ChiFixed)d2;

        var rng = new ChiRng(ChiSeed.Scramble("FFixed", d1 * 100 + d2));
        var expectedMean = (double)d2 / (d2 - 2);
        var expectedVariance = 2.0 * d2 * d2 * (d1 + d2 - 2) / (d1 * Math.Pow(d2 - 2, 2) * (d2 - 4));
        var expectedStdDev = Math.Sqrt(expectedVariance);
        var maxBound = expectedMean + 8 * expectedStdDev;
        var histogram = new Histogram(0, maxBound, 150);
        var sampler = new ChiFixedFSampler(df1, df2);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedFSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, $"F(d1={d1}, d2={d2}) using ChiFixed");

        var actualMean = histogram.CalculateMean();
        actualMean.Should().BeApproximately(expectedMean, expectedMean * 0.15,
            "because the mean of F distribution should be d2/(d2-2).");

        var actualVariance = Math.Pow(histogram.CalculateStdDev(actualMean), 2);
        actualVariance.Should().BeApproximately(expectedVariance, expectedVariance * 0.25,
            "because the variance of F distribution follows the theoretical formula.");
    }

    [Fact]
    public void Sample_LargeDoF_ApproachesOne()
    {
        // As both d1 and d2 become large, F-distribution peaks sharply at 1
        var df1 = (ChiFixed)100m;
        var df2 = (ChiFixed)100m;

        var rng = new ChiRng("FLargeDoFFixed");
        var histogram = new Histogram(0, 3, 100);
        var sampler = new ChiFixedFSampler(df1, df2);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedFSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, "F(100, 100) using ChiFixed - should peak at 1");

        var actualMean = histogram.CalculateMean();
        actualMean.Should().BeApproximately(1.0, 0.1,
            "because F(large, large) peaks sharply at 1.");
    }

    private readonly struct ChiFixedFSampler(ChiFixed d1, ChiFixed d2) :
        IHistogramSamplerWithRange<ChiFixed, ChiRng>
    {
        public ChiFixed NextSample(ref ChiRng rng)
        {
            return rng.F(d1, d2).Sample();
        }

        public double Normalize(ChiFixed value)
        {
            return double.CreateChecked(value);
        }
    }
}