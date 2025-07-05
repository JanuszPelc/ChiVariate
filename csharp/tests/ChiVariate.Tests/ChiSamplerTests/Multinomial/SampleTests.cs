using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Multinomial;

public class SampleTests
{
    private const int SampleCount = 50_000;

    [Fact]
    public void Sample_ProducesDistributionsWithCorrectMeanCounts()
    {
        // Arrange
        ReadOnlySpan<double> probabilities = [0.1, 0.2, 0.3, 0.4];
        const int numTrials = 20;

        var rng = new ChiRng("Multinomial_MeanCheck");
        var multinomial = rng.Multinomial(numTrials, probabilities);
        var totalCounts = new long[probabilities.Length];

        // Act
        foreach (var buffer in multinomial.Sample(SampleCount))
            using (buffer)
            {
                for (var j = 0; j < buffer.Length; j++)
                    totalCounts[j] += buffer[j];
            }

        // Assert
        totalCounts.AssertIsMultinomial(SampleCount, numTrials, probabilities, 0.1);
    }

    [Fact]
    public void Sample_SumOfCounts_AlwaysEqualsNumTrials()
    {
        // Arrange
        var probabilities = new[] { 0.1, 0.2, 0.7 };
        const int numTrials = 100;

        var rng = new ChiRng("Multinomial_SumCheck");

        // Act & Assert
        for (var i = 0; i < 1000; i++)
        {
            using var buffer = rng.Multinomial(numTrials, probabilities).Sample();
            buffer.ToArray().Sum().Should().Be(numTrials);
        }
    }

    [Fact]
    public void Multinomial_WithEmptyProbabilities_ThrowsArgumentException()
    {
        // Arrange
        var probabilities = Array.Empty<double>();

        // Act
        var act = () =>
        {
            var rng = new ChiRng(0);
            _ = rng.Multinomial(10, probabilities).Sample();
        };

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}