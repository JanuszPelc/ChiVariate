using ChiVariate.Providers;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Variate;

public class BasicIntegrationTests
{
    [Fact]
    public void ChiRealProvider_NextChiFixed_ProducesValueInUnitInterval()
    {
        var rng = new ChiRng(42);

        var sample = ChiRealProvider.Next<ChiRng, ChiFixed>(ref rng);

        Assert.True(sample >= ChiFixed.Zero);
        Assert.True(sample < ChiFixed.One);
    }

    [Fact]
    public void ChiRealProvider_NextChiFixed_ProducesDistinctValues()
    {
        var rng = new ChiRng(42);

        var sample1 = ChiRealProvider.Next<ChiRng, ChiFixed>(ref rng);
        var sample2 = ChiRealProvider.Next<ChiRng, ChiFixed>(ref rng);
        var sample3 = ChiRealProvider.Next<ChiRng, ChiFixed>(ref rng);

        Assert.NotEqual(sample1, sample2);
        Assert.NotEqual(sample2, sample3);
        Assert.NotEqual(sample1, sample3);
    }

    [Fact]
    public void ChiRealProvider_NextChiFixed_IsDeterministic()
    {
        var rng1 = new ChiRng(12345);
        var rng2 = new ChiRng(12345);

        for (var i = 0; i < 100; i++)
        {
            var sample1 = ChiRealProvider.Next<ChiRng, ChiFixed>(ref rng1);
            var sample2 = ChiRealProvider.Next<ChiRng, ChiFixed>(ref rng2);

            Assert.Equal(sample1, sample2);
        }
    }

    [Fact]
    public void ChiRealProvider_NextChiFixed_ExcludeMin_NeverReturnsZero()
    {
        var rng = new ChiRng(42);

        for (var i = 0; i < 10_000; i++)
        {
            var sample = ChiRealProvider.Next<ChiRng, ChiFixed>(ref rng, ChiIntervalOptions.ExcludeMin);

            Assert.True(sample > ChiFixed.Zero);
            Assert.True(sample < ChiFixed.One);
        }
    }

    [Fact]
    public void ChiRealProvider_NextChiFixed_UsesBitwiseInjection()
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

        Assert.True(foundSmallFractional);
    }
}