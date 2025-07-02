using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChiVariate;

/// <summary>
///     Samples from a Negative Binomial distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerNegativeBinomialExtensions.NegativeBinomial{TRng,T}" />
///     method.
/// </remarks>
public readonly ref struct ChiSamplerNegativeBinomial<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : unmanaged, IBinaryInteger<T>
{
    private readonly ref TRng _rng;
    private readonly T _numSuccesses;
    private readonly double _probability;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerNegativeBinomial(ref TRng rng, T numSuccesses, double probability)
    {
        if (!T.IsFinite(numSuccesses) || numSuccesses <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(numSuccesses), "Number of successes must be positive.");
        if (!double.IsFinite(probability) || probability <= 0.0 || probability > 1.0)
            throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be in the (0, 1] interval.");

        _rng = ref rng;
        _numSuccesses = numSuccesses;
        _probability = probability;
    }

    /// <summary>
    ///     Samples a single random value from the configured Negative Binomial distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var trials = T.Zero;
        var successes = T.Zero;

        while (successes < _numSuccesses)
        {
            trials++;
            if (_rng.Chance().NextBool(_probability))
                successes++;
        }

        return trials;
    }

    /// <summary>
    ///     Generates a sequence of random values from the Negative Binomial distribution.
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
///     Provides extension methods for sampling from the Negative Binomial distribution.
/// </summary>
public static class ChiSamplerNegativeBinomialExtensions
{
    /// <summary>
    ///     Returns a sampler for the Negative Binomial distribution, which models the number of trials required to achieve a
    ///     fixed number of successes.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The integer type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="numSuccesses">The target number of successes (r), which must be a positive integer.</param>
    /// <param name="probability">The probability of success (p) on any given trial. Must be in the interval (0, 1].</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Models how many attempts are needed to reach a target number of successes, useful for
    ///         scenarios like customer acquisition (how many contacts to get 10 sales), reliability testing, or game mechanics
    ///         with accumulating success.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(r/p), meaning performance is proportional to the expected number of trials.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // How many contacts to make to get 10 sales, with a 5% success rate?
    /// var contactsNeeded = rng.NegativeBinomial(10, 0.05).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerNegativeBinomial<TRng, T> NegativeBinomial<TRng, T>(this ref TRng rng, T numSuccesses,
        double probability)
        where TRng : struct, IChiRngSource<TRng>
        where T : unmanaged, IBinaryInteger<T>
    {
        return new ChiSamplerNegativeBinomial<TRng, T>(ref rng, numSuccesses, probability);
    }
}