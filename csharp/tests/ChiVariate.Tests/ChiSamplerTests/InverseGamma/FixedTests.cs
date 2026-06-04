using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.InverseGamma;

/// <summary>
///     Tests for Inverse-Gamma distribution using ChiFixed type.
///     InverseGamma is 1/Gamma, testing the Gamma chain which depends on Normal/Ziggurat.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(3.0, 5.0)] // Case where InvGamma variance is defined but unstable
    [InlineData(5.0, 2.0)] // Well-behaved case
    public void Sample_Reciprocal_MatchesCorrespondingGamma(double shape, double scale)
    {
        // The definitional property of InverseGamma: its reciprocal is Gamma
        var s = (ChiFixed)(decimal)shape;
        var sc = (ChiFixed)(decimal)scale;

        var rng = new ChiRng(ChiSeed.Scramble("InverseGammaFixed", shape * 100 + scale));
        var gammaShape = shape;
        var gammaScale = 1.0 / scale;
        var expectedGammaMean = gammaShape * gammaScale;
        var expectedGammaVariance = gammaShape * gammaScale * gammaScale;
        var histogram = new Histogram(0, expectedGammaMean * 5, 150);
        var sampler = new ChiFixedInverseGammaReciprocalSampler(s, sc);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedInverseGammaReciprocalSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, $"1/InverseGamma({shape}, {scale}) using ChiFixed");

        var actualMean = histogram.CalculateMean();
        actualMean.Should().BeApproximately(expectedGammaMean, expectedGammaMean * 0.15,
            "because the reciprocal of InverseGamma should match Gamma's mean.");

        var actualVariance = Math.Pow(histogram.CalculateStdDev(actualMean), 2);
        actualVariance.Should().BeApproximately(expectedGammaVariance, expectedGammaVariance * 0.2,
            "because the reciprocal of InverseGamma should match Gamma's variance.");
    }

    [Fact]
    public void Sample_WellBehavedCase_MatchesInverseGammaMean()
    {
        // InverseGamma(shape, scale) has mean = scale / (shape - 1) for shape > 1
        var shape = (ChiFixed)5m;
        var scale = (ChiFixed)8m;

        var rng = new ChiRng("InverseGammaWellBehavedFixed");
        var expectedMean = 8.0 / (5.0 - 1.0); // = 2.0
        var histogram = new Histogram(0, 10, 100);
        var sampler = new ChiFixedInverseGammaDirectSampler(shape, scale);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedInverseGammaDirectSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, "InverseGamma(5, 8) using ChiFixed");

        var actualMean = histogram.CalculateMean();
        actualMean.Should().BeApproximately(expectedMean, expectedMean * 0.15,
            "because InverseGamma(a, b) has mean = b/(a-1) for a > 1.");
    }

    /// <summary>Sampler that returns the reciprocal of InverseGamma samples to verify Gamma relationship.</summary>
    private readonly struct ChiFixedInverseGammaReciprocalSampler(ChiFixed shape, ChiFixed scale) :
        IHistogramSamplerWithRange<ChiFixed, ChiRng>
    {
        public ChiFixed NextSample(ref ChiRng rng)
        {
            var invGamma = rng.InverseGamma(shape, scale).Sample();
            return ChiFixed.One / invGamma;
        }

        public double Normalize(ChiFixed value)
        {
            return double.CreateChecked(value);
        }
    }

    /// <summary>Sampler that returns InverseGamma samples directly.</summary>
    private readonly struct ChiFixedInverseGammaDirectSampler(ChiFixed shape, ChiFixed scale) :
        IHistogramSamplerWithRange<ChiFixed, ChiRng>
    {
        public ChiFixed NextSample(ref ChiRng rng)
        {
            return rng.InverseGamma(shape, scale).Sample();
        }

        public double Normalize(ChiFixed value)
        {
            return double.CreateChecked(value);
        }
    }
}