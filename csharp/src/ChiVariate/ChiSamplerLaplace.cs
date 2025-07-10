// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChiVariate;

/// <summary>
///     Samples from a Laplace distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerLaplaceExtensions.Laplace{TRng,T}" /> method.
/// </remarks>
public readonly ref struct ChiSamplerLaplace<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private readonly ref TRng _rng;
    private readonly T _location;
    private readonly T _scale;

    private static readonly T PointFive = T.CreateChecked(0.5);
    private static readonly T Two = T.CreateChecked(2.0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerLaplace(ref TRng rng, T location, T scale)
    {
        if (!T.IsFinite(scale) || scale <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(scale), "Scale parameter (b) must be positive.");

        _rng = ref rng;
        _location = location;
        _scale = scale;
    }

    /// <summary>
    ///     Samples a single random value from the configured Laplace distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var uniform = _rng.Uniform(T.Zero, T.One).Sample();
        var u = uniform - PointFive;

        var sign = T.CreateChecked(T.Sign(u));
        var absU = T.Abs(u);
        var epsilon = ChiMath.Const<T>.Epsilon;
        var logArg = T.Max(epsilon, T.One - Two * absU);

        return _location - _scale * sign * ChiMath.Log(logArg);
    }

    /// <summary>
    ///     Generates a sequence of random values from the Laplace distribution.
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
///     Provides extension methods for sampling from the Laplace distribution.
/// </summary>
public static class ChiSamplerLaplaceExtensions
{
    /// <summary>
    ///     Returns a sampler for the Laplace distribution, a symmetric, "pointy" distribution that is more robust to outliers
    ///     than the Normal.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="location">The location parameter (μ), which is the mean and median of the distribution.</param>
    /// <param name="scale">The scale parameter (b), which controls the spread. Must be positive.</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Also known as the "double exponential" distribution. Useful in robust regression, signal
    ///         processing, and any model where errors are expected to be concentrated near the mean but with occasional large
    ///         deviations.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Sample from a standard Laplace distribution
    /// var value = rng.Laplace(0.0, 1.0).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerLaplace<TRng, T> Laplace<TRng, T>(this ref TRng rng, T location, T scale)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerLaplace<TRng, T>(ref rng, location, scale);
    }
}