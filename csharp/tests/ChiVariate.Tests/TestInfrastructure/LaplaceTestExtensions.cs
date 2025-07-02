using System.Numerics;
using FluentAssertions;

namespace ChiVariate.Tests.TestInfrastructure;

#pragma warning disable CS1591

public static class LaplaceTestExtensions
{
    public static void AssertIsLaplace<T>(this Histogram histogram, T location, T scale, double tolerance)
        where T : IFloatingPoint<T>
    {
        histogram.TotalSamples.Should().BeGreaterThan(0);

        var expectedMean = double.CreateChecked(location);
        var actualMean = histogram.CalculateMean();
        actualMean.Should().BeApproximately(expectedMean, Math.Max(0.01, Math.Abs(expectedMean * tolerance)),
            "because the mean of a Laplace distribution is its location parameter μ.");

        var b = double.CreateChecked(scale);
        var expectedVariance = 2.0 * b * b;
        var actualVariance = histogram.CalculateStdDev(actualMean) * histogram.CalculateStdDev(actualMean);
        actualVariance.Should().BeApproximately(expectedVariance, expectedVariance * tolerance * 2.0,
            "because the variance of a Laplace distribution is 2b².");
    }
}