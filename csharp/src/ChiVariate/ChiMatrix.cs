// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ChiVariate.Internal;

namespace ChiVariate;

/// <summary>
///     Provides a collection of static factory methods for creating and manipulating matrices
///     based on the <see cref="ChiMatrix{T}" /> type.
/// </summary>
public static class ChiMatrix
{
    /// <summary>
    ///     Creates a new matrix of the specified size, with all elements initialized to the specified value.
    /// </summary>
    /// <param name="rows">The number of rows.</param>
    /// <param name="columns">The number of columns.</param>
    /// <param name="value">The value to assign to every element of the matrix.</param>
    /// <returns>A new <see cref="ChiMatrix{T}" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiMatrix<T> Full<T>(int rows, int columns, T value)
        where T : unmanaged, IFloatingPoint<T>
    {
        var matrix = Unsafe.Uninitialized<T>(rows, columns);
        matrix.Span.Fill(value);
        return matrix;
    }

    /// <summary>
    ///     Creates a new matrix and initializes each element using a factory function based on its linear index.
    /// </summary>
    /// <param name="rows">The number of rows in the new matrix.</param>
    /// <param name="columns">The number of columns in the new matrix.</param>
    /// <param name="value">A factory function that takes the 0-based linear index of an element and returns its value.</param>
    /// <typeparam name="T">The floating-point type of the matrix elements.</typeparam>
    /// <returns>A new, fully initialized <see cref="ChiMatrix{T}" />.</returns>
    /// <remarks>
    ///     The index provided to the factory function is the flattened index of the element in row-major order.
    ///     For example, in a 2x2 matrix, element (1, 0) corresponds to linear index 2.
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// // Create a 3x1 column vector with values [1, 2, 3]
    /// var sequence = ChiMatrix.Full<double>(3, 1, i => i + 1.0);
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiMatrix<T> Full<T>(int rows, int columns, Func<int, T> value)
        where T : unmanaged, IFloatingPoint<T>
    {
        var matrix = Unsafe.Uninitialized<T>(rows, columns);
        var flatView = matrix.Span;

        for (var i = 0; i < matrix.Length; i++)
            flatView[i] = value(i);

        return matrix;
    }

    /// <summary>
    ///     Creates a new matrix and initializes each element using a non-capturing factory function
    ///     based on its linear index and a state argument.
    /// </summary>
    /// <param name="rows">The number of rows in the new matrix.</param>
    /// <param name="columns">The number of columns in the new matrix.</param>
    /// <param name="arg">A state argument to be passed to the factory function on each invocation.</param>
    /// <param name="value">
    ///     A factory function that takes the state argument and the 0-based linear index of an element, and
    ///     returns its value.
    /// </param>
    /// <typeparam name="T">The floating-point type of the matrix elements.</typeparam>
    /// <typeparam name="TArg">The type of the state argument.</typeparam>
    /// <returns>A new, fully initialized <see cref="ChiMatrix{T}" />.</returns>
    /// <remarks>
    ///     This overload is designed for high-performance scenarios. By passing state through the <paramref name="arg" />
    ///     parameter, the <paramref name="value" /> function can be declared as a <c>static</c> lambda,
    ///     preventing heap allocations associated with closures.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiMatrix<T> Full<T, TArg>(int rows, int columns, TArg arg, Func<TArg, int, T> value)
        where T : unmanaged, IFloatingPoint<T>
    {
        var matrix = Unsafe.Uninitialized<T>(rows, columns);
        var flatView = matrix.Span;

        for (var i = 0; i < matrix.Length; i++)
            flatView[i] = value(arg, i);

        return matrix;
    }

    /// <summary>
    ///     Creates a new matrix and initializes each element using a factory function based on its row and column index.
    /// </summary>
    /// <param name="rows">The number of rows in the new matrix.</param>
    /// <param name="columns">The number of columns in the new matrix.</param>
    /// <param name="value">A factory function that takes the 0-based row and column index of an element and returns its value.</param>
    /// <typeparam name="T">The floating-point type of the matrix elements.</typeparam>
    /// <returns>A new, fully initialized <see cref="ChiMatrix{T}" />.</returns>
    /// <example>
    ///     <code><![CDATA[
    /// // Create a 2x3 matrix where each element is the sum of its indices
    /// var m = ChiMatrix.Full<double>(2, 3, (row, col) => row + col);
    /// // Result:
    /// // [ 0, 1, 2 ]
    /// // [ 1, 2, 3 ]
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiMatrix<T> Full<T>(int rows, int columns, Func<int, int, T> value)
        where T : unmanaged, IFloatingPoint<T>
    {
        var matrix = Unsafe.Uninitialized<T>(rows, columns);
        var flatView = matrix.Span;

        var i = 0;
        for (var row = 0; row < rows; row++)
        for (var col = 0; col < columns; col++)
            flatView[i++] = value(row, col);

        return matrix;
    }

    /// <summary>
    ///     Creates a new matrix and initializes each element using a non-capturing factory function
    ///     based on its row and column index and a state argument.
    /// </summary>
    /// <param name="rows">The number of rows in the new matrix.</param>
    /// <param name="columns">The number of columns in the new matrix.</param>
    /// <param name="arg">A state argument to be passed to the factory function on each invocation.</param>
    /// <param name="value">
    ///     A factory function that takes the state argument and the 0-based row and column index of an
    ///     element, and returns its value.
    /// </param>
    /// <typeparam name="T">The floating-point type of the matrix elements.</typeparam>
    /// <typeparam name="TArg">The type of the state argument.</typeparam>
    /// <returns>A new, fully initialized <see cref="ChiMatrix{T}" />.</returns>
    /// <remarks>
    ///     This overload is designed for high-performance scenarios. By passing state through the <paramref name="arg" />
    ///     parameter, the <paramref name="value" /> function can be declared as a <c>static</c> lambda,
    ///     preventing heap allocations associated with closures.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiMatrix<T> Full<T, TArg>(int rows, int columns, TArg arg, Func<TArg, int, int, T> value)
        where T : unmanaged, IFloatingPoint<T>
    {
        var matrix = Unsafe.Uninitialized<T>(rows, columns);
        var flatView = matrix.Span;

        var i = 0;
        for (var row = 0; row < rows; row++)
        for (var col = 0; col < columns; col++)
            flatView[i++] = value(arg, row, col);

        return matrix;
    }

    /// <summary>
    ///     Creates a new Hilbert matrix of the specified size.
    /// </summary>
    /// <param name="size">The number of rows and columns for the square matrix.</param>
    /// <typeparam name="T">The floating-point type of the matrix elements.</typeparam>
    /// <returns>A new <see cref="ChiMatrix{T}" /> representing the Hilbert matrix.</returns>
    /// <remarks>
    ///     A Hilbert matrix is a classic example of an ill-conditioned matrix, often used for testing
    ///     the numerical stability of linear algebra algorithms. Each element is defined as H(i,j) = 1 / (i + j - 1).
    ///     (Using 0-based indexing, this becomes 1 / (row + col + 1)).
    /// </remarks>
    public static ChiMatrix<T> Hilbert<T>(int size)
        where T : unmanaged, IFloatingPoint<T>
    {
        return Full(size, size, (row, col) =>
            T.One / T.CreateChecked(row + col + 1)
        );
    }

