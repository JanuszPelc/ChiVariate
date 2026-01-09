// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Runtime.CompilerServices;

namespace ChiVariate.Internal.ChiFixed;

/// <summary>
///     Table-based arctangent for Q21.42 fixed-point numbers.
/// </summary>
/// <remarks>
///     Algorithm: Direct table lookup for atan(x) where x ∈ [0, 1], with
///     reciprocal identity for |x| > 1: atan(x) = π/2 - atan(1/x).
///     Table has 4096 entries covering atan(0) to atan(1).
///     For values > 1, use the identity to map back to [0, 1] range.
///     Atan2(y, x) handles all quadrants using the standard formula:
///     - x &gt; 0: atan(y/x)
///     - x &lt; 0, y ≥ 0: π + atan(y/x)
///     - x &lt; 0, y &lt; 0: -π + atan(y/x)
///     - x = 0: ±π/2 depending on sign of y
/// </remarks>
internal static class TableAtan
{
    private const int TableBits = 12; // 4096 entries
    private const int TableSize = 1 << TableBits;

    private static readonly long[] AtanTable; // atan(x) for x ∈ [0, 1]
    private static readonly long HalfPi;
    private static readonly long Pi;

    static TableAtan()
    {
        HalfPi = CordicTables.HalfPi;
        Pi = CordicTables.Pi;

        // Precompute atan(x) for x = 0, 1/4096, 2/4096, ..., 1
        AtanTable = new long[TableSize + 1];
        for (var i = 0; i <= TableSize; i++)
        {
            var x = (double)i / TableSize;
            AtanTable[i] = (long)(Math.Atan(x) * ChiVariate.ChiFixed.ScaleFactor);
        }
    }

    /// <summary>
    ///     Computes atan(x) using table lookup and reciprocal identity.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Atan(long valueRaw)
    {
        if (valueRaw == 0)
            return 0;

        var negative = valueRaw < 0;
        if (negative)
            valueRaw = -valueRaw;

        long result;
        if (valueRaw <= ChiVariate.ChiFixed.ScaleFactor)
        {
            // |x| ≤ 1: direct table lookup
            var index = (int)(valueRaw * TableSize / ChiVariate.ChiFixed.ScaleFactor);
            if (index > TableSize) index = TableSize;
            result = AtanTable[index];
        }
        else
        {
            // |x| > 1: use identity atan(x) = π/2 - atan(1/x)
            var reciprocal = FixedMath.Div(ChiVariate.ChiFixed.ScaleFactor, valueRaw);
            var index = (int)(reciprocal * TableSize / ChiVariate.ChiFixed.ScaleFactor);
            if (index > TableSize) index = TableSize;
            result = HalfPi - AtanTable[index];
        }

        return negative ? -result : result;
    }

    /// <summary>
    ///     Computes atan2(y, x) - the angle from positive x-axis to point (x, y).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Atan2(long yRaw, long xRaw)
    {
        // Handle axis cases
        if (xRaw == 0)
        {
            if (yRaw == 0)
                return 0; // Origin: undefined, return 0
            return yRaw > 0 ? HalfPi : -HalfPi; // Positive/negative y-axis
        }

        if (yRaw == 0)
            return xRaw > 0 ? 0 : Pi; // Positive/negative x-axis

        // x > 0: standard atan(y/x)
        if (xRaw > 0) return AtanRatio(yRaw, xRaw);

        // x < 0: adjust by ±π
        var baseAngle = AtanRatio(yRaw, -xRaw);
        return yRaw >= 0 ? Pi - baseAngle : -Pi - baseAngle;
    }

    /// <summary>
    ///     Helper: computes atan(y/x) for x > 0 using table lookup.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long AtanRatio(long yRaw, long xRaw)
    {
        var negative = yRaw < 0;
        if (negative)
            yRaw = -yRaw;

        long result;
        if (yRaw <= xRaw)
        {
            // |y/x| ≤ 1: direct lookup
            var ratio = FixedMath.Div(yRaw, xRaw);
            var index = (int)(ratio * TableSize / ChiVariate.ChiFixed.ScaleFactor);
            if (index > TableSize) index = TableSize;
            result = AtanTable[index];
        }
        else
        {
            // |y/x| > 1: use identity atan(y/x) = π/2 - atan(x/y)
            var ratio = FixedMath.Div(xRaw, yRaw);
            var index = (int)(ratio * TableSize / ChiVariate.ChiFixed.ScaleFactor);
            if (index > TableSize) index = TableSize;
            result = HalfPi - AtanTable[index];
        }

        return negative ? -result : result;
    }
}