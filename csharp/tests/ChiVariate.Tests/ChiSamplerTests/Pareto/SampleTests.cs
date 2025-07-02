using ChiVariate.Tests.TestInfrastructure;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Pareto;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(1.0, 1.16)] // Classic 80/20 rule shape (alpha ≈ 1.16)
    [InlineData(1.0, 2.0)] // Finite mean, infinite variance
    [InlineData(10.0, 3.0)] // Finite mean and variance
    public void Sample_ProducesDistributionWithCorrectShape(double scale, double shape)
    {
        // Arrange
        var rng = new ChiRng(ChiSeed.Scramble("Pareto", ChiHash.Hash(scale, shape)));
        var maxBound = scale * 10;
        var histogram = new Histogram(scale, maxBound, 150);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Pareto(scale, shape).Sample();
            if (sample < maxBound) histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Pareto(xₘ={scale}, α={shape})");
        histogram.AssertIsPareto(scale, shape, 0.2);
    }

    [Theory]
    [InlineData(0.0, 1.0)]
    [InlineData(1.0, 0.0)]
    public void Pareto_WithInvalidParameters_ThrowsArgumentOutOfRangeException(double scale, double shape)
    {
        // Arrange
        var rng = new ChiRng(0);

        // Act
        Action act = () => rng.Pareto(scale, shape).Sample();

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("10.0", "3.0")] // Finite mean and variance
    [InlineData("1.0", "2.1")] // Finite mean, infinite variance
    public void Sample_Decimal_ProducesDistributionWithCorrectShape(string scaleStr, string shapeStr)
    {
        // Arrange
        var scale = decimal.Parse(scaleStr);
        var shape = decimal.Parse(shapeStr);

        var rng = new ChiRng(ChiSeed.Scramble("ParetoDecimal", ChiHash.Hash(scale, shape)));
        var maxBound = (double)scale * 10;
        var histogram = new Histogram((double)scale, maxBound, 150);
        var sampler = new DecimalParetoSampler(scale, shape);

        // Act
        histogram.Generate<decimal, ChiRng, DecimalParetoSampler>(ref rng, 100_000, sampler);

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Pareto(xₘ={scale}, α={shape})");
        histogram.AssertIsPareto(scale, shape, 0.25);
    }

    private readonly struct DecimalParetoSampler(decimal scale, decimal shape)
        : IHistogramSamplerWithRange<decimal, ChiRng>
    {
        public decimal NextSample(ref ChiRng rng)
        {
            return rng.Pareto(scale, shape).Sample();
        }

        public double Normalize(decimal value)
        {
            return (double)value;
        }
    }
}