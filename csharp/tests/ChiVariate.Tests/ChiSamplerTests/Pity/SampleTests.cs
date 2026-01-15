using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Pity;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(0.10, 0.0, 0, 0)]
    [InlineData(0.25, 0.0, 0, 0)]
    [InlineData(0.50, 0.0, 0, 0)]
    public void Sample_WithNoEscalation_HasCorrectMean(
        double baseProbability, double increment, int softThreshold, int hardCap)
    {
        var rng = new ChiRng(ChiSeed.Scramble("PityNoEscalation", baseProbability));
        var pity = rng.Pity(baseProbability, increment, softThreshold, hardCap);
        var successCount = 0;

        for (var i = 0; i < SampleCount; i++)
            successCount += pity.Sample();

        var actualMean = (double)successCount / SampleCount;

        testOutputHelper.WriteLine(
            $"Pity(base={baseProbability}, inc={increment}, soft={softThreshold}, hard={hardCap})");
        testOutputHelper.WriteLine($"Expected: ~{baseProbability:P2}, Actual: {actualMean:P2}");

        actualMean.Should().BeApproximately(baseProbability, 0.02,
            "because with no escalation, the mean should equal base probability.");
    }

    [Fact]
    public void Sample_WithEscalation_HasHigherSuccessRateThanBase()
    {
        const double baseProbability = 0.05;
        const double increment = 0.05;

        var rng = new ChiRng(ChiSeed.Scramble("PityEscalation", 42));
        var pity = rng.Pity(baseProbability, increment);
        var successCount = 0;

        for (var i = 0; i < SampleCount; i++)
            successCount += pity.Sample();

        var actualMean = (double)successCount / SampleCount;

        testOutputHelper.WriteLine($"Pity with escalation: base={baseProbability}, increment={increment}");
        testOutputHelper.WriteLine($"Base probability: {baseProbability:P2}, Actual rate: {actualMean:P2}");

        actualMean.Should().BeGreaterThan(baseProbability,
            "because escalating probability should increase overall success rate.");
    }

    [Fact]
    public void Sample_WithHardCap_GuaranteesSuccessAtCap()
    {
        const double baseProbability = 0.001;
        const int hardCap = 10;

        var rng = new ChiRng(ChiSeed.Scramble("PityHardCap", 42));
        var pity = rng.Pity(baseProbability, 0.0, 0, hardCap);

        for (var trial = 0; trial < 1000; trial++)
        {
            var attempts = 0;
            while (pity.Sample() == 0)
            {
                attempts++;
                attempts.Should().BeLessThan(hardCap,
                    $"because hard cap at {hardCap} should guarantee success before reaching cap.");
            }
        }
    }

    [Fact]
    public void Sample_FailureCount_IncrementsOnFailure()
    {
        var rng = new ChiRng(ChiSeed.Scramble("PityFailureCount", 42));
        var pity = rng.Pity(0.0, 0.0, 0, 100);

        pity.FailureCount.Should().Be(0, "because initial failure count should be zero.");

        pity.Sample();
        pity.FailureCount.Should().Be(1, "because failure count should increment after a failure.");

        pity.Sample();
        pity.FailureCount.Should().Be(2, "because failure count should increment again.");
    }

    [Fact]
    public void Sample_FailureCount_ResetsOnSuccess()
    {
        var rng = new ChiRng(ChiSeed.Scramble("PityReset", 42));
        var pity = rng.Pity(0.0, 0.0, 0, 5);

        while (pity.Sample() == 0)
        {
        }

        pity.FailureCount.Should().Be(0, "because failure count should reset after success.");
    }

    [Fact]
    public void Sample_CurrentProbability_IncreasesAfterSoftThreshold()
    {
        const double baseProbability = 0.01;
        const double increment = 0.05;
        const int softThreshold = 3;

        var rng = new ChiRng(ChiSeed.Scramble("PitySoftThreshold", 42));
        var pity = rng.Pity(baseProbability, increment, softThreshold, 100);

        pity.CurrentProbability.Should().Be(baseProbability);

        pity.Sample();
        pity.CurrentProbability.Should().Be(baseProbability,
            "because probability should not increase before soft threshold.");

        pity.Sample();
        pity.CurrentProbability.Should().Be(baseProbability);

        pity.Sample();
        pity.CurrentProbability.Should().Be(baseProbability);

        pity.Sample();
        pity.CurrentProbability.Should().BeApproximately(baseProbability + increment, 1e-10,
            "because probability should increase after exceeding soft threshold.");
    }

    [Fact]
    public void Sample_CurrentProbability_CapsAtOne()
    {
        const double baseProbability = 0.0;
        const double increment = 1.5;

        var rng = new ChiRng(ChiSeed.Scramble("PityCap", 42));
        var pity = rng.Pity(baseProbability, increment);

        pity.Sample();

        pity.CurrentProbability.Should().Be(1.0,
            "because probability should be capped at 1.0 (0 + 1.5 -> capped to 1.0).");
    }

    [Fact]
    public void Sample_CurrentProbability_ResetsOnSuccess()
    {
        const double baseProbability = 0.01;
        const double increment = 0.1;

        var rng = new ChiRng(ChiSeed.Scramble("PityProbReset", 42));
        var pity = rng.Pity(baseProbability, increment, 0, 20);

        while (pity.Sample() == 0)
        {
        }

        pity.CurrentProbability.Should().Be(baseProbability,
            "because probability should reset to base after success.");
    }

    [Fact]
    public void SampleCount_ReturnsCorrectEnumerable()
    {
        var rng = new ChiRng(ChiSeed.Scramble("PitySampleCount", 42));
        var pity = rng.Pity(0.5, 0.0);

        using var samples = pity.Sample(100);

        samples.List.Count.Should().Be(100);

        var successCount = 0;
        foreach (var sample in samples)
        {
            sample.Should().BeOneOf(0, 1);
            successCount += sample;
        }

        successCount.Should().BeGreaterThan(0);
        successCount.Should().BeLessThan(100);
    }

    [Theory]
    [InlineData(-0.1, 0.0, 0, 0, "baseProbability")]
    [InlineData(1.1, 0.0, 0, 0, "baseProbability")]
    [InlineData(double.NaN, 0.0, 0, 0, "baseProbability")]
    [InlineData(0.5, -0.1, 0, 0, "increment")]
    [InlineData(0.5, double.NaN, 0, 0, "increment")]
    public void Pity_WithInvalidParameters_ThrowsArgumentOutOfRangeException(
        double baseProbability, double increment, int softThreshold, int hardCap, string expectedParam)
    {
        var act = () =>
        {
            var rng = new ChiRng(0);
            rng.Pity(baseProbability, increment, softThreshold, hardCap);
        };

        act.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be(expectedParam);
    }

    [Fact]
    public void Pity_WithNegativeSoftThreshold_ThrowsArgumentOutOfRangeException()
    {
        var act = () =>
        {
            var rng = new ChiRng(0);
            rng.Pity(0.5, 0.1, -1);
        };

        act.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be("softPityThreshold");
    }

    [Fact]
    public void Pity_WithNegativeHardCap_ThrowsArgumentOutOfRangeException()
    {
        var act = () =>
        {
            var rng = new ChiRng(0);
            rng.Pity(0.5, 0.1, 0, -1);
        };

        act.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be("hardPityCap");
    }

    [Theory]
    [InlineData(0.10, 0.05, 0, 50)]
    [InlineData(0.05, 0.03, 10, 100)]
    public void Sample_TrialsUntilSuccess_ShowsPRDDistribution(
        double baseProbability, double increment, int softThreshold, int hardCap)
    {
        const int trials = 50_000;
        var rng = new ChiRng(ChiSeed.Scramble("PityHistogram", baseProbability + increment));
        var pity = rng.Pity(baseProbability, increment, softThreshold, hardCap);

        // Histogram of "trials until success" (like Geometric but with pity)
        var histogram = new Histogram(1, hardCap + 1, hardCap, true);

        for (var i = 0; i < trials; i++)
        {
            var attempts = 1;
            while (pity.Sample() == 0)
                attempts++;

            histogram.AddSample(attempts);
        }

        histogram.DebugPrint(testOutputHelper,
            $"Pity(base={baseProbability}, inc={increment}, soft={softThreshold}, hard={hardCap})");

        histogram.AssertIsPity(baseProbability, hardCap, 0.1);
    }

    [Theory]
    [InlineData(20)]
    [InlineData(50)]
    public void Sample_WithHardCap_HistogramShowsNothingBeyondCap(int hardCap)
    {
        const int trials = 10_000;
        var rng = new ChiRng(ChiSeed.Scramble("PityHardCapHistogram", hardCap));
        var pity = rng.Pity(0.01, 0.0, 0, hardCap);

        var histogram = new Histogram(1, hardCap + 10, hardCap + 9, true);

        for (var i = 0; i < trials; i++)
        {
            var attempts = 1;
            while (pity.Sample() == 0)
                attempts++;

            histogram.AddSample(attempts);
        }

        histogram.DebugPrint(testOutputHelper, $"Pity with Hard Cap = {hardCap}");
        histogram.AssertHardCapEnforced(hardCap);
    }

    [Fact]
    public void Sample_PRDvsGeometric_HasLowerVariance()
    {
        const int trials = 50_000;
        const double probability = 0.10;

        var rng1 = new ChiRng("PityPRD");
        var rng2 = new ChiRng("PityGeometric");

        // PRD with escalation
        var pity = rng1.Pity(probability, probability);

        var prdHistogram = new Histogram(1, 50, 49, true);
        var geometricHistogram = new Histogram(1, 50, 49, true);

        for (var i = 0; i < trials; i++)
        {
            // PRD trials until success
            var prdAttempts = 1;
            while (pity.Sample() == 0)
                prdAttempts++;
            prdHistogram.AddSample(prdAttempts);

            // Pure Geometric (Bernoulli) trials until success
            var geoAttempts = 1;
            while (rng2.Bernoulli(probability).Sample() == 0)
                geoAttempts++;
            geometricHistogram.AddSample(geoAttempts);
        }

        prdHistogram.DebugPrint(testOutputHelper, "PRD Distribution (trials until success)");
        geometricHistogram.DebugPrint(testOutputHelper, "Geometric Distribution (trials until success)");

        var prdMean = prdHistogram.CalculateMean();
        var geoMean = geometricHistogram.CalculateMean();
        var prdStdDev = prdHistogram.CalculateStdDev(prdMean);
        var geoStdDev = geometricHistogram.CalculateStdDev(geoMean);

        testOutputHelper.WriteLine($"PRD:       Mean={prdMean:F2}, StdDev={prdStdDev:F2}");
        testOutputHelper.WriteLine($"Geometric: Mean={geoMean:F2}, StdDev={geoStdDev:F2}");

        prdMean.Should().BeLessThan(geoMean,
            "because PRD escalation should reduce average trials.");
        prdStdDev.Should().BeLessThan(geoStdDev,
            "because PRD should have lower variance (fewer long streaks).");
    }

    [Fact]
    public void Snapshot_WithRestoredState_ProducesIdenticalSamples()
    {
        var rng = new ChiRng("PitySnapshot");
        var pity = rng.Pity(0.02, 0.01, 50, 90);
        _ = pity.Sample(rng.Chance().PickBetween(100, 1000)).ToList();

        var rngSnapshot = rng.Snapshot();
        var pitySnapshot = pity.Snapshot();

        var rngClone = new ChiRng(rngSnapshot);
        var pityClone = rngClone.Pity(0.02, 0.01, 50, 90).WithSnapshot(pitySnapshot);

        for (var i = 0; i < 100; i++)
            pity.Sample().Should().Be(pityClone.Sample());
    }
}