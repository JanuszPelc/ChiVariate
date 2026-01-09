// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChiVariate.Internal.ChiFixed;

/// <summary>
///     Core mathematical operations for Q21.42 fixed-point numbers.
/// </summary>
/// <remarks>
///     ChiFixed uses Q21.42 format: 21 integer bits + 42 fractional bits in a 64-bit long.
///     - ScaleFactor = 2^42 = 4,398,046,511,104
///     - Range: approximately ±2.1 million
///     - Precision: ~10^-12 (sub-picometer resolution)
///     Fixed-point arithmetic rules:
///     - Addition/Subtraction: direct long addition (same scale)
///     - Multiplication: (a * b) &gt;&gt; 42 (product has 84 fractional bits, shift to get 42)
///     - Division: (a &lt;&lt; 42) / b (scale numerator before dividing)
/// </remarks>
internal static class FixedMath
{
    // Precomputed powers of 5 for decimal conversion (5^0 to 5^28)
    private static readonly UInt128[] PrecomputedPow5 = CreatePow5Lookup();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal ToDecimal(long raw)
    {
        const long mask = ChiVariate.ChiFixed.ScaleFactor - 1;

        var integerPart = raw >> ChiVariate.ChiFixed.FractionalBits;
        if ((raw & mask) == 0)
            return integerPart;

        var fractionalBits = raw & mask;
        const decimal scaleAsDecimal = ChiVariate.ChiFixed.ScaleFactor;
        return integerPart + fractionalBits / scaleAsDecimal;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long FromDecimal(decimal v)
    {
        if (v == 0m)
            return 0;

        Span<int> bits = stackalloc int[4];
        decimal.GetBits(v, bits);
        var lo = (uint)bits[0];
        var mid = (uint)bits[1];
        var hi = (uint)bits[2];
        var signBit = (uint)bits[3];

        var negative = (signBit & 0x80000000) != 0;
        var scale = (int)((signBit >> 16) & 0x7F);

        if (scale == 0 && hi == 0)
        {
            var mant64 = ((ulong)mid << 32) | lo;
            const int fracBits = ChiVariate.ChiFixed.FractionalBits;

            if (mant64 <= (ulong)long.MaxValue >> fracBits)
            {
                var scaled = (long)(mant64 << fracBits);
                return negative ? -scaled : scaled;
            }
        }

        var mantissa =
            ((UInt128)hi << 64) |
            ((UInt128)mid << 32) |
            lo;

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

    private static UInt128[] CreatePow5Lookup()
    {
        var result = new UInt128[29];
        result[0] = 1;

        for (var i = 1; i < result.Length; i++)
            result[i] = result[i - 1] * 5;
        return result;
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
    ///     Fixed-point multiplication: (a * b) >> 42.
    /// </summary>
    /// <remarks>
    ///     When multiplying two Q21.42 numbers, the product has 84 fractional bits.
    ///     We need to shift right by 42 to get back to Q21.42 format.
    ///     Uses 128-bit intermediate to avoid overflow.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Mul(long a, long b)
    {
        // Math.BigMul returns 128-bit result as (hi, lo)
        var hi = Math.BigMul(a, b, out var lo);
        // Combine hi and lo, shifted right by 42 bits
        return (hi << (64 - ChiVariate.ChiFixed.FractionalBits)) | (lo >>> ChiVariate.ChiFixed.FractionalBits);
    }

    /// <summary>
    ///     Fixed-point division: (a &lt;&lt; 42) / b.
    /// </summary>
    /// <remarks>
    ///     To maintain precision, we scale the numerator up by 2^42 before dividing.
    ///     This requires 128-bit arithmetic to avoid overflow.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Div(long a, long b)
    {
        if (b == 0) throw new DivideByZeroException();

        var negative = (a ^ b) < 0;
        var uA = (ulong)(a < 0 ? -a : a);
        var uB = (ulong)(b < 0 ? -b : b);

        // Scale numerator: a << 42 = (hi, lo) where hi has overflow bits
        const int integerBits = 64 - ChiVariate.ChiFixed.FractionalBits;
        var dividendHi = uA >> integerBits;
        var dividendLo = uA << ChiVariate.ChiFixed.FractionalBits;

        var quotient = (long)Div128By64(dividendHi, dividendLo, uB);
        return negative ? -quotient : quotient;
    }

    /// <summary>
    ///     128-bit by 64-bit division using Knuth's Algorithm D.
    /// </summary>
    /// <remarks>
    ///     Divides a 128-bit number (hi:lo) by a 64-bit divisor.
    ///     Uses normalization and quotient digit estimation for efficiency.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong Div128By64(ulong hi, ulong lo, ulong divisor)
    {
        const int totalBits = 64;
        const int halfBits = 32;
        const ulong lowMask = 0xFFFF_FFFF;

        if (hi == 0)
            return lo / divisor;

        if (hi >= divisor)
            throw new OverflowException("Quotient exceeds 64 bits");

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