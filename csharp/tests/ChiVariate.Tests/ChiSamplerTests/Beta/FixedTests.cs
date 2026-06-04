using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Beta;

/// <summary>
///     Tests for Beta distribution using ChiFixed type.
///     Beta is bounded on [0,1] with elegant shape control via alpha and beta parameters.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(2.0, 5.0)] // Skewed right (mode near 0.2)
    [InlineData(5.0, 2.0)] // Skewed left (mode near 0.8)
    [InlineData(5.0, 5.0)] // Symmetric bell-shaped (mode at 0.5)
    public void Sample_AcrossShapeParameters_MatchesBetaDistribution(double alpha, double beta)
    {
        var a = (ChiFixed)(decimal)alpha;
        var b = (ChiFixed)(decimal)beta;

        var rng = new ChiRng(ChiSeed.Scramble("BetaFixed", alpha * 100 + beta));
        var expectedMean = alpha / (alpha + beta);
        var histogram = new Histogram(0, 1, 100);
        var sampler = new ChiFixedBetaSampler(a, b);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedBetaSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, $"Beta(alpha={alpha}, beta={beta}) using ChiFixed");
        histogram.AssertIsBeta(expectedMean, 0.10);
    }

    [Fact]
    public void Sample_UniformCase_MatchesUniformDistribution()
    {
        // Beta(1, 1) is the uniform distribution on [0, 1]
        var a = ChiFixed.One;
        var b = ChiFixed.One;

        var rng = new ChiRng("BetaUniformFixed");
        var histogram = new Histogram(0, 1, 100);
        var sampler = new ChiFixedBetaSampler(a, b);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedBetaSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, "Beta(1, 1) = Uniform[0,1] using ChiFixed");
        histogram.AssertIsUniform(0.15);
    }

    [Fact]
    public void Sample_SymmetricCase_HasModeAtHalf()
    {
        // Beta(5, 5) is symmetric with mode at 0.5
        var a = (ChiFixed)5;
        var b = (ChiFixed)5;

        var rng = new ChiRng("BetaSymmetricFixed");
        var histogram = new Histogram(0, 1, 100);
        var sampler = new ChiFixedBetaSampler(a, b);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedBetaSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, "Beta(5, 5) using ChiFixed - symmetric");

        var mode = histogram.CalculateMode();
        Assert.InRange(mode, 0.45, 0.55); // Mode should be near 0.5
    }

    private readonly struct ChiFixedBetaSampler(ChiFixed alpha, ChiFixed beta) :
        IHistogramSamplerWithRange<ChiFixed, ChiRng>
    {
        public ChiFixed NextSample(ref ChiRng rng)
        {
            return rng.Beta(alpha, beta).Sample();
        }

        public double Normalize(ChiFixed value)
        {
            return double.CreateChecked(value);
        }
    }
}