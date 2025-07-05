using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerUtlTests.Chance;

public class SnapshotTests
{
    [Fact]
    public void Snapshot_WhenCloned_YieldsIdenticalSequences()
    {
        // Arrange
        var rng = new ChiRng();
        var skipCount = 100 + rng.Chance().RollDie(100);
        for (var i = 0; i < skipCount; i++)
            rng.Chance().Next();

        // Act
        var synced = new ChiRng(rng.Snapshot());

        // Assert
        const int verifyCount = 500;
        for (var i = 0; i < verifyCount; i++)
        {
            var expected = rng.Chance().Next();
            var actual = synced.Chance().Next();
            actual.Should().Be(expected);
        }
    }

    [Fact]
    public void Snapshot_Restoration_ReproducesOriginalSequence()
    {
        // Arrange
        var rng = new ChiRng();
        var skipCount = 100 + rng.Chance().RollDie(100);
        for (var i = 0; i < skipCount; i++)
            rng.Chance().Next();

        // Act
        var state = rng.Snapshot();

        var original = new double[42];
        for (var i = 0; i < original.Length; i++)
            original[i] = rng.Pareto(1.0, 1.16).Sample();

        rng = new ChiRng(state);

        // Assert
        foreach (var expected in original)
        {
            var actual = rng.Pareto(1.0, 1.16).Sample();
            actual.Should().Be(expected);
        }
    }
}