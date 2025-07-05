using AwesomeAssertions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.TestInfrastructure;

public static class ZipfHistogramExtensions
{
    public static void AssertIsZipf(this Histogram histogram, int numElements)
    {
        histogram.TotalSamples.Should().BeGreaterThan(0, "because samples must be generated before asserting.");
        if (numElements <= 1) return;

        long maxCount = 0;
        var peakBinIndex = -1;
        for (var i = 0; i < histogram.Bins.Length; i++)
            if (histogram.Bins[i] > maxCount)
            {
                maxCount = histogram.Bins[i];
                peakBinIndex = i;
            }

        peakBinIndex.Should().Be(0, "because rank 1 must have the highest frequency in a Zipf distribution.");

        if (histogram.Bins.Length > 2)
            histogram.Bins[0].Should()
                .BeGreaterThan(histogram.Bins[1], "because rank 1 must be more frequent than rank 2.");
        if (histogram.Bins.Length > 3)
            histogram.Bins[1].Should()
                .BeGreaterThan(histogram.Bins[2], "because rank 2 must be more frequent than rank 3.");

        var secondHalfStartIndex = histogram.Bins.Length / 2;
        long secondHalfSum = 0;
        for (var i = secondHalfStartIndex; i < histogram.Bins.Length; i++) secondHalfSum += histogram.Bins[i];
        var secondHalfAverage = (double)secondHalfSum / (histogram.Bins.Length - secondHalfStartIndex);

        if (secondHalfAverage > 0)
            (histogram.Bins[0] / secondHalfAverage).Should().BeGreaterThan(2.0,
                "because the first rank should be dramatically more frequent than the average of the tail-end ranks.");
    }
}