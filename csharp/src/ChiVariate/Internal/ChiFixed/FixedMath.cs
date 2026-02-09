// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ChiVariate.Internal.ChiFixed;

/// <summary>
///     Core mathematical operations for ChiFixed fixed-point numbers.
/// </summary>
/// <remarks>
///     Fixed-point arithmetic rules:
///     - Addition/Subtraction: direct long addition (same scale)
///     - Multiplication: (a * b) >> FractionalBits (normalize double-scaled product)
///     - Division: (a &lt;&lt; FractionalBits) / b (scale numerator before dividing)
/// </remarks>
internal static class FixedMath
{
    // Precomputed powers of 5 for decimal conversion (5^0 to 5^28)
    private static readonly UInt128[] PrecomputedPow5 = CreatePow5Lookup();

    // Precomputed powers of 5 (ulong) for fast decimal path (5^0 to 5^24)
    // Limit 24: for scale s, remainder r < 5^s, and (r << (32-s)) must fit in ulong.
    // Constraint: 5^s * 2^(32-s) < 2^64, which holds for s ≤ 24.
    private static readonly ulong[] PrecomputePow5Fast = CreatePow5FastLookup();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal ToDecimal(long raw)
    {
        const long fracMask = ChiVariate.ChiFixed.ScaleFactor - 1;
        if ((raw & fracMask) == 0)
            return raw >> ChiVariate.ChiFixed.FractionalBits;

        var negative = raw < 0;
        var absRaw = (ulong)(negative ? -raw : raw);

        // mantissa = absRaw * 10^10 / 2^32, with banker's rounding
        // UInt128 multiply is fast (two 64-bit multiplies), unlike UInt128 division
        var product = (UInt128)absRaw * 10_000_000_000UL;
        var rem = (uint)product;
        var m0 = (uint)(product >> 32);
        var m1 = (uint)(product >> 64);
        var m2 = (uint)(product >> 96);

        if (rem > 0x80000000U || (rem == 0x80000000U && (m0 & 1) != 0))
            if (++m0 == 0)
                if (++m1 == 0)
                    m2++;

        return new decimal((int)m0, (int)m1, (int)m2, negative, 10);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long FromDecimal(decimal v)
    {
        if (v == 0m)
            return 0;

        ref readonly var d = ref Unsafe.As<decimal, DecimalRaw>(ref Unsafe.AsRef(in v));
        var lo64 = d.Lo64;
        var hi32 = d.Hi32;
        var flags = d.Flags;
        var negative = flags < 0;
        var scale = (flags >> 16) & 0x7F;

        if (scale == 0 && hi32 == 0)
            if (lo64 <= (ulong)long.MaxValue >> ChiVariate.ChiFixed.FractionalBits)
            {
                var scaled = (long)(lo64 << ChiVariate.ChiFixed.FractionalBits);
                return negative ? -scaled : scaled;
            }

        // Fast path: hi == 0, scale 1-24 — pure ulong arithmetic
        // Uses 10^s = 5^s × 2^s, so mantissa × 2^32 / 10^s = (mantissa / 5^s) << (32 - s)
        if (hi32 == 0 && scale <= 24)
        {
            var p5 = PrecomputePow5Fast[scale];
            var sh = ChiVariate.ChiFixed.FractionalBits - scale;

            var quot = lo64 / p5;
            var rem5 = lo64 - quot * p5;

            if (quot >> (63 - sh) != 0)
                return negative
                    ? ChiVariate.ChiFixed.NegativeInfinity.Raw
                    : ChiVariate.ChiFixed.PositiveInfinity.Raw;

            // rem5 < 5^s and sh = 32-s, so rem5 << sh < 5^s × 2^(32-s) < 2^64 for s ≤ 24
            var remShifted = rem5 << sh;
            var frac = remShifted / p5;
            var fracRem = remShifted - frac * p5;

            // Banker's rounding
            if (fracRem * 2 > p5 || (fracRem * 2 == p5 && (frac & 1) != 0))
                frac++;

            var result = (long)((quot << sh) + frac);
            if (result < 0)
                return negative
                    ? ChiVariate.ChiFixed.NegativeInfinity.Raw
                    : ChiVariate.ChiFixed.PositiveInfinity.Raw;

            return negative ? -result : result;
        }

        var mantissa =
            ((UInt128)hi32 << 64) |
            lo64;

        var shift = ChiVariate.ChiFixed.FractionalBits - scale;
        var maxMantissa = UInt128.MaxValue >> shift;

        if (mantissa > maxMantissa)
            return negative ? ChiVariate.ChiFixed.NegativeInfinity.Raw : ChiVariate.ChiFixed.PositiveInfinity.Raw;

        var numerator = mantissa << shift;
        var denominator = PrecomputedPow5[scale];

        var q = numerator / denominator;
        var p = q * denominator;
        var r = p == numerator ? 0 : numerator - p;

        if (r * 2 > denominator || (r * 2 == denominator && (q & 1) != 0))
            q++;

        long raw;
        if (q > (UInt128)long.MaxValue)
        {
            raw = negative ? ChiVariate.ChiFixed.NegativeInfinity.Raw : ChiVariate.ChiFixed.PositiveInfinity.Raw;
        }
        else
        {
            raw = (long)q;
            if (negative)
                raw = -raw;
        }

        return raw;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long FromDouble(double v)
    {
        const double scale = ChiVariate.ChiFixed.ScaleFactor;
        var scaled = Math.Round(v * scale, MidpointRounding.ToEven);
        if (scaled >= long.MaxValue) return ChiVariate.ChiFixed.PositiveInfinity.Raw;
        if (scaled <= long.MinValue) return ChiVariate.ChiFixed.NegativeInfinity.Raw;
        return (long)scaled;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToDouble(long raw)
    {
        return raw * (1.0 / ChiVariate.ChiFixed.ScaleFactor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long FromFloat(float v)
    {
        return FromDouble(v);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToFloat(long raw)
    {
        return (float)ToDouble(raw);
    }

    /// <summary>
    ///     Rounds raw Q31.32 to nearest integer using banker's rounding.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToIntegerRounded(long raw)
    {
        const long half = ChiVariate.ChiFixed.ScaleFactor >> 1;
        const long fracMask = ChiVariate.ChiFixed.ScaleFactor - 1;

        var intPart = raw >> ChiVariate.ChiFixed.FractionalBits;
        var frac = raw & fracMask;

        if (frac > half || (frac == half && (intPart & 1) != 0))
            intPart++;

        return intPart;
    }

    private static UInt128[] CreatePow5Lookup()
    {
        var result = new UInt128[29];
        result[0] = 1;

        for (var i = 1; i < result.Length; i++)
            result[i] = result[i - 1] * 5;
        return result;
    }

    private static ulong[] CreatePow5FastLookup()
    {
        var result = new ulong[25]; // 5^0 to 5^24
        result[0] = 1;

        for (var i = 1; i < result.Length; i++)
            result[i] = result[i - 1] * 5;
        return result;
    }

    /// <summary>
    ///     Mirrors the internal layout of System.Decimal in CoreCLR.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    private readonly struct DecimalRaw
    {
        [FieldOffset(0)] public readonly int Flags;
        [FieldOffset(4)] public readonly uint Hi32;
        [FieldOffset(8)] public readonly ulong Lo64;
    }

    #region ChiFixed Overloads (Table/CORDIC)

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiVariate.ChiFixed Sin(ChiVariate.ChiFixed x)
    {
        return new ChiVariate.ChiFixed(TableSinCos.Sin(x.Raw));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiVariate.ChiFixed Cos(ChiVariate.ChiFixed x)
    {
        return new ChiVariate.ChiFixed(TableSinCos.Cos(x.Raw));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiVariate.ChiFixed Tan(ChiVariate.ChiFixed x)
    {
        return new ChiVariate.ChiFixed(TableSinCos.Tan(x.Raw));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (ChiVariate.ChiFixed Sin, ChiVariate.ChiFixed Cos) SinCos(ChiVariate.ChiFixed x)
    {
        var (sin, cos) = TableSinCos.SinCos(x.Raw);
        return (new ChiVariate.ChiFixed(sin), new ChiVariate.ChiFixed(cos));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiVariate.ChiFixed Atan(ChiVariate.ChiFixed x)
    {
        return new ChiVariate.ChiFixed(TableAtan.Atan(x.Raw));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiVariate.ChiFixed Atan2(ChiVariate.ChiFixed y, ChiVariate.ChiFixed x)
    {
        return new ChiVariate.ChiFixed(TableAtan.Atan2(y.Raw, x.Raw));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiVariate.ChiFixed Sqrt(ChiVariate.ChiFixed x)
    {
        return new ChiVariate.ChiFixed(NewtonSqrt.Sqrt(x.Raw));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiVariate.ChiFixed Cbrt(ChiVariate.ChiFixed x)
    {
        return new ChiVariate.ChiFixed(NewtonCbrt.Cbrt(x.Raw));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiVariate.ChiFixed Asin(ChiVariate.ChiFixed x)
    {
        var raw = x.Raw;

        switch (raw)
        {
            case 0:
                return ChiVariate.ChiFixed.Zero;
            case ChiVariate.ChiFixed.ScaleFactor:
                return new ChiVariate.ChiFixed(CordicTables.HalfPi);
            case -ChiVariate.ChiFixed.ScaleFactor:
                return new ChiVariate.ChiFixed(-CordicTables.HalfPi);
            case > ChiVariate.ChiFixed.ScaleFactor or < -ChiVariate.ChiFixed.ScaleFactor:
                throw new ArgumentException("Asin is only defined for values in [-1, 1].");
        }

        var oneMinusX2 = ChiVariate.ChiFixed.ScaleFactor -
                         (long)(((Int128)raw * raw) >> ChiVariate.ChiFixed.FractionalBits);
        var sqrt = NewtonSqrt.Sqrt(oneMinusX2);
        var ratio = (long)(((Int128)raw << ChiVariate.ChiFixed.FractionalBits) / sqrt);

        return new ChiVariate.ChiFixed(CordicAtan.Atan(ratio));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiVariate.ChiFixed Acos(ChiVariate.ChiFixed x)
    {
        return new ChiVariate.ChiFixed(CordicTables.HalfPi - Asin(x).Raw);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiVariate.ChiFixed Exp(ChiVariate.ChiFixed x)
    {
        return new ChiVariate.ChiFixed(TaylorExp.Exp(x.Raw));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiVariate.ChiFixed Ln(ChiVariate.ChiFixed x)
    {
        return new ChiVariate.ChiFixed(TableLn.Ln(x.Raw));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiVariate.ChiFixed Round(ChiVariate.ChiFixed x, int digits, MidpointRounding mode)
    {
        return new ChiVariate.ChiFixed(Pow10Round.Round(x.Raw, digits, mode));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiVariate.ChiFixed Floor(ChiVariate.ChiFixed x)
    {
        return new ChiVariate.ChiFixed(x.Raw & ~(ChiVariate.ChiFixed.ScaleFactor - 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiVariate.ChiFixed Ceiling(ChiVariate.ChiFixed x)
    {
        const long fracMask = ChiVariate.ChiFixed.ScaleFactor - 1;
        var floor = x.Raw & ~fracMask;
        return (x.Raw & fracMask) != 0
            ? new ChiVariate.ChiFixed(floor + ChiVariate.ChiFixed.ScaleFactor)
            : new ChiVariate.ChiFixed(floor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiVariate.ChiFixed Truncate(ChiVariate.ChiFixed x)
    {
        const long fracMask = ChiVariate.ChiFixed.ScaleFactor - 1;
        var floor = x.Raw & ~fracMask;
        return x.Raw < 0 && (x.Raw & fracMask) != 0
            ? new ChiVariate.ChiFixed(floor + ChiVariate.ChiFixed.ScaleFactor)
            : new ChiVariate.ChiFixed(floor);
    }

    /// <summary>
    ///     Fixed-point addition with saturation on overflow.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Add(long a, long b)
    {
        var result = a + b;
        if (((a ^ result) & (b ^ result)) < 0)
            result = a >= 0 ? long.MaxValue : long.MinValue;
        return result;
    }

    /// <summary>
    ///     Fixed-point subtraction with saturation on overflow.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Sub(long a, long b)
    {
        var result = a - b;
        if (((a ^ result) & (~b ^ result)) < 0)
            result = a >= 0 ? long.MaxValue : long.MinValue;
        return result;
    }

    /// <summary>
    ///     Fixed-point multiplication with saturation on overflow.
    /// </summary>
    /// <remarks>
    ///     The product has double the fractional bits, so we shift right to normalize.
    ///     Uses 128-bit intermediate to maintain precision.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Mul(long a, long b)
    {
        // Math.BigMul returns 128-bit result as (hi, lo)
        var hi = Math.BigMul(a, b, out var lo);

        // Combine hi and lo, shifted right by FractionalBits
        const int shift = ChiVariate.ChiFixed.FractionalBits;
        var result = (hi << (64 - shift)) | (lo >>> shift);

        // Check for overflow: verify hi bits match sign extension of result
        var expectedHi = result >> (64 - shift);
        if (hi != expectedHi)
            return (a ^ b) < 0 ? long.MinValue : long.MaxValue; // Saturate

        return result;
    }

    /// <summary>
    ///     Fixed-point division with scaling to maintain precision.
    /// </summary>
    /// <remarks>
    ///     Scales the numerator up by the scale factor before dividing.
    ///     This requires 128-bit arithmetic to avoid overflow.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Div(long a, long b)
    {
        if (b == 0) throw new DivideByZeroException();

        var negative = (a ^ b) < 0;
        var uA = (ulong)(a < 0 ? -a : a);
        var uB = (ulong)(b < 0 ? -b : b);

        // Scale numerator by FractionalBits, splitting into (hi, lo) for 128-bit dividend
        const int integerBits = 64 - ChiVariate.ChiFixed.FractionalBits;
        var dividendHi = uA >> integerBits;
        var dividendLo = uA << ChiVariate.ChiFixed.FractionalBits;

        var quotient = Div128By64(dividendHi, dividendLo, uB, true);
        if (quotient > long.MaxValue)
            return negative ? long.MinValue : long.MaxValue; // Saturate
        return negative ? -(long)quotient : (long)quotient;
    }

    /// <summary>
    ///     128-bit by 64-bit division using Knuth's Algorithm D.
    /// </summary>
    /// <remarks>
    ///     Divides a 128-bit number (hi:lo) by a 64-bit divisor.
    ///     Uses normalization and quotient digit estimation for efficiency.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong Div128By64(ulong hi, ulong lo, ulong divisor, bool saturate = false)
    {
        const int totalBits = 64;
        const int halfBits = 32;
        const ulong lowMask = 0xFFFF_FFFF;

        if (hi == 0)
            return lo / divisor;

        if (hi >= divisor)
            return saturate ? ulong.MaxValue : throw new OverflowException("Quotient exceeds 64 bits");

        var shift = BitOperations.LeadingZeroCount(divisor);
        divisor <<= shift;

        var nHi = (hi << shift) | (shift == 0 ? 0 : lo >> (totalBits - shift));
        var nLo = lo << shift;

        var dHi = divisor >> halfBits;
        var dLo = divisor & lowMask;

        var qHi = EstimateQuotientDigit(nHi, nLo >> halfBits, dHi, dLo);
        var rem = ((nHi << halfBits) | (nLo >> halfBits)) - qHi * divisor;

        var qLo = EstimateQuotientDigit(rem, nLo & lowMask, dHi, dLo);

        return (qHi << halfBits) | qLo;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong EstimateQuotientDigit(ulong nHi, ulong nLo, ulong dHi, ulong dLo)
        {
            const int halfBits = 32;
            const ulong halfMax = 0x1_0000_0000;

            var q = nHi / dHi;
            var r = nHi - q * dHi;

            while (q >= halfMax || q * dLo > ((r << halfBits) | nLo))
            {
                q--;
                r += dHi;
                if (r >= halfMax) break;
            }

            return q;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Mod(long a, long b)
    {
        return a % b;
    }

    #endregion
}