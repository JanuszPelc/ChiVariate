// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

namespace ChiVariate.Internal.Ziggurat;

/// <summary>
///     Deterministic reference tables for Ziggurat normal distribution sampling.
///     Computed using decimal arithmetic for cross-platform consistency.
/// </summary>
/// <remarks>
///     Based on Marsaglia &amp; Tsang "The Ziggurat Method for Generating Random Variables" (2000).
///     Uses 128 layers with tail cutoff at R ≈ 3.44.
/// </remarks>
internal static class ZigguratNormalTables
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

    static ZigguratNormalTables()
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