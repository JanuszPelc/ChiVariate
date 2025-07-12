using AwesomeAssertions;
using ChiVariate.Internal;
using Xunit;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSeedTests;

public class ChiSeedApiTests
{
    [Theory]
    [InlineData(0L)]
    [InlineData(1L)]
    [InlineData(-1L)]
    [InlineData(123456789012345L)]
    [InlineData(-987654321098765L)]
    [InlineData(long.MaxValue)]
    [InlineData(long.MinValue)]
    public void Scramble_GivenSameInput_ReturnsSameOutput(long value)
    {
        // Arrange & Act
        var scrambled1 = ChiSeed.Scramble(value);
        var scrambled2 = ChiSeed.Scramble(value);

        // Assert
        scrambled2.Should().Be(scrambled1);
    }

    [Theory]
    [InlineData(0L, 1L)]
    [InlineData(1L, -1L)]
    [InlineData(123L, 124L)]
    [InlineData(long.MaxValue, long.MaxValue - 1)]
    [InlineData(long.MinValue, long.MinValue + 1)]
    [InlineData(0L, -1L)]
    public void Scramble_GivenDifferentInputs_ReturnsDifferentOutputs(long value1, long value2)
    {
        // Arrange
        value1.Should().NotBe(value2);

        // Act
        var scrambled1 = ChiSeed.Scramble(value1);
        var scrambled2 = ChiSeed.Scramble(value2);

        // Assert
        scrambled2.Should().NotBe(scrambled1);
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(1L)]
    [InlineData(-1L)]
    [InlineData(123456789012345L)]
    [InlineData(-987654321098765L)]
    [InlineData(long.MaxValue)]
    [InlineData(long.MinValue)]
    public void Scramble_LongGivenSameInput_ReturnsSameOutput(long value)
    {
        // Arrange & Act
        var scrambled1 = ChiSeed.Scramble(value);
        var scrambled2 = ChiSeed.Scramble(value);

        // Assert
        scrambled2.Should().Be(scrambled1);
    }

    [Theory]
    [InlineData(0L, 1L)]
    [InlineData(1L, -1L)]
    [InlineData(123L, 124L)]
    [InlineData(long.MaxValue, long.MaxValue - 1)]
    [InlineData(long.MinValue, long.MinValue + 1)]
    [InlineData(0L, -1L)]
    public void Scramble_LongGivenDifferentInputs_ReturnsDifferentOutputs(long value1, long value2)
    {
        // Arrange
        value1.Should().NotBe(value2);

        // Act
        var scrambled1 = ChiSeed.Scramble(value1);
        var scrambled2 = ChiSeed.Scramble(value2);

        // Assert
        scrambled2.Should().NotBe(scrambled1);
    }

    [Theory]
    [InlineData("")] // Empty string
    [InlineData("test")]
    [InlineData("Hello, World!")]
    [InlineData("A moderately long string seed for testing determinism.")]
    [InlineData("\t\n\r ")] // Whitespace variations
    [InlineData("你好世界")] // Non-ASCII BMP
    [InlineData("👍")] // Supplementary character
    public void Scramble_StringGivenSameInput_ReturnsSameOutput(string input)
    {
        // Arrange & Act
        var scrambled1 = ChiSeed.Scramble(input);
        var scrambled2 = ChiSeed.Scramble(input);

        // Assert
        scrambled1.Should().Be(scrambled2);
    }

    [Theory]
    [InlineData("abc", "abd")]
    [InlineData("Test", "test")] // Case sensitivity
    [InlineData("Hello", " Hello")] // Whitespace sensitivity
    [InlineData("Short", "Longer")]
    [InlineData("", " ")] // Empty vs Space
    [InlineData("👍", "👎")] // Different supplementary chars
    [InlineData("A", "a")]
    public void
        Scramble_StringGivenDifferentInputs_ReturnsDifferentOutputs(string input1, string input2)
    {
        // Arrange
        input1.Should().NotBe(input2);

        // Act
        var scrambled1 = ChiSeed.Scramble(input1);
        var scrambled2 = ChiSeed.Scramble(input2);

        // Assert
        scrambled2.Should().NotBe(scrambled1);
    }

    [Fact]
    public void Scramble_StringGivenNullInput_ThrowsArgumentNullException() // GWT: Given null string input...
    {
        // Arrange
        string? nullInput = null;
        Action act = () => ChiSeed.Scramble(nullInput!);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Scramble_StringGivenEmptyInput_ReturnsDeterministicOutput()
    {
        // Arrange
        const string emptyString = "";

        // Act
        var scrambled1 = ChiSeed.Scramble(emptyString);
        var scrambled2 = ChiSeed.Scramble(emptyString);

        // Assert
        scrambled1.Should().Be(scrambled2);
        scrambled1.Should().NotBe(0L);
    }

    [Theory]
    [InlineData("", 5091119643593974729L)] // Empty string
    [InlineData("test", -6718970030811709247L)]
    [InlineData("Hello, World!", -5048483389635163709L)]
    [InlineData("A moderately long string seed for testing determinism.", -8572949766761681549L)]
    [InlineData("\t\n\r ", 6552475925946505334L)] // Whitespace variations
    [InlineData("你好世界", 3950144026336539582L)] // Non-ASCII BMP
    [InlineData("👍", -7582485440326701622L)] // Supplementary character
    public void ChiMix64_GivenStringInput_ReturnsDeterministicOutput(string input, long expected)
    {
        // Arrange & Act
        if (string.IsNullOrEmpty(input)) input = null!;
        var scrambled = ChiMix64.MixString(ChiMix64.InitialValue, input);

        // Assert
        scrambled.Should().Be(expected);
    }
}