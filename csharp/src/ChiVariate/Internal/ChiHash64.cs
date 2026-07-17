// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ChiVariate.Internal;

/// <summary>
///     Internal static class containing the core 64-bit hashing algorithms.
/// </summary>
internal static class ChiHash64
{
    /// <summary>
    ///     Gets the initial prime value for starting a new hash operation.
    /// </summary>
    public const long InitialValue = 0x46A74A57896EA3C9;

    /// <summary>
    ///     Computes a hash for a value of any supported type.
    /// </summary>
    /// <typeparam name="T">The type of value to hash.</typeparam>
    /// <param name="value">The value to hash.</param>
    /// <param name="hash">The current hash value to update.</param>
    /// <returns>The updated hash value.</returns>
    /// <exception cref="NotSupportedException">Thrown when the type T is not supported.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long HashValue<T>(T value, long hash)
    {
        if (TryHashPrimitive(value, ref hash) || TryHashComplex(value, ref hash) || TryHashString(value, ref hash))
            return hash;

        throw new NotSupportedException(
            $"Type {typeof(T).Name} is not supported. " +
            $"Supported types: all numeric types implementing INumber<T>, string, bool, enums, " +
            $"Guid, Complex, DateTime, DateTimeOffset, TimeSpan.");
    }

    /// <summary>
    ///     Updates the hash state with a new 64-bit integer value.
    /// </summary>
    /// <param name="hash">The current hash value.</param>
    /// <param name="value">The 64-bit integer value to incorporate.</param>
    /// <returns>The updated hash value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long UpdateHashValue(long hash, long value)
    {
        const ulong multiplierPrime = 0x8A2AB4D322468F2D;
        const ulong bitmaskPrime = 0xFC2ED86788EEAD7F;

        var combinedHash = (ulong)(hash ^ value) * multiplierPrime;
        var offset = (int)combinedHash & 63;

        return hash ^ (long)BitOperations.RotateRight(combinedHash ^ bitmaskPrime, offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryHashPrimitive<T>(T value, ref long hash, bool failIfUnsupported = false)
    {
        if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(byte))
        {
            var byteValue = Unsafe.As<T, byte>(ref value);
            hash = UpdateHashValue(hash, byteValue);
        }
        else if (typeof(T) == typeof(short) || typeof(T) == typeof(ushort) || typeof(T) == typeof(char))
        {
            var shortValue = Unsafe.As<T, ushort>(ref value);
            hash = UpdateHashValue(hash, shortValue);
        }
        else if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
        {
            var intValue = Unsafe.As<T, uint>(ref value);
            hash = UpdateHashValue(hash, intValue);
        }
        else if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
        {
            var longValue = Unsafe.As<T, ulong>(ref value);
            hash = UpdateHashValue(hash, (long)longValue);
        }
        else if (typeof(T) == typeof(Int128) || typeof(T) == typeof(UInt128))
        {
            var int128Value = Unsafe.As<T, UInt128>(ref value);
            hash = UpdateHashValue(hash, (long)int128Value);
            hash = UpdateHashValue(hash, (long)(int128Value >> 64));
        }
        else
        {
            return failIfUnsupported
                ? throw new UnreachableException()
                : false;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryHashComplex<T>(T value, ref long hash)
    {
        if (typeof(T) == typeof(double))
        {
            var dv = Unsafe.As<T, double>(ref value);
            if (double.IsNaN(dv))
            {
                const ulong canonicalQNaN = 0x7FF8_0000_0000_0000UL;
                TryHashPrimitive(canonicalQNaN, ref hash, true);
            }
            else
            {
                if (dv == 0d) dv = +0d;
                TryHashPrimitive(BitConverter.DoubleToUInt64Bits(dv), ref hash, true);
            }
        }
        else if (typeof(T) == typeof(float))
        {
            var fv = Unsafe.As<T, float>(ref value);
            if (float.IsNaN(fv))
            {
                const uint canonicalQNaN = 0x7FC0_0000U;
                TryHashPrimitive(canonicalQNaN, ref hash, true);
            }
            else
            {
                if (fv == 0f) fv = +0f;
                TryHashPrimitive(BitConverter.SingleToUInt32Bits(fv), ref hash, true);
            }
        }
        else if (typeof(T) == typeof(Half))
        {
            var hv = Unsafe.As<T, Half>(ref value);
            if (Half.IsNaN(hv))
            {
                const ushort canonicalQNaN = 0x7E00;
                TryHashPrimitive(canonicalQNaN, ref hash, true);
            }
            else
            {
                if (hv == (Half)0f) hv = +(Half)0f;
                TryHashPrimitive(BitConverter.HalfToUInt16Bits(hv), ref hash, true);
            }
        }
        else if (typeof(T) == typeof(bool))
        {
            var boolValue = Unsafe.As<T, bool>(ref value);
            TryHashPrimitive(boolValue ? 1 : 0, ref hash, true);
        }
        else if (typeof(T) == typeof(decimal))
        {
            var decimalValue = Unsafe.As<T, decimal>(ref value);
            Span<int> parts = stackalloc int[4];
            decimal.TryGetBits(decimalValue, parts, out _);
            foreach (var part in parts)
                TryHashPrimitive(part, ref hash, true);
        }
        else if (typeof(T).IsEnum)
        {
            var underlyingType = Enum.GetUnderlyingType(typeof(T));
            if (underlyingType == typeof(byte))
                TryHashPrimitive(Unsafe.As<T, byte>(ref value), ref hash, true);
            else if (underlyingType == typeof(sbyte))
                TryHashPrimitive(Unsafe.As<T, sbyte>(ref value), ref hash, true);
            else if (underlyingType == typeof(short))
                TryHashPrimitive(Unsafe.As<T, short>(ref value), ref hash, true);
            else if (underlyingType == typeof(ushort))
                TryHashPrimitive(Unsafe.As<T, ushort>(ref value), ref hash, true);
            else if (underlyingType == typeof(int))
                TryHashPrimitive(Unsafe.As<T, int>(ref value), ref hash, true);
            else if (underlyingType == typeof(uint))
                TryHashPrimitive(Unsafe.As<T, uint>(ref value), ref hash, true);
            else if (underlyingType == typeof(long))
                TryHashPrimitive(Unsafe.As<T, long>(ref value), ref hash, true);
            else if (underlyingType == typeof(ulong))
                TryHashPrimitive(Unsafe.As<T, ulong>(ref value), ref hash, true);
            else if (underlyingType == typeof(Int128))
                TryHashPrimitive(Unsafe.As<T, Int128>(ref value), ref hash, true);
            else if (underlyingType == typeof(UInt128))
                TryHashPrimitive(Unsafe.As<T, UInt128>(ref value), ref hash, true);
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

            var longSpan = MemoryMarshal.Cast<byte, long>(bytes);
            if (!BitConverter.IsLittleEndian)
                for (var index = 0; index < longSpan.Length; index++)
                    longSpan[index] = BinaryPrimitives.ReverseEndianness(longSpan[index]);

            foreach (var chunk in longSpan)
                TryHashPrimitive(chunk, ref hash, true);
        }
        else if (typeof(T) == typeof(Complex))
        {
            var complex = Unsafe.As<T, Complex>(ref value);
            var realBits = BitConverter.DoubleToUInt64Bits(complex.Real);
            var imagBits = BitConverter.DoubleToUInt64Bits(complex.Imaginary);
            TryHashPrimitive(realBits, ref hash, true);
            TryHashPrimitive(imagBits, ref hash, true);
        }
        else if (typeof(T) == typeof(DateTime))
        {
            var dateTime = Unsafe.As<T, DateTime>(ref value);
            TryHashPrimitive(dateTime.ToBinary(), ref hash, true);
        }
        else if (typeof(T) == typeof(DateTimeOffset))
        {
            var dateTimeOffset = Unsafe.As<T, DateTimeOffset>(ref value);
            TryHashPrimitive(dateTimeOffset.Ticks, ref hash, true);
            TryHashPrimitive(dateTimeOffset.Offset.Ticks, ref hash, true);
        }
        else if (typeof(T) == typeof(TimeSpan))
        {
            var timeSpan = Unsafe.As<T, TimeSpan>(ref value);
            TryHashPrimitive(timeSpan.Ticks, ref hash, true);
        }
        else
        {
            return false;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryHashString<T>(T value, ref long hash)
    {
        if (typeof(T) != typeof(string))
            return false;

        var stringValue = Unsafe.As<T, string?>(ref value) ?? "";
        var currentHash = hash;
        const int maxStackAllocSize = 512;

        var byteCount = Encoding.UTF8.GetByteCount(stringValue);
        byte[]? rentedArray = null;
        var byteSpan = byteCount <= maxStackAllocSize
            ? stackalloc byte[byteCount]
            : RentArray(byteCount, out rentedArray);

        try
        {
            Encoding.UTF8.GetBytes(stringValue, byteSpan);

            var longCount = byteSpan.Length & ~7;
            var longSpan = MemoryMarshal.Cast<byte, long>(byteSpan[..longCount]);

            if (!BitConverter.IsLittleEndian)
                for (var index = 0; index < longSpan.Length; index++)
                    longSpan[index] = BinaryPrimitives.ReverseEndianness(longSpan[index]);

            foreach (var chunk in longSpan)
                currentHash = UpdateHashValue(currentHash, chunk);

            var orphanCount = byteSpan.Length & 7;
            if (orphanCount > 0)
            {
                var tailBytes = byteSpan[^orphanCount..];
                var lastChunk = 0L;
                for (var i = 0; i < orphanCount; i++)
                    lastChunk |= (long)tailBytes[i] << (i * 8);
                currentHash = UpdateHashValue(currentHash, lastChunk);
            }
        }
        finally
        {
            if (rentedArray != null) ArrayPool<byte>.Shared.Return(rentedArray);
        }

        hash = currentHash;
        return true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Span<byte> RentArray(int byteCount, out byte[] rentedArray)
        {
            rentedArray = ArrayPool<byte>.Shared.Rent(byteCount);
            return rentedArray.AsSpan(0, byteCount);
        }
    }
}