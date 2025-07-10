// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChiVariate;

/// <summary>
///     Samples from a Multinomial distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerMultinomialExtensions.Multinomial{TRng,T}" /> method.
/// </remarks>
public ref struct ChiSamplerMultinomial<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : unmanaged, IBinaryInteger<T>
{
    private readonly T _numberOfTrials;
    private ChiSamplerCategorical<TRng, double> _categoricalSampler;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerMultinomial(ref TRng rng, T numberOfTrials, ReadOnlySpan<double> probabilities)
    {
        if (!T.IsFinite(numberOfTrials) || numberOfTrials < T.Zero)
            throw new ArgumentOutOfRangeException(nameof(numberOfTrials), "Number of trials cannot be negative.");
        if (probabilities.IsEmpty)
            throw new ArgumentException("Probabilities span cannot be empty.", nameof(probabilities));

        foreach (var t in probabilities)
            if (!double.IsFinite(t) || t <= 0)
                throw new ArgumentException("All probabilities must be positive.", nameof(probabilities));

        _numberOfTrials = numberOfTrials;
        _categoricalSampler = rng.Categorical(probabilities);
    }

    /// <summary>
    ///     Generates a vector of counts for the number of outcomes in each category after a fixed number of trials.
    /// </summary>
    /// <returns>A new <see cref="ChiVector{T}" /> containing the outcome counts, which will sum to the total number of trials.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiVector<T> Sample()
    {
        var counts = ChiVector.Zeros<T>(_categoricalSampler.Count);

        for (var i = T.Zero; i < _numberOfTrials; i++)
        {
            var category = _categoricalSampler.Sample();
            counts[category]++;
        }

        return counts;
    }

    /// <summary>
    ///     Generates a sequence of random count vectors from the Multinomial distribution.
    /// </summary>
    /// <param name="count">The number of count vectors to sample.</param>
    /// <returns>An enumerable collection of <see cref="ChiVector{T}" /> instances.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiEnumerable<ChiVector<T>> Sample(int count)
    {
        var enumerable = ChiEnumerable<ChiVector<T>>.Rent(count);
        var list = enumerable.List;
        for (var i = 0; i < list.Count; i++)
            list[i] = Sample();
        return enumerable;
    }
}

/// <summary>
///     Provides extension methods for sampling from the Multinomial distribution.
/// </summary>
public static class ChiSamplerMultinomialExtensions
{
    /// <inheritdoc cref="SharedDocumentation.Multinomial{TRng, T}" />
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Roll a 4-sided die 20 times and get the counts for each face.
    /// var probabilities = new[] { 1.0, 1.0, 1.0, 1.0 };
    /// var results = rng
    ///     .Multinomial(20, probabilities)
    ///     .Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerMultinomial<TRng, T> Multinomial<TRng, T>(
        this ref TRng rng, T numberOfTrials, double[] probabilities)
        where TRng : struct, IChiRngSource<TRng>
        where T : unmanaged, IBinaryInteger<T>
    {
        return new ChiSamplerMultinomial<TRng, T>(ref rng, numberOfTrials, probabilities);
    }
}

/// <summary>
///     Provides non-allocating extension methods for sampling from the Multinomial distribution.
/// </summary>
public static class ChiSamplerMultinomialSpanExtensions
{
    /// <inheritdoc cref="SharedDocumentation.Multinomial{TRng, T}" />
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Roll a 4-sided die 20 times and get the counts for each face.
    /// var results = rng
    ///     .Multinomial(20, [1.0, 1.0, 1.0, 1.0])
    ///     .Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerMultinomial<TRng, T> Multinomial<TRng, T>(
        this ref TRng rng, T numberOfTrials, ReadOnlySpan<double> probabilities)
        where TRng : struct, IChiRngSource<TRng>
        where T : unmanaged, IBinaryInteger<T>
    {
        return new ChiSamplerMultinomial<TRng, T>(ref rng, numberOfTrials, probabilities);
    }
}

// ReSharper disable once UnusedType.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local
file static class SharedDocumentation
{
    /// <summary>
    ///     Returns a sampler for the Multinomial distribution, which models the number of outcomes in each category after a
    ///     fixed number of independent trials.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The integer type of the generated counts.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="numberOfTrials">The total number of trials (n), which must be a non-negative integer.</param>
    /// <param name="probabilities">
    ///     A vector of probabilities (p) for each category. All probabilities must be positive, but
    ///     they do not need to sum to 1.
    /// </param>
    /// <returns>A sampler that can be used to generate a vector of outcome counts.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Generalizes the Binomial distribution to multiple categories. Use it to model rolling a die
    ///         multiple times, sorting items into different bins, or simulating survey results across several demographic
    ///         groups.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(k + n), where k is the number of categories and n is the number of trials.
    ///     </para>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerMultinomial<TRng, T> Multinomial<TRng, T>(
        this ref TRng rng, T numberOfTrials, double[] probabilities)
        where TRng : struct, IChiRngSource<TRng>
        where T : unmanaged, IBinaryInteger<T>
    {
        throw new UnreachableException();
    }
}
// ReSharper restore UnusedParameter.Local
// ReSharper restore UnusedMember.Local