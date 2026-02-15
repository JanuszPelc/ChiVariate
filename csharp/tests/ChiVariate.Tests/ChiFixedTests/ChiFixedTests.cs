using AwesomeAssertions;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests;

public class ChiFixedTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Constants_Zero_HasValueZero()
    {
        ChiFixed.Zero.Raw.Should().Be(0L);
    }

    [Fact]
    public void Constants_One_HasCorrectValue()
    {
        ChiFixed.One.Raw.Should().Be(ChiFixed.ScaleFactor);
    }

    [Fact]
    public void Constants_NegativeOne_HasCorrectValue()
    {
        ChiFixed.NegativeOne.Should().Be(-(ChiFixed)1m);
    }

    [Fact]
    public void Constants_Epsilon_HasValueOne()
    {
        ChiFixed.Epsilon.Raw.Should().Be(1L);
    }

    [Fact]
    public void Constants_PositiveInfinity_IsMaxValue()
    {
        ChiFixed.PositiveInfinity.Raw.Should().Be(ChiFixed.MaxValue.Raw);
    }

    [Fact]
    public void Constants_NegativeInfinity_IsMinValue()
    {
        ChiFixed.NegativeInfinity.Raw.Should().Be(ChiFixed.MinValue.Raw);
    }

    [Fact]
    public void ToString_Zero_ReturnsZeroString()
    {
        var result = ChiFixed.Zero.ToString();

        result.Should().Be("0");
    }

    [Fact]
    public void ToString_One_ReturnsOneString()
    {
        var result = ChiFixed.One.ToString();

        result.Should().Be("1");
    }

    [Fact]
    public void ToString_WholeNumber_NoDecimalPoint()
    {
        var value = (ChiFixed)42m;

        var result = value.ToString();

        result.Should().Be("42");
    }

    [Fact]
    public void ToString_FractionalValue_ShowsDecimals()
    {
        var value = ChiFixed.Pi;

        var result = value.ToString();

        result.Should().StartWith("3.14159265");
    }

    [Fact]
    public void ToString_NegativeValue_HasMinusSign()
    {
        var value = (ChiFixed)(-42.5m);

        var result = value.ToString();

        result.Should().Be("-42.5");
    }

    [Fact]
    public void ToString_HighPrecision_ShowsAllSignificantDigits()
    {
        var value = (ChiFixed)0.123046875m;

        var result = value.ToString();

        result.Should().Be("0.123046875");
    }

    [Fact]
    public void ToString_RoundTrip_PreservesValue()
    {
        var original = (ChiFixed)12.5m;

        var str = original.ToString();
        var parsed = ChiFixed.Parse(str);

        parsed.Should().Be(original);
    }

    [Fact]
    public void Equality_StructEquality_UsesRawValue()
    {
        var a = new ChiFixed(12345);
        var b = new ChiFixed(12345);
        var c = new ChiFixed(54321);

        a.Equals(b).Should().BeTrue();
        a.Equals(c).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_EqualValues_SameHashCode()
    {
        var a = (ChiFixed)42m;
        var b = (ChiFixed)42m;

        b.GetHashCode().Should().Be(a.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentValues_LikelyDifferentHashCode()
    {
        var a = (ChiFixed)42m;
        var b = (ChiFixed)43m;

        b.GetHashCode().Should().NotBe(a.GetHashCode());
    }

    [Fact]
    public void RawProperty_Accessed_ReturnsUnderlyingValue()
    {
        const long rawValue = 12345678L;
        var value = new ChiFixed(rawValue);

        value.Raw.Should().Be(rawValue);
    }

    [Fact]
    public void Constructor_WithRawValue_StoresValueDirectly()
    {
        const long rawValue = ChiFixed.ScaleFactor * 10;
        var value = new ChiFixed(rawValue);

        value.Should().Be((ChiFixed)10m);
    }

    [Fact]
    public void Precision_VerySmallValue_RepresentableAboveEpsilon()
    {
        var epsilon = ChiFixed.Epsilon;
        var zero = ChiFixed.Zero;

        (epsilon > zero).Should().BeTrue();
        epsilon.Should().NotBe(zero);
    }

    [Fact]
    public void Precision_EpsilonAddition_IncrementsBySmallestAmount()
    {
        var value = (ChiFixed)1m;
        var incremented = new ChiFixed(value.Raw + 1);

        (incremented > value).Should().BeTrue();
        (incremented.Raw - value.Raw).Should().Be(1L);
    }

    [Fact]
    public void Integration_ParseAndToString_RoundTrip()
    {
        const string input = "12.5";

        var parsed = ChiFixed.Parse(input);
        var output = parsed.ToString();
        var reparsed = ChiFixed.Parse(output);

        reparsed.Should().Be(parsed);
    }

    [Fact]
    public void Integration_ArithmeticWithConstants_WorksCorrectly()
    {
        var result = ChiFixed.One + ChiFixed.One;

        result.Should().Be((ChiFixed)2m);
    }

    [Fact]
    public void Integration_ComplexExpression_ProducesExpectedResult()
    {
        var a = (ChiFixed)10.5m;
        var b = (ChiFixed)2.5m;
        var c = (ChiFixed)3m;

        var result = (a - b) * c + ChiFixed.One;

        result.Should().Be((ChiFixed)25m);
    }

    [Fact]
    public void FactoryMethods_EquivalentValues_AreProduced()
    {
        var fromDecimal = (ChiFixed)0.5m;
        var fromRational = (ChiFixed)(1, 2);
        var fromString = ChiFixed.Parse("0.5");

        fromRational.Should().Be(fromDecimal);
        fromString.Should().Be(fromRational);
        fromDecimal.Should().Be(fromString);
    }

    [Fact]
    public void ToString_OneSeventh_ShowsExpectedApproximation()
    {
        var oneSeventh = (ChiFixed)(1, 7);

        var result = oneSeventh.ToString();

        result.Should().StartWith("0.1428571");
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

        (maxError <= maxAcceptableError).Should().BeTrue(
            $"Max error {maxError} exceeds acceptable {maxAcceptableError}. Average: {averageError:F2}");
        (averageError <= maxAcceptableError / 2.0).Should().BeTrue(
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

            y.Raw.Should().Be(x.Raw);
        }
    }

    [Fact]
    public void WarmUp_Called_ReturnsPerTableTimings()
    {
        var timings = ChiFixed.WarmUp();

        timings.Should().NotBeEmpty();

        var total = TimeSpan.Zero;
        foreach (var (type, elapsed) in timings.OrderByDescending(kv => kv.Value))
        {
            testOutputHelper.WriteLine($"{type.Name,-20} {elapsed.TotalMilliseconds,8:F2} ms");
            total += elapsed;
        }

        testOutputHelper.WriteLine($"{"Total",-20} {total.TotalMilliseconds,8:F2} ms");
    }
}