// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChiVariate.Providers;

/// <summary>
///     Provides methods for generating high-quality floating-point values from an integer-based PRNG source.
/// </summary>
/// <remarks>
///     <para>
///         This class uses precision-aware mantissa bit-filling to create uniformly distributed floating-point numbers.
///         This avoids the precision loss and potential biases common in simpler conversion methods (like division by
///         MaxValue).
///     </para>
///     <para>
///         <b>Why not just divide by MaxValue?</b>
///         Naive conversion like <c>randomInt / (double)uint.MaxValue</c> has issues:
///         <list type="bullet">
///             <item>Loses precision: only uses 32 bits even though double has 53-bit mantissa</item>
///             <item>Non-uniform distribution: some floating-point values are more likely than others</item>
///             <item>Includes rounding artifacts from the division</item>
///         </list>
///     </para>
///     <para>
///         <b>Mantissa bit-filling approach:</b>
///         Instead, we directly fill the mantissa bits with random data:
///         <list type="bullet">
///             <item>float: 24 random bits (23-bit mantissa + 1 implicit bit)</item>
///             <item>double: 53 random bits (52-bit mantissa + 1 implicit bit)</item>
///             <item>ChiFixed: random bits directly into the fractional part</item>
///         </list>
///         Then scale by the appropriate power of 2 to get [0, 1).
///     </para>
/// </remarks>
public static class ChiRealProvider
{
    /// <summary>
    ///     Generates a random floating-point number of a generic type <typeparamref name="T" />, dispatching to the
    ///     appropriate high-quality sampler.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Next<TRng, T>(ref TRng rng, ChiIntervalOptions options = ChiIntervalOptions.None)
        where T : IFloatingPoint<T> where TRng : struct, IChiRngSource<TRng>
    {
        if (typeof(T) == typeof(float))
        {
            var sample = NextSingle(ref rng, options);
            return Unsafe.As<float, T>(ref sample);
        }

        if (typeof(T) == typeof(Half))
        {
            var sample = NextHalf(ref rng, options);
            return Unsafe.As<Half, T>(ref sample);
        }

        if (typeof(T) == typeof(double))
        {
            var sample = NextDouble(ref rng, options);
            return Unsafe.As<double, T>(ref sample);
        }

        if (typeof(T) == typeof(decimal))
        {
            var sample = NextDecimal(ref rng, options);
            return Unsafe.As<decimal, T>(ref sample);
        }

        if (typeof(T) == typeof(ChiFixed))
        {
            var sample = NextChiFixed(ref rng, options);
            return Unsafe.As<ChiFixed, T>(ref sample);
        }

        // Fallback for other IFloatingPoint types (e.g., custom number types)
        return Unsafe.SizeOf<T>() <= 4
            ? T.CreateChecked(NextSingle(ref rng, options))
            : T.CreateChecked(NextDouble(ref rng, options));
    }

    /// <summary>
    ///     Generates a random <see cref="float" /> using the upper bits of a 32-bit random integer.
    /// </summary>
    /// <remarks>
    ///     Uses the upper 24 bits of a 32-bit random integer to fill the mantissa of the float,
    ///     providing full precision for the [0.0, 1.0) range.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float NextSingle<TRng>(ref TRng rng, ChiIntervalOptions options = ChiIntervalOptions.None)
        where TRng : struct, IChiRngSource<TRng>
    {
        const int targetPrecision = 24; // Single-precision float has a 23-bit mantissa + 1 implicit bit.
        const uint maxSample = 1u << targetPrecision;
        const int shiftAmount = 32 - targetPrecision;

        var sample = TRng.NextUInt32(ref rng) >> shiftAmount;

        if (options.HasFlag(ChiIntervalOptions.ExcludeMin))
            while (sample == 0)
                sample = TRng.NextUInt32(ref rng) >> shiftAmount;

        var scalingFactor = options.HasFlag(ChiIntervalOptions.IncludeMax)
            ? 1.0f / (maxSample - 1)
            : 1.0f / maxSample;

        return sample * scalingFactor;
    }

    /// <summary>
    ///     Generates a random <see cref="double" /> using the upper bits of a 64-bit random integer.
    /// </summary>
    /// <remarks>
    ///     Uses the upper 53 bits of a 64-bit random integer to fill the mantissa of the double,
    ///     providing full precision for the [0.0, 1.0) range.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double NextDouble<TRng>(ref TRng rng, ChiIntervalOptions options = ChiIntervalOptions.None)
        where TRng : struct, IChiRngSource<TRng>
    {
        const int targetPrecision = 53; // Double-precision float has a 52-bit mantissa + 1 implicit bit.
        const ulong maxSample = 1ul << targetPrecision;
        const int shiftAmount = 64 - targetPrecision;

        var sample = TRng.NextUInt64(ref rng) >> shiftAmount;

        if (options.HasFlag(ChiIntervalOptions.ExcludeMin))
            while (sample == 0)
                sample = TRng.NextUInt64(ref rng) >> shiftAmount;

        var scalingFactor = options.HasFlag(ChiIntervalOptions.IncludeMax)
            ? 1.0 / (maxSample - 1)
            : 1.0 / maxSample;

        return sample * scalingFactor;
    }

