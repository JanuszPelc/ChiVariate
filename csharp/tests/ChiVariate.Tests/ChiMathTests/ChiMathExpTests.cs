using System.Globalization;
using AwesomeAssertions;
using Xunit;

namespace ChiVariate.Tests.ChiMathTests;

#pragma warning disable CS1591
public class ChiMathExpTests
{
    #region Float Tests

    [Theory]
    [InlineData(0.0f, 1.0f)]
    [InlineData(1.0f, 2.718282f)]
    [InlineData(-1.0f, 0.367879f)]
    [InlineData(2.0f, 7.389056f)]
    [InlineData(0.693147f, 2.0f)]
    public void Exp_Float_ReturnsExpectedResults(float input, float expected)
    {
        var result = ChiMath.Exp(input);

        result.Should().BeApproximately(expected, 1e-4f);
    }

    #endregion

    #region Consistency Tests

    [Theory]
    [InlineData(0.0)]
    [InlineData(1.0)]
    [InlineData(-1.0)]
    [InlineData(2.0)]
    [InlineData(0.5)]
    public void Exp_AllTypes_ReturnsConsistentResults(double value)
    {
        var doubleResult = ChiMath.Exp(value);
        var floatResult = ChiMath.Exp((float)value);
        var decimalResult = ChiMath.Exp((decimal)value);
        var fixedResult = ChiMath.Exp((ChiFixed)(decimal)value);

        ((double)decimalResult).Should().BeApproximately(doubleResult, 1e-10);
        floatResult.Should().BeApproximately((float)doubleResult, 1e-5f);
        ((double)(decimal)fixedResult).Should().BeApproximately(doubleResult, 1e-2);
    }

    #endregion

    #region Double Tests

    [Theory]
    [InlineData(0.0, 1.0)]
    [InlineData(1.0, 2.718281828459045)]
    [InlineData(-1.0, 0.36787944117144233)]
    [InlineData(2.0, 7.38905609893065)]
    [InlineData(-2.0, 0.1353352832366127)]
    [InlineData(0.6931471805599453, 2.0)]
    [InlineData(2.302585092994046, 10.0)]
    public void Exp_Double_ReturnsExpectedResults(double input, double expected)
    {
        var result = ChiMath.Exp(input);

        result.Should().BeApproximately(expected, 1e-10);
    }

    [Fact]
    public void Exp_DoubleZero_ReturnsOne()
    {
        var result = ChiMath.Exp(0.0);

        result.Should().Be(1.0);
    }

    [Theory]
    [InlineData(0.001)]
    [InlineData(0.01)]
    [InlineData(0.1)]
    [InlineData(-0.001)]
    [InlineData(-0.01)]
    [InlineData(-0.1)]
    public void Exp_DoubleSmallValues_MatchesSystemMath(double input)
    {
        var result = ChiMath.Exp(input);

        var expected = Math.Exp(input);
        result.Should().BeApproximately(expected, 1e-14);
    }

    [Theory]
    [InlineData(10.0)]
    [InlineData(20.0)]
    [InlineData(50.0)]
    public void Exp_DoubleLargePositive_ReturnsFiniteResult(double input)
    {
        var result = ChiMath.Exp(input);

        result.Should().BePositive();
        double.IsFinite(result).Should().BeTrue();
    }

    [Theory]
    [InlineData(-10.0)]
    [InlineData(-20.0)]
    [InlineData(-50.0)]
    public void Exp_DoubleLargeNegative_ReturnsSmallPositive(double input)
    {
        var result = ChiMath.Exp(input);

        result.Should().BePositive();
        result.Should().BeLessThan(1.0);
    }

    #endregion

    #region Decimal Tests

    [Theory]
    [InlineData("0", "1")]
    [InlineData("1", "2.71828182845904523536028747135266249775724709369995957496697")]
    [InlineData("-1", "0.367879441171442321595523770161460867445811131031767834507837")]
    [InlineData("2", "7.38905609893065022723042746057500781318031557055184732408713")]
    [InlineData("0.693147180559945309417232121458176568075500134360255254120680", "2")]
    [InlineData("2.302585092994045684017991454684364207601101488628772976033", "10")]
    public void Exp_Decimal_ReturnsExpectedResults(string inputStr, string expectedStr)
    {
        var input = decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Exp(input);

        result.Should().BeApproximately(expected, 1e-25m);
    }

    [Fact]
    public void Exp_DecimalZero_ReturnsExactlyOne()
    {
        var result = ChiMath.Exp(0m);

        result.Should().Be(1m);
    }

