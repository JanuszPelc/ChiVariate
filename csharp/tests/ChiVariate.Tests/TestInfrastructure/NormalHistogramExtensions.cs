using FluentAssertions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.TestInfrastructure;

public static class NormalHistogramExtensions
{
    public static void AssertIsNormal(
        this Histogram histogram, double expectedMean, double expectedStdDev, double tolerance)
    {
        var actualMean = histogram.CalculateMean();
        actualMean.Should().BeApproximately(expectedMean, expectedStdDev * 0.1);

        var actualStdDev = histogram.CalculateStdDev(actualMean);
        actualStdDev.Should().BeApproximately(expectedStdDev, expectedStdDev * 0.1);

        long oneStdDevCount = 0;
        var lowerBound = actualMean - actualStdDev;
        var upperBound = actualMean + actualStdDev;

        for (var i = 0; i < histogram.BinCount; i++)
        {
            var binCenter = histogram.GetBinCenter(i);
            if (binCenter >= lowerBound && binCenter <= upperBound) oneStdDevCount += histogram.Bins[i];
        }

        var percentageWithinOneStdDev = (double)oneStdDevCount / histogram.TotalSamples;

        percentageWithinOneStdDev.Should().BeApproximately(0.68, tolerance,
            "because approximately 68% of samples should fall within one standard deviation of the mean.");
    }
}