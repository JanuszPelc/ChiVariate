using ChiVariate.Tests.TestInfrastructure;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.LogNormal;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(0.0, 1.0)] // Standard Log-Normal
    [InlineData(1.0, 0.5)] // Shifted, less skewed
    [InlineData(0.0, 0.25)] // Tightly clustered near 1
    public void Sample_ProducesDistributionWithCorrectStatistics(double mu, double sigma)
    {
        // Arrange
        var rng = new ChiRng(ChiSeed.Scramble("LogNormal", ChiHash.Hash(mu, sigma)));

        var expectedMean = Math.Exp(mu + sigma * sigma / 2.0);
        var expectedVariance = (Math.Exp(sigma * sigma) - 1) * Math.Exp(2 * mu + sigma * sigma);
        var expectedStdDev = Math.Sqrt(expectedVariance);

        var maxBound = expectedMean + 4 * expectedStdDev;
        var histogram = new Histogram(0, maxBound, 100);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.LogNormal(mu, sigma).Sample();
            if (sample < maxBound)
                histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper, $"LogNormal(mu={mu}, sigma={sigma})");
        histogram.AssertIsLogNormal(mu, sigma, 0.1);
    }

    [Theory]
    [InlineData(0.0, 0.0)]
    [InlineData(0.0, -1.0)]
    public void LogNormal_WithInvalidSigma_ThrowsArgumentOutOfRangeException(double logMean, double logStdDev)
    {
        // Arrange
        var rng = new ChiRng(0);

        // Act
        Action act = () => rng.LogNormal(logMean, logStdDev).Sample();

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be("logStandardDeviation");
    }

    [Theory]
    [InlineData("1.0", "0.5")] // Shifted, less skewed
    [InlineData("0.0", "0.25")] // Tightly clustered near 1
    public void Sample_Decimal_ProducesDistributionWithCorrectStatistics(string muStr, string sigmaStr)
    {
        // Arrange
        var mu = decimal.Parse(muStr);
        var sigma = decimal.Parse(sigmaStr);

        var rng = new ChiRng(ChiSeed.Scramble("LogNormalDecimal", ChiHash.Hash(mu, sigma)));

        var expectedMean = Math.Exp((double)mu + (double)sigma * (double)sigma / 2.0);
        var expectedVariance = (Math.Exp((double)sigma * (double)sigma) - 1) *
                               Math.Exp(2 * (double)mu + (double)sigma * (double)sigma);
        var expectedStdDev = Math.Sqrt(expectedVariance);

        var maxBound = expectedMean + 4 * expectedStdDev;
        var histogram = new Histogram(1e-10, maxBound, 100);
        var sampler = new DecimalLogNormalSampler(mu, sigma);

        // Act
        histogram.Generate<decimal, ChiRng, DecimalLogNormalSampler>(ref rng, 20_000, sampler);

        // Assert
        histogram.DebugPrint(testOutputHelper, $"LogNormal(mu={mu}, sigma={sigma})");
        histogram.AssertIsLogNormal(mu, sigma, 0.15);
    }

    private readonly struct DecimalLogNormalSampler(decimal mu, decimal sigma) :
        IHistogramSamplerWithRange<decimal, ChiRng>
    {
        public decimal NextSample(ref ChiRng rng)
        {
            return rng.LogNormal(mu, sigma).Sample();
        }

        public double Normalize(decimal value)
        {
            return (double)value;
        }
    }
}