using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Normal;

public class SmokeTests
{
    private const int Seed = 1337;

    [Fact]
    public void Next_DefaultDouble_ReturnsExpectedValue()
    {
        // Arrange
        var rng = new ChiRng(Seed);

        // Act
        var result = rng.Normal(0.0, 1.0).Sample();

        // Assert
        result.Should().BeApproximately(-0.13411, 0.00001);
    }

    [Fact]
    public void Next_ConfiguredFloat_ReturnsExpectedValue()
    {
        // Arrange
        var rng = new ChiRng(Seed);

        // Act
        var result = rng.Normal(10.0f, 5.0f).Sample();

        // Assert
        result.Should().BeApproximately(7.523f, 0.001f);
    }

    [Fact]
    public void Normal_WithZeroStdDev_ThrowsArgumentOutOfRangeException()
    {
        var rng = new ChiRng(Seed);

        try
        {
            _ = rng.Normal(0.0, 0.0);
            throw new Exception("Expected ArgumentOutOfRangeException was not thrown");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            ex.ParamName.Should().Be("standardDeviation");
        }
    }

    [Fact]
    public void Normal_WithNegativeStdDev_ThrowsArgumentOutOfRangeException()
    {
        var rng = new ChiRng(Seed);

        try
        {
            _ = rng.Normal(0.0, -1.0);
            throw new Exception("Expected ArgumentOutOfRangeException was not thrown");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            ex.ParamName.Should().Be("standardDeviation");
        }
    }
}