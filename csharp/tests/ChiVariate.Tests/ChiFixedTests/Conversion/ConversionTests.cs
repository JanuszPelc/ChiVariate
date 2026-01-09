using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Conversion;

public class ConversionTests
{
    [Fact]
    public void TryConvertFromChecked_Byte_Success()
    {
        var success = ChiFixed.TryConvertFromChecked<byte>(42, out var result);

        Assert.True(success);
        Assert.Equal((ChiFixed)42m, result);
    }

    [Fact]
    public void TryConvertFromChecked_SByte_Success()
    {
        var success = ChiFixed.TryConvertFromChecked<sbyte>(-42, out var result);

        Assert.True(success);
        Assert.Equal((ChiFixed)(-42m), result);
    }

    [Fact]
    public void TryConvertFromChecked_Short_Success()
    {
        var success = ChiFixed.TryConvertFromChecked<short>(1000, out var result);

        Assert.True(success);
        Assert.Equal((ChiFixed)1000m, result);
    }

    [Fact]
    public void TryConvertFromChecked_UShort_Success()
    {
        var success = ChiFixed.TryConvertFromChecked<ushort>(1000, out var result);

        Assert.True(success);
        Assert.Equal((ChiFixed)1000m, result);
    }

    [Fact]
    public void TryConvertFromChecked_Int_Success()
    {
        var success = ChiFixed.TryConvertFromChecked(12345, out var result);

        Assert.True(success);
        Assert.Equal((ChiFixed)12345m, result);
    }

    [Fact]
    public void TryConvertFromChecked_UInt_Success()
    {
        var success = ChiFixed.TryConvertFromChecked(12345u, out var result);

        Assert.True(success);
        Assert.Equal((ChiFixed)12345m, result);
    }

    [Fact]
    public void TryConvertFromChecked_LongWithinRange_Success()
    {
        var success = ChiFixed.TryConvertFromChecked(1000L, out var result);

        Assert.True(success);
        Assert.Equal((ChiFixed)1000m, result);
    }

    [Fact]
    public void TryConvertFromChecked_LongTooLarge_Fails()
    {
        var success = ChiFixed.TryConvertFromChecked(long.MaxValue, out _);

        Assert.False(success);
    }

    [Fact]
    public void TryConvertFromChecked_LongTooSmall_Fails()
    {
        var success = ChiFixed.TryConvertFromChecked(long.MinValue, out _);

        Assert.False(success);
    }

    [Fact]
    public void TryConvertFromChecked_ULongWithinRange_Success()
    {
        var success = ChiFixed.TryConvertFromChecked(1000UL, out var result);

        Assert.True(success);
        Assert.Equal((ChiFixed)1000m, result);
    }

    [Fact]
    public void TryConvertFromChecked_ULongTooLarge_Fails()
    {
        var success = ChiFixed.TryConvertFromChecked(ulong.MaxValue, out _);

        Assert.False(success);
    }

    [Fact]
    public void TryConvertFromChecked_Float_Success()
    {
        var success = ChiFixed.TryConvertFromChecked(42.5f, out var result);

        Assert.True(success);
        Assert.Equal((ChiFixed)42.5m, result);
    }

    [Fact]
    public void TryConvertFromChecked_Double_Success()
    {
        var success = ChiFixed.TryConvertFromChecked(42.5, out var result);

        Assert.True(success);
        Assert.Equal((ChiFixed)42.5m, result);
    }

    [Fact]
    public void TryConvertFromChecked_Decimal_Success()
    {
        var success = ChiFixed.TryConvertFromChecked(123.456m, out var result);

        Assert.True(success);
        Assert.Equal((ChiFixed)123.456m, result);
    }

    [Fact]
    public void TryConvertFromChecked_Half_Success()
    {
        var success = ChiFixed.TryConvertFromChecked((Half)42.5, out var result);

        Assert.True(success);
        Assert.Equal((ChiFixed)42.5m, result);
    }

