using System.Numerics;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiHashTests;

public class ChiHashReproducibilityTests
{
    [Fact]
    public void ChiHash_WithSameInputs_ProducesSameOutput()
    {
        // Arrange
        const int input1 = 123;
        const double input2 = 456.789;
        const long input3 = -987654321L;

        var hash1 = new ChiHash()
            .Add(input1)
            .Add(input2)
            .Add(input3)
            .Hash;

        // Act
        var hash2 = new ChiHash()
            .Add(input1)
            .Add(input2)
            .Add(input3)
            .Hash;

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void ChiHash_WithInputsInDifferentOrder_ProducesDifferentOutputs()
    {
        // Arrange
        var inputA = 1.23f;
        var inputB = 456;

        var hashOrderAb = new ChiHash()
            .Add(inputA)
            .Add(inputB)
            .Hash;

        // Act
        var hashOrderBa = new ChiHash()
            .Add(inputB)
            .Add(inputA)
            .Hash;

        // Assert
        Assert.NotEqual(hashOrderAb, hashOrderBa);
    }

    [Fact]
    public void ChiHash_WithOneDifferentInput_ProducesDifferentOutput()
    {
        // Arrange
        const int input1 = 123;
        const double input2 = 456.789;
        const long input3A = -987654321L;
        const long input3B = -987654320L;

        var hashA = new ChiHash()
            .Add(input1)
            .Add(input2)
            .Add(input3A)
            .Hash;

        // Act
        var hashB = new ChiHash()
            .Add(input1)
            .Add(input2)
            .Add(input3B)
            .Hash;

        // Assert
        Assert.NotEqual(hashA, hashB);
    }

    [Fact]
    public void ChiHash_WithDifferentNumberOfInputs_ProducesDifferentOutputs()
    {
        // Arrange
        const int input1 = 10;
        const long input2 = 20L;
        const double input3 = 30.0;

        var hash2Inputs = new ChiHash()
            .Add(input1)
            .Add(input2)
            .Hash;

        // Act
        var hash3Inputs = new ChiHash()
            .Add(input1)
            .Add(input2)
            .Add(input3)
            .Hash;

        // Assert
        Assert.NotEqual(hash2Inputs, hash3Inputs);
    }

    [Fact]
    public void ChiHash_NumericTypes_ProducesConsistentResults()
    {
        // Test determinism across all numeric types
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
        VerifyDeterminism(123.456m);
    }

    [Fact]
    public void ChiHash_DifferentNumericValues_ProduceDifferentHashes()
    {
        // Verify sensitivity to input changes
        VerifySensitivity(1, 2);
        VerifySensitivity(10L, -10L);
        VerifySensitivity(0UL, 1UL);
        VerifySensitivity<Int128>(0, 1);
        VerifySensitivity<byte>(254, 255);
        VerifySensitivity('a', 'b');

        // Floating point sensitivity
        VerifySensitivity(1.0, 1.000000000000001);
        VerifySensitivity(0.0, -0.0);
        VerifySensitivity(100.0f, -100.0f);
        VerifySensitivity((Half)1.0f, (Half)1.5f);
        VerifySensitivity(123.456m, 123.457m);
    }

    [Fact]
    public void ChiHash_StringInputs_ProducesConsistentResults()
    {
        // Arrange & Act & Assert
        VerifyStringDeterminism("");
        VerifyStringDeterminism("hello");
        VerifyStringDeterminism("Hello"); // Case sensitivity
        VerifyStringDeterminism("hello world");
        VerifyStringDeterminism("🌍"); // Unicode
        VerifyStringDeterminism("café"); // UTF-8 multi-byte
        VerifyStringDeterminism("こんにちは"); // Japanese
        VerifyStringDeterminism(new string('x', 1000)); // Large string

        // Null handling
        string? nullString = null;
        var hashNull1 = new ChiHash().Add(nullString).Hash;
        var hashNull2 = new ChiHash().Add(nullString).Hash;
        Assert.Equal(hashNull1, hashNull2);

        // Null should equal empty string
        var hashEmpty = new ChiHash().Add("").Hash;
        Assert.Equal(hashNull1, hashEmpty);
    }

    [Fact]
    public void ChiHash_SpecialTypes_ProducesConsistentResults()
    {
        // Bool
        VerifyDeterminism(true);
        VerifyDeterminism(false);
        VerifySensitivity(true, false);

        // Guid
        var guid1 = Guid.Parse("12345678-1234-5678-9abc-123456789abc");
        var guid2 = Guid.Parse("87654321-4321-8765-cba9-cba987654321");
        VerifyDeterminism(guid1);
        VerifyDeterminism(guid2);
        VerifySensitivity(guid1, guid2);

        // DateTime
        var date1 = new DateTime(2023, 12, 25, 15, 30, 45, DateTimeKind.Utc);
        var date2 = new DateTime(2023, 12, 25, 15, 30, 46, DateTimeKind.Utc);
        VerifyDeterminism(date1);
        VerifyDeterminism(date2);
        VerifySensitivity(date1, date2);

        // DateTimeOffset
        var dto1 = new DateTimeOffset(2023, 12, 25, 15, 30, 45, TimeSpan.FromHours(5));
        var dto2 = new DateTimeOffset(2023, 12, 25, 15, 30, 45, TimeSpan.FromHours(-5));
        VerifyDeterminism(dto1);
        VerifyDeterminism(dto2);
        VerifySensitivity(dto1, dto2);

        // TimeSpan
        var ts1 = TimeSpan.FromMinutes(123);
        var ts2 = TimeSpan.FromMinutes(124);
        VerifyDeterminism(ts1);
        VerifyDeterminism(ts2);
        VerifySensitivity(ts1, ts2);

        // Complex
        var complex1 = new Complex(1.5, 2.5);
        var complex2 = new Complex(1.5, 2.6);
        VerifyDeterminism(complex1);
        VerifyDeterminism(complex2);
        VerifySensitivity(complex1, complex2);

        // BigInteger
        var big1 = BigInteger.Parse("123456789012345678901234567890");
        var big2 = BigInteger.Parse("123456789012345678901234567891");
        VerifyDeterminism(big1);
        VerifyDeterminism(big2);
        VerifySensitivity(big1, big2);
    }

    [Fact]
    public void ChiHash_Enums_ProducesConsistentResults()
    {
        // Test enum support
        VerifyDeterminism(TestEnum.Value1);
        VerifyDeterminism(TestEnum.Value2);
        VerifySensitivity(TestEnum.Value1, TestEnum.Value2);

        VerifyDeterminism(TestByteEnum.A);
        VerifyDeterminism(TestByteEnum.B);
        VerifySensitivity(TestByteEnum.A, TestByteEnum.B);

        VerifyDeterminism(TestLongEnum.Large);
        VerifyDeterminism(TestLongEnum.Larger);
        VerifySensitivity(TestLongEnum.Large, TestLongEnum.Larger);
    }

    [Fact]
    public void ChiHash_SpanInputs_ProducesConsistentResults()
    {
        // Numeric spans
        var intArray = new[] { 1, 2, 3, 4, 5 };
        var hash1 = new ChiHash().Add(intArray.AsSpan()).Hash;
        var hash2 = new ChiHash().Add(intArray.AsSpan()).Hash;
        Assert.Equal(hash1, hash2);

        // Different spans should produce different hashes
        var intArray2 = new[] { 1, 2, 3, 4, 6 };
        var hash3 = new ChiHash().Add(intArray2.AsSpan()).Hash;
        Assert.NotEqual(hash1, hash3);

        // Special type spans
        var guidArray = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var guidHash1 = new ChiHash().Add(guidArray.AsSpan()).Hash;
        var guidHash2 = new ChiHash().Add(guidArray.AsSpan()).Hash;
        Assert.Equal(guidHash1, guidHash2);
    }

    [Fact]
    public void ChiHash_MixedTypes_ProducesConsistentResults()
    {
        // Complex mixed scenario
        var guid = Guid.Parse("12345678-1234-5678-9abc-123456789abc");
        var dateTime = new DateTime(2023, 12, 25, 15, 30, 45, DateTimeKind.Utc);
        var bigInt = BigInteger.Parse("123456789012345678901234567890");

        var hash1 = new ChiHash()
            .Add(ChiHash.Seed)
            .Add(42)
            .Add("hello")
            .Add(3.14159)
            .Add(true)
            .Add(guid)
            .Add(dateTime)
            .Add(TestEnum.Value1)
            .Add(bigInt)
            .Hash;

        var hash2 = new ChiHash()
            .Add(ChiHash.Seed)
            .Add(42)
            .Add("hello")
            .Add(3.14159)
            .Add(true)
            .Add(guid)
            .Add(dateTime)
            .Add(TestEnum.Value1)
            .Add(bigInt)
            .Hash;

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void ChiHash_EmptyState_HasConsistentInitialValue()
    {
        var hash1 = new ChiHash().Hash;
        var hash2 = new ChiHash().Hash;
        Assert.Equal(hash1, hash2);
        Assert.Equal(0, hash1); // Should start at 0
    }

    [Fact]
    public void ChiHash_Seed_ChangesAcrossApplicationRuns()
    {
        // Note: This test verifies the Seed property exists and is consistent within the same run
        var seed1 = ChiHash.Seed;
        var seed2 = ChiHash.Seed;
        Assert.Equal(seed1, seed2);
        Assert.NotEqual(0L, seed1); // Should not be zero (extremely unlikely)
    }

    private static void VerifyDeterminism<T>(T input)
    {
        var hash1 = new ChiHash().Add(input).Hash;
        var hash2 = new ChiHash().Add(input).Hash;
        Assert.Equal(hash1, hash2);
    }

    private static void VerifySensitivity<T>(T input1, T input2)
    {
        var hash1 = new ChiHash().Add(input1).Hash;
        var hash2 = new ChiHash().Add(input2).Hash;
        Assert.NotEqual(hash1, hash2);
    }

    private static void VerifyStringDeterminism(string input)
    {
        var hash1 = new ChiHash().Add(input).Hash;
        var hash2 = new ChiHash().Add(input).Hash;
        Assert.Equal(hash1, hash2);
    }

    // Test enums for enum testing
    private enum TestEnum
    {
        Value1 = 10,
        Value2 = 20
    }

    private enum TestByteEnum : byte
    {
        A = 100,
        B = 200
    }

    private enum TestLongEnum : long
    {
        Large = 1_000_000_000L,
        Larger = 2_000_000_000L
    }
}