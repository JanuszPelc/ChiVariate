using System.Buffers.Binary;
using System.Globalization;
using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Validation;

public class BinaryRepresentationTests
{
    [Fact]
    public void GetExponentByteCount_Default_ReturnsZero()
    {
        var value = ChiFixed.One;
        value.GetExponentByteCount().Should().Be(0);
    }

    [Fact]
    public void GetExponentShortestBitLength_Default_ReturnsZero()
    {
        var value = ChiFixed.One;
        value.GetExponentShortestBitLength().Should().Be(0);
    }

    [Fact]
    public void GetSignificandBitLength_Default_Returns63()
    {
        var value = ChiFixed.One;
        value.GetSignificandBitLength().Should().Be(63);
    }

    [Fact]
    public void GetSignificandByteCount_Default_Returns8()
    {
        var value = ChiFixed.One;
        value.GetSignificandByteCount().Should().Be(8);
    }

    [Fact]
    public void TryWriteExponent_BigAndLittleEndian_ReturnsFalse()
    {
        var value = ChiFixed.One;
        Span<byte> destination = stackalloc byte[8];

        var bigEndianResult = value.TryWriteExponentBigEndian(destination, out var bigEndianBytes);
        bigEndianResult.Should().BeFalse();
        bigEndianBytes.Should().Be(0);

        var littleEndianResult = value.TryWriteExponentLittleEndian(destination, out var littleEndianBytes);
        littleEndianResult.Should().BeFalse();
        littleEndianBytes.Should().Be(0);
    }

    [Fact]
    public void TryWriteSignificandBigEndian_SufficientSpan_WritesValueAndReturnsTrue()
    {
        var value = (ChiFixed)123.456m;
        Span<byte> destination = stackalloc byte[8];

        var result = value.TryWriteSignificandBigEndian(destination, out var bytesWritten);

        result.Should().BeTrue();
        bytesWritten.Should().Be(8);
        BinaryPrimitives.ReadInt64BigEndian(destination).Should().Be(value.Raw);
    }

    [Fact]
    public void TryWriteSignificandBigEndian_InsufficientSpan_ReturnsFalse()
    {
        var value = (ChiFixed)123.456m;
        Span<byte> destination = stackalloc byte[7];

        var result = value.TryWriteSignificandBigEndian(destination, out var bytesWritten);

        result.Should().BeFalse();
        bytesWritten.Should().Be(0);
    }

    [Fact]
    public void TryWriteSignificandLittleEndian_SufficientSpan_WritesValueAndReturnsTrue()
    {
        var value = (ChiFixed)123.456m;
        Span<byte> destination = stackalloc byte[8];

        var result = value.TryWriteSignificandLittleEndian(destination, out var bytesWritten);

        result.Should().BeTrue();
        bytesWritten.Should().Be(8);
        BinaryPrimitives.ReadInt64LittleEndian(destination).Should().Be(value.Raw);
    }

    [Fact]
    public void TryWriteSignificandLittleEndian_InsufficientSpan_ReturnsFalse()
    {
        var value = (ChiFixed)123.456m;
        Span<byte> destination = stackalloc byte[7];

        var result = value.TryWriteSignificandLittleEndian(destination, out var bytesWritten);

        result.Should().BeFalse();
        bytesWritten.Should().Be(0);
    }

    [Fact]
    public void BitIncrement_RegularValue_IncrementsRawByOne()
    {
        var value = (ChiFixed)1.5m;
        var result = ChiFixed.BitIncrement(value);
        result.Raw.Should().Be(value.Raw + 1);
    }

    [Fact]
    public void BitIncrement_MaxValue_ReturnsPositiveInfinity()
    {
        var result = ChiFixed.BitIncrement(ChiFixed.MaxValue);
        result.Should().Be(ChiFixed.PositiveInfinity);
    }

    [Fact]
    public void BitIncrement_Zero_ReturnsEpsilon()
    {
        var result = ChiFixed.BitIncrement(ChiFixed.Zero);
        result.Should().Be(ChiFixed.Epsilon);
    }

    [Fact]
    public void BitDecrement_RegularValue_DecrementsRawByOne()
    {
        var value = (ChiFixed)1.5m;
        var result = ChiFixed.BitDecrement(value);
        result.Raw.Should().Be(value.Raw - 1);
    }

    [Fact]
    public void BitDecrement_MinValue_ReturnsNegativeInfinity()
    {
        var result = ChiFixed.BitDecrement(ChiFixed.MinValue);
        result.Should().Be(ChiFixed.NegativeInfinity);
    }

    [Fact]
    public void BitDecrement_Zero_ReturnsNegativeEpsilon()
    {
        var result = ChiFixed.BitDecrement(ChiFixed.Zero);
        result.Should().Be(new ChiFixed(-1));
    }

    [Fact]
    public void BitDecrement_Epsilon_ReturnsZero()
    {
        var result = ChiFixed.BitDecrement(ChiFixed.Epsilon);
        result.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void ScaleB_PositiveN_MultipliesByPowerOfTwo()
    {
        var value = ChiFixed.One;
        var result = ChiFixed.ScaleB(value, 3);
        result.Should().Be((ChiFixed)8m);
    }

    [Fact]
    public void ScaleB_NegativeN_DividesByPowerOfTwo()
    {
        var value = (ChiFixed)8m;
        var result = ChiFixed.ScaleB(value, -3);
        result.Should().Be(ChiFixed.One);
    }

    [Fact]
    public void ScaleB_ZeroN_ReturnsSameValue()
    {
        var value = (ChiFixed)42m;
        var result = ChiFixed.ScaleB(value, 0);
        result.Should().Be(value);
    }

    [Fact]
    public void ScaleB_LargePositiveN_ReturnsPositiveInfinity()
    {
        var value = ChiFixed.One;
        var result = ChiFixed.ScaleB(value, 64);
        result.Should().Be(ChiFixed.PositiveInfinity);
    }

    [Fact]
    public void ScaleB_LargeNegativeN_ReturnsZero()
    {
        var value = ChiFixed.One;
        var result = ChiFixed.ScaleB(value, -64);
        result.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void ScaleB_NegativeWithLargeN_ReturnsNegativeInfinity()
    {
        var value = ChiFixed.NegativeOne;
        var result = ChiFixed.ScaleB(value, 64);
        result.Should().Be(ChiFixed.NegativeInfinity);
    }

    [Fact]
    public void ScaleB_Overflow_ReturnsPositiveInfinity()
    {
        var value = ChiFixed.MaxValue;
        var result = ChiFixed.ScaleB(value, 1);
        result.Should().Be(ChiFixed.PositiveInfinity);
    }

    [Theory]
    [InlineData("1", 1, "2")]
    [InlineData("1", 2, "4")]
    [InlineData("2", 3, "16")]
    [InlineData("0.5", 1, "1")]
    [InlineData("4", -2, "1")]
    public void ScaleB_VariousValues_ReturnsExpectedResult(string inputStr, int n, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.ScaleB(input, n);
        result.Should().Be(expected);
    }
}