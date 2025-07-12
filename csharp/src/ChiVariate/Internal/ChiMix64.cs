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
///     Internal static class containing simplified 64-bit hashing algorithms.
/// </summary>
internal static class ChiMix64
{
    /// <summary>
    ///     Gets the initial prime value for starting a new mix operation.
    /// </summary>
    public const long InitialValue = 0x46A74A57896EA3C9;

    /// <summary>
    ///     Mixes a value of any supported type into the current mix state.
    /// </summary>
    /// <typeparam name="T">The type of value to mix.</typeparam>
    /// <param name="mix">The current mix state to update.</param>
    /// <param name="value">The value to mix.</param>
    /// <returns>The updated mix state.</returns>
    /// <exception cref="NotSupportedException">Thrown when the type T is not supported.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long MixValue<T>(long mix, T value)
    {
        var currentMix = mix;

        if (TryMixPrimitive(value, ref currentMix) || TryMixComplex(value, ref currentMix))
            return currentMix;

        throw new NotSupportedException(
            $"Type {typeof(T).Name} is not supported. " +
            $"Supported types: all numeric types implementing INumber<T>, string, bool, enums, " +
            $"Guid, Complex, DateTime, DateTimeOffset, TimeSpan.");
    }

    /// <summary>
    ///     Mixes a string value into the current mix state.
    /// </summary>
    /// <param name="mix">The current mix state to update.</param>
    /// <param name="value">The string to mix.</param>
    /// <returns>The updated mix state.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long MixString(long mix, string? value)
    {
        value ??= "";
        var currentMix = mix;
        const int maxStackAllocSize = 512;

        var byteCount = Encoding.UTF8.GetByteCount(value);
        byte[]? rentedArray = null;
        var byteSpan = byteCount <= maxStackAllocSize
            ? stackalloc byte[byteCount]
            : RentArray(byteCount, out rentedArray);

        try
        {
            Encoding.UTF8.GetBytes(value, byteSpan);

            var longCount = byteSpan.Length & ~7;
            var longSpan = MemoryMarshal.Cast<byte, long>(byteSpan[..longCount]);

            if (!BitConverter.IsLittleEndian)
                for (var index = 0; index < longSpan.Length; index++)
                    longSpan[index] = BinaryPrimitives.ReverseEndianness(longSpan[index]);

            foreach (var chunk in longSpan)
                currentMix = UpdateMixValue(currentMix, chunk);

            var orphanCount = byteSpan.Length & 7;
            if (orphanCount > 0)
            {
                var tailBytes = byteSpan[^orphanCount..];
                var lastChunk = 0L;
                for (var i = 0; i < orphanCount; i++)
                    lastChunk |= (long)tailBytes[i] << (i * 8);
                currentMix = UpdateMixValue(currentMix, lastChunk);
            }
        }
        finally
        {
            if (rentedArray != null) ArrayPool<byte>.Shared.Return(rentedArray);
        }

        return currentMix;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Span<byte> RentArray(int byteCount, out byte[] rentedArray)
        {
            rentedArray = ArrayPool<byte>.Shared.Rent(byteCount);
            return rentedArray.AsSpan(0, byteCount);
        }
    }

