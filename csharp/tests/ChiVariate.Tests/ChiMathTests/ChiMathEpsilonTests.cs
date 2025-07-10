using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiMathTests;

public class ChiMathEpsilonTests
{
    [Fact]
    public void Epsilon_Double_ShouldReturnAppropriateValue()
    {
        // Act
        var epsilon = ChiMath.Const<double>.Epsilon;

        // Assert
        epsilon.Should().Be(1e-14);
    }

    [Fact]
    public void Epsilon_Float_ShouldReturnAppropriateValue()
    {
        // Act
        var epsilon = ChiMath.Const<float>.Epsilon;

        // Assert
        epsilon.Should().Be(1e-6f);
    }

    [Fact]
    public void Epsilon_Decimal_ShouldReturnAppropriateValue()
    {
        // Act
        var epsilon = ChiMath.Const<decimal>.Epsilon;

        // Assert
        epsilon.Should().Be(1e-27m);
    }

    [Theory]
    [InlineData(typeof(double))]
    [InlineData(typeof(float))]
    [InlineData(typeof(decimal))]
    public void Epsilon_SupportedTypes_ShouldBePositive(Type floatingPointType)
    {
        // Arrange
        var method = typeof(ChiMath.Const<>)
            .MakeGenericType(floatingPointType)
            .GetProperty("Epsilon");
        var epsilon = method!.GetValue(null);

        // Act & Assert
        epsilon.Should().NotBeNull();
        Convert.ToDouble(epsilon).Should().BeGreaterThan(0.0);
    }
}