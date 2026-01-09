using System.Buffers.Binary;
using System.Globalization;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Validation;

public class BinaryRepresentationTests
{
    [Fact]
    public void GetExponentByteCount_Called_ReturnsZero()
    {
        var value = ChiFixed.One;
        Assert.Equal(0, value.GetExponentByteCount());
    }

    [Fact]
    public void GetExponentShortestBitLength_Called_ReturnsZero()
    {
        var value = ChiFixed.One;
        Assert.Equal(0, value.GetExponentShortestBitLength());
    }

    [Fact]
    public void GetSignificandBitLength_Called_Returns63()
    {
        var value = ChiFixed.One;
        Assert.Equal(63, value.GetSignificandBitLength());
    }

    [Fact]
    public void GetSignificandByteCount_Called_Returns8()
    {
        var value = ChiFixed.One;
        Assert.Equal(8, value.GetSignificandByteCount());
    }

    [Fact]
    public void TryWriteExponent_BigAndLittleEndian_ReturnsFalse()
    {
        var value = ChiFixed.One;
        Span<byte> destination = stackalloc byte[8];

        var bigEndianResult = value.TryWriteExponentBigEndian(destination, out var bigEndianBytes);
        Assert.False(bigEndianResult);
        Assert.Equal(0, bigEndianBytes);

        var littleEndianResult = value.TryWriteExponentLittleEndian(destination, out var littleEndianBytes);
        Assert.False(littleEndianResult);
        Assert.Equal(0, littleEndianBytes);
    }

    [Fact]
    public void TryWriteSignificandBigEndian_SufficientSpan_WritesValueAndReturnsTrue()
    {
        var value = (ChiFixed)123.456m;
        Span<byte> destination = stackalloc byte[8];

        var result = value.TryWriteSignificandBigEndian(destination, out var bytesWritten);

        Assert.True(result);
        Assert.Equal(8, bytesWritten);
        Assert.Equal(value.Raw, BinaryPrimitives.ReadInt64BigEndian(destination));
    }

    [Fact]
    public void TryWriteSignificandBigEndian_InsufficientSpan_ReturnsFalse()
    {
        var value = (ChiFixed)123.456m;
        Span<byte> destination = stackalloc byte[7];

        var result = value.TryWriteSignificandBigEndian(destination, out var bytesWritten);

        Assert.False(result);
        Assert.Equal(0, bytesWritten);
    }

    [Fact]
    public void TryWriteSignificandLittleEndian_SufficientSpan_WritesValueAndReturnsTrue()
    {
        var value = (ChiFixed)123.456m;
        Span<byte> destination = stackalloc byte[8];

        var result = value.TryWriteSignificandLittleEndian(destination, out var bytesWritten);

        Assert.True(result);
        Assert.Equal(8, bytesWritten);
        Assert.Equal(value.Raw, BinaryPrimitives.ReadInt64LittleEndian(destination));
    }

    [Fact]
    public void TryWriteSignificandLittleEndian_InsufficientSpan_ReturnsFalse()
    {
        var value = (ChiFixed)123.456m;
        Span<byte> destination = stackalloc byte[7];

        var result = value.TryWriteSignificandLittleEndian(destination, out var bytesWritten);

        Assert.False(result);
        Assert.Equal(0, bytesWritten);
    }

    [Fact]
    public void BitIncrement_RegularValue_IncrementsRawByOne()
    {
        var value = (ChiFixed)1.5m;
        var result = ChiFixed.BitIncrement(value);
        Assert.Equal(value.Raw + 1, result.Raw);
    }

    [Fact]
    public void BitIncrement_MaxValue_ReturnsPositiveInfinity()
    {
        var result = ChiFixed.BitIncrement(ChiFixed.MaxValue);
        Assert.Equal(ChiFixed.PositiveInfinity, result);
    }

    [Fact]
    public void BitIncrement_Zero_ReturnsEpsilon()
    {
        var result = ChiFixed.BitIncrement(ChiFixed.Zero);
        Assert.Equal(ChiFixed.Epsilon, result);
    }

    [Fact]
    public void BitDecrement_RegularValue_DecrementsRawByOne()
    {
        var value = (ChiFixed)1.5m;
        var result = ChiFixed.BitDecrement(value);
        Assert.Equal(value.Raw - 1, result.Raw);
    }

    [Fact]
    public void BitDecrement_MinValue_ReturnsNegativeInfinity()
    {
        var result = ChiFixed.BitDecrement(ChiFixed.MinValue);
        Assert.Equal(ChiFixed.NegativeInfinity, result);
    }

    [Fact]
    public void BitDecrement_Zero_ReturnsNegativeEpsilon()
    {
        var result = ChiFixed.BitDecrement(ChiFixed.Zero);
        Assert.Equal(new ChiFixed(-1), result);
    }

    [Fact]
    public void BitDecrement_Epsilon_ReturnsZero()
    {
        var result = ChiFixed.BitDecrement(ChiFixed.Epsilon);
        Assert.Equal(ChiFixed.Zero, result);
    }

    [Fact]
    public void ScaleB_PositiveN_MultipliesByPowerOfTwo()
    {
        var value = ChiFixed.One;
        var result = ChiFixed.ScaleB(value, 3);
        Assert.Equal((ChiFixed)8m, result);
    }

    [Fact]
    public void ScaleB_NegativeN_DividesByPowerOfTwo()
    {
        var value = (ChiFixed)8m;
        var result = ChiFixed.ScaleB(value, -3);
        Assert.Equal(ChiFixed.One, result);
    }

    [Fact]
    public void ScaleB_ZeroN_ReturnsSameValue()
    {
        var value = (ChiFixed)42m;
        var result = ChiFixed.ScaleB(value, 0);
        Assert.Equal(value, result);
    }

    [Fact]
    public void ScaleB_LargePositiveN_ReturnsInfinity()
    {
        var value = ChiFixed.One;
        var result = ChiFixed.ScaleB(value, 64);
        Assert.Equal(ChiFixed.PositiveInfinity, result);
    }

    [Fact]
    public void ScaleB_LargeNegativeN_ReturnsZero()
    {
        var value = ChiFixed.One;
        var result = ChiFixed.ScaleB(value, -64);
        Assert.Equal(ChiFixed.Zero, result);
    }

    [Fact]
    public void ScaleB_NegativeWithLargeN_ReturnsNegativeInfinity()
    {
        var value = ChiFixed.NegativeOne;
        var result = ChiFixed.ScaleB(value, 64);
        Assert.Equal(ChiFixed.NegativeInfinity, result);
    }

    [Fact]
    public void ScaleB_Overflow_ReturnsInfinity()
    {
        var value = ChiFixed.MaxValue;
        var result = ChiFixed.ScaleB(value, 1);
        Assert.Equal(ChiFixed.PositiveInfinity, result);
    }

    [Theory]
    [InlineData("1", 1, "2")]
    [InlineData("1", 2, "4")]
    [InlineData("2", 3, "16")]
    [InlineData("0.5", 1, "1")]
    [InlineData("4", -2, "1")]
    public void ScaleB_VariousValues_ReturnsCorrectResult(string inputStr, int n, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.ScaleB(input, n);
        Assert.Equal(expected, result);
    }
}