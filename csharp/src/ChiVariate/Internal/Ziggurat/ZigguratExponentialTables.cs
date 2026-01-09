// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

namespace ChiVariate.Internal.Ziggurat;

/// <summary>
///     Deterministic reference tables for Ziggurat exponential distribution sampling.
///     Computed using decimal arithmetic for cross-platform consistency.
/// </summary>
/// <remarks>
///     Based on Marsaglia &amp; Tsang "The Ziggurat Method for Generating Random Variables" (2000).
///     Uses 256 layers for optimal rejection rate.
/// </remarks>
internal static class ZigguratExponentialTables
{
    /// <summary>Number of Ziggurat layers.</summary>
    internal const int LayerCount = 256;

    /// <summary>Tail cutoff R where we switch to tail sampling.</summary>
    internal static readonly decimal R;

    /// <summary>Area of each Ziggurat layer.</summary>
    internal static readonly decimal V;

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

    static ZigguratExponentialTables()
    {
        // Standard Ziggurat constants for 256 layers (exponential distribution)
        // These are computed to give equal-area rectangles under f(x) = exp(-x)
        const decimal de = 7.697117470131050077725674991m; // Tail cutoff R
        const decimal ve = 0.003949659822581557199253251687m; // Layer area V

        R = de;
        V = ve;

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