    /// <summary>
    ///     Generates a random <see cref="Half" /> using the upper bits of a 32-bit random integer.
    /// </summary>
    /// <remarks>
    ///     Uses the upper 11 bits of a 32-bit random integer to fill the mantissa of the half-precision float.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Half NextHalf<TRng>(ref TRng rng, ChiIntervalOptions options = ChiIntervalOptions.None)
        where TRng : struct, IChiRngSource<TRng>
    {
        const int targetPrecision = 11; // Half-precision float has a 10-bit mantissa + 1 implicit bit.
        const uint maxSample = 1u << targetPrecision;
        const int shiftAmount = 32 - targetPrecision;

        var sample = TRng.NextUInt32(ref rng) >> shiftAmount;

        if (options.HasFlag(ChiIntervalOptions.ExcludeMin))
            while (sample == 0)
                sample = TRng.NextUInt32(ref rng) >> shiftAmount;

        var scalingFactor = options.HasFlag(ChiIntervalOptions.IncludeMax)
            ? 1.0f / (maxSample - 1)
            : 1.0f / maxSample;

        return (Half)(sample * scalingFactor);
    }

    /// <summary>
    ///     Generates a random <see cref="decimal" /> in the range [0.0, 1.0].
    /// </summary>
    /// <remarks>
    ///     Constructs a decimal from 96 random bits and sets the scale to 28 to create a value between 0 and ~7.92.
    ///     This value is then normalized to the [0, 1] range.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static decimal NextDecimal<TRng>(ref TRng rng, ChiIntervalOptions options = ChiIntervalOptions.None)
        where TRng : struct, IChiRngSource<TRng>
    {
        uint lo, mid, hi;

        do
        {
            lo = TRng.NextUInt32(ref rng);
            mid = TRng.NextUInt32(ref rng);
            hi = TRng.NextUInt32(ref rng);
        } while (options.HasFlag(ChiIntervalOptions.ExcludeMin) && lo == 0 && mid == 0 && hi == 0);

        var randomDecimal = new decimal((int)lo, (int)mid, (int)hi, false, 28);

        if (options.HasFlag(ChiIntervalOptions.IncludeMax))
        {
            const decimal maxDecimalWithScale28 = 7.9228162514264337593543950335m;

            return randomDecimal / maxDecimalWithScale28;
        }
        else
        {
            const decimal maxDecimalWithScale28 = 7.9228162514264337593543950335m;

            // Rejection sample to ensure the result is strictly less than 1.0
            while (randomDecimal >= maxDecimalWithScale28)
            {
                lo = TRng.NextUInt32(ref rng);
                mid = TRng.NextUInt32(ref rng);
                hi = TRng.NextUInt32(ref rng);
                randomDecimal = new decimal((int)lo, (int)mid, (int)hi, false, 28);
            }

            return randomDecimal / maxDecimalWithScale28;
        }
    }

    /// <summary>
    ///     Generates a random <see cref="ChiFixed" /> in [0, 1) by injecting random bits into the fractional part.
    /// </summary>
    /// <remarks>
    ///     This method directly injects random bits into the fractional part,
    ///     providing full precision without any division or multiplication.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ChiFixed NextChiFixed<TRng>(ref TRng rng, ChiIntervalOptions options = ChiIntervalOptions.None)
        where TRng : struct, IChiRngSource<TRng>
    {
        const int fractionalBits = ChiFixed.FractionalBits;
        const long maxSample = 1L << fractionalBits;

#pragma warning disable CS0162 // Unreachable code detected
        // Use NextUInt32 when fractional bits fit in 32 bits (faster), otherwise NextUInt64
        var raw = fractionalBits <= 32
            ? TRng.NextUInt32(ref rng)
            : (long)(TRng.NextUInt64(ref rng) >> (64 - fractionalBits));

        if (options.HasFlag(ChiIntervalOptions.ExcludeMin))
            while (raw == 0)
                raw = fractionalBits <= 32
                    ? TRng.NextUInt32(ref rng)
                    : (long)(TRng.NextUInt64(ref rng) >> (64 - fractionalBits));
#pragma warning restore CS0162 // Unreachable code detected

        if (!options.HasFlag(ChiIntervalOptions.IncludeMax))
            return new ChiFixed(raw);

        // For IncludeMax, we need to scale so maxSample maps to exactly 1.0
        // Use rejection sampling: if we get maxSample, that's exactly 1.0
        if (raw == maxSample - 1)
            // Small chance to bump to exactly 1.0
            if ((TRng.NextUInt32(ref rng) & 1) == 0)
                return ChiFixed.One;

        // raw ∈ [0, ScaleFactor) which is [0, 1) in ChiFixed representation
        return new ChiFixed(raw);
    }
}