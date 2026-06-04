using System.Globalization;
using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Chi;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 250_000;

    [Theory]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    public void Sample_Squared_MatchesChiSquaredDistribution(int degreesOfFreedom)
    {
        var rng = new ChiRng($"Chi_k={degreesOfFreedom}");

        var expectedMean = (double)degreesOfFreedom;
        var expectedVariance = 2.0 * degreesOfFreedom;
        var expectedStdDev = Math.Sqrt(expectedVariance);
        var maxBound = expectedMean + 8 * expectedStdDev;
        var histogram = new Histogram(0, maxBound, 100);

        for (var i = 0; i < SampleCount; i++)
        {
            var chiSample = rng.Chi((double)degreesOfFreedom).Sample();
            var chiSquaredSample = chiSample * chiSample;
            histogram.AddSample(chiSquaredSample);
        }

        histogram.DebugPrint(testOutputHelper, $"Chi(k={degreesOfFreedom})^2 Distribution");
        histogram.AssertIsChiSquared(degreesOfFreedom, 0.1, 0.15);
    }

    [Fact]
    public void Sample_With2DoF_MatchesRayleighDistribution()
    {
        var chiRng = new ChiRng("Chi_vs_Rayleigh");
        var rayleighRng = new ChiRng("Chi_vs_Rayleigh"); // Same seed

        var chiHistogram = new Histogram(0, 8, 100);
        var rayleighHistogram = new Histogram(0, 8, 100);

        for (var i = 0; i < SampleCount; i++)
        {
            chiHistogram.AddSample(chiRng.Chi(2.0).Sample());
            rayleighHistogram.AddSample(rayleighRng.Rayleigh(1.0).Sample());
        }

        var chiMean = chiHistogram.CalculateMean();
        var rayleighMean = rayleighHistogram.CalculateMean();
        rayleighMean.Should().BeApproximately(chiMean, 0.01,
            "because a Chi(2) distribution should be equivalent to a Rayleigh(1) distribution.");

        var chiMode = chiHistogram.CalculateMode();
        var rayleighMode = rayleighHistogram.CalculateMode();
        rayleighMode.Should().BeApproximately(chiMode, 0.2);
    }

    [Theory]
    [InlineData("2")]
    [InlineData("5")]
    public void Sample_DecimalSquared_MatchesChiSquaredDistribution(string degreesOfFreedomStr)
    {
        var degreesOfFreedom = decimal.Parse(degreesOfFreedomStr, CultureInfo.InvariantCulture);
        var intDof = (int)degreesOfFreedom;
        var rng = new ChiRng(ChiSeed.Scramble("ChiDecimal", (long)degreesOfFreedom));

        var expectedMean = (double)degreesOfFreedom;
        var expectedVariance = 2.0 * (double)degreesOfFreedom;
        var expectedStdDev = Math.Sqrt(expectedVariance);
        var maxBound = expectedMean + 8 * expectedStdDev;
        var histogram = new Histogram(0, maxBound, 100);

        for (var i = 0; i < 20_000; i++)
        {
            var chiSample = rng.Chi(degreesOfFreedom).Sample();
            var chiSquaredSample = chiSample * chiSample;
            histogram.AddSample((double)chiSquaredSample);
        }

        histogram.DebugPrint(testOutputHelper, $"Chi(k={degreesOfFreedom})^2 Distribution");
        histogram.AssertIsChiSquared(intDof, 0.1, 0.2);
    }

    [Theory]
    [InlineData("Deterministic")]
    [InlineData("Randomized")]
    public void Snapshot_WithRestoredState_ProducesIdenticalSamples(string seed)
    {
        var rng = seed == "Randomized" ? new ChiRng() : new ChiRng(seed);
        var warmupCount = rng.Chance().PickBetween(100, 1000);
        for (var i = 0; i < warmupCount; i++)
            _ = rng.Chi(3.0).Sample();

        var rngSnapshot = rng.Snapshot();

        var rngClone = new ChiRng(rngSnapshot);

        for (var i = 0; i < 10_000; i++)
            rng.Chi(3.0).Sample().Should().Be(rngClone.Chi(3.0).Sample());
    }
}