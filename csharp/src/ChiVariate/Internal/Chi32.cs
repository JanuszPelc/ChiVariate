// MIT License
//
// Copyright (c) 2025 Janusz Pelc
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

// Implementation of Cascading Hash Interleave 32-bit (CHI32)
// Documentation and specification: https://github.com/JanuszPelc/chi32

using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChiVariate.Internal;

/// <summary>
///     Implements the core, stateless, deterministic functions of the CHI32 algorithm.
/// </summary>
internal static class Chi32
{
    /// <summary>
    ///     Calculates a 32-bit pseudo-random value based on a specified selector and index.
    /// </summary>
    /// <param name="selector">
    ///     The <see cref="long" /> value serving as a pseudo-random sequence selector.
    /// </param>
    /// <param name="index">
    ///     The <see cref="long" /> value used as an index within this sequence.
    /// </param>
    /// <returns>
    ///     An <see cref="int" /> representing the pseudo-random value derived from the given selector and index.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int DeriveValueAt(long selector, long index)
    {
        unchecked
        {
            var state = (ulong)ApplyCascadingHashInterleave(selector, index);

            var lowSixBits = (int)state;
            var midSixBits = (int)(state >> 29);
            var highSixBits = (int)(state >> 58);
            var offset = (lowSixBits ^ midSixBits ^ highSixBits) & 63;

            return (int)BitOperations.RotateLeft(state, offset);
        }
    }

    /// <summary>
    ///     Calculates a well-mixed 64-bit value from a specified selector and index.
    /// </summary>
    /// <param name="selector">
    ///     The <see cref="long" /> value serving as a pseudo-random sequence selector.
    /// </param>
    /// <param name="index">
    ///     The <see cref="long" /> value used as an index within this sequence.
    /// </param>
    /// <returns>A 64-bit value representing the thoroughly mixed result of the CHI32 algorithm.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ApplyCascadingHashInterleave(long selector, long index)
    {
        unchecked
        {
            const ulong goldenRatioPrimeMultiplier = 0x9E3779B97F4A7C55;

            var primaryAnchor = (ulong)selector;
            var alternateAnchor = (ulong)~selector * goldenRatioPrimeMultiplier;
            var anchorCouplingMask = primaryAnchor & alternateAnchor;

            var primaryOffset = (ulong)index;
            var alternateOffset = (ulong)~index ^ anchorCouplingMask;

            var primaryPointer = primaryAnchor + primaryOffset;
            var alternatePointer = alternateAnchor - alternateOffset;

            var primaryPointerLow = (int)primaryPointer;
            var primaryPointerHigh = (int)(primaryPointer >> 32);

            var alternatePointerLow = (int)alternatePointer;
            var alternatePointerHigh = (int)(alternatePointer >> 32);

            const int interleaveBitOffset = 16;
            const int wrapAroundBitOffset = interleaveBitOffset * 3;

            ulong hashAccumulator = (uint)UpdateHashValue(0, alternatePointerLow);
            hashAccumulator = (uint)UpdateHashValue((int)hashAccumulator, alternatePointerHigh)
                              ^ (hashAccumulator << interleaveBitOffset);
            hashAccumulator = (uint)UpdateHashValue((int)hashAccumulator, primaryPointerHigh)
                              ^ (hashAccumulator << interleaveBitOffset);
            hashAccumulator = (uint)UpdateHashValue((int)hashAccumulator, primaryPointerLow)
                              ^ (hashAccumulator << interleaveBitOffset)
                              ^ (hashAccumulator >> wrapAroundBitOffset);

            const ulong finalStepPrimeMultiplier = 0x72A4EB92D796ED93;

            return (long)(hashAccumulator * finalStepPrimeMultiplier);
        }
    }

    /// <summary>
    ///     Computes a 32-bit hash value from a prior hash and a new input.
    /// </summary>
    /// <param name="previousHash">
    ///     The running <see cref="int" /> hash state. For single-value hashing, use <c>0</c>.
    ///     For sequential updates, initialize to <c>0</c> for the first value, then pass the updated result forward.
    /// </param>
    /// <param name="value">
    ///     The <see cref="int" /> input value to incorporate.
    /// </param>
    /// <returns>
    ///     The resulting <see cref="int" /> hash after combining the input.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int UpdateHashValue(int previousHash, int value)
    {
        unchecked
        {
            const uint primeNumber1 = 0x8addb2d1;
            const uint primeNumber2 = 0x8c723b45;
            const uint primeNumber3 = 0xfd923173;
            const uint primeNumber4 = 0x89a6aa0b;
            const uint primeNumber5 = 0x1f844cb7;
            const uint primeNumber6 = 0xfd2c1e9d;

            const int shiftOffset1 = 15;
            const int shiftOffset2 = 7;
            const int shiftOffset3 = 29;
            const int shiftOffset4 = 16;

            var hash = (uint)previousHash;

            hash ^= primeNumber1;
            hash += primeNumber2 ^ BitOperations.RotateLeft((uint)value, (int)(hash & 31));
            hash *= primeNumber3;

            hash ^= hash >> shiftOffset1;
            hash *= primeNumber4;

            hash ^= hash >> shiftOffset2;
            hash += hash >> shiftOffset3;
            hash *= primeNumber5;

            hash ^= hash >> shiftOffset4;
            hash *= primeNumber6;

            return (int)hash;
        }
    }
}