    [Fact]
    public void TryConvertFromChecked_ChiFixed_Success()
    {
        var original = (ChiFixed)42.5m;
        var success = ChiFixed.TryConvertFromChecked(original, out var result);

        Assert.True(success);
        Assert.Equal(original, result);
    }

    [Fact]
    public void TryConvertFromSaturating_LongTooLarge_Saturates()
    {
        var success = ChiFixed.TryConvertFromSaturating(long.MaxValue, out var result);

        Assert.True(success);
        Assert.Equal(ChiFixed.MaxValue, result);
    }

    [Fact]
    public void TryConvertFromSaturating_LongTooSmall_Saturates()
    {
        var success = ChiFixed.TryConvertFromSaturating(long.MinValue, out var result);

        Assert.True(success);
        Assert.Equal(ChiFixed.MinValue, result);
    }

    [Fact]
    public void TryConvertFromSaturating_ULongTooLarge_Saturates()
    {
        var success = ChiFixed.TryConvertFromSaturating(ulong.MaxValue, out var result);

        Assert.True(success);
        Assert.Equal(ChiFixed.MaxValue, result);
    }

    [Fact]
    public void TryConvertFromTruncating_LongTooLarge_Truncates()
    {
        var success = ChiFixed.TryConvertFromTruncating(long.MaxValue, out _);

        Assert.True(success);
    }

    [Fact]
    public void TryConvertToChecked_Byte_Success()
    {
        var value = (ChiFixed)42m;
        var success = ChiFixed.TryConvertToChecked(value, out byte result);

        Assert.True(success);
        Assert.Equal(42, result);
    }

    [Fact]
    public void TryConvertToChecked_ByteNegative_Fails()
    {
        var value = (ChiFixed)(-1m);
        var success = ChiFixed.TryConvertToChecked(value, out byte _);

        Assert.False(success);
    }

    [Fact]
    public void TryConvertToChecked_ByteTooLarge_Fails()
    {
        var value = (ChiFixed)300m;
        var success = ChiFixed.TryConvertToChecked(value, out byte _);

        Assert.False(success);
    }

    [Fact]
    public void TryConvertToChecked_SByte_Success()
    {
        var value = (ChiFixed)(-42m);
        var success = ChiFixed.TryConvertToChecked(value, out sbyte result);

        Assert.True(success);
        Assert.Equal(-42, result);
    }

    [Fact]
    public void TryConvertToChecked_SByteTooLarge_Fails()
    {
        var value = (ChiFixed)200m;
        var success = ChiFixed.TryConvertToChecked(value, out sbyte _);

        Assert.False(success);
    }

    [Fact]
    public void TryConvertToChecked_SByteTooSmall_Fails()
    {
        var value = (ChiFixed)(-200m);
        var success = ChiFixed.TryConvertToChecked(value, out sbyte _);

        Assert.False(success);
    }

    [Fact]
    public void TryConvertToChecked_Int_Success()
    {
        var value = (ChiFixed)12345m;
        var success = ChiFixed.TryConvertToChecked(value, out int result);

        Assert.True(success);
        Assert.Equal(12345, result);
    }

    [Fact]
    public void TryConvertToChecked_IntWithFractional_Rounds()
    {
        var value = (ChiFixed)12.7m;
        var success = ChiFixed.TryConvertToChecked(value, out int result);

        Assert.True(success);
        Assert.Equal(13, result);
    }

    [Fact]
    public void TryConvertToChecked_Long_Success()
    {
        var value = (ChiFixed)12345m;
        var success = ChiFixed.TryConvertToChecked(value, out long result);

        Assert.True(success);
        Assert.Equal(12345L, result);
    }

    [Fact]
    public void TryConvertToChecked_ULong_Success()
    {
        var value = (ChiFixed)12345m;
        var success = ChiFixed.TryConvertToChecked(value, out ulong result);

        Assert.True(success);
        Assert.Equal(12345UL, result);
    }

    [Fact]
    public void TryConvertToChecked_ULongNegative_Fails()
    {
        var value = (ChiFixed)(-1m);
        var success = ChiFixed.TryConvertToChecked(value, out ulong _);

        Assert.False(success);
    }

