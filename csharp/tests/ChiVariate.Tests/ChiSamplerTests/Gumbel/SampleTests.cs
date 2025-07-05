using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Gumbel;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 150_000;
    private const double EulerMascheroni = 0.57721566490153286; // γ

    [Theory]
    [InlineData(0.0, 1.0)] // Standard Gumbel
    [InlineData(5.0, 2.0)] // Shifted and scaled
    [InlineData(-10.0, 0.5)] // Shifted and narrow
    public void Sample_ProducesDistributionWithCorrectStatistics(double location, double scale)
    {
        // Arrange
        var rng = new ChiRng(ChiSeed.Scramble("Gumbel", ChiHash.Hash(location, scale)));
        var sampler = rng.Gumbel(location, scale);

        var expectedMean = location + scale * EulerMascheroni;
        var expectedVariance = Math.PI * Math.PI / 6.0 * (scale * scale);
        var expectedStdDev = Math.Sqrt(expectedVariance);

        var maxBound = expectedMean + 8 * expectedStdDev;
        var minBound = expectedMean - 8 * expectedStdDev;
        var histogram = new Histogram(minBound, maxBound, 200);

        // Act
        foreach (var sample in sampler.Sample(SampleCount)) histogram.AddSample(sample);

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Gumbel (μ={location}, β={scale})");

        var actualMean = histogram.CalculateMean();
        actualMean.Should().BeApproximately(expectedMean, Math.Abs(expectedMean * 0.1) + 0.01,
            "because the sample mean should match the theoretical mean μ + βγ.");

        var actualVariance = Math.Pow(histogram.CalculateStdDev(actualMean), 2);
        actualVariance.Should().BeApproximately(expectedVariance, expectedVariance * 0.15,
            "because the sample variance should match the theoretical variance (π²β²/6).");
    }

    [Fact]
    public void Sample_Decimal_ProducesCorrectStatistics()
    {
        // Arrange
        const decimal location = 5.0m;
        const decimal scale = 2.0m;
        var rng = new ChiRng(ChiSeed.Scramble("GumbelDecimal", ChiHash.Hash((double)location, (double)scale)));
        var sampler = rng.Gumbel(location, scale);

        var expectedMean = (double)(location + scale * (decimal)EulerMascheroni);
        var expectedVariance = Math.PI * Math.PI / 6.0 * (double)(scale * scale);

        double sum = 0;
        double sumOfSquares = 0;

        // Act
        foreach (var sample in sampler.Sample(SampleCount))
        {
            var s = (double)sample;
            sum += s;
            sumOfSquares += s * s;
        }

        // Assert
        var actualMean = sum / SampleCount;
        var actualVariance = sumOfSquares / SampleCount - actualMean * actualMean;

        actualMean.Should().BeApproximately(expectedMean, Math.Abs(expectedMean * 0.1));
        actualVariance.Should().BeApproximately(expectedVariance, expectedVariance * 0.15);
    }

    [Fact]
    public void Sample_WithFixedSeed_IsDeterministic()
    {
        // Arrange
        var rng = new ChiRng(1337);

        // Act
        var result = rng.Gumbel(5.0, 2.0).Sample();

        // Assert
        result.Should().BeApproximately(5.51681, 0.00001);
    }

    [Theory]
    [InlineData(0.0, 0.0)]
    [InlineData(0.0, -1.0)]
    public void Gumbel_WithInvalidScale_ThrowsArgumentOutOfRangeException(double location, double scale)
    {
        // Arrange
        var rng = new ChiRng();
        var act = () => { rng.Gumbel(location, scale); };

        // Act & Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("scale");
    }
}