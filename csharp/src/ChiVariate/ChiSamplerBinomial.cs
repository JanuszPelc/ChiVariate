using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChiVariate;

/// <summary>
///     Samples from a Binomial distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerBinomialExtensions.Binomial{TRng,T}" /> method.
/// </remarks>
public readonly ref struct ChiSamplerBinomial<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : unmanaged, IBinaryInteger<T>
{
    private readonly ref TRng _rng;
    private readonly T _numberOfTrials;
    private readonly double _probability;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerBinomial(ref TRng rng, T numberOfTrials, double probability)
    {
        if (!T.IsFinite(numberOfTrials) || numberOfTrials < T.Zero)
            throw new ArgumentOutOfRangeException(nameof(numberOfTrials), "Number of trials must be non-negative.");
        if (!double.IsFinite(probability) || probability < 0.0 || probability > 1.0)
            throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be between 0.0 and 1.0.");

        _rng = ref rng;
        _numberOfTrials = numberOfTrials;
        _probability = probability;
    }

    /// <summary>
    ///     Samples a single random value from the configured Binomial distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution, representing the number of successes.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var successes = T.Zero;
        for (var i = T.Zero; i < _numberOfTrials; i++)
            if (_rng.Chance().NextBool(_probability))
                successes++;

        return successes;
    }

    /// <summary>
    ///     Generates a sequence of random values from the Binomial distribution.
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
///     Provides extension methods for sampling from the Binomial distribution.
/// </summary>
public static class ChiSamplerBinomialExtensions
{
    /// <summary>
    ///     Returns a sampler for the Binomial distribution, which models the number of successes in a fixed number of
    ///     independent trials.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The integer type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="numberOfTrials">The number of trials (n), which must be a non-negative integer.</param>
    /// <param name="probability">The probability of success (p) on any given trial. Must be in the interval [0, 1].</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Counts successes in repeated events, such as how many shots hit a target out of 10 attempts
    ///         or how many customers make a purchase out of 100 visitors. Essential for quality control and A/B testing
    ///         scenarios.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(n), where n is the number of trials.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // How many of 20 shots hit, if the hit chance is 75%?
    /// var hits = rng.Binomial(20, 0.75).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerBinomial<TRng, T> Binomial<TRng, T>(this ref TRng rng, T numberOfTrials, double probability)
        where TRng : struct, IChiRngSource<TRng>
        where T : unmanaged, IBinaryInteger<T>
    {
        return new ChiSamplerBinomial<TRng, T>(ref rng, numberOfTrials, probability);
    }
}