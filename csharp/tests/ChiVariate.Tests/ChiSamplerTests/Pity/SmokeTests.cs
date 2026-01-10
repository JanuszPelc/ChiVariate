using AwesomeAssertions;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Pity;

public class SmokeTests(ITestOutputHelper output)
{
    private const int Seed = 42;

    [Fact]
    public void Sample_WithKnownSeed_ReturnsExpectedSequence()
    {
        var rng = new ChiRng(Seed);
        var pity = rng.Pity(0.1, 0.05);

        var results = new int[10];
        for (var i = 0; i < 10; i++)
            results[i] = pity.Sample();

        results.Should().Equal(0, 0, 1, 1, 0, 0, 1, 0, 0, 0);
    }

    [Fact]
    public void GenshinWishSystem_SimulatePulls_ShowsPityMechanics()
    {
        // Simulates Genshin Impact's 5-star wish system:
        // - 0.6% base rate
        // - Soft pity at 74 pulls (+6% per pull after)
        // - Hard pity at 90 pulls (guaranteed)

        var rng = new ChiRng("GenshinWishes");
        var fiveStarPity = rng.Pity(
            0.006,
            0.06,
            73,
            90);

        var pullsUntilFiveStar = new List<int>();
        var totalPulls = 0;

        for (var banner = 0; banner < 100; banner++)
        {
            var pullsThisBanner = 0;
            while (fiveStarPity.Sample() == 0)
                pullsThisBanner++;
            pullsThisBanner++;

            pullsUntilFiveStar.Add(pullsThisBanner);
            totalPulls += pullsThisBanner;
        }

        output.WriteLine("Genshin 5-Star Wish Simulation (100 banners):");
        output.WriteLine($"  Average pulls per 5-star: {pullsUntilFiveStar.Average():F1}");
        output.WriteLine($"  Min pulls: {pullsUntilFiveStar.Min()}, Max pulls: {pullsUntilFiveStar.Max()}");
        output.WriteLine($"  Total pulls: {totalPulls}");

        pullsUntilFiveStar.Max().Should().BeLessThan(91,
            "because hard pity guarantees 5-star at 90 pulls.");
        pullsUntilFiveStar.Average().Should().BeLessThan(90,
            "because soft pity increases rates after 73 pulls.");
    }

    [Fact]
    public void Dota2SkullBasher_SimulateProcs_ShowsAntiStreakBehavior()
    {
        // Simulates Dota 2's Skull Basher bash mechanic:
        // - 25% proc chance (displayed)
        // - Uses PRD to prevent long streaks

        var rng = new ChiRng("SkullBasher");

        // PRD C-value for 25% = ~8.5% starting, escalates
        var basher = rng.Pity(
            0.085,
            0.085);

        var procCount = 0;
        var longestStreak = 0;
        var currentStreak = 0;
        const int attacks = 10000;

        for (var i = 0; i < attacks; i++)
            if (basher.Sample() == 1)
            {
                procCount++;
                longestStreak = Math.Max(longestStreak, currentStreak);
                currentStreak = 0;
            }
            else
            {
                currentStreak++;
            }

        var actualProcRate = (double)procCount / attacks;

        output.WriteLine("Dota 2 Skull Basher Simulation (10,000 attacks):");
        output.WriteLine($"  Proc rate: {actualProcRate:P1} (target: ~25%)");
        output.WriteLine($"  Longest non-proc streak: {longestStreak}");

        longestStreak.Should().BeLessThan(20,
            "because PRD prevents extremely long dry streaks.");
    }

    [Fact]
    public void LegendaryLootDrop_SimulateRuns_GuaranteesBadLuckProtection()
    {
        // Simulates a dungeon with legendary drop:
        // - 5% base drop rate
        // - +2% per run after 20 runs
        // - Guaranteed after 50 runs

        var rng = new ChiRng("LegendaryLoot");
        var legendaryDrop = rng.Pity(
            0.05,
            0.02,
            20,
            50);

        var runsPerDrop = new List<int>();

        for (var attempt = 0; attempt < 1000; attempt++)
        {
            var runs = 0;
            while (legendaryDrop.Sample() == 0)
                runs++;
            runs++;

            runsPerDrop.Add(runs);
        }

        output.WriteLine("Legendary Loot Drop Simulation (1,000 attempts):");
        output.WriteLine($"  Average runs per drop: {runsPerDrop.Average():F1}");
        output.WriteLine($"  Min runs: {runsPerDrop.Min()}, Max runs: {runsPerDrop.Max()}");
        output.WriteLine($"  Drops within 20 runs: {runsPerDrop.Count(r => r <= 20)}");
        output.WriteLine($"  Drops at hard pity (50): {runsPerDrop.Count(r => r == 50)}");

        runsPerDrop.Max().Should().BeLessThan(51,
            "because hard pity guarantees drop at 50 runs.");
    }

    [Fact]
    public void CriticalHitSystem_FairerThanPureBernoulli()
    {
        // Compare PRD critical hits vs pure Bernoulli
        // Both target 20% crit rate, but PRD should have less variance

        var rng1 = new ChiRng("CritPRD");
        var rng2 = new ChiRng("CritBernoulli");

        // PRD with ~5.7% base (C-value for 20% effective rate)
        var prdCrit = rng1.Pity(0.057, 0.057);

        var prdStreaks = new List<int>();
        var bernoulliStreaks = new List<int>();
        const int samples = 50000;

        var prdCurrentStreak = 0;
        var bernoulliCurrentStreak = 0;

        for (var i = 0; i < samples; i++)
        {
            if (prdCrit.Sample() == 1)
            {
                if (prdCurrentStreak > 0) prdStreaks.Add(prdCurrentStreak);
                prdCurrentStreak = 0;
            }
            else
            {
                prdCurrentStreak++;
            }

            if (rng2.Bernoulli(0.2).Sample() == 1)
            {
                if (bernoulliCurrentStreak > 0) bernoulliStreaks.Add(bernoulliCurrentStreak);
                bernoulliCurrentStreak = 0;
            }
            else
            {
                bernoulliCurrentStreak++;
            }
        }

        output.WriteLine("Critical Hit Comparison (50,000 samples):");
        output.WriteLine($"  PRD max non-crit streak: {prdStreaks.DefaultIfEmpty(0).Max()}");
        output.WriteLine($"  Bernoulli max non-crit streak: {bernoulliStreaks.DefaultIfEmpty(0).Max()}");

        prdStreaks.DefaultIfEmpty(0).Max().Should().BeLessThan(bernoulliStreaks.DefaultIfEmpty(0).Max(),
            "because PRD should have shorter worst-case streaks than pure Bernoulli.");
    }

    [Fact]
    public void GachaSystem_ShowsProgressivePity()
    {
        // Track how probability increases over pulls

        var rng = new ChiRng("GachaProgress");
        var gacha = rng.Pity(
            0.01,
            0.02,
            5,
            20);

        output.WriteLine("Gacha Probability Escalation:");
        output.WriteLine("  Pull | Failures | Current Probability");
        output.WriteLine("  -----|----------|--------------------");

        for (var pull = 1; pull <= 15; pull++)
        {
            var prob = gacha.CurrentProbability;
            var failures = gacha.FailureCount;

            output.WriteLine($"  {pull,4} | {failures,8} | {prob:P1}");

            if (gacha.Sample() == 1)
            {
                output.WriteLine($"  >>> SUCCESS on pull {pull}! Reset.");
                break;
            }
        }
    }
}