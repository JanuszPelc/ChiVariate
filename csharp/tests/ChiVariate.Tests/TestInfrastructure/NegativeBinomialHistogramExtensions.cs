using AwesomeAssertions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.TestInfrastructure;

public static class NegativeBinomialHistogramExtensions
{
    public static void AssertIsNegativeBinomial(this Histogram histogram, double numSuccesses, double probability,
        double tolerance)
    {
        histogram.TotalSamples.Should().BeGreaterThan(0, "because samples must be generated before asserting.");

        var expectedMean = numSuccesses / probability;
        var actualMean = histogram.CalculateMean();

        actualMean.Should().BeApproximately(expectedMean, expectedMean * tolerance,
            "because the mean of a Negative Binomial distribution should be r/p.");

        var expectedVariance = numSuccesses * (1 - probability) / (probability * probability);
        var actualVariance = histogram.CalculateStdDev(actualMean) * histogram.CalculateStdDev(actualMean);

        actualVariance.Should().BeApproximately(expectedVariance, expectedVariance * tolerance * 1.5,
            "because the variance should also match the theoretical value.");
    }
}