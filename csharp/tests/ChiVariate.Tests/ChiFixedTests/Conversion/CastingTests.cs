using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Conversion;

public class CastingTests
{
    #region Cross-Casting Equivalence

    [Fact]
    public void CastingMethods_EquivalentValues_AreEqual()
    {
        var fromDecimal = (ChiFixed)0.33333333333333m;
        var fromRational = (ChiFixed)(1, 3);
        var fromString = ChiFixed.Parse("0.33333333333333");

        fromRational.Should().Be(fromDecimal);
        fromString.Should().Be(fromRational);
        fromDecimal.Should().Be(fromString);
    }

    #endregion

    #region Decimal to ChiFixed Casting

    [Fact]
    public void DecimalToChiFixed_Zero_ReturnsChiFixedZero()
    {
        var result = (ChiFixed)0.0m;

        result.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void DecimalToChiFixed_One_ReturnsChiFixedOne()
    {
        var result = (ChiFixed)1.0m;

        result.Should().Be(ChiFixed.One);
    }

    [Theory]
    [InlineData(0.5)]
    [InlineData(0.25)]
    [InlineData(0.75)]
    [InlineData(123.456)]
    [InlineData(-42.5)]
    public void DecimalToChiFixed_PositiveAndNegativeValues_PreservesValue(decimal input)
    {
        var result = (ChiFixed)input;
        var backToDecimal = (decimal)result;

        backToDecimal.Should().BeApproximately(input, 0.000001m);
    }

    #endregion

    #region ChiFixed to Decimal Casting

    [Fact]
    public void ChiFixedToDecimal_Zero_ReturnsZero()
    {
        var result = (decimal)ChiFixed.Zero;

        result.Should().Be(0.0m);
    }

    [Fact]
    public void ChiFixedToDecimal_One_ReturnsOne()
    {
        var result = (decimal)ChiFixed.One;

        result.Should().Be(1.0m);
    }

    [Fact]
    public void ChiFixedToDecimal_NegativeOne_ReturnsNegativeOne()
    {
        var result = (decimal)ChiFixed.NegativeOne;

        result.Should().Be(-1.0m);
    }

    [Theory]
    [InlineData(0.5)]
    [InlineData(0.25)]
    [InlineData(123.456)]
    [InlineData(-42.75)]
    [InlineData(3.141592653589)]
    public void ChiFixedToDecimal_VariousValues_ConvertsCorrectly(decimal expected)
    {
        var fixedValue = (ChiFixed)expected;
        var result = (decimal)fixedValue;

        result.Should().BeApproximately(expected, 0.000001m);
    }

    [Fact]
    public void ChiFixedToDecimal_MaxValue_ReturnsLargePositiveDecimal()
    {
        var result = (decimal)ChiFixed.MaxValue;

        (result > 2_000_000_000m).Should().BeTrue();
        (result < 2_200_000_000m).Should().BeTrue();
    }

    [Fact]
    public void ChiFixedToDecimal_MinValue_ReturnsLargeNegativeDecimal()
    {
        var result = (decimal)ChiFixed.MinValue;

        (result < -2_000_000_000m).Should().BeTrue();
        (result > -2_200_000_000m).Should().BeTrue();
    }

    [Fact]
    public void ChiFixedToDecimal_RoundTrip_PreservesValue()
    {
        const decimal original = 123.456789m;
        var fixedValue = (ChiFixed)original;
        var roundTrip = (decimal)fixedValue;

        roundTrip.Should().BeApproximately(original, 0.000001m);
    }

    #endregion

    #region Int to ChiFixed Casting

    [Fact]
    public void IntToChiFixed_Zero_ReturnsChiFixedZero()
    {
        var result = (ChiFixed)0;

        result.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void IntToChiFixed_One_ReturnsChiFixedOne()
    {
        var result = (ChiFixed)1;

        result.Should().Be(ChiFixed.One);
    }

    [Fact]
    public void IntToChiFixed_NegativeOne_ReturnsChiFixedNegativeOne()
    {
        var result = (ChiFixed)(-1);

        result.Should().Be(ChiFixed.NegativeOne);
    }

    [Theory]
    [InlineData(42)]
    [InlineData(-42)]
    [InlineData(1000000)]
    [InlineData(-1000000)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void IntToChiFixed_VariousValues_PreservesValueExactly(int input)
    {
        var result = (ChiFixed)input;
        var backToInt = (int)result;

        backToInt.Should().Be(input);
    }

    [Fact]
    public void IntToChiFixed_MaxInt32_IsRepresentableExactly()
    {
        var result = (ChiFixed)int.MaxValue;
        var asDecimal = (decimal)result;

        asDecimal.Should().Be(int.MaxValue);
    }

    [Fact]
    public void IntToChiFixed_MinInt32_IsRepresentableExactly()
    {
        var result = (ChiFixed)int.MinValue;
        var asDecimal = (decimal)result;

        asDecimal.Should().Be(int.MinValue);
    }

    #endregion

    #region ChiFixed to Int Casting

    [Fact]
    public void ChiFixedToInt_Zero_ReturnsZero()
    {
        var result = (int)ChiFixed.Zero;

        result.Should().Be(0);
    }

    [Fact]
    public void ChiFixedToInt_One_ReturnsOne()
    {
        var result = (int)ChiFixed.One;

        result.Should().Be(1);
    }

    [Fact]
    public void ChiFixedToInt_NegativeOne_ReturnsNegativeOne()
    {
        var result = (int)ChiFixed.NegativeOne;

        result.Should().Be(-1);
    }

    [Theory]
    [InlineData(3.7, 3)]
    [InlineData(3.2, 3)]
    [InlineData(-3.7, -3)]
    [InlineData(-3.2, -3)]
    [InlineData(0.999, 0)]
    [InlineData(-0.999, 0)]
    public void ChiFixedToInt_FractionalValues_TruncatesTowardZero(decimal input, int expected)
    {
        var fixedValue = (ChiFixed)input;
        var result = (int)fixedValue;

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(42)]
    [InlineData(-42)]
    [InlineData(1000000)]
    [InlineData(-1000000)]
    public void ChiFixedToInt_WholeNumbers_ReturnsExactValue(int expected)
    {
        var fixedValue = (ChiFixed)expected;
        var result = (int)fixedValue;

        result.Should().Be(expected);
    }

    #endregion

    #region Rational to ChiFixed Casting

    [Fact]
    public void RationalToChiFixed_OneThird_ReturnsApproximateValue()
    {
        var result = (ChiFixed)(1, 3);

        var resultStr = result.ToString();
        resultStr.Should().StartWith("0.33333333");
    }

    [Fact]
    public void RationalToChiFixed_OneHalf_ReturnsExactValue()
    {
        var result = (ChiFixed)(1, 2);

        result.Should().Be((ChiFixed)0.5m);
    }

    [Fact]
    public void RationalToChiFixed_ZeroDenominator_ThrowsDivideByZero()
    {
        var act = () => (ChiFixed)(1, 0);
        act.Should().Throw<DivideByZeroException>();
    }

    [Theory]
    [InlineData(1, 4, 0.25)]
    [InlineData(3, 4, 0.75)]
    [InlineData(-1, 2, -0.5)]
    [InlineData(22, 7, 3.1428571428)]
    public void RationalToChiFixed_VariousFractions_ReturnsExpectedValue(
        int numerator,
        int denominator,
        decimal expected)
    {
        var result = (ChiFixed)(numerator, denominator);
        var expectedChiFixed = (ChiFixed)expected;

        result.Should().Be(expectedChiFixed);
    }

    [Fact]
    public void RationalToChiFixed_LargeNumerator_HandlesCorrectly()
    {
        var result = (ChiFixed)(8000000, 1);
        var expected = (ChiFixed)8000000m;

        result.Should().Be(expected);
    }

    [Fact]
    public void RationalToChiFixed_VeryLargeDenominator_HandlesCorrectly()
    {
        var result = (ChiFixed)(1, 1000000000);

        (result > ChiFixed.Zero).Should().BeTrue();
        (result < (ChiFixed)0.001m).Should().BeTrue();
    }

    [Fact]
    public void RationalToChiFixed_LargeNumeratorAndDenominator_ProducesCorrectRatio()
    {
        var maxAsDecimal = (decimal)ChiFixed.MaxValue;
        var largeValue = (int)(maxAsDecimal / 2) - 1000;
        var numerator = largeValue;
        var denominator = largeValue * 2;

        var result = (ChiFixed)(numerator, denominator);
        var expected = (ChiFixed)0.5m;

        result.Should().Be(expected);
    }

    [Fact]
    public void RationalToChiFixed_EqualLargeValues_ReturnsOne()
    {
        const int value = 5000000;
        var result = (ChiFixed)(value, value);

        result.Should().Be(ChiFixed.One);
    }

    [Fact]
    public void RationalToChiFixed_NegativeLargeValues_HandlesCorrectly()
    {
        const int value = 5000000;
        var result = (ChiFixed)(-value, value);

        result.Should().Be(-ChiFixed.One);
    }

    [Fact]
    public void RationalToChiFixed_LargeNumerators_ProducesValidResults()
    {
        var maxAsDecimal = (decimal)ChiFixed.MaxValue;
        var largeValue = (int)maxAsDecimal - 1000;
        const decimal tolerance = 0.00000001m;

        var testCases = new[]
        {
            (numerator: largeValue, denominator: 3),
            (numerator: largeValue, denominator: 7),
            (numerator: -largeValue, denominator: 3)
        };

        foreach (var (numerator, denominator) in testCases)
        {
            var result = (ChiFixed)(numerator, denominator);
            var expectedDecimal = (decimal)numerator / denominator;
            var resultDecimal = (decimal)result;

            var diff = Math.Abs(resultDecimal - expectedDecimal);

            (diff < tolerance).Should().BeTrue(
                $"Case ({numerator}/{denominator}): Expected {expectedDecimal}, got {resultDecimal}, diff {diff}");
        }
    }

    #endregion
}