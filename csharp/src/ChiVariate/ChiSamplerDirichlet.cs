// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChiVariate;

/// <summary>
///     Samples from a Dirichlet distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerDirichletExtensions.Dirichlet{TRng,T}" /> method.
/// </remarks>
public readonly ref struct ChiSamplerDirichlet<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : unmanaged, IFloatingPoint<T>
{
    private readonly ref TRng _rng;
    private readonly ReadOnlySpan<T> _alpha;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerDirichlet(ref TRng rng, ReadOnlySpan<T> alpha)
    {
        if (alpha.IsEmpty)
            throw new ArgumentException("Alpha parameters cannot be empty.", nameof(alpha));

        foreach (var t in alpha)
            if (!T.IsFinite(t) || t <= T.Zero)
                throw new ArgumentException("All alpha parameters must be positive.", nameof(alpha));

        _rng = ref rng;
        _alpha = alpha;
    }

    /// <summary>
    ///     Generates a random probability vector from the configured Dirichlet distribution.
    /// </summary>
    /// <returns>A <see cref="ChiVector{T}" /> containing the sampled probability vector.</returns>
    public ChiVector<T> Sample()
    {
        var k = _alpha.Length;
        var result = ChiVector.Unsafe.Uninitialized<T>(k);
        var vector = result.Span;

        var sum = T.Zero;
        for (var i = 0; i < k; i++)
        {
            var gammaSample = _rng.Gamma(_alpha[i], T.One).Sample();
            vector[i] = gammaSample;
            sum += gammaSample;
        }

        if (sum > T.Zero)
        {
            for (var i = 0; i < k; i++)
                vector[i] /= sum;
        }
        else
        {
            var equalShare = T.One / T.CreateChecked(k);
            vector.Fill(equalShare);
        }

        return result;
    }

    /// <summary>
    ///     Generates multiple random probability vectors from the configured Dirichlet distribution.
    /// </summary>
    /// <param name="count">The number of vectors to sample.</param>
    /// <returns>An enumerable sequence of <see cref="ChiVector{T}" /> instances.</returns>
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
///     Provides extension methods for sampling from the Dirichlet distribution.
/// </summary>
public static class ChiSamplerDirichletExtensions
{
    /// <inheritdoc cref="SharedDocumentation.Dirichlet{TRng, T}" />
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// var alpha = new[] { 0.1, 1.0, 10.0 };
    /// var probabilities = rng.Dirichlet(alpha)
    ///     .Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerDirichlet<TRng, T> Dirichlet<TRng, T>(
        this ref TRng rng,
        T[] alpha)
        where TRng : struct, IChiRngSource<TRng>
        where T : unmanaged, IFloatingPoint<T>
    {
        return new ChiSamplerDirichlet<TRng, T>(ref rng, alpha);
    }
}

/// <summary>
///     Provides non-allocating extension methods for sampling from the Dirichlet distribution.
/// </summary>
public static class ChiSamplerDirichletSpanExtensions
{
    /// <inheritdoc cref="SharedDocumentation.Dirichlet{TRng, T}" />
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// var probabilities = rng.Dirichlet([0.1, 1.0, 10.0])
    ///     .Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerDirichlet<TRng, T> Dirichlet<TRng, T>(
        this ref TRng rng,
        ReadOnlySpan<T> alpha)
        where TRng : struct, IChiRngSource<TRng>
        where T : unmanaged, IFloatingPoint<T>
    {
        return new ChiSamplerDirichlet<TRng, T>(ref rng, alpha);
    }
}

// ReSharper disable once UnusedType.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local
file static class SharedDocumentation
{
    /// <summary>
    ///     Returns a sampler for the Dirichlet distribution, which generates a random vector of probabilities that sum to 1.
    ///     It is the multivariate generalization of the Beta distribution.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the vector components.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="alpha">The alpha vector (α), where each element must be a positive number.</param>
    /// <returns>A sampler that can be used to generate random probability vectors.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Generates a set of related proportions. Perfect for modeling topic mixtures in text analysis,
    ///         population genetics, or creating believable resource allocation patterns in strategy games.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(k) per sample, where k is the number of alpha parameters.
    ///     </para>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerDirichlet<TRng, T> Dirichlet<TRng, T>(
        this ref TRng rng,
        T[] alpha)
        where TRng : struct, IChiRngSource<TRng>
        where T : unmanaged, IFloatingPoint<T>
    {
        throw new UnreachableException();
    }
}
// ReSharper restore UnusedParameter.Local
// ReSharper restore UnusedMember.Local