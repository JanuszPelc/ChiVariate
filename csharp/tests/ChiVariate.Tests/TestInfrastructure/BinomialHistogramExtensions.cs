using AwesomeAssertions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.TestInfrastructure;

public static class BinomialHistogramExtensions
{
    public static void AssertIsBinomial(this Histogram histogram, double expectedMean, double expectedStdDev,
        double tolerance)
    {
        var actualMean = histogram.CalculateMean();
        actualMean.Should().BeApproximately(expectedMean, expectedMean * tolerance,
            "because the mean of a Binomial distribution should be n * p.");

        var actualStdDev = histogram.CalculateStdDev(actualMean);
        actualStdDev.Should().BeApproximately(expectedStdDev, expectedStdDev * (tolerance * 1.5),
            "because the standard deviation of a Binomial distribution should be sqrt(n * p * (1-p)).");
    }
}