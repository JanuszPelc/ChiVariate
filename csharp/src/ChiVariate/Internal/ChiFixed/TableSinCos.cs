// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Runtime.CompilerServices;

namespace ChiVariate.Internal.ChiFixed;

/// <summary>
///     Table-based sine and cosine for ChiFixed fixed-point numbers.
/// </summary>
/// <remarks>
///     Algorithm: Quarter-wave table lookup exploiting trigonometric symmetries.
///     Only sin(0) to sin(π/2) is stored (4096 entries). All quadrants derived by:
///     - Q1 [0, π/2):     sin(θ) = table[θ],           cos(θ) = table[π/2 - θ]
///     - Q2 [π/2, π):     sin(θ) = table[π - θ],       cos(θ) = -table[θ - π/2]
///     - Q3 [π, 3π/2):    sin(θ) = -table[θ - π],      cos(θ) = -table[3π/2 - θ]
///     - Q4 [3π/2, 2π):   sin(θ) = -table[2π - θ],     cos(θ) = table[θ - 3π/2]
///     This uses 4x less memory than storing all four quadrants.
/// </remarks>
internal static class TableSinCos
{
    private const int TableBits = 12; // 4096 entries for quarter wave
    private const int TableSize = 1 << TableBits;

    private static readonly long[] SinTable; // sin(θ) for θ ∈ [0, π/2]
    private static readonly long HalfPi;
    private static readonly long Pi;
    private static readonly long ThreeHalfPi;
    private static readonly long TwoPi;

    static TableSinCos()
    {
        HalfPi = CordicTables.HalfPi;
        Pi = CordicTables.Pi;
        ThreeHalfPi = HalfPi + Pi;
        TwoPi = CordicTables.TwoPi;

        // Precompute sin(θ) for θ = 0, π/8192, 2π/8192, ..., π/2
        SinTable = new long[TableSize + 1];
        for (var i = 0; i <= TableSize; i++)
        {
            var angle = (double)i / TableSize * Math.PI / 2.0;
            SinTable[i] = (long)(Math.Sin(angle) * ChiVariate.ChiFixed.ScaleFactor);
        }
    }

    /// <summary>
    ///     Computes both sin and cos using quarter-wave symmetry.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static (long sin, long cos) SinCos(long angleRaw)
    {
        // Reduce angle to [0, 2π)
        angleRaw %= TwoPi;
        if (angleRaw < 0)
            angleRaw += TwoPi;

        long sin, cos;
        int index;

        // Use quarter-wave symmetries to handle all quadrants
        if (angleRaw < HalfPi)
        {
            // Q1: [0, π/2) - direct lookup
            index = (int)(angleRaw * TableSize / HalfPi);
            if (index > TableSize) index = TableSize;
            sin = SinTable[index];
            cos = SinTable[TableSize - index]; // cos(θ) = sin(π/2 - θ)
        }
        else if (angleRaw < Pi)
        {
            // Q2: [π/2, π) - sin positive, cos negative
            index = (int)((Pi - angleRaw) * TableSize / HalfPi);
            if (index > TableSize) index = TableSize;
            sin = SinTable[index];
            cos = -SinTable[TableSize - index];
        }
        else if (angleRaw < ThreeHalfPi)
        {
            // Q3: [π, 3π/2) - both negative
            index = (int)((angleRaw - Pi) * TableSize / HalfPi);
            if (index > TableSize) index = TableSize;
            sin = -SinTable[index];
            cos = -SinTable[TableSize - index];
        }
        else
        {
            // Q4: [3π/2, 2π) - sin negative, cos positive
            index = (int)((TwoPi - angleRaw) * TableSize / HalfPi);
            if (index > TableSize) index = TableSize;
            sin = -SinTable[index];
            cos = SinTable[TableSize - index];
        }

        return (sin, cos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Sin(long angleRaw)
    {
        var (sin, _) = SinCos(angleRaw);
        return sin;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Cos(long angleRaw)
    {
        var (_, cos) = SinCos(angleRaw);
        return cos;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Tan(long angleRaw)
    {
        var (sin, cos) = SinCos(angleRaw);

        if (cos == 0)
            throw new DivideByZeroException("Tangent is undefined at this value (cos = 0).");

        return FixedMath.Div(sin, cos);
    }
}