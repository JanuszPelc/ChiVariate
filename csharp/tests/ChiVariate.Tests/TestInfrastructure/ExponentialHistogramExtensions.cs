using FluentAssertions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.TestInfrastructure;

public static class ExponentialHistogramExtensions
{
    public static void AssertIsExponential(this Histogram histogram, double expectedMean, double tolerance)
    {
        var actualMean = histogram.CalculateMean();
        actualMean.Should().BeApproximately(expectedMean, expectedMean * tolerance,
            "because the mean of an exponential distribution should be 1 / lambda.");

        histogram.Bins[0].Should().BeGreaterThan(histogram.Bins[1],
            "because an exponential distribution is front-loaded.");
    }
}