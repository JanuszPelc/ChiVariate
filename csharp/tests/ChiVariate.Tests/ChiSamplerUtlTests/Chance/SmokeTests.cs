using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerUtlTests.Chance;

public class SmokeTests
{
    private const int Seed = 42;
    private const long MaxIntSeed = -1836038751;

    [Fact]
    public void Next_DefaultOverload_ReturnsExpectedValue()
    {
        // Arrange
        var rng = new ChiRng(Seed);

        // Act
        var result = rng.Chance().Next();

        // Assert
        result.Should().Be(-768277222);
    }

    [Fact]
    public void Next_MaxExclusiveOverload_ReturnsExpectedValue()
    {
        // Arrange
        var rng = new ChiRng(Seed);

        // Act
        var result = rng.Chance().Next(1000);

        // Assert
        result.Should().Be(74);
    }

    [Fact]
    public void NextFullRange_WithMaxIntSeed_ReturnsIntMaxValue()
    {
        // Arrange
        var rng = new ChiRng(MaxIntSeed);

        // Act
        var result = rng.Chance().NextFullRange<int>();

        // Assert
        result.Should().Be(int.MaxValue);
    }

    [Fact]
    public void Next_WithMaxIntSeed_ReturnsZero()
    {
        // Arrange
        var rng = new ChiRng(MaxIntSeed);

        // Act
        var result = rng.Chance().Next<int>();

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void Next_GenericMaxExclusiveOverload_ReturnsExpectedValue()
    {
        // Arrange
        var rng = new ChiRng(Seed);

        // Act
        var result = rng.Chance().Next<ushort>(10000);

        // Assert
        result.Should().Be(74);
    }

    [Fact]
    public void Shuffle_Called_ProducesExpectedOrder()
    {
        // Arrange
        var rng = new ChiRng(Seed);
        var items = new[] { "A", "B", "C", "D", "E" };

        // Act
        rng.Chance().Shuffle(items.AsSpan());

        // Assert
        items.Should().Equal("E", "A", "B", "C", "D");
    }

    [Fact]
    public void PickItem_Called_ReturnsExpectedItem()
    {
        // Arrange
        var rng = new ChiRng(Seed);
        var choices = new[] { 10, 20, 30, 40, 50 };

        // Act
        var result = rng.Chance().PickItem(new ReadOnlySpan<int>(choices));

        // Assert
        result.Should().Be(50);
    }

    [Fact]
    public void NextBool_Called_ReturnsExpectedValue()
    {
        // Arrange
        var rng = new ChiRng(Seed);

        // Act
        var result = rng.Chance().NextBool(0.75);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public void NextAngleRadians_ReturnsExpectedValue()
    {
        // Arrange
        var rng = new ChiRng(Seed);

        // Act
        var result = rng.Chance().NextAngleRadians();

        // Assert
        result.Should().BeApproximately(5.15925f, 0.0001f);
    }

    [Fact]
    public void NextAngleDegrees_ReturnsExpectedValue()
    {
        // Arrange
        var rng = new ChiRng(Seed);

        // Act
        var result = rng.Chance().NextAngleDegrees();

        // Assert
        result.Should().BeApproximately(295.60376f, 0.001f);
    }

    [Fact]
    public void PickItem_WithWeights_ReturnsExpectedItem()
    {
        // Arrange
        var rng = new ChiRng(Seed);
        var items = new[] { "Low", "Mid", "High" };
        var weights = new[] { 0.2, 0.3, 0.5 };

        // Act
        var result = rng.Chance().PickItem((ReadOnlySpan<string>)items, weights);

        // Assert
        result.Should().Be("High");
    }

    [Fact]
    public void RollDie_Default_ReturnsExpectedValue()
    {
        // Arrange
        var rng = new ChiRng(Seed);

        // Act
        var result = rng.Chance().RollDie();

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void RollDie_WithCustomSides_ReturnsExpectedValue()
    {
        // Arrange
        var rng = new ChiRng(Seed);

        // Act
        var result = rng.Chance().RollDie(20);

        // Assert
        result.Should().Be(15);
    }

    [Fact]
    public void FlipCoin_ReturnsExpectedValue()
    {
        // Arrange
        var rng = new ChiRng(Seed);

        // Act
        var result = rng.Chance().FlipCoin();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void OneIn_ReturnsExpectedValue()
    {
        // Arrange
        var rng = new ChiRng(Seed);

        // Act
        var result = rng.Chance().OneIn(100);

        // Assert
        result.Should().BeFalse();
    }
}