using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Conversion;

public class ConversionTests
{
    [Fact]
    public void TryConvertFromChecked_Byte_ReturnsExpectedValue()
    {
        var success = ChiFixed.TryConvertFromChecked<byte>(42, out var result);

        success.Should().BeTrue();
        result.Should().Be((ChiFixed)42m);
    }

    [Fact]
    public void TryConvertFromChecked_SByte_ReturnsExpectedValue()
    {
        var success = ChiFixed.TryConvertFromChecked<sbyte>(-42, out var result);

        success.Should().BeTrue();
        result.Should().Be((ChiFixed)(-42m));
    }

    [Fact]
    public void TryConvertFromChecked_Short_ReturnsExpectedValue()
    {
        var success = ChiFixed.TryConvertFromChecked<short>(1000, out var result);

        success.Should().BeTrue();
        result.Should().Be((ChiFixed)1000m);
    }

    [Fact]
    public void TryConvertFromChecked_UShort_ReturnsExpectedValue()
    {
        var success = ChiFixed.TryConvertFromChecked<ushort>(1000, out var result);

        success.Should().BeTrue();
        result.Should().Be((ChiFixed)1000m);
    }

    [Fact]
    public void TryConvertFromChecked_Int_ReturnsExpectedValue()
    {
        var success = ChiFixed.TryConvertFromChecked(12345, out var result);

        success.Should().BeTrue();
        result.Should().Be((ChiFixed)12345m);
    }

    [Fact]
    public void TryConvertFromChecked_UInt_ReturnsExpectedValue()
    {
        var success = ChiFixed.TryConvertFromChecked(12345u, out var result);

        success.Should().BeTrue();
        result.Should().Be((ChiFixed)12345m);
    }

    [Fact]
    public void TryConvertFromChecked_LongWithinRange_ReturnsExpectedValue()
    {
        var success = ChiFixed.TryConvertFromChecked(1000L, out var result);

        success.Should().BeTrue();
        result.Should().Be((ChiFixed)1000m);
    }

    [Fact]
    public void TryConvertFromChecked_LongTooLarge_ReturnsFalse()
    {
        var success = ChiFixed.TryConvertFromChecked(long.MaxValue, out _);

        success.Should().BeFalse();
    }

    [Fact]
    public void TryConvertFromChecked_LongTooSmall_ReturnsFalse()
    {
        var success = ChiFixed.TryConvertFromChecked(long.MinValue, out _);

        success.Should().BeFalse();
    }

    [Fact]
    public void TryConvertFromChecked_ULongWithinRange_ReturnsExpectedValue()
    {
        var success = ChiFixed.TryConvertFromChecked(1000UL, out var result);

        success.Should().BeTrue();
        result.Should().Be((ChiFixed)1000m);
    }

    [Fact]
    public void TryConvertFromChecked_ULongTooLarge_ReturnsFalse()
    {
        var success = ChiFixed.TryConvertFromChecked(ulong.MaxValue, out _);

        success.Should().BeFalse();
    }

    [Fact]
    public void TryConvertFromChecked_Float_ReturnsExpectedValue()
    {
        var success = ChiFixed.TryConvertFromChecked(42.5f, out var result);

        success.Should().BeTrue();
        result.Should().Be((ChiFixed)42.5m);
    }

    [Fact]
    public void TryConvertFromChecked_Double_ReturnsExpectedValue()
    {
        var success = ChiFixed.TryConvertFromChecked(42.5, out var result);

        success.Should().BeTrue();
        result.Should().Be((ChiFixed)42.5m);
    }

    [Fact]
    public void TryConvertFromChecked_Decimal_ReturnsExpectedValue()
    {
        var success = ChiFixed.TryConvertFromChecked(123.456m, out var result);

        success.Should().BeTrue();
        result.Should().Be((ChiFixed)123.456m);
    }

    [Fact]
    public void TryConvertFromChecked_Half_ReturnsExpectedValue()
    {
        var success = ChiFixed.TryConvertFromChecked((Half)42.5, out var result);

        success.Should().BeTrue();
        result.Should().Be((ChiFixed)42.5m);
    }

