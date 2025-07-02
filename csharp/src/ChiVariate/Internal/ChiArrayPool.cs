using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChiVariate.Internal;

/// <summary>
///     A high-performance, thread-safe array pool that uses power-of-two sizing for optimal memory reuse.
///     Arrays that aren't returned are safely garbage collected without memory leaks.
/// </summary>
/// <typeparam name="T">The element type. Must be an unmanaged type.</typeparam>
internal static class ChiArrayPool<T> where T : unmanaged
{
    #region Constants

    private const int MinPooledSizeBytes = 16;
    private const int MaxPooledSizeBytes = 1024 * 1024; // 1MB
    private const int MaxBucketSizeBytes = 64 * 1024 * 1024; // 64MB per bucket

    private static readonly int ElementSize = Unsafe.SizeOf<T>();

    // ReSharper disable StaticMemberInGenericType
    private static readonly int MinPooledElements = Math.Max(1, MinPooledSizeBytes / ElementSize);
    private static readonly int MaxPooledElements = MaxPooledSizeBytes / ElementSize;
    private static readonly int MaxBucketElements = MaxBucketSizeBytes / ElementSize;

    #endregion

    #region Bucket Infrastructure

    private static readonly ConcurrentQueue<T[]>[] Buckets;
    private static readonly int MinBucketIndex;

    static ChiArrayPool()
    {
        MinBucketIndex = GetBucketIndex(MinPooledElements);
        var maxBucketIndex = GetBucketIndex(MaxPooledElements);

        var bucketCount = maxBucketIndex - MinBucketIndex + 1;
        Buckets = new ConcurrentQueue<T[]>[bucketCount];

        for (var i = 0; i < bucketCount; i++)
            Buckets[i] = new ConcurrentQueue<T[]>();
    }

    #endregion

    #region Public API

    /// <summary>
    ///     Rents an array from the pool with at least the specified number of elements.
    /// </summary>
    /// <param name="minimumLength">The minimum number of elements required.</param>
    /// <param name="clearArray">Whether to clear the array contents before returning it.</param>
    /// <returns>An array with length equal to the next power of two >= minimumLength, or a fresh allocation if too large.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Rent(int minimumLength, bool clearArray = true)
    {
        if (minimumLength <= 0)
            throw new ArgumentOutOfRangeException(nameof(minimumLength),
                "Minimum length must be positive.");

        if (minimumLength > MaxPooledElements)
        {
            var largeArray = new T[minimumLength];
            return largeArray;
        }

        var adjustedLength = Math.Max(minimumLength, MinPooledElements);
        var powerOfTwoSize = RoundUpToPowerOfTwo(adjustedLength);
        var bucketIndex = GetBucketIndex(powerOfTwoSize) - MinBucketIndex;

        Debug.Assert(bucketIndex >= 0 && bucketIndex < Buckets.Length);

        if (!Buckets[bucketIndex].TryDequeue(out var pooledArray))
            return new T[powerOfTwoSize];

        if (clearArray) Array.Clear(pooledArray);

        return pooledArray;
    }

    /// <summary>
    ///     Returns an array to the pool for reuse. The array must have a power-of-two length.
    /// </summary>
    /// <param name="array">The array to return. If null, this method returns immediately.</param>
    /// <exception cref="ArgumentException">Thrown if the array length is not a power of two.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Return(T[]? array)
    {
        if (array is null) return;

        var length = array.Length;
        if (length == 0) return;

        var targetLength = Math.Max(length, MinPooledElements);

        if (!IsPowerOfTwo(targetLength))
            throw new ArgumentException($"Array length {length} rounds to {targetLength} which is not a power of two.",
                nameof(array));

        if (targetLength > MaxPooledElements) return;

        var bucketIndex = GetBucketIndex(targetLength) - MinBucketIndex;
        Debug.Assert(bucketIndex >= 0 && bucketIndex < Buckets.Length);

#if DEBUG
        Debug.Assert(!Buckets[bucketIndex].Contains(array),
            $"Attempted to return an array that is already in the pool. Type: {typeof(T).Name}");
#endif

        var currentBucketCount = Buckets[bucketIndex].Count;
        var maxArraysInBucket = MaxBucketElements / targetLength;
        if (currentBucketCount >= maxArraysInBucket)
            return;

        Buckets[bucketIndex].Enqueue(array);
    }

    #endregion

    #region Helper Methods

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsPowerOfTwo(int value)
    {
        return int.IsPow2(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int RoundUpToPowerOfTwo(int value)
    {
        if (value <= 1) return 1;
        if ((value & (value - 1)) == 0) return value;

        var msb = 31 - BitOperations.LeadingZeroCount((uint)value);
        return 1 << (msb + 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetBucketIndex(int powerOfTwoSize)
    {
        Debug.Assert(IsPowerOfTwo(powerOfTwoSize));

        return 31 - BitOperations.LeadingZeroCount((uint)powerOfTwoSize);
    }

    #endregion
}