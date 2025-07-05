using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Rayleigh;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 200_000;

    [Theory]
    [InlineData(1.0)]
    [InlineData(5.0)]
    [InlineData(0.5)]
    public void Sample_WithCorrectProperties_ProducesDistribution(double sigma)
    {
        // Arrange
        var rng = new ChiRng(ChiSeed.Scramble("Rayleigh", sigma));
        var histogram = new Histogram(0, sigma * 6, 100);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Rayleigh(sigma).Sample();
            histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Rayleigh(sigma={sigma})");
        histogram.AssertIsRayleigh(sigma, 0.07);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    public void Rayleigh_WithInvalidSigma_ThrowsArgumentOutOfRangeException(double invalidSigma)
    {
        // Arrange
        var rng = new ChiRng(0);

        // Act
        Action act = () => rng.Rayleigh(invalidSigma).Sample();

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be("sigma");
    }

    [Theory]
    [InlineData("1.0")]
    [InlineData("5.0")]
    public void Sample_Decimal_WithCorrectProperties_ProducesDistribution(string sigmaStr)
    {
        // Arrange
        var sigma = decimal.Parse(sigmaStr);
        var rng = new ChiRng(ChiSeed.Scramble("RayleighDecimal", (long)(sigma * 10)));
        var histogram = new Histogram(0, (double)sigma * 6, 100);
        var sampler = new DecimalRayleighSampler(sigma);

        // Act
        histogram.Generate<decimal, ChiRng, DecimalRayleighSampler>(ref rng, 200_000, sampler);

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Rayleigh(sigma={sigma})");
        histogram.AssertIsRayleigh(sigma, 0.07);
    }

    private readonly struct DecimalRayleighSampler(decimal sigma)
        : IHistogramSamplerWithRange<decimal, ChiRng>
    {
        public decimal NextSample(ref ChiRng rng)
        {
            return rng.Rayleigh(sigma).Sample();
        }

        public double Normalize(decimal value)
        {
            return (double)value;
        }
    }
}