using System.Numerics;
using AwesomeAssertions;

namespace ChiVariate.Tests.TestInfrastructure;

#pragma warning disable CS1591

public static class StudentTTestExtensions
{
    public static void AssertIsStudentT<T>(this Histogram histogram, T degreesOfFreedom, double tolerance)
        where T : IFloatingPoint<T>
    {
        histogram.TotalSamples.Should().BeGreaterThan(0);
        var v = double.CreateChecked(degreesOfFreedom);

        if (v > 1)
        {
            var actualMean = histogram.CalculateMean();
            actualMean.Should().BeApproximately(0.0, 0.1,
                "because the mean of a t-distribution with v > 1 is 0.");
        }

        if (v > 2)
        {
            var expectedVariance = v / (v - 2.0);
            var actualVariance = histogram.CalculateStdDev(histogram.CalculateMean()) *
                                 histogram.CalculateStdDev(histogram.CalculateMean());
            actualVariance.Should().BeApproximately(expectedVariance, expectedVariance * tolerance,
                "because the variance should match v / (v - 2).");
        }
    }
}