using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Rayleigh;

/// <summary>
///     Tests for Rayleigh distribution using ChiFixed type.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(0.5)]
    [InlineData(1.0)]
    [InlineData(5.0)]
    public void Sample_WithVariousSigma_ProducesRayleighDistribution(double sigma)
    {
        var s = (ChiFixed)(decimal)sigma;

        var rng = new ChiRng(ChiSeed.Scramble("RayleighFixed", sigma));
        var histogram = new Histogram(0, sigma * 6, 100);
        var sampler = new ChiFixedRayleighSampler(s);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedRayleighSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, $"Rayleigh(sigma={sigma}) using ChiFixed");
        histogram.AssertIsRayleigh(sigma, 0.1);
    }

    [Fact]
    public void Sample_SigmaOne_HasCorrectMean()
    {
        // Rayleigh(1) has mean = sqrt(pi/2) ≈ 1.253
        var sigma = ChiFixed.One;

        var rng = new ChiRng("RayleighSigmaOneFixed");
        var histogram = new Histogram(0, 6, 100);
        var sampler = new ChiFixedRayleighSampler(sigma);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedRayleighSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, "Rayleigh(sigma=1) using ChiFixed");

        var expectedMean = Math.Sqrt(Math.PI / 2);
        var actualMean = histogram.CalculateMean();
        Assert.InRange(actualMean, expectedMean * 0.95, expectedMean * 1.05);
    }

    private readonly struct ChiFixedRayleighSampler(ChiFixed sigma) :
        IHistogramSamplerWithRange<ChiFixed, ChiRng>
    {
        public ChiFixed NextSample(ref ChiRng rng)
        {
            return rng.Rayleigh(sigma).Sample();
        }

        public double Normalize(ChiFixed value)
        {
            return double.CreateChecked(value);
        }
    }
}