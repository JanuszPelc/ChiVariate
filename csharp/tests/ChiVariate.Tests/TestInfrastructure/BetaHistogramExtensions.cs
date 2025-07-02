using FluentAssertions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.TestInfrastructure;

public static class BetaHistogramExtensions
{
    public static void AssertIsBeta(this Histogram histogram, double expectedMean, double tolerance)
    {
        var actualMean = histogram.CalculateMean();
        actualMean.Should().BeApproximately(expectedMean, expectedMean * tolerance,
            "because the mean of a Beta distribution should be alpha / (alpha + beta).");
    }
}