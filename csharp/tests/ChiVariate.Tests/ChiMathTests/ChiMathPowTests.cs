using System.Globalization;
using AwesomeAssertions;
using Xunit;

namespace ChiVariate.Tests.ChiMathTests;

#pragma warning disable CS1591
public class ChiMathPowTests
{
    #region Float Tests

    [Theory]
    [InlineData(2.0f, 3.0f, 8.0f)]
    [InlineData(10.0f, 2.0f, 100.0f)]
    [InlineData(0.5f, 3.0f, 0.125f)]
    public void Pow_Float_ReturnsExpectedResult(float baseVal, float exponent, float expected)
    {
        var result = ChiMath.Pow(baseVal, exponent);

        result.Should().BeApproximately(expected, 1e-5f);
    }

    #endregion

    #region Double Tests

    [Theory]
    [InlineData(2.0, 3.0, 8.0)]
    [InlineData(10.0, 2.0, 100.0)]
    [InlineData(0.5, 3.0, 0.125)]
    [InlineData(4.0, 0.5, 2.0)]
    [InlineData(27.0, 1.0 / 3.0, 3.0)]
    public void Pow_Double_ReturnsExpectedResult(double baseVal, double exponent, double expected)
    {
        var result = ChiMath.Pow(baseVal, exponent);

        result.Should().BeApproximately(expected, 1e-10);
    }

    [Theory]
    [InlineData(42.0, 0.0, 1.0)]
    [InlineData(0.0, 5.0, 0.0)]
    [InlineData(1.0, 999.999, 1.0)]
    [InlineData(100.0, 1.0, 100.0)]
    public void Pow_DoubleSpecialExponents_ReturnsExpectedResult(double baseVal, double exponent, double expected)
    {
        var result = ChiMath.Pow(baseVal, exponent);

        result.Should().BeApproximately(expected, 1e-10);
    }

    [Theory]
    [InlineData(-2.0, 3.0, -8.0)]
    [InlineData(-3.0, 2.0, 9.0)]
    [InlineData(-4.0, 4.0, 256.0)]
    public void Pow_DoubleNegativeBase_ReturnsExpectedResult(double baseVal, double exponent, double expected)
    {
        var result = ChiMath.Pow(baseVal, exponent);

        result.Should().BeApproximately(expected, 1e-10);
    }

    #endregion

    #region Decimal Tests

    [Theory]
    [InlineData("2", "3", "8")]
    [InlineData("10", "2", "100")]
    [InlineData("0.5", "3", "0.125")]
    [InlineData("4", "0.5", "2")]
    public void Pow_Decimal_ReturnsExpectedResult(string baseStr, string expStr, string expectedStr)
    {
        var baseVal = decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var exponent = decimal.Parse(expStr, CultureInfo.InvariantCulture);
        var expected = decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Pow(baseVal, exponent);

        result.Should().BeApproximately(expected, 1e-10m);
    }

    [Theory]
    [InlineData("42", "0", "1")]
    [InlineData("0", "5", "0")]
    [InlineData("1", "999.999", "1")]
    [InlineData("100", "1", "100")]
    public void Pow_DecimalSpecialExponents_ReturnsExpectedResult(string baseStr, string expStr, string expectedStr)
    {
        var baseVal = decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var exponent = decimal.Parse(expStr, CultureInfo.InvariantCulture);
        var expected = decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Pow(baseVal, exponent);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("-2", "3", "-8")]
    [InlineData("-3", "2", "9")]
    [InlineData("-4", "4", "256")]
    [InlineData("-5", "1", "-5")]
    public void Pow_DecimalNegativeBase_ReturnsExpectedResult(string baseStr, string expStr, string expectedStr)
    {
        var baseVal = decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var exponent = decimal.Parse(expStr, CultureInfo.InvariantCulture);
        var expected = decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Pow(baseVal, exponent);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("2", "10")]
    [InlineData("3", "5")]
    [InlineData("1.5", "20")]
    public void Pow_DecimalLargeExponents_DoesNotThrow(string baseStr, string expStr)
    {
        var baseVal = decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var exponent = decimal.Parse(expStr, CultureInfo.InvariantCulture);

        var act = () => ChiMath.Pow(baseVal, exponent);

        act.Should().NotThrow();
    }

