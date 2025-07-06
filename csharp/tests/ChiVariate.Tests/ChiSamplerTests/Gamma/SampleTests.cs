using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Gamma;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 200_000;

    [Theory]
    [InlineData(0.5, 2.0)]
    [InlineData(1.0, 2.0)]
    [InlineData(9.0, 2.0)]
    public void Sample_ProducesDistributionWithCorrectStatistics(double shape, double scale)
    {
        // Arrange
        var rng = new ChiRng(ChiSeed.Scramble("Gamma", shape * 100 + scale));
        var expectedMean = shape * scale;
        var expectedStdDev = Math.Sqrt(shape) * scale;
        var maxBound = expectedMean + 6 * expectedStdDev;
        var histogram = new Histogram(0, maxBound, 100);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Gamma(shape, scale).Sample();
            histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Gamma(shape={shape}, scale={scale})");
        histogram.AssertIsGamma(expectedMean, expectedStdDev, 0.05);
    }

    [Fact]
    public void Gamma_MatchesEquivalentExponentialDistribution()
    {
        // Arrange
        const double scale = 2.0;
        const double rateLambda = 1.0 / scale;

        var gammaRng = new ChiRng(123);
        var expRng = new ChiRng(123); // Same seed

        var gammaHistogram = new Histogram(0, 20, 100);
        var expHistogram = new Histogram(0, 20, 100);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            gammaHistogram.AddSample(gammaRng.Gamma(1.0, scale).Sample());
            expHistogram.AddSample(expRng.Exponential(rateLambda).Sample());
        }

        // Assert
        var gammaMean = gammaHistogram.CalculateMean();
        var expMean = expHistogram.CalculateMean();

        gammaMean.Should().BeApproximately(expMean, 0.01,
            "because Gamma(1, scale) must be statistically equivalent to Exponential(1/scale)");

        var gammaStdDev = gammaHistogram.CalculateStdDev(gammaMean);
        var expStdDev = expHistogram.CalculateStdDev(expMean);

        gammaStdDev.Should().BeApproximately(expStdDev, 0.01);
    }

    [Theory]
    [InlineData(0.0, 1.0)]
    [InlineData(-1.0, 1.0)]
    [InlineData(1.0, 0.0)]
    [InlineData(1.0, -1.0)]
    public void Gamma_WithInvalidParameters_ThrowsArgumentOutOfRangeException(double shape, double scale)
    {
        // Arrange
        var rng = new ChiRng(0);

        // Act
        Action act = () => rng.Gamma(shape, scale).Sample();

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("9.0", "2.0")] // Good, well-behaved case
    [InlineData("0.5", "2.0")] // Shape < 1 case
    public void Sample_Decimal_ProducesDistributionWithCorrectStatistics(string shapeStr, string scaleStr)
    {
        // Arrange
        var shape = decimal.Parse(shapeStr);
        var scale = decimal.Parse(scaleStr);
        var expectedMean = (double)(shape * scale);
        var expectedStdDev = Math.Sqrt((double)shape) * (double)scale;

        var rng = new ChiRng(ChiSeed.Scramble("GammaDecimal", new ChiHash().Add(shape).Add(scale).Hash));
        var maxBound = expectedMean + 6 * expectedStdDev;
        var histogram = new Histogram(0, maxBound, 100);
        var sampler = new DecimalGammaSampler(shape, scale);

        // Act
        histogram.Generate<decimal, ChiRng, DecimalGammaSampler>(ref rng, 20_000, sampler);

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Gamma(shape={shape}, scale={scale})");
        histogram.AssertIsGamma(expectedMean, expectedStdDev, 0.1);
    }

    private readonly struct DecimalGammaSampler(decimal shape, decimal scale) :
        IHistogramSamplerWithRange<decimal, ChiRng>
    {
        public decimal NextSample(ref ChiRng rng)
        {
            return rng.Gamma(shape, scale).Sample();
        }

        public double Normalize(decimal value)
        {
            return (double)value;
        }
    }
}