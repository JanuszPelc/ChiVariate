using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerUtlTests.Chance;

public class NextBoolTests
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(0.25)]
    [InlineData(0.50)]
    [InlineData(0.75)]
    [InlineData(0.90)]
    public void NextBool_WithProbability_ReturnsTrueAtCorrectFrequency(double probability)
    {
        // Arrange
        var rng = new ChiRng((long)(probability * 100));
        var trueCount = 0;

        // Act
        for (var i = 0; i < SampleCount; i++)
            if (rng.Chance().NextBool(probability))
                trueCount++;

        // Assert
        var actualFrequency = (double)trueCount / SampleCount;
        actualFrequency.Should().BeApproximately(probability, 0.01,
            $"the frequency of true results should match the probability of {probability}");
    }

    [Fact]
    public void NextBool_WithProbabilityZero_AlwaysReturnsFalse()
    {
        // Arrange
        var rng = new ChiRng("NextBoolZero");

        // Act & Assert
        for (var i = 0; i < 1000; i++)
            rng.Chance().NextBool(0.0).Should().BeFalse("because probability is 0.0");
    }

    [Fact]
    public void NextBool_WithProbabilityOne_AlwaysReturnsTrue()
    {
        // Arrange
        var rng = new ChiRng("NextBoolOne");

        // Act & Assert
        for (var i = 0; i < 1000; i++)
            rng.Chance().NextBool(1.0).Should().BeTrue("because probability is 1.0");
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void NextBool_WithInvalidProbability_ThrowsArgumentOutOfRangeException(double invalidProbability)
    {
        // Arrange
        var rng = new ChiRng();

        // Act
        Action act = () => rng.Chance().NextBool(invalidProbability);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("probability");
    }
}