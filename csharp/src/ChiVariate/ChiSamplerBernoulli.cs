using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Generators;

namespace ChiVariate;

/// <summary>
///     Samples from a Bernoulli distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerBernoulliExtensions.Bernoulli{TRng,T}" /> method.
/// </remarks>
public readonly ref struct ChiSamplerBernoulli<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private readonly ref TRng _rng;
    private readonly T _probability;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerBernoulli(ref TRng rng, T probability)
    {
        if (!T.IsFinite(probability) || probability < T.Zero || probability > T.One)
            throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be between 0.0 and 1.0.");

        _rng = ref rng;
        _probability = probability;
    }

    /// <summary>
    ///     Samples a single random value from the configured Bernoulli distribution.
    /// </summary>
    /// <returns>1 for success, 0 for failure.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Sample()
    {
        return ChiRealGenerator.Next<TRng, T>(ref _rng) < _probability ? 1 : 0;
    }

    /// <summary>
    ///     Generates a sequence of random values from the Bernoulli distribution.
    /// </summary>
    /// <param name="count">The number of values to sample from the distribution.</param>
    /// <returns>An enumerable collection of values (1s and 0s) sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiEnumerable<int> Sample(int count)
    {
        var enumerable = ChiEnumerable<int>.Rent(count);
        var list = enumerable.List;
        for (var i = 0; i < list.Count; i++)
            list[i] = Sample();
        return enumerable;
    }
}

/// <summary>
///     Provides extension methods for sampling from the Bernoulli distribution.
/// </summary>
public static class ChiSamplerBernoulliExtensions
{
    /// <summary>
    ///     Returns a sampler for the Bernoulli distribution, which models a single trial that results in 1 (success) or 0
    ///     (failure).
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the probability.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="probability">The probability of success (p). Must be in the interval [0, 1].</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Models a single yes/no outcome, like whether a player succeeds at a skill check or an event
    ///         is detected. Perfect for coin flips, critical hits, or any binary decision with a known probability.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Did the player's 30% chance skill check succeed?
    /// var isSuccess = rng.Bernoulli(0.3).Sample() == 1;
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerBernoulli<TRng, T> Bernoulli<TRng, T>(this ref TRng rng, T probability)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerBernoulli<TRng, T>(ref rng, probability);
    }
}