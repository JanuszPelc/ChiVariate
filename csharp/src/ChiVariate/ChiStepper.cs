// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChiVariate;

/// <summary>
///     Accumulates a fractional rate using integer-only arithmetic (Bresenham algorithm in 1D).
/// </summary>
/// <remarks>
///     <para>
///         ChiStepper implements the core insight of Bresenham's algorithm: stepping at a fractional
///         rate using only integer addition, with zero precision loss in the accumulator.
///     </para>
///     <para>
///         Common use cases include:
///     </para>
///     <list type="bullet">
///         <item>
///             <description>Animation timing (e.g., 24fps content on 60fps display)</description>
///         </item>
///         <item>
///             <description>Audio resampling (e.g., 44100 Hz to 48000 Hz conversion)</description>
///         </item>
///         <item>
///             <description>Tile-based movement at fractional speeds</description>
///         </item>
///         <item>
///             <description>Physics sub-stepping without drift</description>
///         </item>
///     </list>
/// </remarks>
public readonly struct ChiStepper
{
    /// <summary>
    ///     Creates a new stepper with the specified rate (numerator/denominator).
    /// </summary>
    /// <param name="numerator">The amount to add per step.</param>
    /// <param name="denominator">The divisor for calculating whole units.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when denominator is zero.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiStepper(int numerator, int denominator)
        : this((long)numerator, denominator)
    {
    }

    /// <summary>
    ///     Creates a new stepper with the specified rate (numerator/denominator).
    /// </summary>
    /// <param name="numerator">The amount to add per step.</param>
    /// <param name="denominator">The divisor for calculating whole units.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when denominator is zero.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiStepper(long numerator, long denominator)
    {
        if (denominator == 0)
            throw new ArgumentOutOfRangeException(nameof(denominator), "Denominator cannot be zero.");

        Numerator = numerator;
        Denominator = denominator;
        Accumulated = 0;
    }

    private ChiStepper(long numerator, long denominator, long accumulated)
    {
        Numerator = numerator;
        Denominator = denominator;
        Accumulated = accumulated;
    }

    /// <summary>
    ///     Gets the numerator (step size) of the rate.
    /// </summary>
    public long Numerator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    /// <summary>
    ///     Gets the denominator of the rate.
    /// </summary>
    public long Denominator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    /// <summary>
    ///     Gets the raw accumulated value (before division by denominator).
    /// </summary>
    public long Accumulated
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    /// <summary>
    ///     Gets the number of complete whole units accumulated (accumulated / denominator).
    /// </summary>
    public long WholeUnits
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Accumulated / Denominator;
    }

    /// <summary>
    ///     Gets the remainder after extracting whole units (accumulated % denominator).
    /// </summary>
    public long Remainder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Accumulated % Denominator;
    }

    /// <summary>
    ///     Advances by one step (adds numerator to accumulator).
    /// </summary>
    /// <returns>A new ChiStepper with the updated accumulator.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiStepper Step()
    {
        return new ChiStepper(Numerator, Denominator, Accumulated + Numerator);
    }

    /// <summary>
    ///     Advances by the specified number of steps.
    /// </summary>
    /// <param name="count">The number of steps to advance.</param>
    /// <returns>A new ChiStepper with the updated accumulator.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiStepper Step(int count)
    {
        return new ChiStepper(Numerator, Denominator, Accumulated + Numerator * count);
    }

    /// <summary>
    ///     Advances by the specified number of steps.
    /// </summary>
    /// <param name="count">The number of steps to advance.</param>
    /// <returns>A new ChiStepper with the updated accumulator.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiStepper Step(long count)
    {
        return new ChiStepper(Numerator, Denominator, Accumulated + Numerator * count);
    }

    /// <summary>
    ///     Resets the accumulator to zero.
    /// </summary>
    /// <returns>A new ChiStepper with the accumulator reset.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiStepper Reset()
    {
        return new ChiStepper(Numerator, Denominator, 0);
    }

    /// <summary>
    ///     Converts the current accumulated value to the specified numeric type.
    /// </summary>
    /// <typeparam name="T">The target numeric type.</typeparam>
    /// <returns>The accumulated value as type T (accumulated / denominator).</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T AsValue<T>() where T : INumberBase<T>
    {
        return T.CreateChecked(Accumulated) / T.CreateChecked(Denominator);
    }
}