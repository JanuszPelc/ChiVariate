// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Runtime.CompilerServices;

namespace ChiVariate.Internal.ChiFixed;

/// <summary>
///     Table-based natural logarithm for Q21.42 fixed-point numbers.
/// </summary>
/// <remarks>
///     Algorithm: Argument reduction + table lookup with linear interpolation.
///     1. Normalize x to range [1.0, 2.0) by halving/doubling, tracking scale n
///     2. Look up ln(normalized) from precomputed table with interpolation
///     3. Result = ln(normalized) + n * ln(2), since ln(x * 2^n) = ln(x) + n*ln(2)
///     Table has 16384 entries covering ln(1.0) to ln(2.0) with linear interpolation
///     between entries for sub-entry precision.
/// </remarks>
internal static class TableLn
{
    private const int TableBits = 14; // 16384 entries for high precision
    private const int TableSize = 1 << TableBits;
    private const long One = ChiVariate.ChiFixed.ScaleFactor;
    private const long Two = One << 1;

    private static readonly long Ln2Raw; // ln(2) in fixed-point
    private static readonly long[] LnTable; // ln(1 + i/16384) for i = 0..16384

    static TableLn()
    {
        // ln(2) ≈ 0.693... used for argument reduction
        Ln2Raw = FixedMath.FromDecimal(0.69314718055994530941723212146m);

        // Precompute ln(1 + f) for f = 0, 1/16384, 2/16384, ..., 1
        LnTable = new long[TableSize + 1];
        for (var i = 0; i <= TableSize; i++)
        {
            var x = 1m + (decimal)i / TableSize;
            var ln = DecimalLn(x);
            LnTable[i] = FixedMath.FromDecimal(ln);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Ln(long xRaw)
    {
        switch (xRaw)
        {
            case <= 0:
                throw new ArgumentException("Logarithm is only defined for positive values.");
            case One:
                return 0; // ln(1) = 0
        }

        // Argument reduction: normalize x to [1, 2) by tracking powers of 2
        // ln(x * 2^n) = ln(x) + n * ln(2)
        var scale = 0;

        while (xRaw >= Two)
        {
            xRaw >>= 1;
            scale++;
        }

        while (xRaw < One)
        {
            xRaw <<= 1;
            scale--;
        }

        // Now xRaw is in [1.0, 2.0) represented as [One, Two)
        // Look up ln(xRaw) from table with linear interpolation
        var fractional = xRaw - One; // How far above 1.0
        var index = (int)(fractional >> (ChiVariate.ChiFixed.FractionalBits - TableBits));
        var remainder = fractional & ((1L << (ChiVariate.ChiFixed.FractionalBits - TableBits)) - 1);

        // Linear interpolation between table entries
        var lnLow = LnTable[index];
        var lnHigh = LnTable[index + 1];
        var lnInterpolated =
            lnLow + (long)(((Int128)(lnHigh - lnLow) * remainder) >> (ChiVariate.ChiFixed.FractionalBits - TableBits));

        // Final result: ln(original) = ln(normalized) + scale * ln(2)
        return lnInterpolated + scale * Ln2Raw;
    }

    /// <summary>
    ///     Decimal logarithm using Taylor series (for table generation only).
    /// </summary>
    private static decimal DecimalLn(decimal x)
    {
        if (x <= 0m)
            throw new ArgumentException("Logarithm is only defined for positive values.");

        const decimal e = 2.71828182845904523536028747135m;
        var scale = 0;

        while (x > 1.5m)
        {
            x /= e;
            scale++;
        }

        while (x < 0.5m)
        {
            x *= e;
            scale--;
        }

        var u = x - 1m;
        var term = u;
        var sum = u;

        for (var n = 2; n <= 100; n++)
        {
            term *= -u;
            var delta = term / n;
            if (Math.Abs(delta) < 1e-28m)
                break;
            sum += delta;
        }

        return sum + scale;
    }
}