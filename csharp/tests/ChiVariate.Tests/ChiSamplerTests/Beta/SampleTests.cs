using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Beta;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(1.0, 1.0)] // Uniform case
    [InlineData(5.0, 5.0)] // Symmetric, bell-shaped case
    [InlineData(2.0, 5.0)] // Skewed right
    [InlineData(5.0, 2.0)] // Skewed left
    [InlineData(0.5, 0.5)] // U-shaped case
    public void Sample_ProducesDistributionWithCorrectMean(double alpha, double beta)
    {
        // Arrange
        var rng = new ChiRng(ChiSeed.Scramble("Beta", alpha * 100 + beta));
        var expectedMean = alpha / (alpha + beta);
        var histogram = new Histogram(0, 1, 100);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Beta(alpha, beta).Sample();
            histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Beta(alpha={alpha}, beta={beta})");
        histogram.AssertIsBeta(expectedMean, 0.10);

        if (Math.Abs(alpha - 1.0) < 0.000001 && Math.Abs(beta - 1.0) < 0.000001)
            histogram.AssertIsUniform(0.15);
    }

    [Theory]
    [InlineData(0.0, 1.0)]
    [InlineData(-1.0, 1.0)]
    [InlineData(1.0, 0.0)]
    [InlineData(1.0, -1.0)]
    public void Beta_WithInvalidParameters_ThrowsArgumentOutOfRangeException(double alpha, double beta)
    {
        // Arrange
        var rng = new ChiRng(0);

        // Act
        Action act = () => rng.Beta(alpha, beta).Sample();

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("2.0", "5.0")] // Skewed case
    [InlineData("0.5", "0.5")] // U-shaped case
    public void Sample_Decimal_ProducesDistributionWithCorrectMean(string alphaStr, string betaStr)
    {
        // Arrange
        var alpha = decimal.Parse(alphaStr);
        var beta = decimal.Parse(betaStr);
        var expectedMean = (double)alpha / (double)(alpha + beta);

        var rng = new ChiRng(ChiSeed.Scramble("BetaDecimal", ChiHash.Hash(alpha, beta)));
        var histogram = new Histogram(0, 1, 100);
        var sampler = new DecimalBetaSampler(alpha, beta);

        // Act
        histogram.Generate<decimal, ChiRng, DecimalBetaSampler>(ref rng, 20_000, sampler);

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Beta(alpha={alpha}, beta={beta})");
        histogram.AssertIsBeta(expectedMean, 0.15);
    }

    private readonly struct DecimalBetaSampler(decimal alpha, decimal beta) :
        IHistogramSamplerWithRange<decimal, ChiRng>
    {
        public decimal NextSample(ref ChiRng rng)
        {
            return rng.Beta(alpha, beta).Sample();
        }

        public double Normalize(decimal value)
        {
            return (double)value;
        }
    }
}