// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChiVariate;

/// <summary>
///     Samples from a Chi distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerChiExtensions.Chi{TRng,T}" /> method.
/// </remarks>
public ref struct ChiSamplerChi<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private ChiSamplerChiSquared<TRng, T> _chiSquaredSampler;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerChi(ref TRng rng, T degreesOfFreedom)
    {
        if (!T.IsFinite(degreesOfFreedom) || degreesOfFreedom <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(degreesOfFreedom), "Degrees of freedom must be positive.");

        _chiSquaredSampler = new ChiSamplerChiSquared<TRng, T>(ref rng, degreesOfFreedom);
    }

    /// <summary>
    ///     Samples a single random value from the configured Chi distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var chiSquaredSample = _chiSquaredSampler.Sample();
        return ChiMath.Sqrt(chiSquaredSample);
    }

    /// <summary>
    ///     Generates a sequence of random values from the Chi distribution.
    /// </summary>
    /// <param name="count">The number of values to sample from the distribution.</param>
    /// <returns>An enumerable collection of values sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiEnumerable<T> Sample(int count)
    {
        var enumerable = ChiEnumerable<T>.Rent(count);
        var list = enumerable.List;
        for (var i = 0; i < list.Count; i++)
            list[i] = Sample();
        return enumerable;
    }
}

/// <summary>
///     Provides extension methods for sampling from the Chi distribution.
/// </summary>
public static class ChiSamplerChiExtensions
{
    /// <summary>
    ///     Returns a sampler for the Chi distribution, which models the Euclidean distance of k independent standard normal
    ///     variables from the origin.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="degreesOfFreedom">The degrees of freedom (k), which must be a positive integer.</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Models the magnitude of multi-dimensional Gaussian noise. Useful for calculating signal
    ///         strength from I/Q components, error distances in targeting systems, or the speed of a particle with random
    ///         velocity components.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> Amortized O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Sample the error distance in a 3D targeting system
    /// var errorDistance = rng.Chi(3.0).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerChi<TRng, T> Chi<TRng, T>(this ref TRng rng, T degreesOfFreedom)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerChi<TRng, T>(ref rng, degreesOfFreedom);
    }
}