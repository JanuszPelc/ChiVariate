using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.F;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(5, 10)] // Test variance for d2 >= 10
    [InlineData(10, 20)]
    [InlineData(20, 30)]
    public void Sample_ProducesDistributionWithCorrectStatistics(int d1, int d2)
    {
        // Arrange
        var rng = new ChiRng(ChiSeed.Scramble("F-dist", ChiHash.Hash(d1, d2)));
        var sampler = rng.F(d1, (double)d2);

        var expectedMean = (double)d2 / (d2 - 2);
        var expectedVariance = 2.0 * d2 * d2 * (d1 + d2 - 2) / (d1 * Math.Pow(d2 - 2, 2) * (d2 - 4));
        var expectedStdDev = Math.Sqrt(expectedVariance);

        var maxBound = expectedMean + 8 * expectedStdDev;
        var histogram = new Histogram(0, maxBound, 200);

        // Act
        foreach (var sample in sampler.Sample(SampleCount)) histogram.AddSample(sample);

        // Assert
        histogram.DebugPrint(testOutputHelper, $"F-Distribution (d1={d1}, d2={d2})");

        var actualMean = histogram.CalculateMean();
        actualMean.Should().BeApproximately(expectedMean, expectedMean * 0.1,
            "because the sample mean should match the theoretical mean for d2 > 2.");

        var actualVariance = Math.Pow(histogram.CalculateStdDev(actualMean), 2);
        actualVariance.Should().BeApproximately(expectedVariance, expectedVariance * 0.2, // Variance has higher error
            "because the sample variance should match the theoretical variance for d2 > 4.");
    }

    [Fact]
    public void Sample_WithSmallDof_ProducesCorrectMean()
    {
        // Arrange
        const int d1 = 5, d2 = 5;
        var rng = new ChiRng(ChiSeed.Scramble("F-dist", ChiHash.Hash(d1, d2)));
        var sampler = rng.F(d1, (double)d2);

        var expectedMean = (double)d2 / (d2 - 2);
        var histogram = new Histogram(0, 25, 100);

        // Act
        foreach (var sample in sampler.Sample(200_000)) // Increase samples for stability
            histogram.AddSample(sample);

        // Assert
        histogram.DebugPrint(testOutputHelper, $"F-Distribution (d1={d1}, d2={d2})");
        var actualMean = histogram.CalculateMean();
        actualMean.Should().BeApproximately(expectedMean, expectedMean * 0.15,
            "because the sample mean should still converge even if variance is unstable.");
    }

    [Fact]
    public void Sample_WithLargeDof_ApproachesOne()
    {
        // Arrange
        var rng = new ChiRng(ChiSeed.Scramble("F-dist-large-dof"));
        var sampler = rng.F(100.0, 100.0);
        var histogram = new Histogram(0, 3, 100);

        // Act
        foreach (var sample in sampler.Sample(SampleCount)) histogram.AddSample(sample);

        // Assert
        histogram.DebugPrint(testOutputHelper, $"F-Distribution (d1=100, d2={100})");
        var actualMean = histogram.CalculateMean();
        actualMean.Should().BeApproximately(1.0, 0.05,
            "because as d1 and d2 become large, the F-distribution peaks sharply at 1.");
    }

    [Fact]
    public void Sample_Decimal_ProducesCorrectStatistics()
    {
        // Arrange
        const decimal d1 = 5.0m;
        const decimal d2 = 10.0m;
        var rng = new ChiRng(ChiSeed.Scramble("F-dist-decimal", ChiHash.Hash((double)d1, (double)d2)));
        var sampler = rng.F(d1, d2);

        var expectedMean = (double)(d2 / (d2 - 2));
        var expectedVariance = 2.0 * (double)(d2 * d2) * (double)(d1 + d2 - 2) /
                               ((double)d1 * Math.Pow((double)(d2 - 2), 2) * (double)(d2 - 4));

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

        actualMean.Should().BeApproximately(expectedMean, expectedMean * 0.1);
        actualVariance.Should().BeApproximately(expectedVariance, expectedVariance * 0.2);
    }

    [Fact]
    public void Sample_WithFixedSeed_IsDeterministic()
    {
        // Arrange
        var rng = new ChiRng(1337);

        // Act
        var result = rng.F(5.0, 10.0).Sample();

        // Assert
        result.Should().BeApproximately(0.540679, 0.00001);
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(5, 0)]
    [InlineData(-1, 5)]
    [InlineData(5, -1)]
    public void F_WithInvalidDegreesOfFreedom_ThrowsArgumentOutOfRangeException(int d1, int d2)
    {
        // Arrange
        var rng = new ChiRng();
        var act = () => { rng.F(d1, (double)d2); };

        // Act & Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}