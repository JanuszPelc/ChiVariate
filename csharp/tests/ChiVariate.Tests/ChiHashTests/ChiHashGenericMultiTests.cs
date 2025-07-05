using System.Numerics;
using AwesomeAssertions;
using Xunit;

namespace ChiVariate.Tests.ChiHashTests;

/// <summary>
///     Contains unit tests targeting the generic <see cref="ChiHash.Hash{T1, T2}" /> methods family
///     for various unmanaged numeric types implementing <see cref="INumberBase{TSelf}" />.
/// </summary>
public class ChiHashGenericMultiTests
{
    /// <summary>
    ///     Verifies that multi-argument generic Hash methods produce the same output when called
    ///     multiple times with the same sequence of inputs.
    ///     Uses <see cref="ChiHash.Hash{T1, T2, T3}" /> as a representative example.
    /// </summary>
    [Fact]
    public void Hash_WithSameGenericMultiArgsInputs_ProducesSameOutput()
    {
        // Arrange
        const int input1 = 123;
        const double input2 = 456.789;
        const long input3 = -987654321L;

        var hash1 = ChiHash.Hash(input1, input2, input3);

        // Act
        var hash2 = ChiHash.Hash(input1, input2, input3);

        // Assert
        hash2.Should().Be(hash1);
    }

    /// <summary>
    ///     Verifies that the order of arguments matters for multi-argument generic Hash methods.
    ///     Uses <see cref="ChiHash.Hash{T1, T2}" /> as a representative example.
    /// </summary>
    [Fact]
    public void Hash_WithGenericMultiArgInputsInDifferentOrder_ProducesDifferentOutputs()
    {
        // Arrange
        var inputA = 1.23f;
        var inputB = 456;

        var hashOrderAb = ChiHash.Hash(inputA, inputB);

        // Act
        var hashOrderBa = ChiHash.Hash(inputB, inputA); // Swapped order

        // Assert
        hashOrderBa.Should().NotBe(hashOrderAb);
    }

    /// <summary>
    ///     Verifies that changing one argument in a multi-argument generic Hash call
    ///     results in a different hash output.
    ///     Uses <see cref="ChiHash.Hash{T1, T2, T3}" /> as a representative example.
    /// </summary>
    [Fact]
    public void Hash_WithOneDifferentGenericMultiArgInput_ProducesDifferentOutput()
    {
        // Arrange
        const int input1 = 123;
        const double input2 = 456.789;
        const long input3A = -987654321L;
        const long input3B = -987654320L;

        var hashA = ChiHash.Hash(input1, input2, input3A);

        // Act
        var hashB = ChiHash.Hash(input1, input2, input3B);

        // Assert
        hashB.Should().NotBe(hashA);
    }

    /// <summary>
    ///     Verifies that different overloads (different number of arguments) produce different results
    ///     even if the initial arguments are the same.
    /// </summary>
    [Fact]
    public void Hash_WithDifferentGenericMultiArgsArgumentCounts_ProducesDifferentOutputs()
    {
        // Arrange
        const int input1 = 10;
        const long input2 = 20L;
        const double input3 = 30.0;

        // Act
        var hashT2 = ChiHash.Hash(input1, input2);
        var hashT3 = ChiHash.Hash(input1, input2, input3);

        // Assert
        hashT3.Should().NotBe(hashT2);
    }
}