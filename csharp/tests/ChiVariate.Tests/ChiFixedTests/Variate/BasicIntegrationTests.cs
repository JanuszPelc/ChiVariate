using AwesomeAssertions;
using ChiVariate.Providers;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Variate;

public class BasicIntegrationTests
{
    [Fact]
    public void Next_Default_ReturnsValueInUnitInterval()
    {
        var rng = new ChiRng(42);

        var sample = ChiRealProvider.Next<ChiRng, ChiFixed>(ref rng);

        (sample >= ChiFixed.Zero).Should().BeTrue();
        (sample < ChiFixed.One).Should().BeTrue();
    }

    [Fact]
    public void Next_SuccessiveCalls_ReturnsDistinctValues()
    {
        var rng = new ChiRng(42);

        var sample1 = ChiRealProvider.Next<ChiRng, ChiFixed>(ref rng);
        var sample2 = ChiRealProvider.Next<ChiRng, ChiFixed>(ref rng);
        var sample3 = ChiRealProvider.Next<ChiRng, ChiFixed>(ref rng);

        sample2.Should().NotBe(sample1);
        sample3.Should().NotBe(sample2);
        sample3.Should().NotBe(sample1);
    }

    [Fact]
    public void Next_SameSeed_ReturnsIdenticalSequence()
    {
        var rng1 = new ChiRng(12345);
        var rng2 = new ChiRng(12345);

        for (var i = 0; i < 100; i++)
        {
            var sample1 = ChiRealProvider.Next<ChiRng, ChiFixed>(ref rng1);
            var sample2 = ChiRealProvider.Next<ChiRng, ChiFixed>(ref rng2);

            sample2.Should().Be(sample1);
        }
    }

    [Fact]
    public void Next_ExcludeMin_NeverReturnsZero()
    {
        var rng = new ChiRng(42);

        for (var i = 0; i < 10_000; i++)
        {
            var sample = ChiRealProvider.Next<ChiRng, ChiFixed>(ref rng, ChiIntervalOptions.ExcludeMin);

            (sample > ChiFixed.Zero).Should().BeTrue();
            (sample < ChiFixed.One).Should().BeTrue();
        }
    }

    [Fact]
    public void Next_OverManySamples_PopulatesLowFractionalBits()
    {
        var rng = new ChiRng(42);

        var foundSmallFractional = false;
        for (var i = 0; i < 10_000 && !foundSmallFractional; i++)
        {
            var sample = ChiRealProvider.Next<ChiRng, ChiFixed>(ref rng);
            var lowerBits = sample.Raw & 0x3FF;
            if (lowerBits != 0 && sample.Raw < ChiFixed.ScaleFactor / 2)
                foundSmallFractional = true;
        }

        foundSmallFractional.Should().BeTrue();
    }
}