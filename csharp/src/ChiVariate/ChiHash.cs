using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using ChiVariate.Internal;

namespace ChiVariate;

/// <summary>
///     A deterministic hash builder that provides cross-platform reproducible hash codes
///     for non-cryptographic purposes.
/// </summary>
/// <remarks>
///     <para>
///         ChiHash produces deterministic hash values that remain consistent across
///         different platforms, .NET versions, and application runs. This makes it
///         suitable for scenarios requiring reproducible hashing: save files,
///         networking protocols, procedural generation, and distributed systems.
///     </para>
///     <para>
///         For hash table security and DoS protection, seed with ChiHash.Seed
///         or a custom entropy source to introduce per-application randomization.
///     </para>
///     <para>
///         Important: This is not cryptographically secure and must not be used for
///         security-sensitive purposes such as password hashing or digital signatures.
///     </para>
/// </remarks>
/// <example>
///     <code>
/// // Deterministic hashing
/// var hash = new ChiHash()
///     .Add(playerId)
///     .Add(playerName)
///     .Add(level)
///     .HashCode;
/// 
/// // Security-conscious hashing (DoS protection)
/// var secureHash = new ChiHash()
///     .Add(ChiHash.Seed)
///     .Add(data)
///     .HashCode;
/// </code>
/// </example>
public ref struct ChiHash
{
    /// <summary>
    ///     A pseudo-randomly generated seed value that remains constant for the lifetime
    ///     of the current application instance. Each application restart generates a new value.
    /// </summary>
    /// <remarks>
    ///     Use this seed when you need non-deterministic hashing for security purposes
    ///     (e.g., DoS protection in hash tables) while maintaining consistency within
    ///     the current application session.
    /// </remarks>
    public static long Seed { get; } = ChiSeed.GenerateUnique();

    /// <summary>
    ///     Gets the current 32-bit hash code based on all values added so far.
    /// </summary>
    /// <value>A 32-bit signed integer hash code.</value>
    public int Hash { get; private set; }

    /// <summary>
    ///     Adds a string value to the hash calculation.
    /// </summary>
    /// <param name="value">The string to add to the hash. Null values are treated as empty strings.</param>
    /// <returns>A new ChiHash instance with the string incorporated into the hash calculation.</returns>
    /// <remarks>
    ///     <para>
    ///         This method returns a new ChiHash instance and does not modify the original.
    ///         Always use the returned value:
    ///     </para>
    ///     <code>
    /// // Correct - fluent style
    /// var hash = new ChiHash().Add(value1).Add(value2).HashCode;
    ///  
    /// // Correct - with reassignment  
    /// var builder = new ChiHash();
    /// builder = builder.Add(value);
    ///  
    /// // Incorrect - mutation is lost
    /// var builder = new ChiHash();
    /// builder.Add(value); // This does nothing!
    /// </code>
    /// </remarks>
    [Pure]
    public ChiHash Add(string? value)
    {
        HashString(value ?? "");
        return this;
    }

    /// <summary>
    ///     Adds a value of any supported type to the hash calculation.
    /// </summary>
    /// <typeparam name="T">The type of value to add.</typeparam>
    /// <param name="value">The value to add to the hash.</param>
    /// <returns>A new ChiHash instance with the value incorporated into the hash calculation.</returns>
    /// <remarks>
    ///     <para>
    ///         Supports all standard numeric types, bool, enums, BigInteger, Guid, Complex,
    ///         DateTime, DateTimeOffset, and TimeSpan.
    ///     </para>
    /// </remarks>
    /// <inheritdoc cref="Add(string)" />
    [Pure]
    public ChiHash Add<T>(T value)
    {
        var result = this;

        // Standard numeric types
        if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(byte))
        {
            var byteValue = Unsafe.As<T, byte>(ref value);
            result.Hash = Chi32.UpdateHashValue(result.Hash, byteValue);
        }
        else if (typeof(T) == typeof(short) || typeof(T) == typeof(ushort) || typeof(T) == typeof(char))
        {
            var shortValue = Unsafe.As<T, ushort>(ref value);
            result.Hash = Chi32.UpdateHashValue(result.Hash, shortValue);
        }
        else if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
        {
            var intValue = Unsafe.As<T, uint>(ref value);
            result.Hash = Chi32.UpdateHashValue(result.Hash, (int)intValue);
        }
        else if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
        {
            var longValue = Unsafe.As<T, ulong>(ref value);
            result.Hash = Chi32.UpdateHashValue(result.Hash, (int)(longValue & 0xFFFFFFFF));
            result.Hash = Chi32.UpdateHashValue(result.Hash, (int)(longValue >> 32));
        }
        else if (typeof(T) == typeof(Int128) || typeof(T) == typeof(UInt128))
        {
            var int128Value = Unsafe.As<T, UInt128>(ref value);
            result.Hash = Chi32.UpdateHashValue(result.Hash, (int)(int128Value & 0xFFFFFFFF));
            result.Hash = Chi32.UpdateHashValue(result.Hash, (int)((int128Value >> 32) & 0xFFFFFFFF));
            result.Hash = Chi32.UpdateHashValue(result.Hash, (int)((int128Value >> 64) & 0xFFFFFFFF));
            result.Hash = Chi32.UpdateHashValue(result.Hash, (int)((int128Value >> 96) & 0xFFFFFFFF));
        }
        else if (typeof(T) == typeof(double))
        {
            var doubleValue = Unsafe.As<T, double>(ref value);
            var bits = BitConverter.DoubleToUInt64Bits(doubleValue);
            result.Hash = Chi32.UpdateHashValue(result.Hash, (int)(bits & 0xFFFFFFFF));
            result.Hash = Chi32.UpdateHashValue(result.Hash, (int)(bits >> 32));
        }
        else if (typeof(T) == typeof(float))
        {
            var floatValue = Unsafe.As<T, float>(ref value);
            var bits = BitConverter.SingleToUInt32Bits(floatValue);
            result.Hash = Chi32.UpdateHashValue(result.Hash, (int)bits);
        }
        else if (typeof(T) == typeof(Half))
        {
            var halfValue = Unsafe.As<T, Half>(ref value);
            var bits = BitConverter.HalfToUInt16Bits(halfValue);
            result.Hash = Chi32.UpdateHashValue(result.Hash, bits);
        }
        else if (typeof(T) == typeof(decimal))
        {
            var decimalValue = Unsafe.As<T, decimal>(ref value);
            Span<int> parts = stackalloc int[4];
            decimal.TryGetBits(decimalValue, parts, out _);
            for (var i = 0; i < 4; i++)
                result.Hash = Chi32.UpdateHashValue(result.Hash, parts[i]);
        }
        // Special types
        else if (typeof(T) == typeof(bool))
        {
            var boolValue = Unsafe.As<T, bool>(ref value);
            result.Hash = Chi32.UpdateHashValue(result.Hash, boolValue ? 1 : 0);
        }
        else if (typeof(T).IsEnum)
        {
            var underlyingType = Enum.GetUnderlyingType(typeof(T));

            if (underlyingType == typeof(byte))
            {
                result.Hash = Chi32.UpdateHashValue(result.Hash, Unsafe.As<T, byte>(ref value));
            }
            else if (underlyingType == typeof(sbyte))
            {
                result.Hash = Chi32.UpdateHashValue(result.Hash, Unsafe.As<T, sbyte>(ref value));
            }
            else if (underlyingType == typeof(short))
            {
                result.Hash = Chi32.UpdateHashValue(result.Hash, Unsafe.As<T, short>(ref value));
            }
            else if (underlyingType == typeof(ushort))
            {
                result.Hash = Chi32.UpdateHashValue(result.Hash, Unsafe.As<T, ushort>(ref value));
            }
            else if (underlyingType == typeof(int))
            {
                result.Hash = Chi32.UpdateHashValue(result.Hash, Unsafe.As<T, int>(ref value));
            }
            else if (underlyingType == typeof(uint))
            {
                result.Hash = Chi32.UpdateHashValue(result.Hash, (int)Unsafe.As<T, uint>(ref value));
            }
            else if (underlyingType == typeof(long))
            {
                var longValue = Unsafe.As<T, ulong>(ref value);
                result.Hash = Chi32.UpdateHashValue(result.Hash, (int)(longValue & 0xFFFFFFFF));
                result.Hash = Chi32.UpdateHashValue(result.Hash, (int)(longValue >> 32));
            }
            else if (underlyingType == typeof(ulong))
            {
                var longValue = Unsafe.As<T, ulong>(ref value);
                result.Hash = Chi32.UpdateHashValue(result.Hash, (int)(longValue & 0xFFFFFFFF));
                result.Hash = Chi32.UpdateHashValue(result.Hash, (int)(longValue >> 32));
            }
        }
        else if (typeof(T) == typeof(BigInteger))
        {
            var bigInt = Unsafe.As<T, BigInteger>(ref value);
            var bytes = bigInt.ToByteArray();

            var intCount = bytes.Length & ~3;
            for (var i = 0; i < intCount; i += 4)
            {
                var chunk = bytes[i] | (bytes[i + 1] << 8) | (bytes[i + 2] << 16) | (bytes[i + 3] << 24);
                result.Hash = Chi32.UpdateHashValue(result.Hash, chunk);
            }

            var orphanCount = bytes.Length & 3;
            if (orphanCount > 0)
            {
                var lastChunk = 0;
                for (var i = 0; i < orphanCount; i++)
                    lastChunk |= bytes[intCount + i] << (i * 8);
                result.Hash = Chi32.UpdateHashValue(result.Hash, lastChunk);
            }
        }
        else if (typeof(T) == typeof(Guid))
        {
            var guid = Unsafe.As<T, Guid>(ref value);
            Span<byte> bytes = stackalloc byte[16];
            guid.TryWriteBytes(bytes);

            var intSpan = MemoryMarshal.Cast<byte, int>(bytes);
            if (!BitConverter.IsLittleEndian)
                for (var index = 0; index < intSpan.Length; index++)
                    intSpan[index] = BinaryPrimitives.ReverseEndianness(intSpan[index]);

            foreach (var chunk in intSpan)
                result.Hash = Chi32.UpdateHashValue(result.Hash, chunk);
        }
        else if (typeof(T) == typeof(Complex))
        {
            var complex = Unsafe.As<T, Complex>(ref value);
            return result.Add(complex.Real).Add(complex.Imaginary);
        }
        else if (typeof(T) == typeof(DateTime))
        {
            var dateTime = Unsafe.As<T, DateTime>(ref value);
            return result.Add(dateTime.ToBinary());
        }
        else if (typeof(T) == typeof(DateTimeOffset))
        {
            var dateTimeOffset = Unsafe.As<T, DateTimeOffset>(ref value);
            return result.Add(dateTimeOffset.Ticks).Add(dateTimeOffset.Offset.Ticks);
        }
        else if (typeof(T) == typeof(TimeSpan))
        {
            var timeSpan = Unsafe.As<T, TimeSpan>(ref value);
            return result.Add(timeSpan.Ticks);
        }
        else
        {
            throw new NotSupportedException(
                $"Type {typeof(T).Name} is not supported. " +
                $"Supported types: all numeric types implementing INumber<T>, string, bool, enums, " +
                $"BigInteger, Guid, Complex, DateTime, DateTimeOffset, TimeSpan.");
        }

        return result;
    }

    /// <summary>
    ///     Adds a span of values to the hash calculation.
    /// </summary>
    /// <typeparam name="T">The type of values to add.</typeparam>
    /// <param name="values">The span of values to add to the hash.</param>
    /// <returns>A new ChiHash instance with all values incorporated into the hash calculation.</returns>
    /// <inheritdoc cref="Add(string)" />
    [Pure]
    public ChiHash Add<T>(scoped ReadOnlySpan<T> values)
    {
        var result = this;
        foreach (var value in values)
            result = result.Add(value);
        return result;
    }

    /// <summary>
    ///     Adds a span of values to the hash calculation.
    /// </summary>
    /// <typeparam name="T">The type of values to add.</typeparam>
    /// <param name="values">The span of values to add to the hash.</param>
    /// <returns>A new ChiHash instance with all values incorporated into the hash calculation.</returns>
    /// <inheritdoc cref="Add(string)" />
    [Pure]
    public ChiHash Add<T>(scoped Span<T> values)
    {
        var result = this;
        foreach (var value in values)
            result = result.Add(value);
        return result;
    }

    private void HashString(string value)
    {
        const int maxStackAllocSize = 512;

        var byteCount = Encoding.UTF8.GetByteCount(value);
        byte[]? rentedArray = null;
        var byteSpan = byteCount <= maxStackAllocSize
            ? stackalloc byte[byteCount]
            : RentArray(byteCount, out rentedArray);

        try
        {
            Encoding.UTF8.GetBytes(value, byteSpan);

            // Process in 32-bit chunks with endianness normalization for determinism
            var intCount = byteSpan.Length & ~3;
            var intSpan = MemoryMarshal.Cast<byte, int>(byteSpan[..intCount]);

            // Normalize endianness to ensure cross-platform determinism
            if (!BitConverter.IsLittleEndian)
                for (var index = 0; index < intSpan.Length; index++)
                    intSpan[index] = BinaryPrimitives.ReverseEndianness(intSpan[index]);

            foreach (var chunk in intSpan)
                Hash = Chi32.UpdateHashValue(Hash, chunk);

            // Handle remaining bytes (always in little-endian order)
            var orphanCount = byteSpan.Length & 3;
            if (orphanCount > 0)
            {
                var tailBytes = byteSpan[^orphanCount..];
                var lastChunk = 0;
                for (var i = 0; i < orphanCount; i++)
                    lastChunk |= tailBytes[i] << (i * 8);
                Hash = Chi32.UpdateHashValue(Hash, lastChunk);
            }
        }
        finally
        {
            if (rentedArray != null) ArrayPool<byte>.Shared.Return(rentedArray);
        }

        static Span<byte> RentArray(int byteCount, out byte[] rentedArray)
        {
            rentedArray = ArrayPool<byte>.Shared.Rent(byteCount);
            return rentedArray.AsSpan(0, byteCount);
        }
    }
}