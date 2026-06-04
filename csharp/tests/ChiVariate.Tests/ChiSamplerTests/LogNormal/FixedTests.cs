using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.LogNormal;

/// <summary>
///     Tests for Log-Normal distribution using ChiFixed type.
///     LogNormal is exp(Normal), directly testing the Normal/Ziggurat chain.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(0.0, 0.25)] // Tightly clustered near 1
    [InlineData(1.0, 0.5)] // Shifted, less skewed
    [InlineData(0.0, 1.0)] // Standard Log-Normal
    public void Sample_AcrossMuAndSigma_MatchesLogNormalDistribution(double mu, double sigma)
    {
        var m = (ChiFixed)(decimal)mu;
        var s = (ChiFixed)(decimal)sigma;

        var rng = new ChiRng(ChiSeed.Scramble("LogNormalFixed", mu * 100 + sigma));
        var expectedMean = Math.Exp(mu + sigma * sigma / 2.0);
        var expectedVariance = (Math.Exp(sigma * sigma) - 1) * Math.Exp(2 * mu + sigma * sigma);
        var expectedStdDev = Math.Sqrt(expectedVariance);
        var maxBound = expectedMean + 4 * expectedStdDev;
        var histogram = new Histogram(0, maxBound, 100);
        var sampler = new ChiFixedLogNormalSampler(m, s);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedLogNormalSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, $"LogNormal(mu={mu}, sigma={sigma}) using ChiFixed");
        histogram.AssertIsLogNormal(mu, sigma, 0.15);
    }

    [Fact]
    public void Sample_StandardLogNormal_ReturnsExpectedMean()
    {
        // LogNormal(0, 1) has mean = e^0.5 ≈ 1.649, mode = e^-1 ≈ 0.368
        var mu = ChiFixed.Zero;
        var sigma = ChiFixed.One;

        var rng = new ChiRng("LogNormalStandardFixed");
        var histogram = new Histogram(0, 10, 100);
        var sampler = new ChiFixedLogNormalSampler(mu, sigma);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedLogNormalSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, "LogNormal(0, 1) using ChiFixed");

        var expectedMean = Math.Exp(0.5);
        var actualMean = histogram.CalculateMean();
        Assert.InRange(actualMean, expectedMean * 0.9, expectedMean * 1.1);
    }

    private readonly struct ChiFixedLogNormalSampler(ChiFixed mu, ChiFixed sigma) :
        IHistogramSamplerWithRange<ChiFixed, ChiRng>
    {
        public ChiFixed NextSample(ref ChiRng rng)
        {
            return rng.LogNormal(mu, sigma).Sample();
        }

        public double Normalize(ChiFixed value)
        {
            return double.CreateChecked(value);
        }
    }
}