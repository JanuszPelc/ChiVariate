// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Runtime.CompilerServices;

namespace ChiVariate.Internal.ChiFixed;

/// <summary>
///     CORDIC-based arctangent for ChiFixed fixed-point numbers.
/// </summary>
/// <remarks>
///     CORDIC (COordinate Rotation DIgital Computer) computes atan(y/x) by
///     rotating the vector (x, y) toward the x-axis using only shifts and additions.
///     At each iteration i:
///     - Rotate by ±arctan(2^-i) to reduce the y-component toward zero
///     - Accumulate the rotation angle in z
///     After all iterations:
///     - The vector has been rotated to lie along the x-axis
///     - z contains the total rotation angle = atan(y/x)
///     Note: This implementation doesn't scale by K because we only need the angle,
///     not the final vector magnitude.
/// </remarks>
internal static class CordicAtan
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Atan(long valueRaw)
    {
        if (valueRaw == 0)
            return 0;

        // atan(value) = atan(value/1), so start with x=1, y=value
        const long x = ChiVariate.ChiFixed.ScaleFactor;
        return AtanCore(x, valueRaw);
    }

    /// <summary>
    ///     CORDIC vectoring mode: rotates (x, y) toward x-axis, accumulates angle in z.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long AtanCore(long x, long y)
    {
        var z = 0L; // Accumulated angle

        for (var i = 0; i < CordicTables.Iterations; i++)
        {
            // Precomputed: rotation by arctan(2^-i) uses shifts instead of multiplies
            var dx = y >> i; // y * 2^-i
            var dy = x >> i; // x * 2^-i
            var dz = CordicTables.AtanTable[i]; // arctan(2^-i)

            // Choose rotation direction to drive y toward zero
            if (y >= 0)
            {
                // Rotate clockwise (negative angle)
                x += dx;
                y -= dy;
                z += dz;
            }
            else
            {
                // Rotate counter-clockwise (positive angle)
                x -= dx;
                y += dy;
                z -= dz;
            }
        }

        return z;
    }
}