using System.Numerics;
using AwesomeAssertions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.TestInfrastructure;

public static class RayleighHistogramExtensions
{
    public static void AssertIsRayleigh<T>(this Histogram histogram, T sigma, double tolerance)
        where T : IFloatingPoint<T>
    {
        histogram.TotalSamples.Should().BeGreaterThan(0, "because samples must be generated before asserting.");
        histogram.OutOfBoundsSamples.Should().Be(0, "because all samples must be positive.");

        var sigmaD = double.CreateChecked(sigma);

        var expectedMean = sigmaD * Math.Sqrt(Math.PI / 2.0);
        var actualMean = histogram.CalculateMean();

        actualMean.Should().BeApproximately(expectedMean, sigmaD * tolerance,
            "because the mean of the generated samples should match the distribution's theoretical mean.");

        var actualMode = histogram.CalculateMode();

        actualMode.Should().BeApproximately(sigmaD, sigmaD * tolerance,
            "because the peak of the histogram should be close to the specified mode (sigma).");
    }
}