    /// <summary>
    ///     Creates a new symmetric Toeplitz matrix from its first row.
    /// </summary>
    /// <param name="firstRow">A span containing the elements of the first row of the matrix.</param>
    /// <typeparam name="T">The floating-point type of the matrix elements.</typeparam>
    /// <returns>A new square <see cref="ChiMatrix{T}" /> representing the symmetric Toeplitz matrix.</returns>
    /// <remarks>
    ///     A symmetric Toeplitz matrix has constant values along each of its diagonals.
    ///     It is commonly used in signal processing and time-series analysis.
    /// </remarks>
    public static ChiMatrix<T> Toeplitz<T>(scoped ReadOnlySpan<T> firstRow)
        where T : unmanaged, IFloatingPoint<T>
    {
        var length = firstRow.Length;
        using var bufferVector = ChiVector.With(firstRow);

        return Full(length, length, bufferVector,
            static (vector, row, col) =>
                vector[int.Abs(row - col)]);
    }

    /// <summary>
    ///     Creates a new Vandermonde matrix from a vector of elements.
    /// </summary>
    /// <param name="vector">A span containing the elements (x_0, x_1, ...) to generate the matrix from.</param>
    /// <param name="numColumns">
    ///     The number of columns in the matrix, corresponding to the polynomial degree plus one. If null,
    ///     a square matrix is created.
    /// </param>
    /// <typeparam name="T">The floating-point type of the matrix elements.</typeparam>
    /// <returns>A new <see cref="ChiMatrix{T}" /> representing the Vandermonde matrix.</returns>
    /// <remarks>
    ///     A Vandermonde matrix is used in polynomial interpolation. Each element is defined as V(i,j) = x_i^j.
    /// </remarks>
    public static ChiMatrix<T> Vandermonde<T>(scoped ReadOnlySpan<T> vector, int? numColumns = null)
        where T : unmanaged, IFloatingPoint<T>
    {
        var numRows = vector.Length;
        using var bufferVector = ChiVector.With(vector);

        var length = numColumns ?? numRows;
        return Full(numRows, length, bufferVector,
            static (bufferVector, row, column) =>
                ChiMath.Pow(bufferVector[row], T.CreateChecked(column))
        );
    }

    /// <summary>
    ///     Creates a new matrix of the specified size, with all elements set to zero.
    /// </summary>
    /// <param name="rows">The number of rows.</param>
    /// <param name="columns">The number of columns.</param>
    /// <returns>A new <see cref="ChiMatrix{T}" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiMatrix<T> Zeros<T>(int rows, int columns)
        where T : unmanaged, IFloatingPoint<T>
    {
        return Full(rows, columns, T.Zero);
    }

    /// <summary>
    ///     Creates a new matrix of the specified size, with all elements set to one.
    /// </summary>
    /// <param name="rows">The number of rows.</param>
    /// <param name="columns">The number of columns.</param>
    /// <returns>A new <see cref="ChiMatrix{T}" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiMatrix<T> Ones<T>(int rows, int columns)
        where T : unmanaged, IFloatingPoint<T>
    {
        return Full(rows, columns, T.One);
    }

    /// <summary>
    ///     Creates a new 1×1 matrix containing the specified scalar value.
    /// </summary>
    /// <param name="value">The scalar value.</param>
    /// <returns>A new <see cref="ChiMatrix{T}" /> representing a scalar.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiMatrix<T> Scalar<T>(T value)
        where T : unmanaged, IFloatingPoint<T>
    {
        return Full(1, 1, value);
    }

    /// <summary>
    ///     Creates a new square identity matrix of the specified size.
    /// </summary>
    /// <param name="size">The dimension of the identity matrix.</param>
    /// <returns>A new <see cref="ChiMatrix{T}" /> with ones on the diagonal and zeros elsewhere.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiMatrix<T> Eye<T>(int size)
        where T : unmanaged, IFloatingPoint<T>
    {
        var matrix = Zeros<T>(size, size);

        for (var i = 0; i < size; i++)
            matrix[i, i] = T.One;

        return matrix;
    }

    /// <summary>
    ///     Creates a new square diagonal matrix from the given vector of diagonal elements.
    /// </summary>
    /// <param name="vector">A span containing the elements for the main diagonal.</param>
    /// <returns>A new <see cref="ChiMatrix{T}" /> with the specified diagonal values.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiMatrix<T> Diagonal<T>(scoped ReadOnlySpan<T> vector)
        where T : unmanaged, IFloatingPoint<T>
    {
        var size = vector.Length;
        var matrix = Zeros<T>(size, size);

        for (var i = 0; i < size; i++)
            matrix[i, i] = vector[i];

        return matrix;
    }

    /// <summary>
    ///     Creates a new <see cref="ChiMatrix{T}" /> by copying the elements from a standard 2D array.
    /// </summary>
    /// <param name="sourceArray">The two-dimensional source array to copy from.</param>
    /// <returns>A new <see cref="ChiMatrix{T}" /> containing a copy of the source data.</returns>
    /// <remarks>
    ///     This method provides an efficient bridge for interoperating with code that uses standard T[,] arrays.
    ///     It performs a single, optimized block copy of the data into the new matrix's backing memory.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiMatrix<T> With<T>(T[,] sourceArray)
        where T : unmanaged, IFloatingPoint<T>
    {
        var length = sourceArray.Length;
        var sourceSpan = MemoryMarshal.CreateSpan(ref sourceArray[0, 0], length);

        var rows = sourceArray.GetLength(0);
        var columns = sourceArray.GetLength(1);
        var result = Unsafe.Uninitialized<T>(rows, columns);

        sourceSpan.CopyTo(result.Span);

        return result;
    }

    /// <summary>
    ///     Creates a new Nx1 matrix (column vector) from a 1D span of elements.
    /// </summary>
    /// <param name="sourceSpan">The one-dimensional source span to copy from.</param>
    /// <returns>A new Nx1 <see cref="ChiMatrix{T}" /> representing a column vector.</returns>
    /// <example>
    ///     <code><![CDATA[
    /// var mean = ChiMatrix.With([10.0, 20.0, 30.0]);
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiMatrix<T> With<T>(scoped ReadOnlySpan<T> sourceSpan)
        where T : unmanaged, IFloatingPoint<T>
    {
        var result = Unsafe.Uninitialized<T>(sourceSpan.Length, 1);
        sourceSpan.CopyTo(result.Span);
        return result;
    }

