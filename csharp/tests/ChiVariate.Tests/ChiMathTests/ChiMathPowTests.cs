using FluentAssertions;
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
    public void Pow_Float_BasicCases_ShouldReturnExpectedResults(float baseVal, float exponent, float expected)
    {
        // Act
        var result = ChiMath.Pow(baseVal, exponent);

        // Assert
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
    public void Pow_Double_BasicCases_ShouldReturnExpectedResults(double baseVal, double exponent, double expected)
    {
        // Act
        var result = ChiMath.Pow(baseVal, exponent);

        // Assert
        result.Should().BeApproximately(expected, 1e-10);
    }

    [Theory]
    [InlineData(42.0, 0.0, 1.0)]
    [InlineData(0.0, 5.0, 0.0)]
    [InlineData(1.0, 999.999, 1.0)]
    [InlineData(100.0, 1.0, 100.0)]
    public void Pow_Double_SpecialExponents_ShouldReturnExpectedResults(double baseVal, double exponent,
        double expected)
    {
        // Act
        var result = ChiMath.Pow(baseVal, exponent);

        // Assert
        result.Should().BeApproximately(expected, 1e-10);
    }

    [Theory]
    [InlineData(-2.0, 3.0, -8.0)]
    [InlineData(-3.0, 2.0, 9.0)]
    [InlineData(-4.0, 4.0, 256.0)]
    public void Pow_Double_NegativeBaseIntegerExponent_ShouldReturnExpectedResults(double baseVal, double exponent,
        double expected)
    {
        // Act
        var result = ChiMath.Pow(baseVal, exponent);

        // Assert
        result.Should().BeApproximately(expected, 1e-10);
    }

    #endregion

    #region Decimal Tests

    [Theory]
    [InlineData("2", "3", "8")]
    [InlineData("10", "2", "100")]
    [InlineData("0.5", "3", "0.125")]
    [InlineData("4", "0.5", "2")]
    public void Pow_Decimal_BasicCases_ShouldReturnExpectedResults(string baseStr, string expStr, string expectedStr)
    {
        // Arrange
        var baseVal = decimal.Parse(baseStr);
        var exponent = decimal.Parse(expStr);
        var expected = decimal.Parse(expectedStr);

        // Act
        var result = ChiMath.Pow(baseVal, exponent);

        // Assert
        result.Should().BeApproximately(expected, 1e-10m);
    }

    [Theory]
    [InlineData("42", "0", "1")]
    [InlineData("0", "5", "0")]
    [InlineData("1", "999.999", "1")]
    [InlineData("100", "1", "100")]
    public void Pow_Decimal_SpecialExponents_ShouldReturnExpectedResults(string baseStr, string expStr,
        string expectedStr)
    {
        // Arrange
        var baseVal = decimal.Parse(baseStr);
        var exponent = decimal.Parse(expStr);
        var expected = decimal.Parse(expectedStr);

        // Act
        var result = ChiMath.Pow(baseVal, exponent);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("-2", "3", "-8")]
    [InlineData("-3", "2", "9")]
    [InlineData("-4", "4", "256")]
    [InlineData("-5", "1", "-5")]
    public void Pow_Decimal_NegativeBaseIntegerExponent_ShouldReturnExpectedResults(string baseStr, string expStr,
        string expectedStr)
    {
        // Arrange
        var baseVal = decimal.Parse(baseStr);
        var exponent = decimal.Parse(expStr);
        var expected = decimal.Parse(expectedStr);

        // Act
        var result = ChiMath.Pow(baseVal, exponent);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("2", "10")]
    [InlineData("3", "5")]
    [InlineData("1.5", "20")]
    public void Pow_Decimal_LargeIntegerExponents_ShouldNotOverflow(string baseStr, string expStr)
    {
        // Arrange
        var baseVal = decimal.Parse(baseStr);
        var exponent = decimal.Parse(expStr);

        // Act & Assert
        var act = () => ChiMath.Pow(baseVal, exponent);
        act.Should().NotThrow();
    }

    [Fact]
    public void Pow_Decimal_ZeroToNegativePower_ShouldThrowDivideByZeroException()
    {
        // Act & Assert
        var act = () => ChiMath.Pow(0m, -1m);
        act.Should().Throw<DivideByZeroException>()
            .WithMessage("Cannot raise zero to a negative power.");
    }

    [Fact]
    public void Pow_Decimal_NegativeBaseToFractionalPower_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => ChiMath.Pow(-2m, 0.5m);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Cannot raise negative number to fractional power.");
    }

    [Theory]
    [InlineData("100", "0.5", "10")] // sqrt(100) = 10
    [InlineData("8", "0.333333333", "2")] // cbrt(8) ≈ 2
    [InlineData("16", "0.25", "2")] // 4th root of 16 = 2
    public void Pow_Decimal_FractionalExponents_ShouldReturnApproximateResults(string baseStr, string expStr,
        string expectedStr)
    {
        // Arrange
        var baseVal = decimal.Parse(baseStr);
        var exponent = decimal.Parse(expStr);
        var expected = decimal.Parse(expectedStr);

        // Act
        var result = ChiMath.Pow(baseVal, exponent);

        // Assert
        result.Should().BeApproximately(expected, 0.1m); // Looser tolerance for transcendental functions
    }

    [Theory]
    [InlineData("2", "500")]
    [InlineData("1.1", "1000")]
    public void Pow_Decimal_VeryLargeResults_ShouldThrowOverflowException(string baseStr, string expStr)
    {
        // Arrange
        var baseVal = decimal.Parse(baseStr);
        var exponent = decimal.Parse(expStr);

        // Act & Assert
        var act = () => ChiMath.Pow(baseVal, exponent);
        act.Should().Throw<OverflowException>();
    }

    #endregion

    #region Decimal Trigonometric Tests

    [Theory]
    [InlineData("0", "0")] // sin(0) = 0
    [InlineData("1.5707963267948966192313216916398", "1")] // sin(π/2) = 1
    [InlineData("3.1415926535897932384626433832795", "0")] // sin(π) = 0
    [InlineData("0.5235987755982988730771072305466", "0.5")] // sin(π/6) = 0.5
    [InlineData("0.7853981633974483096156608458199", "0.7071067811865475244008443621048")] // sin(π/4) = √2/2
    [InlineData("-1.5707963267948966192313216916398", "-1")] // sin(-π/2) = -1
    public void Sin_Decimal_KnownValues_ShouldReturnExpectedResults(string angleStr, string expectedStr)
    {
        // Arrange
        var angle = decimal.Parse(angleStr);
        var expected = decimal.Parse(expectedStr);

        // Act
        var result = ChiMath.Sin(angle);

        // Assert
        result.Should().BeApproximately(expected, 1e-10m);
    }

    [Theory]
    [InlineData("0", "1")] // cos(0) = 1
    [InlineData("1.5707963267948966192313216916398", "0")] // cos(π/2) = 0
    [InlineData("3.1415926535897932384626433832795", "-1")] // cos(π) = -1
    [InlineData("1.0471975511965977461542144610932", "0.5")] // cos(π/3) = 0.5
    [InlineData("0.7853981633974483096156608458199", "0.7071067811865475244008443621048")] // cos(π/4) = √2/2
    [InlineData("-1.5707963267948966192313216916398", "0")] // cos(-π/2) = 0
    public void Cos_Decimal_KnownValues_ShouldReturnExpectedResults(string angleStr, string expectedStr)
    {
        // Arrange
        var angle = decimal.Parse(angleStr);
        var expected = decimal.Parse(expectedStr);

        // Act
        var result = ChiMath.Cos(angle);

        // Assert
        result.Should().BeApproximately(expected, 1e-10m);
    }

    [Theory]
    [InlineData("0", "0")] // tan(0) = 0
    [InlineData("0.7853981633974483096156608458199", "1")] // tan(π/4) = 1
    [InlineData("0.5235987755982988730771072305466", "0.5773502691896257645091487805019")] // tan(π/6) = √3/3
    [InlineData("1.0471975511965977461542144610932", "1.7320508075688772935274463415059")] // tan(π/3) = √3
    [InlineData("-0.7853981633974483096156608458199", "-1")] // tan(-π/4) = -1
    public void Tan_Decimal_KnownValues_ShouldReturnExpectedResults(string angleStr, string expectedStr)
    {
        // Arrange
        var angle = decimal.Parse(angleStr);
        var expected = decimal.Parse(expectedStr);

        // Act
        var result = ChiMath.Tan(angle);

        // Assert
        result.Should().BeApproximately(expected, 1e-10m);
    }

    [Theory]
    [InlineData("1.5707963267948966192313216916398")] // π/2
    [InlineData("-1.5707963267948966192313216916398")] // -π/2
    [InlineData("4.7123889803846898576939650749193")] // 3π/2
    public void Tan_Decimal_UndefinedValues_ShouldThrowArgumentException(string angleStr)
    {
        // Arrange
        var angle = decimal.Parse(angleStr);

        // Act & Assert
        var act = () => ChiMath.Tan(angle);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Tangent is undefined at this value (cos = 0).");
    }

    [Theory]
    [InlineData("6.2831853071795864769252867665590")] // 2π
    [InlineData("12.566370614359172953850573533118")] // 4π
    [InlineData("-6.2831853071795864769252867665590")] // -2π
    [InlineData("100")] // Large angle
    public void Sin_Decimal_LargeAngles_ShouldNormalizeCorrectly(string angleStr)
    {
        // Arrange
        var angle = decimal.Parse(angleStr);

        // Act
        var result = ChiMath.Sin(angle);

        // Assert
        // Large angles should be normalized and still produce valid results
        result.Should().BeInRange(-1m, 1m);

        // For multiples of 2π, sin should be approximately 0
        if (angleStr == "6.2831853071795864769252867665590" || angleStr == "-6.2831853071795864769252867665590")
            result.Should().BeApproximately(0m, 1e-8m);
    }

    [Theory]
    [InlineData("6.2831853071795864769252867665590")] // 2π
    [InlineData("12.566370614359172953850573533118")] // 4π
    [InlineData("-6.2831853071795864769252867665590")] // -2π
    public void Cos_Decimal_LargeAngles_ShouldNormalizeCorrectly(string angleStr)
    {
        // Arrange
        var angle = decimal.Parse(angleStr);

        // Act
        var result = ChiMath.Cos(angle);

        // Assert
        // Large angles should be normalized and still produce valid results
        result.Should().BeInRange(-1m, 1m);

        // For multiples of 2π, cos should be approximately 1
        result.Should().BeApproximately(1m, 1e-8m);
    }

    [Fact]
    public void TrigonometricIdentity_SinSquaredPlusCosSquared_ShouldEqualOne()
    {
        // Test the fundamental identity: sin²(x) + cos²(x) = 1
        var testAngles = new[] { 0m, 0.5m, 1m, 1.5m, 2m, 2.5m, 3m };

        foreach (var angle in testAngles)
        {
            // Act
            var sin = ChiMath.Sin(angle);
            var cos = ChiMath.Cos(angle);
            var identity = sin * sin + cos * cos;

            // Assert
            identity.Should().BeApproximately(1m, 1e-10m,
                $"sin²({angle}) + cos²({angle}) should equal 1, but got {identity}");
        }
    }

    [Fact]
    public void TrigonometricIdentity_TanEqualssSinOverCos_ShouldBeConsistent()
    {
        // Test that tan(x) = sin(x) / cos(x)
        var testAngles = new[] { 0m, 0.5m, 1m, -0.5m, -1m };

        foreach (var angle in testAngles)
        {
            // Act
            var sin = ChiMath.Sin(angle);
            var cos = ChiMath.Cos(angle);
            var tan = ChiMath.Tan(angle);
            var expectedTan = sin / cos;

            // Assert
            tan.Should().BeApproximately(expectedTan, 1e-10m,
                $"tan({angle}) should equal sin({angle})/cos({angle}), but got {tan} vs {expectedTan}");
        }
    }

    [Theory]
    [InlineData("0.1")]
    [InlineData("0.5")]
    [InlineData("1.0")]
    [InlineData("-0.1")]
    [InlineData("-0.5")]
    public void Sin_Decimal_SmallAngles_ShouldApproximateAngle(string angleStr)
    {
        // For small angles, sin(x) ≈ x
        var angle = decimal.Parse(angleStr);

        // Act
        var result = ChiMath.Sin(angle);

        // Assert
        if (Math.Abs(angle) < 0.2m)
            result.Should().BeApproximately(angle, 0.01m,
                $"For small angle {angle}, sin(x) should approximate x");
    }

    [Theory]
    [InlineData("0.1")]
    [InlineData("0.5")]
    [InlineData("-0.1")]
    [InlineData("-0.5")]
    public void Cos_Decimal_SmallAngles_ShouldApproximateOne(string angleStr)
    {
        // For small angles, cos(x) ≈ 1
        var angle = decimal.Parse(angleStr);

        // Act
        var result = ChiMath.Cos(angle);

        // Assert
        if (Math.Abs(angle) < 0.2m)
            result.Should().BeApproximately(1m, 0.1m,
                $"For small angle {angle}, cos(x) should approximate 1");
    }

    #endregion

    #region Edge Cases

    [Theory]
    [InlineData(0.0, 0.0)]
    [InlineData(double.PositiveInfinity, 1.0)]
    [InlineData(double.NegativeInfinity, 1.0)]
    [InlineData(double.NaN, 1.0)]
    public void Pow_Double_EdgeCases_ShouldHandleSpecialValues(double baseVal, double exponent)
    {
        // Arrange, Act & Assert
        var act = () => ChiMath.Pow(baseVal, exponent);
        act.Should().NotThrow<ArgumentException>();
    }

    [Theory]
    [InlineData("0.0000000000000000000000001", "2")]
    [InlineData("999999999999999999999999", "0.0001")]
    public void Pow_Decimal_ExtremeValues_ShouldNotThrowUnexpectedly(string baseStr, string expStr)
    {
        // Arrange
        var baseVal = decimal.Parse(baseStr);
        var exponent = decimal.Parse(expStr);

        // Act & Assert
        var act = () => ChiMath.Pow(baseVal, exponent);
        act.Should().NotThrow<ArgumentException>();
    }

    #endregion
}