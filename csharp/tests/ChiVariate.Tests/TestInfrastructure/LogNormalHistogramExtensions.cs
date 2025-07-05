using System.Numerics;
using AwesomeAssertions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.TestInfrastructure;

public static class LogNormalHistogramExtensions
{
    public static void AssertIsLogNormal<T>(this Histogram histogram, T mu, T sigma, double tolerance)
        where T : IFloatingPoint<T>
    {
        histogram.TotalSamples.Should().BeGreaterThan(0, "because samples must be generated before asserting.");

        var underflowPercentage = (double)histogram.OutOfBoundsSamples / histogram.TotalSamples;
        underflowPercentage.Should()
            .BeLessThan(0.01, "because only a tiny fraction of samples should underflow to zero.");

        var muD = double.CreateChecked(mu);
        var sigmaD = double.CreateChecked(sigma);

        var expectedMean = Math.Exp(muD + sigmaD * sigmaD / 2.0);
        var actualMean = histogram.CalculateMean();

        actualMean.Should().BeApproximately(expectedMean, expectedMean * tolerance,
            "because the mean of a Log-Normal distribution should match the theoretical formula.");

        var expectedMedian = Math.Exp(muD);
        var actualMedian = histogram.CalculateMedian();

        actualMedian.Should().BeApproximately(expectedMedian, expectedMedian * tolerance * 1.5,
            "because the median of a Log-Normal distribution should be exp(mu).");
    }
}