    /// <summary>
    ///     Creates a new 1xN matrix (row vector) from a 1D span of elements.
    /// </summary>
    /// <param name="sourceSpan">The one-dimensional source span to copy from.</param>
    /// <returns>A new 1xN <see cref="ChiMatrix{T}" /> representing a row vector.</returns>
    /// <example>
    ///     <code><![CDATA[
    /// var rowVector = ChiMatrix.WithTransposed([10.0, 20.0, 30.0]);
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiMatrix<T> WithTransposed<T>(scoped ReadOnlySpan<T> sourceSpan)
        where T : unmanaged, IFloatingPoint<T>
    {
        var result = Unsafe.Uninitialized<T>(1, sourceSpan.Length);
        sourceSpan.CopyTo(result.Span);
        return result;
    }

    /// <summary>
    ///     Creates a new 2xN matrix from two row spans of equal length.
    /// </summary>
    /// <param name="row0">The first row of the matrix.</param>
    /// <param name="row1">The second row of the matrix.</param>
    /// <returns>A new 2xN <see cref="ChiMatrix{T}" /> representing the matrix.</returns>
    /// <example>
    ///     <code><![CDATA[
    /// var m = ChiMatrix.With<double>(
    ///     [1.0, 2.0, 3.0],
    ///     [4.0, 5.0, 6.0]);
    /// ]]></code>
    /// </example>
    /// <exception cref="ArgumentException">Thrown if any row is empty or if row lengths differ.</exception>
    public static ChiMatrix<T> With<T>(scoped ReadOnlySpan<T> row0, ReadOnlySpan<T> row1)
        where T : unmanaged, IFloatingPoint<T>
    {
        var cols = row0.Length;
        if (cols == 0) throw new ArgumentException("Matrix rows cannot be empty.", nameof(row0));
        if (row1.Length != cols) throw new ArgumentException("All rows must have the same length.");

        var matrix = Unsafe.Uninitialized<T>(2, cols);
        var span = matrix.Span;

        row0.CopyTo(span.Slice(0 * cols, cols));
        row1.CopyTo(span.Slice(1 * cols, cols));

        return matrix;
    }

    /// <summary>
    ///     Creates a new 3xN matrix from three row spans of equal length.
    /// </summary>
    /// <param name="row0">The first row of the matrix.</param>
    /// <param name="row1">The second row of the matrix.</param>
    /// <param name="row2">The third row of the matrix.</param>
    /// <returns>A new 3xN <see cref="ChiMatrix{T}" /> representing the matrix.</returns>
    /// <example>
    ///     <code><![CDATA[
    /// var m = ChiMatrix.With<double>(
    ///     [1.0, 2.0, 3.0],
    ///     [4.0, 5.0, 6.0],
    ///     [7.0, 8.0, 9.0]);
    /// ]]></code>
    /// </example>
    /// <exception cref="ArgumentException">Thrown if any row is empty or if row lengths differ.</exception>
    public static ChiMatrix<T> With<T>(scoped ReadOnlySpan<T> row0, ReadOnlySpan<T> row1, ReadOnlySpan<T> row2)
        where T : unmanaged, IFloatingPoint<T>
    {
        var cols = row0.Length;
        if (cols == 0) throw new ArgumentException("Matrix rows cannot be empty.", nameof(row0));
        if (row1.Length != cols || row2.Length != cols)
            throw new ArgumentException("All rows must have the same length.");

        var matrix = Unsafe.Uninitialized<T>(3, cols);
        var span = matrix.Span;

        row0.CopyTo(span.Slice(0 * cols, cols));
        row1.CopyTo(span.Slice(1 * cols, cols));
        row2.CopyTo(span.Slice(2 * cols, cols));

        return matrix;
    }

    /// <summary>
    ///     Creates a new 4xN matrix from four row spans of equal length.
    /// </summary>
    /// <param name="row0">The first row of the matrix.</param>
    /// <param name="row1">The second row of the matrix.</param>
    /// <param name="row2">The third row of the matrix.</param>
    /// <param name="row3">The fourth row of the matrix.</param>
    /// <returns>A new 4xN <see cref="ChiMatrix{T}" /> representing the matrix.</returns>
    /// <example>
    ///     <code><![CDATA[
    /// var m = ChiMatrix.With<double>(
    ///     [1.0, 2.0, 3.0],
    ///     [4.0, 5.0, 6.0],
    ///     [7.0, 8.0, 9.0],
    ///     [10.0, 11.0, 12.0]);
    /// ]]></code>
    /// </example>
    /// <exception cref="ArgumentException">Thrown if any row is empty or if row lengths differ.</exception>
    public static ChiMatrix<T> With<T>(scoped ReadOnlySpan<T> row0, ReadOnlySpan<T> row1, ReadOnlySpan<T> row2,
        ReadOnlySpan<T> row3)
        where T : unmanaged, IFloatingPoint<T>
    {
        var cols = row0.Length;
        if (cols == 0) throw new ArgumentException("Matrix rows cannot be empty.", nameof(row0));
        if (row1.Length != cols || row2.Length != cols || row3.Length != cols)
            throw new ArgumentException("All rows must have the same length.");

        var matrix = Unsafe.Uninitialized<T>(4, cols);
        var span = matrix.Span;

        row0.CopyTo(span.Slice(0 * cols, cols));
        row1.CopyTo(span.Slice(1 * cols, cols));
        row2.CopyTo(span.Slice(2 * cols, cols));
        row3.CopyTo(span.Slice(3 * cols, cols));

        return matrix;
    }

    /// <summary>
    ///     Creates a new 5xN matrix from five row spans of equal length.
    /// </summary>
    /// <param name="row0">The first row of the matrix.</param>
    /// <param name="row1">The second row of the matrix.</param>
    /// <param name="row2">The third row of the matrix.</param>
    /// <param name="row3">The fourth row of the matrix.</param>
    /// <param name="row4">The fifth row of the matrix.</param>
    /// <returns>A new 5xN <see cref="ChiMatrix{T}" /> representing the matrix.</returns>
    /// <example>
    ///     <code><![CDATA[
    /// var m = ChiMatrix.With<double>(
    ///     [1.0, 2.0, 3.0],
    ///     [4.0, 5.0, 6.0],
    ///     [7.0, 8.0, 9.0],
    ///     [10.0, 11.0, 12.0],
    ///     [13.0, 14.0, 15.0]);
    /// ]]></code>
    /// </example>
    /// <exception cref="ArgumentException">Thrown if any row is empty or if row lengths differ.</exception>
    public static ChiMatrix<T> With<T>(scoped ReadOnlySpan<T> row0, ReadOnlySpan<T> row1, ReadOnlySpan<T> row2,
        ReadOnlySpan<T> row3, ReadOnlySpan<T> row4)
        where T : unmanaged, IFloatingPoint<T>
    {
        var cols = row0.Length;
        if (cols == 0) throw new ArgumentException("Matrix rows cannot be empty.", nameof(row0));
        if (row1.Length != cols || row2.Length != cols || row3.Length != cols || row4.Length != cols)
            throw new ArgumentException("All rows must have the same length.");

        var matrix = Unsafe.Uninitialized<T>(5, cols);
        var span = matrix.Span;

        row0.CopyTo(span.Slice(0 * cols, cols));
        row1.CopyTo(span.Slice(1 * cols, cols));
        row2.CopyTo(span.Slice(2 * cols, cols));
        row3.CopyTo(span.Slice(3 * cols, cols));
        row4.CopyTo(span.Slice(4 * cols, cols));

        return matrix;
    }

