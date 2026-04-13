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
        var rng = new ChiRng(Seed);

        var result = rng.Chance().Next();

        result.Should().Be(-768277222);
    }

    [Fact]
    public void Next_MaxExclusiveOverload_ReturnsExpectedValue()
    {
        var rng = new ChiRng(Seed);

        var result = rng.Chance().Next(1000);

        result.Should().Be(74);
    }

    [Fact]
    public void NextFullRange_WithMaxIntSeed_ReturnsIntMaxValue()
    {
        var rng = new ChiRng(MaxIntSeed);

        var result = rng.Chance().NextFullRange<int>();

        result.Should().Be(int.MaxValue);
    }

    [Fact]
    public void Next_WithMaxIntSeed_ReturnsZero()
    {
        var rng = new ChiRng(MaxIntSeed);

        var result = rng.Chance().Next<int>();

        result.Should().Be(0);
    }

    [Fact]
    public void Next_GenericMaxExclusiveOverload_ReturnsExpectedValue()
    {
        var rng = new ChiRng(Seed);

        var result = rng.Chance().Next<ushort>(10000);

        result.Should().Be(74);
    }

    [Fact]
    public void Shuffle_Called_ProducesExpectedOrder()
    {
        var rng = new ChiRng(Seed);
        var items = new[] { "A", "B", "C", "D", "E" };

        rng.Chance().Shuffle(items.AsSpan());

        items.Should().Equal("E", "A", "B", "C", "D");
    }

    [Fact]
    public void PickItem_Called_ReturnsExpectedItem()
    {
        var rng = new ChiRng(Seed);
        var choices = new[] { 10, 20, 30, 40, 50 };

        var result = rng.Chance().PickItem(new ReadOnlySpan<int>(choices));

        result.Should().Be(50);
    }

    [Fact]
    public void NextBool_Called_ReturnsExpectedValue()
    {
        var rng = new ChiRng(Seed);

        var result = rng.Chance().NextBool(0.75);

        result.Should().Be(false);
    }


    [Fact]
    public void PickItem_WithWeights_ReturnsExpectedItem()
    {
        var rng = new ChiRng(Seed);
        var items = new[] { "Low", "Mid", "High" };
        var weights = new[] { 0.2, 0.3, 0.5 };

        var result = rng.Chance().PickItem((ReadOnlySpan<string>)items, weights);

        result.Should().Be("High");
    }

    [Fact]
    public void RollDie_Default_ReturnsExpectedValue()
    {
        var rng = new ChiRng(Seed);

        var result = rng.Chance().RollDie();

        result.Should().Be(1);
    }

    [Fact]
    public void RollDie_WithCustomSides_ReturnsExpectedValue()
    {
        var rng = new ChiRng(Seed);

        var result = rng.Chance().RollDie(20);

        result.Should().Be(15);
    }

    [Fact]
    public void FlipCoin_ReturnsExpectedValue()
    {
        var rng = new ChiRng(Seed);

        var result = rng.Chance().FlipCoin();

        result.Should().BeFalse();
    }

    [Fact]
    public void OneIn_ReturnsExpectedValue()
    {
        var rng = new ChiRng(Seed);

        var result = rng.Chance().OneIn(100);

        result.Should().BeFalse();
    }

    [Fact]
    public void NextFixed_ReturnsExpectedValue()
    {
        var rng = new ChiRng(Seed);

        var result = rng.Chance().NextFixed();

        result.Raw.Should().Be(3526690074L);
    }
}