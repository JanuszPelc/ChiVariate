// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Internal.Ziggurat;

namespace ChiVariate.Providers;

/// <summary>
///     Provides a stateless method for generating pairs of standard normal variables.
/// </summary>
/// <typeparam name="T">The floating-point type of the generated values.</typeparam>
public static class ChiNormalProvider<T>
    where T : IFloatingPoint<T>
{
    /// <summary>
    ///     Generates a pair of independent, standard normal (Gaussian) random variables.
    /// </summary>
    /// <param name="rng">The random number generator to use.</param>
    /// <returns>A tuple containing two standard normal variables (z1, z2).</returns>
    /// <remarks>
    ///     This implementation uses the Ziggurat algorithm, which provides efficient sampling
    ///     with bounded intermediate values suitable for all numeric types including fixed-point.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (T z1, T z2) NextStandardNormalPair<TRng>(ref TRng rng)
        where TRng : struct, IChiRngSource<TRng>
    {
        return ZigguratNormal<T>.NextPair(ref rng);
    }
}