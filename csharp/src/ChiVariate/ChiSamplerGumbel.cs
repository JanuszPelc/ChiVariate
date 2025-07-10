// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate;

/// <summary>
///     Samples from a Gumbel distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerGumbelExtensions.Gumbel{TRng,T}" /> method.
/// </remarks>
public readonly ref struct ChiSamplerGumbel<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private readonly ref TRng _rng;
    private readonly T _location;
    private readonly T _scale;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerGumbel(ref TRng rng, T location, T scale)
    {
        if (!T.IsFinite(location))
            throw new ArgumentOutOfRangeException(nameof(location), "Location (mu) must be a finite number.");
        if (!T.IsFinite(scale) || scale <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(scale), "Scale (beta) must be positive.");

        _rng = ref rng;
        _location = location;
        _scale = scale;
    }

    /// <summary>
    ///     Samples a single random value from the configured Gumbel distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var u = ChiRealProvider.Next<TRng, T>(ref _rng, ChiIntervalOptions.ExcludeMin);

        return _location - _scale * ChiMath.Log(-ChiMath.Log(u));
    }

    /// <summary>
    ///     Generates a sequence of random values from the Gumbel distribution.
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
///     Provides extension methods for sampling from the Gumbel distribution.
/// </summary>
public static class ChiSamplerGumbelExtensions
{
    /// <summary>
    ///     Returns a sampler for the Gumbel distribution, an extreme value distribution used to model the maximum (or minimum)
    ///     of a set of samples.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="location">The location parameter (μ).</param>
    /// <param name="scale">The scale parameter (β), which must be positive.</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Essential in risk management, engineering, and hydrology for modeling events like maximum
    ///         floods or wind speeds. In Machine Learning, it is the foundation of the Gumbel-Max trick for training
    ///         generative models to make differentiable discrete choices.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Sample a plausible maximum daily high temperature
    /// var maxTemp = rng.Gumbel(25.0, 2.0).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerGumbel<TRng, T> Gumbel<TRng, T>(this ref TRng rng, T location, T scale)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerGumbel<TRng, T>(ref rng, location, scale);
    }
}