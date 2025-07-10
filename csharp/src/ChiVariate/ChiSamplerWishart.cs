// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate;

/// <summary>
///     Samples from a Wishart distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerWishartExtensions.Wishart{TRng, T}" /> method.
/// </remarks>
public ref struct ChiSamplerWishart<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : unmanaged, IFloatingPoint<T>
{
    private readonly ref TRng _rng;
    private readonly int _dimension;

    private ChiMatrix<T> _choleskyFactor;
    private ChiMatrix<T> _choleskyFactorTranspose;
    private ChiVector<(T shape, T scale)> _gammaParamsForChiSq;

    private ChiStatefulNormalProvider<TRng, T> _normalProvider;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerWishart(ref TRng rng, int degreesOfFreedom, ChiMatrix<T> scaleMatrix)
    {
        _dimension = scaleMatrix.RowCount;
        if (degreesOfFreedom < _dimension)
            throw new ArgumentException("Degrees of freedom must be greater than or equal to the matrix dimension.",
                nameof(degreesOfFreedom));

        if (!scaleMatrix.IsSquare)
            throw new ArgumentException("Scale matrix must be square.", nameof(scaleMatrix));

        _rng = ref rng;
        _normalProvider = new ChiStatefulNormalProvider<TRng, T>(ref rng);

        _choleskyFactor = scaleMatrix.Peek().Cholesky();
        _choleskyFactorTranspose = _choleskyFactor.Peek().Transpose();

        _gammaParamsForChiSq = ChiVector.Unsafe.Uninitialized<(T, T)>(_dimension);
        var span = _gammaParamsForChiSq.Span;
        for (var i = 0; i < _dimension; i++)
        {
            var shape = T.CreateChecked(degreesOfFreedom - i) * ChiMath.Const<T>.OneHalf;
            var scale = ChiMath.Const<T>.Two;
            span[i] = (shape, scale);
        }
    }

    /// <summary>
    ///     Samples a single random matrix from the configured Wishart distribution.
    /// </summary>
    /// <returns>A new <see cref="ChiMatrix{T}" /> sampled from the distribution.</returns>
    public ChiMatrix<T> Sample()
    {
        var a = ChiMatrix.Zeros<T>(_dimension, _dimension);
        var gammaParamsSpan = _gammaParamsForChiSq.Span;

        for (var i = 0; i < _dimension; i++)
        {
            var (shape, scale) = gammaParamsSpan[i];
            var chiSquaredSample = _rng.Gamma(shape, scale).Sample();
            a[i, i] = ChiMath.Sqrt(chiSquaredSample);

            for (var j = 0; j < i; j++)
                a[i, j] = _normalProvider.Next();
        }

        var aTranspose = a.Peek().Transpose();
        var aProduct = a.Consume() * aTranspose.Consume();

        var intermediateProduct = _choleskyFactor.Peek() * aProduct.Peek();
        var finalMatrix = intermediateProduct.Consume() * _choleskyFactorTranspose.Peek();

        return finalMatrix;
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
///     Provides extension methods for sampling from the Wishart distribution.
/// </summary>
public static class ChiSamplerWishartExtensions
{
    /// <inheritdoc cref="SharedDocumentation.Wishart{TRng,T}" />
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// var scaleMatrix = ChiMatrix.With([1.0, 0.5], [0.5, 2.0]);
    /// var randomCovariance = rng.Wishart(10, scaleMatrix).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerWishart<TRng, T> Wishart<TRng, T>(
        this ref TRng rng, int degreesOfFreedom, ChiMatrix<T> scaleMatrix)
        where TRng : struct, IChiRngSource<TRng>
        where T : unmanaged, IFloatingPoint<T>
    {
        return new ChiSamplerWishart<TRng, T>(ref rng, degreesOfFreedom, scaleMatrix);
    }
}

/// <summary>
///     Provides allocating extension methods for sampling from the Wishart distribution.
/// </summary>
public static class ChiSamplerWishartArrayExtensions
{
    /// <inheritdoc cref="SharedDocumentation.Wishart{TRng,T}" />
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// var scaleMatrix = new[,] { { 1.0, 0.5 }, { 0.5, 2.0 } };
    /// var randomCovariance = rng.Wishart(10, scaleMatrix).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerWishart<TRng, T> Wishart<TRng, T>(
        this ref TRng rng, int degreesOfFreedom, T[,] scaleMatrix)
        where TRng : struct, IChiRngSource<TRng>
        where T : unmanaged, IFloatingPoint<T>
    {
        return new ChiSamplerWishart<TRng, T>(ref rng, degreesOfFreedom, ChiMatrix.With(scaleMatrix));
    }
}

// ReSharper disable UnusedType.Local
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedVariable
// ReSharper disable MemberCanBePrivate.Local
file static class SharedDocumentation
{
    /// <summary>
    ///     Returns a sampler for the Wishart distribution, which generates random, symmetric, positive-semidefinite matrices.
    ///     It is the multivariate generalization of the Chi-Squared distribution.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the matrix elements.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="degreesOfFreedom">
    ///     The degrees of freedom (ν). Must be an integer greater than or equal to the dimension of the scale matrix.
    /// </param>
    /// <param name="scaleMatrix">The k-by-k positive-definite scale matrix (Σ).</param>
    /// <returns>A sampler that can be used to generate random matrices from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> A cornerstone of multivariate Bayesian analysis. It is used to generate random covariance
    ///         matrices, allowing you to model uncertainty in the relationships between multiple variables, such as in
    ///         financial risk models or sensor fusion.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(k³) per sample, plus a one-time O(k³) setup cost for Cholesky decomposition.
    ///     </para>
    /// </remarks>
    public static ChiSamplerWishart<TRng, T> Wishart<TRng, T>(
        this ref TRng rng,
        int degreesOfFreedom,
        DummyType scaleMatrix)
        where TRng : struct, IChiRngSource<TRng>
        where T : unmanaged, IFloatingPoint<T>
    {
        throw new UnreachableException();
    }

    private static void Examples()
    {
        {
            var rng = new ChiRng();
            var scaleMatrix = ChiMatrix.With([1.0, 0.5], [0.5, 2.0]);
            var randomCovariance = rng.Wishart(10, scaleMatrix).Sample();
        }

        {
            var rng = new ChiRng();
            var scaleMatrix = new[,] { { 1.0, 0.5 }, { 0.5, 2.0 } };
            var randomCovariance = rng.Wishart(10, scaleMatrix).Sample();
        }
    }

    public class DummyType;
}
// ReSharper restore MemberCanBePrivate.Local
// ReSharper restore UnusedVariable
// ReSharper restore UnusedParameter.Local
// ReSharper restore UnusedMember.Local
// ReSharper restore ClassNeverInstantiated.Local
// ReSharper restore UnusedType.Local