    [Fact]
    public void TryConvertToChecked_Float_Success()
    {
        var value = (ChiFixed)42.5m;
        var success = ChiFixed.TryConvertToChecked(value, out float result);

        Assert.True(success);
        Assert.Equal(42.5f, result);
    }

    [Fact]
    public void TryConvertToChecked_Double_Success()
    {
        var value = (ChiFixed)42.5m;
        var success = ChiFixed.TryConvertToChecked(value, out double result);

        Assert.True(success);
        Assert.Equal(42.5, result);
    }

    [Fact]
    public void TryConvertToChecked_Decimal_Success()
    {
        var value = (ChiFixed)42.5m;
        var success = ChiFixed.TryConvertToChecked(value, out decimal result);

        Assert.True(success);
        Assert.Equal(42.5m, result);
    }

    [Fact]
    public void TryConvertToChecked_Half_Success()
    {
        var value = (ChiFixed)42.5m;
        var success = ChiFixed.TryConvertToChecked(value, out Half result);

        Assert.True(success);
        Assert.Equal((Half)42.5, result);
    }

    [Fact]
    public void TryConvertToChecked_ChiFixed_Success()
    {
        var value = (ChiFixed)42.5m;
        var success = ChiFixed.TryConvertToChecked(value, out ChiFixed result);

        Assert.True(success);
        Assert.Equal(value, result);
    }

    [Fact]
    public void TryConvertToSaturating_ByteNegative_Saturates()
    {
        var value = (ChiFixed)(-10m);
        var success = ChiFixed.TryConvertToSaturating(value, out byte result);

        Assert.True(success);
        Assert.Equal(0, result);
    }

    [Fact]
    public void TryConvertToSaturating_ByteTooLarge_Saturates()
    {
        var value = (ChiFixed)300m;
        var success = ChiFixed.TryConvertToSaturating(value, out byte result);

        Assert.True(success);
        Assert.Equal(byte.MaxValue, result);
    }

    [Fact]
    public void TryConvertToSaturating_SByteTooLarge_Saturates()
    {
        var value = (ChiFixed)200m;
        var success = ChiFixed.TryConvertToSaturating(value, out sbyte result);

        Assert.True(success);
        Assert.Equal(sbyte.MaxValue, result);
    }

    [Fact]
    public void TryConvertToSaturating_SByteTooSmall_Saturates()
    {
        var value = (ChiFixed)(-200m);
        var success = ChiFixed.TryConvertToSaturating(value, out sbyte result);

        Assert.True(success);
        Assert.Equal(sbyte.MinValue, result);
    }

    [Fact]
    public void TryConvertToSaturating_ULongNegative_Saturates()
    {
        var value = (ChiFixed)(-10m);
        var success = ChiFixed.TryConvertToSaturating(value, out ulong result);

        Assert.True(success);
        Assert.Equal(0UL, result);
    }

    [Fact]
    public void TryConvertToTruncating_ByteNegative_Wraps()
    {
        var value = (ChiFixed)(-1m);
        var success = ChiFixed.TryConvertToTruncating(value, out byte result);

        Assert.True(success);
        Assert.Equal(255, result);
    }

    [Fact]
    public void TryConvertToTruncating_ByteTooLarge_Wraps()
    {
        var value = (ChiFixed)256m;
        var success = ChiFixed.TryConvertToTruncating(value, out byte result);

        Assert.True(success);
        Assert.Equal(0, result);
    }

    [Fact]
    public void TryConvertToTruncating_SByteTooLarge_Wraps()
    {
        var value = (ChiFixed)128m;
        var success = ChiFixed.TryConvertToTruncating(value, out sbyte result);

        Assert.True(success);
        Assert.Equal(-128, result);
    }

    [Fact]
    public void TryConvertToTruncating_IntNaN_ReturnsZero()
    {
        var success = ChiFixed.TryConvertToTruncating(ChiFixed.NaN, out int result);

        Assert.True(success);
        Assert.Equal(0, result);
    }
}