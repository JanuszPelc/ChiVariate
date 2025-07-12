using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Logistic;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 250_000;

    [Theory]
    [InlineData(0.0, 1.0)] // Standard Logistic
    [InlineData(5.0, 2.0)] // Shifted and scaled
    [InlineData(-10.0, 0.5)] // Shifted and narrow
    public void Sample_ProducesDistributionWithCorrectStatistics(double location, double scale)
    {
        // Arrange
        var rng = new ChiRng($"Logistic_loc={location}_scale={scale}");

        var stdDev = scale * Math.PI / Math.Sqrt(3.0);
        var minBound = location - 6 * stdDev;
        var maxBound = location + 6 * stdDev;
        var histogram = new Histogram(minBound, maxBound, 100);

        // Act
        for (var i = 0; i < SampleCount; i++)
            histogram.AddSample(rng.Logistic(location, scale).Sample());

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Logistic(μ={location}, s={scale}) Distribution");
        histogram.AssertIsLogistic(location, scale, 0.1);
    }

    [Theory]
    [InlineData("5.0", "2.0")] // Shifted and scaled
    [InlineData("-10.0", "0.5")] // Shifted and narrow
    public void Sample_Decimal_ProducesDistributionWithCorrectStatistics(string locationStr, string scaleStr)
    {
        // Arrange
        var location = decimal.Parse(locationStr);
        var scale = decimal.Parse(scaleStr);

        var rng = new ChiRng(ChiSeed.Scramble("LogisticDecimal", new ChiHash().Add(location).Add(scale).Hash));

        var stdDev = (double)(scale * (decimal)Math.PI / (decimal)Math.Sqrt(3.0));
        var minBound = (double)location - 6 * stdDev;
        var maxBound = (double)location + 6 * stdDev;
        var histogram = new Histogram(minBound, maxBound, 100);
        var sampler = new DecimalLogisticSampler(location, scale);

        // Act
        histogram.Generate<decimal, ChiRng, DecimalLogisticSampler>(ref rng, 50_000, sampler);

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Logistic(μ={location}, s={scale}) Distribution");
        histogram.AssertIsLogistic(location, scale, 0.15);
    }

    private readonly struct DecimalLogisticSampler(decimal location, decimal scale) :
        IHistogramSamplerWithRange<decimal, ChiRng>
    {
        public decimal NextSample(ref ChiRng rng)
        {
            return rng.Logistic(location, scale).Sample();
        }

        public double Normalize(decimal value)
        {
            return (double)value;
        }
    }
}