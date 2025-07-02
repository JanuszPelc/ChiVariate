using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChiVariate.Generators;

/// <summary>
///     Provides centralized methods for generating statistically robust random integers.
/// </summary>
/// <remarks>
///     This class is the single source of truth for converting raw, uniform bits from an
///     <see cref="IChiRngSource{TRng}" />
///     into bounded integers, using techniques that avoid common statistical biases (e.g., modulo bias).
/// </remarks>
public static class ChiIntegerGenerator
{
    /// <summary>
    ///     Returns a random 32-bit integer that is within a specified range.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Next<TRng>(ref TRng rng, int minInclusive, int maxExclusive)
        where TRng : struct, IChiRngSource<TRng>
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(minInclusive, maxExclusive);

        var sample = TRng.NextUInt32(ref rng);
        var range = unchecked((uint)((long)maxExclusive - minInclusive));

        // A fast path for the common case of sampling from the full positive range.
        var isFullPositiveRange = (minInclusive | (maxExclusive ^ int.MaxValue)) == 0;
        if (isFullPositiveRange)
        {
            while (sample == int.MaxValue) // Exclude int.MaxValue
                sample = TRng.NextUInt32(ref rng);

            return (int)sample;
        }

        // A fast path for ranges that are a power of two.
        if (uint.IsPow2(range))
        {
            var shiftAmount = 1 + BitOperations.LeadingZeroCount(range);
            return shiftAmount < 32
                ? minInclusive + (int)(sample >> shiftAmount)
                : minInclusive;
        }

        // The general case: rejection sampling to avoid modulo bias.
        var threshold = uint.MaxValue - uint.MaxValue % range;
        while (sample >= threshold)
            sample = TRng.NextUInt32(ref rng);

        return minInclusive + (int)(sample % range);
    }

    /// <summary>
    ///     Returns a random integer of a specified generic type that is within a specified range.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Next<TRng, T>(ref TRng rng, T minInclusive, T maxExclusive)
        where TRng : struct, IChiRngSource<TRng>
        where T : IBinaryInteger<T>, IMinMaxValue<T>
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(minInclusive, maxExclusive);

        var bitCount = LocalCache.Number<T>.BitCount;

        switch (bitCount)
        {
            case <= 32:
            {
                var range = uint.CreateTruncating(unchecked(maxExclusive - minInclusive));
                if (uint.IsPow2(range))
                {
                    var bitsNeeded = int.CreateChecked(uint.PopCount(range - 1));
                    var sample = T.CreateTruncating(NextBits<TRng, uint>(ref rng, bitsNeeded));
                    return minInclusive + sample;
                }

                var sample32 = TRng.NextUInt32(ref rng);
                var threshold = uint.MaxValue - uint.MaxValue % range;
                while (sample32 >= threshold)
                    sample32 = TRng.NextUInt32(ref rng);

                return minInclusive + T.CreateTruncating(sample32 % range);
            }
            case <= 64:
            {
                var range = ulong.CreateTruncating(unchecked(maxExclusive - minInclusive));
                if (ulong.IsPow2(range))
                {
                    var bitsNeeded = int.CreateChecked(ulong.PopCount(range - 1));
                    var sample = T.CreateTruncating(NextBits<TRng, ulong>(ref rng, bitsNeeded));
                    return minInclusive + sample;
                }

                var sample64 = TRng.NextUInt64(ref rng);
                var threshold = ulong.MaxValue - ulong.MaxValue % range;
                while (sample64 >= threshold)
                    sample64 = TRng.NextUInt64(ref rng);

                return minInclusive + T.CreateTruncating(sample64 % range);
            }
            default:
            {
                var range = UInt128.CreateTruncating(unchecked(maxExclusive - minInclusive));
                if (UInt128.IsPow2(range))
                {
                    var bitsNeeded = int.CreateChecked(UInt128.PopCount(range - 1));
                    var sample = T.CreateTruncating(NextBits<TRng, UInt128>(ref rng, bitsNeeded));
                    return minInclusive + sample;
                }

                var sample128 = ((UInt128)TRng.NextUInt64(ref rng) << 64) | TRng.NextUInt64(ref rng);
                var threshold = UInt128.MaxValue - UInt128.MaxValue % range;
                while (sample128 >= threshold)
                    sample128 = ((UInt128)TRng.NextUInt64(ref rng) << 64) | TRng.NextUInt64(ref rng);

                return minInclusive + T.CreateTruncating(sample128 % range);
            }
        }
    }

    /// <summary>
    ///     Returns an integer containing a specified number of random bits from the most significant side.
    ///     This is a private helper for power-of-two optimizations.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T NextBits<TRng, T>(ref TRng rng, int bitCount)
        where TRng : struct, IChiRngSource<TRng>
        where T : unmanaged, IBinaryInteger<T>
    {
        switch (bitCount)
        {
            case > 0 and <= 32:
            {
                const int availableBits = 32;
                var sample = TRng.NextUInt32(ref rng);
                var shift = availableBits - bitCount;
                return T.CreateChecked(sample >> shift);
            }
            case > 32 and <= 64:
            {
                const int availableBits = 64;
                var sample = TRng.NextUInt64(ref rng);
                var shift = availableBits - bitCount;
                return T.CreateChecked(sample >> shift);
            }
            case > 64 and <= 128:
            {
                const int availableBits = 128;
                var sample = ((UInt128)TRng.NextUInt64(ref rng) << 64) | TRng.NextUInt64(ref rng);
                var shift = availableBits - bitCount;
                return T.CreateChecked(sample >> shift);
            }
            default:
                throw new UnreachableException();
        }
    }
}

internal static class LocalCache
{
    public static class Number<T>
        where T : IBinaryInteger<T>
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Lazy<int> LazyBitCount = new(() =>
            int.CreateChecked(T.PopCount(T.AllBitsSet))
        );

        public static int BitCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => LazyBitCount.Value;
        }
    }
}