    /// <summary>
    ///     Provides access to advanced, performance-oriented factory methods that require the caller
    ///     to adhere to specific safety contracts to avoid undefined behavior.
    /// </summary>
    public static class Unsafe
    {
        /// <summary>
        ///     [Advanced] Creates a matrix without initializing its contents, returning a buffer that
        ///     may contain arbitrary garbage data from previous memory operations.
        /// </summary>
        /// <param name="rows">The number of rows in the matrix.</param>
        /// <param name="columns">The number of columns in the matrix.</param>
        /// <typeparam name="T">The floating-point type of the matrix elements.</typeparam>
        /// <returns>
        ///     A new <see cref="ChiMatrix{T}" /> with uninitialized backing memory.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <b>Warning:</b> This method is a high-performance feature for advanced scenarios.
        ///         The caller assumes full responsibility for initializing every element of the returned matrix
        ///         before any read operations are performed. Failure to do so will result in reading
        ///         uninitialized memory, leading to unpredictable and erroneous behavior.
        ///     </para>
        ///     <para>
        ///         Use this method only when you can guarantee that the entire content of the matrix will be
        ///         unconditionally overwritten immediately after creation. For all other cases, prefer safer factory
        ///         methods like <see cref="ChiMatrix.Zeros{T}" />.
        ///     </para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ChiMatrix<T> Uninitialized<T>(int rows, int columns) where T : unmanaged, IFloatingPoint<T>
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(rows, 0, nameof(rows));
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(columns, 0, nameof(columns));

            var length = rows * columns;
            var matrix = default(ChiMatrix<T>) with
            {
                RowCount = rows,
                ColumnCount = columns,
                Length = rows * columns,
                IsValid = true,
                HeapData = length > ChiMatrix<T>.MaxInlineLength
                    ? ChiArrayPool<T>.Rent(length, false)
                    : null
            };
            return matrix;
        }
    }
}

