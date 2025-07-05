using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using ChiVariate.Internal;

namespace ChiVariate;

/// <summary>
///     Provides a collection of static methods for generating non-cryptographic hash values
///     from various input types.
/// </summary>
/// <remarks>
///     <para>
///         Important: this class isn't cryptographically secure, and not suitable for security-sensitive purposes.
///     </para>
///     <para>
///         Designed for use in scenarios such as data indexing and retrieval, deterministic data transformation
///         and partitioning, and non-cryptographic hashing. For generating high-entropy `long` seed values
///         suitable for `ChiRng`, see the <see cref="ChiSeed" /> class.
///     </para>
/// </remarks>
public static class ChiHash
{
    /// <summary>
    ///     Computes a 32-bit hash for an integer value.
    /// </summary>
    /// <param name="value">The <see cref="int" /> value to incorporate into the hash calculation.</param>
    /// <returns>An <see cref="int" /> representing the 32-bit integer hash value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Hash(int value)
    {
        return Chi32.UpdateHashValue(0, value);
    }

    /// <summary>
    ///     Computes a 32-bit hash for any number of strings.
    /// </summary>
    /// <param name="value">A <see cref="string" /> value to be hashed. Can't be null.</param>
    /// <returns>An <see cref="int" /> representing the 32-bit integer hash value.</returns>
    /// <remarks>
    ///     Computes a 32-bit hash for the specified string by processing its UTF-8 byte representation.
    ///     This ensures deterministic results across different platforms.
    ///     Handles buffer allocation efficiently using <c>stackalloc</c> for smaller strings
    ///     and <see cref="ArrayPool{T}" /> for larger ones.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="value" /> is null.
    /// </exception>
    public static int Hash(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        const int maxStackAllocSize = 512;

        var byteCount = Encoding.UTF8.GetByteCount(value);
        byte[]? rentedArray = null;
        var byteSpan = byteCount <= maxStackAllocSize
            ? stackalloc byte[byteCount]
            : RentArray(byteCount, out rentedArray);

        var combinedValue = InitializeCombiner();

        try
        {
            Encoding.UTF8.GetBytes(value, byteSpan);

            var intCount = byteSpan.Length & ~3;
            var intSpan = MemoryMarshal.Cast<byte, int>(byteSpan[..intCount]);

            foreach (var int32 in intSpan)
                CombineValue(ref combinedValue, int32);

            var orphanCount = byteSpan.Length & 3;
            if (orphanCount > 0)
            {
                var tailBytes = byteSpan[^orphanCount..];
                var last = 0;
                for (var i = 0; i < orphanCount; i++)
                    last |= tailBytes[i] << (i * 8);

                CombineValue(ref combinedValue, last);
            }
        }
        finally
        {
            if (byteSpan.Length > maxStackAllocSize)
                ArrayPool<byte>.Shared.Return(rentedArray!);
        }

        return Chi32.UpdateHashValue(0, combinedValue);

        static Span<byte> RentArray(int byteCount, out byte[] rentedArray)
        {
            rentedArray = ArrayPool<byte>.Shared.Rent(byteCount);
            return rentedArray.AsSpan(0, byteCount);
        }
    }

    /// <summary>
    ///     Computes a 32-bit hash value from a numeric inputs span of an unmanaged type
    ///     implementing <see cref="INumberBase{TSelf}" />.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="values">The span of numeric value to hash.</param>
    /// <returns>An <see cref="int" /> representing the 32-bit integer hash value.</returns>
    /// <remarks>
    ///     <para>
    ///         This method computes a deterministic 32-bit hash.
    ///         Each input value's complete bit pattern contributes to the final result, ensuring cross-platform
    ///         and cross-version reproducibility.
    ///         Values are reduced and combined using an intermediate mixing process before finalization.
    ///     </para>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Hash<T>(scoped ReadOnlySpan<T> values)
        where T : unmanaged, INumberBase<T>
    {
        var combinedValue = InitializeCombiner();

        foreach (var value in values)
            CombineValue(ref combinedValue, ReduceToInt32(value));

        return Chi32.UpdateHashValue(0, combinedValue);
    }

    /// <summary>
    ///     Computes a 32-bit hash value from a single numeric input of an unmanaged type
    ///     implementing <see cref="INumberBase{TSelf}" />.
    /// </summary>
    /// <typeparam name="T1">The type of the value.</typeparam>
    /// <param name="v1">The numeric value to hash.</param>
    /// <returns>An <see cref="int" /> representing the 32-bit integer hash value.</returns>
    /// <remarks>
    ///     <para>
    ///         This method computes a deterministic 32-bit hash.
    ///         Each input value's complete bit pattern contributes to the final result, ensuring cross-platform
    ///         and cross-version reproducibility.
    ///         Values are reduced and combined using an intermediate mixing process before finalization.
    ///     </para>
    /// </remarks>
    public static int Hash<T1>(T1 v1)
        where T1 : unmanaged, INumberBase<T1>
    {
        return Chi32.UpdateHashValue(0, ReduceToInt32(v1));
    }

