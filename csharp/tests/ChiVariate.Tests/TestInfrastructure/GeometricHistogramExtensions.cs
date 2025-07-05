using AwesomeAssertions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.TestInfrastructure;

public static class GeometricHistogramExtensions
{
    public static void AssertIsGeometric(this Histogram histogram, double probability, double tolerance)
    {
        histogram.TotalSamples.Should().BeGreaterThan(0, "because samples must be generated before asserting.");

        var expectedMean = 1.0 / probability;
        var actualMean = histogram.CalculateMean();

        actualMean.Should().BeApproximately(expectedMean, expectedMean * tolerance,
            "because the mean of a Geometric distribution should be 1/p.");

        var stdDev = histogram.CalculateStdDev(actualMean);
        var checkUpperBound = (int)Math.Ceiling(actualMean + 3 * stdDev);
        var lastBinToCheck = Math.Min(checkUpperBound, histogram.Bins.Length - 2);

        for (var i = 0; i < lastBinToCheck; i++)
        {
            var allowedNoiseMargin = Math.Max(10, histogram.Bins[i] * 0.1);

            histogram.Bins[i].Should().BeGreaterThanOrEqualTo(histogram.Bins[i + 1] - (long)allowedNoiseMargin,
                $"because the count for bin {i} (value {i + 1}) should be generally greater than bin {i + 1} (value {i + 2}).");
        }
    }
}