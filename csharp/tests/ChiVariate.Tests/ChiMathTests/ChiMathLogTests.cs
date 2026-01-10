using System.Globalization;
using AwesomeAssertions;
using Xunit;

namespace ChiVariate.Tests.ChiMathTests;

#pragma warning disable CS1591
public class ChiMathLogTests
{
    #region Float Tests

    [Theory]
    [InlineData(1.0f, 0.0f)]
    [InlineData(2.71828f, 1.0f)]
    [InlineData(2.0f, 0.693147f)]
    [InlineData(10.0f, 2.302585f)]
    [InlineData(7.389056f, 2.0f)]
    [InlineData(0.5f, -0.693147f)]
    public void Log_FloatBasicCases_ReturnsExpectedResults(float input, float expected)
    {
        var result = ChiMath.Log(input);

        result.Should().BeApproximately(expected, 1e-5f);
    }

    #endregion

    #region Optimized Path Tests

    [Fact]
    public void Log_OptimizedPaths_ReturnsExact()
    {
        ChiMath.Log(1.0).Should().Be(0.0);
        ChiMath.Log(1.0f).Should().Be(0.0f);
        ChiMath.Log(1m).Should().Be(0m);

        ChiMath.Log(Math.E).Should().Be(1.0);
        ChiMath.Log((float)Math.E).Should().BeApproximately(1.0f, 1e-6f);
        ChiMath.Log(ChiMath.Const<decimal>.E).Should().BeApproximately(1m, 1e-25m);

        var expectedLn2Double = Math.Log(2.0);
        ChiMath.Log(2.0).Should().Be(expectedLn2Double);
        ChiMath.Log(2.0f).Should().BeApproximately((float)expectedLn2Double, 1e-6f);
        ChiMath.Log(2m).Should().BeApproximately(ChiMath.Const<decimal>.Ln2, 1e-25m);

        var expectedLn10Double = Math.Log(10.0);
        ChiMath.Log(10.0).Should().Be(expectedLn10Double);
        ChiMath.Log(10.0f).Should().BeApproximately((float)expectedLn10Double, 1e-6f);
        ChiMath.Log(10m).Should().BeApproximately(ChiMath.Const<decimal>.Ln10, 1e-25m);
    }

    #endregion

    #region Consistency Tests

    [Theory]
    [InlineData(1.0)]
    [InlineData(2.0)]
    [InlineData(10.0)]
    [InlineData(0.5)]
    [InlineData(100.0)]
    public void Log_AllTypes_ReturnsConsistentResults(double value)
    {
        var doubleResult = ChiMath.Log(value);
        var floatResult = ChiMath.Log((float)value);
        var decimalResult = ChiMath.Log((decimal)value);
        var fixedResult = ChiMath.Log((ChiFixed)(decimal)value);

        ((double)decimalResult).Should().BeApproximately(doubleResult, 1e-12);
        floatResult.Should().BeApproximately((float)doubleResult, 1e-5f);
        ((double)(decimal)fixedResult).Should().BeApproximately(doubleResult, 1e-8);
    }

    #endregion

    #region Double Tests

