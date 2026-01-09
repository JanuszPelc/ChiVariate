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

        Assert.Equal(fromDecimal, fromRational);
        Assert.Equal(fromRational, fromString);
        Assert.Equal(fromString, fromDecimal);
    }

    #endregion

    #region Decimal to ChiFixed Casting

    [Fact]
    public void DecimalToChiFixed_Zero_ReturnsChiFixedZero()
    {
        var result = (ChiFixed)0.0m;

        Assert.Equal(ChiFixed.Zero, result);
    }

    [Fact]
    public void DecimalToChiFixed_One_ReturnsChiFixedOne()
    {
        var result = (ChiFixed)1.0m;

        Assert.Equal(ChiFixed.One, result);
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

        Assert.Equal(input, backToDecimal, 6);
    }

    #endregion

    #region ChiFixed to Decimal Casting

    [Fact]
    public void ChiFixedToDecimal_Zero_ReturnsZero()
    {
        var result = (decimal)ChiFixed.Zero;

        Assert.Equal(0.0m, result);
    }

    [Fact]
    public void ChiFixedToDecimal_One_ReturnsOne()
    {
        var result = (decimal)ChiFixed.One;

        Assert.Equal(1.0m, result);
    }

    [Fact]
    public void ChiFixedToDecimal_NegativeOne_ReturnsNegativeOne()
    {
        var result = (decimal)ChiFixed.NegativeOne;

        Assert.Equal(-1.0m, result);
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

        Assert.Equal(expected, result, 6);
    }

    [Fact]
    public void ChiFixedToDecimal_MaxValue_ReturnsLargePositiveDecimal()
    {
        var result = (decimal)ChiFixed.MaxValue;

        Assert.True(result > 2000000m);
        Assert.True(result < 3000000m);
    }

    [Fact]
    public void ChiFixedToDecimal_MinValue_ReturnsLargeNegativeDecimal()
    {
        var result = (decimal)ChiFixed.MinValue;

        Assert.True(result < -2000000m);
        Assert.True(result > -3000000m);
    }

    [Fact]
    public void ChiFixedToDecimal_RoundTrip_PreservesValue()
    {
        const decimal original = 123.456789m;
        var fixedValue = (ChiFixed)original;
        var roundTrip = (decimal)fixedValue;

        Assert.Equal(original, roundTrip, 6);
    }

    #endregion

    #region Rational to ChiFixed Casting

    [Fact]
    public void RationalToChiFixed_OneThird_ReturnsApproximateValue()
    {
        var result = (ChiFixed)(1, 3);

        var resultStr = result.ToString();
        Assert.StartsWith("0.3333333333", resultStr);
    }

    [Fact]
    public void RationalToChiFixed_OneHalf_ReturnsExactValue()
    {
        var result = (ChiFixed)(1, 2);

        Assert.Equal((ChiFixed)0.5m, result);
    }

    [Fact]
    public void RationalToChiFixed_ZeroDenominator_ThrowsDivideByZero()
    {
        Assert.Throws<DivideByZeroException>(() => (ChiFixed)(1, 0));
    }

    [Theory]
    [InlineData(1, 4, 0.25)]
    [InlineData(3, 4, 0.75)]
    [InlineData(-1, 2, -0.5)]
    [InlineData(22, 7, 3.142857142857)]
    public void RationalToChiFixed_VariousFractions_ReturnsExpectedValue(
        int numerator,
        int denominator,
        decimal expected)
    {
        var result = (ChiFixed)(numerator, denominator);
        var expectedChiFixed = (ChiFixed)expected;

        Assert.Equal(expectedChiFixed, result);
    }

    [Fact]
    public void RationalToChiFixed_LargeNumerator_HandlesCorrectly()
    {
        var result = (ChiFixed)(8000000, 1);
        var expected = (ChiFixed)8000000m;

        Assert.Equal(expected, result);
    }

    [Fact]
    public void RationalToChiFixed_VeryLargeDenominator_HandlesCorrectly()
    {
        var result = (ChiFixed)(1, 1000000000);

        Assert.True(result > ChiFixed.Zero);
        Assert.True(result < (ChiFixed)0.001m);
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

        Assert.Equal(expected, result);
    }

    [Fact]
    public void RationalToChiFixed_EqualLargeValues_ReturnsOne()
    {
        const int value = 5000000;
        var result = (ChiFixed)(value, value);

        Assert.Equal(ChiFixed.One, result);
    }

    [Fact]
    public void RationalToChiFixed_NegativeLargeValues_HandlesCorrectly()
    {
        const int value = 5000000;
        var result = (ChiFixed)(-value, value);

        Assert.Equal(-ChiFixed.One, result);
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

            Assert.True(diff < tolerance,
                $"Case ({numerator}/{denominator}): Expected {expectedDecimal}, got {resultDecimal}, diff {diff}");
        }
    }

    #endregion
}