    /// <summary>
    ///     Computes a 32-bit hash value from two numeric inputs of unmanaged types
    ///     implementing <see cref="INumberBase{TSelf}" />.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <param name="v1">The first numeric value to hash.</param>
    /// <param name="v2">The second numeric value to hash.</param>
    /// <returns>An <see cref="int" /> representing the 32-bit integer hash value.</returns>
    /// <inheritdoc cref="Hash{T1}(T1)" />
    public static int Hash<T1, T2>(T1 v1, T2 v2)
        where T1 : unmanaged, INumberBase<T1>
        where T2 : unmanaged, INumberBase<T2>
    {
        var combinedValue = InitializeCombiner();

        CombineValue(ref combinedValue, ReduceToInt32(v1));
        CombineValue(ref combinedValue, ReduceToInt32(v2));

        return Chi32.UpdateHashValue(0, combinedValue);
    }

    /// <summary>
    ///     Computes a 32-bit hash value from three numeric inputs of unmanaged types
    ///     implementing <see cref="INumberBase{TSelf}" />.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <param name="v1">The first numeric value to hash.</param>
    /// <param name="v2">The second numeric value to hash.</param>
    /// <param name="v3">The third numeric value to hash.</param>
    /// <returns>An <see cref="int" /> representing the 32-bit integer hash value.</returns>
    /// <inheritdoc cref="Hash{T1}(T1)" />
    public static int Hash<T1, T2, T3>(T1 v1, T2 v2, T3 v3)
        where T1 : unmanaged, INumberBase<T1>
        where T2 : unmanaged, INumberBase<T2>
        where T3 : unmanaged, INumberBase<T3>
    {
        var combinedValue = InitializeCombiner();

        CombineValue(ref combinedValue, ReduceToInt32(v1));
        CombineValue(ref combinedValue, ReduceToInt32(v2));
        CombineValue(ref combinedValue, ReduceToInt32(v3));

        return Chi32.UpdateHashValue(0, combinedValue);
    }

    /// <summary>
    ///     Computes a 32-bit hash value from four numeric inputs of unmanaged types
    ///     implementing <see cref="INumberBase{TSelf}" />.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <param name="v1">The first numeric value to hash.</param>
    /// <param name="v2">The second numeric value to hash.</param>
    /// <param name="v3">The third numeric value to hash.</param>
    /// <param name="v4">The fourth numeric value to hash.</param>
    /// <returns>An <see cref="int" /> representing the 32-bit integer hash value.</returns>
    /// <inheritdoc cref="Hash{T1}(T1)" />
    public static int Hash<T1, T2, T3, T4>(T1 v1, T2 v2, T3 v3, T4 v4)
        where T1 : unmanaged, INumberBase<T1>
        where T2 : unmanaged, INumberBase<T2>
        where T3 : unmanaged, INumberBase<T3>
        where T4 : unmanaged, INumberBase<T4>
    {
        var combinedValue = InitializeCombiner();

        CombineValue(ref combinedValue, ReduceToInt32(v1));
        CombineValue(ref combinedValue, ReduceToInt32(v2));
        CombineValue(ref combinedValue, ReduceToInt32(v3));
        CombineValue(ref combinedValue, ReduceToInt32(v4));

        return Chi32.UpdateHashValue(0, combinedValue);
    }

