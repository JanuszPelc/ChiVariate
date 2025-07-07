using System.Numerics;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiHashTests;

/// <summary>
///     Provides a series of tests to validate the reproducibility and consistency
///     of the ChiHash implementation under various scenarios.
/// </summary>
public class ChiHashReproducibilityTests(ITestOutputHelper testOutputHelper)
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
        VerifyDeterminism(123.456m);
    }

    [Fact]
    public void ChiHash_DifferentNumericValues_ProduceDifferentHashes()
    {
        // Arrange & Act & Assert
        VerifySensitivity(1, 2);
        VerifySensitivity(10L, -10L);
        VerifySensitivity(0UL, 1UL);
        VerifySensitivity<Int128>(0, 1);
        VerifySensitivity<byte>(254, 255);
        VerifySensitivity('a', 'b');

        VerifySensitivity(1.0, 1.000000000000001);
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
        VerifyStringDeterminism("Hello");
        VerifyStringDeterminism("hello world");
        VerifyStringDeterminism("🌍");
        VerifyStringDeterminism("café");
        VerifyStringDeterminism("こんにちは");
        VerifyStringDeterminism(new string('x', 1000));

        string? nullString = null;
        var hashNull1 = new ChiHash().Add(nullString).Hash;
        var hashNull2 = new ChiHash().Add(nullString).Hash;
        Assert.Equal(hashNull1, hashNull2);

        var hashEmpty = new ChiHash().Add("").Hash;
        Assert.Equal(hashNull1, hashEmpty);
    }

    [Fact]
    public void ChiHash_SpecialTypes_ProducesConsistentResults()
    {
        // Arrange & Act & Assert
        VerifyDeterminism(true);
        VerifyDeterminism(false);
        VerifySensitivity(true, false);

        var guid1 = Guid.Parse("12345678-1234-5678-9abc-123456789abc");
        var guid2 = Guid.Parse("87654321-4321-8765-cba9-cba987654321");
        VerifyDeterminism(guid1);
        VerifyDeterminism(guid2);
        VerifySensitivity(guid1, guid2);

        var date1 = new DateTime(2023, 12, 25, 15, 30, 45, DateTimeKind.Utc);
        var date2 = new DateTime(2023, 12, 25, 15, 30, 46, DateTimeKind.Utc);
        VerifyDeterminism(date1);
        VerifyDeterminism(date2);
        VerifySensitivity(date1, date2);

        var dto1 = new DateTimeOffset(2023, 12, 25, 15, 30, 45, TimeSpan.FromHours(5));
        var dto2 = new DateTimeOffset(2023, 12, 25, 15, 30, 45, TimeSpan.FromHours(-5));
        VerifyDeterminism(dto1);
        VerifyDeterminism(dto2);
        VerifySensitivity(dto1, dto2);

        var ts1 = TimeSpan.FromMinutes(123);
        var ts2 = TimeSpan.FromMinutes(124);
        VerifyDeterminism(ts1);
        VerifyDeterminism(ts2);
        VerifySensitivity(ts1, ts2);

        var complex1 = new Complex(1.5, 2.5);
        var complex2 = new Complex(1.5, 2.6);
        VerifyDeterminism(complex1);
        VerifyDeterminism(complex2);
        VerifySensitivity(complex1, complex2);
    }

    [Fact]
    public void ChiHash_Enums_ProducesConsistentResults()
    {
        // Arrange & Act & Assert
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
        // Arrange
        var intArray = new[] { 1, 2, 3, 4, 5 };
        var intArray2 = new[] { 1, 2, 3, 4, 6 };
        var guidArray = new[] { Guid.NewGuid(), Guid.NewGuid() };

        // Act
        var hash1 = new ChiHash().Add((ReadOnlySpan<int>)intArray.AsSpan()).Hash;
        var hash2 = new ChiHash().Add(intArray.AsSpan()).Hash;
        var hash3 = new ChiHash().Add(intArray2.AsSpan()).Hash;
        var guidHash1 = new ChiHash().Add((ReadOnlySpan<Guid>)guidArray.AsSpan()).Hash;
        var guidHash2 = new ChiHash().Add(guidArray.AsSpan()).Hash;

        // Assert
        Assert.Equal(hash1, hash2);
        Assert.NotEqual(hash1, hash3);
        Assert.Equal(guidHash1, guidHash2);
    }

    [Fact]
    public void ChiHash_MixedTypes_ProducesConsistentResults()
    {
        // Arrange
        var guid = Guid.Parse("12345678-1234-5678-9abc-123456789abc");
        var dateTime = new DateTime(2023, 12, 25, 15, 30, 45, DateTimeKind.Utc);

        // Act
        var hash1 = new ChiHash()
            .Add(ChiHash.Seed)
            .Add(42)
            .Add("hello")
            .Add(3.14159)
            .Add(true)
            .Add(guid)
            .Add(dateTime)
            .Add(TestEnum.Value1)
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
            .Hash;

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void ChiHash_DecimalScale_PreservesScaleDifferences()
    {
        // Arrange
        const decimal decimal1 = 1m;
        const decimal decimal10 = 1.0m;
        const decimal decimal100 = 1.00m;
        const decimal decimal1000 = 1.000m;

        // Act
        var hash1 = new ChiHash().Add(decimal1).Hash;
        var hash10 = new ChiHash().Add(decimal10).Hash;
        var hash100 = new ChiHash().Add(decimal100).Hash;
        var hash1000 = new ChiHash().Add(decimal1000).Hash;

        // Assert
        Assert.NotEqual(hash1, hash10);
        Assert.NotEqual(hash1, hash100);
        Assert.NotEqual(hash1, hash1000);
        Assert.NotEqual(hash10, hash100);
        Assert.NotEqual(hash10, hash1000);
        Assert.NotEqual(hash100, hash1000);

        Assert.True(decimal1 == decimal10);
        Assert.True(decimal1 == decimal100);
        Assert.True(decimal1 == decimal1000);

        var bits1 = decimal.GetBits(decimal1);
        var bits10 = decimal.GetBits(decimal10);
        var bits100 = decimal.GetBits(decimal100);

        Assert.NotEqual(bits1[3], bits10[3]);
        Assert.NotEqual(bits1[3], bits100[3]);
    }

    [Fact]
    public void ChiHash_FloatingPointEdgeCases_OutputBitPatterns()
    {
        // Arrange & Act
        testOutputHelper.WriteLine("=== FLOAT (32-bit) EDGE CASES ===");

        OutputFloatBits("float.NaN", float.NaN);
        OutputFloatBits("float.PositiveInfinity", float.PositiveInfinity);
        OutputFloatBits("float.NegativeInfinity", float.NegativeInfinity);
        OutputFloatBits("float.Epsilon", float.Epsilon);
        OutputFloatBits("+0.0f", +0.0f);
        OutputFloatBits("-0.0f", -0.0f);
        OutputFloatBits("0.0f / 0.0f", 0.0f / 0.0f);
        OutputFloatBits("Math.Sqrt(-1.0f)", (float)Math.Sqrt(-1.0));
        OutputFloatBits("float.NaN + 1.0f", float.NaN + 1.0f);
        OutputFloatBits("float.PositiveInfinity - float.PositiveInfinity",
            float.PositiveInfinity - float.PositiveInfinity);

        testOutputHelper.WriteLine("");
        testOutputHelper.WriteLine("=== HALF (16-bit) EDGE CASES ===");

        OutputHalfBits("Half.NaN", Half.NaN);
        OutputHalfBits("Half.PositiveInfinity", Half.PositiveInfinity);
        OutputHalfBits("Half.NegativeInfinity", Half.NegativeInfinity);
        OutputHalfBits("Half.Epsilon", Half.Epsilon);
        OutputHalfBits("(Half)(+0.0f)", (Half)(+0.0f));
        OutputHalfBits("(Half)(-0.0f)", (Half)(-0.0f));
        OutputHalfBits("(Half)(0.0f / 0.0f)", (Half)(0.0f / 0.0f));
        OutputHalfBits("(Half)float.NaN", (Half)float.NaN);
        OutputHalfBits("Half.NaN + (Half)1.0f", Half.NaN + (Half)1.0f);
        OutputHalfBits("Half.PositiveInfinity - Half.PositiveInfinity", Half.PositiveInfinity - Half.PositiveInfinity);

        testOutputHelper.WriteLine("");
        testOutputHelper.WriteLine("=== DOUBLE (64-bit) EDGE CASES ===");

        OutputDoubleBits("double.NaN", double.NaN);
        OutputDoubleBits("double.PositiveInfinity", double.PositiveInfinity);
        OutputDoubleBits("double.NegativeInfinity", double.NegativeInfinity);
        OutputDoubleBits("double.Epsilon", double.Epsilon);
        OutputDoubleBits("+0.0", +0.0);
        OutputDoubleBits("-0.0", -0.0);
        OutputDoubleBits("0.0 / 0.0", 0.0 / 0.0);
        OutputDoubleBits("Math.Sqrt(-1.0)", Math.Sqrt(-1.0));
        OutputDoubleBits("double.NaN + 1.0", double.NaN + 1.0);
        OutputDoubleBits("double.PositiveInfinity - double.PositiveInfinity",
            double.PositiveInfinity - double.PositiveInfinity);
        OutputDoubleBits("Math.Acos(2.0)", Math.Acos(2.0));
        OutputDoubleBits("Math.Log(-1.0)", Math.Log(-1.0));

        testOutputHelper.WriteLine("");
        testOutputHelper.WriteLine("=== CANONICALIZATION VERIFICATION ===");

        var nanHash1 = new ChiHash().Add(float.NaN).Hash;
        var nanHash2 = new ChiHash().Add(0.0f / 0.0f).Hash;
        var zeroHash1 = new ChiHash().Add(+0.0f).Hash;
        var zeroHash2 = new ChiHash().Add(-0.0f).Hash;
        var halfNanHash1 = new ChiHash().Add(Half.NaN).Hash;
        var halfNanHash2 = new ChiHash().Add((Half)float.NaN).Hash;
        var doubleNanHash1 = new ChiHash().Add(double.NaN).Hash;
        var doubleNanHash2 = new ChiHash().Add(Math.Sqrt(-1.0)).Hash;

        // Assert
        Assert.Equal(nanHash1, nanHash2);
        Assert.Equal(zeroHash1, zeroHash2);
        Assert.Equal(halfNanHash1, halfNanHash2);
        Assert.Equal(doubleNanHash1, doubleNanHash2);

        return;

        void OutputFloatBits(string description, float value)
        {
            var rawBits = BitConverter.SingleToUInt32Bits(value);
            var valueHash = new ChiHash().Add(value).Hash;
            var canonicalHash = new ChiHash().Add(rawBits).Hash;
            var canonicalized = valueHash == canonicalHash ? "Canonical" : "Canonicalized";

            testOutputHelper.WriteLine(
                $"{description,-24} | Bits: 0x{rawBits:X8} | Hash: {valueHash,11} | {canonicalized}");
        }

        void OutputDoubleBits(string description, double value)
        {
            var rawBits = BitConverter.DoubleToUInt64Bits(value);
            var valueHash = new ChiHash().Add(value).Hash;
            var canonicalHash = new ChiHash().Add(rawBits).Hash;
            var canonicalized = valueHash == canonicalHash ? "Canonical" : "Canonicalized";

            testOutputHelper.WriteLine(
                $"{description,-24} | Bits: 0x{rawBits:X16} | Hash: {valueHash,11} | {canonicalized}");
        }

        void OutputHalfBits(string description, Half value)
        {
            var rawBits = BitConverter.HalfToUInt16Bits(value);
            var valueHash = new ChiHash().Add(value).Hash;
            var canonicalHash = new ChiHash().Add(rawBits).Hash;
            var canonicalized = valueHash == canonicalHash ? "Canonical" : "Canonicalized";

            testOutputHelper.WriteLine(
                $"{description,-24} | Bits: 0x{rawBits:X4} | Hash: {valueHash,11} | {canonicalized}");
        }
    }

    [Fact]
    public void ChiHash_EmptyState_HasConsistentInitialValue()
    {
        // Arrange & Act
        var hash1 = new ChiHash().Hash;
        var hash2 = new ChiHash().Hash;

        // Assert
        Assert.Equal(hash1, hash2);
        Assert.Equal(0, hash1);
    }

    [Fact]
    public void ChiHash_Seed_StaysTheSameAcrossApplicationRuns()
    {
        // Arrange & Act
        var seed1 = ChiHash.Seed;
        var seed2 = ChiHash.Seed;

        // Assert
        Assert.Equal(seed1, seed2);
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