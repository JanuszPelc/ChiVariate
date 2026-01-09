using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests;

public class ChiFixedTests
{
    [Fact]
    public void Constants_Zero_HasValueZero()
    {
        Assert.Equal(0L, ChiFixed.Zero.Raw);
    }

    [Fact]
    public void Constants_One_HasCorrectValue()
    {
        Assert.Equal(ChiFixed.ScaleFactor, ChiFixed.One.Raw);
    }

    [Fact]
    public void Constants_NegativeOne_HasCorrectValue()
    {
        Assert.Equal(-(ChiFixed)1m, ChiFixed.NegativeOne);
    }

    [Fact]
    public void Constants_Epsilon_HasValueOne()
    {
        Assert.Equal(1L, ChiFixed.Epsilon.Raw);
    }

    [Fact]
    public void Constants_PositiveInfinity_IsMaxValue()
    {
        Assert.Equal(ChiFixed.MaxValue.Raw, ChiFixed.PositiveInfinity.Raw);
    }

    [Fact]
    public void Constants_NegativeInfinity_IsMinValue()
    {
        Assert.Equal(ChiFixed.MinValue.Raw, ChiFixed.NegativeInfinity.Raw);
    }

    [Fact]
    public void ToString_Zero_ReturnsZeroString()
    {
        var result = ChiFixed.Zero.ToString();

        Assert.Equal("0", result);
    }

    [Fact]
    public void ToString_One_ReturnsOneString()
    {
        var result = ChiFixed.One.ToString();

        Assert.Equal("1", result);
    }

    [Fact]
    public void ToString_WholeNumber_NoDecimalPoint()
    {
        var value = (ChiFixed)42m;

        var result = value.ToString();

        Assert.Equal("42", result);
    }

    [Fact]
    public void ToString_FractionalValue_ShowsDecimals()
    {
        var value = ChiFixed.Pi;

        var result = value.ToString();

        Assert.StartsWith("3.1415926535898", result);
    }

    [Fact]
    public void ToString_NegativeValue_HasMinusSign()
    {
        var value = (ChiFixed)(-42.5m);

        var result = value.ToString();

        Assert.Equal("-42.5", result);
    }

    [Fact]
    public void ToString_HighPrecision_ShowsAllSignificantDigits()
    {
        var value = (ChiFixed)0.123046875m;

        var result = value.ToString();

        Assert.Equal("0.123046875", result);
    }

    [Fact]
    public void ToString_RoundTrip_PreservesValue()
    {
        var original = (ChiFixed)12.5m;

        var str = original.ToString();
        var parsed = ChiFixed.Parse(str);

        Assert.Equal(original, parsed);
    }

    [Fact]
    public void Equality_StructEquality_UsesRawValue()
    {
        var a = new ChiFixed(12345);
        var b = new ChiFixed(12345);
        var c = new ChiFixed(54321);

        Assert.True(a.Equals(b));
        Assert.False(a.Equals(c));
    }

    [Fact]
    public void GetHashCode_EqualValues_SameHashCode()
    {
        var a = (ChiFixed)42m;
        var b = (ChiFixed)42m;

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentValues_LikelyDifferentHashCode()
    {
        var a = (ChiFixed)42m;
        var b = (ChiFixed)43m;

        Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void RawProperty_Accessed_ReturnsUnderlyingValue()
    {
        const long rawValue = 12345678L;
        var value = new ChiFixed(rawValue);

        Assert.Equal(rawValue, value.Raw);
    }

    [Fact]
    public void Constructor_WithRawValue_StoresValueDirectly()
    {
        const long rawValue = ChiFixed.ScaleFactor * 10;
        var value = new ChiFixed(rawValue);

        Assert.Equal((ChiFixed)10m, value);
    }

    [Fact]
    public void Precision_VerySmallValue_RepresentableAboveEpsilon()
    {
        var epsilon = ChiFixed.Epsilon;
        var zero = ChiFixed.Zero;

        Assert.True(epsilon > zero);
        Assert.NotEqual(epsilon, zero);
    }

    [Fact]
    public void Precision_EpsilonAddition_IncrementsBySmallestAmount()
    {
        var value = (ChiFixed)1m;
        var incremented = new ChiFixed(value.Raw + 1);

        Assert.True(incremented > value);
        Assert.Equal(1L, incremented.Raw - value.Raw);
    }

    [Fact]
    public void Integration_ParseAndToString_RoundTrip()
    {
        const string input = "12.5";

        var parsed = ChiFixed.Parse(input);
        var output = parsed.ToString();
        var reparsed = ChiFixed.Parse(output);

        Assert.Equal(parsed, reparsed);
    }

    [Fact]
    public void Integration_ArithmeticWithConstants_WorksCorrectly()
    {
        var result = ChiFixed.One + ChiFixed.One;

        Assert.Equal((ChiFixed)2m, result);
    }

    [Fact]
    public void Integration_ComplexExpression_ProducesExpectedResult()
    {
        var a = (ChiFixed)10.5m;
        var b = (ChiFixed)2.5m;
        var c = (ChiFixed)3m;

        var result = (a - b) * c + ChiFixed.One;

        Assert.Equal((ChiFixed)25m, result);
    }

    [Fact]
    public void FactoryMethods_EquivalentValues_AreProduced()
    {
        var fromDecimal = (ChiFixed)0.5m;
        var fromRational = (ChiFixed)(1, 2);
        var fromString = ChiFixed.Parse("0.5");

        Assert.Equal(fromDecimal, fromRational);
        Assert.Equal(fromRational, fromString);
        Assert.Equal(fromString, fromDecimal);
    }

    [Fact]
    public void ToString_OneSeventh_ShowsExpectedApproximation()
    {
        var oneSeventh = (ChiFixed)(1, 7);

        var result = oneSeventh.ToString();

        Assert.Equal("0.1428571428571", result);
    }

    [Fact]
    public void ToString_RoundTrip_PreservesRationalValues()
    {
        const int maxValue = 1_000;
        const int maxAcceptableError = 0;

        long maxError = 0;
        long totalError = 0;
        var count = 0;

        for (var numerator = 1; numerator <= maxValue; numerator++)
        for (var denominator = 1; denominator <= maxValue; denominator++)
        {
            var original = (ChiFixed)(numerator, denominator);
            var str = original.ToString();
            var parsed = ChiFixed.Parse(str);

            var error = Math.Abs(original.Raw - parsed.Raw);
            maxError = Math.Max(maxError, error);
            totalError += error;
            count++;
        }

        var averageError = (double)totalError / count;

        Assert.True(maxError <= maxAcceptableError,
            $"Max error {maxError} exceeds acceptable {maxAcceptableError}. Average: {averageError:F2}");
        Assert.True(averageError <= maxAcceptableError / 2.0,
            $"Average error {averageError:F2} exceeds acceptable {maxAcceptableError / 2.0:F2}");
    }

    [Fact]
    public void ToString_RoundTrip_PreservesRandomValues()
    {
        var rng = new Random(123);

        for (var i = 0; i < 100_000; i++)
        {
            var raw = ((long)rng.Next() << 32) ^ rng.Next();
            raw = Math.Clamp(raw, ChiFixed.MinValue.Raw, ChiFixed.MaxValue.Raw);

            var x = new ChiFixed(raw);
            var s = x.ToString();
            var y = ChiFixed.Parse(s);

            Assert.Equal(x.Raw, y.Raw);
        }
    }
}