using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Cauchy;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(0.0, 1.0)] // Standard Cauchy
    [InlineData(10.0, 5.0)] // Shifted and scaled
    [InlineData(0.0, 0.2)] // Narrower peak
    public void Sample_ProducesDistributionWithCorrectMedian(double location, double scale)
    {
        // Arrange
        var rng = new ChiRng(ChiSeed.Scramble("Cauchy", new ChiHash().Add(location).Add(scale).Hash));
        var histogramRange = 10 * scale;
        var minBound = location - histogramRange;
        var maxBound = location + histogramRange;

        var histogram = new Histogram(minBound, maxBound, 201);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Cauchy(location, scale).Sample();
            histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Cauchy(x₀={location}, γ={scale})");
        histogram.AssertIsCauchy(location, 0.1);
    }

    [Theory]
    [InlineData(0.0, 0.0)]
    [InlineData(0.0, -1.0)]
    public void Cauchy_WithInvalidScale_ThrowsArgumentOutOfRangeException(double location, double scale)
    {
        // Arrange
        var rng = new ChiRng(0);

        // Act
        Action act = () => rng.Cauchy(location, scale).Sample();

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be("scale");
    }

    [Theory]
    [InlineData("0.0", "1.0")] // Standard Cauchy
    [InlineData("10.0", "5.0")] // Shifted and scaled
    public void Sample_Decimal_ProducesDistributionWithCorrectMedian(string locationStr, string scaleStr)
    {
        // Arrange
        var location = decimal.Parse(locationStr);
        var scale = decimal.Parse(scaleStr);

        var rng = new ChiRng(ChiSeed.Scramble("CauchyDecimal", new ChiHash().Add(location).Add(scale).Hash));
        var histogramRange = (double)(10 * scale);
        var minBound = (double)location - histogramRange;
        var maxBound = (double)location + histogramRange;

        var histogram = new Histogram(minBound, maxBound, 201);
        var sampler = new DecimalCauchySampler(location, scale);

        // Act
        histogram.Generate<decimal, ChiRng, DecimalCauchySampler>(ref rng, 100_000, sampler);

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Cauchy(x₀={location}, γ={scale})");
        histogram.AssertIsCauchy((double)location, 0.15);
    }

    private readonly struct DecimalCauchySampler(decimal location, decimal scale) :
        IHistogramSamplerWithRange<decimal, ChiRng>
    {
        public decimal NextSample(ref ChiRng rng)
        {
            return rng.Cauchy(location, scale).Sample();
        }

        public double Normalize(decimal value)
        {
            return (double)value;
        }
    }
}