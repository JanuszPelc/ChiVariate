// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate;

/// <summary>
///     Samples from a Pseudo-Random Distribution (PRD) with escalating probability.
/// </summary>
/// <remarks>
///     <para>
///         This sampler implements the industry-standard "pity system" found in games like Dota 2 and Genshin Impact.
///         Unlike independent Bernoulli trials, the probability escalates after each failure, bounding worst-case
///         streaks while maintaining randomness.
///     </para>
///     <para>
///         The algorithm works as follows:
///     </para>
///     <list type="number">
///         <item>
///             <description>Start with base probability</description>
///         </item>
///         <item>
///             <description>On failure: increment failure count; if past soft pity threshold, increase probability</description>
///         </item>
///         <item>
///             <description>On success (or hard cap reached): reset to base probability</description>
///         </item>
///     </list>
/// </remarks>
public ref struct ChiSamplerPity<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private readonly ref TRng _rng;
    private readonly T _baseProbability;
    private readonly T _increment;
    private readonly int _softPityThreshold;
    private readonly int _hardPityCap;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerPity(ref TRng rng, T baseProbability, T increment, int softPityThreshold, int hardPityCap)
    {
        if (!T.IsFinite(baseProbability) || baseProbability < T.Zero || baseProbability > T.One)
            throw new ArgumentOutOfRangeException(nameof(baseProbability),
                "Base probability must be between 0 and 1.");
        if (!T.IsFinite(increment) || increment < T.Zero)
            throw new ArgumentOutOfRangeException(nameof(increment),
                "Increment must be non-negative.");
        if (softPityThreshold < 0)
            throw new ArgumentOutOfRangeException(nameof(softPityThreshold),
                "Soft pity threshold must be non-negative.");
        if (hardPityCap < 0)
            throw new ArgumentOutOfRangeException(nameof(hardPityCap),
                "Hard pity cap must be non-negative.");

        _rng = ref rng;
        _baseProbability = baseProbability;
        _increment = increment;
        _softPityThreshold = softPityThreshold;
        _hardPityCap = hardPityCap;
        CurrentProbability = baseProbability;
        FailureCount = 0;
    }

    /// <summary>
    ///     Gets the current number of consecutive failures.
    /// </summary>
    public int FailureCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        private set;
    }

    /// <summary>
    ///     Gets the current probability of success (escalates after soft pity threshold).
    /// </summary>
    public T CurrentProbability
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        private set;
    }

    /// <summary>
    ///     Samples once from the pity distribution.
    /// </summary>
    /// <returns>1 for success (state resets), 0 for failure (probability escalates).</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Sample()
    {
        var u = ChiRealProvider.Next<TRng, T>(ref _rng);

        if (u < CurrentProbability)
        {
            Reset();
            return 1;
        }

        FailureCount++;

        if (_hardPityCap > 0 && FailureCount >= _hardPityCap)
        {
            Reset();
            return 1;
        }

        if (FailureCount > _softPityThreshold)
        {
            CurrentProbability += _increment;
            if (CurrentProbability > T.One)
                CurrentProbability = T.One;
        }

        return 0;
    }

    /// <summary>
    ///     Samples multiple times from the pity distribution.
    /// </summary>
    /// <param name="count">The number of times to sample.</param>
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Reset()
    {
        CurrentProbability = _baseProbability;
        FailureCount = 0;
    }
}

/// <summary>
///     Provides extension methods for sampling from the Pity (PRD) distribution.
/// </summary>
public static class ChiSamplerPityExtensions
{
    /// <summary>
    ///     Returns a sampler for a Pseudo-Random Distribution (PRD) with escalating probability,
    ///     commonly known as a "pity system".
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the probabilities.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="baseProbability">The starting probability of success (0 to 1).</param>
    /// <param name="increment">
    ///     The amount to add to probability after each failure past the soft pity threshold.
    /// </param>
    /// <param name="softPityThreshold">
    ///     The number of failures before probability starts escalating. Default is 0 (immediate escalation).
    /// </param>
    /// <param name="hardPityCap">
    ///     The number of failures that guarantees success. Default is 0 (no hard cap).
    /// </param>
    /// <returns>A sampler that can be used to generate pity-based random outcomes.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Implements fair randomness for games where bad luck streaks feel punishing.
    ///         Common examples include critical hit rates, loot drops, gacha pulls, and proc chances.
    ///         Used in games like Dota 2 (Skull Basher, etc.) and Genshin Impact (wish system).
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // 2% base chance, +1% per failure after 50 failures, guaranteed at 90
    /// var lootDrop = rng.Pity(0.02, 0.01, softPityThreshold: 50, hardPityCap: 90);
    ///
    /// while (lootDrop.Sample() == 0)
    ///     Console.WriteLine($"Miss #{lootDrop.FailureCount}");
    /// Console.WriteLine("Got the rare item!");
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerPity<TRng, T> Pity<TRng, T>(
        this ref TRng rng,
        T baseProbability,
        T increment,
        int softPityThreshold = 0,
        int hardPityCap = 0)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerPity<TRng, T>(ref rng, baseProbability, increment, softPityThreshold, hardPityCap);
    }
}