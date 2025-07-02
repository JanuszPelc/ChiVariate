using System.Numerics;
using FluentAssertions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.TestInfrastructure;

public static class ParetoHistogramExtensions
{
    public static void AssertIsPareto<T>(this Histogram histogram, T scale, T shape, double tolerance)
        where T : IFloatingPoint<T>
    {
        histogram.TotalSamples.Should().BeGreaterThan(0, "because samples must be generated before asserting.");

        long maxCount = 0;
        var peakBinIndex = -1;
        for (var i = 0; i < histogram.Bins.Length; i++)
        {
            if (histogram.Bins[i] <= maxCount)
                continue;

            maxCount = histogram.Bins[i];
            peakBinIndex = i;
        }

        peakBinIndex.Should().Be(0, "because the mode of the Pareto distribution is at x_m (the first bin).");

        var scaleD = double.CreateChecked(scale);
        var shapeD = double.CreateChecked(shape);

        var expectedMedian = scaleD * Math.Pow(2.0, 1.0 / shapeD);
        var actualMedian = histogram.CalculateMedian();
        actualMedian.Should().BeApproximately(expectedMedian, expectedMedian * tolerance,
            "because the median should match the theoretical formula.");

        if (!(shapeD > 2.0)) return;

        var expectedMean = shapeD * scaleD / (shapeD - 1.0);
        var actualMean = histogram.CalculateMean();

        actualMean.Should().BeApproximately(expectedMean, expectedMean * tolerance,
            "because the mean is finite and should match the theoretical formula for alpha > 2.");
    }
}