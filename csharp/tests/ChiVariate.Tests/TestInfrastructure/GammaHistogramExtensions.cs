using FluentAssertions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.TestInfrastructure;

public static class GammaHistogramExtensions
{
    public static void AssertIsGamma(
        this Histogram histogram, double expectedMean, double expectedStdDev, double tolerance)
    {
        var actualMean = histogram.CalculateMean();
        actualMean.Should().BeApproximately(expectedMean, expectedMean * tolerance,
            "because the mean of a Gamma distribution should be shape * scale.");

        var actualStdDev = histogram.CalculateStdDev(actualMean);
        actualStdDev.Should().BeApproximately(expectedStdDev, expectedStdDev * (tolerance * 1.5),
            "because the standard deviation of a Gamma distribution should be sqrt(shape) * scale.");
    }
}