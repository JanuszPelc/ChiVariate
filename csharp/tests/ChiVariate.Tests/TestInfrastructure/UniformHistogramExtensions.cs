using AwesomeAssertions;

namespace ChiVariate.Tests.TestInfrastructure;

#pragma warning disable CS1591

public static class UniformHistogramExtensions
{
    public static void AssertIsUniform(this Histogram histogram, double tolerance)
    {
        histogram.TotalSamples.Should().BeGreaterThan(0, "because samples must be generated before asserting.");

        var outOfBoundsPercentage = (double)histogram.OutOfBoundsSamples /
                                    (histogram.TotalSamples + histogram.OutOfBoundsSamples);
        outOfBoundsPercentage.Should().BeLessThan(0.001,
            "because an insignificant number of samples should be out of bounds due to floating point inaccuracies.");

        var expectedCountPerBin = (double)histogram.TotalSamples / histogram.BinCount;
        var minAllowed = (long)(expectedCountPerBin * (1.0 - tolerance));
        var maxAllowed = (long)(expectedCountPerBin * (1.0 + tolerance));

        for (var i = 0; i < histogram.BinCount; i++)
            histogram.Bins[i].Should().BeInRange(minAllowed, maxAllowed,
                $"because bin {i} should be within {tolerance:P0} of the expected average");
    }
}