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
///     Provides static utility methods for generating and manipulating long seed values
///     for pseudo-random number generation.
/// </summary>
/// <remarks>
///     This class uses a more complex and statistically robust mixing algorithm than the general-purpose
///     <see cref="ChiHash" /> utility, making it ideal for creating the high-entropy seeds required
///     by <see cref="ChiRng" />.
/// </remarks>
public static class ChiSeed
{
    /// <summary>
    ///     Generates a reasonably unique seed value.
    /// </summary>
    /// <returns>
    ///     A <see cref="long" /> value representing the generated pseudo-unique seed.
    /// </returns>
    public static long GenerateUnique()
    {
        lock (Global.Lock)
        {
            unchecked
            {
                var timeSeed = Global.TimerTicks;

                Global.SeedCounter += 0x1F844CB7FD2C1EAD;

                return Chi32.ApplyCascadingHashInterleave((long)timeSeed, (long)Global.SeedCounter);
            }
        }
    }

    /// <summary>
    ///     Scrambles a 64-bit value into a reproducible and well-mixed 64-bit form.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="long" /> value to be scrambled.
    /// </param>
    /// <returns>
    ///     A <see cref="long" /> representing a well-mixed value derived from the input value.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Scramble(long value)
    {
        unchecked
        {
            var selector = InitializeCombiner();
            CombineValue(ref selector, (int)(uint)value);
            CombineValue(ref selector, (int)(uint)(value >> 32));

            return Chi32.ApplyCascadingHashInterleave(selector, value);
        }
    }

    /// <summary>
    ///     Transforms a string into a reproducible and well-mixed 64-bit form.
    /// </summary>
    /// <param name="string">
    ///     The <see cref="string" /> value to be scrambled.
    /// </param>
    /// <returns>
    ///     A <see cref="long" /> representing a well-mixed value derived from the input string.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="string" /> is null.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Scramble(string @string)
    {
        ArgumentNullException.ThrowIfNull(@string);

        return Scramble(@string, @string.Length);
    }

    /// <summary>
    ///     Produces a well-mixed 64-bit hash value by combining a scrambled string and a numeric input.
    /// </summary>
    /// <param name="string">The <see cref="string" /> value to be incorporated into the hash calculation.</param>
    /// <param name="number">A numeric value of type <typeparamref name="TNumber" /> to contribute to the hash.</param>
    /// <typeparam name="TNumber">The unmanaged numeric type implementing <see cref="System.Numerics.INumber{T}" />.</typeparam>
    /// <returns>
    ///     A <see cref="long" /> representing a well-mixed value derived from the input string.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="string" /> is null.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Scramble<TNumber>(string @string, TNumber number)
        where TNumber : unmanaged, INumber<TNumber>
    {
        ArgumentNullException.ThrowIfNull(@string);

        var selector = long.CreateTruncating(number);
        var index = HashString(@string);

        return Chi32.ApplyCascadingHashInterleave(selector, index);
    }

    private static long HashString(string @string)
    {
        const int maxStackAllocSize = 512;

        var byteCount = Encoding.UTF8.GetByteCount(@string);
        byte[]? rentedArray = null;
        var byteSpan = byteCount <= maxStackAllocSize
            ? stackalloc byte[byteCount]
            : RentArray(byteCount, out rentedArray);

        var result = InitializeCombiner();

        try
        {
            Encoding.UTF8.GetBytes(@string, byteSpan);

            var intCount = byteSpan.Length & ~3;
            var intSpan = MemoryMarshal.Cast<byte, int>(byteSpan[..intCount]);

            foreach (var int32 in intSpan)
                CombineValue(ref result, int32);

            var orphanCount = byteSpan.Length & 3;
            if (orphanCount > 0)
            {
                var tailBytes = byteSpan[^orphanCount..];
                var last = 0;
                for (var i = 0; i < orphanCount; i++)
                    last |= tailBytes[i] << (i * 8);

                CombineValue(ref result, last);
            }
        }
        finally
        {
            if (byteSpan.Length > maxStackAllocSize)
                ArrayPool<byte>.Shared.Return(rentedArray!);
        }

        return result;

        static Span<byte> RentArray(int byteCount, out byte[] rentedArray)
        {
            rentedArray = ArrayPool<byte>.Shared.Rent(byteCount);
            return rentedArray.AsSpan(0, byteCount);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long InitializeCombiner()
    {
        const ulong initialStateSeed = 0x46A74A57896EA3C9;

        return (long)initialStateSeed;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CombineValue(ref long combinedValue, int value)
    {
        const ulong multiplier = 0x8A2AB4D322468F2D;
        const ulong xorValue = 0xFC2ED86788EEAD7F;

        var combinedHash = (uint)(combinedValue ^ value) * multiplier;
        var offset = (int)combinedHash & 63;

        combinedValue ^= (long)BitOperations.RotateRight(combinedHash ^ xorValue, offset);
    }

    private static class Global
    {
        public static ulong TimerTicks => (ulong)Stopwatch.GetTimestamp();
        public static object Lock { get; } = new();
        public static ulong SeedCounter { get; set; } = ~BinaryPrimitives.ReverseEndianness(TimerTicks);
    }
}