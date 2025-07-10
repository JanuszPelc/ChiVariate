// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ChiVariate.Internal;

namespace ChiVariate;

/// <summary>
///     Provides a collection of static factory methods for creating and manipulating one-dimensional
///     vectors based on the <see cref="ChiVector{T}" /> type.
/// </summary>
public static class ChiVector
{
    #region Factory methods

    /// <summary>
    ///     Creates a new vector of the specified length, with all elements initialized to a specified value.
    /// </summary>
    /// <param name="length">The number of elements in the vector.</param>
    /// <param name="value">The value to assign to every element of the vector.</param>
    /// <returns>A new <see cref="ChiVector{T}" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiVector<T> Full<T>(int length, T value)
        where T : unmanaged
    {
        var vector = Unsafe.Uninitialized<T>(length);
        vector.Span.Fill(value);
        return vector;
    }

    /// <summary>
    ///     Creates a new vector of the specified length, with all elements set to zero.
    /// </summary>
    /// <param name="length">The number of elements in the vector.</param>
    /// <returns>A new <see cref="ChiVector{T}" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiVector<T> Zeros<T>(int length)
        where T : unmanaged
    {
        var vector = Unsafe.Uninitialized<T>(length);
        vector.Span.Clear();
        return vector;
    }

    /// <summary>
    ///     Creates a new vector by copying the elements from a source span.
    /// </summary>
    /// <param name="sourceSpan">The one-dimensional source span to copy from.</param>
    /// <returns>A new <see cref="ChiVector{T}" /> containing a copy of the source data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiVector<T> With<T>(scoped ReadOnlySpan<T> sourceSpan)
        where T : unmanaged
    {
        var vector = Unsafe.Uninitialized<T>(sourceSpan.Length);
        sourceSpan.CopyTo(vector.Span);
        return vector;
    }

    /// <summary>
    ///     Provides access to advanced, performance-oriented factory methods that require the caller
    ///     to adhere to specific safety contracts to avoid undefined behavior.
    /// </summary>
    public static class Unsafe
    {
        /// <summary>
        ///     [Advanced] Creates a vector without initializing its contents, returning a buffer that
        ///     may contain arbitrary garbage data from previous memory operations.
        /// </summary>
        /// <param name="length">The number of elements in the vector.</param>
        /// <typeparam name="T">The unmanaged type of the vector elements.</typeparam>
        /// <returns>
        ///     A new <see cref="ChiVector{T}" /> with uninitialized backing memory.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <b>Warning:</b> This method is a high-performance feature for advanced scenarios.
        ///         The caller assumes full responsibility for initializing every element of the returned vector
        ///         before any read operations are performed. Failure to do so will result in reading
        ///         uninitialized memory, leading to unpredictable and erroneous behavior.
        ///     </para>
        ///     <para>
        ///         Use this method only when you can guarantee that the entire content of the vector will be
        ///         unconditionally overwritten immediately after creation. For all other cases, prefer safer factory
        ///         methods like <see cref="ChiVector.Zeros{T}" />.
        ///     </para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ChiVector<T> Uninitialized<T>(int length) where T : unmanaged
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length, nameof(length));

            var vector = default(ChiVector<T>) with
            {
                Length = length,
                IsValid = true,
                HeapData = length > ChiVector<T>.MaxInlineLength
                    ? ChiArrayPool<T>.Rent(length, false)
                    : null
            };
            return vector;
        }
    }

    #endregion
}

