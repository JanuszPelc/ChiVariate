using ChiVariate.Tests.TestInfrastructure;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Poisson;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(0.5)]
    [InlineData(5.0)]
    [InlineData(20.0)]
    public void Sample_WithCorrectMean_ProducesDistribution(double mean)
    {
        // Arrange
        var rng = new ChiRng(ChiSeed.Scramble("Poisson", mean));
        var maxBound = (int)Math.Ceiling(mean + 5 * Math.Sqrt(mean));
        var histogram = new Histogram(0, maxBound, maxBound, true);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = (double)rng.Poisson(mean).Sample();
            histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper);
        histogram.AssertIsPoisson(mean, 0.05);
    }

    [Theory]
    [InlineData(-double.Epsilon)]
    [InlineData(-10.0)]
    public void Poisson_WithInvalidMean_ThrowsArgumentOutOfRangeException(double invalidMean)
    {
        // Arrange
        var rng = new ChiRng(0);

        // Act
        Action act = () => rng.Poisson(invalidMean).Sample();

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be("mean");
    }
}