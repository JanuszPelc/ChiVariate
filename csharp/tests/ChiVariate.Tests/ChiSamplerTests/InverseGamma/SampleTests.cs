using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.InverseGamma;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(1.5, 5.0)] // Case where InvGamma variance is undefined
    [InlineData(3.0, 5.0)] // Case where InvGamma variance is defined but unstable
    [InlineData(5.0, 2.0)] // Well-behaved case
    public void Sample_Reciprocal_MatchesCorrespondingGamma(double shape, double scale)
    {
        // This is the most robust test. The definitional property of an Inverse-Gamma
        // distribution is that its reciprocal is a Gamma distribution. The Gamma
        // distribution is much better behaved and its statistics are easier to verify.

        // Arrange
        var rng = new ChiRng(ChiSeed.Scramble("InverseGammaReciprocal", new ChiHash().Add(shape).Add(scale).Hash));
        var sampler = rng.InverseGamma(shape, scale);

        // Theoretical properties of the RECONSTRUCTED Gamma distribution
        var gammaShape = shape; // Gamma shape = InvGamma shape
        var gammaScale = 1.0 / scale; // Gamma scale = 1 / InvGamma scale
        var expectedGammaMean = gammaShape * gammaScale;
        var expectedGammaVariance = gammaShape * (gammaScale * gammaScale);

        var histogram = new Histogram(0, expectedGammaMean * 5, 150);

        // Act
        foreach (var invGammaSample in sampler.Sample(SampleCount))
            if (invGammaSample > 1e-9) // Avoid division by a near-zero sample
            {
                var reconstructedGammaSample = 1.0 / invGammaSample;
                histogram.AddSample(reconstructedGammaSample);
            }

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Reconstructed Gamma from 1/InvGamma(α={shape}, β={scale})");

        var actualMean = histogram.CalculateMean();
        actualMean.Should().BeApproximately(expectedGammaMean, expectedGammaMean * 0.1,
            "because the mean of the reciprocal samples should match the corresponding Gamma distribution's mean.");

        var actualVariance = Math.Pow(histogram.CalculateStdDev(actualMean), 2);
        actualVariance.Should().BeApproximately(expectedGammaVariance, expectedGammaVariance * 0.15,
            "because the variance of the reciprocal samples should match the corresponding Gamma distribution's variance.");
    }

    [Fact]
    public void Sample_Decimal_Reciprocal_MatchesCorrespondingGamma()
    {
        // Arrange
        const decimal shape = 3.0m;
        const decimal scale = 5.0m;
        var rng = new ChiRng(ChiSeed.Scramble("InvGammaDecimalReciprocal"));
        var sampler = rng.InverseGamma(shape, scale);

        var gammaShape = shape;
        var gammaScale = 1.0m / scale;
        var expectedGammaMean = (double)(gammaShape * gammaScale);
        var expectedGammaVariance = (double)(gammaShape * gammaScale * gammaScale);

        double sum = 0;
        double sumOfSquares = 0;
        var count = 0;

        // Act
        foreach (var invGammaSample in sampler.Sample(SampleCount))
            if (invGammaSample > 1e-9m)
            {
                var s = (double)(1.0m / invGammaSample);
                sum += s;
                sumOfSquares += s * s;
                count++;
            }

        // Assert
        var actualMean = sum / count;
        var actualVariance = sumOfSquares / count - actualMean * actualMean;

        actualMean.Should().BeApproximately(expectedGammaMean, expectedGammaMean * 0.1);
        actualVariance.Should().BeApproximately(expectedGammaVariance, expectedGammaVariance * 0.15);
    }

    [Fact]
    public void Sample_WithFixedSeed_IsDeterministic()
    {
        // Arrange
        var rng = new ChiRng(1337);

        // Act
        var result = rng.InverseGamma(3.0, 5.0).Sample();

        // Assert
        result.Should().BeApproximately(2.03781, 0.00001);
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(5, 0)]
    [InlineData(-1, 5)]
    [InlineData(5, -1)]
    public void InverseGamma_WithInvalidParameters_ThrowsArgumentOutOfRangeException(double shape, double scale)
    {
        // Arrange
        var rng = new ChiRng();
        var act = () => { rng.InverseGamma(shape, scale); };

        // Act & Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}