/// <summary>
///     Represents a high-performance, one-dimensional vector with a hybrid memory model.
///     Small vectors are stored inline in a fixed-size byte buffer; larger vectors use pooled heap memory.
/// </summary>
/// <typeparam name="T">The numeric type of the vector elements. Must be an unmanaged type.</typeparam>
/// <remarks>
///     <para>
///         This type uses a fixed 1024-byte inline buffer that can store different numbers of elements
///         depending on the size of type T. For example: 256 ints, 128 longs, 32 decimals, etc.
///     </para>
///     <para>
///         This type is a core building block for zero-allocation sampling. It provides a disposable,
///         span-like container that can be returned from methods without causing heap allocations for common sizes.
///         It is designed to be short-lived and should be disposed of promptly, ideally with a `using` statement.
///     </para>
/// </remarks>
[DebuggerDisplay("{DebuggerDisplay()}")]
public struct ChiVector<T> : IDisposable, IEquatable<ChiVector<T>>
    where T : unmanaged
{
    #region Disposal

    /// <summary>
    ///     Releases any heap-allocated memory rented from the memory pool.
    ///     If the vector is stored inline (in the byte buffer), this method is a no-op.
    /// </summary>
    public void Dispose()
    {
        if (!IsValid)
            return;

        ChiArrayPool<T>.Return(HeapData);
        IsValid = false;
    }

    #endregion

    #region Instance Methods

    /// <summary>
    ///     Creates a new, one-dimensional heap-allocated array by copying the elements from the vector.
    /// </summary>
    /// <returns>A new T[] array containing a copy of the vector's data.</returns>
    /// <remarks>
    ///     This method provides a convenient way to convert a <see cref="ChiVector{T}" />
    ///     into a standard array for interoperability or longer-term storage.
    ///     The returned array is completely independent of the source vector.
    /// </remarks>
    public T[] ToArray()
    {
        var array = new T[Length];
        Span.CopyTo(array);
        return array;
    }

    /// <summary>
    ///     Gets a <see cref="Span{T}" /> view over the vector's underlying contiguous memory.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the vector has been disposed and its memory was heap-allocated.</exception>
    public Span<T> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (HeapData is not null)
            {
                if (!IsValid)
                    throw new ObjectDisposedException(nameof(ChiVector<T>));
                return HeapData.AsSpan()[..Length];
            }

            ref var byteRef = ref Unsafe.As<InlineBuffer, byte>(ref _inlineBuffer);
            ref var elementRef = ref Unsafe.As<byte, T>(ref byteRef);
            return MemoryMarshal.CreateSpan(ref elementRef, Length);
        }
    }

    #endregion

    #region Internal Fields & Constants

    private const int InlineBufferBytes = 1024;
    internal static int MaxInlineLength => InlineBufferBytes / Unsafe.SizeOf<T>();

    internal bool IsValid { get; set; }
    internal T[]? HeapData { get; init; }

    private InlineBuffer _inlineBuffer;

    #endregion

    #region Instance Properties

    /// <summary>
    ///     Gets the total number of elements in the vector.
    /// </summary>
    public int Length { get; internal init; }

    /// <summary>
    ///     Gets a reference to the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>A reference to the element at the specified position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is out of bounds.</exception>
    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ((uint)index >= (uint)Length)
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    $"Index {index} is out of bounds for a vector of length {Length}.");

            return ref Span[index];
        }
    }

    #endregion

    #region Internal & Boilerplate

    /// <summary>
    ///     Fixed-size byte buffer for inline storage.
    /// </summary>
    [InlineArray(InlineBufferBytes)]
    private struct InlineBuffer
    {
        private byte _element0;
    }

    /// <summary>
    ///     Intentionally prevents usage of the parameterless constructor.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    [Obsolete(UseStaticFactoryMethodsMessage, true)]
    public ChiVector()
    {
        throw new InvalidOperationException(UseStaticFactoryMethodsMessage);
    }

    private const string UseStaticFactoryMethodsMessage =
        $"Please use the static factory methods on '{nameof(ChiVector)}' to create '{nameof(ChiVector)}<T>' instances.";

    private string DebuggerDisplay()
    {
        var maxInline = MaxInlineLength;
        var storageType = HeapData is not null ? "Heap" : "Inline";
        return $"ChiVector<{typeof(T).Name}>[{Length}] ({storageType}, Max Inline: {maxInline})";
    }

    /// <summary>
    ///     Provides a snapshot of the vector's internal state for debugging purposes.
    /// </summary>
    public readonly ref struct DebugInfo
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DebugInfo" /> struct.
        /// </summary>
        /// <param name="vector">The vector being inspected.</param>
        internal DebugInfo(ref ChiVector<T> vector)
        {
            _vector = ref vector;
        }

        private readonly ref ChiVector<T> _vector;

        /// <summary>
        ///     Gets a value indicating whether the vector's data is stored on the heap (rented from a pool).
        /// </summary>
        public bool IsHeapBacked => _vector.HeapData is not null;

        /// <summary>
        ///     Gets a value indicating whether the vector is still valid (i.e., has not been disposed).
        /// </summary>
        public bool IsValid => _vector.IsValid;
    }

    /// <summary>
    ///     Returns a hash code for the current vector based on its length and element values.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Length);
        hash.AddBytes(MemoryMarshal.AsBytes(Span));
        return hash.ToHashCode();
    }

    /// <summary>
    ///     Determines whether the specified <see cref="ChiVector{T}" /> is equal to the current vector.
    /// </summary>
    /// <param name="other">The vector to compare with the current vector.</param>
    public override bool Equals(object? other)
    {
        return other is ChiVector<T> vector && Equals(vector);
    }

    /// <summary>
    ///     Determines whether the specified <see cref="ChiVector{T}" /> is equal to the current vector.
    /// </summary>
    /// <param name="other">The vector to compare with the current vector.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(ChiVector<T> other)
    {
        return Length == other.Length && Span.SequenceEqual(other.Span);
    }

    /// <summary>
    ///     Determines whether two <see cref="ChiVector{T}" /> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="ChiVector{T}" /> to compare.</param>
    /// <param name="right">The second <see cref="ChiVector{T}" /> to compare.</param>
    /// <returns>
    ///     <c>true</c> if the two <see cref="ChiVector{T}" /> instances are equal; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ChiVector<T> left, ChiVector<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Determines whether two <see cref="ChiVector{T}" /> instances are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="ChiVector{T}" /> to compare.</param>
    /// <param name="right">The second <see cref="ChiVector{T}" /> to compare.</param>
    /// <returns>
    ///     <c>true</c> if the specified instances are not equal; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ChiVector<T> left, ChiVector<T> right)
    {
        return !(left == right);
    }

    #endregion
}