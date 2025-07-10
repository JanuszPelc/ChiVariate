// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChiVariate;

/// <summary>
///     Samples from a Hypergeometric distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerHypergeometricExtensions.Hypergeometric{TRng,T}" /> method.
/// </remarks>
public readonly ref struct ChiSamplerHypergeometric<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : unmanaged, IBinaryInteger<T>
{
    private readonly ref TRng _rng;
    private readonly T _populationSize;
    private readonly T _numberOfSuccesses;
    private readonly T _sampleSize;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerHypergeometric(ref TRng rng, T populationSize, T numberOfSuccesses, T sampleSize)
    {
        if (!T.IsFinite(populationSize) || populationSize < T.Zero)
            throw new ArgumentOutOfRangeException(nameof(populationSize),
                "Population size cannot be negative.");
        if (!T.IsFinite(numberOfSuccesses) || numberOfSuccesses < T.Zero || numberOfSuccesses > populationSize)
            throw new ArgumentOutOfRangeException(nameof(numberOfSuccesses),
                "Number of successes must be between 0 and population size.");
        if (!T.IsFinite(sampleSize) || sampleSize < T.Zero || sampleSize > populationSize)
            throw new ArgumentOutOfRangeException(nameof(sampleSize),
                "Sample size must be between 0 and population size.");

        _rng = ref rng;
        _populationSize = populationSize;
        _numberOfSuccesses = numberOfSuccesses;
        _sampleSize = sampleSize;
    }

    /// <summary>
    ///     Samples a single random value from the configured Hypergeometric distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var N = _populationSize;
        var K = _numberOfSuccesses;
        var n = _sampleSize;

        var successesDrawn = T.Zero;

        for (var i = T.Zero; i < n; i++)
        {
            var p = double.CreateChecked(K) / double.CreateChecked(N);

            if (_rng.Chance().NextBool(p))
            {
                successesDrawn++;
                K--;
            }

            N--;

            if (K == T.Zero) break;
            if (N < n - i) break;
        }

        return successesDrawn;
    }

    /// <summary>
    ///     Generates a sequence of random values from the Hypergeometric distribution.
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
///     Provides extension methods for sampling from the Hypergeometric distribution.
/// </summary>
public static class ChiSamplerHypergeometricExtensions
{
    /// <summary>
    ///     Returns a sampler for the Hypergeometric distribution, which models the number of successes in a sample drawn
    ///     without replacement from a finite population.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The integer type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="populationSize">The total size of the population (N).</param>
    /// <param name="numberOfSuccesses">The number of items with the desired feature in the population (K).</param>
    /// <param name="sampleSize">The number of items drawn from the population (n).</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Models sampling without replacement, essential for card games ("drawing 5 cards from a
    ///         deck"), lottery systems, or quality control where items are not returned to the population after being
    ///         inspected.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(n) per sample, where n is the sample size.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // How many hearts in a 5-card hand from a standard 52-card deck?
    /// var heartCount = rng.Hypergeometric(52, 13, 5).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerHypergeometric<TRng, T> Hypergeometric<TRng, T>(this ref TRng rng,
        T populationSize, T numberOfSuccesses, T sampleSize)
        where TRng : struct, IChiRngSource<TRng>
        where T : unmanaged, IBinaryInteger<T>
    {
        return new ChiSamplerHypergeometric<TRng, T>(ref rng, populationSize, numberOfSuccesses, sampleSize);
    }
}