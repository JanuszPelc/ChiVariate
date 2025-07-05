using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerUtlTests.Chance;

public class GetItemsTests
{
    private readonly string[] _choices = ["A", "B", "C", "D", "E"];

    // ====================================================================
    // GetItems (With Replacement) Tests
    // ====================================================================

    [Fact]
    public void GetItems_Called_ReturnsCorrectNumberOfItems()
    {
        // Arrange
        var rng = new ChiRng(1);
        const int count = 10;
        var result = new string[count];

        // Act
        rng.Chance().GetItems(_choices, result.AsSpan());

        // Assert
        result.Should().HaveCount(count);
    }

    [Fact]
    public void GetItems_Called_ReturnsOnlyItemsFromSource()
    {
        // Arrange
        var rng = new ChiRng(2);
        const int count = 100;
        var result = new string[count];

        // Act
        rng.Chance().GetItems(_choices, result.AsSpan());

        // Assert
        result.Should().OnlyContain(item => _choices.Contains(item),
            "because all returned items must originate from the choices span.");
    }

    // ====================================================================
    // GetItemsPreferUnique (Without Replacement) Tests
    // ====================================================================

    [Fact]
    public void GetItemsPreferUnique_WhenCountIsLessThanChoices_ReturnsAllUniqueItems()
    {
        // Arrange
        var rng = new ChiRng(3);
        const int count = 3;
        var result = new string[count];

        // Act
        rng.Chance().GetItemsPreferUnique(_choices, result.AsSpan());

        // Assert
        result.Should().HaveCount(count);
        result.Should().OnlyHaveUniqueItems("because the request count is less than the number of available choices.");
        result.Should().BeSubsetOf(_choices, "because all items must come from the source choices.");
    }

    [Fact]
    public void GetItemsPreferUnique_WhenCountIsEqualToChoices_ReturnsShuffledChoices()
    {
        // Arrange
        var rng = new ChiRng(4);
        var count = _choices.Length;
        var result = new string[count];

        // Act
        rng.Chance().GetItemsPreferUnique(_choices, result.AsSpan());

        // Assert
        result.Should().HaveCount(count);
        result.Should().BeEquivalentTo(_choices,
            "because the result should contain the same elements as the source, just in a different order.");
        result.Should().NotEqual(_choices,
            "because the result should be shuffled (this has a tiny chance of failing if it shuffles to the same order).");
    }

    [Fact]
    public void GetItemsPreferUnique_WhenCountIsGreaterThanChoices_ReturnsCorrectNumberOfItems()
    {
        // Arrange
        var rng = new ChiRng(5);
        var count = _choices.Length + 3;
        var result = new string[count];

        // Act
        rng.Chance().GetItemsPreferUnique(_choices, result.AsSpan());

        // Assert
        result.Should().HaveCount(count,
            "because the method must always return the number of items requested.");

        result.Take(_choices.Length).Should().OnlyHaveUniqueItems(
            "because the first batch should be a complete shuffle of the unique choices.");

        result.Should().BeSubsetOf(_choices, "because all items must originate from the choices span.");
    }

    // ... Edge case tests ...

    [Fact]
    public void GetItemsPreferUnique_WithEmptyChoicesAndEmptyCount_ReturnsEmptyArray()
    {
        // Arrange
        var rng = new ChiRng(8);
        var emptyChoices = Array.Empty<string>();
        var result = Array.Empty<string>();

        // Act
        rng.Chance().GetItemsPreferUnique(emptyChoices, result.AsSpan());

        // Assert
        result.Should().BeEmpty();
    }
}