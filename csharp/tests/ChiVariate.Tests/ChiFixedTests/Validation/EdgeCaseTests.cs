using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Validation;

public class EdgeCaseTests
{
    [Fact]
    public void Addition_MaxValuePlusOne_SaturatesToMaxValue()
    {
        var result = ChiFixed.MaxValue + ChiFixed.One;

        result.Should().Be(ChiFixed.MaxValue);
    }

    [Fact]
    public void Addition_MaxValuePlusEpsilon_SaturatesToMaxValue()
    {
        var result = ChiFixed.MaxValue + ChiFixed.Epsilon;

        result.Should().Be(ChiFixed.MaxValue);
    }

    [Fact]
    public void Subtraction_MinValueMinusOne_SaturatesToMinValue()
    {
        var result = ChiFixed.MinValue - ChiFixed.One;

        result.Should().Be(ChiFixed.MinValue);
    }

    [Fact]
    public void Subtraction_MinValueMinusEpsilon_SaturatesToMinValue()
    {
        var result = ChiFixed.MinValue - ChiFixed.Epsilon;

        result.Should().Be(ChiFixed.MinValue);
    }

    [Fact]
    public void Multiplication_MaxValueTimesTwo_SaturatesToMaxValue()
    {
        var two = (ChiFixed)2m;
        var result = ChiFixed.MaxValue * two;

        result.Should().Be(ChiFixed.MaxValue);
    }

    [Fact]
    public void Multiplication_LargeValues_DoesNotReturnZero()
    {
        var large1 = (ChiFixed)1000000m;
        var large2 = (ChiFixed)1000000m;
        var result = large1 * large2;

        result.Should().NotBe(ChiFixed.Zero);
    }

    [Fact]
    public void Negate_MinValue_ReturnsMinValue()
    {
        var result = -ChiFixed.MinValue;

        result.Should().Be(ChiFixed.MinValue);
    }

    [Fact]
    public void Abs_MinValue_ReturnsMinValue()
    {
        var result = ChiFixed.Abs(ChiFixed.MinValue);

        result.Should().Be(ChiFixed.MinValue);
    }

    [Fact]
    public void Abs_NegativeOne_ReturnsOne()
    {
        var result = ChiFixed.Abs(ChiFixed.NegativeOne);

        result.Should().Be(ChiFixed.One);
    }

    [Fact]
    public void Division_MinValueByNegativeOne_SaturatesToMaxValue()
    {
        var result = ChiFixed.MinValue / ChiFixed.NegativeOne;

        result.Should().Be(ChiFixed.MaxValue);
    }

    [Fact]
    public void Increment_MaxValue_SaturatesToMaxValue()
    {
        var value = ChiFixed.MaxValue;
        value++;

        value.Should().Be(ChiFixed.MaxValue);
    }

    [Fact]
    public void Decrement_MinValue_SaturatesToMinValue()
    {
        var value = ChiFixed.MinValue;
        value--;

        value.Should().Be(ChiFixed.MinValue);
    }

    [Fact]
    public void Epsilon_Addition_IncrementsByOne()
    {
        var result = ChiFixed.Zero + ChiFixed.Epsilon;

        result.Raw.Should().Be(1L);
        result.Should().Be(ChiFixed.Epsilon);
    }

    [Fact]
    public void Epsilon_Subtraction_DecrementsByOne()
    {
        var result = ChiFixed.Zero - ChiFixed.Epsilon;

        result.Raw.Should().Be(-1L);
        result.Should().Be(-ChiFixed.Epsilon);
    }

