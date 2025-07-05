using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Binomial;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(20, 0.5)]
    [InlineData(100, 0.2)]
    public void Sample_ProducesDistributionWithCorrectStatistics(int numTrials, double probability)
    {
        // Arrange
        var rng = new ChiRng(ChiSeed.Scramble("Binomial", numTrials * 100 + probability));
        var expectedMean = numTrials * probability;
        var expectedStdDev = Math.Sqrt(numTrials * probability * (1 - probability));
        var histogram = new Histogram(0, numTrials + 1, numTrials + 1, true);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = (double)rng.Binomial(numTrials, probability).Sample();
            histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Binomial(n={numTrials}, p={probability})");
        histogram.AssertIsBinomial(expectedMean, expectedStdDev, 0.05);
    }

    [Fact]
    public void Sample_WithZeroTrials_AlwaysReturnsZero()
    {
        var rng = new ChiRng(0);
        var result = rng.Binomial(0L, 0.5).Sample();
        result.Should().Be(0);
    }

    [Theory]
    [InlineData(-1, 0.5)]
    [InlineData(10, -0.1)]
    [InlineData(10, 1.1)]
    public void Binomial_WithInvalidParameters_ThrowsArgumentOutOfRangeException(int numTrials, double probability)
    {
        // Arrange
        var rng = new ChiRng(0);

        // Act
        Action act = () => rng.Binomial(numTrials, probability).Sample();

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}