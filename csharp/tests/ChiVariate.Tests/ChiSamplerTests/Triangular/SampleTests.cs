using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Triangular;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(0.0, 10.0, 5.0)] // Symmetric
    [InlineData(0.0, 10.0, 2.0)] // Left-skewed
    [InlineData(0.0, 10.0, 8.0)] // Right-skewed
    [InlineData(-10.0, 10.0, 0.0)] // Symmetric around zero
    [InlineData(50.0, 60.0, 51.0)] // Narrow and shifted
    public void Sample_ProducesDistributionWithCorrectProperties(double min, double max, double mode)
    {
        // Arrange
        var rng = new ChiRng(ChiSeed.Scramble("Triangular", mode));
        var histogram = new Histogram(min, max, 100);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Triangular(min, max, mode).Sample();
            histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Triangular(min={min}, max={max}, mode={mode})");
        histogram.AssertIsTriangular(min, max, mode, 0.05);
    }

    [Theory]
    [InlineData(10, 0, 5)] // min > max
    [InlineData(0, 10, -1)] // mode < min
    [InlineData(0, 10, 11)] // mode > max
    [InlineData(5, 5, 5)] // min = max
    public void Triangular_WithInvalidParameters_ThrowsException(double min, double max, double mode)
    {
        // Arrange
        var rng = new ChiRng(0);

        // Act
        Action act = () => rng.Triangular(min, max, mode).Sample();

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("0.0", "10.0", "2.0")] // Left-skewed
    [InlineData("-10.0", "10.0", "0.0")] // Symmetric around zero
    public void Sample_Decimal_ProducesDistributionWithCorrectProperties(string minStr, string maxStr, string modeStr)
    {
        // Arrange
        var min = decimal.Parse(minStr);
        var max = decimal.Parse(maxStr);
        var mode = decimal.Parse(modeStr);

        var rng = new ChiRng(ChiSeed.Scramble("TriangularDecimal", mode));
        var histogram = new Histogram((double)min, (double)max, 100);
        var sampler = new DecimalTriangularSampler(min, max, mode);

        // Act
        histogram.Generate<decimal, ChiRng, DecimalTriangularSampler>(ref rng, 200_000, sampler);

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Triangular(min={min}, max={max}, mode={mode})");
        histogram.AssertIsTriangular(min, max, mode, 0.05);
    }

    private readonly struct DecimalTriangularSampler(decimal min, decimal max, decimal mode)
        : IHistogramSamplerWithRange<decimal, ChiRng>
    {
        public decimal NextSample(ref ChiRng rng)
        {
            return rng.Triangular(min, max, mode).Sample();
        }

        public double Normalize(decimal value)
        {
            return (double)value;
        }
    }
}