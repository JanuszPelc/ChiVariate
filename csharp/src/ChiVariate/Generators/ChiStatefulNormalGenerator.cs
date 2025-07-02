using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChiVariate.Generators;

/// <summary>
///     An efficient sampler for standard normal variables that uses a standby value.
/// </summary>
/// <remarks>
///     Since the Marsaglia polar method generates two independent normal variables at once,
///     this struct uses one immediately and holds the other in "standby" for the next call.
///     This amortizes the cost of generation over two calls.
/// </remarks>
public ref struct ChiStatefulNormalGenerator<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private readonly ref TRng _rng;
    private T _standby;
    private bool _hasValue;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ChiStatefulNormalGenerator{TRng,T}" />.
    /// </summary>
    /// <param name="rng">A reference to the random number generator to use.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiStatefulNormalGenerator(ref TRng rng)
    {
        _rng = ref rng;
        _standby = default!;
        _hasValue = false;
    }

    /// <summary>
    ///     Gets the next standard normal variable, either from the standby value or by generating a new pair.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Next()
    {
        if (_hasValue)
        {
            _hasValue = false;
            return _standby;
        }

        var (z1, z2) = ChiNormalGenerator<T>.NextStandardNormalPair(ref _rng);

        _standby = z2;
        _hasValue = true;

        return z1;
    }
}