using System.Numerics;
using AwesomeAssertions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.TestInfrastructure;

public static class WeibullHistogramExtensions
{
    public static void AssertIsWeibull<T>(this Histogram histogram, T shape, T scale, double tolerance)
        where T : IFloatingPoint<T>
    {
        histogram.TotalSamples.Should().BeGreaterThan(0, "because samples must be generated before asserting.");

        var shapeD = double.CreateChecked(shape);
        var scaleD = double.CreateChecked(scale);

        if (Math.Abs(shapeD - 1.0) < 1e-9)
        {
            var expHistogram = (Histogram)histogram.Clone();
            expHistogram.AssertIsExponential(scaleD, tolerance);
        }
        else
        {
            var expectedMedian = scaleD * Math.Pow(Math.Log(2), 1.0 / shapeD);
            var actualMedian = histogram.CalculateMedian();

            actualMedian.Should().BeApproximately(expectedMedian, expectedMedian * tolerance,
                "because the median of a Weibull distribution should match the theoretical formula.");
        }
    }
}