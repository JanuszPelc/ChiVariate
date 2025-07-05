using System.Numerics;
using AwesomeAssertions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.TestInfrastructure;

public static class CategoricalHistogramExtensions
{
    public static void AssertIsCategorical<T>(this Histogram histogram, T[] probabilities,
        double tolerance)
        where T : IFloatingPoint<T>
    {
        histogram.TotalSamples.Should().BeGreaterThan(0, "because samples must be generated before asserting.");

        for (var i = 0; i < probabilities.Length; i++)
        {
            var expectedFrequency = double.CreateChecked(probabilities[i]);
            var actualFrequency = (double)histogram.Bins[i] / histogram.TotalSamples;

            actualFrequency.Should().BeApproximately(expectedFrequency, tolerance,
                $"because the frequency for category {i} should match its specified probability.");
        }
    }
}