using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Laplace;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 50_000;

    [Theory]
    [InlineData(0.0, 1.0)] // Standard Laplace
    [InlineData(10.0, 5.0)] // Shifted and scaled
    [InlineData(-5.0, 0.5)] // Shifted and narrow
    public void Sample_ProducesDistributionWithCorrectStatistics(double location, double scale)
    {
        // Arrange
        var rng = new ChiRng($"Laplace_loc={location}_scale={scale}");

        var maxBound = location + 10 * scale;
        var minBound = location - 10 * scale;
        var histogram = new Histogram(minBound, maxBound, 100);

        // Act
        for (var i = 0; i < SampleCount; i++) histogram.AddSample(rng.Laplace(location, scale).Sample());

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Laplace(μ={location}, b={scale}) Distribution");
        histogram.AssertIsLaplace(location, scale, 0.1);
    }

    [Theory]
    [InlineData("10.0", "5.0")] // Shifted and scaled
    [InlineData("-5.0", "0.5")] // Shifted and narrow
    public void Sample_Decimal_ProducesDistributionWithCorrectStatistics(string locationStr, string scaleStr)
    {
        // Arrange
        var location = decimal.Parse(locationStr);
        var scale = decimal.Parse(scaleStr);

        var rng = new ChiRng(ChiSeed.Scramble("LaplaceDecimal", ChiHash.Hash(location, scale)));

        var stdDev = (double)scale * Math.Sqrt(2.0);
        var minBound = (double)location - 8 * stdDev;
        var maxBound = (double)location + 8 * stdDev;
        var histogram = new Histogram(minBound, maxBound, 100);
        var sampler = new DecimalLaplaceSampler(location, scale);

        // Act
        histogram.Generate<decimal, ChiRng, DecimalLaplaceSampler>(ref rng, 50_000, sampler);

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Laplace(μ={location}, b={scale}) Distribution");
        histogram.AssertIsLaplace(location, scale, 0.15);
    }

    private readonly struct DecimalLaplaceSampler(decimal location, decimal scale) :
        IHistogramSamplerWithRange<decimal, ChiRng>
    {
        public decimal NextSample(ref ChiRng rng)
        {
            return rng.Laplace(location, scale).Sample();
        }

        public double Normalize(decimal value)
        {
            return (double)value;
        }
    }
}