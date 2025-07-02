using FluentAssertions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.TestInfrastructure;

public static class HypergeometricHistogramExtensions
{
    public static void AssertIsHypergeometric(this Histogram histogram, double populationSize, double numSuccesses,
        double sampleSize, double tolerance)
    {
        histogram.TotalSamples.Should().BeGreaterThan(0, "because samples must be generated before asserting.");

        var expectedMean = sampleSize * (numSuccesses / populationSize);
        var actualMean = histogram.CalculateMean();

        actualMean.Should().BeApproximately(expectedMean, expectedMean * tolerance,
            "because the mean of a Hypergeometric distribution should be n * (K/N).");

        var varianceMultiplier = (populationSize - sampleSize) / (populationSize - 1);
        var expectedVariance = sampleSize * (numSuccesses / populationSize) * (1 - numSuccesses / populationSize) *
                               varianceMultiplier;
        var actualVariance = histogram.CalculateStdDev(actualMean) * histogram.CalculateStdDev(actualMean);

        if (expectedVariance > 0.01)
            actualVariance.Should().BeApproximately(expectedVariance, expectedVariance * tolerance * 2.0,
                "because the variance should match the theoretical value.");
    }
}