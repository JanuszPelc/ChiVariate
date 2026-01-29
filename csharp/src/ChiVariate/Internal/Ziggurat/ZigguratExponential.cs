// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate.Internal.Ziggurat;

/// <summary>
///     Generic Ziggurat sampler for standard exponential distribution (rate = 1).
///     DecimalTables are converted from golden decimal tables once per type T.
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
        const int n = DecimalTables.LayerCount;

        // Convert golden tables to type T (once per type)
        We = new T[n + 1];
        De = new T[n + 1];
        Fe = new T[n + 1];

        for (var i = 0; i <= n; i++)
        {
            We[i] = T.CreateChecked(DecimalTables.We[i]);
            De[i] = T.CreateChecked(DecimalTables.De[i]);
            Fe[i] = T.CreateChecked(DecimalTables.Fe[i]);
        }

        R = T.CreateChecked(DecimalTables.R);
    }

    /// <summary>
    ///     Generates a single standard exponential variate (rate = 1) using the Ziggurat algorithm.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Next<TRng>(ref TRng rng)
        where TRng : struct, IChiRngSource<TRng>
    {
        const int layerCount = DecimalTables.LayerCount;
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

/// <summary>
///     Deterministic reference tables for Ziggurat exponential distribution sampling.
///     Computed using decimal arithmetic for cross-platform consistency.
/// </summary>
/// <remarks>
///     Based on Marsaglia &amp; Tsang "The Ziggurat Method for Generating Random Variables" (2000).
///     Uses 256 layers for optimal rejection rate.
/// </remarks>
file static class DecimalTables
{
    /// <summary>Number of Ziggurat layers.</summary>
    internal const int LayerCount = 256;

    /// <summary>Tail cutoff R where we switch to tail sampling.</summary>
    internal static readonly decimal R;

    /// <summary>
    ///     We[i] = width of layer i for scaling uniform samples.
    /// </summary>
    internal static readonly decimal[] We;

    /// <summary>
    ///     Fe[i] = f(De[i]) = exp(-De[i]), the PDF value at each layer boundary.
    /// </summary>
    internal static readonly decimal[] Fe;

    /// <summary>
    ///     De[i] = x-coordinate of the right edge of layer i.
    /// </summary>
    internal static readonly decimal[] De;

    static DecimalTables()
    {
        // Standard Ziggurat constants for 256 layers (exponential distribution)
        // These are computed to give equal-area rectangles under f(x) = exp(-x)
        const decimal de = 7.697117470131050077725674991m; // Tail cutoff R
        const decimal ve = 0.003949659822581557199253251687m; // Layer area V

        R = de;

        We = new decimal[LayerCount + 1];
        Fe = new decimal[LayerCount + 1];
        De = new decimal[LayerCount + 1];

        // Layer 255 is at tail cutoff (rightmost)
        De[255] = de;
        Fe[255] = ChiDecimalMath.Exp(-de);

        // Build layers 254 down to 1
        // Recurrence: De[i] = -ln(ve/De[i+1] + Fe[i+1])
        for (var i = 254; i >= 1; i--)
        {
            De[i] = -ChiDecimalMath.Ln(ve / De[i + 1] + Fe[i + 1]);
            Fe[i] = ChiDecimalMath.Exp(-De[i]);
        }

        // Base layer (i=0): De[0] = ve / Fe[1]
        De[0] = ve / Fe[1];
        Fe[0] = 1.0m; // f(0) = exp(0) = 1

        // Sentinel at index 256
        De[LayerCount] = 0m;
        Fe[LayerCount] = 1.0m;

        // We[i] = sampling width for layer i
        for (var i = 0; i < LayerCount; i++)
            We[i] = De[i];
        We[255] = ve / Fe[255]; // Base layer width includes tail area
        We[LayerCount] = De[LayerCount];
    }
}