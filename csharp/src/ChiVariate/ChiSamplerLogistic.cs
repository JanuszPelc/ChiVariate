// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate;

/// <summary>
///     Samples from a Logistic distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerLogisticExtensions.Logistic{TRng,T}" /> method.
/// </remarks>
public readonly ref struct ChiSamplerLogistic<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private readonly ref TRng _rng;
    private readonly T _location;
    private readonly T _scale;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerLogistic(ref TRng rng, T location, T scale)
    {
        if (!T.IsFinite(scale) || scale <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(scale), "Scale parameter (s) must be positive.");

        _rng = ref rng;
        _location = location;
        _scale = scale;
    }

    /// <summary>
    ///     Samples a single random value from the configured Logistic distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var u = ChiRealProvider.Next<TRng, T>(ref _rng, ChiIntervalOptions.ExcludeMin);

        return _location + _scale * ChiMath.Log(u / (T.One - u));
    }

    /// <summary>
    ///     Generates a sequence of random values from the Logistic distribution.
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
///     Provides extension methods for sampling from the Logistic distribution.
/// </summary>
public static class ChiSamplerLogisticExtensions
{
    /// <summary>
    ///     Returns a sampler for the Logistic distribution, a bell-shaped curve similar to the Normal but with heavier tails.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="location">The location parameter (μ), which is the mean and median of the distribution.</param>
    /// <param name="scale">The scale parameter (s), which controls the spread. Must be positive.</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> The basis for logistic regression in machine learning. Useful for modeling population growth,
    ///         dose-response curves, or any process that follows an S-shaped (sigmoid) curve.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Sample from a standard Logistic distribution
    /// var value = rng.Logistic(0.0, 1.0).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerLogistic<TRng, T> Logistic<TRng, T>(this ref TRng rng, T location, T scale)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerLogistic<TRng, T>(ref rng, location, scale);
    }
}