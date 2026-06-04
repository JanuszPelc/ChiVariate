using System.Globalization;
using AwesomeAssertions;
using Xunit;

namespace ChiVariate.Tests.ChiMathTests;

#pragma warning disable CS1591
public class ChiMathCbrtTests
{
    #region Consistency Tests

    [Theory]
    [InlineData(8.0)]
    [InlineData(27.0)]
    [InlineData(2.0)]
    [InlineData(0.5)]
    [InlineData(-8.0)]
    public void Cbrt_AllTypes_ReturnsConsistentResults(double value)
    {
        var doubleResult = ChiMath.Cbrt(value);
        var floatResult = ChiMath.Cbrt((float)value);
        var decimalResult = ChiMath.Cbrt((decimal)value);
        var fixedResult = ChiMath.Cbrt((ChiFixed)(decimal)value);

        ((double)decimalResult).Should().BeApproximately(doubleResult, 1e-10);
        floatResult.Should().BeApproximately((float)doubleResult, 1e-5f);
        ((double)(decimal)fixedResult).Should().BeApproximately(doubleResult, 1e-2);
    }

    #endregion

    #region Float Tests

    [Theory]
    [InlineData(8.0f, 2.0f)]
    [InlineData(27.0f, 3.0f)]
    [InlineData(64.0f, 4.0f)]
    [InlineData(125.0f, 5.0f)]
    [InlineData(1.0f, 1.0f)]
    [InlineData(0.125f, 0.5f)]
    public void Cbrt_FloatPerfectCubes_ReturnsExactResults(float input, float expected)
    {
        var result = ChiMath.Cbrt(input);

        result.Should().BeApproximately(expected, 1e-5f);
    }

    [Theory]
    [InlineData(-8.0f, -2.0f)]
    [InlineData(-27.0f, -3.0f)]
    [InlineData(-1.0f, -1.0f)]
    public void Cbrt_FloatNegative_ReturnsNegativeResult(float input, float expected)
    {
        var result = ChiMath.Cbrt(input);

        result.Should().BeApproximately(expected, 1e-5f);
    }

    #endregion

    #region Double Tests

    [Theory]
    [InlineData(8.0, 2.0)]
    [InlineData(27.0, 3.0)]
    [InlineData(64.0, 4.0)]
    [InlineData(125.0, 5.0)]
    [InlineData(1000.0, 10.0)]
    [InlineData(1.0, 1.0)]
    [InlineData(0.125, 0.5)]
    [InlineData(0.001, 0.1)]
    public void Cbrt_DoublePerfectCubes_ReturnsExactResults(double input, double expected)
    {
        var result = ChiMath.Cbrt(input);

        result.Should().BeApproximately(expected, 1e-10);
    }

    [Theory]
    [InlineData(-8.0, -2.0)]
    [InlineData(-27.0, -3.0)]
    [InlineData(-64.0, -4.0)]
    [InlineData(-1.0, -1.0)]
    [InlineData(-0.125, -0.5)]
    public void Cbrt_DoubleNegative_ReturnsNegativeResult(double input, double expected)
    {
        var result = ChiMath.Cbrt(input);

        result.Should().BeApproximately(expected, 1e-10);
    }

    [Theory]
    [InlineData(2.0)]
    [InlineData(3.0)]
    [InlineData(5.0)]
    [InlineData(10.0)]
    [InlineData(100.0)]
    public void Cbrt_DoubleIrrational_CubedEqualsInput(double input)
    {
        var result = ChiMath.Cbrt(input);

        var verification = result * result * result;
        verification.Should().BeApproximately(input, 1e-10);
    }

    [Fact]
    public void Cbrt_DoubleZero_ReturnsZero()
    {
        var result = ChiMath.Cbrt(0.0);

        result.Should().Be(0.0);
    }

    [Fact]
    public void Cbrt_DoubleOne_ReturnsOne()
    {
        var result = ChiMath.Cbrt(1.0);

        result.Should().Be(1.0);
    }

    #endregion

    #region Decimal Tests

    [Theory]
    [InlineData("8", "2")]
    [InlineData("27", "3")]
    [InlineData("64", "4")]
    [InlineData("125", "5")]
    [InlineData("1000", "10")]
    [InlineData("1", "1")]
    [InlineData("0.125", "0.5")]
    [InlineData("0.001", "0.1")]
    public void Cbrt_DecimalPerfectCubes_ReturnsExactResults(string inputStr, string expectedStr)
    {
        var input = decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Cbrt(input);

        result.Should().BeApproximately(expected, 1e-10m);
    }

