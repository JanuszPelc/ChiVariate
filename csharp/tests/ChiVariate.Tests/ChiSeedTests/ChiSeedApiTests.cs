using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSeedTests;

/// <summary>
///     Contains unit tests for the static methods within the <see cref="ChiHash" /> class.
/// </summary>
public class ChiSeedApiTests
{
    /// <summary>
    ///     Verifies that <see cref="ChiSeed.Scramble(long)" /> produces the same output when called
    ///     multiple times with the same input value.
    /// </summary>
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

    /// <summary>
    ///     Verifies that <see cref="ChiSeed.Scramble(long)" /> produces different outputs when called
    ///     with different input values.
    /// </summary>
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

    /// <summary>
    ///     Verifies that <see cref="ChiSeed.Scramble(long)" /> produces the same output when called
    ///     multiple times with the same input value.
    /// </summary>
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

    /// <summary>
    ///     Verifies that <see cref="ChiSeed.Scramble(long)" /> produces different outputs when called
    ///     with different input values.
    /// </summary>
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

    /// <summary>
    ///     Verifies that <see cref="ChiSeed.Scramble(string)" /> produces the same output when called
    ///     multiple times with the same input string.
    /// </summary>
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

    /// <summary>
    ///     Verifies that <see cref="ChiSeed.Scramble(string)" /> produces different outputs when called
    ///     with different input strings.
    /// </summary>
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

    /// <summary>
    ///     Verifies that <see cref="ChiSeed.Scramble(string)" /> throws an ArgumentNullException when the
    ///     input string is null.
    /// </summary>
    [Fact]
    public void Scramble_StringGivenNullInput_ThrowsArgumentNullException() // GWT: Given null string input...
    {
        // Arrange
        string? nullInput = null;
        Action act = () => ChiSeed.Scramble(nullInput!);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    ///     Verifies that <see cref="ChiSeed.Scramble(string)" /> produces a deterministic and non-zero
    ///     output for an empty input string.
    /// </summary>
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

    /// <summary>
    ///     Verifies that <see cref="ChiSeed.Core.Hash64(string)" /> produces a deterministic output.
    /// </summary>
    [Theory]
    [InlineData("", 0L)] // Empty string
    [InlineData("test", 5151988072576516256L)]
    [InlineData("Hello, World!", -5175036796766112071L)]
    [InlineData("A moderately long string seed for testing determinism.", -6960743544550810265L)]
    [InlineData("\t\n\r ", -7339662360663615370L)] // Whitespace variations
    [InlineData("你好世界", -1060883054426207415L)] // Non-ASCII BMP
    [InlineData("👍", -8015312806335124022L)] // Supplementary character
    public void CoreHash64_GivenStringInput_ReturnsDeterministicOutput(string input, long expected)
    {
        // Arrange & Act
        var scrambled = ChiSeed.Core.Hash64(input);

        // Assert
        scrambled.Should().Be(expected);
    }
}