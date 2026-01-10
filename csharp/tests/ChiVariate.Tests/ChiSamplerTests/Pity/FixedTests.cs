using AwesomeAssertions;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Pity;

public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(0.10)]
    [InlineData(0.25)]
    [InlineData(0.50)]
    public void Sample_WithFixedProbability_HasCorrectSuccessRate(double probability)
    {
        var baseProbability = (ChiFixed)(decimal)probability;
        var increment = ChiFixed.Zero;

        var rng = new ChiRng(ChiSeed.Scramble("PityFixed", probability));
        var pity = rng.Pity(baseProbability, increment, 0, 0);
        var successCount = 0;

        for (var i = 0; i < SampleCount; i++)
            successCount += pity.Sample();

        var actualRate = (double)successCount / SampleCount;

        testOutputHelper.WriteLine($"Pity(base={probability}) using ChiFixed");
        testOutputHelper.WriteLine($"Expected: {probability:P2}, Actual: {actualRate:P2}");

        actualRate.Should().BeApproximately(probability, 0.02,
            "because the success rate should match the ChiFixed base probability.");
    }

    [Fact]
    public void Sample_WithEscalation_IncreasesSuccessRate()
    {
        var baseProbability = (ChiFixed)0.05m;
        var increment = (ChiFixed)0.05m;

        var rng = new ChiRng(ChiSeed.Scramble("PityFixedEscalation", 42));
        var pity = rng.Pity(baseProbability, increment, 0, 0);
        var successCount = 0;

        for (var i = 0; i < SampleCount; i++)
            successCount += pity.Sample();

        var actualRate = (double)successCount / SampleCount;

        testOutputHelper.WriteLine("Pity with escalation using ChiFixed");
        testOutputHelper.WriteLine($"Base: 5%, Actual rate: {actualRate:P2}");

        actualRate.Should().BeGreaterThan(0.05,
            "because escalating probability should increase overall success rate.");
    }

    [Fact]
    public void Sample_HardCap_GuaranteesSuccessWithChiFixed()
    {
        var baseProbability = (ChiFixed)0.001m;
        var increment = ChiFixed.Zero;
        var hardCap = 10;

        var rng = new ChiRng(ChiSeed.Scramble("PityFixedHardCap", 42));
        var pity = rng.Pity(baseProbability, increment, 0, hardCap);

        for (var trial = 0; trial < 1000; trial++)
        {
            var attempts = 0;
            while (pity.Sample() == 0)
            {
                attempts++;
                attempts.Should().BeLessThan(hardCap,
                    $"because hard cap at {hardCap} should guarantee success.");
            }
        }
    }

    [Fact]
    public void Sample_CurrentProbability_WorksWithChiFixed()
    {
        var baseProbability = (ChiFixed)0.01m;
        var increment = (ChiFixed)0.05m;
        var softThreshold = 2;

        var rng = new ChiRng(ChiSeed.Scramble("PityFixedCurrentProb", 42));
        var pity = rng.Pity(baseProbability, increment, softThreshold, 100);

        ((decimal)pity.CurrentProbability).Should().BeApproximately(0.01m, 1e-8m);

        pity.Sample();
        pity.Sample();

        ((decimal)pity.CurrentProbability).Should().BeApproximately(0.01m, 1e-8m,
            "because probability should not increase before exceeding soft threshold.");

        pity.Sample();

        ((decimal)pity.CurrentProbability).Should().BeApproximately(0.06m, 1e-8m,
            "because probability should increase after exceeding soft threshold.");
    }

    [Fact]
    public void Sample_FailureCount_WorksWithChiFixed()
    {
        var baseProbability = ChiFixed.Zero;
        var increment = ChiFixed.Zero;

        var rng = new ChiRng(ChiSeed.Scramble("PityFixedFailureCount", 42));
        var pity = rng.Pity(baseProbability, increment, 0, 100);

        pity.FailureCount.Should().Be(0);

        pity.Sample();
        pity.FailureCount.Should().Be(1);

        pity.Sample();
        pity.FailureCount.Should().Be(2);
    }

    [Fact]
    public void Sample_ExtremeProbabilities_WorkCorrectly()
    {
        var rng = new ChiRng("PityExtremeFixed");

        var pZero = rng.Pity(ChiFixed.Zero, ChiFixed.Zero, 0, 0);
        var zeroSuccesses = 0;
        for (var i = 0; i < 1000; i++)
            zeroSuccesses += pZero.Sample();
        zeroSuccesses.Should().Be(0, "because p=0 with no hard cap should never succeed.");

        var pOne = rng.Pity(ChiFixed.One, ChiFixed.Zero, 0, 0);
        var oneSuccesses = 0;
        for (var i = 0; i < 1000; i++)
            oneSuccesses += pOne.Sample();
        oneSuccesses.Should().Be(1000, "because p=1 should always succeed.");
    }
}