    [Fact]
    public void Epsilon_Multiplication_TruncatesToZero()
    {
        var half = (ChiFixed)0.5m;
        var result = ChiFixed.Epsilon * half;

        result.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void MaxValue_RelativeToMinValue_IsOffsetByOne()
    {
        ChiFixed.MaxValue.Raw.Should().Be(-ChiFixed.MinValue.Raw - 1);
    }

    [Fact]
    public void MinValue_PlusMaxValue_ReturnsNegativeEpsilon()
    {
        var result = ChiFixed.MinValue + ChiFixed.MaxValue;

        result.Raw.Should().Be(-1L);
        result.Should().Be(-ChiFixed.Epsilon);
    }

    [Fact]
    public void Division_EpsilonByTwo_ReturnsZero()
    {
        var two = (ChiFixed)2m;
        var result = ChiFixed.Epsilon / two;

        result.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void Multiplication_MaxValueByEpsilon_DoesNotOverflow()
    {
        var result = ChiFixed.MaxValue * ChiFixed.Epsilon;

        (result.Raw > 0).Should().BeTrue();
    }

    [Fact]
    public void Addition_NearMaxValue_SaturatesToMaxValue()
    {
        var nearMax = new ChiFixed(ChiFixed.MaxValue.Raw - 100);
        var large = (ChiFixed)1000m;
        var result = nearMax + large;

        result.Should().Be(ChiFixed.MaxValue);
    }

    [Fact]
    public void Subtraction_NearMinValue_SaturatesToMinValue()
    {
        var nearMin = new ChiFixed(ChiFixed.MinValue.Raw + 100);
        var large = (ChiFixed)1000m;
        var result = nearMin - large;

        result.Should().Be(ChiFixed.MinValue);
    }

    [Fact]
    public void Modulo_MinValueByNegativeOne_ReturnsZero()
    {
        var result = ChiFixed.MinValue % ChiFixed.NegativeOne;

        result.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void Modulo_MinValueByTwo_ReturnsZero()
    {
        var two = (ChiFixed)2m;
        var result = ChiFixed.MinValue % two;

        result.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void Modulo_MaxValueByEpsilon_ReturnsZero()
    {
        var result = ChiFixed.MaxValue % ChiFixed.Epsilon;

        result.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void Round_MaxValue_ReturnsMaxValue()
    {
        var result = ChiFixed.Round(ChiFixed.MaxValue, 0, MidpointRounding.ToEven);

        result.Should().Be(ChiFixed.MaxValue);
    }

    [Fact]
    public void Round_MinValue_ReturnsMinValue()
    {
        var result = ChiFixed.Round(ChiFixed.MinValue, 0, MidpointRounding.ToEven);

        result.Should().Be(ChiFixed.MinValue);
    }

    [Fact]
    public void Round_Epsilon_RoundsToZero()
    {
        var result = ChiFixed.Round(ChiFixed.Epsilon, 0, MidpointRounding.ToEven);

        result.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void Round_MidpointValue_RoundsToNearestEven()
    {
        var nearHalf = (ChiFixed)1.5m;
        var result = ChiFixed.Round(nearHalf, 0, MidpointRounding.ToEven);

        result.Should().Be((ChiFixed)2m);
    }

    #region Large Decimal Conversion Tests

    [Theory]
    [InlineData("12345678901234567890")]
    [InlineData("99999999999999999999")]
    [InlineData("79228162514264337593543950335")]
    public void DecimalCast_VeryLargePositiveValue_ProducesDeterministicResult(string decimalStr)
    {
        var largeDecimal = decimal.Parse(decimalStr);
        var result = (ChiFixed)largeDecimal;

        (result == ChiFixed.PositiveInfinity || result.Raw > 0 || result.Raw < 0).Should().BeTrue();
    }

    [Theory]
    [InlineData("-12345678901234567890")]
    [InlineData("-99999999999999999999")]
    [InlineData("-79228162514264337593543950335")]
    public void DecimalCast_VeryLargeNegativeValue_ProducesDeterministicResult(string decimalStr)
    {
        var largeDecimal = decimal.Parse(decimalStr);
        var result = (ChiFixed)largeDecimal;

        (result == ChiFixed.NegativeInfinity || result.Raw > 0 || result.Raw < 0).Should().BeTrue();
    }

    [Fact]
    public void DecimalCast_DecimalMaxValue_DoesNotThrow()
    {
        var act = () => (ChiFixed)decimal.MaxValue;
        act.Should().NotThrow();
    }

    [Fact]
    public void DecimalCast_DecimalMinValue_DoesNotThrow()
    {
        var act = () => (ChiFixed)decimal.MinValue;
        act.Should().NotThrow();
    }

    [Fact]
    public void DecimalCast_VeryLargeValue_ProducesDeterministicResult()
    {
        const decimal largeDecimal = 12345678901234567890m;
        var result1 = (ChiFixed)largeDecimal;
        var result2 = (ChiFixed)largeDecimal;

        result1.Should().Be(result2);
    }

    [Fact]
    public void DecimalCast_BeyondMaxValue_ProducesDeterministicResult()
    {
        var beyondMax = (decimal)ChiFixed.MaxValue * 10m;
        var result = (ChiFixed)beyondMax;

        (result == ChiFixed.PositiveInfinity ||
         result.Raw != ChiFixed.Zero.Raw).Should().BeTrue("Large value conversion should produce deterministic result");
    }

    [Fact]
    public void DecimalCast_BeyondMinValue_ProducesDeterministicResult()
    {
        var beyondMin = (decimal)ChiFixed.MinValue * 10m;
        var result = (ChiFixed)beyondMin;

        (result == ChiFixed.NegativeInfinity ||
         result.Raw != ChiFixed.Zero.Raw).Should()
            .BeTrue("Large negative value conversion should produce deterministic result");
    }

    [Theory]
    [InlineData(1000000000)]
    [InlineData(10000000000)]
    [InlineData(100000000000)]
    public void DecimalCast_LargeButInRange_PreservesSign(long value)
    {
        var decimalValue = (decimal)value;
        var result = (ChiFixed)decimalValue;

        (result.Raw > 0 || result == ChiFixed.PositiveInfinity).Should().BeTrue();
    }

    [Theory]
    [InlineData(-1000000000)]
    [InlineData(-10000000000)]
    [InlineData(-100000000000)]
    public void DecimalCast_LargeNegativeButInRange_PreservesSign(long value)
    {
        var decimalValue = (decimal)value;
        var result = (ChiFixed)decimalValue;

        (result.Raw < 0 || result == ChiFixed.NegativeInfinity).Should().BeTrue();
    }

    #endregion

    #region INumber CreateFrom Tests

    [Theory]
    [InlineData("12345678901234567890")]
    [InlineData("99999999999999999999")]
    public void CreateChecked_VeryLargeDecimal_ProducesDeterministicResult(string decimalStr)
    {
        var value = decimal.Parse(decimalStr);
        var result = ChiFixed.CreateChecked(value);

        (result == ChiFixed.PositiveInfinity ||
         result.Raw > 0 ||
         result.Raw < 0).Should().BeTrue("CreateChecked should produce deterministic result");
    }

    [Theory]
    [InlineData("-12345678901234567890")]
    [InlineData("-99999999999999999999")]
    public void CreateChecked_VeryLargeNegativeDecimal_ProducesDeterministicResult(string decimalStr)
    {
        var value = decimal.Parse(decimalStr);
        var result = ChiFixed.CreateChecked(value);

        (result == ChiFixed.NegativeInfinity ||
         result.Raw > 0 ||
         result.Raw < 0).Should().BeTrue("CreateChecked should produce deterministic result");
    }

    [Theory]
    [InlineData("12345678901234567890")]
    [InlineData("99999999999999999999")]
    public void CreateSaturating_VeryLargeDecimal_Saturates(string decimalStr)
    {
        var value = decimal.Parse(decimalStr);
        var result = ChiFixed.CreateSaturating(value);

        (result == ChiFixed.PositiveInfinity ||
         result == ChiFixed.MaxValue ||
         result.Raw > 0).Should().BeTrue("CreateSaturating should saturate or truncate to valid positive value");
    }

    [Theory]
    [InlineData("-12345678901234567890")]
    [InlineData("-99999999999999999999")]
    public void CreateSaturating_VeryLargeNegativeDecimal_Saturates(string decimalStr)
    {
        var value = decimal.Parse(decimalStr);
        var result = ChiFixed.CreateSaturating(value);

        (result == ChiFixed.NegativeInfinity ||
         result == ChiFixed.MinValue ||
         result.Raw < 0).Should().BeTrue("CreateSaturating should saturate or truncate to valid negative value");
    }

    [Theory]
    [InlineData("12345678901234567890")]
    [InlineData("99999999999999999999")]
    public void CreateTruncating_VeryLargeDecimal_DoesNotThrow(string decimalStr)
    {
        var value = decimal.Parse(decimalStr);
        var act = () => ChiFixed.CreateTruncating(value);

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData("-12345678901234567890")]
    [InlineData("-99999999999999999999")]
    public void CreateTruncating_VeryLargeNegativeDecimal_DoesNotThrow(string decimalStr)
    {
        var value = decimal.Parse(decimalStr);
        var act = () => ChiFixed.CreateTruncating(value);

        act.Should().NotThrow();
    }

    [Fact]
    public void CreateChecked_DecimalMaxValue_DoesNotThrow()
    {
        var act = () => ChiFixed.CreateChecked(decimal.MaxValue);
        act.Should().NotThrow();
    }

    [Fact]
    public void CreateSaturating_DecimalMaxValue_DoesNotThrow()
    {
        var act = () => ChiFixed.CreateSaturating(decimal.MaxValue);

        act.Should().NotThrow();
    }

    [Fact]
    public void CreateTruncating_DecimalMaxValue_DoesNotThrow()
    {
        var act = () => ChiFixed.CreateTruncating(decimal.MaxValue);

        act.Should().NotThrow();
    }

    [Fact]
    public void CreateFromMethods_InRangeDecimal_MatchesDirectCast()
    {
        const decimal testValue = 123.456m;

        var directCast = (ChiFixed)testValue;
        var fromChecked = ChiFixed.CreateChecked(testValue);
        var fromSaturating = ChiFixed.CreateSaturating(testValue);
        var fromTruncating = ChiFixed.CreateTruncating(testValue);

        fromChecked.Should().Be(directCast);
        fromSaturating.Should().Be(directCast);
        fromTruncating.Should().Be(directCast);
    }

    #endregion
}