    [Fact]
    public void TryConvertFromChecked_ChiFixed_ReturnsExpectedValue()
    {
        var original = (ChiFixed)42.5m;
        var success = ChiFixed.TryConvertFromChecked(original, out var result);

        success.Should().BeTrue();
        result.Should().Be(original);
    }

    [Fact]
    public void TryConvertFromSaturating_LongTooLarge_Saturates()
    {
        var success = ChiFixed.TryConvertFromSaturating(long.MaxValue, out var result);

        success.Should().BeTrue();
        result.Should().Be(ChiFixed.MaxValue);
    }

    [Fact]
    public void TryConvertFromSaturating_LongTooSmall_Saturates()
    {
        var success = ChiFixed.TryConvertFromSaturating(long.MinValue, out var result);

        success.Should().BeTrue();
        result.Should().Be(ChiFixed.MinValue);
    }

    [Fact]
    public void TryConvertFromSaturating_ULongTooLarge_Saturates()
    {
        var success = ChiFixed.TryConvertFromSaturating(ulong.MaxValue, out var result);

        success.Should().BeTrue();
        result.Should().Be(ChiFixed.MaxValue);
    }

    [Fact]
    public void TryConvertFromTruncating_LongTooLarge_ReturnsTrue()
    {
        var success = ChiFixed.TryConvertFromTruncating(long.MaxValue, out _);

        success.Should().BeTrue();
    }

    [Fact]
    public void TryConvertToChecked_Byte_ReturnsExpectedValue()
    {
        var value = (ChiFixed)42m;
        var success = ChiFixed.TryConvertToChecked(value, out byte result);

        success.Should().BeTrue();
        result.Should().Be(42);
    }

    [Fact]
    public void TryConvertToChecked_ByteNegative_ReturnsFalse()
    {
        var value = (ChiFixed)(-1m);
        var success = ChiFixed.TryConvertToChecked(value, out byte _);

        success.Should().BeFalse();
    }

    [Fact]
    public void TryConvertToChecked_ByteTooLarge_ReturnsFalse()
    {
        var value = (ChiFixed)300m;
        var success = ChiFixed.TryConvertToChecked(value, out byte _);

        success.Should().BeFalse();
    }

    [Fact]
    public void TryConvertToChecked_SByte_ReturnsExpectedValue()
    {
        var value = (ChiFixed)(-42m);
        var success = ChiFixed.TryConvertToChecked(value, out sbyte result);

        success.Should().BeTrue();
        result.Should().Be(-42);
    }

    [Fact]
    public void TryConvertToChecked_SByteTooLarge_ReturnsFalse()
    {
        var value = (ChiFixed)200m;
        var success = ChiFixed.TryConvertToChecked(value, out sbyte _);

        success.Should().BeFalse();
    }

    [Fact]
    public void TryConvertToChecked_SByteTooSmall_ReturnsFalse()
    {
        var value = (ChiFixed)(-200m);
        var success = ChiFixed.TryConvertToChecked(value, out sbyte _);

        success.Should().BeFalse();
    }

    [Fact]
    public void TryConvertToChecked_Int_ReturnsExpectedValue()
    {
        var value = (ChiFixed)12345m;
        var success = ChiFixed.TryConvertToChecked(value, out int result);

        success.Should().BeTrue();
        result.Should().Be(12345);
    }

    [Fact]
    public void TryConvertToChecked_IntWithFractional_Rounds()
    {
        var value = (ChiFixed)12.7m;
        var success = ChiFixed.TryConvertToChecked(value, out int result);

        success.Should().BeTrue();
        result.Should().Be(13);
    }

    [Fact]
    public void TryConvertToChecked_Long_ReturnsExpectedValue()
    {
        var value = (ChiFixed)12345m;
        var success = ChiFixed.TryConvertToChecked(value, out long result);

        success.Should().BeTrue();
        result.Should().Be(12345L);
    }

    [Fact]
    public void TryConvertToChecked_ULong_ReturnsExpectedValue()
    {
        var value = (ChiFixed)12345m;
        var success = ChiFixed.TryConvertToChecked(value, out ulong result);

        success.Should().BeTrue();
        result.Should().Be(12345UL);
    }

    [Fact]
    public void TryConvertToChecked_ULongNegative_ReturnsFalse()
    {
        var value = (ChiFixed)(-1m);
        var success = ChiFixed.TryConvertToChecked(value, out ulong _);

        success.Should().BeFalse();
    }

