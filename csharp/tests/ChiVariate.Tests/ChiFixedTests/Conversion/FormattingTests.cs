using System.Globalization;
using AwesomeAssertions;
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

        result.Should().Be("42.5");
    }

    [Fact]
    public void ToString_WithFormatF_ReturnsFixedPoint()
    {
        var value = (ChiFixed)42.5m;

        var result = value.ToString("F", CultureInfo.InvariantCulture);

        result.Should().StartWith("42.5");
    }

    [Fact]
    public void ToString_WithFormatF2_ReturnsTwoDecimals()
    {
        var value = (ChiFixed)42.5m;

        var result = value.ToString("F2", CultureInfo.InvariantCulture);

        result.Should().Be("42.50");
    }

    [Fact]
    public void ToString_WithFormatF5_ReturnsFiveDecimals()
    {
        var value = (ChiFixed)3.14m;

        var result = value.ToString("F5", CultureInfo.InvariantCulture);

        result.Should().Be("3.14000");
    }

    [Fact]
    public void ToString_WithFormatF0_ReturnsNoDecimals()
    {
        var value = (ChiFixed)42.7m;

        var result = value.ToString("F0", CultureInfo.InvariantCulture);

        result.Should().Be("43");
    }

    [Fact]
    public void ToString_NullFormat_UsesGeneralFormat()
    {
        var value = (ChiFixed)42.5m;

        var result = value.ToString(null, CultureInfo.InvariantCulture);

        result.Should().Be("42.5");
    }

    [Fact]
    public void ToString_EmptyFormat_UsesGeneralFormat()
    {
        var value = (ChiFixed)42.5m;

        var result = value.ToString("", CultureInfo.InvariantCulture);

        result.Should().Be("42.5");
    }

    [Fact]
    public void ToString_GermanCulture_UsesCommaDecimalSeparator()
    {
        var value = (ChiFixed)123.456m;
        var germanCulture = new CultureInfo("de-DE");

        var result = value.ToString("F3", germanCulture);

        result.Should().Be("123,456");
    }

    [Fact]
    public void ToString_FrenchCulture_UsesCommaDecimalSeparator()
    {
        var value = (ChiFixed)42.5m;
        var frenchCulture = new CultureInfo("fr-FR");

        var result = value.ToString("F1", frenchCulture);

        result.Should().Be("42,5");
    }

    [Fact]
    public void ToString_InvalidFormat_ThrowsFormatException()
    {
        var value = (ChiFixed)42m;

        var act = () => value.ToString("X", CultureInfo.InvariantCulture);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void TryFormat_SufficientSpace_ReturnsTrue()
    {
        var value = (ChiFixed)42.5m;
        Span<char> buffer = stackalloc char[100];

        var success = value.TryFormat(buffer, out var charsWritten, "G", CultureInfo.InvariantCulture);

        success.Should().BeTrue();
        buffer[..charsWritten].ToString().Should().Be("42.5");
    }

    [Fact]
    public void TryFormat_InsufficientSpace_ReturnsFalse()
    {
        var value = (ChiFixed)42.5m;
        Span<char> buffer = stackalloc char[2];

        var success = value.TryFormat(buffer, out var charsWritten, "G", CultureInfo.InvariantCulture);

        success.Should().BeFalse();
        charsWritten.Should().Be(0);
    }

    [Fact]
    public void TryFormat_WithFormatF3_FormatsCorrectly()
    {
        var value = (ChiFixed)3.14m;
        Span<char> buffer = stackalloc char[100];

        var success = value.TryFormat(buffer, out var charsWritten, "F3", CultureInfo.InvariantCulture);

        success.Should().BeTrue();
        buffer[..charsWritten].ToString().Should().Be("3.140");
    }

    [Fact]
    public void TryFormat_WithCulture_RespectsDecimalSeparator()
    {
        var value = (ChiFixed)123.45m;
        var germanCulture = new CultureInfo("de-DE");
        Span<char> buffer = stackalloc char[100];

        var success = value.TryFormat(buffer, out var charsWritten, "F2", germanCulture);

        success.Should().BeTrue();
        buffer[..charsWritten].ToString().Should().Be("123,45");
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

        result.Should().Be(expected);
    }

    [Fact]
    public void ToString_FormatG_MatchesDefaultToString()
    {
        var value = (ChiFixed)42.5m;

        var withFormat = value.ToString("G", CultureInfo.InvariantCulture);
        var withoutFormat = value.ToString();

        withFormat.Should().Be(withoutFormat);
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

            success.Should().BeTrue();
            result.Should().Be("123.45");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}