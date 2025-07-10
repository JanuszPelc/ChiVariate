// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Collections;
using System.Runtime.CompilerServices;
using ChiVariate.Internal;

namespace ChiVariate;

/// <summary>
///     A high-performance, pooled, and disposable enumerable collection used for temporary sample generation.
/// </summary>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
/// <remarks>
///     <para>
///         This class is part of ChiVariate's zero-allocation strategy. Instances are rented from a pool
///         and should be disposed of to return them to the pool.
///     </para>
///     <para>
///         The recommended way to use this is with a `foreach` loop or `LINQ` queries,
///         which will automatically handle disposal.
///     </para>
/// </remarks>
public sealed class ChiEnumerable<T> : IEnumerable<T>, IDisposable
{
    private static T[] _scaffoldingArray = [];
    private static readonly ChiPool<ChiEnumerable<T>> EnumerablePool = new();
    private static readonly ChiPool<ChiPoolableEnumerator<T>> EnumeratorPool = new();

    /// <summary>
    ///     The list containing the generated samples.
    /// </summary>
    /// <remarks>
    ///     This is exposed for advanced scenarios where direct manipulation of the underlying list is required.
    /// </remarks>
    public readonly List<T> List = [];

    private bool _disposed;
    private ChiPoolableEnumerator<T> _enumerator = null!;

    private ChiEnumerable()
    {
    }

    /// <summary>
    ///     Releases the resources used by the <see cref="ChiEnumerable{T}" />, clearing its internal list
    ///     and returning it and its enumerator to their respective pools for reuse.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (_disposed)
            return;

        List.Clear();

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (_enumerator is not null)
            EnumeratorPool.Recycle(_enumerator);
        _enumerator = null!;

        _disposed = true;

        EnumerablePool.Recycle(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        _enumerator = EnumeratorPool.Rent(static () => new ChiPoolableEnumerator<T>());

        _enumerator.Enumerable = this;
        _enumerator.List = List;
        _enumerator.Count = List.Count;
        _enumerator.Index = -1;

        return _enumerator;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<T>)this).GetEnumerator();
    }

    /// <summary>
    ///     Returns a high-performance, allocation-free enumerator that iterates through the collection.
    /// </summary>
    /// <returns>A <see cref="ChiEnumerator{T}" /> for this collection.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiEnumerator<T> GetEnumerator()
    {
        return new ChiEnumerator<T>(this);
    }

    /// <summary>
    ///     Rents a pooled <see cref="ChiEnumerable{T}" /> instance for use in custom sampler implementations.
    /// </summary>
    /// <param name="count">The number of items the enumerable should be pre-sized for.</param>
    /// <returns>A disposable <see cref="ChiEnumerable{T}" /> instance.</returns>
    /// <remarks>
    ///     This is an advanced method for library extensibility. The returned instance
    ///     must be disposed correctly to return it to the pool.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiEnumerable<T> Rent(int count)
    {
        var enumerable = EnumerablePool.Rent(() => new ChiEnumerable<T>());

        enumerable._disposed = false;

        FillWithDefault(enumerable.List, count);

        return enumerable;

        static void FillWithDefault(List<T> list, int count)
        {
            list.EnsureCapacity(count);

            lock (_scaffoldingArray)
            {
                if (_scaffoldingArray.Length < count)
                    Array.Resize(ref _scaffoldingArray, count);

                list.AddRange(_scaffoldingArray.AsSpan()[..count]);
            }
        }
    }
}

/// <summary>
///     A high-performance, allocation-free enumerator for <see cref="ChiEnumerable{T}" />.
/// </summary>
/// <typeparam name="T">The type of elements to enumerate.</typeparam>
/// <remarks>
///     This is a <c>ref struct</c> to ensure it is always stack-allocated, avoiding heap allocations during enumeration.
/// </remarks>
public ref struct ChiEnumerator<T>
{
    private readonly List<T> _list;
    private readonly int _count;

    private ChiEnumerable<T> _enumerable;
    private int _index;

    internal ChiEnumerator(ChiEnumerable<T> enumerable)
    {
        _enumerable = enumerable;
        _list = enumerable.List;
        _count = enumerable.List.Count;
        _index = -1;
    }

    /// <summary>
    ///     Gets the element at the current position of the enumerator.
    /// </summary>
    public readonly T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _list[_index];
    }

    /// <summary>
    ///     Advances the enumerator to the next element of the collection.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the enumerator was successfully advanced to the next element; <c>false</c> if the enumerator
    ///     has passed the end of the collection.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        return ++_index < _count;
    }

    /// <summary>
    ///     Disposes the underlying <see cref="ChiEnumerable{T}" />, returning it to the pool.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (_enumerable is not { } enumerable)
            return;

        _enumerable = null!;

        enumerable.Dispose();
    }

    /// <summary>
    ///     This method is not supported and will throw a <see cref="NotSupportedException" />.
    /// </summary>
    public void Reset()
    {
        throw new NotSupportedException();
    }
}

/// <summary>
///     A pooled enumerator that implements the <see cref="IEnumerator{T}" /> interface for compatibility,
///     used as a fallback when heap allocation is unavoidable.
/// </summary>
/// <typeparam name="T">The type of elements to enumerate.</typeparam>
public sealed class ChiPoolableEnumerator<T> : IEnumerator<T>
{
    internal int Count = -1;
    internal ChiEnumerable<T> Enumerable = null!;
    internal int Index = -1;
    internal List<T> List = null!;

    internal ChiPoolableEnumerator()
    {
    }

    /// <inheritdoc />
    public T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => List[Index];
    }

    object? IEnumerator.Current => Current;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        return ++Index < Count;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (Enumerable is not { } enumerable)
            return;

        Enumerable = null!;

        enumerable.Dispose();
    }

    /// <inheritdoc />
    void IEnumerator.Reset()
    {
        throw new NotSupportedException();
    }
}