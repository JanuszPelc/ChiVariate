// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate.Internal.Ziggurat;

/// <summary>
///     Generic Ziggurat sampler for standard normal distribution.
///     Tables are converted from golden decimal tables once per type T.
/// </summary>
internal static class ZigguratNormal<T>
    where T : IFloatingPoint<T>
{
    private static readonly T[] Wn; // Layer widths for sampling
    private static readonly T[] Dn; // Layer boundaries for fast path check
    private static readonly T[] Fn; // PDF values at boundaries
    private static readonly T R; // Tail cutoff
    private static readonly T Two;
    private static readonly T NegativeHalf;

    static ZigguratNormal()
    {
        var n = ZigguratNormalTables.LayerCount;

        // Convert golden tables to type T (once per type)
        Wn = new T[n + 1];
        Dn = new T[n + 1];
        Fn = new T[n + 1];

        for (var i = 0; i <= n; i++)
        {
            Wn[i] = T.CreateChecked(ZigguratNormalTables.Wn[i]);
            Dn[i] = T.CreateChecked(ZigguratNormalTables.Dn[i]);
            Fn[i] = T.CreateChecked(ZigguratNormalTables.Fn[i]);
        }

        R = T.CreateChecked(ZigguratNormalTables.R);
        Two = T.CreateChecked(2);
        NegativeHalf = T.CreateChecked(-0.5m);
    }

    /// <summary>
    ///     Generates a pair of independent standard normal variates.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (T z1, T z2) NextPair<TRng>(ref TRng rng)
        where TRng : struct, IChiRngSource<TRng>
    {
        return (Next(ref rng), Next(ref rng));
    }

    /// <summary>
    ///     Generates a single standard normal variate using the Ziggurat algorithm.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Next<TRng>(ref TRng rng)
        where TRng : struct, IChiRngSource<TRng>
    {
        const int layerCount = ZigguratNormalTables.LayerCount;
        const int layerMask = layerCount - 1; // 127 for 128 layers

        while (true)
        {
            // Generate random bits: use lower 7 bits for layer index, rest for sign and magnitude
            var bits = TRng.NextUInt64(ref rng);
            var i = (int)(bits & layerMask); // Layer index 0-127
            var sign = (bits & 128) != 0; // Sign bit

            // Generate uniform in [0, 1) and scale to layer width
            // Note: Wn[0] > R to allow base layer to reach tail region
            var u = ChiRealProvider.Next<TRng, T>(ref rng);
            var x = u * Wn[i];

            // Fast path (~97.5% of samples): x falls within the rectangle
            if (x < Dn[i + 1])
                return sign ? -x : x;

            // Layer 127 (base at tail): handle overhang and tail
            if (i == 127)
            {
                var result = SampleBaseLayer(ref rng, x);
                return sign ? -result : result;
            }

            // Rejection sampling in the wedge region (~2.5% of samples)
            // Accept if random point falls under the PDF curve
            var yFloor = Fn[i + 1];
            var yCeil = Fn[i];
            var yRandom = yFloor + ChiRealProvider.Next<TRng, T>(ref rng) * (yCeil - yFloor);

            // PDF value at x: f(x) = exp(-x²/2)
            var fx = ChiMath.Exp(NegativeHalf * x * x);

            if (yRandom < fx)
                return sign ? -x : x;

            // Rejected - try again
        }
    }

    /// <summary>
    ///     Handles sampling from layer 127 (the base layer at the tail),
    ///     which includes the rectangular part and the tail region.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static T SampleBaseLayer<TRng>(ref TRng rng, T x)
        where TRng : struct, IChiRngSource<TRng>
    {
        // x is in [0, Wn[127]] where Wn[127] > R
        // The rectangle [0, R] x [0, Fn[127]] is entirely under the PDF curve
        // so any x < R can be accepted directly.

        if (x < R)
            return x;

        // Tail region (x >= R): use Marsaglia's tail algorithm
        return SampleTail(ref rng);
    }

    /// <summary>
    ///     Samples from the tail of the normal distribution (|x| > R).
    ///     Uses Marsaglia's method for sampling from the normal tail.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static T SampleTail<TRng>(ref TRng rng)
        where TRng : struct, IChiRngSource<TRng>
    {
        // Marsaglia's method for normal tail:
        // Generate (x, y) where x = -ln(u1)/R and y = -ln(u2)
        // Accept if 2y > x²
        // Return R + x

        while (true)
        {
            var u1 = ChiRealProvider.Next<TRng, T>(ref rng, ChiIntervalOptions.ExcludeMin);
            var u2 = ChiRealProvider.Next<TRng, T>(ref rng, ChiIntervalOptions.ExcludeMin);

            var x = -ChiMath.Log(u1) / R;
            var y = -ChiMath.Log(u2);

            if (Two * y > x * x)
                return R + x;
        }
    }
}