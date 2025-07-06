using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Uniform;

public class NextTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 200_000;

    [Fact]
    public void NextDouble_WithBounds_ProducesUniformDistribution()
    {
        RunFloatingPointDistributionTest<double, BoundedDoubleSampler>(new BoundedDoubleSampler());
    }

    [Fact]
    public void NextSingle_WithBounds_ProducesUniformDistribution()
    {
        // Arrange
        var rng = new ChiRng(ChiHashObsolete.Hash($"{nameof(Single)}_bounded"));
        var histogram = new Histogram(0.0, 1.0, 10);
        var sampler = new BoundedSingleSampler();

        // Act
        histogram.Generate<float, ChiRng, BoundedSingleSampler>(ref rng, SampleCount, sampler);

        // Assert
        histogram.AssertIsUniform(0.05);
    }

    // ====================================================================
    // Test Helpers
    // ====================================================================

    private void RunIntegerDistributionTest<T, TSampler>(
        TSampler sampler, [CallerMemberName] string methodName = null!)
        where T : unmanaged, IBinaryInteger<T>
        where TSampler : IHistogramSamplerWithRange<T, ChiRng>
    {
        // Arrange
        var rng = new ChiRng(ChiHashObsolete.Hash(typeof(T).Name));
        var histogram = new Histogram(0.0, 1.0, 10);

        // Act
        histogram.Generate<T, ChiRng, TSampler>(ref rng, SampleCount, sampler);

        // Assert
        histogram.DebugPrint(testOutputHelper, methodName);
        histogram.AssertIsUniform(0.05);
    }

    private void RunFloatingPointDistributionTest<T, TSampler>(
        TSampler sampler, [CallerMemberName] string methodName = null!)
        where T : IFloatingPoint<T>
        where TSampler : IHistogramSamplerWithRange<T, ChiRng>
    {
        // Arrange
        var rng = new ChiRng(ChiHashObsolete.Hash(typeof(T).Name));
        var histogram = new Histogram(0.0, 1.0, 10);
        const int sampleCount = 100_000;

        // Act
        histogram.Generate<T, ChiRng, TSampler>(ref rng, sampleCount, sampler);

        // Assert
        histogram.DebugPrint(testOutputHelper, methodName);
        histogram.AssertIsUniform(0.05);
    }

    [Fact]
    public void Next_ForByte_ProducesUniformDistribution()
    {
        RunIntegerDistributionTest<byte, ByteSampler>(new ByteSampler());
    }

    [Fact]
    public void Next_ForInt_ProducesUniformDistribution()
    {
        RunIntegerDistributionTest<int, IntSampler>(new IntSampler());
    }

    [Fact]
    public void Next_ForLong_ProducesUniformDistribution()
    {
        RunIntegerDistributionTest<long, LongSampler>(new LongSampler());
    }

    [Fact]
    public void Next_ForInt128_ProducesUniformDistribution()
    {
        RunIntegerDistributionTest<Int128, Int128Sampler>(new Int128Sampler());
    }

    [Fact]
    public void Next_ForUShort_ProducesUniformDistribution()
    {
        RunIntegerDistributionTest<ushort, UShortSampler>(new UShortSampler());
    }

    [Fact]
    public void Next_ForUInt_ProducesUniformDistribution()
    {
        RunIntegerDistributionTest<uint, UIntSampler>(new UIntSampler());
    }

    [Fact]
    public void Next_ForULong_ProducesUniformDistribution()
    {
        RunIntegerDistributionTest<ulong, ULongSampler>(new ULongSampler());
    }

    private readonly struct ByteSampler : IHistogramSamplerWithRange<byte, ChiRng>
    {
        private const byte Min = 10, Max = 250;

        public byte NextSample(ref ChiRng rng)
        {
            return rng.Uniform(Min, Max).Sample();
        }

        public double Normalize(byte value)
        {
            return (double)(value - Min) / (Max - Min);
        }
    }

    private readonly struct IntSampler : IHistogramSamplerWithRange<int, ChiRng>
    {
        private const int Min = -500, Max = 500;

        public int NextSample(ref ChiRng rng)
        {
            return rng.Chance().Next(Min, Max);
        }

        public double Normalize(int value)
        {
            return (double)(value - Min) / (Max - Min);
        }
    }

    private readonly struct LongSampler : IHistogramSamplerWithRange<long, ChiRng>
    {
        private const long Min = long.MinValue / 2, Max = long.MaxValue / 2;

        public long NextSample(ref ChiRng rng)
        {
            return rng.Uniform(Min, Max).Sample();
        }

        public double Normalize(long value)
        {
            return ((double)value - Min) / ((double)Max - Min);
        }
    }

    private readonly struct Int128Sampler : IHistogramSamplerWithRange<Int128, ChiRng>
    {
        private static readonly Int128 Min = Int128.MaxValue / -2;
        private static readonly Int128 Max = Int128.MaxValue / 2;

        public Int128 NextSample(ref ChiRng rng)
        {
            return rng.Uniform(Min, Max).Sample();
        }

        public double Normalize(Int128 value)
        {
            return ((double)value - (double)Min) / ((double)Max - (double)Min);
        }
    }

    private readonly struct UShortSampler : IHistogramSamplerWithRange<ushort, ChiRng>
    {
        private const ushort Min = 1000, Max = 65000;

        public ushort NextSample(ref ChiRng rng)
        {
            return rng.Uniform(Min, Max).Sample();
        }

        public double Normalize(ushort value)
        {
            return (double)(value - Min) / (Max - Min);
        }
    }

    private readonly struct UIntSampler : IHistogramSamplerWithRange<uint, ChiRng>
    {
        private const uint Min = 1_000_000, Max = 4_000_000_000;

        public uint NextSample(ref ChiRng rng)
        {
            return rng.Uniform(Min, Max).Sample();
        }

        public double Normalize(uint value)
        {
            return ((double)value - Min) / ((double)Max - Min);
        }
    }

    private readonly struct ULongSampler : IHistogramSamplerWithRange<ulong, ChiRng>
    {
        private const ulong Min = ulong.MaxValue / 4, Max = ulong.MaxValue / 4 * 3;

        public ulong NextSample(ref ChiRng rng)
        {
            return rng.Uniform(Min, Max).Sample();
        }

        public double Normalize(ulong value)
        {
            return ((double)value - Min) / ((double)Max - Min);
        }
    }

    private readonly struct BoundedDoubleSampler : IHistogramSamplerWithRange<double, ChiRng>
    {
        private const double Min = -100.0, Max = 100.0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double NextSample(ref ChiRng rng)
        {
            return rng.Uniform(Min, Max).Sample();
        }

        public double Normalize(double value)
        {
            return (value - Min) / (Max - Min);
        }
    }

    private readonly struct BoundedSingleSampler : IHistogramSamplerWithRange<float, ChiRng>
    {
        private const float Min = -50.0f, Max = 50.0f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float NextSample(ref ChiRng rng)
        {
            return rng.Uniform(Min, Max).Sample();
        }

        public double Normalize(float value)
        {
            return ((double)value - Min) / (Max - Min);
        }
    }
}