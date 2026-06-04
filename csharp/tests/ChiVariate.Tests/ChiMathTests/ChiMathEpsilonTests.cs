using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiMathTests;

public class ChiMathEpsilonTests
{
    [Fact]
    public void Epsilon_ChiFixed_ReturnsExpectedValue()
    {
        var epsilon = ChiMath.Const<ChiFixed>.Epsilon;

        epsilon.Should().Be((ChiFixed)1e-12m);
    }

    [Fact]
    public void Epsilon_Double_ReturnsExpectedValue()
    {
        var epsilon = ChiMath.Const<double>.Epsilon;

        epsilon.Should().Be(1e-14);
    }

    [Fact]
    public void Epsilon_Float_ReturnsExpectedValue()
    {
        var epsilon = ChiMath.Const<float>.Epsilon;

        epsilon.Should().Be(1e-6f);
    }

    [Fact]
    public void Epsilon_Decimal_ReturnsExpectedValue()
    {
        var epsilon = ChiMath.Const<decimal>.Epsilon;

        epsilon.Should().Be(1e-27m);
    }
}