    /// <summary>
    ///     Updates the mix state with a new 64-bit integer value.
    /// </summary>
    /// <param name="mix">The current mix state.</param>
    /// <param name="value">The 64-bit integer value to incorporate.</param>
    /// <returns>The updated mix state.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long UpdateMixValue(long mix, long value)
    {
        const ulong multiplierPrime = 0x8A2AB4D322468F2D;
        const ulong bitmaskPrime = 0xFC2ED86788EEAD7F;

        var combinedHash = (ulong)(mix ^ value) * multiplierPrime;
        var offset = (int)combinedHash & 63;

        return mix ^ (long)BitOperations.RotateRight(combinedHash ^ bitmaskPrime, offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryMixPrimitive<T>(T value, ref long mix, bool failIfUnsupported = false)
    {
        if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(byte))
        {
            var byteValue = Unsafe.As<T, byte>(ref value);
            mix = UpdateMixValue(mix, byteValue);
        }
        else if (typeof(T) == typeof(short) || typeof(T) == typeof(ushort) || typeof(T) == typeof(char))
        {
            var shortValue = Unsafe.As<T, ushort>(ref value);
            mix = UpdateMixValue(mix, shortValue);
        }
        else if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
        {
            var intValue = Unsafe.As<T, uint>(ref value);
            mix = UpdateMixValue(mix, intValue);
        }
        else if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
        {
            var longValue = Unsafe.As<T, ulong>(ref value);
            mix = UpdateMixValue(mix, (long)longValue);
        }
        else if (typeof(T) == typeof(Int128) || typeof(T) == typeof(UInt128))
        {
            var int128Value = Unsafe.As<T, UInt128>(ref value);
            mix = UpdateMixValue(mix, (long)int128Value);
            mix = UpdateMixValue(mix, (long)(int128Value >> 64));
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
    private static bool TryMixComplex<T>(T value, ref long mix)
    {
        if (typeof(T) == typeof(double))
        {
            var dv = Unsafe.As<T, double>(ref value);
            if (double.IsNaN(dv))
            {
                const ulong canonicalQNaN = 0x7FF8_0000_0000_0000UL;
                TryMixPrimitive(canonicalQNaN, ref mix, true);
            }
            else
            {
                if (dv == 0d) dv = +0d;
                TryMixPrimitive(BitConverter.DoubleToUInt64Bits(dv), ref mix, true);
            }
        }
        else if (typeof(T) == typeof(float))
        {
            var fv = Unsafe.As<T, float>(ref value);
            if (float.IsNaN(fv))
            {
                const uint canonicalQNaN = 0x7FC0_0000U;
                TryMixPrimitive(canonicalQNaN, ref mix, true);
            }
            else
            {
                if (fv == 0f) fv = +0f;
                TryMixPrimitive(BitConverter.SingleToUInt32Bits(fv), ref mix, true);
            }
        }
        else if (typeof(T) == typeof(Half))
        {
            var hv = Unsafe.As<T, Half>(ref value);
            if (Half.IsNaN(hv))
            {
                const ushort canonicalQNaN = 0x7E00;
                TryMixPrimitive(canonicalQNaN, ref mix, true);
            }
            else
            {
                if (hv == (Half)0f) hv = +(Half)0f;
                TryMixPrimitive(BitConverter.HalfToUInt16Bits(hv), ref mix, true);
            }
        }
        else if (typeof(T) == typeof(bool))
        {
            var boolValue = Unsafe.As<T, bool>(ref value);
            TryMixPrimitive(boolValue ? 1 : 0, ref mix, true);
        }
        else if (typeof(T) == typeof(decimal))
        {
            var decimalValue = Unsafe.As<T, decimal>(ref value);
            Span<int> parts = stackalloc int[4];
            decimal.TryGetBits(decimalValue, parts, out _);
            foreach (var part in parts)
                TryMixPrimitive(part, ref mix, true);
        }
        else if (typeof(T).IsEnum)
        {
            var underlyingType = Enum.GetUnderlyingType(typeof(T));
            if (underlyingType == typeof(byte))
                TryMixPrimitive(Unsafe.As<T, byte>(ref value), ref mix, true);
            else if (underlyingType == typeof(sbyte))
                TryMixPrimitive(Unsafe.As<T, sbyte>(ref value), ref mix, true);
            else if (underlyingType == typeof(short))
                TryMixPrimitive(Unsafe.As<T, short>(ref value), ref mix, true);
            else if (underlyingType == typeof(ushort))
                TryMixPrimitive(Unsafe.As<T, ushort>(ref value), ref mix, true);
            else if (underlyingType == typeof(int))
                TryMixPrimitive(Unsafe.As<T, int>(ref value), ref mix, true);
            else if (underlyingType == typeof(uint))
                TryMixPrimitive(Unsafe.As<T, uint>(ref value), ref mix, true);
            else if (underlyingType == typeof(long))
                TryMixPrimitive(Unsafe.As<T, long>(ref value), ref mix, true);
            else if (underlyingType == typeof(ulong))
                TryMixPrimitive(Unsafe.As<T, ulong>(ref value), ref mix, true);
            else if (underlyingType == typeof(Int128))
                TryMixPrimitive(Unsafe.As<T, Int128>(ref value), ref mix, true);
            else if (underlyingType == typeof(UInt128))
                TryMixPrimitive(Unsafe.As<T, UInt128>(ref value), ref mix, true);
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
                TryMixPrimitive(chunk, ref mix, true);
        }
        else if (typeof(T) == typeof(Complex))
        {
            var complex = Unsafe.As<T, Complex>(ref value);
            var realBits = BitConverter.DoubleToUInt64Bits(complex.Real);
            var imagBits = BitConverter.DoubleToUInt64Bits(complex.Imaginary);
            TryMixPrimitive(realBits, ref mix, true);
            TryMixPrimitive(imagBits, ref mix, true);
        }
        else if (typeof(T) == typeof(DateTime))
        {
            var dateTime = Unsafe.As<T, DateTime>(ref value);
            TryMixPrimitive(dateTime.ToBinary(), ref mix, true);
        }
        else if (typeof(T) == typeof(DateTimeOffset))
        {
            var dateTimeOffset = Unsafe.As<T, DateTimeOffset>(ref value);
            TryMixPrimitive(dateTimeOffset.Ticks, ref mix, true);
            TryMixPrimitive(dateTimeOffset.Offset.Ticks, ref mix, true);
        }
        else if (typeof(T) == typeof(TimeSpan))
        {
            var timeSpan = Unsafe.As<T, TimeSpan>(ref value);
            TryMixPrimitive(timeSpan.Ticks, ref mix, true);
        }
        else
        {
            return false;
        }

        return true;
    }
}