using AwesomeAssertions;
using Xunit;

namespace ChiVariate.Tests.ChiHashTests;

/// <summary>
///     Contains unit tests for the static ChiHash class and its <see cref="ChiHash.Hash(int)" /> method.
/// </summary>
public class ChiHashIntTests
{
    /// <summary>
    ///     Verifies that Hash(int) produces the same output when called multiple times with the same input value.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(123456789)]
    [InlineData(-987654321)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void HashInt_WithSameInput_ProducesSameOutput(int input)
    {
        // Arrange
        var hash1 = ChiHash.Hash(input);

        // Act
        var hash2 = ChiHash.Hash(input);

        // Assert
        hash2.Should().Be(hash1);
    }

    /// <summary>
    ///     Verifies that Hash(int) produces different outputs for different input values.
    /// </summary>
    [Theory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    [InlineData(123, 124)]
    [InlineData(-500, 500)]
    [InlineData(int.MaxValue, int.MaxValue - 1)]
    [InlineData(int.MinValue, int.MinValue + 1)]
    [InlineData(0, -1)]
    public void Hash_WithDifferentIntInputs_ProducesDifferentOutputs(int input1, int input2)
    {
        // Arrange
        var hash1 = ChiHash.Hash(input1);

        // Act
        var hash2 = ChiHash.Hash(input2);

        // Assert
        hash2.Should().NotBe(hash1);
    }
}