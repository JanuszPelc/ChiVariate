using ChiVariate.Tests.TestInfrastructure;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Weibull;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 150_000;

    [Theory]
    [InlineData(1.0, 5.0)] // k=1, identical to Exponential(1/5)
    [InlineData(0.5, 5.0)] // k<1, decreasing failure rate (L-shape)
    [InlineData(2.0, 5.0)] // k>1, increasing failure rate (Rayleigh-like)
    [InlineData(5.0, 10.0)] // k>>1, bell-shaped (approaching Normal)
    public void Sample_ProducesDistributionWithCorrectShape(double shape, double scale)
    {
        // Arrange
        var rng = new ChiRng(ChiSeed.Scramble("Weibull", ChiHash.Hash(shape, scale)));
        var maxBound = scale * 10;
        var histogram = new Histogram(0, maxBound, 150);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Weibull(shape, scale).Sample();
            if (sample < maxBound) histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Weibull(k={shape}, λ={scale})");
        histogram.AssertIsWeibull(shape, scale, 0.15);
    }

    [Theory]
    [InlineData(0.0, 1.0)]
    [InlineData(1.0, 0.0)]
    public void Weibull_WithInvalidParameters_ThrowsArgumentOutOfRangeException(double shape, double scale)
    {
        // Arrange
        var rng = new ChiRng(0);

        // Act
        Action act = () => rng.Weibull(shape, scale).Sample();

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("2.0", "5.0")] // k>1, increasing failure rate
    [InlineData("5.0", "10.0")] // k>>1, bell-shaped
    public void Sample_Decimal_ProducesDistributionWithCorrectShape(string shapeStr, string scaleStr)
    {
        // Arrange
        var shape = decimal.Parse(shapeStr);
        var scale = decimal.Parse(scaleStr);

        var rng = new ChiRng(ChiSeed.Scramble("WeibullDecimal", ChiHash.Hash(shape, scale)));
        var maxBound = (double)scale * 4; // Weibull can have a long tail
        var histogram = new Histogram(0, maxBound, 150);
        var sampler = new DecimalWeibullSampler(shape, scale);

        // Act
        histogram.Generate<decimal, ChiRng, DecimalWeibullSampler>(ref rng, 150_000, sampler);

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Weibull(k={shape}, λ={scale})");
        histogram.AssertIsWeibull(shape, scale, 0.20);
    }

    private readonly struct DecimalWeibullSampler(decimal shape, decimal scale)
        : IHistogramSamplerWithRange<decimal, ChiRng>
    {
        public decimal NextSample(ref ChiRng rng)
        {
            return rng.Weibull(shape, scale).Sample();
        }

        public double Normalize(decimal value)
        {
            return (double)value;
        }
    }
}