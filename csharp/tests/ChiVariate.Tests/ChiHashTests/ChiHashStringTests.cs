using FluentAssertions;
using Xunit;

namespace ChiVariate.Tests.ChiHashTests;

/// <summary>
///     Contains unit tests targeting the <see cref="ChiHash.Hash(string)" /> method,
///     which operates on UTF-16 code units.
/// </summary>
public class ChiHashStringTests
{
    /// <summary>
    ///     Verifies that Hash(string) produces the same output when called multiple times with the same input string.
    /// </summary>
    [Theory]
    [InlineData("test")]
    [InlineData("Hello, World!")]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")]
    [InlineData("\t\n\r ")]
    [InlineData("你好世界")] // Example with non-ASCII BMP characters
    [InlineData("👍")] // Example with supplementary character (U+1F44D)
    public void HashString_WithSameInput_ProducesSameOutput(string input)
    {
        // Arrange
        var hash1 = ChiHash.Hash(input);

        // Act
        var hash2 = ChiHash.Hash(input);

        // Assert
        hash2.Should().Be(hash1);
    }

    /// <summary>
    ///     Verifies that Hash(string) produces a consistent, deterministic, and non-zero
    ///     output for an empty input string.
    /// </summary>
    [Fact]
    public void HashString_WithEmptyInput_IsDeterministicAndNonZero()
    {
        // Arrange
        const string emptyString = "";

        // Act
        var actualHash1 = ChiHash.Hash(emptyString);
        var actualHash2 = ChiHash.Hash(emptyString);

        // Assert
        actualHash1.Should().NotBe(0);
        actualHash2.Should().Be(actualHash1);
    }

    /// <summary>
    ///     Verifies that Hash(string) throws an ArgumentNullException when the input string is null.
    /// </summary>
    [Fact]
    public void HashString_WithNullInput_ThrowsArgumentNullException()
    {
        // Arrange
        string? input = null;

        // Act
        Action act = () => ChiHash.Hash(input!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }


    /// <summary>
    ///     Verifies that Hash(string) produces different outputs for different input strings,
    ///     including variations in case and whitespace.
    /// </summary>
    [Theory]
    [InlineData("abc", "abd")]
    [InlineData("Test", "test")]
    [InlineData("Hello", " Hello")]
    [InlineData("World ", "World")]
    [InlineData("One", "Two")]
    [InlineData("A", "B")]
    [InlineData("你好世界", "こんにちは世界")]
    [InlineData("👍", "😁")]
    public void HashString_WithDifferentInputs_ProducesDifferentOutputs(string input1, string input2)
    {
        // Arrange
        var hash1 = ChiHash.Hash(input1);

        // Act
        var hash2 = ChiHash.Hash(input2);

        // Assert
        hash2.Should().NotBe(hash1);
    }
}