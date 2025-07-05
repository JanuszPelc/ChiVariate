using System.Numerics;
using AwesomeAssertions;

namespace ChiVariate.Tests.TestInfrastructure;

#pragma warning disable CS1591

public static class LogisticTestExtensions
{
    public static void AssertIsLogistic<T>(this Histogram histogram, T location, T scale, double tolerance)
        where T : IFloatingPoint<T>
    {
        histogram.TotalSamples.Should().BeGreaterThan(0);

        var expectedMean = double.CreateChecked(location);
        var actualMean = histogram.CalculateMean();

        actualMean.Should().BeApproximately(expectedMean, Math.Max(0.01, Math.Abs(expectedMean * tolerance)),
            "because the mean of a Logistic distribution is its location parameter μ.");

        var s = double.CreateChecked(scale);
        var pi = double.CreateChecked(T.Pi);
        var expectedVariance = s * s * pi * pi / 3.0;
        var actualVariance = histogram.CalculateStdDev(actualMean) * histogram.CalculateStdDev(actualMean);

        actualVariance.Should().BeApproximately(expectedVariance, expectedVariance * tolerance * 2.0,
            "because the variance of a Logistic distribution is (s² * π²) / 3.");
    }
}