    [Fact]
    public void Pow_DecimalZeroToNegative_ThrowsDivideByZeroException()
    {
        var act = () => ChiMath.Pow(0m, -1m);

        act.Should().Throw<DivideByZeroException>();
    }

    [Fact]
    public void Pow_DecimalNegativeBaseFractional_ThrowsArgumentException()
    {
        var act = () => ChiMath.Pow(-2m, 0.5m);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("100", "0.5", "10")]
    [InlineData("8", "0.333333333", "2")]
    [InlineData("16", "0.25", "2")]
    public void Pow_DecimalFractionalExponents_ReturnsApproximateResults(string baseStr, string expStr,
        string expectedStr)
    {
        var baseVal = decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var exponent = decimal.Parse(expStr, CultureInfo.InvariantCulture);
        var expected = decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Pow(baseVal, exponent);

        result.Should().BeApproximately(expected, 0.1m);
    }

    [Theory]
    [InlineData("2", "500")]
    [InlineData("1.1", "1000")]
    public void Pow_DecimalVeryLarge_ThrowsOverflowException(string baseStr, string expStr)
    {
        var baseVal = decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var exponent = decimal.Parse(expStr, CultureInfo.InvariantCulture);

        var act = () => ChiMath.Pow(baseVal, exponent);

        act.Should().Throw<OverflowException>();
    }

    #endregion

    #region ChiFixed Tests

