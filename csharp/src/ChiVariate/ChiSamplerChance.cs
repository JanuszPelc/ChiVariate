// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate;

/// <summary>
///     A sampler providing a toolkit for common, expressive randomization tasks.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerChanceExtensions.Chance{TRng}" /> method.
///     It serves as a zero-allocation, statistically robust replacement for <c>System.Random</c>.
/// </remarks>
public readonly ref struct ChiSamplerChance<TRng>(ref TRng rng)
    where TRng : struct, IChiRngSource<TRng>
{
    private readonly ref TRng _rng = ref rng;

    /// <summary>
    ///     Simulates rolling a die with a specified number of sides.
    /// </summary>
    /// <param name="sides">The number of sides on the die. Defaults to 6.</param>
    /// <returns>A random integer between 1 and <paramref name="sides" /> (inclusive).</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int RollDie(int sides = 6)
    {
        return ChiIntegerProvider.Next(ref _rng, 1, sides + 1);
    }

    /// <summary>
    ///     Returns true with a probability of 1 in <paramref name="chances" />.
    /// </summary>
    /// <param name="chances">The denominator of the probability. Must be greater than 0.</param>
    /// <returns><c>true</c> with a 1/<paramref name="chances" /> probability, otherwise <c>false</c>.</returns>
    /// <example>
    ///     <code><![CDATA[
    /// bool isCriticalHit = rng.Chance().OneIn(20); // 5% chance
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool OneIn(int chances)
    {
        if (chances <= 0)
            throw new ArgumentOutOfRangeException(nameof(chances), "Chance must be > 0.");
        return ChiIntegerProvider.Next(ref _rng, 0, chances) == 0;
    }

    /// <summary>
    ///     Picks a random integer from the specified inclusive range.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T PickBetween<T>(T minInclusive, T maxInclusive)
        where T : IBinaryInteger<T>, IMinMaxValue<T>
    {
        return ChiIntegerProvider.Next(ref _rng, minInclusive, maxInclusive + T.One);
    }

    /// <summary>
    ///     Picks a random value from any <see langword="enum" /> type.
    ///     Enum fields decorated with <see cref="ChiEnumWeightAttribute" /> will be weighted accordingly.
    ///     Fields without the attribute receive a default weight of 1.0 for equal probability.
    /// </summary>
    /// <typeparam name="T">The enum type to pick from.</typeparam>
    /// <returns>A randomly selected value of the enum <typeparamref name="T" />.</returns>
    /// <example>
    ///     <code><![CDATA[
    /// public enum WeatherType
    /// {
    ///     [ChiEnumWeight(49.0)] Sunny,  // 49%
    ///     [ChiEnumWeight(29.0)] Cloudy, // 29%
    ///     [ChiEnumWeight(20.0)] Rainy,  // 20%
    ///     Snowy, // Default weight 1.0 = 1%
    ///     Foggy  // Default weight 1.0 = 1%
    /// }
    /// // Total weight: 49 + 29 + 20 + 1 + 1 = 100
    ///     
    /// var weather = rng.PickEnum<WeatherType>();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T PickEnum<T>()
        where T : unmanaged, Enum
    {
        var valuesSpan = ChiEnum<T>.Values;
        var weightsSpan = ChiEnum<T>.Weights;

        return weightsSpan.IsEmpty
            ? PickItem(valuesSpan)
            : PickItem(valuesSpan, weightsSpan);
    }

    /// <summary>
    ///     Shuffles the elements of a span in-place using the Fisher-Yates algorithm.
    /// </summary>
    /// <param name="span">The span of elements to shuffle.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Shuffle<T>(scoped Span<T> span)
    {
        for (var i = 0; i < span.Length - 1; i++)
        {
            var j = ChiIntegerProvider.Next(ref _rng, i, span.Length);
            if (i != j) (span[i], span[j]) = (span[j], span[i]);
        }
    }

    /// <summary>
    ///     Fills the destination span with items chosen randomly from the source span (with replacement).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetItems<T>(scoped ReadOnlySpan<T> choices, Span<T> destination)
    {
        if (choices.IsEmpty)
            throw new ArgumentException("Choices span cannot be empty", nameof(choices));

        for (var i = 0; i < destination.Length; i++)
        {
            var index = ChiIntegerProvider.Next(ref _rng, 0, choices.Length);
            destination[i] = choices[index];
        }
    }

    /// <summary>
    ///     Fills the destination span with items chosen from the source, preferring unique items first.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetItemsPreferUnique<T>(scoped ReadOnlySpan<T> choices, Span<T> destination)
    {
        if (choices.IsEmpty)
        {
            if (destination.IsEmpty) return;
            throw new ArgumentException("Choices span cannot be empty.", nameof(choices));
        }

        var indexBuffer = ArrayPool<int>.Shared.Rent(choices.Length);
        try
        {
            var indices = indexBuffer.AsSpan(0, choices.Length);
            for (var i = 0; i < indices.Length; i++)
                indices[i] = i;

            var destinationRemaining = destination;

            while (destinationRemaining.Length > 0)
            {
                Shuffle(indices);

                var batchSize = Math.Min(indices.Length, destinationRemaining.Length);
                for (var i = 0; i < batchSize; i++)
                    destinationRemaining[i] = choices[indices[i]];

                destinationRemaining = destinationRemaining[batchSize..];
            }
        }
        finally
        {
            ArrayPool<int>.Shared.Return(indexBuffer);
        }
    }

    /// <summary>
    ///     Picks a single random item from a span.
    /// </summary>
    /// <param name="choices">A non-empty span of items to choose from.</param>
    /// <returns>A randomly selected item from the span.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T PickItem<T>(scoped ReadOnlySpan<T> choices)
    {
        if (choices.IsEmpty)
            throw new ArgumentException("Choices span cannot be empty.", nameof(choices));

        var index = ChiIntegerProvider.Next(ref _rng, 0, choices.Length);
        return choices[index];
    }

    /// <summary>
    ///     Picks a single random item from a span, according to a set of corresponding weights.
    /// </summary>
    /// <param name="items">A span of items to choose from.</param>
    /// <param name="weights">A span of weights corresponding to each item. Higher weights are more likely to be chosen.</param>
    /// <returns>A randomly selected item from the span, based on the provided weights.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T PickItem<T>(scoped ReadOnlySpan<T> items, ReadOnlySpan<double> weights)
    {
        if (items.Length != weights.Length)
            throw new ArgumentException("Items and weights must have the same length.");

        if (items.IsEmpty)
            throw new ArgumentException("Items span cannot be empty.", nameof(items));

        var categorical = _rng.Categorical(weights);
        var index = categorical.Sample();

        return items[index];
    }

    /// <summary>
    ///     Returns a non-negative random 32-bit integer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Next()
    {
        return ChiIntegerProvider.Next<TRng, int>(ref _rng);
    }

    /// <summary>
    ///     Returns a non-negative random 32-bit integer that is less than the specified maximum.
    /// </summary>
    /// <param name="maxExclusive">The exclusive upper bound of the random number to be generated.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Next(int maxExclusive)
    {
        return ChiIntegerProvider.Next(ref _rng, 0, maxExclusive);
    }

    /// <summary>
    ///     Returns a random 32-bit integer that is within a specified range.
    /// </summary>
    /// <param name="minInclusive">The inclusive lower bound of the random number returned.</param>
    /// <param name="maxExclusive">The exclusive upper bound of the random number returned.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Next(int minInclusive, int maxExclusive)
    {
        return ChiIntegerProvider.Next(ref _rng, minInclusive, maxExclusive);
    }

    /// <summary>
    ///     Returns a random integer of the specified type.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Next<T>() where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        return ChiIntegerProvider.Next<TRng, T>(ref _rng);
    }

    /// <summary>
    ///     Returns a random integer of the specified type that is less than the specified maximum.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Next<T>(T maxExclusive) where T : IBinaryInteger<T>, IMinMaxValue<T>
    {
        return ChiIntegerProvider.Next(ref _rng, T.Zero, maxExclusive);
    }

    /// <summary>
    ///     Returns a random integer of the specified type that is within a specified range.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Next<T>(T minInclusive, T maxExclusive) where T : IBinaryInteger<T>, IMinMaxValue<T>
    {
        return ChiIntegerProvider.Next(ref _rng, minInclusive, maxExclusive);
    }

    /// <summary>
    ///     Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float NextSingle(ChiIntervalOptions options = ChiIntervalOptions.None)
    {
        return ChiRealProvider.Next<TRng, float>(ref _rng, options);
    }

    /// <summary>
    ///     Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double NextDouble(ChiIntervalOptions options = ChiIntervalOptions.None)
    {
        return ChiRealProvider.Next<TRng, double>(ref _rng, options);
    }

    /// <summary>
    ///     Returns a random boolean value.
    /// </summary>
    /// <param name="probability">The probability of returning <c>true</c>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool NextBool<T>(T probability)
        where T : IFloatingPoint<T>
    {
        return _rng.Bernoulli(probability).Sample() > 0;
    }

    /// <summary>
    ///     Returns a random sign, either 1 or -1.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int NextSign()
    {
        return _rng.Bernoulli(0.5).Sample() > 0 ? 1 : -1;
    }

    /// <summary>
    ///     Returns a random angle between 0 and 2π radians.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float NextAngleRadians()
    {
        return NextSingle() * (2.0f * MathF.PI);
    }

    /// <summary>
    ///     Returns a random angle between 0 and 360 degrees.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float NextAngleDegrees()
    {
        return NextSingle() * 360.0f;
    }

    /// <summary>
    ///     Simulates a fair coin flip.
    /// </summary>
    /// <returns><c>true</c> for heads, <c>false</c> for tails.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool FlipCoin()
    {
        return NextBool(0.5);
    }
}

/// <summary>
///     Provides extension methods for accessing the general-purpose randomization toolkit.
/// </summary>
public static class ChiSamplerChanceExtensions
{
    /// <summary>
    ///     Returns a handy toolkit for general-purpose randomness and collection manipulation.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <param name="rng">The random number generator to use.</param>
    /// <returns>A sampler that provides access to common randomization methods.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Provides an API for common tasks like dice rolls (`RollDie`), coin flips (`FlipCoin`), random
    ///         selections (`PickItem`), and shuffling (`Shuffle`). It serves as a zero-allocation, statistically robust
    ///         replacement for `System.Random`.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> Generally O(1), except for collection methods like `Shuffle` (O(n)).
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// int damage = rng.Chance().RollDie(8) + rng.Chance().RollDie(8); // 2d8
    /// bool didCrit = rng.Chance().OneIn(10); // 10% chance
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerChance<TRng> Chance<TRng>(ref this TRng rng)
        where TRng : struct, IChiRngSource<TRng>
    {
        return new ChiSamplerChance<TRng>(ref rng);
    }
}