    /// <summary>
    ///     Computes a 32-bit hash value from five numeric inputs of unmanaged types
    ///     implementing <see cref="INumberBase{TSelf}" />.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="T5">The type of the fifth value.</typeparam>
    /// <param name="v1">The first numeric value to hash.</param>
    /// <param name="v2">The second numeric value to hash.</param>
    /// <param name="v3">The third numeric value to hash.</param>
    /// <param name="v4">The fourth numeric value to hash.</param>
    /// <param name="v5">The fifth numeric value to hash.</param>
    /// <returns>An <see cref="int" /> representing the 32-bit integer hash value.</returns>
    /// <inheritdoc cref="Hash{T1}(T1)" />
    public static int Hash<T1, T2, T3, T4, T5>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5)
        where T1 : unmanaged, INumberBase<T1>
        where T2 : unmanaged, INumberBase<T2>
        where T3 : unmanaged, INumberBase<T3>
        where T4 : unmanaged, INumberBase<T4>
        where T5 : unmanaged, INumberBase<T5>
    {
        var combinedValue = InitializeCombiner();

        CombineValue(ref combinedValue, ReduceToInt32(v1));
        CombineValue(ref combinedValue, ReduceToInt32(v2));
        CombineValue(ref combinedValue, ReduceToInt32(v3));
        CombineValue(ref combinedValue, ReduceToInt32(v4));
        CombineValue(ref combinedValue, ReduceToInt32(v5));

        return Chi32.UpdateHashValue(0, combinedValue);
    }

    /// <summary>
    ///     Computes a 32-bit hash value from six numeric inputs of unmanaged types
    ///     implementing <see cref="INumberBase{TSelf}" />.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="T5">The type of the fifth value.</typeparam>
    /// <typeparam name="T6">The type of the sixth value.</typeparam>
    /// <param name="v1">The first numeric value to hash.</param>
    /// <param name="v2">The second numeric value to hash.</param>
    /// <param name="v3">The third numeric value to hash.</param>
    /// <param name="v4">The fourth numeric value to hash.</param>
    /// <param name="v5">The fifth numeric value to hash.</param>
    /// <param name="v6">The sixth numeric value to hash.</param>
    /// <returns>An <see cref="int" /> representing the 32-bit integer hash value.</returns>
    /// <inheritdoc cref="Hash{T1}(T1)" />
    public static int Hash<T1, T2, T3, T4, T5, T6>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6)
        where T1 : unmanaged, INumberBase<T1>
        where T2 : unmanaged, INumberBase<T2>
        where T3 : unmanaged, INumberBase<T3>
        where T4 : unmanaged, INumberBase<T4>
        where T5 : unmanaged, INumberBase<T5>
        where T6 : unmanaged, INumberBase<T6>
    {
        var combinedValue = InitializeCombiner();

        CombineValue(ref combinedValue, ReduceToInt32(v1));
        CombineValue(ref combinedValue, ReduceToInt32(v2));
        CombineValue(ref combinedValue, ReduceToInt32(v3));
        CombineValue(ref combinedValue, ReduceToInt32(v4));
        CombineValue(ref combinedValue, ReduceToInt32(v5));
        CombineValue(ref combinedValue, ReduceToInt32(v6));

        return Chi32.UpdateHashValue(0, combinedValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int InitializeCombiner()
    {
        const int initialSeed = 0x46a74a57;

        return initialSeed;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CombineValue(ref int combinedValue, int value)
    {
        const uint multiplier = 0x8a2ab4d3;
        const uint xorValue = 0xfc2ed867;

        var combinedHash = (uint)(combinedValue ^ value) * multiplier;
        var offset = (int)combinedHash & 31;

        combinedValue ^= (int)BitOperations.RotateRight(combinedHash ^ xorValue, offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ReduceToInt32<T>(T value)
        where T : unmanaged, INumberBase<T>
    {
        if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(byte))
        {
            var valueAsByte = (uint)Unsafe.As<T, byte>(ref value);
            return (int)valueAsByte;
        }

        if (typeof(T) == typeof(short) || typeof(T) == typeof(ushort) || typeof(T) == typeof(char))
        {
            var valueAsUShort = (uint)Unsafe.As<T, ushort>(ref value);
            return (int)valueAsUShort;
        }

        if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
        {
            var valueAsUInt = Unsafe.As<T, uint>(ref value);
            return (int)valueAsUInt;
        }

        if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
        {
            var valueAsULong = Unsafe.As<T, ulong>(ref value);
            return FoldInt64ToInt32(valueAsULong);
        }

        if (typeof(T) == typeof(Int128) || typeof(T) == typeof(UInt128))
        {
            var valueAsUInt128 = Unsafe.As<T, UInt128>(ref value);
            return FoldInt128ToInt32(valueAsUInt128);
        }

        if (typeof(T) == typeof(double))
        {
            var valueAsULong = BitConverter.DoubleToUInt64Bits(Unsafe.As<T, double>(ref value));
            return FoldInt64ToInt32(valueAsULong);
        }

        if (typeof(T) == typeof(float))
        {
            var valueAsUInt = BitConverter.SingleToUInt32Bits(Unsafe.As<T, float>(ref value));
            return (int)valueAsUInt;
        }

        if (typeof(T) == typeof(Half))
        {
            var valueAsUInt = BitConverter.HalfToUInt16Bits(Unsafe.As<T, Half>(ref value));
            return valueAsUInt;
        }

        if (typeof(T) == typeof(decimal))
        {
            var decimalValue = Unsafe.As<T, decimal>(ref value);
            return FoldDecimalToInt32(decimalValue);
        }

        throw new NotSupportedException(
            $"Unsupported type provided: {typeof(T).Name}. " +
            $"{nameof(Hash)} requires types that can be explicitly converted to numeric values.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FoldInt64ToInt32(ulong value)
    {
        var mixed = unchecked((uint)value);
        value >>= 32;
        mixed ^= unchecked((uint)value);

        return (int)mixed;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FoldInt128ToInt32(UInt128 value)
    {
        var mixed = unchecked((uint)value);
        value >>= 32;
        mixed ^= unchecked((uint)value);
        value >>= 32;
        mixed ^= unchecked((uint)value);
        value >>= 32;
        mixed ^= unchecked((uint)value);

        return (int)mixed;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FoldDecimalToInt32(decimal value)
    {
        Span<int> parts = stackalloc int[4];
        decimal.TryGetBits(value, parts, out _);

        return parts[0] ^ parts[1] ^ parts[2] ^ parts[3];
    }
}