    [Theory]
    [InlineData("0.001")]
    [InlineData("0.01")]
    [InlineData("0.1")]
    [InlineData("-0.001")]
    [InlineData("-0.01")]
    [InlineData("-0.1")]
    public void Exp_DecimalSmallValues_ReturnsPositiveResult(string inputStr)
    {
        var input = decimal.Parse(inputStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Exp(input);

        result.Should().BePositive();
        if (input > 0)
            result.Should().BeGreaterThan(1m);
        else
            result.Should().BeLessThan(1m);
    }

    [Theory]
    [InlineData("-10")]
    [InlineData("-20")]
    [InlineData("-30")]
    public void Exp_DecimalLargeNegative_ReturnsSmallPositive(string inputStr)
    {
        var input = decimal.Parse(inputStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Exp(input);

        result.Should().BePositive();
        result.Should().BeLessThan(1m);
    }

    [Theory]
    [InlineData("50")]
    [InlineData("60")]
    public void Exp_DecimalVeryLarge_ThrowsOverflowException(string inputStr)
    {
        var input = decimal.Parse(inputStr, CultureInfo.InvariantCulture);

        var act = () => ChiMath.Exp(input);

        act.Should().Throw<OverflowException>();
    }

    #endregion

    #region ChiFixed Tests

    [Theory]
    [InlineData("0", "1")]
    [InlineData("1", "2.718281828459045")]
    [InlineData("-1", "0.367879441171442")]
    [InlineData("2", "7.38905609893065")]
    [InlineData("0.693147180559945", "2")]
    public void Exp_ChiFixed_ReturnsExpectedResults(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Exp(input);

        ((decimal)result).Should().BeApproximately((decimal)expected, 1e-2m);
    }

    [Fact]
    public void Exp_ChiFixedZero_ReturnsOne()
    {
        var result = ChiMath.Exp(ChiFixed.Zero);

        result.Should().Be(ChiFixed.One);
    }

    [Theory]
    [InlineData("0.5")]
    [InlineData("1.5")]
    [InlineData("-0.5")]
    [InlineData("-1.5")]
    public void Exp_ChiFixedMixedSignValues_ReturnsPositiveResult(string inputStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Exp(input);

        result.Should().BeGreaterThan(ChiFixed.Zero);
    }

    [Theory]
    [InlineData("-5")]
    [InlineData("-10")]
    public void Exp_ChiFixedLargeNegative_ReturnsSmallPositive(string inputStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Exp(input);

        result.Should().BeGreaterThan(ChiFixed.Zero);
        result.Should().BeLessThan(ChiFixed.One);
    }

    #endregion

    #region Mathematical Properties Tests

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("0.5")]
    [InlineData("-1")]
    [InlineData("-0.5")]
    public void Exp_LogExpRoundTrip_ReturnsOriginalValue(string inputStr)
    {
        var input = decimal.Parse(inputStr, CultureInfo.InvariantCulture);

        var expResult = ChiMath.Exp(input);
        var roundTrip = ChiMath.Log(expResult);

        roundTrip.Should().BeApproximately(input, 1e-25m);
    }

    [Theory]
    [InlineData("1", "2")]
    [InlineData("0.5", "1.5")]
    [InlineData("-1", "1")]
    public void Exp_ProductRule_ExpSumEqualsProduct(string aStr, string bStr)
    {
        var a = decimal.Parse(aStr, CultureInfo.InvariantCulture);
        var b = decimal.Parse(bStr, CultureInfo.InvariantCulture);

        var expSum = ChiMath.Exp(a + b);
        var expProduct = ChiMath.Exp(a) * ChiMath.Exp(b);

        expSum.Should().BeApproximately(expProduct, 1e-20m);
    }

    [Theory]
    [InlineData("2", "3")]
    [InlineData("1.5", "2")]
    [InlineData("0.5", "4")]
    public void Exp_PowerRule_ExpMultipleEqualsPower(string baseStr, string nStr)
    {
        var baseVal = decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var n = decimal.Parse(nStr, CultureInfo.InvariantCulture);

        var expMultiple = ChiMath.Exp(n * baseVal);
        var expPower = ChiMath.Pow(ChiMath.Exp(baseVal), n);

        expMultiple.Should().BeApproximately(expPower, 1e-15m);
    }

    #endregion

    #region Edge Cases

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Exp_DoubleSpecialValues_DoesNotThrow(double input)
    {
        var act = () => ChiMath.Exp(input);

        act.Should().NotThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(float.NaN)]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity)]
    public void Exp_FloatSpecialValues_DoesNotThrow(float input)
    {
        var act = () => ChiMath.Exp(input);

        act.Should().NotThrow<ArgumentException>();
    }

    #endregion
}