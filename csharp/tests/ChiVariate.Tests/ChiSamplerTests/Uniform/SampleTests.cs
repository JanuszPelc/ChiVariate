using ChiVariate.Tests.TestInfrastructure;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Uniform;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Fact]
    public void Sample_WithStandardRange_ProducesUniformDistribution()
    {
        // Arrange
        var rng = new ChiRng(ChiHash.Hash("BoundedUniform_Standard"));
        const double min = 0.0;
        const double max = 1.0;
        var histogram = new Histogram(min, max, 10);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Uniform(min, max).Sample();
            histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper);
        histogram.AssertIsUniform(0.05);
    }

    [Fact]
    public void Sample_WithShiftedRange_ProducesUniformDistribution()
    {
        // Arrange
        var rng = new ChiRng(ChiHash.Hash("BoundedUniform_Shifted"));
        const float min = -100.0f;
        const float max = 100.0f;
        var histogram = new Histogram(min, max, 20);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Uniform(min, max).Sample();
            histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper);
        histogram.AssertIsUniform(0.10);
    }

    [Fact]
    public void Sample_Always_StaysWithinBounds()
    {
        // Arrange
        var rng = new ChiRng(ChiHash.Hash("BoundedUniform_BoundsCheck"));
        const double min = 10.0;
        const double max = 20.0;

        // Act & Assert
        for (var i = 0; i < 10000; i++)
        {
            var sample = rng.Uniform(min, max).Sample();
            sample.Should().BeGreaterThanOrEqualTo(min).And.BeLessThan(max);
        }
    }

    [Fact]
    public void Uniform_WithInvalidRange_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var rng = new ChiRng(0);

        // Act
        Action act = () => rng.Uniform(10.0, 5.0).Sample();

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}