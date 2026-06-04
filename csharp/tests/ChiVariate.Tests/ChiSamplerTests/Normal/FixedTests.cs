using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Normal;

public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Fact]
    public void Sample_StandardNormal_ProducesNormalDistribution()
    {
        var mean = ChiFixed.Zero;
        var stdDev = ChiFixed.One;

        var rng = new ChiRng(42);
        var histogram = new Histogram(-4.0, 4.0, 100);
        var sampler = new ChiFixedNormalSampler(mean, stdDev);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedNormalSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, $"Normal(μ={mean}, σ={stdDev})");
        histogram.AssertIsNormal(0.0, 1.0, 0.02);
    }

    [Fact]
    public void Sample_ShiftedDistribution_IsCenteredOnMean()
    {
        var mean = (ChiFixed)50;
        var stdDev = ChiFixed.One;

        var rng = new ChiRng("ShiftedNormalFixed");
        var histogram = new Histogram(45.0, 55.0, 100);
        var sampler = new ChiFixedNormalSampler(mean, stdDev);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedNormalSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, $"Normal(μ={mean}, σ={stdDev})");
        histogram.AssertIsNormal(50.0, 1.0, 0.02);
    }

    [Fact]
    public void Sample_WiderDistribution_MatchesStandardDeviation()
    {
        var mean = ChiFixed.Zero;
        var stdDev = (ChiFixed)15;

        var rng = new ChiRng("WiderNormalFixed");
        var histogram = new Histogram(-60.0, 60.0, 100);
        var sampler = new ChiFixedNormalSampler(mean, stdDev);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedNormalSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, $"Normal(μ={mean}, σ={stdDev})");
        histogram.AssertIsNormal(0.0, 15.0, 0.03);
    }

    private readonly struct ChiFixedNormalSampler(ChiFixed mean, ChiFixed stdDev) :
        IHistogramSamplerWithRange<ChiFixed, ChiRng>
    {
        public ChiFixed NextSample(ref ChiRng rng)
        {
            return rng.Normal(mean, stdDev).Sample();
        }

        public double Normalize(ChiFixed value)
        {
            return double.CreateChecked(value);
        }
    }
}