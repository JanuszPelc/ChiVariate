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
        // Arrange
        var rng = new ChiRng($"Chi_k={degreesOfFreedom}");

        var expectedMean = (double)degreesOfFreedom;
        var expectedVariance = 2.0 * degreesOfFreedom;
        var expectedStdDev = Math.Sqrt(expectedVariance);
        var maxBound = expectedMean + 8 * expectedStdDev;
        var histogram = new Histogram(0, maxBound, 100);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var chiSample = rng.Chi((double)degreesOfFreedom).Sample();
            var chiSquaredSample = chiSample * chiSample;
            histogram.AddSample(chiSquaredSample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Chi(k={degreesOfFreedom})^2 Distribution");
        histogram.AssertIsChiSquared(degreesOfFreedom, 0.1, 0.15);
    }

    [Fact]
    public void Sample_With2DoF_MatchesRayleighDistribution()
    {
        // Arrange
        var chiRng = new ChiRng("Chi_vs_Rayleigh");
        var rayleighRng = new ChiRng("Chi_vs_Rayleigh"); // Same seed

        var chiHistogram = new Histogram(0, 8, 100);
        var rayleighHistogram = new Histogram(0, 8, 100);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            chiHistogram.AddSample(chiRng.Chi(2.0).Sample());
            rayleighHistogram.AddSample(rayleighRng.Rayleigh(1.0).Sample());
        }

        // Assert
        var chiMean = chiHistogram.CalculateMean();
        var rayleighMean = rayleighHistogram.CalculateMean();
        rayleighMean.Should().BeApproximately(chiMean, 0.01,
            "because a Chi(2) distribution should be equivalent to a Rayleigh(1) distribution.");

        var chiMode = chiHistogram.CalculateMode();
        var rayleighMode = rayleighHistogram.CalculateMode();
        rayleighMode.Should().BeApproximately(chiMode, 0.1);
    }

    [Theory]
    [InlineData("2")]
    [InlineData("5")]
    public void Sample_Decimal_Squared_MatchesChiSquaredDistribution(string degreesOfFreedomStr)
    {
        // Arrange
        var degreesOfFreedom = decimal.Parse(degreesOfFreedomStr);
        var intDof = (int)degreesOfFreedom;
        var rng = new ChiRng(ChiSeed.Scramble("ChiDecimal", (long)degreesOfFreedom));

        var expectedMean = (double)degreesOfFreedom;
        var expectedVariance = 2.0 * (double)degreesOfFreedom;
        var expectedStdDev = Math.Sqrt(expectedVariance);
        var maxBound = expectedMean + 8 * expectedStdDev;
        var histogram = new Histogram(0, maxBound, 100);

        // Act
        for (var i = 0; i < 20_000; i++)
        {
            var chiSample = rng.Chi(degreesOfFreedom).Sample();
            var chiSquaredSample = chiSample * chiSample;
            histogram.AddSample((double)chiSquaredSample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Chi(k={degreesOfFreedom})^2 Distribution");
        histogram.AssertIsChiSquared(intDof, 0.1, 0.2);
    }
}