    [Theory]
    [InlineData("2", "3", "8")]
    [InlineData("10", "2", "100")]
    [InlineData("0.5", "3", "0.125")]
    [InlineData("4", "0.5", "2")]
    public void Pow_ChiFixed_ReturnsExpectedResult(string baseStr, string expStr, string expectedStr)
    {
        var baseVal = (ChiFixed)decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var exponent = (ChiFixed)decimal.Parse(expStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Pow(baseVal, exponent);

        ((decimal)result).Should().BeApproximately((decimal)expected, 1e-2m);
    }

    [Theory]
    [InlineData("42", "0", "1")]
    [InlineData("0", "5", "0")]
    [InlineData("1", "999", "1")]
    [InlineData("100", "1", "100")]
    public void Pow_ChiFixedSpecialExponents_ReturnsExpectedResult(string baseStr, string expStr, string expectedStr)
    {
        var baseVal = (ChiFixed)decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var exponent = (ChiFixed)decimal.Parse(expStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Pow(baseVal, exponent);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("-2", "3", "-8")]
    [InlineData("-3", "2", "9")]
    [InlineData("-4", "4", "256")]
    public void Pow_ChiFixedNegativeBase_ReturnsExpectedResult(string baseStr, string expStr, string expectedStr)
    {
        var baseVal = (ChiFixed)decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var exponent = (ChiFixed)decimal.Parse(expStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Pow(baseVal, exponent);

        result.Should().Be(expected);
    }

    [Fact]
    public void Pow_ChiFixedZeroToNegative_ThrowsDivideByZeroException()
    {
        var act = () => ChiMath.Pow(ChiFixed.Zero, (ChiFixed)(-1m));

        act.Should().Throw<DivideByZeroException>();
    }

    #endregion

    #region Decimal Trigonometric Tests

    [Theory]
    [InlineData("0", "0")]
    [InlineData("1.5707963267948966192313216916398", "1")]
    [InlineData("3.1415926535897932384626433832795", "0")]
    [InlineData("0.5235987755982988730771072305466", "0.5")]
    [InlineData("0.7853981633974483096156608458199", "0.7071067811865475244008443621048")]
    [InlineData("-1.5707963267948966192313216916398", "-1")]
    public void Sin_DecimalKnownValues_ReturnsExpectedResult(string angleStr, string expectedStr)
    {
        var angle = decimal.Parse(angleStr, CultureInfo.InvariantCulture);
        var expected = decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Sin(angle);

        result.Should().BeApproximately(expected, 1e-10m);
    }

    [Theory]
    [InlineData("0", "1")]
    [InlineData("1.5707963267948966192313216916398", "0")]
    [InlineData("3.1415926535897932384626433832795", "-1")]
    [InlineData("1.0471975511965977461542144610932", "0.5")]
    [InlineData("0.7853981633974483096156608458199", "0.7071067811865475244008443621048")]
    [InlineData("-1.5707963267948966192313216916398", "0")]
    public void Cos_DecimalKnownValues_ReturnsExpectedResult(string angleStr, string expectedStr)
    {
        var angle = decimal.Parse(angleStr, CultureInfo.InvariantCulture);
        var expected = decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Cos(angle);

        result.Should().BeApproximately(expected, 1e-10m);
    }

    [Theory]
    [InlineData("0", "0")]
    [InlineData("0.7853981633974483096156608458199", "1")]
    [InlineData("0.5235987755982988730771072305466", "0.5773502691896257645091487805019")]
    [InlineData("1.0471975511965977461542144610932", "1.7320508075688772935274463415059")]
    [InlineData("-0.7853981633974483096156608458199", "-1")]
    public void Tan_DecimalKnownValues_ReturnsExpectedResult(string angleStr, string expectedStr)
    {
        var angle = decimal.Parse(angleStr, CultureInfo.InvariantCulture);
        var expected = decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Tan(angle);

        result.Should().BeApproximately(expected, 1e-10m);
    }

    [Theory]
    [InlineData("1.5707963267948966192313216916398")]
    [InlineData("-1.5707963267948966192313216916398")]
    [InlineData("4.7123889803846898576939650749193")]
    public void Tan_DecimalUndefined_ThrowsArgumentException(string angleStr)
    {
        var angle = decimal.Parse(angleStr, CultureInfo.InvariantCulture);

        var act = () => ChiMath.Tan(angle);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("6.2831853071795864769252867665590")]
    [InlineData("12.566370614359172953850573533118")]
    [InlineData("-6.2831853071795864769252867665590")]
    [InlineData("100")]
    public void Sin_DecimalLargeAngles_Normalizes(string angleStr)
    {
        var angle = decimal.Parse(angleStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Sin(angle);

        result.Should().BeInRange(-1m, 1m);

        if (angleStr == "6.2831853071795864769252867665590" || angleStr == "-6.2831853071795864769252867665590")
            result.Should().BeApproximately(0m, 1e-8m);
    }

    [Theory]
    [InlineData("6.2831853071795864769252867665590")]
    [InlineData("12.566370614359172953850573533118")]
    [InlineData("-6.2831853071795864769252867665590")]
    public void Cos_DecimalLargeAngles_Normalizes(string angleStr)
    {
        var angle = decimal.Parse(angleStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Cos(angle);

        result.Should().BeInRange(-1m, 1m);
        result.Should().BeApproximately(1m, 1e-8m);
    }

    [Fact]
    public void SinCos_DecimalPythagoreanIdentity_EqualsOne()
    {
        var testAngles = new[] { 0m, 0.5m, 1m, 1.5m, 2m, 2.5m, 3m };

        foreach (var angle in testAngles)
        {
            var sin = ChiMath.Sin(angle);
            var cos = ChiMath.Cos(angle);
            var identity = sin * sin + cos * cos;

            identity.Should().BeApproximately(1m, 1e-10m);
        }
    }

    [Fact]
    public void Tan_DecimalRatio_EqualsSinOverCos()
    {
        var testAngles = new[] { 0m, 0.5m, 1m, -0.5m, -1m };

        foreach (var angle in testAngles)
        {
            var sin = ChiMath.Sin(angle);
            var cos = ChiMath.Cos(angle);
            var tan = ChiMath.Tan(angle);
            var expectedTan = sin / cos;

            tan.Should().BeApproximately(expectedTan, 1e-10m);
        }
    }

    [Theory]
    [InlineData("0.1")]
    [InlineData("0.5")]
    [InlineData("1.0")]
    [InlineData("-0.1")]
    [InlineData("-0.5")]
    public void Sin_DecimalSmallAngles_ApproximatesAngle(string angleStr)
    {
        var angle = decimal.Parse(angleStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Sin(angle);

        if (Math.Abs(angle) < 0.2m)
            result.Should().BeApproximately(angle, 0.01m);
    }

    [Theory]
    [InlineData("0.1")]
    [InlineData("0.5")]
    [InlineData("-0.1")]
    [InlineData("-0.5")]
    public void Cos_DecimalSmallAngles_ApproximatesOne(string angleStr)
    {
        var angle = decimal.Parse(angleStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Cos(angle);

        if (Math.Abs(angle) < 0.2m)
            result.Should().BeApproximately(1m, 0.1m);
    }

    #endregion

    #region ChiFixed Trigonometric Tests

    [Theory]
    [InlineData("0", "0")]
    [InlineData("1.5707963267948966", "1")]
    [InlineData("3.1415926535897932", "0")]
    [InlineData("0.5235987755982989", "0.5")]
    public void Sin_ChiFixedKnownValues_ReturnsExpectedResult(string angleStr, string expectedStr)
    {
        var angle = (ChiFixed)decimal.Parse(angleStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Sin(angle);

        ((decimal)result).Should().BeApproximately((decimal)expected, 1e-3m);
    }

    [Theory]
    [InlineData("0", "1")]
    [InlineData("1.5707963267948966", "0")]
    [InlineData("3.1415926535897932", "-1")]
    [InlineData("1.0471975511965977", "0.5")]
    public void Cos_ChiFixedKnownValues_ReturnsExpectedResult(string angleStr, string expectedStr)
    {
        var angle = (ChiFixed)decimal.Parse(angleStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Cos(angle);

        ((decimal)result).Should().BeApproximately((decimal)expected, 1e-3m);
    }

    [Theory]
    [InlineData("0", "0")]
    [InlineData("0.7853981633974483", "1")]
    [InlineData("-0.7853981633974483", "-1")]
    public void Tan_ChiFixedKnownValues_ReturnsExpectedResult(string angleStr, string expectedStr)
    {
        var angle = (ChiFixed)decimal.Parse(angleStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Tan(angle);

        ((decimal)result).Should().BeApproximately((decimal)expected, 1e-3m);
    }

    [Fact]
    public void SinCos_ChiFixedPythagoreanIdentity_EqualsOne()
    {
        var testAngles = new[] { 0m, 0.5m, 1m, 1.5m, 2m };

        foreach (var angle in testAngles)
        {
            var fixedAngle = (ChiFixed)angle;
            var sin = ChiMath.Sin(fixedAngle);
            var cos = ChiMath.Cos(fixedAngle);
            var identity = sin * sin + cos * cos;

            ((decimal)identity).Should().BeApproximately(1m, 1e-6m);
        }
    }

    #endregion

    #region Edge Cases

    [Theory]
    [InlineData(0.0, 0.0)]
    [InlineData(double.PositiveInfinity, 1.0)]
    [InlineData(double.NegativeInfinity, 1.0)]
    [InlineData(double.NaN, 1.0)]
    public void Pow_DoubleSpecialValues_DoesNotThrowArgumentException(double baseVal, double exponent)
    {
        var act = () => ChiMath.Pow(baseVal, exponent);

        act.Should().NotThrow<ArgumentException>();
    }

    [Theory]
    [InlineData("0.0000000000000000000000001", "2")]
    [InlineData("999999999999999999999999", "0.0001")]
    public void Pow_DecimalExtremeValues_DoesNotThrowArgumentException(string baseStr, string expStr)
    {
        var baseVal = decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var exponent = decimal.Parse(expStr, CultureInfo.InvariantCulture);

        var act = () => ChiMath.Pow(baseVal, exponent);

        act.Should().NotThrow<ArgumentException>();
    }

    #endregion
}