    [Theory]
    [InlineData("-8", "-2")]
    [InlineData("-27", "-3")]
    [InlineData("-64", "-4")]
    [InlineData("-1", "-1")]
    [InlineData("-0.125", "-0.5")]
    public void Cbrt_DecimalNegative_ReturnsNegativeResult(string inputStr, string expectedStr)
    {
        var input = decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Cbrt(input);

        result.Should().BeApproximately(expected, 1e-10m);
    }

    [Theory]
    [InlineData("2")]
    [InlineData("3")]
    [InlineData("5")]
    [InlineData("10")]
    [InlineData("0.5")]
    public void Cbrt_DecimalIrrational_CubedEqualsInput(string inputStr)
    {
        var input = decimal.Parse(inputStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Cbrt(input);

        var verification = result * result * result;
        verification.Should().BeApproximately(input, 1e-8m);
    }

    [Fact]
    public void Cbrt_DecimalZero_ReturnsZero()
    {
        var result = ChiMath.Cbrt(0m);

        result.Should().Be(0m);
    }

    [Theory]
    [InlineData("0.000001")]
    [InlineData("1000000")]
    public void Cbrt_DecimalExtremeValues_DoesNotThrow(string inputStr)
    {
        var input = decimal.Parse(inputStr, CultureInfo.InvariantCulture);

        var act = () => ChiMath.Cbrt(input);

        act.Should().NotThrow();
    }

    #endregion

    #region ChiFixed Tests

    [Theory]
    [InlineData("8", "2")]
    [InlineData("27", "3")]
    [InlineData("64", "4")]
    [InlineData("125", "5")]
    [InlineData("1", "1")]
    [InlineData("0.125", "0.5")]
    public void Cbrt_ChiFixedPerfectCubes_ReturnsExactResults(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Cbrt(input);

        ((decimal)result).Should().BeApproximately((decimal)expected, 1e-3m);
    }

    [Theory]
    [InlineData("-8", "-2")]
    [InlineData("-27", "-3")]
    [InlineData("-1", "-1")]
    public void Cbrt_ChiFixedNegative_ReturnsNegativeResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Cbrt(input);

        ((decimal)result).Should().BeApproximately((decimal)expected, 1e-3m);
    }

    [Theory]
    [InlineData("2")]
    [InlineData("3")]
    [InlineData("5")]
    [InlineData("10")]
    [InlineData("0.5")]
    public void Cbrt_ChiFixedIrrational_CubedEqualsInput(string inputStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Cbrt(input);

        var verification = result * result * result;
        ((decimal)verification).Should().BeApproximately((decimal)input, 1e-2m);
    }

    [Fact]
    public void Cbrt_ChiFixedZero_ReturnsZero()
    {
        var result = ChiMath.Cbrt(ChiFixed.Zero);

        result.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void Cbrt_ChiFixedOne_ReturnsOne()
    {
        var result = ChiMath.Cbrt(ChiFixed.One);

        result.Should().Be(ChiFixed.One);
    }

    #endregion

    #region Mathematical Properties Tests

    [Theory]
    [InlineData("2")]
    [InlineData("3")]
    [InlineData("5")]
    [InlineData("0.5")]
    public void Cbrt_ComparedToPowOneThird_MatchesResult(string inputStr)
    {
        var input = decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var oneThird = 1m / 3m;

        var cbrtResult = ChiMath.Cbrt(input);
        var powResult = ChiMath.Pow(input, oneThird);

        cbrtResult.Should().BeApproximately(powResult, 1e-10m);
    }

    [Theory]
    [InlineData("2", "3")]
    [InlineData("5", "7")]
    [InlineData("0.5", "2")]
    public void Cbrt_OfProduct_EqualsProductOfCbrts(string aStr, string bStr)
    {
        var a = decimal.Parse(aStr, CultureInfo.InvariantCulture);
        var b = decimal.Parse(bStr, CultureInfo.InvariantCulture);

        var cbrtProduct = ChiMath.Cbrt(a * b);
        var productCbrt = ChiMath.Cbrt(a) * ChiMath.Cbrt(b);

        cbrtProduct.Should().BeApproximately(productCbrt, 1e-10m);
    }

    #endregion

    #region Edge Cases

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Cbrt_DoubleSpecialValues_DoesNotThrow(double input)
    {
        var act = () => ChiMath.Cbrt(input);

        act.Should().NotThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(float.NaN)]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity)]
    public void Cbrt_FloatSpecialValues_DoesNotThrow(float input)
    {
        var act = () => ChiMath.Cbrt(input);

        act.Should().NotThrow<ArgumentException>();
    }

    #endregion
}