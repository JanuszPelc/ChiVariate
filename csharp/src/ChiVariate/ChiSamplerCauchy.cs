// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate;

/// <summary>
///     Samples from a Cauchy distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerCauchyExtensions.Cauchy{TRng,T}" /> method.
/// </remarks>
public readonly ref struct ChiSamplerCauchy<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private readonly ref TRng _rng;
    private readonly T _location;
    private readonly T _scale;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerCauchy(ref TRng rng, T location, T scale)
    {
        if (!T.IsFinite(location))
            throw new ArgumentOutOfRangeException(nameof(location), "Location (x₀) must be a finite number.");
        if (!T.IsFinite(scale) || scale <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(scale), "Scale (gamma) must be positive.");

        _rng = ref rng;
        _location = location;
        _scale = scale;
    }

    /// <summary>
    ///     Samples a single random value from the configured Cauchy distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var u = ChiRealProvider.Next<TRng, T>(ref _rng);
        var tanArg = T.Pi * (u - ChiMath.Const<T>.OneHalf);

        return _location + _scale * ChiMath.Tan(tanArg);
    }

    /// <summary>
    ///     Generates a sequence of random values from the Cauchy distribution.
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
///     Provides extension methods for sampling from the Cauchy distribution.
/// </summary>
public static class ChiSamplerCauchyExtensions
{
    /// <summary>
    ///     Returns a sampler for the Cauchy distribution, a bell-shaped curve with very heavy tails leading to frequent
    ///     extreme outliers.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="location">The location parameter (x₀), which specifies the median of the distribution.</param>
    /// <param name="scale">The scale parameter (γ), which controls the spread. Must be positive.</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Models physical resonance or systems prone to extreme events. Use it to create dramatic
    ///         visual effects in particle systems, simulate financial market volatility, or generate noise that includes
    ///         occasional, sharp spikes.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Sample a value for a dramatic particle effect
    /// var velocityKick = rng.Cauchy(0.0, 0.9).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerCauchy<TRng, T> Cauchy<TRng, T>(this ref TRng rng, T location, T scale)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerCauchy<TRng, T>(ref rng, location, scale);
    }
}