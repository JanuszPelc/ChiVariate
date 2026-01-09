using System.Globalization;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Conversion;

public class ParserTests
{
    [Fact]
    public void Parse_Zero_ReturnsChiFixedZero()
    {
        var result = ChiFixed.Parse("0");

        Assert.Equal(ChiFixed.Zero, result);
    }

    [Fact]
    public void Parse_One_ReturnsChiFixedOne()
    {
        var result = ChiFixed.Parse("1");

        Assert.Equal(ChiFixed.One, result);
    }

    [Theory]
    [InlineData("0.5")]
    [InlineData("0.25")]
    [InlineData("123.456")]
    [InlineData("-42.5")]
    [InlineData("+3.14159")]
    public void Parse_ValidDecimalString_ReturnsCorrectValue(string input)
    {
        var expected = (ChiFixed)decimal.Parse(input, CultureInfo.InvariantCulture);

        var result = ChiFixed.Parse(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Parse_NegativeValue_ReturnsNegativeChiFixed()
    {
        var result = ChiFixed.Parse("-10.5");

        Assert.True(result < ChiFixed.Zero);
        Assert.Equal((ChiFixed)(-10.5m), result);
    }

    [Fact]
    public void Parse_HighPrecisionString_MatchesDecimalConversion()
    {
        const string input = "0.123456789012345678901234567890";

        var fromParse = ChiFixed.Parse(input);
        var fromDecimal = (ChiFixed)decimal.Parse(input, CultureInfo.InvariantCulture);

        Assert.Equal(fromDecimal, fromParse);
    }

    [Fact]
    public void Parse_EmptyString_ThrowsFormatException()
    {
        const string emptyString = "";

        Assert.Throws<FormatException>(() => ChiFixed.Parse(emptyString));
    }

    [Fact]
    public void Parse_OnlyDecimalPoint_ThrowsFormatException()
    {
        const string onlyDecimalPoint = ".";

        Assert.Throws<FormatException>(() => ChiFixed.Parse(onlyDecimalPoint));
    }

    [Fact]
    public void Parse_MultipleDecimalPoints_ThrowsFormatException()
    {
        const string multipleDecimals = "1.5.3";

        Assert.Throws<FormatException>(() => ChiFixed.Parse(multipleDecimals));
    }

    [Fact]
    public void Parse_OnlySign_ThrowsFormatException()
    {
        const string onlyMinus = "-";
        const string onlyPlus = "+";

        Assert.Throws<FormatException>(() => ChiFixed.Parse(onlyMinus));
        Assert.Throws<FormatException>(() => ChiFixed.Parse(onlyPlus));
    }

    [Fact]
    public void Parse_InvalidCharacters_ThrowsFormatException()
    {
        const string withLetters = "12.3abc";
        const string withSpaces = "12 .5";
        const string withSymbols = "12@34";

        Assert.Throws<FormatException>(() => ChiFixed.Parse(withLetters));
        Assert.Throws<FormatException>(() => ChiFixed.Parse(withSpaces));
        Assert.Throws<FormatException>(() => ChiFixed.Parse(withSymbols));
    }

    [Fact]
    public void Parse_NoDigitsBeforeDecimal_ParsesAsZeroPointX()
    {
        const string noIntegerPart = ".5";

        var result = ChiFixed.Parse(noIntegerPart);

        Assert.Equal(ChiFixed.Parse("0.5"), result);
    }

    [Fact]
    public void Parse_NoDigitsAfterDecimal_ParsesAsIntegerValue()
    {
        const string noFractionalPart = "42.";

        var result = ChiFixed.Parse(noFractionalPart);

        Assert.Equal(ChiFixed.Parse("42"), result);
    }

    [Fact]
    public void Parse_InvariantCulture_ParsesCorrectly()
    {
        var result = ChiFixed.Parse("123.456", CultureInfo.InvariantCulture);

        Assert.Equal((ChiFixed)123.456m, result);
    }

    [Fact]
    public void Parse_AllowLeadingWhite_ParsesCorrectly()
    {
        var result = ChiFixed.Parse("  42.5", NumberStyles.AllowLeadingWhite | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture);

        Assert.Equal((ChiFixed)42.5m, result);
    }

    [Fact]
    public void Parse_AllowTrailingWhite_ParsesCorrectly()
    {
        var result = ChiFixed.Parse("42.5  ", NumberStyles.AllowTrailingWhite | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture);

        Assert.Equal((ChiFixed)42.5m, result);
    }

    [Fact]
    public void Parse_AllowLeadingSign_ParsesNegative()
    {
        var result = ChiFixed.Parse("-123.456", NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture);

        Assert.Equal((ChiFixed)(-123.456m), result);
    }

    [Fact]
    public void TryParse_ValidInput_ReturnsTrue()
    {
        var success = ChiFixed.TryParse("42.5", CultureInfo.InvariantCulture, out var result);

        Assert.True(success);
        Assert.Equal((ChiFixed)42.5m, result);
    }

    [Fact]
    public void TryParse_InvalidInput_ReturnsFalse()
    {
        var success = ChiFixed.TryParse("not a number", CultureInfo.InvariantCulture, out var result);

        Assert.False(success);
        Assert.Equal(ChiFixed.Zero, result);
    }

    [Fact]
    public void TryParse_NullInput_ReturnsFalse()
    {
        var success = ChiFixed.TryParse(null, CultureInfo.InvariantCulture, out var result);

        Assert.False(success);
        Assert.Equal(ChiFixed.Zero, result);
    }

    [Fact]
    public void TryParse_EmptyInput_ReturnsFalse()
    {
        var success = ChiFixed.TryParse("", CultureInfo.InvariantCulture, out _);

        Assert.False(success);
    }

    [Fact]
    public void TryParse_WithStylesValid_ReturnsTrue()
    {
        var success = ChiFixed.TryParse("  -42.5  ",
            NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite |
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out var result);

        Assert.True(success);
        Assert.Equal((ChiFixed)(-42.5m), result);
    }

    [Fact]
    public void TryParse_SpanValid_ReturnsTrue()
    {
        ReadOnlySpan<char> input = "123.456";
        var success = ChiFixed.TryParse(input, CultureInfo.InvariantCulture, out var result);

        Assert.True(success);
        Assert.Equal((ChiFixed)123.456m, result);
    }

    [Fact]
    public void TryParse_SpanInvalid_ReturnsFalse()
    {
        ReadOnlySpan<char> input = "invalid";
        var success = ChiFixed.TryParse(input, CultureInfo.InvariantCulture, out var result);

        Assert.False(success);
        Assert.Equal(ChiFixed.Zero, result);
    }

    [Fact]
    public void Parse_InvalidInput_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => ChiFixed.Parse("not a number", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void Parse_InvalidInputWithStyles_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() =>
            ChiFixed.Parse("invalid", NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture));
    }

    [Theory]
    [InlineData("0", 0)]
    [InlineData("1", 1)]
    [InlineData("-1", -1)]
    [InlineData("0.5", 0.5)]
    [InlineData("-0.5", -0.5)]
    [InlineData("  123.456  ", 123.456)]
    public void Parse_VariousFormats_ParsesCorrectly(string input, decimal expected)
    {
        var result = ChiFixed.Parse(input, CultureInfo.InvariantCulture);

        Assert.Equal((ChiFixed)expected, result);
    }

    [Fact]
    public void Parse_GermanCulture_CommaAsDecimalSeparator()
    {
        var germanCulture = new CultureInfo("de-DE");
        var result = ChiFixed.Parse("123,456", germanCulture);

        Assert.Equal((ChiFixed)123.456m, result);
    }

    [Fact]
    public void Parse_FrenchCulture_CommaAsDecimalSeparator()
    {
        var frenchCulture = new CultureInfo("fr-FR");
        var result = ChiFixed.Parse("42,5", frenchCulture);

        Assert.Equal((ChiFixed)42.5m, result);
    }

    [Fact]
    public void Parse_USCulture_PeriodAsDecimalSeparator()
    {
        var usCulture = new CultureInfo("en-US");
        var result = ChiFixed.Parse("123.456", usCulture);

        Assert.Equal((ChiFixed)123.456m, result);
    }

    [Fact]
    public void TryParse_GermanCultureValid_ReturnsTrue()
    {
        var germanCulture = new CultureInfo("de-DE");
        var success = ChiFixed.TryParse("3,14", germanCulture, out var result);

        Assert.True(success);
        Assert.Equal((ChiFixed)3.14m, result);
    }

    [Fact]
    public void TryParse_WrongDecimalSeparator_ReturnsFalse()
    {
        var germanCulture = new CultureInfo("de-DE");
        var success = ChiFixed.TryParse("3.14", NumberStyles.AllowDecimalPoint, germanCulture, out _);

        Assert.False(success);
    }

    [Fact]
    public void Parse_CultureSpecific_NegativeNumbers()
    {
        var germanCulture = new CultureInfo("de-DE");
        var result = ChiFixed.Parse("-123,456", germanCulture);

        Assert.Equal((ChiFixed)(-123.456m), result);
    }

    [Fact]
    public void Parse_RoundTrip_AcrossCultures()
    {
        var original = (ChiFixed)12345.6789m;
        var formatted = original.ToString();
        var parsed = ChiFixed.Parse(formatted, CultureInfo.InvariantCulture);

        Assert.Equal(original, parsed);
    }

    [Fact]
    public void Parse_SpanWithProvider_ParsesCorrectly()
    {
        ReadOnlySpan<char> input = "123.456";
        var result = ChiFixed.Parse(input, CultureInfo.InvariantCulture);

        Assert.Equal((ChiFixed)123.456m, result);
    }

    [Fact]
    public void Parse_SpanWithNumberStyles_ParsesCorrectly()
    {
        ReadOnlySpan<char> input = "  -42.5  ";
        var result = ChiFixed.Parse(input,
            NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite |
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture);

        Assert.Equal((ChiFixed)(-42.5m), result);
    }

    [Fact]
    public void TryParse_SpanWithStylesValid_ReturnsTrue()
    {
        ReadOnlySpan<char> input = "  3.14  ";
        var success = ChiFixed.TryParse(input,
            NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out var result);

        Assert.True(success);
        Assert.Equal((ChiFixed)3.14m, result);
    }

    [Fact]
    public void TryParse_SpanWithStylesInvalid_ReturnsFalse()
    {
        ReadOnlySpan<char> input = "invalid";
        var success = ChiFixed.TryParse(input,
            NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out var result);

        Assert.False(success);
        Assert.Equal(ChiFixed.Zero, result);
    }
}