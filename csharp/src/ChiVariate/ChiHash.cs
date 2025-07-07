using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
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
///         For hash table security and DoS protection, use ChiHash.Seed value
///         or a custom entropy source to introduce per-application randomization.
///     </para>
///     <para>
///         This hashing algorithm is not cryptographically secure and should not be used for
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
///     .Hash;
///  
/// // Security-conscious hashing
/// var secureHash = new ChiHash()
///     .Add(ChiHash.Seed)
///     .Add(data)
///     .Hash;
/// </code>
/// </example>
public ref struct ChiHash
{
    /// <summary>
    ///     A pseudo-randomly generated seed value that remains constant for the lifetime
    ///     of the current application instance. Each application restart generates a new value.
    /// </summary>
    /// <remarks>
    ///     This seed enables non-deterministic hashing for security purposes
    ///     (e.g., DoS protection in hash tables) while maintaining consistency within
    ///     the current application session.
    /// </remarks>
    public static int Seed { get; } = new ChiHash().Add(ChiSeed.GenerateUnique()).Hash;

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
    ///         This method returns a new ChiHash instance. It is recommended to always use the returned value:
    ///     </para>
    ///     <code>
    /// // Recommended - fluent style
    /// var hash = new ChiHash().Add(value1).Add(value2).Hash;
    ///  
    /// // Correct - with reassignment  
    /// var builder = new ChiHash();
    /// builder = builder.Add(value);
    ///  
    /// // Discouraged - class-like usage
    /// var builder = new ChiHash();
    /// builder.Add(value);
    /// </code>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(string? value)
    {
        AddString(value ?? "");
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
    ///         Supports standard numeric types, bool, enums, Guid, Complex, DateTime, DateTimeOffset, and TimeSpan.
    ///     </para>
    /// </remarks>
    /// <inheritdoc cref="Add(string)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add<T>(T value)
    {
        if (TryAddPrimitive(value) || TryAddComplex(value))
            return this;

        throw new NotSupportedException(
            $"Type {typeof(T).Name} is not supported. " +
            $"Supported types: all numeric types implementing INumber<T>, string, bool, enums, " +
            $"Guid, Complex, DateTime, DateTimeOffset, TimeSpan.");
    }

    /// <summary>
    ///     Adds a span of values supported by <see cref="Add{T}(T)" /> to the hash calculation.
    /// </summary>
    /// <typeparam name="T">The type of values to add.</typeparam>
    /// <param name="values">The span of values to add to the hash.</param>
    /// <returns>A new ChiHash instance with all values incorporated into the hash calculation.</returns>
    /// <inheritdoc cref="Add(string)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add<T>(scoped ReadOnlySpan<T> values)
    {
        foreach (var value in values)
            this = Add(value);
        return this;
    }

    /// <summary>
    ///     Adds a span of values supported by <see cref="Add{T}(T)" /> to the hash calculation.
    /// </summary>
    /// <typeparam name="T">The type of values to add.</typeparam>
    /// <param name="values">The span of values to add to the hash.</param>
    /// <returns>A new ChiHash instance with all values incorporated into the hash calculation.</returns>
    /// <inheritdoc cref="Add(string)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add<T>(scoped Span<T> values)
    {
        foreach (var value in values)
            this = Add(value);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddString(string value)
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

            var intCount = byteSpan.Length & ~3;
            var intSpan = MemoryMarshal.Cast<byte, int>(byteSpan[..intCount]);

            if (!BitConverter.IsLittleEndian)
                for (var index = 0; index < intSpan.Length; index++)
                    intSpan[index] = BinaryPrimitives.ReverseEndianness(intSpan[index]);

            foreach (var chunk in intSpan)
                Hash = Chi32.UpdateHashValue(Hash, chunk);

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

        return;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Span<byte> RentArray(int byteCount, out byte[] rentedArray)
        {
            rentedArray = ArrayPool<byte>.Shared.Rent(byteCount);
            return rentedArray.AsSpan(0, byteCount);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryAddPrimitive<T>(T value)
    {
        if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(byte))
        {
            var byteValue = Unsafe.As<T, byte>(ref value);
            Hash = Chi32.UpdateHashValue(Hash, byteValue);
        }
        else if (typeof(T) == typeof(short) || typeof(T) == typeof(ushort) || typeof(T) == typeof(char))
        {
            var shortValue = Unsafe.As<T, ushort>(ref value);
            Hash = Chi32.UpdateHashValue(Hash, shortValue);
        }
        else if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
        {
            var intValue = Unsafe.As<T, uint>(ref value);
            Hash = Chi32.UpdateHashValue(Hash, (int)intValue);
        }
        else if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
        {
            var longValue = Unsafe.As<T, ulong>(ref value);
            Hash = Chi32.UpdateHashValue(Hash, (int)longValue);
            Hash = Chi32.UpdateHashValue(Hash, (int)(longValue >> 32));
        }
        else if (typeof(T) == typeof(Int128) || typeof(T) == typeof(UInt128))
        {
            var int128Value = Unsafe.As<T, UInt128>(ref value);
            Hash = Chi32.UpdateHashValue(Hash, (int)int128Value);
            Hash = Chi32.UpdateHashValue(Hash, (int)(int128Value >> 32));
            Hash = Chi32.UpdateHashValue(Hash, (int)(int128Value >> 64));
            Hash = Chi32.UpdateHashValue(Hash, (int)(int128Value >> 96));
        }
        else
        {
            return false;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryAddComplex<T>(T value)
    {
        if (typeof(T) == typeof(double))
        {
            var dv = Unsafe.As<T, double>(ref value);
            var bits = double.IsNaN(dv) ? 0x7FF8_0000_0000_0000UL :
                double.IsPositiveInfinity(dv) ? 0x7FF0_0000_0000_0000UL :
                double.IsNegativeInfinity(dv) ? 0xFFF0_0000_0000_0000UL :
                dv == 0d ? double.IsPositive(dv) ? 0x0000_0000_0000_0000UL : 0x8000_0000_0000_0000UL :
                BitConverter.DoubleToUInt64Bits(dv);
            AddPrimitive(bits);
        }
        else if (typeof(T) == typeof(float))
        {
            var fv = Unsafe.As<T, float>(ref value);
            var bits = float.IsNaN(fv) ? 0x7FC0_0000U :
                float.IsPositiveInfinity(fv) ? 0x7F80_0000U :
                float.IsNegativeInfinity(fv) ? 0xFF80_0000U :
                fv == 0f ? float.IsPositive(fv) ? 0x0000_0000U : 0x8000_0000U :
                BitConverter.SingleToUInt32Bits(fv);
            AddPrimitive(bits);
        }
        else if (typeof(T) == typeof(Half))
        {
            var hv = Unsafe.As<T, Half>(ref value);
            var bits = Half.IsNaN(hv) ? (ushort)0x7E00 :
                Half.IsPositiveInfinity(hv) ? (ushort)0x7C00 :
                Half.IsNegativeInfinity(hv) ? (ushort)0xFC00 :
                hv == (Half)0f ? Half.IsPositive(hv) ? (ushort)0x0000 : (ushort)0x8000 :
                BitConverter.HalfToUInt16Bits(hv);
            AddPrimitive(bits);
        }
        else if (typeof(T) == typeof(bool))
        {
            var boolValue = Unsafe.As<T, bool>(ref value);
            AddPrimitive(boolValue ? 1 : 0);
        }
        else if (typeof(T) == typeof(decimal))
        {
            var decimalValue = Unsafe.As<T, decimal>(ref value);
            Span<int> parts = stackalloc int[4];
            decimal.TryGetBits(decimalValue, parts, out _);
            foreach (var part in parts)
                AddPrimitive(part);
        }
        else if (typeof(T).IsEnum)
        {
            var underlyingType = Enum.GetUnderlyingType(typeof(T));
            if (underlyingType == typeof(byte))
                AddPrimitive(Unsafe.As<T, byte>(ref value));
            else if (underlyingType == typeof(sbyte))
                AddPrimitive(Unsafe.As<T, sbyte>(ref value));
            else if (underlyingType == typeof(short))
                AddPrimitive(Unsafe.As<T, short>(ref value));
            else if (underlyingType == typeof(ushort))
                AddPrimitive(Unsafe.As<T, ushort>(ref value));
            else if (underlyingType == typeof(int))
                AddPrimitive(Unsafe.As<T, int>(ref value));
            else if (underlyingType == typeof(uint))
                AddPrimitive(Unsafe.As<T, uint>(ref value));
            else if (underlyingType == typeof(long))
                AddPrimitive(Unsafe.As<T, long>(ref value));
            else if (underlyingType == typeof(ulong))
                AddPrimitive(Unsafe.As<T, ulong>(ref value));
            else if (underlyingType == typeof(Int128))
                AddPrimitive(Unsafe.As<T, Int128>(ref value));
            else if (underlyingType == typeof(UInt128))
                AddPrimitive(Unsafe.As<T, UInt128>(ref value));
            else
                throw new NotSupportedException(
                    $"Enum type {typeof(T).Name} is not supported. " +
                    $"Supported enum types: byte, sbyte, short, ushort, int, uint, long, ulong, Int128, UInt128.");
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
                AddPrimitive(chunk);
        }
        else if (typeof(T) == typeof(Complex))
        {
            var complex = Unsafe.As<T, Complex>(ref value);
            var realBits = BitConverter.DoubleToUInt64Bits(complex.Real);
            var imagBits = BitConverter.DoubleToUInt64Bits(complex.Imaginary);
            AddPrimitive(realBits);
            AddPrimitive(imagBits);
        }
        else if (typeof(T) == typeof(DateTime))
        {
            var dateTime = Unsafe.As<T, DateTime>(ref value);
            AddPrimitive(dateTime.ToBinary());
        }
        else if (typeof(T) == typeof(DateTimeOffset))
        {
            var dateTimeOffset = Unsafe.As<T, DateTimeOffset>(ref value);
            AddPrimitive(dateTimeOffset.Ticks);
            AddPrimitive(dateTimeOffset.Offset.Ticks);
        }
        else if (typeof(T) == typeof(TimeSpan))
        {
            var timeSpan = Unsafe.As<T, TimeSpan>(ref value);
            AddPrimitive(timeSpan.Ticks);
        }
        else
        {
            return false;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddPrimitive<T>(T value)
    {
#if DEBUG
        var added =
#endif
        TryAddPrimitive(value);

#if DEBUG
        if (!added)
            ShouldNeverHappen();
#endif
        return;

#pragma warning disable CS8321 // Local function is declared but never used
        static void ShouldNeverHappen()
        {
            throw new UnreachableException();
        }
#pragma warning restore CS8321 // Local function is declared but never used
    }
}