using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Categorical;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Fact]
    public void Sample_WithSimpleWeights_ProducesCorrectDistribution()
    {
        // Arrange
        var probabilities = new[] { 0.1f, 0.6f, 0.3f };
        var rng = new ChiRng("Categorical_Simple");
        var histogram = new Histogram(0, probabilities.Length, probabilities.Length, true);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Categorical(probabilities).Sample();
            histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper);
        histogram.AssertIsCategorical(probabilities, 0.01);
    }

    [Fact]
    public void Sample_WithManyCategories_ProducesCorrectDistribution()
    {
        // Arrange
        var probabilities = new[] { 0.05, 0.1, 0.15, 0.2, 0.25, 0.15, 0.1 }; // Sums to 1.0
        var rng = new ChiRng("Categorical_Many");
        var categorical = rng.Categorical((ReadOnlySpan<double>)probabilities);
        var histogram = new Histogram(0, probabilities.Length, probabilities.Length, true);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = categorical.Sample();
            histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper);
        histogram.AssertIsCategorical(probabilities, 0.01);
    }

    [Fact]
    public void Categorical_WithSingleCategory_AlwaysReturnsZero()
    {
        var probabilities = new[] { 1.0 };
        var rng = new ChiRng(0);

        for (var i = 0; i < 100; i++)
            rng.Categorical(probabilities).Sample().Should().Be(0);
    }

    [Fact]
    public void Categorical_WithEmptyProbabilities_ThrowsArgumentException()
    {
        // Arrange
        var probabilities = Array.Empty<float>();
        var rng = new ChiRng(0);

        // Act
        Action act = () => rng.Categorical((ReadOnlySpan<float>)probabilities).Sample();

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}