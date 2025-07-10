// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChiVariate.Providers;

/// <summary>
///     Provides a stateless method for generating pairs of standard normal variables.
/// </summary>
/// <typeparam name="T">The floating-point type of the generated values.</typeparam>
public static class ChiNormalProvider<T>
    where T : IFloatingPoint<T>
{
    private static readonly T Two = T.CreateChecked(2.0);
    private static readonly T NegativeTwo = -Two;

    /// <summary>
    ///     Generates a pair of independent, standard normal (Gaussian) random variables.
    /// </summary>
    /// <param name="rng">The random number generator to use.</param>
    /// <returns>A tuple containing two standard normal variables (z1, z2).</returns>
    /// <remarks>
    ///     This implementation uses the Marsaglia polar method, which is a variation of the Box-Muller transform.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (T z1, T z2) NextStandardNormalPair<TRng>(ref TRng rng)
        where TRng : struct, IChiRngSource<TRng>
    {
        T u1, u2, s;

        do
        {
            u1 = ChiRealProvider.Next<TRng, T>(ref rng);
            u2 = ChiRealProvider.Next<TRng, T>(ref rng);

            u1 = Two * u1 - T.One;
            u2 = Two * u2 - T.One;
            s = u1 * u1 + u2 * u2;
        } while (s >= T.One || s == T.Zero);

        var multiplier = ChiMath.Sqrt(NegativeTwo * ChiMath.Log(s) / s);

        var z1 = u1 * multiplier;
        var z2 = u2 * multiplier;

        return (z1, z2);
    }
}