/// <summary>
///     Represents a high-performance, two-dimensional matrix with a hybrid memory model.
///     Matrices up to 5×5 are stored inline on the stack; larger matrices use pooled heap memory.
/// </summary>
/// <typeparam name="T">The floating-point type of the matrix elements.</typeparam>
/// <remarks>
///     <para>
///         <b>Conceptual Immutability and Safe Composition</b>
///     </para>
///     <para>
///         While matrix elements can be modified through the indexer, <see cref="ChiMatrix{T}" /> is designed
///         to behave as a value type in mathematical expressions.
///         All algebraic operations (such as +, *, -) are conceptually immutable: they return new
///         <see cref="ChiMatrix{T}" /> instances rather than mutating inputs.
///     </para>
///     <para>
///         To use these operators safely, an operational view should first be obtained via
///         <see cref="ChiMatrixExtensions.Peek{T}" />.
///         This returns a temporary, stack-only <see cref="ChiMatrixOp{T}" /> that defines all arithmetic operators:
///         <code>using var c = a.Value() * b.Value();</code>
///     </para>
///     <para>
///         This pattern enforces compile-time safety: it prevents accidental operations on temporary
///         values (such as property getters), which might otherwise result in memory leaks or undefined behavior.
///     </para>
/// </remarks>
[DebuggerDisplay("{DebuggerDisplay()}")]
public struct ChiMatrix<T> : IDisposable, IEquatable<ChiMatrix<T>>
    where T : unmanaged, IFloatingPoint<T>
{
    #region Disposal

    /// <summary>
    ///     Releases any heap-allocated memory rented from the memory pool.
    ///     If the matrix is stored inline (on the stack), this method is a no-op.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Matrices up to 5×5 are stack-allocated and do not require explicit disposal or memory management.
    ///     </para>
    /// </remarks>
    public void Dispose()
    {
        if (!IsValid)
            return;

        IsValid = false;

        ChiArrayPool<T>.Return(HeapData);
    }

    #endregion

    #region Instance Methods

    /// <summary>
    ///     Creates a new, two-dimensional heap-allocated array and copies the matrix's elements into it.
    /// </summary>
    /// <returns>A new T[,] array containing a copy of the matrix data.</returns>
    /// <remarks>
    ///     This method performs a full copy of the matrix data and allocates a new array on the managed heap.
    ///     It is useful for interoperating with other libraries or APIs that expect a standard 2D array.
    ///     For performance-sensitive code, prefer operating directly on the matrix's <see cref="Span" />.
    ///     The returned array is completely independent of the source matrix; disposing the source matrix
    ///     will not affect the returned array.
    /// </remarks>
    public T[,] ToArray()
    {
        var array = new T[RowCount, ColumnCount];
        if (Length <= 0) return array;

        var destinationSpan = MemoryMarshal.CreateSpan(ref array[0, 0], Length);
        Span.CopyTo(destinationSpan);

        return array;
    }

    /// <summary>
    ///     Creates a new, one-dimensional heap-allocated array by copying the elements from a vector or scalar matrix.
    /// </summary>
    /// <returns>A new T[] array containing a copy of the matrix data.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the matrix is not a row vector, column vector, or a 1x1 scalar.
    /// </exception>
    /// <remarks>
    ///     This method provides a convenient way to convert a <see cref="ChiMatrix{T}" /> that represents
    ///     a vector back into a standard 1D array for interoperability.
    /// </remarks>
    public T[] VectorToArray()
    {
        if (!IsVector)
            throw new InvalidOperationException(
                $"Cannot convert a [{RowCount}x{ColumnCount}] matrix to a 1D vector array. The matrix must be a row vector, column vector, or scalar.");

        var array = new T[Length];
        Span.CopyTo(array);
        return array;
    }

    #endregion

    #region Private Fields & Constants

    internal const int MaxInlineLength = 5 * 5;
    internal bool IsValid { get; set; }
    internal T[]? HeapData { get; init; }
    private InlineData _inlineData;

    #endregion

    #region Instance Properties

    /// <summary>
    ///     Gets the number of rows in the matrix.
    /// </summary>
    public int RowCount { get; internal init; }

    /// <summary>
    ///     Gets the number of columns in the matrix.
    /// </summary>
    public int ColumnCount { get; internal init; }

    /// <summary>
    ///     Gets the total number of elements in the matrix (Rows * Cols).
    /// </summary>
    public int Length { get; internal init; }

    /// <summary>
    ///     Gets a value indicating whether the matrix is square (i.e., Rows == Cols).
    /// </summary>
    public bool IsSquare => RowCount == ColumnCount;

    /// <summary>
    ///     Gets a value indicating whether the matrix is a row vector (i.e., has exactly one row).
    /// </summary>
    public bool IsRow => RowCount == 1;

    /// <summary>
    ///     Gets a value indicating whether the matrix is a column vector (i.e., has exactly one column).
    /// </summary>
    public bool IsColumn => ColumnCount == 1;

    /// <summary>
    ///     Gets a value indicating whether the matrix is a column vector or a row vector.
    /// </summary>
    public bool IsVector => ColumnCount == 1 || RowCount == 1;

    /// <summary>
    ///     Gets a value indicating whether the matrix is a scalar (i.e., is 1x1).
    /// </summary>
    public bool IsScalar => RowCount == 1 && ColumnCount == 1;

    /// <summary>
    ///     Gets a <see cref="Span{T}" /> view over the matrix's underlying contiguous memory.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the matrix has been disposed and its memory was heap-allocated.</exception>
    public Span<T> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (HeapData is not null)
            {
                if (!IsValid)
                    throw new ObjectDisposedException(nameof(ChiMatrix<T>));
                return HeapData.AsSpan()[..Length];
            }

            ref var dataPointer = ref Unsafe.As<InlineData, T>(ref _inlineData);
            return MemoryMarshal.CreateSpan(ref dataPointer, Length);
        }
    }

    /// <summary>
    ///     Gets a reference to the element at the specified row and column for 2D matrix access.
    /// </summary>
    /// <param name="row">The zero-based row index.</param>
    /// <param name="column">The zero-based column index.</param>
    /// <returns>A reference to the element at the specified position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the row or column is out of bounds.</exception>
    public ref T this[int row, int column]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var index = row * ColumnCount + column;
            if ((uint)row >= (uint)RowCount || (uint)column >= (uint)ColumnCount)
                throw new ArgumentOutOfRangeException(
                    $"Index [{row},{column}] is out of bounds for matrix of size [{RowCount}x{ColumnCount}].");

            return ref Span[index];
        }
    }

    /// <summary>
    ///     Gets a reference to the element at the specified index for vector access.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>A reference to the element at the specified position.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the matrix is not a vector (row, column, or scalar).</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is out of bounds.</exception>
    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (!IsVector)
                throw new InvalidOperationException(
                    $"The single-dimensional indexer can only be used on a row vector, column vector, or scalar, but the matrix is [{RowCount}x{ColumnCount}].");

            if ((uint)index >= (uint)Length)
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    $"Index {index} is out of bounds for a vector of length {Length}.");

            return ref Span[index];
        }
    }

    #endregion

    #region Internal & Boilerplate

    [InlineArray(MaxInlineLength)]
    private struct InlineData
    {
        private T _element0;
    }

    /// <summary>
    ///     Intentionally prevents usage of the parameterless constructor.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    [Obsolete(UseStaticFactoryMethodsMessage, true)]
    public ChiMatrix()
    {
        throw new InvalidOperationException(UseStaticFactoryMethodsMessage);
    }

    private const string UseStaticFactoryMethodsMessage =
        $"Please use the static factory methods on '{nameof(ChiMatrix)}' to create '{nameof(ChiMatrix)}<T>' instances.";

    private string DebuggerDisplay()
    {
        return $"ChiMatrix<{typeof(T).Name}> [{RowCount} x {ColumnCount}] ({Length} total)";
    }

    /// <summary>
    ///     A snapshot of a matrix's internal state for debugging purposes.
    /// </summary>
    public readonly ref struct DebugInfo
    {
        /// <summary>
        ///     A snapshot of a matrix's internal state for debugging purposes.
        /// </summary>
        /// <param name="matrix">The matrix being inspected.</param>
        internal DebugInfo(ref ChiMatrix<T> matrix)
        {
            _matrix = ref matrix;
        }

        private readonly ref ChiMatrix<T> _matrix;

        /// <summary>
        ///     Gets a value indicating whether the matrix data is stored on the heap (rented from a pool).
        /// </summary>
        public bool IsHeapBacked => _matrix.HeapData is not null;

        /// <summary>
        ///     Gets a value indicating whether the matrix is still valid (i.e., has not been disposed).
        /// </summary>
        public bool IsValid => _matrix.IsValid;
    }

    /// <summary>
    ///     Returns a hash code for the current matrix based on its dimensions and element values.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(RowCount);
        hash.Add(ColumnCount);
        hash.AddBytes(MemoryMarshal.AsBytes(Span));
        return hash.ToHashCode();
    }

    /// <summary>
    ///     Determines whether the specified <see cref="ChiMatrix{T}" /> is equal to the current vector.
    /// </summary>
    /// <param name="other">The vector to compare with the current vector.</param>
    public override bool Equals(object? other)
    {
        return other is ChiMatrix<T> matrix && Equals(matrix);
    }

    /// <summary>
    ///     Determines whether the specified <see cref="ChiMatrix{T}" /> is equal to the current vector.
    /// </summary>
    /// <param name="other">The vector to compare with the current vector.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(ChiMatrix<T> other)
    {
        return RowCount == other.RowCount && ColumnCount == other.ColumnCount && Span.SequenceEqual(other.Span);
    }

    /// <summary>
    ///     Determines whether two <see cref="ChiMatrix{T}" /> instances are equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns><c>true</c> if the two instances are equal; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ChiMatrix<T> left, ChiMatrix<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Determines whether two specified <see cref="ChiMatrix{T}" /> instances are not equal.
    /// </summary>
    /// <param name="left">The first matrix to compare.</param>
    /// <param name="right">The second matrix to compare.</param>
    /// <returns>
    ///     <c>true</c> if the matrices are not equal; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ChiMatrix<T> left, ChiMatrix<T> right)
    {
        return !(left == right);
    }

    #endregion
}

