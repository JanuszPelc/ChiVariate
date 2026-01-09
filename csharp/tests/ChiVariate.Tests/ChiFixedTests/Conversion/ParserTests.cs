using System.Globalization;
using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Conversion;

public class ParserTests
{
    [Fact]
    public void Parse_Zero_ReturnsChiFixedZero()
    {
        var result = ChiFixed.Parse("0");

        result.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void Parse_One_ReturnsChiFixedOne()
    {
        var result = ChiFixed.Parse("1");

        result.Should().Be(ChiFixed.One);
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

        result.Should().Be(expected);
    }

    [Fact]
    public void Parse_NegativeValue_ReturnsNegativeChiFixed()
    {
        var result = ChiFixed.Parse("-10.5");

        (result < ChiFixed.Zero).Should().BeTrue();
        result.Should().Be((ChiFixed)(-10.5m));
    }

    [Fact]
    public void Parse_HighPrecisionString_MatchesDecimalConversion()
    {
        const string input = "0.123456789012345678901234567890";

        var fromParse = ChiFixed.Parse(input);
        var fromDecimal = (ChiFixed)decimal.Parse(input, CultureInfo.InvariantCulture);

        fromParse.Should().Be(fromDecimal);
    }

    [Fact]
    public void Parse_EmptyString_ThrowsFormatException()
    {
        const string emptyString = "";

        var act = () => ChiFixed.Parse(emptyString);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_OnlyDecimalPoint_ThrowsFormatException()
    {
        const string onlyDecimalPoint = ".";

        var act = () => ChiFixed.Parse(onlyDecimalPoint);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_MultipleDecimalPoints_ThrowsFormatException()
    {
        const string multipleDecimals = "1.5.3";

        var act = () => ChiFixed.Parse(multipleDecimals);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_OnlySign_ThrowsFormatException()
    {
        const string onlyMinus = "-";
        const string onlyPlus = "+";

        var actMinus = () => ChiFixed.Parse(onlyMinus);
        actMinus.Should().Throw<FormatException>();
        var actPlus = () => ChiFixed.Parse(onlyPlus);
        actPlus.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_InvalidCharacters_ThrowsFormatException()
    {
        const string withLetters = "12.3abc";
        const string withSpaces = "12 .5";
        const string withSymbols = "12@34";

        var actLetters = () => ChiFixed.Parse(withLetters);
        actLetters.Should().Throw<FormatException>();
        var actSpaces = () => ChiFixed.Parse(withSpaces);
        actSpaces.Should().Throw<FormatException>();
        var actSymbols = () => ChiFixed.Parse(withSymbols);
        actSymbols.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_NoDigitsBeforeDecimal_ParsesAsZeroPointX()
    {
        const string noIntegerPart = ".5";

        var result = ChiFixed.Parse(noIntegerPart);

        result.Should().Be(ChiFixed.Parse("0.5"));
    }

    [Fact]
    public void Parse_NoDigitsAfterDecimal_ParsesAsIntegerValue()
    {
        const string noFractionalPart = "42.";

        var result = ChiFixed.Parse(noFractionalPart);

        result.Should().Be(ChiFixed.Parse("42"));
    }

    [Fact]
    public void Parse_InvariantCulture_ParsesCorrectly()
    {
        var result = ChiFixed.Parse("123.456", CultureInfo.InvariantCulture);

        result.Should().Be((ChiFixed)123.456m);
    }

    [Fact]
    public void Parse_AllowLeadingWhite_ParsesCorrectly()
    {
        var result = ChiFixed.Parse("  42.5", NumberStyles.AllowLeadingWhite | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture);

        result.Should().Be((ChiFixed)42.5m);
    }

    [Fact]
    public void Parse_AllowTrailingWhite_ParsesCorrectly()
    {
        var result = ChiFixed.Parse("42.5  ", NumberStyles.AllowTrailingWhite | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture);

        result.Should().Be((ChiFixed)42.5m);
    }

    [Fact]
    public void Parse_AllowLeadingSign_ParsesNegative()
    {
        var result = ChiFixed.Parse("-123.456", NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture);

        result.Should().Be((ChiFixed)(-123.456m));
    }

    [Fact]
    public void TryParse_ValidInput_ReturnsTrue()
    {
        var success = ChiFixed.TryParse("42.5", CultureInfo.InvariantCulture, out var result);

        success.Should().BeTrue();
        result.Should().Be((ChiFixed)42.5m);
    }

    [Fact]
    public void TryParse_InvalidInput_ReturnsFalse()
    {
        var success = ChiFixed.TryParse("not a number", CultureInfo.InvariantCulture, out var result);

        success.Should().BeFalse();
        result.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void TryParse_NullInput_ReturnsFalse()
    {
        var success = ChiFixed.TryParse(null, CultureInfo.InvariantCulture, out var result);

        success.Should().BeFalse();
        result.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void TryParse_EmptyInput_ReturnsFalse()
    {
        var success = ChiFixed.TryParse("", CultureInfo.InvariantCulture, out _);

        success.Should().BeFalse();
    }

    [Fact]
    public void TryParse_WithStylesValid_ReturnsTrue()
    {
        var success = ChiFixed.TryParse("  -42.5  ",
            NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite |
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out var result);

        success.Should().BeTrue();
        result.Should().Be((ChiFixed)(-42.5m));
    }

    [Fact]
    public void TryParse_SpanValid_ReturnsTrue()
    {
        ReadOnlySpan<char> input = "123.456";
        var success = ChiFixed.TryParse(input, CultureInfo.InvariantCulture, out var result);

        success.Should().BeTrue();
        result.Should().Be((ChiFixed)123.456m);
    }

    [Fact]
    public void TryParse_SpanInvalid_ReturnsFalse()
    {
        ReadOnlySpan<char> input = "invalid";
        var success = ChiFixed.TryParse(input, CultureInfo.InvariantCulture, out var result);

        success.Should().BeFalse();
        result.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void Parse_InvalidInput_ThrowsFormatException()
    {
        var act = () => ChiFixed.Parse("not a number", CultureInfo.InvariantCulture);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_InvalidInputWithStyles_ThrowsFormatException()
    {
        var act = () =>
            ChiFixed.Parse("invalid", NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
        act.Should().Throw<FormatException>();
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

        result.Should().Be((ChiFixed)expected);
    }

    [Fact]
    public void Parse_GermanCulture_CommaAsDecimalSeparator()
    {
        var germanCulture = new CultureInfo("de-DE");
        var result = ChiFixed.Parse("123,456", germanCulture);

        result.Should().Be((ChiFixed)123.456m);
    }

    [Fact]
    public void Parse_FrenchCulture_CommaAsDecimalSeparator()
    {
        var frenchCulture = new CultureInfo("fr-FR");
        var result = ChiFixed.Parse("42,5", frenchCulture);

        result.Should().Be((ChiFixed)42.5m);
    }

    [Fact]
    public void Parse_USCulture_PeriodAsDecimalSeparator()
    {
        var usCulture = new CultureInfo("en-US");
        var result = ChiFixed.Parse("123.456", usCulture);

        result.Should().Be((ChiFixed)123.456m);
    }

    [Fact]
    public void TryParse_GermanCultureValid_ReturnsTrue()
    {
        var germanCulture = new CultureInfo("de-DE");
        var success = ChiFixed.TryParse("3,14", germanCulture, out var result);

        success.Should().BeTrue();
        result.Should().Be((ChiFixed)3.14m);
    }

    [Fact]
    public void TryParse_WrongDecimalSeparator_ReturnsFalse()
    {
        var germanCulture = new CultureInfo("de-DE");
        var success = ChiFixed.TryParse("3.14", NumberStyles.AllowDecimalPoint, germanCulture, out _);

        success.Should().BeFalse();
    }

    [Fact]
    public void Parse_CultureSpecific_NegativeNumbers()
    {
        var germanCulture = new CultureInfo("de-DE");
        var result = ChiFixed.Parse("-123,456", germanCulture);

        result.Should().Be((ChiFixed)(-123.456m));
    }

    [Fact]
    public void Parse_RoundTrip_AcrossCultures()
    {
        var original = (ChiFixed)12345.6789m;
        var formatted = original.ToString();
        var parsed = ChiFixed.Parse(formatted, CultureInfo.InvariantCulture);

        parsed.Should().Be(original);
    }

    [Fact]
    public void Parse_SpanWithProvider_ParsesCorrectly()
    {
        ReadOnlySpan<char> input = "123.456";
        var result = ChiFixed.Parse(input, CultureInfo.InvariantCulture);

        result.Should().Be((ChiFixed)123.456m);
    }

    [Fact]
    public void Parse_SpanWithNumberStyles_ParsesCorrectly()
    {
        ReadOnlySpan<char> input = "  -42.5  ";
        var result = ChiFixed.Parse(input,
            NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite |
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture);

        result.Should().Be((ChiFixed)(-42.5m));
    }

    [Fact]
    public void TryParse_SpanWithStylesValid_ReturnsTrue()
    {
        ReadOnlySpan<char> input = "  3.14  ";
        var success = ChiFixed.TryParse(input,
            NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out var result);

        success.Should().BeTrue();
        result.Should().Be((ChiFixed)3.14m);
    }

    [Fact]
    public void TryParse_SpanWithStylesInvalid_ReturnsFalse()
    {
        ReadOnlySpan<char> input = "invalid";
        var success = ChiFixed.TryParse(input,
            NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out var result);

        success.Should().BeFalse();
        result.Should().Be(ChiFixed.Zero);
    }
}