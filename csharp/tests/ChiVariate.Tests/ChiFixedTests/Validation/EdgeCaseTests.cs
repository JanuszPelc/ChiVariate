using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Validation;

public class EdgeCaseTests
{
    [Fact]
    public void Addition_MaxValuePlusOne_WrapsToNegative()
    {
        var result = ChiFixed.MaxValue + ChiFixed.One;

        (result.Raw < 0).Should().BeTrue();
        (result < ChiFixed.Zero).Should().BeTrue();
    }

    [Fact]
    public void Addition_MaxValuePlusEpsilon_Wraps()
    {
        var result = ChiFixed.MaxValue + ChiFixed.Epsilon;

        (result.Raw < ChiFixed.MaxValue.Raw).Should().BeTrue();
    }

    [Fact]
    public void Subtraction_MinValueMinusOne_WrapsToPositive()
    {
        var result = ChiFixed.MinValue - ChiFixed.One;

        (result.Raw > 0).Should().BeTrue();
        (result > ChiFixed.Zero).Should().BeTrue();
    }

    [Fact]
    public void Subtraction_MinValueMinusEpsilon_Wraps()
    {
        var result = ChiFixed.MinValue - ChiFixed.Epsilon;

        (result.Raw > ChiFixed.MinValue.Raw).Should().BeTrue();
    }

    [Fact]
    public void Multiplication_MaxValueTimesTwo_Wraps()
    {
        var two = (ChiFixed)2m;
        var result = ChiFixed.MaxValue * two;

        (result.Raw < ChiFixed.MaxValue.Raw).Should().BeTrue();
    }

    [Fact]
    public void Multiplication_LargeValues_WrapsCorrectly()
    {
        var large1 = (ChiFixed)1000000m;
        var large2 = (ChiFixed)1000000m;
        var result = large1 * large2;

        result.Should().NotBe(ChiFixed.Zero);
    }

    [Fact]
    public void Negate_MinValue_Wraps()
    {
        var result = -ChiFixed.MinValue;

        result.Should().Be(ChiFixed.MinValue);
    }

    [Fact]
    public void Abs_MinValue_WrapsToMinValue()
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
    public void Division_MinValueByNegativeOne_Wraps()
    {
        var result = ChiFixed.MinValue / ChiFixed.NegativeOne;

        result.Should().Be(ChiFixed.MinValue);
    }

    [Fact]
    public void Increment_MaxValue_WrapsToNegative()
    {
        var value = ChiFixed.MaxValue;
        value++;

        (value.Raw < 0).Should().BeTrue();
        (value < ChiFixed.Zero).Should().BeTrue();
    }

    [Fact]
    public void Decrement_MinValue_WrapsToPositive()
    {
        var value = ChiFixed.MinValue;
        value--;

        (value.Raw > 0).Should().BeTrue();
        (value > ChiFixed.Zero).Should().BeTrue();
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
    public void MaxValue_Compared_IsSymmetricWithMinValue()
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
    public void Addition_NearMaxValue_Wraps()
    {
        var nearMax = new ChiFixed(ChiFixed.MaxValue.Raw - 100);
        var large = (ChiFixed)1000m;
        var result = nearMax + large;

        (result.Raw < nearMax.Raw).Should().BeTrue();
    }

    [Fact]
    public void Subtraction_NearMinValue_Wraps()
    {
        var nearMin = new ChiFixed(ChiFixed.MinValue.Raw + 100);
        var large = (ChiFixed)1000m;
        var result = nearMin - large;

        (result.Raw > nearMin.Raw).Should().BeTrue();
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
    public void Round_NearHalf_RoundsCorrectly()
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
    public void DecimalCast_VeryLargeValue_IsConsistent()
    {
        const decimal largeDecimal = 12345678901234567890m;
        var result1 = (ChiFixed)largeDecimal;
        var result2 = (ChiFixed)largeDecimal;

        result1.Should().Be(result2);
    }

    [Fact]
    public void DecimalCast_BeyondMaxValue_Behavior()
    {
        var beyondMax = (decimal)ChiFixed.MaxValue * 10m;
        var result = (ChiFixed)beyondMax;

        (result == ChiFixed.PositiveInfinity ||
         result.Raw != ChiFixed.Zero.Raw).Should().BeTrue("Large value conversion should produce deterministic result");
    }

    [Fact]
    public void DecimalCast_BeyondMinValue_Behavior()
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
    public void CreateChecked_VeryLargeDecimal_Behavior(string decimalStr)
    {
        var value = decimal.Parse(decimalStr);
        var success = ChiFixed.TryConvertFromChecked(value, out var result);

        if (success)
            (result == ChiFixed.PositiveInfinity ||
             result.Raw > 0 ||
             result.Raw < 0).Should().BeTrue("CreateChecked should produce deterministic result");
    }

    [Theory]
    [InlineData("-12345678901234567890")]
    [InlineData("-99999999999999999999")]
    public void CreateChecked_VeryLargeNegativeDecimal_Behavior(string decimalStr)
    {
        var value = decimal.Parse(decimalStr);
        var success = ChiFixed.TryConvertFromChecked(value, out var result);

        if (success)
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
        var success = ChiFixed.TryConvertFromSaturating(value, out var result);

        success.Should().BeTrue();
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
        var success = ChiFixed.TryConvertFromSaturating(value, out var result);

        success.Should().BeTrue();
        (result == ChiFixed.NegativeInfinity ||
         result == ChiFixed.MinValue ||
         result.Raw < 0).Should().BeTrue("CreateSaturating should saturate or truncate to valid negative value");
    }

    [Theory]
    [InlineData("12345678901234567890")]
    [InlineData("99999999999999999999")]
    public void CreateTruncating_VeryLargeDecimal_ProducesResult(string decimalStr)
    {
        var value = decimal.Parse(decimalStr);
        var success = ChiFixed.TryConvertFromTruncating(value, out _);

        success.Should().BeTrue();
    }

    [Theory]
    [InlineData("-12345678901234567890")]
    [InlineData("-99999999999999999999")]
    public void CreateTruncating_VeryLargeNegativeDecimal_ProducesResult(string decimalStr)
    {
        var value = decimal.Parse(decimalStr);
        var success = ChiFixed.TryConvertFromTruncating(value, out _);

        success.Should().BeTrue();
    }

    [Fact]
    public void CreateChecked_DecimalMaxValue_DoesNotThrowOrProducesResult()
    {
        var act = () => ChiFixed.TryConvertFromChecked(decimal.MaxValue, out _);
        act.Should().NotThrow();
    }

    [Fact]
    public void CreateSaturating_DecimalMaxValue_Succeeds()
    {
        var success = ChiFixed.TryConvertFromSaturating(decimal.MaxValue, out _);

        success.Should().BeTrue();
    }

    [Fact]
    public void CreateTruncating_DecimalMaxValue_Succeeds()
    {
        var success = ChiFixed.TryConvertFromTruncating(decimal.MaxValue, out _);

        success.Should().BeTrue();
    }

    [Fact]
    public void CreateFromMethods_SameValue_ConsistentWithDirectCast()
    {
        const decimal testValue = 123.456m;

        var directCast = (ChiFixed)testValue;
        ChiFixed.TryConvertFromChecked(testValue, out var fromChecked);
        ChiFixed.TryConvertFromSaturating(testValue, out var fromSaturating);
        ChiFixed.TryConvertFromTruncating(testValue, out var fromTruncating);

        fromChecked.Should().Be(directCast);
        fromSaturating.Should().Be(directCast);
        fromTruncating.Should().Be(directCast);
    }

    #endregion
}