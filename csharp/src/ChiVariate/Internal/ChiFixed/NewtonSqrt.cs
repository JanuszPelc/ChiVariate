// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Runtime.CompilerServices;

namespace ChiVariate.Internal.ChiFixed;

/// <summary>
///     Table-based square root for ChiFixed fixed-point numbers.
/// </summary>
/// <remarks>
///     Algorithm: Table lookup with bit manipulation (no Newton iterations at runtime).
///     1. Normalize input to [1.0, 4.0) by tracking the binary exponent
///     2. Use top 12 bits as index into precomputed sqrt table
///     3. Adjust result exponent: sqrt(x * 2^n) = sqrt(x) * 2^(n/2)
///     Two tables are needed because sqrt changes behavior at powers of 2:
///     - Even exponent: normalize to [1, 2), use table[0..4095]
///     - Odd exponent: normalize to [2, 4), use table[4096..8191]
/// </remarks>
internal static class NewtonSqrt
{
    private const int TableBits = 12; // 4096 entries per table
    private const int TableSize = 1 << TableBits;
    private static readonly long[] SqrtTable = CreateSqrtTable();

    /// <summary>
    ///     Creates lookup tables for sqrt in ranges [1,2) and [2,4).
    /// </summary>
    private static long[] CreateSqrtTable()
    {
        var table = new long[TableSize * 2];
        for (var i = 0; i < TableSize; i++)
        {
            // Table for even exponents: input in [1.0, 2.0)
            var evenInput = 1.0 + (double)i / TableSize;
            table[i] = (long)(Math.Sqrt(evenInput) * ChiVariate.ChiFixed.ScaleFactor);

            // Table for odd exponents: input in [2.0, 4.0)
            var oddInput = 2.0 + 2.0 * i / TableSize;
            table[TableSize + i] = (long)(Math.Sqrt(oddInput) * ChiVariate.ChiFixed.ScaleFactor);
        }

        return table;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Sqrt(long valueRaw)
    {
        if (valueRaw <= 0)
        {
            if (valueRaw < 0)
                throw new ArgumentException("Cannot compute square root of negative number.");
            return 0;
        }

        // Find position of highest set bit (determines binary exponent)
        var highBit = 63 - (int)ulong.LeadingZeroCount((ulong)valueRaw);

        // Determine if exponent is odd/even relative to fixed-point binary point
        // This selects which table to use: [1,2) for even, [2,4) for odd
        var isOddExponent = ((highBit ^ ChiVariate.ChiFixed.FractionalBits) & 1) != 0;
        var tableOffset = isOddExponent ? TableSize : 0;

        // Extract top 12 bits of mantissa as table index
        var shiftForIndex = highBit - TableBits;
        var index = shiftForIndex >= 0
            ? (int)((valueRaw >> shiftForIndex) & (TableSize - 1))
            : (int)((valueRaw << -shiftForIndex) & (TableSize - 1));

        var sqrtNormalized = SqrtTable[tableOffset + index];

        // Adjust for exponent: sqrt(x * 2^n) = sqrt(x) * 2^(n/2)
        var resultShift = (highBit - ChiVariate.ChiFixed.FractionalBits) >> 1;
        return resultShift >= 0
            ? sqrtNormalized << resultShift
            : sqrtNormalized >> -resultShift;
    }
}