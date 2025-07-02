using FluentAssertions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.TestInfrastructure;

public static class MultinomialTestExtensions
{
    public static void AssertIsMultinomial(this long[] totalCounts, int numSamples, int numTrials,
        ReadOnlySpan<double> probabilities, double tolerance)
    {
        numSamples.Should().BeGreaterThan(0);
        totalCounts.Length.Should().Be(probabilities.Length);

        for (var i = 0; i < probabilities.Length; i++)
        {
            var expectedMeanCount = numTrials * probabilities[i];
            var actualMeanCount = (double)totalCounts[i] / numSamples;

            var absoluteTolerance = Math.Max(0.1, expectedMeanCount * tolerance);

            actualMeanCount.Should().BeApproximately(expectedMeanCount, absoluteTolerance,
                $"because the mean count for category {i} should be n * p_i.");
        }
    }
}