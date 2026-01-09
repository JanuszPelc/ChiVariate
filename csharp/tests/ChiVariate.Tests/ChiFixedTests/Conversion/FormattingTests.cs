using System.Globalization;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Conversion;

public class FormattingTests
{
    [Fact]
    public void ToString_WithFormatG_ReturnsMinimalRepresentation()
    {
        var value = (ChiFixed)42.5m;

        var result = value.ToString("G", CultureInfo.InvariantCulture);

        Assert.Equal("42.5", result);
    }

    [Fact]
    public void ToString_WithFormatF_ReturnsFixedPoint()
    {
        var value = (ChiFixed)42.5m;

        var result = value.ToString("F", CultureInfo.InvariantCulture);

        Assert.StartsWith("42.5", result);
    }

    [Fact]
    public void ToString_WithFormatF2_ReturnsTwoDecimals()
    {
        var value = (ChiFixed)42.5m;

        var result = value.ToString("F2", CultureInfo.InvariantCulture);

        Assert.Equal("42.50", result);
    }

    [Fact]
    public void ToString_WithFormatF5_ReturnsFiveDecimals()
    {
        var value = (ChiFixed)3.14m;

        var result = value.ToString("F5", CultureInfo.InvariantCulture);

        Assert.Equal("3.14000", result);
    }

    [Fact]
    public void ToString_WithFormatF0_ReturnsNoDecimals()
    {
        var value = (ChiFixed)42.7m;

        var result = value.ToString("F0", CultureInfo.InvariantCulture);

        Assert.Equal("43", result);
    }

    [Fact]
    public void ToString_NullFormat_UsesGeneralFormat()
    {
        var value = (ChiFixed)42.5m;

        var result = value.ToString(null, CultureInfo.InvariantCulture);

        Assert.Equal("42.5", result);
    }

    [Fact]
    public void ToString_EmptyFormat_UsesGeneralFormat()
    {
        var value = (ChiFixed)42.5m;

        var result = value.ToString("", CultureInfo.InvariantCulture);

        Assert.Equal("42.5", result);
    }

    [Fact]
    public void ToString_GermanCulture_UsesCommaDecimalSeparator()
    {
        var value = (ChiFixed)123.456m;
        var germanCulture = new CultureInfo("de-DE");

        var result = value.ToString("F3", germanCulture);

        Assert.Equal("123,456", result);
    }

    [Fact]
    public void ToString_FrenchCulture_UsesCommaDecimalSeparator()
    {
        var value = (ChiFixed)42.5m;
        var frenchCulture = new CultureInfo("fr-FR");

        var result = value.ToString("F1", frenchCulture);

        Assert.Equal("42,5", result);
    }

    [Fact]
    public void ToString_InvalidFormat_ThrowsFormatException()
    {
        var value = (ChiFixed)42m;

        Assert.Throws<FormatException>(() => value.ToString("X", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void TryFormat_SufficientSpace_ReturnsTrue()
    {
        var value = (ChiFixed)42.5m;
        Span<char> buffer = stackalloc char[100];

        var success = value.TryFormat(buffer, out var charsWritten, "G", CultureInfo.InvariantCulture);

        Assert.True(success);
        Assert.Equal("42.5", buffer[..charsWritten].ToString());
    }

    [Fact]
    public void TryFormat_InsufficientSpace_ReturnsFalse()
    {
        var value = (ChiFixed)42.5m;
        Span<char> buffer = stackalloc char[2];

        var success = value.TryFormat(buffer, out var charsWritten, "G", CultureInfo.InvariantCulture);

        Assert.False(success);
        Assert.Equal(0, charsWritten);
    }

    [Fact]
    public void TryFormat_WithFormatF3_FormatsCorrectly()
    {
        var value = (ChiFixed)3.14m;
        Span<char> buffer = stackalloc char[100];

        var success = value.TryFormat(buffer, out var charsWritten, "F3", CultureInfo.InvariantCulture);

        Assert.True(success);
        Assert.Equal("3.140", buffer[..charsWritten].ToString());
    }

    [Fact]
    public void TryFormat_WithCulture_RespectsDecimalSeparator()
    {
        var value = (ChiFixed)123.45m;
        var germanCulture = new CultureInfo("de-DE");
        Span<char> buffer = stackalloc char[100];

        var success = value.TryFormat(buffer, out var charsWritten, "F2", germanCulture);

        Assert.True(success);
        Assert.Equal("123,45", buffer[..charsWritten].ToString());
    }

    [Theory]
    [InlineData(0, "G", "0")]
    [InlineData(1, "G", "1")]
    [InlineData(-1, "G", "-1")]
    [InlineData(0.5, "F1", "0.5")]
    [InlineData(123.456, "F2", "123.46")]
    [InlineData(999.999, "F1", "1000.0")]
    public void ToString_VariousFormats_ProducesExpectedOutput(decimal value, string format, string expected)
    {
        var fixedValue = (ChiFixed)value;

        var result = fixedValue.ToString(format, CultureInfo.InvariantCulture);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToString_FormatG_MatchesDefaultToString()
    {
        var value = (ChiFixed)42.5m;

        var withFormat = value.ToString("G", CultureInfo.InvariantCulture);
        var withoutFormat = value.ToString();

        Assert.Equal(withoutFormat, withFormat);
    }

    [Fact]
    public void ToString_GeneralFormat_WorksWithNonEnglishCurrentCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");

            var value = (ChiFixed)123.450000m;
            Span<char> buffer = stackalloc char[100];
            var success = value.TryFormat(buffer, out var charsWritten, "G", CultureInfo.InvariantCulture);
            var result = buffer[..charsWritten].ToString();

            Assert.True(success);
            Assert.Equal("123.45", result);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}