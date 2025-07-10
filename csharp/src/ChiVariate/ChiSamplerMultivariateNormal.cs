// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate;

/// <summary>
///     Samples from a Multivariate Normal distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerMultivariateNormalExtensions.MultivariateNormal{TRng,T}" />
///     method.
/// </remarks>
public ref struct ChiSamplerMultivariateNormal<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : unmanaged, IFloatingPoint<T>
{
    private ChiMatrix<T> _meanVector;
    private ChiMatrix<T> _choleskyFactor;
    private ChiStatefulNormalProvider<TRng, T> _normalProvider;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerMultivariateNormal(ref TRng rng, ChiMatrix<T> meanVector, ChiMatrix<T> covarianceMatrix)
    {
        if (!meanVector.IsColumn)
            throw new ArgumentException("Mean must be a column vector (Nx1 matrix).", nameof(meanVector));
        if (!covarianceMatrix.IsSquare || covarianceMatrix.RowCount != meanVector.RowCount)
            throw new ArgumentException(
                "Covariance matrix must be square and have dimensions matching the mean vector.",
                nameof(covarianceMatrix));

        _choleskyFactor = covarianceMatrix.Peek().Cholesky();
        _meanVector = meanVector;

        _normalProvider = new ChiStatefulNormalProvider<TRng, T>(ref rng);
    }

    /// <summary>
    ///     Generates a new random vector from the configured Multivariate Normal distribution.
    /// </summary>
    /// <returns>An Nx1 <see cref="ChiMatrix{T}" /> (column vector) containing the sampled values.</returns>
    public ChiMatrix<T> Sample()
    {
        var k = _meanVector.RowCount;
        var zVector = ChiMatrix.Zeros<T>(k, 1);

        var zSpan = zVector.Span;
        for (var i = 0; i < k; i++)
            zSpan[i] = _normalProvider.Next();

        var correlatedSample = _choleskyFactor.Peek() * zVector.Consume();

        return _meanVector.Peek() + correlatedSample.Consume();
    }

    /// <summary>
    ///     Generates a sequence of random matrices from the Wishart distribution.
    /// </summary>
    /// <param name="count">The number of matrices to sample from the distribution.</param>
    /// <returns>An enumerable collection of matrices sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiEnumerable<ChiMatrix<T>> Sample(int count)
    {
        var enumerable = ChiEnumerable<ChiMatrix<T>>.Rent(count);
        var list = enumerable.List;
        for (var i = 0; i < list.Count; i++)
            list[i] = Sample();
        return enumerable;
    }
}

/// <summary>
///     Provides extension methods for sampling from the Multivariate Normal distribution.
/// </summary>
public static class ChiSamplerMultivariateNormalExtensions
{
    /// <summary>
    ///     Returns a sampler for the Multivariate Normal distribution, which generates vectors of correlated random variables.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the vector components.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="meanVector">The k-dimensional mean vector (μ), represented as a Kx1 <see cref="ChiMatrix{T}" />.</param>
    /// <param name="covarianceMatrix">
    ///     The k-by-k positive-semidefinite covariance matrix (Σ), as a <see cref="ChiMatrix{T}" />
    ///     .
    /// </param>
    /// <returns>A sampler that can be used to generate random vectors from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> The multivariate generalization of the Normal distribution. Essential for realistic physics
    ///         simulations with coupled variables, financial portfolio modeling, or any scenario where multiple random factors
    ///         influence each other systematically.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(k²) per sample, plus a one-time O(k³) setup cost for Cholesky decomposition.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// var mean = ChiMatrix.With([10.0, 20.0, 30.0]);
    /// var covariance = ChiMatrix.With(
    ///     [1.0, 0.0, 0.0],
    ///     [0.0, 4.0, 0.0],
    ///     [0.0, 0.0, 9.0]
    /// );
    ///  
    /// var sampler = rng.MultivariateNormal(mean, cov);
    /// var matrix = multivariateNormal.Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerMultivariateNormal<TRng, T> MultivariateNormal<TRng, T>(
        this ref TRng rng, ChiMatrix<T> meanVector, ChiMatrix<T> covarianceMatrix)
        where TRng : struct, IChiRngSource<TRng>
        where T : unmanaged, IFloatingPoint<T>
    {
        return new ChiSamplerMultivariateNormal<TRng, T>(ref rng, meanVector, covarianceMatrix);
    }
}