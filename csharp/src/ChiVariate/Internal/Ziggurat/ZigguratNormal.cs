// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate.Internal.Ziggurat;

/// <summary>
///     Generic Ziggurat sampler for standard normal distribution.
///     DecimalTables are converted from golden decimal tables once per type T.
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
        const int n = DecimalTables.LayerCount;

        // Convert golden tables to type T (once per type)
        Wn = new T[n + 1];
        Dn = new T[n + 1];
        Fn = new T[n + 1];

        for (var i = 0; i <= n; i++)
        {
            Wn[i] = T.CreateChecked(DecimalTables.Wn[i]);
            Dn[i] = T.CreateChecked(DecimalTables.Dn[i]);
            Fn[i] = T.CreateChecked(DecimalTables.Fn[i]);
        }

        R = T.CreateChecked(DecimalTables.R);
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
        const int layerCount = DecimalTables.LayerCount;
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

/// <summary>
///     Deterministic reference tables for Ziggurat normal distribution sampling.
///     Computed using decimal arithmetic for cross-platform consistency.
/// </summary>
/// <remarks>
///     Based on Marsaglia &amp; Tsang "The Ziggurat Method for Generating Random Variables" (2000).
///     Uses 128 layers with tail cutoff at R ≈ 3.44.
/// </remarks>
file static class DecimalTables
{
    /// <summary>Number of Ziggurat layers.</summary>
    internal const int LayerCount = 128;

    /// <summary>Tail cutoff R where we switch to tail sampling.</summary>
    internal static readonly decimal R;

    /// <summary>Area of each Ziggurat layer.</summary>
    internal static readonly decimal V;

    /// <summary>
    ///     Wn[i] = width of layer i for scaling uniform samples.
    ///     The base layer (i=0) is special and includes the tail.
    /// </summary>
    internal static readonly decimal[] Wn;

    /// <summary>
    ///     Fn[i] = f(Dn[i]) = exp(-Dn[i]²/2), the PDF value at each layer boundary.
    /// </summary>
    internal static readonly decimal[] Fn;

    /// <summary>
    ///     Dn[i] = x-coordinate of the right edge of layer i.
    ///     Dn[0] = R (base layer at tail cutoff), Dn[127] near 0 (top layer).
    /// </summary>
    internal static readonly decimal[] Dn;

    static DecimalTables()
    {
        // Standard Ziggurat constants for 128 layers (from Marsaglia & Tsang)
        const decimal dn = 3.442619855899m;
        const decimal vn = 0.00991256303526217m;

        R = dn;
        V = vn;

        Wn = new decimal[LayerCount + 1];
        Fn = new decimal[LayerCount + 1];
        Dn = new decimal[LayerCount + 1];

        // The Ziggurat for the normal distribution has (Marsaglia & Tsang convention):
        // - Layer 0: the base layer (WIDEST), includes tail area
        // - Layer 127: the top layer (narrowest), at tail cutoff R
        // - Dn[i] = right boundary of layer i (DECREASES as i increases)
        // - Fn[i] = f(Dn[i]) = exp(-Dn[i]²/2)

        // Start from layer 127 (tail cutoff R) and work backward to layer 0 (base)
        Dn[127] = dn;
        Fn[127] = ChiDecimalMath.Exp(-0.5m * dn * dn);

        // Build layers 126 down to 1
        // The recurrence: Dn[i] = sqrt(-2 * ln(vn/Dn[i+1] + Fn[i+1]))
        for (var i = 126; i >= 1; i--)
        {
            Dn[i] = ChiDecimalMath.Sqrt(-2.0m * ChiDecimalMath.Ln(vn / Dn[i + 1] + Fn[i + 1]));
            Fn[i] = ChiDecimalMath.Exp(-0.5m * Dn[i] * Dn[i]);
        }

        // Base layer (i=0) is special: Dn[0] = vn / Fn[1] (LARGEST, includes tail)
        Dn[0] = vn / Fn[1];
        Fn[0] = 1.0m; // f(0) = 1, base layer starts at x = 0

        // Sentinel at index 128: Dn[128] = 0 so fast path check works for all layers
        Dn[LayerCount] = 0m;
        Fn[LayerCount] = 1.0m;

        // Wn[i] = sampling width for layer i
        // For most layers: Wn[i] = Dn[i]
        // For base layer (i=127): Wn[127] = V / Fn[127] to include tail (Wn[127] > R)
        for (var i = 0; i < LayerCount; i++)
            Wn[i] = Dn[i];
        Wn[127] = vn / Fn[127]; // Base layer width includes tail area
        Wn[LayerCount] = Dn[LayerCount];
    }
}