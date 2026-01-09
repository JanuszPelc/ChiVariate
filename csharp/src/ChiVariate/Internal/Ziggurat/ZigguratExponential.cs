// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate.Internal.Ziggurat;

/// <summary>
///     Generic Ziggurat sampler for standard exponential distribution (rate = 1).
///     Tables are converted from golden decimal tables once per type T.
/// </summary>
internal static class ZigguratExponential<T>
    where T : IFloatingPoint<T>
{
    private static readonly T[] We; // Layer widths for sampling
    private static readonly T[] De; // Layer boundaries for fast path check
    private static readonly T[] Fe; // PDF values at boundaries
    private static readonly T R; // Tail cutoff

    static ZigguratExponential()
    {
        var n = ZigguratExponentialTables.LayerCount;

        // Convert golden tables to type T (once per type)
        We = new T[n + 1];
        De = new T[n + 1];
        Fe = new T[n + 1];

        for (var i = 0; i <= n; i++)
        {
            We[i] = T.CreateChecked(ZigguratExponentialTables.We[i]);
            De[i] = T.CreateChecked(ZigguratExponentialTables.De[i]);
            Fe[i] = T.CreateChecked(ZigguratExponentialTables.Fe[i]);
        }

        R = T.CreateChecked(ZigguratExponentialTables.R);
    }

    /// <summary>
    ///     Generates a single standard exponential variate (rate = 1) using the Ziggurat algorithm.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Next<TRng>(ref TRng rng)
        where TRng : struct, IChiRngSource<TRng>
    {
        const int layerCount = ZigguratExponentialTables.LayerCount;
        const int layerMask = layerCount - 1; // 255 for 256 layers

        while (true)
        {
            // Generate random bits: use lower 8 bits for layer index
            var bits = TRng.NextUInt64(ref rng);
            var i = (int)(bits & layerMask); // Layer index 0-255

            // Generate uniform in [0, 1) and scale to layer width
            var u = ChiRealProvider.Next<TRng, T>(ref rng);
            var x = u * We[i];

            // Fast path (~98% of samples): x falls within the rectangle
            if (x < De[i + 1])
                return x;

            // Layer 255 (base at tail): handle overhang and tail
            if (i == 255)
                return SampleBaseLayer(ref rng, x);

            // Rejection sampling in the wedge region (~2% of samples)
            // Accept if random point falls under the PDF curve
            var yFloor = Fe[i + 1];
            var yCeil = Fe[i];
            var yRandom = yFloor + ChiRealProvider.Next<TRng, T>(ref rng) * (yCeil - yFloor);

            // PDF value at x: f(x) = exp(-x)
            var fx = ChiMath.Exp(-x);

            if (yRandom < fx)
                return x;

            // Rejected - try again
        }
    }

    /// <summary>
    ///     Handles sampling from layer 255 (the base layer at the tail),
    ///     which includes the rectangular part and the tail region.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static T SampleBaseLayer<TRng>(ref TRng rng, T x)
        where TRng : struct, IChiRngSource<TRng>
    {
        // x is in [0, We[255]] where We[255] > R
        // The rectangle [0, R] x [0, Fe[255]] is entirely under the PDF curve
        if (x < R)
            return x;

        // Tail region (x >= R): use shifted exponential
        // For exponential, tail is simply: R + Exp(1)
        return R + Next(ref rng);
    }
}