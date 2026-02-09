// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Runtime.CompilerServices;

namespace ChiVariate.Internal.ChiFixed;

/// <summary>
///     Table-based exponential (e^x) for ChiFixed fixed-point numbers.
/// </summary>
/// <remarks>
///     Algorithm: Split x into integer and fractional parts, use precomputed tables.
///     e^x = e^(int + frac) = e^int * e^frac
///     Two tables:
///     - EPowers[n] = e^n for n = 0, 1, 2, ..., 27 (precomputed powers of e)
///     - ExpFracTable[i] = e^(i/1024) for i = 0..1023 (fractional part lookup)
///     For negative x: e^(-x) = 1 / e^x
/// </remarks>
internal static class TaylorExp
{
    private const int TableBits = 10; // 1024 entries for fractional part
    private const int TableSize = 1 << TableBits;

    private static readonly long[] EPowers; // e^0, e^1, e^2, ..., e^27
    private static readonly long[] ExpFracTable; // e^(0/1024), e^(1/1024), ..., e^(1023/1024)

    static TaylorExp()
    {
        var eRaw = FixedMath.FromDecimal(2.71828182845904523536028747135m);

        // Precompute e^n for integer n = 0 to 27
        // e^27 ≈ 5.3e11, e^28 would overflow ChiFixed's ~2.1M max
        EPowers = new long[28];
        EPowers[0] = ChiVariate.ChiFixed.ScaleFactor; // e^0 = 1
        EPowers[1] = eRaw; // e^1 = e

        for (var i = 2; i < EPowers.Length; i++)
        {
            var product = FixedMath.Mul(EPowers[i - 1], eRaw);
            if (product > long.MaxValue / 2)
                EPowers[i] = long.MaxValue; // Overflow protection
            else
                EPowers[i] = product;
        }

        // Precompute e^(f) for f = 0, 1/1024, 2/1024, ..., 1023/1024
        ExpFracTable = new long[TableSize];
        for (var i = 0; i < TableSize; i++)
        {
            var x = (decimal)i / TableSize;
            ExpFracTable[i] = FixedMath.FromDecimal(ChiDecimalMath.Exp(x));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Exp(long xRaw)
    {
        if (xRaw == 0)
            return ChiVariate.ChiFixed.ScaleFactor; // e^0 = 1

        var negative = xRaw < 0;
        if (negative)
            xRaw = -xRaw;

        // Split x into integer and fractional parts
        var intPart = (int)(xRaw >> ChiVariate.ChiFixed.FractionalBits);
        var fracPart = xRaw & ((1L << ChiVariate.ChiFixed.FractionalBits) - 1);

        // Overflow: e^28 ≈ 1.4e12 exceeds ChiFixed range
        if (intPart >= EPowers.Length)
            return negative ? 0 : long.MaxValue;

        // e^x = e^int * e^frac
        var intExp = EPowers[intPart];

        if (fracPart == 0)
            return negative ? Reciprocal(intExp) : intExp;

        // Look up e^frac from table (use top 10 bits of fractional part)
        var fracIndex = (int)(fracPart >> (ChiVariate.ChiFixed.FractionalBits - TableBits));
        var fracExp = ExpFracTable[fracIndex];

        var result = FixedMath.Mul(intExp, fracExp);

        // For negative x: e^(-x) = 1 / e^x
        return negative ? Reciprocal(result) : result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long Reciprocal(long xRaw)
    {
        if (xRaw == 0)
            return long.MaxValue;

        return FixedMath.Div(ChiVariate.ChiFixed.ScaleFactor, xRaw);
    }
}