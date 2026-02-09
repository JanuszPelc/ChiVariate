// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Runtime.CompilerServices;

namespace ChiVariate.Internal.ChiFixed;

/// <summary>
///     Precomputed tables for CORDIC (COordinate Rotation DIgital Computer) algorithm.
/// </summary>
/// <remarks>
///     CORDIC computes trigonometric functions using only shifts and additions.
///     Key insight: rotation by angle θ can be decomposed into rotations by
///     arctan(2^-i) for i = 0, 1, 2, ... which only require shifts.
///     Tables:
///     - AtanTable[i] = arctan(2^-i) for i = 0..47
///     - K = scaling factor = Π cos(arctan(2^-i)) ≈ 0.6073 (compensates for CORDIC gain)
///     48 iterations provide ~48 bits of precision, sufficient for ChiFixed format.
/// </remarks>
internal static class CordicTables
{
    internal const int Iterations = 48; // ~48 bits of precision

    private const decimal DecimalPi = 3.1415926535897932384626433833m;
    private const decimal DecimalEpsilon = 1e-28m;

    internal static readonly long[] AtanTable; // arctan(2^-i) for i = 0..47
    internal static readonly long K; // CORDIC gain factor ≈ 0.6073
    internal static readonly long Pi;
    internal static readonly long HalfPi;
    internal static readonly long TwoPi;

    static CordicTables()
    {
        AtanTable = new long[Iterations];

        // Compute CORDIC gain K = Π cos(arctan(2^-i))
        // Each CORDIC iteration scales by sqrt(1 + 2^-2i), so K compensates for this
        var k = 1m;
        for (var i = 0; i < Iterations; i++)
        {
            var powerOf2 = 1m / (1L << i); // 2^-i
            var atan = ChiDecimalMath.Atan(powerOf2); // arctan(2^-i)
            AtanTable[i] = FixedMath.FromDecimal(atan);
            k *= DecimalCos(atan); // Accumulate gain factor
        }

        K = FixedMath.FromDecimal(k);
        Pi = FixedMath.FromDecimal(DecimalPi);
        HalfPi = FixedMath.FromDecimal(DecimalPi / 2m);
        TwoPi = FixedMath.FromDecimal(DecimalPi * 2m);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static decimal DecimalCos(decimal x)
    {
        var result = 1m;
        var term = 1m;
        var xSquared = x * x;

        for (var n = 1; n <= 50 && Math.Abs(term) > DecimalEpsilon; n++)
        {
            term *= -xSquared / ((2 * n - 1) * 2 * n);
            result += term;
        }

        return result;
    }
}