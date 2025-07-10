// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate;

/// <summary>
///     Samples from a Categorical distribution using Vose's Alias Method.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerCategoricalExtensions.Categorical{TRng,T}" /> method.
/// </remarks>
public ref struct ChiSamplerCategorical<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : unmanaged, IFloatingPoint<T>
{
    private readonly ref TRng _rng;
    private readonly bool _isUniform;

    private ChiVector<T> _probVector;
    private ChiVector<int> _aliasVector;

    /// <summary>
    ///     Gets the number of categories (or weights) in the distribution.
    /// </summary>
    public int Count { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerCategorical(ref TRng rng, ReadOnlySpan<T> weights)
    {
        if (weights.IsEmpty)
            throw new ArgumentException("Weights span cannot be empty.", nameof(weights));

        _rng = ref rng;
        Count = weights.Length;

        if (AreAllWeightsEqual(weights))
        {
            _isUniform = true;
            return;
        }

        _isUniform = false;

        _probVector = ChiVector.Unsafe.Uninitialized<T>(Count);
        _aliasVector = ChiVector.Unsafe.Uninitialized<int>(Count);

        var n = T.CreateChecked(Count);
        var sum = T.Zero;
        foreach (var w in weights)
        {
            if (!T.IsFinite(w) || w < T.Zero)
                throw new ArgumentException("All weights must be non-negative and finite.", nameof(weights));
            sum += w;
        }

        if (sum <= T.Zero)
            throw new ArgumentException("Sum of weights must be positive.", nameof(weights));

        var probSpan = _probVector.Span;
        var aliasSpan = _aliasVector.Span;

        for (var i = 0; i < Count; i++)
            probSpan[i] = weights[i] * n / sum;

        using var small = ChiVector.Unsafe.Uninitialized<int>(Count);
        using var large = ChiVector.Unsafe.Uninitialized<int>(Count);

        var (smallCount, largeCount) = BuildAliasTables(probSpan, aliasSpan, large.Span, small.Span, Count);

        while (largeCount > 0)
            probSpan[large[--largeCount]] = T.One;
        while (smallCount > 0)
            probSpan[small[--smallCount]] = T.One;
    }

    /// <summary>
    ///     Samples a single random category index from the configured Categorical distribution.
    /// </summary>
    /// <returns>A new integer index sampled from the distribution, from 0 to k-1.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Sample()
    {
        if (_isUniform)
            return ChiIntegerProvider.Next(ref _rng, 0, Count);

        var i = ChiIntegerProvider.Next(ref _rng, 0, Count);
        var u = ChiRealProvider.Next<TRng, T>(ref _rng);

        var probSpan = _probVector.Span;
        var aliasSpan = _aliasVector.Span;

        return u < probSpan[i] ? i : aliasSpan[i];
    }

    /// <summary>
    ///     Generates a sequence of random category indices from the Categorical distribution.
    /// </summary>
    /// <param name="count">The number of indices to sample from the distribution.</param>
    /// <returns>An enumerable collection of indices sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiEnumerable<int> Sample(int count)
    {
        var enumerable = ChiEnumerable<int>.Rent(count);
        var list = enumerable.List;
        for (var i = 0; i < list.Count; i++)
            list[i] = Sample();
        return enumerable;
    }

    #region Helper Methods

    private static (int, int) BuildAliasTables(
        Span<T> probSpan, Span<int> aliasSpan, Span<int> large, Span<int> small, int count)
    {
        var smallCount = 0;
        var largeCount = 0;

        for (var i = 0; i < count; i++)
            if (probSpan[i] < T.One)
                small[smallCount++] = i;
            else
                large[largeCount++] = i;

        while (smallCount > 0 && largeCount > 0)
        {
            var l = small[--smallCount];
            var g = large[--largeCount];

            aliasSpan[l] = g;
            probSpan[g] = probSpan[g] + probSpan[l] - T.One;

            if (probSpan[g] < T.One)
                small[smallCount++] = g;
            else
                large[largeCount++] = g;
        }

        return (smallCount, largeCount);
    }

    private static bool AreAllWeightsEqual(ReadOnlySpan<T> weights)
    {
        if (weights.Length <= 1) return true;

        var firstWeight = weights[0];
        if (!T.IsFinite(firstWeight) || firstWeight <= T.Zero)
            throw new ArgumentException("All weights must be positive and finite.", nameof(weights));

        for (var i = 1; i < weights.Length; i++)
        {
            var currentWeight = weights[i];
            if (!T.IsFinite(currentWeight) || currentWeight <= T.Zero)
                throw new ArgumentException("All weights must be positive and finite.", nameof(weights));

            if (currentWeight != firstWeight) return false;
        }

        return true;
    }

    #endregion
}

/// <summary>
///     Provides extension methods for sampling from the Categorical distribution.
/// </summary>
public static class ChiSamplerCategoricalExtensions
{
    /// <inheritdoc cref="SharedDocumentation.Categorical{TRng,T}" />
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Define spawn weights for Common, Uncommon, Rare, and Epic
    /// var spawnWeights = new[] { 50.0, 30.0, 15.0, 5.0 };
    /// var enemyTypeIndex = rng.Categorical(spawnWeights).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerCategorical<TRng, T> Categorical<TRng, T>(this ref TRng rng, T[] weights)
        where TRng : struct, IChiRngSource<TRng>
        where T : unmanaged, IFloatingPoint<T>
    {
        return new ChiSamplerCategorical<TRng, T>(ref rng, (ReadOnlySpan<T>)weights);
    }
}

/// <summary>
///     Provides non-allocating extension methods for sampling from the Categorical distribution.
/// </summary>
public static class ChiSamplerCategoricalSpanExtensions
{
    /// <inheritdoc cref="SharedDocumentation.Categorical{TRng,T}" />
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Define spawn weights for Common, Uncommon, Rare, and Epic
    /// ReadOnlySpan<double> spawnWeights = [ 50.0, 30.0, 15.0, 5.0 ];
    /// var enemyTypeIndex = rng.Categorical(spawnWeights).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerCategorical<TRng, T> Categorical<TRng, T>(this ref TRng rng, ReadOnlySpan<T> weights)
        where TRng : struct, IChiRngSource<TRng>
        where T : unmanaged, IFloatingPoint<T>
    {
        return new ChiSamplerCategorical<TRng, T>(ref rng, weights);
    }
}

// ReSharper disable once UnusedType.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local
file static class SharedDocumentation
{
    /// <summary>
    ///     Returns a sampler for the Categorical distribution, which selects an outcome from a set of discrete probabilities.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the probabilities.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="weights">
    ///     A vector of weights (p) for each category. All must be positive, but they do not need to
    ///     sum to 1.
    /// </param>
    /// <returns>A sampler that can be used to generate random category indices.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Chooses one option from multiple categories with different likelihoods, such as which type of
    ///         enemy spawns, which item drops from a weighted loot table, or which response is chosen in a survey.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(1) per sample, with a one-time O(k) setup cost, where k is the number of categories.
    ///         For small arrays (≤25 elements), uses zero-allocation inline storage.
    ///     </para>
    /// </remarks>
    private static ChiSamplerCategorical<TRng, T> Categorical<TRng, T>(this ref TRng rng, T[] weights)
        where TRng : struct, IChiRngSource<TRng>
        where T : unmanaged, IFloatingPoint<T>
    {
        throw new UnreachableException();
    }
}
// ReSharper restore UnusedParameter.Local
// ReSharper restore UnusedMember.Local