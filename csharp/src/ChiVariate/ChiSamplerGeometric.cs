using System.Runtime.CompilerServices;

namespace ChiVariate;

/// <summary>
///     Samples from a Geometric distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerGeometricExtensions.Geometric{TRng}" /> method.
/// </remarks>
public readonly ref struct ChiSamplerGeometric<TRng>
    where TRng : struct, IChiRngSource<TRng>
{
    private readonly ref TRng _rng;
    private readonly double _probability;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerGeometric(ref TRng rng, double probability)
    {
        if (!double.IsFinite(probability) || probability <= 0.0 || probability > 1.0)
            throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be in the (0, 1] interval.");

        _rng = ref rng;
        _probability = probability;
    }

    /// <summary>
    ///     Samples a single random value from the configured Geometric distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Sample()
    {
        var trials = 1;
        while (!_rng.Chance().NextBool(_probability))
            trials++;

        return trials;
    }

    /// <summary>
    ///     Generates a sequence of random values from the Geometric distribution.
    /// </summary>
    /// <param name="count">The number of values to sample from the distribution.</param>
    /// <returns>An enumerable collection of values sampled from the distribution.</returns>
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
///     Provides extension methods for sampling from the Geometric distribution.
/// </summary>
public static class ChiSamplerGeometricExtensions
{
    /// <summary>
    ///     Returns a sampler for the Geometric distribution, which models the number of trials needed to achieve the first
    ///     success.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="probability">The probability of success (p) on any given trial. Must be in the interval (0, 1].</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Models how many attempts it takes to succeed once, like the number of attacks needed to land
    ///         a critical hit, the number of API calls to get a successful response, or the iterations required for a search
    ///         algorithm to find a match.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(1/p), meaning performance is proportional to the expected number of trials.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // How many attacks to land a critical hit with a 10% chance?
    /// var attempts = rng.Geometric(0.1).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerGeometric<TRng> Geometric<TRng>(this ref TRng rng, double probability)
        where TRng : struct, IChiRngSource<TRng>
    {
        return new ChiSamplerGeometric<TRng>(ref rng, probability);
    }
}