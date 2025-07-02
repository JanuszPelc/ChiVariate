using FluentAssertions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.TestInfrastructure;

public static class CauchyHistogramExtensions
{
    public static void AssertIsCauchy(this Histogram histogram, double location, double tolerance)
    {
        histogram.TotalSamples.Should().BeGreaterThan(0, "because samples must be generated before asserting.");

        var expectedMedian = location;
        var actualMedian = histogram.CalculateMedian();
        var absoluteTolerance = histogram.GetBinWidth() * 5;

        actualMedian.Should().BeApproximately(expectedMedian, absoluteTolerance,
            "because the median of a Cauchy distribution should be its location parameter (x₀).");

        var outOfBoundsPercentage = (double)histogram.OutOfBoundsSamples /
                                    (histogram.TotalSamples + histogram.OutOfBoundsSamples);
        outOfBoundsPercentage.Should().BeGreaterThan(0.01,
            "because a Cauchy distribution should produce a significant number of 'outlier' values (fat tails).");
    }
}