    [Fact]
    public void TryConvertToChecked_Float_ReturnsExpectedValue()
    {
        var value = (ChiFixed)42.5m;
        var success = ChiFixed.TryConvertToChecked(value, out float result);

        success.Should().BeTrue();
        result.Should().Be(42.5f);
    }

    [Fact]
    public void TryConvertToChecked_Double_ReturnsExpectedValue()
    {
        var value = (ChiFixed)42.5m;
        var success = ChiFixed.TryConvertToChecked(value, out double result);

        success.Should().BeTrue();
        result.Should().Be(42.5);
    }

    [Fact]
    public void TryConvertToChecked_Decimal_ReturnsExpectedValue()
    {
        var value = (ChiFixed)42.5m;
        var success = ChiFixed.TryConvertToChecked(value, out decimal result);

        success.Should().BeTrue();
        result.Should().Be(42.5m);
    }

    [Fact]
    public void TryConvertToChecked_Half_ReturnsExpectedValue()
    {
        var value = (ChiFixed)42.5m;
        var success = ChiFixed.TryConvertToChecked(value, out Half result);

        success.Should().BeTrue();
        result.Should().Be((Half)42.5);
    }

    [Fact]
    public void TryConvertToChecked_ChiFixed_ReturnsExpectedValue()
    {
        var value = (ChiFixed)42.5m;
        var success = ChiFixed.TryConvertToChecked(value, out ChiFixed result);

        success.Should().BeTrue();
        result.Should().Be(value);
    }

    [Fact]
    public void TryConvertToSaturating_ByteNegative_Saturates()
    {
        var value = (ChiFixed)(-10m);
        var success = ChiFixed.TryConvertToSaturating(value, out byte result);

        success.Should().BeTrue();
        result.Should().Be(0);
    }

    [Fact]
    public void TryConvertToSaturating_ByteTooLarge_Saturates()
    {
        var value = (ChiFixed)300m;
        var success = ChiFixed.TryConvertToSaturating(value, out byte result);

        success.Should().BeTrue();
        result.Should().Be(byte.MaxValue);
    }

    [Fact]
    public void TryConvertToSaturating_SByteTooLarge_Saturates()
    {
        var value = (ChiFixed)200m;
        var success = ChiFixed.TryConvertToSaturating(value, out sbyte result);

        success.Should().BeTrue();
        result.Should().Be(sbyte.MaxValue);
    }

    [Fact]
    public void TryConvertToSaturating_SByteTooSmall_Saturates()
    {
        var value = (ChiFixed)(-200m);
        var success = ChiFixed.TryConvertToSaturating(value, out sbyte result);

        success.Should().BeTrue();
        result.Should().Be(sbyte.MinValue);
    }

    [Fact]
    public void TryConvertToSaturating_ULongNegative_Saturates()
    {
        var value = (ChiFixed)(-10m);
        var success = ChiFixed.TryConvertToSaturating(value, out ulong result);

        success.Should().BeTrue();
        result.Should().Be(0UL);
    }

    [Fact]
    public void TryConvertToTruncating_ByteNegative_Wraps()
    {
        var value = (ChiFixed)(-1m);
        var success = ChiFixed.TryConvertToTruncating(value, out byte result);

        success.Should().BeTrue();
        result.Should().Be(255);
    }

    [Fact]
    public void TryConvertToTruncating_ByteTooLarge_Wraps()
    {
        var value = (ChiFixed)256m;
        var success = ChiFixed.TryConvertToTruncating(value, out byte result);

        success.Should().BeTrue();
        result.Should().Be(0);
    }

    [Fact]
    public void TryConvertToTruncating_SByteTooLarge_Wraps()
    {
        var value = (ChiFixed)128m;
        var success = ChiFixed.TryConvertToTruncating(value, out sbyte result);

        success.Should().BeTrue();
        result.Should().Be(-128);
    }

    [Fact]
    public void TryConvertToTruncating_IntNaN_ReturnsZero()
    {
        var success = ChiFixed.TryConvertToTruncating(ChiFixed.NaN, out int result);

        success.Should().BeTrue();
        result.Should().Be(0);
    }
}