/// <summary>
///     Provides a safe, operational context for a <see cref="ChiMatrix{T}" />.
///     This is an ephemeral, stack-only view that enables algebraic operators
///     while preventing common errors like operating on temporary copies.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay()}")]
public readonly ref struct ChiMatrixOp<T>
    where T : unmanaged, IFloatingPoint<T>
{
    private readonly bool _disposeRef;

    #region Operator Overloads

    /// <summary>
    ///     Performs addition. The operation can be between two matrices of identical dimensions,
    ///     or between a matrix and a scalar (1x1 matrix).
    /// </summary>
    /// <param name="left">The left-hand matrix or scalar.</param>
    /// <param name="right">The right-hand matrix or scalar.</param>
    /// <returns>A new <see cref="ChiMatrix{T}" /> containing the result of the addition.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the matrix dimensions are not compatible for addition.</exception>
    public static ChiMatrix<T> operator +(ChiMatrixOp<T> left, ChiMatrixOp<T> right)
    {
        try
        {
            var leftRef = left._ref;
            var rightRef = right._ref;

            // Case 1 & 2: Scalar addition
            if (leftRef.IsScalar)
                return AddScalarToMatrix(rightRef, leftRef[0, 0]);
            if (rightRef.IsScalar)
                return AddScalarToMatrix(leftRef, rightRef[0, 0]);

            // Case 3: Element-wise addition
            if (leftRef.RowCount == rightRef.RowCount && leftRef.ColumnCount == rightRef.ColumnCount)
                return AddElementWise(leftRef, rightRef);

            throw new InvalidOperationException(
                $"Matrix dimensions must be identical for addition: [{leftRef.RowCount}x{leftRef.ColumnCount}] vs [{rightRef.RowCount}x{rightRef.ColumnCount}]");
        }
        finally
        {
            if (left._disposeRef) left._ref.Dispose();
            if (right._disposeRef) right._ref.Dispose();
        }
    }

    /// <summary>
    ///     Performs subtraction. The operation can be between two matrices of identical dimensions,
    ///     or between a matrix and a scalar (1x1 matrix).
    /// </summary>
    /// <param name="left">The left-hand matrix or scalar (minuend).</param>
    /// <param name="right">The right-hand matrix or scalar (subtrahend).</param>
    /// <returns>A new <see cref="ChiMatrix{T}" /> containing the result of the subtraction.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the matrix dimensions are not compatible for subtraction.</exception>
    public static ChiMatrix<T> operator -(ChiMatrixOp<T> left, ChiMatrixOp<T> right)
    {
        try
        {
            var leftRef = left._ref;
            var rightRef = right._ref;

            // Case 1: Matrix - Scalar
            if (rightRef.IsScalar)
                return AddScalarToMatrix(leftRef, -rightRef[0, 0]); // Subtraction is just addition of the negative

            // Case 2: Scalar - Matrix
            if (leftRef.IsScalar)
                return SubtractMatrixFromScalar(leftRef[0, 0], rightRef);

            // Case 3: Element-wise subtraction
            if (leftRef.RowCount == rightRef.RowCount && leftRef.ColumnCount == rightRef.ColumnCount)
                return SubtractElementWise(leftRef, rightRef);

            throw new InvalidOperationException(
                $"Matrix dimensions must be identical for subtraction: [{leftRef.RowCount}x{leftRef.ColumnCount}] vs [{rightRef.RowCount}x{rightRef.ColumnCount}]");
        }
        finally
        {
            if (left._disposeRef) left._ref.Dispose();
            if (right._disposeRef) right._ref.Dispose();
        }
    }

    /// <summary>
    ///     Performs multiplication. The specific operation (scalar, element-wise, dot product, or matrix multiplication)
    ///     is determined by the dimensions of the input matrices.
    /// </summary>
    /// <param name="left">The left-hand matrix or vector.</param>
    /// <param name="right">The right-hand matrix or vector.</param>
    /// <returns>A new <see cref="ChiMatrix{T}" /> containing the result of the operation.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the matrix dimensions are not compatible for any supported
    ///     multiplication type.
    /// </exception>
    public static ChiMatrix<T> operator *(ChiMatrixOp<T> left, ChiMatrixOp<T> right)
    {
        try
        {
            var leftRef = left._ref;
            var rightRef = right._ref;

            if (leftRef.IsScalar)
                return MultiplyMatrixByScalar(rightRef, leftRef[0, 0]);
            if (rightRef.IsScalar)
                return MultiplyMatrixByScalar(leftRef, rightRef[0, 0]);

            if (leftRef.ColumnCount == rightRef.RowCount)
                return MultiplyMatrixMatrix(leftRef, rightRef);

            throw new InvalidOperationException(
                $"Matrix dimensions are not compatible for multiplication: " +
                $"[{leftRef.RowCount}x{leftRef.ColumnCount}] * [{rightRef.RowCount}x{rightRef.ColumnCount}]");
        }
        finally
        {
            if (left._disposeRef) left._ref.Dispose();
            if (right._disposeRef) right._ref.Dispose();
        }
    }

    #endregion

    #region Instance Methods

    /// <summary>
    ///     Computes the transpose of the matrix.
    /// </summary>
    /// <returns>A new <see cref="ChiMatrix{T}" /> that is the transpose of the original.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiMatrix<T> Transpose()
    {
        try
        {
            return Transpose(_ref);
        }
        finally
        {
            if (_disposeRef) _ref.Dispose();
        }
    }

    /// <summary>
    ///     Computes the Cholesky decomposition of a symmetric, positive-definite matrix.
    /// </summary>
    /// <returns>The lower triangular matrix L, such that L * L' = an original matrix.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the matrix is not square.</exception>
    /// <exception cref="ArgumentException">Thrown if the matrix is not positive-definite.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiMatrix<T> Cholesky()
    {
        if (!_ref.IsSquare)
            throw new InvalidOperationException("Cholesky decomposition can only be applied to a square matrix.");

        try
        {
            return Cholesky(_ref);
        }
        finally
        {
            if (_disposeRef) _ref.Dispose();
        }
    }

    /// <summary>
    ///     Performs element-wise multiplication (the Hadamard product) with another matrix.
    /// </summary>
    /// <param name="other">The matrix to multiply with element-by-element.</param>
    /// <returns>A new <see cref="ChiMatrix{T}" /> containing the result of the Hadamard product.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the dimensions of this matrix and the other matrix are not identical.
    /// </exception>
    /// <remarks>
    ///     The Hadamard product, or element-wise product, is an operation on two matrices of the same dimensions,
    ///     resulting in another matrix of the same dimension where each element (i, j) is the product of
    ///     elements (i, j) of the original two matrices.
    /// </remarks>
    public ChiMatrix<T> Hadamard(ChiMatrixOp<T> other)
    {
        var leftRef = _ref;
        var rightRef = other._ref;

        if (leftRef.RowCount != rightRef.RowCount || leftRef.ColumnCount != rightRef.ColumnCount)
            throw new InvalidOperationException("Matrix dimensions must be identical for the Hadamard product.");

        try
        {
            return MultiplyElementWise(leftRef, rightRef);
        }
        finally
        {
            if (_disposeRef) _ref.Dispose();
            if (other._disposeRef) other._ref.Dispose();
        }
    }

    #endregion

    #region Private Fields & Constants

    private readonly ref ChiMatrix<T> _ref;

    #endregion

    #region Private Addition Operator Helpers

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ChiMatrix<T> AddScalarToMatrix(ChiMatrix<T> matrix, T scalar)
    {
        var result = ChiMatrix.Unsafe.Uninitialized<T>(matrix.RowCount, matrix.ColumnCount);
        var spanM = matrix.Span;
        var spanR = result.Span;

        if (Vector.IsHardwareAccelerated && (typeof(T) == typeof(float) || typeof(T) == typeof(double)))
        {
            var scalarVector = new Vector<T>(scalar);
            ref var rM = ref MemoryMarshal.GetReference(spanM);
            ref var rR = ref MemoryMarshal.GetReference(spanR);
            var i = 0;
            var vectorSize = Vector<T>.Count;
            var length = spanR.Length;

            for (; i <= length - vectorSize; i += vectorSize)
            {
                var vm = Unsafe.As<T, Vector<T>>(ref Unsafe.Add(ref rM, i));
                Unsafe.As<T, Vector<T>>(ref Unsafe.Add(ref rR, i)) = vm + scalarVector;
            }

            for (; i < length; i++)
                Unsafe.Add(ref rR, i) = Unsafe.Add(ref rM, i) + scalar;
        }
        else
        {
            for (var i = 0; i < spanR.Length; i++)
                spanR[i] = spanM[i] + scalar;
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ChiMatrix<T> AddElementWise(ChiMatrix<T> a, ChiMatrix<T> b)
    {
        var result = ChiMatrix.Unsafe.Uninitialized<T>(a.RowCount, a.ColumnCount);
        var spanA = a.Span;
        var spanB = b.Span;
        var spanR = result.Span;

        if (Vector.IsHardwareAccelerated && (typeof(T) == typeof(float) || typeof(T) == typeof(double)))
        {
            ref var rA = ref MemoryMarshal.GetReference(spanA);
            ref var rB = ref MemoryMarshal.GetReference(spanB);
            ref var rR = ref MemoryMarshal.GetReference(spanR);
            var i = 0;
            var vectorSize = Vector<T>.Count;
            var length = spanR.Length;

            for (; i <= length - vectorSize; i += vectorSize)
            {
                var va = Unsafe.As<T, Vector<T>>(ref Unsafe.Add(ref rA, i));
                var vb = Unsafe.As<T, Vector<T>>(ref Unsafe.Add(ref rB, i));
                Unsafe.As<T, Vector<T>>(ref Unsafe.Add(ref rR, i)) = va + vb;
            }

            for (; i < length; i++)
                Unsafe.Add(ref rR, i) = Unsafe.Add(ref rA, i) + Unsafe.Add(ref rB, i);
        }
        else
        {
            for (var i = 0; i < spanR.Length; i++)
                spanR[i] = spanA[i] + spanB[i];
        }

        return result;
    }

    #endregion

    #region Private Subtraction Operator Helpers

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ChiMatrix<T> SubtractMatrixFromScalar(T scalar, ChiMatrix<T> matrix)
    {
        var result = ChiMatrix.Unsafe.Uninitialized<T>(matrix.RowCount, matrix.ColumnCount);
        var spanM = matrix.Span;
        var spanR = result.Span;

        if (Vector.IsHardwareAccelerated && (typeof(T) == typeof(float) || typeof(T) == typeof(double)))
        {
            var scalarVector = new Vector<T>(scalar);
            ref var rM = ref MemoryMarshal.GetReference(spanM);
            ref var rR = ref MemoryMarshal.GetReference(spanR);
            var i = 0;
            var vectorSize = Vector<T>.Count;
            var length = spanR.Length;

            for (; i <= length - vectorSize; i += vectorSize)
            {
                var vm = Unsafe.As<T, Vector<T>>(ref Unsafe.Add(ref rM, i));
                Unsafe.As<T, Vector<T>>(ref Unsafe.Add(ref rR, i)) = scalarVector - vm;
            }

            for (; i < length; i++)
                Unsafe.Add(ref rR, i) = scalar - Unsafe.Add(ref rM, i);
        }
        else
        {
            for (var i = 0; i < spanR.Length; i++)
                spanR[i] = scalar - spanM[i];
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ChiMatrix<T> SubtractElementWise(ChiMatrix<T> a, ChiMatrix<T> b)
    {
        var result = ChiMatrix.Unsafe.Uninitialized<T>(a.RowCount, a.ColumnCount);
        var spanA = a.Span;
        var spanB = b.Span;
        var spanR = result.Span;

        if (Vector.IsHardwareAccelerated && (typeof(T) == typeof(float) || typeof(T) == typeof(double)))
        {
            ref var rA = ref MemoryMarshal.GetReference(spanA);
            ref var rB = ref MemoryMarshal.GetReference(spanB);
            ref var rR = ref MemoryMarshal.GetReference(spanR);
            var i = 0;
            var vectorSize = Vector<T>.Count;
            var length = spanR.Length;

            for (; i <= length - vectorSize; i += vectorSize)
            {
                var va = Unsafe.As<T, Vector<T>>(ref Unsafe.Add(ref rA, i));
                var vb = Unsafe.As<T, Vector<T>>(ref Unsafe.Add(ref rB, i));
                Unsafe.As<T, Vector<T>>(ref Unsafe.Add(ref rR, i)) = va - vb;
            }

            for (; i < length; i++)
                Unsafe.Add(ref rR, i) = Unsafe.Add(ref rA, i) - Unsafe.Add(ref rB, i);
        }
        else
        {
            for (var i = 0; i < spanR.Length; i++)
                spanR[i] = spanA[i] - spanB[i];
        }

        return result;
    }

    #endregion

    #region Private Multiplication Operator Helpers

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ChiMatrix<T> MultiplyMatrixByScalar(ChiMatrix<T> matrix, T scalar)
    {
        var result = ChiMatrix.Unsafe.Uninitialized<T>(matrix.RowCount, matrix.ColumnCount);
        var spanM = matrix.Span;
        var spanR = result.Span;

        if (Vector.IsHardwareAccelerated && (typeof(T) == typeof(float) || typeof(T) == typeof(double)))
        {
            var scalarVector = new Vector<T>(scalar);
            ref var rM = ref MemoryMarshal.GetReference(spanM);
            ref var rR = ref MemoryMarshal.GetReference(spanR);
            var i = 0;
            var vectorSize = Vector<T>.Count;
            for (; i <= spanR.Length - vectorSize; i += vectorSize)
            {
                var vm = Unsafe.As<T, Vector<T>>(ref Unsafe.Add(ref rM, i));
                Unsafe.As<T, Vector<T>>(ref Unsafe.Add(ref rR, i)) = vm * scalarVector;
            }

            for (; i < spanR.Length; i++)
                Unsafe.Add(ref rR, i) = Unsafe.Add(ref rM, i) * scalar;
        }
        else
        {
            for (var i = 0; i < spanR.Length; i++) spanR[i] = spanM[i] * scalar;
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ChiMatrix<T> MultiplyElementWise(ChiMatrix<T> a, ChiMatrix<T> b)
    {
        var result = ChiMatrix.Unsafe.Uninitialized<T>(a.RowCount, a.ColumnCount);
        var spanA = a.Span;
        var spanB = b.Span;
        var spanR = result.Span;

        if (Vector.IsHardwareAccelerated && (typeof(T) == typeof(float) || typeof(T) == typeof(double)))
        {
            ref var rA = ref MemoryMarshal.GetReference(spanA);
            ref var rB = ref MemoryMarshal.GetReference(spanB);
            ref var rR = ref MemoryMarshal.GetReference(spanR);
            var i = 0;
            var vectorSize = Vector<T>.Count;
            for (; i <= spanR.Length - vectorSize; i += vectorSize)
            {
                var va = Unsafe.As<T, Vector<T>>(ref Unsafe.Add(ref rA, i));
                var vb = Unsafe.As<T, Vector<T>>(ref Unsafe.Add(ref rB, i));
                Unsafe.As<T, Vector<T>>(ref Unsafe.Add(ref rR, i)) = va * vb;
            }

            for (; i < spanR.Length; i++)
                Unsafe.Add(ref rR, i) = Unsafe.Add(ref rA, i) * Unsafe.Add(ref rB, i);
        }
        else
        {
            for (var i = 0; i < spanR.Length; i++) spanR[i] = spanA[i] * spanB[i];
        }

        return result;
    }

    private static ChiMatrix<T> MultiplyMatrixMatrix(ChiMatrix<T> a, ChiMatrix<T> b)
    {
        using var bT = Transpose(b);
        var result = ChiMatrix.Unsafe.Uninitialized<T>(a.RowCount, b.ColumnCount);

        for (var i = 0; i < result.RowCount; i++)
        for (var j = 0; j < result.ColumnCount; j++)
        {
            var rowA = a.Span.Slice(i * a.ColumnCount, a.ColumnCount);
            var rowBt = bT.Span.Slice(j * bT.ColumnCount, bT.ColumnCount);

            result[i, j] = DotProductSpan(rowA, rowBt);
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T DotProductSpan(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
    {
        T result;
        if (Vector.IsHardwareAccelerated && (typeof(T) == typeof(float) || typeof(T) == typeof(double)))
        {
            var sumVector = Vector<T>.Zero;
            var i = 0;
            var vectorSize = Vector<T>.Count;
            for (; i <= left.Length - vectorSize; i += vectorSize)
                sumVector += new Vector<T>(left.Slice(i)) * new Vector<T>(right.Slice(i));
            result = Vector.Dot(sumVector, Vector<T>.One);
            for (; i < left.Length; i++) result += left[i] * right[i];
        }
        else
        {
            result = T.Zero;
            for (var i = 0; i < left.Length; i++) result += left[i] * right[i];
        }

        return result;
    }

    #endregion

    #region Private Multiplication Operator Helpers

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ChiMatrix<T> Transpose(ChiMatrix<T> matrix)
    {
        var result = ChiMatrix.Unsafe.Uninitialized<T>(matrix.ColumnCount, matrix.RowCount);

        for (var i = 0; i < matrix.RowCount; i++)
        for (var j = 0; j < matrix.ColumnCount; j++)
            result[j, i] = matrix[i, j];

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ChiMatrix<T> Cholesky(ChiMatrix<T> matrix)
    {
        var n = matrix.RowCount;
        var l = ChiMatrix.Unsafe.Uninitialized<T>(n, n);

        for (var i = 0; i < n; i++)
        for (var j = 0; j <= i; j++)
        {
            var sum = T.Zero;
            for (var k = 0; k < j; k++)
                sum += l[i, k] * l[j, k];

            if (i == j)
            {
                var diagonalValue = matrix[i, i] - sum;
                if (diagonalValue <= T.Zero)
                    throw new ArgumentException("Matrix is not positive-definite for Cholesky decomposition.",
                        nameof(matrix));
                l[i, i] = ChiMath.Sqrt(diagonalValue);
            }
            else
            {
                if (l[j, j] == T.Zero)
                    throw new InvalidOperationException(
                        "Division by zero during Cholesky decomposition; matrix may not be positive-definite.");
                l[i, j] = (matrix[i, j] - sum) / l[j, j];
            }
        }

        return l;
    }

    #endregion

    #region Internal & Boilerplate

    /// <summary>
    ///     Prevents usage of the parameterless constructor. Use static factory methods instead.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    [Obsolete(UseStaticFactoryMethodsMessage, true)]
    public ChiMatrixOp()
    {
        throw new InvalidOperationException(UseStaticFactoryMethodsMessage);
    }

    internal ChiMatrixOp(ref ChiMatrix<T> @ref, bool disposeRef)
    {
        _disposeRef = disposeRef;
        _ref = ref @ref;
    }

    private const string UseStaticFactoryMethodsMessage =
        "Please use static factory methods to create a new instance of ChiMatrix.";

    private string DebuggerDisplay()
    {
        return $"ChiMatrix<{typeof(T).Name}> [{_ref.RowCount} x {_ref.ColumnCount}] ({_ref.Length} total)";
    }

    #endregion
}

/// <summary>
///     Provides extension methods for <see cref="ChiMatrix{T}" />.
/// </summary>
public static class ChiMatrixExtensions
{
    /// <summary>
    ///     Acquires a non-owning, temporary reference to the matrix for use in a single operation.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <b>This is the safe, default choice for all matrix operations.</b>
    ///     </para>
    ///     <para>
    ///         It returns a temporary, stack-only view (<see cref="ChiMatrixOp{T}" />) that enables all algebraic
    ///         operators (+, *, -) and methods like <see cref="ChiMatrixOp{T}.Transpose()" />. It is designed
    ///         to be used inline within a single expression chain, preventing common errors related to operating
    ///         on temporary values.
    ///     </para>
    ///     <para>
    ///         <b>Note on Usage:</b> This method returns a `ref struct` wrapper. The C# compiler will prevent
    ///         you from storing this wrapper in a `using` statement or an `async` method. This is an intentional
    ///         design safeguard.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var c = a.Peek() * b.Peek();
    /// var d = a.Peek() + c.Peek();
    /// ]]></code>
    /// </example>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiMatrixOp<T> Peek<T>(ref this ChiMatrix<T> matrix)
        where T : unmanaged, IFloatingPoint<T>
    {
        return new ChiMatrixOp<T>(ref matrix, false);
    }

    /// <summary>
    ///     Acquires a consuming, temporary reference to the matrix, disposing of it after a single operation.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <b>This is an advanced optimization for performance-critical code.</b>
    ///     </para>
    ///     <para>
    ///         Use this method to immediately release the memory of a large, heap-allocated intermediate matrix
    ///         back to the pool. This can significantly reduce memory pressure in complex calculations. The matrix
    ///         is marked as invalid after the operation completes and should not be used again.
    ///     </para>
    ///     <para>
    ///         <b>Note on Usage:</b> Like <see cref="Peek{T}" />, this method returns a `ref struct` wrapper.
    ///         The C# compiler enforces its ephemeral use, preventing storage in `using` blocks. This ensures
    ///         that a matrix is not accidentally consumed and then reused.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// // b is a large, temporary matrix we no longer need.
    /// var c = a.Peek() * b.Consume(); 
    ///  
    /// // Now, b is invalid and its memory is available for reuse.
    /// ]]></code>
    /// </example>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiMatrixOp<T> Consume<T>(ref this ChiMatrix<T> matrix)
        where T : unmanaged, IFloatingPoint<T>
    {
        return new ChiMatrixOp<T>(ref matrix, true);
    }

    /// <summary>
    ///     Provides access to debug information about the matrix's internal state.
    /// </summary>
    public static ChiMatrix<T>.DebugInfo DebugInfo<T>(ref this ChiMatrix<T> matrix)
        where T : unmanaged, IFloatingPoint<T>
    {
        return new ChiMatrix<T>.DebugInfo(ref matrix);
    }
}