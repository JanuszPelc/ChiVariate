using FluentAssertions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.TestInfrastructure;

public static class PoissonHistogramExtensions
{
    public static void AssertIsPoisson(this Histogram histogram, double expectedMean, double tolerance)
    {
        var actualMean = histogram.CalculateMean();
        actualMean.Should().BeApproximately(expectedMean, expectedMean * tolerance,
            "because the mean of the generated samples should match the distribution's mean.");
    }
}