using AwesomeAssertions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.TestInfrastructure;

public static class ChiSquaredHistogramExtensions
{
    public static void AssertIsChiSquared(this Histogram histogram, int degreesOfFreedom, double meanTolerance,
        double varianceTolerance, double outOfBoundsTolerance = 0.001)
    {
        histogram.TotalSamples.Should().BeGreaterThan(0, "because samples must be generated before asserting.");

        var outOfBoundsPercentage = (double)histogram.OutOfBoundsSamples /
                                    (histogram.TotalSamples + histogram.OutOfBoundsSamples);

        outOfBoundsPercentage.Should().BeLessThan(outOfBoundsTolerance,
            "because only a tiny fraction of samples should fall far out in the long tail.");

        var expectedMean = (double)degreesOfFreedom;
        var actualMean = histogram.CalculateMean();

        actualMean.Should().BeApproximately(expectedMean, expectedMean * meanTolerance,
            "because the mean of a Chi-Squared distribution should be its degrees of freedom (k).");

        var expectedVariance = 2.0 * degreesOfFreedom;
        var actualVariance = histogram.CalculateStdDev(actualMean) * histogram.CalculateStdDev(actualMean);

        actualVariance.Should().BeApproximately(expectedVariance, expectedVariance * varianceTolerance,
            "because the variance of a Chi-Squared distribution should be 2k.");
    }
}