    [Theory]
    [InlineData(1.0, 0.0)]
    [InlineData(2.718281828459045, 1.0)]
    [InlineData(2.0, 0.6931471805599453)]
    [InlineData(10.0, 2.302585092994046)]
    [InlineData(7.38905609893065, 2.0)]
    [InlineData(0.36787944117144233, -1.0)]
    [InlineData(0.5, -0.6931471805599453)]
    [InlineData(100.0, 4.605170185988092)]
    public void Log_DoubleBasicCases_ReturnsExpectedResults(double input, double expected)
    {
        var result = ChiMath.Log(input);

        result.Should().BeApproximately(expected, 1e-14);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(-10.0)]
    public void Log_DoubleNonPositive_ThrowsArgumentException(double input)
    {
        var act = () => ChiMath.Log(input);

        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Decimal Tests

    [Theory]
    [InlineData("1", "0")]
    [InlineData("2.71828182845904523536028747135266249775724709369995957496697", "1")]
    [InlineData("2", "0.693147180559945309417232121458176568075500134360255254120680")]
    [InlineData("10", "2.302585092994045684017991454684364207601101488628772976033")]
    [InlineData("0.5", "-0.693147180559945309417232121458176568075500134360255254120680")]
    [InlineData("100", "4.605170185988091368035982909368728415202202977257545952066")]
    [InlineData("4", "1.386294361119890618834464242916353136151000268720510508241360")]
    public void Log_DecimalBasicCases_ReturnsExpectedResults(string inputStr, string expectedStr)
    {
        var input = decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Log(input);

        result.Should().BeApproximately(expected, 1e-27m);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("-10")]
    [InlineData("-0.5")]
    public void Log_DecimalNonPositive_ThrowsArgumentException(string inputStr)
    {
        var input = decimal.Parse(inputStr, CultureInfo.InvariantCulture);

        var act = () => ChiMath.Log(input);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("1.5")]
    [InlineData("3")]
    [InlineData("5")]
    [InlineData("7")]
    [InlineData("20")]
    [InlineData("50")]
    [InlineData("0.1")]
    [InlineData("0.25")]
    [InlineData("0.75")]
    public void Log_DecimalArbitrary_ExpLogRoundTrips(string inputStr)
    {
        var input = decimal.Parse(inputStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Log(input);

        if (input > 1m)
            result.Should().BePositive();
        else if (input is < 1m and > 0m)
            result.Should().BeNegative();

        var roundTrip = ChiMath.Exp(result);
        roundTrip.Should().BeApproximately(input, 1e-25m);
    }

    [Theory]
    [InlineData("0.0000000000000000000000001")]
    [InlineData("999999999999999999999999")]
    public void Log_DecimalExtremeValues_DoesNotThrow(string inputStr)
    {
        var input = decimal.Parse(inputStr, CultureInfo.InvariantCulture);

        var act = () => ChiMath.Log(input);

        act.Should().NotThrow<OverflowException>();
    }

    #endregion

    #region ChiFixed Tests

    [Theory]
    [InlineData("1", "0")]
    [InlineData("2", "0.693147180559945")]
    [InlineData("10", "2.302585092994046")]
    [InlineData("0.5", "-0.693147180559945")]
    [InlineData("100", "4.605170185988092")]
    public void Log_ChiFixedBasicCases_ReturnsExpectedResults(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Log(input);

        ((decimal)result).Should().BeApproximately((decimal)expected, 1e-8m);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("-10")]
    public void Log_ChiFixedNonPositive_ThrowsArgumentException(string inputStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);

        var act = () => ChiMath.Log(input);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("1.5")]
    [InlineData("3")]
    [InlineData("5")]
    [InlineData("7")]
    [InlineData("20")]
    [InlineData("0.25")]
    [InlineData("0.75")]
    public void Log_ChiFixedArbitrary_ExpLogRoundTrips(string inputStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Log(input);

        if (input > ChiFixed.One)
            result.Should().BeGreaterThan(ChiFixed.Zero);
        else if (input < ChiFixed.One && input > ChiFixed.Zero)
            result.Should().BeLessThan(ChiFixed.Zero);

        var roundTrip = ChiMath.Exp(result);
        var relativeTolerance = (decimal)input * 0.001m;
        ((decimal)roundTrip).Should().BeApproximately((decimal)input, relativeTolerance);
    }

    #endregion

    #region Mathematical Properties Tests

    [Theory]
    [InlineData("2", "3")]
    [InlineData("5", "7")]
    [InlineData("1.5", "2.5")]
    public void Log_ProductRule_SumsLogs(string aStr, string bStr)
    {
        var a = decimal.Parse(aStr, CultureInfo.InvariantCulture);
        var b = decimal.Parse(bStr, CultureInfo.InvariantCulture);
        var product = a * b;

        var logProduct = ChiMath.Log(product);
        var logA = ChiMath.Log(a);
        var logB = ChiMath.Log(b);
        var sumOfLogs = logA + logB;

        logProduct.Should().BeApproximately(sumOfLogs, 1e-25m);
    }

    [Theory]
    [InlineData("8", "2")]
    [InlineData("15", "3")]
    [InlineData("7.5", "2.5")]
    public void Log_QuotientRule_SubtractsLogs(string aStr, string bStr)
    {
        var a = decimal.Parse(aStr, CultureInfo.InvariantCulture);
        var b = decimal.Parse(bStr, CultureInfo.InvariantCulture);
        var quotient = a / b;

        var logQuotient = ChiMath.Log(quotient);
        var logA = ChiMath.Log(a);
        var logB = ChiMath.Log(b);
        var differenceOfLogs = logA - logB;

        logQuotient.Should().BeApproximately(differenceOfLogs, 1e-25m);
    }

    [Theory]
    [InlineData("2", "3")]
    [InlineData("5", "2")]
    [InlineData("1.5", "4")]
    public void Log_PowerRule_ScalesLog(string baseStr, string exponentStr)
    {
        var baseVal = decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var exponent = decimal.Parse(exponentStr, CultureInfo.InvariantCulture);
        var power = ChiMath.Pow(baseVal, exponent);

        var logPower = ChiMath.Log(power);
        var logBase = ChiMath.Log(baseVal);
        var scaledLog = exponent * logBase;

        logPower.Should().BeApproximately(scaledLog, 1e-20m);
    }

    #endregion

    #region Edge Cases

    [Theory]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NaN)]
    public void Log_DoubleSpecialValues_DoesNotThrow(double input)
    {
        var act = () => ChiMath.Log(input);

        act.Should().NotThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NaN)]
    public void Log_FloatSpecialValues_DoesNotThrow(float input)
    {
        var act = () => ChiMath.Log(input);

        act.Should().NotThrow<ArgumentException>();
    }

    #endregion
}