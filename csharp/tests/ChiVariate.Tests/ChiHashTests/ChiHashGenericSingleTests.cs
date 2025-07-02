using System.Numerics;
using FluentAssertions;
using Xunit;

namespace ChiVariate.Tests.ChiHashTests;

/// <summary>
///     Contains unit tests targeting the generic <see cref="ChiHash.Hash{T1}(T1)" /> methods
///     for various unmanaged numeric types implementing <see cref="INumber{TSelf}" />.
/// </summary>
public class ChiHashGenericSingleTests
{
    /// <summary>
    ///     Verifies that Hash produces the same output when called multiple times with the same
    ///     input value for various numeric types.
    /// </summary>
    [Fact]
    public void HashT1_WithSameInput_ProducesSameOutput()
    {
        // Arrange & Act & Assert
        VerifyDeterminism(12345);
        VerifyDeterminism(54321U);
        VerifyDeterminism(9876543210L);
        VerifyDeterminism(12345678901234567890UL);
        VerifyDeterminism<short>(123);
        VerifyDeterminism<ushort>(456);
        VerifyDeterminism<sbyte>(-42);
        VerifyDeterminism<byte>(42);
        VerifyDeterminism(Int128.MaxValue / 2);
        VerifyDeterminism(UInt128.MaxValue / 3);
        VerifyDeterminism('X');

        VerifyDeterminism(BitConverter.Int64BitsToDouble(0x123456789ABCDEF0L));
        VerifyDeterminism(BitConverter.Int32BitsToSingle(0x12345678));
        VerifyDeterminism(BitConverter.UInt16BitsToHalf(0x1234));
    }

    /// <summary>
    ///     Verifies that Hash produces different outputs for different input values of the same type.
    /// </summary>
    [Fact]
    public void Hash_WithDifferentGenericSingleArgInputsOfSameType_ProducesDifferentOutputs()
    {
        // Arrange & Act & Assert
        // Integer Types
        VerifySensitivity(1, 2);
        VerifySensitivity(10L, -10L);
        VerifySensitivity(0UL, 1UL);
        VerifySensitivity<Int128>(0, 1);
        VerifySensitivity<byte>(254, 255);
        VerifySensitivity('a', 'b');

        // Floating Point Types
        VerifySensitivity(1.0, 1.000000000000001);
        VerifySensitivity(0.0, -0.0);
        VerifySensitivity(100.0f, -100.0f);
        VerifySensitivity((Half)1.0f, (Half)1.5f);
    }

    /// <summary>
    ///     Verifies how Hash handles inputs of different types that represent the same numeric value,
    ///     producing identical hashes for types normalized to the same int sequence and different hashes otherwise.
    /// </summary>
    [Fact]
    public void Hash_WithSameGenericSingleArgValueDifferentTypes_ProducesExpectedEqualityAndDifferences()
    {
        // Arrange & Act
        var hashInt = ChiHash.Hash(1);
        var hashByte = ChiHash.Hash((byte)1);
        var hashSByte = ChiHash.Hash((sbyte)1);
        var hashShort = ChiHash.Hash((short)1);
        var hashUShort = ChiHash.Hash((ushort)1);
        var hashChar = ChiHash.Hash((char)1);

        var hashLong = ChiHash.Hash(1L);
        var hashULong = ChiHash.Hash(1UL);

        var hashFloat = ChiHash.Hash(1.0f);
        var hashDouble = ChiHash.Hash(1.0);
        var hashHalf = ChiHash.Hash((Half)1.0f);

        hashInt.Should().Be(hashByte);
        hashInt.Should().Be(hashSByte);
        hashInt.Should().Be(hashShort);
        hashInt.Should().Be(hashUShort);
        hashInt.Should().Be(hashChar);

        hashLong.Should().Be(hashULong);
        hashInt.Should().Be(hashLong);

        hashInt.Should().NotBe(hashFloat);
        hashInt.Should().NotBe(hashDouble);
        hashInt.Should().NotBe(hashHalf);
        hashFloat.Should().NotBe(hashDouble);
        hashFloat.Should().NotBe(hashHalf);
        hashDouble.Should().NotBe(hashHalf);
    }

    private static void VerifyDeterminism<T>(T input) where T : unmanaged, INumber<T>
    {
        var hash1 = ChiHash.Hash(input);
        var hash2 = ChiHash.Hash(input);
        hash2.Should().Be(hash1);
    }

    private static void VerifySensitivity<T>(T input1, T input2) where T : unmanaged, INumber<T>
    {
        var hash1 = ChiHash.Hash(input1);
        var hash2 = ChiHash.Hash(input2);
        hash2.Should().NotBe(hash1);
    }
}