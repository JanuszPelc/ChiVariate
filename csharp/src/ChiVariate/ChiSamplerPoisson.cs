// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate;

/// <summary>
///     Samples from a Poisson distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerPoissonExtensions.Poisson{TRng}" /> method.
/// </remarks>
public readonly ref struct ChiSamplerPoisson<TRng, T>(ref TRng rng, double mean)
    where TRng : struct, IChiRngSource<TRng>
    where T : unmanaged, INumberBase<T>
{
    private readonly ref TRng _rng = ref rng;

    private readonly double _mean = mean >= 0
        ? mean
        : throw new ArgumentOutOfRangeException(nameof(mean), "Mean must be non-negative.");

    /// <summary>
    ///     Samples a single random value from the configured Poisson distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var limit = Math.Exp(-_mean);
        var p = ChiRealProvider.Next<TRng, double>(ref _rng, ChiIntervalOptions.ExcludeMin);
        var k = T.Zero;

        while (p > limit)
        {
            k++;
            p *= ChiRealProvider.Next<TRng, double>(ref _rng, ChiIntervalOptions.ExcludeMin);
        }

        return k;
    }

    /// <summary>
    ///     Generates a sequence of random values from the Poisson distribution.
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
///     Provides extension methods for sampling from the Poisson distribution.
/// </summary>
public static class ChiSamplerPoissonExtensions
{
    /// <summary>
    ///     Returns a sampler for the Poisson distribution, which models the number of events occurring in a fixed interval of
    ///     time or space.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="mean">The mean (λ, lambda) number of occurrences, which must be positive.</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Models counts of "bursty" or rare events that occur at a steady average rate, such as the
    ///         number of emails arriving per minute, customers visiting a store per hour, or random encounters in a game
    ///         level.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(λ), meaning performance is proportional to the mean.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Sample the number of customers arriving in the next minute (mean = 2.5)
    /// var customerCount = rng.Poisson(2.5).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerPoisson<TRng, int> Poisson<TRng>(this ref TRng rng, double mean)
        where TRng : struct, IChiRngSource<TRng>
    {
        return new ChiSamplerPoisson<TRng, int>(ref rng, mean);
    }
}