using ChiVariate.Tests.TestInfrastructure;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Hypergeometric;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(52, 13, 5)] // Drawing 5 cards from a 52-card deck with 13 hearts
    [InlineData(100, 20, 10)] // Drawing 10 marbles from a bag of 100 with 20 red ones
    [InlineData(50, 40, 10)] // High probability of success
    [InlineData(50, 5, 20)] // Low probability of success
    public void Sample_ProducesDistributionWithCorrectStatistics(int populationSize, int numSuccesses, int sampleSize)
    {
        // Arrange
        var rng = new ChiRng(ChiSeed.Scramble("Hypergeometric",
            ChiHash.Hash(populationSize, numSuccesses, sampleSize)));

        var minPossible = Math.Max(0, sampleSize - (populationSize - numSuccesses));
        var maxPossible = Math.Min(sampleSize, numSuccesses);

        var histogram = new Histogram(minPossible, maxPossible + 1, maxPossible - minPossible + 1, true);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Hypergeometric(populationSize, numSuccesses, sampleSize).Sample();
            histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Hypergeometric(N={populationSize}, K={numSuccesses}, n={sampleSize})");
        histogram.AssertIsHypergeometric(populationSize, numSuccesses, sampleSize, 0.1);
    }

    [Theory]
    [InlineData(10, 11, 5)] // K > N
    [InlineData(10, 5, 11)] // n > N
    [InlineData(-10, 5, 5)] // N < 0
    public void Hypergeometric_WithInvalidParameters_ThrowsArgumentOutOfRangeException(int populationSize,
        int numSuccesses, int sampleSize)
    {
        // Arrange
        var rng = new ChiRng(0);

        // Act
        Action act = () => rng.Hypergeometric(populationSize, numSuccesses, sampleSize).Sample();

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}