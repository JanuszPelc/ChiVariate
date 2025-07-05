using System.Numerics;
using AwesomeAssertions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.TestInfrastructure;

public static class TriangularHistogramExtensions
{
    public static void AssertIsTriangular<T>(this Histogram histogram, T min, T max, T mode, double tolerance)
        where T : IFloatingPoint<T>
    {
        histogram.TotalSamples.Should().BeGreaterThan(0, "because samples must be generated before asserting.");
        histogram.OutOfBoundsSamples.Should().Be(0, "because all samples must be within the [min, max] bounds.");

        var min_d = double.CreateChecked(min);
        var max_d = double.CreateChecked(max);
        var mode_d = double.CreateChecked(mode);

        var expectedMean = (min_d + max_d + mode_d) / 3.0;
        var actualMean = histogram.CalculateMean();
        var expectedRange = max_d - min_d;

        var absoluteTolerance = expectedRange * tolerance;
        actualMean.Should().BeApproximately(expectedMean, absoluteTolerance,
            "because the mean of the generated samples should match the distribution's theoretical mean.");

        long maxCount = 0;
        var modeBinIndex = -1;
        for (var i = 0; i < histogram.Bins.Length; i++)
        {
            if (histogram.Bins[i] <= maxCount)
                continue;

            maxCount = histogram.Bins[i];
            modeBinIndex = i;
        }

        modeBinIndex.Should().NotBe(-1, "because there should be a bin with the most samples.");

        var actualMode = histogram.GetBinCenter(modeBinIndex);

        actualMode.Should().BeApproximately(mode_d, expectedRange * tolerance,
            "because the peak of the histogram should be close to the specified mode.");
    }
}