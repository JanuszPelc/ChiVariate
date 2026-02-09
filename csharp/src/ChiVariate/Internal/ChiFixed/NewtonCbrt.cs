// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Runtime.CompilerServices;

namespace ChiVariate.Internal.ChiFixed;

/// <summary>
///     Table-based cube root for ChiFixed fixed-point numbers.
/// </summary>
/// <remarks>
///     Algorithm: Table lookup with bit manipulation (no Newton iterations at runtime).
///     1. Normalize input to [1.0, 8.0) by tracking the binary exponent
///     2. Use top 10 bits as index into precomputed cbrt table
///     3. Adjust result exponent: cbrt(x * 2^n) = cbrt(x) * 2^(n/3)
///     Three tables are needed because cbrt has period 3 in the exponent:
///     - Exponent mod 3 = 0: normalize to [1, 2), use table[0..1023]
///     - Exponent mod 3 = 1: normalize to [2, 4), use table[1024..2047]
///     - Exponent mod 3 = 2: normalize to [4, 8), use table[2048..3071]
/// </remarks>
internal static class NewtonCbrt
{
    private const int TableBits = 10; // 1024 entries per table
    private const int TableSize = 1 << TableBits;
    private static readonly long[] CbrtTable = CreateCbrtTable();

    /// <summary>
    ///     Creates lookup tables for cbrt in ranges [1,2), [2,4), and [4,8).
    /// </summary>
    private static long[] CreateCbrtTable()
    {
        var table = new long[TableSize * 3];
        for (var i = 0; i < TableSize; i++)
        {
            // Table for exponent mod 3 = 0: input in [1.0, 2.0)
            var input0 = 1.0m + (decimal)i / TableSize;
            table[i] = FixedMath.FromDecimal(ChiDecimalMath.Cbrt(input0));

            // Table for exponent mod 3 = 1: input in [2.0, 4.0)
            var input1 = 2.0m + 2.0m * i / TableSize;
            table[TableSize + i] = FixedMath.FromDecimal(ChiDecimalMath.Cbrt(input1));

            // Table for exponent mod 3 = 2: input in [4.0, 8.0)
            var input2 = 4.0m + 4.0m * i / TableSize;
            table[2 * TableSize + i] = FixedMath.FromDecimal(ChiDecimalMath.Cbrt(input2));
        }

        return table;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Cbrt(long valueRaw)
    {
        if (valueRaw == 0)
            return 0;

        // Cube root of negative is negative cube root of absolute value
        var negative = valueRaw < 0;
        if (negative)
            valueRaw = -valueRaw;

        // Find position of highest set bit (determines binary exponent)
        var highBit = 63 - (int)ulong.LeadingZeroCount((ulong)valueRaw);

        // Compute exponent offset from fixed-point binary point
        var expOffset = highBit - ChiVariate.ChiFixed.FractionalBits;
        // Select table based on exponent mod 3 (handles negative mod correctly)
        var remainder = (expOffset % 3 + 3) % 3;
        var tableOffset = remainder * TableSize;

        // Extract top 10 bits of mantissa as table index
        var shiftForIndex = highBit - TableBits;
        var index = shiftForIndex >= 0
            ? (int)((valueRaw >> shiftForIndex) & (TableSize - 1))
            : (int)((valueRaw << -shiftForIndex) & (TableSize - 1));

        var cbrtNormalized = CbrtTable[tableOffset + index];

        // Adjust for exponent: cbrt(x * 2^n) = cbrt(x) * 2^(n/3)
        var resultShift = (expOffset - remainder) / 3;
        var result = resultShift >= 0
            ? cbrtNormalized << resultShift
            : cbrtNormalized >> -resultShift;

        return negative ? -result : result;
    }
}