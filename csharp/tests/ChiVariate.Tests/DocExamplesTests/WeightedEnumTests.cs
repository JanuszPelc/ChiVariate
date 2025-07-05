using AwesomeAssertions;
using Xunit;
using Rarity = ChiVariate.Tests.DocExamplesTests.WeightedEnumTests.ConvenientMagicCardGenerator.Rarity;

namespace ChiVariate.Tests.DocExamplesTests;

#pragma warning disable CS1591

#region boilerplate

#pragma warning disable CS8524
// ReSharper disable NotAccessedPositionalProperty.Local
// ReSharper disable NotAccessedPositionalProperty.Global

#endregion

public class WeightedEnumTests
{
    [Fact]
    public void ConvenientAndPerformantApproaches_WithSynchronizedRng_ProduceSameResults()
    {
        // Arrange
        var rng1 = new ChiRng();
        var rng2 = new ChiRng(rng1.Snapshot());
        var categorical = new PerformantMagicCardGenerator().CreateSampler(ref rng2);

        // Act & Assert
        for (var i = 0; i < 10_000; i++)
        {
            var card1 = new ConvenientMagicCardGenerator().GenerateMagicCard(ref rng1);
            var card2 = new PerformantMagicCardGenerator().GenerateMagicCard(ref rng2, ref categorical);

            card1.Should().BeEquivalentTo(card2);
        }
    }

    internal class ConvenientMagicCardGenerator
    {
        // --- example code begin ---
        public enum Rarity
        {
            [ChiEnumWeight(0.65)] Common,
            [ChiEnumWeight(0.25)] Rare,
            [ChiEnumWeight(0.10)] Epic
        }

        public MagicCard GenerateMagicCard(ref ChiRng rng)
        {
            var rarity = rng.Chance().PickEnum<Rarity>(); // Non-allocating
            var (attack, defense) = RollStatsForRarity(ref rng, rarity);

            // Same results as the manual version with less code
            return new MagicCard(rarity, attack, defense);
        }
        // --- example code end ---

        internal static (int, int) RollStatsForRarity(ref ChiRng rng, Rarity rarity)
        {
            var chance = rng.Chance();
            var (attack, defense) = rarity switch
            {
                Rarity.Common =>
                    (chance.PickBetween(1, 3), chance.PickBetween(1, 4)),
                Rarity.Rare =>
                    (chance.PickBetween(2, 5), chance.PickBetween(3, 6)),
                Rarity.Epic =>
                    (chance.PickBetween(4, 8), chance.PickBetween(5, 9))
            };
            return (attack, defense);
        }
    }

    private class PerformantMagicCardGenerator
    {
        // --- example code begin ---
        public ChiSamplerCategorical<ChiRng, double> CreateSampler(ref ChiRng rng)
        {
            return rng.Categorical(ChiEnum<Rarity>.Weights);
        }

        // Avoids O(k) setup overhead on each call by reusing the categorical sampler
        public MagicCard GenerateMagicCard(ref ChiRng rng, ref ChiSamplerCategorical<ChiRng, double> categorical)
        {
            var selectedIndex = categorical.Sample();
            var selectedRarity = ChiEnum<Rarity>.Values[selectedIndex];
            var (attack, defense) = ConvenientMagicCardGenerator.RollStatsForRarity(ref rng, selectedRarity);

            return new MagicCard(selectedRarity, attack, defense);
        }
        // --- example code end ---
    }

    internal record MagicCard(Rarity Rarity, int Attack, int Defense);
}

#region boilerplate

// ReSharper restore NotAccessedPositionalProperty.Global
// ReSharper restore NotAccessedPositionalProperty.Local
#pragma warning restore CS8524

#endregion