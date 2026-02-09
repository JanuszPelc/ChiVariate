using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Benchmarks;

/// <summary>
///     Benchmarks for ChiFixed conversion operators and Create* methods.
///     Naming: {Source}To{Target}_{Method}
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
[MinIterationTime(500)]
[IterationCount(15)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class FixedConversionBenchmarks
{
    private const int Ops = 1_000;
    private decimal[] _decimalValues = null!;
    private double[] _doubleValues = null!;

    private ChiFixed[] _fixedValues = null!;
    private float[] _floatValues = null!;
    private int[] _intValues = null!;
    private long[] _longValues = null!;
    private uint[] _uintValues = null!;
    private ulong[] _ulongValues = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);

        _fixedValues = new ChiFixed[Ops];
        _doubleValues = new double[Ops];
        _floatValues = new float[Ops];
        _intValues = new int[Ops];
        _uintValues = new uint[Ops];
        _longValues = new long[Ops];
        _ulongValues = new ulong[Ops];
        _decimalValues = new decimal[Ops];

        for (var i = 0; i < Ops; i++)
        {
            var v = random.NextDouble() * 100 - 50;

            _doubleValues[i] = v;
            _floatValues[i] = (float)v;
            _intValues[i] = (int)v;
            _uintValues[i] = (uint)Math.Abs((int)v);
            _longValues[i] = (long)v;
            _ulongValues[i] = (ulong)Math.Abs((int)v);
            _decimalValues[i] = (decimal)v;
            _fixedValues[i] = (ChiFixed)v;
        }
    }

    #region Double

    [Benchmark]
    [BenchmarkCategory("Double")]
    public ChiFixed DoubleToFixed_Cast()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = (ChiFixed)_doubleValues[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Double")]
    public ChiFixed DoubleToFixed_CreateSaturating()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.CreateSaturating(_doubleValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Double")]
    public double FixedToDouble_Cast()
    {
        var result = 0.0;
        for (var i = 0; i < Ops; i++)
            result = (double)_fixedValues[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Double")]
    public double FixedToDouble_CreateSaturating()
    {
        var result = 0.0;
        for (var i = 0; i < Ops; i++)
            result = double.CreateSaturating(_fixedValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Double")]
    public ChiFixed DoubleToFixed_ViaDecimal()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = (ChiFixed)(decimal)_doubleValues[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Double")]
    public double FixedToDouble_ViaDecimal()
    {
        var result = 0.0;
        for (var i = 0; i < Ops; i++)
            result = (double)(decimal)_fixedValues[i];
        return result;
    }

    #endregion

    #region Float

    [Benchmark]
    [BenchmarkCategory("Float")]
    public ChiFixed FloatToFixed_Cast()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = (ChiFixed)_floatValues[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Float")]
    public ChiFixed FloatToFixed_CreateSaturating()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.CreateSaturating(_floatValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Float")]
    public float FixedToFloat_Cast()
    {
        var result = 0f;
        for (var i = 0; i < Ops; i++)
            result = (float)_fixedValues[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Float")]
    public float FixedToFloat_CreateSaturating()
    {
        var result = 0f;
        for (var i = 0; i < Ops; i++)
            result = float.CreateSaturating(_fixedValues[i]);
        return result;
    }

    #endregion

    #region Int

    [Benchmark]
    [BenchmarkCategory("Int")]
    public ChiFixed IntToFixed_Cast()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = (ChiFixed)_intValues[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Int")]
    public ChiFixed IntToFixed_CreateSaturating()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.CreateSaturating(_intValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Int")]
    public int FixedToInt_Cast()
    {
        var result = 0;
        for (var i = 0; i < Ops; i++)
            result = (int)_fixedValues[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Int")]
    public int FixedToInt_CreateSaturating()
    {
        var result = 0;
        for (var i = 0; i < Ops; i++)
            result = int.CreateSaturating(_fixedValues[i]);
        return result;
    }

    #endregion

    #region UInt

    [Benchmark]
    [BenchmarkCategory("UInt")]
    public ChiFixed UIntToFixed_Cast()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = (ChiFixed)_uintValues[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("UInt")]
    public ChiFixed UIntToFixed_CreateSaturating()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.CreateSaturating(_uintValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("UInt")]
    public uint FixedToUInt_Cast()
    {
        var result = 0u;
        for (var i = 0; i < Ops; i++)
            result = (uint)_fixedValues[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("UInt")]
    public uint FixedToUInt_CreateSaturating()
    {
        var result = 0u;
        for (var i = 0; i < Ops; i++)
            result = uint.CreateSaturating(_fixedValues[i]);
        return result;
    }

    #endregion

    #region Long

    [Benchmark]
    [BenchmarkCategory("Long")]
    public ChiFixed LongToFixed_Cast()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = (ChiFixed)_longValues[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Long")]
    public ChiFixed LongToFixed_CreateSaturating()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.CreateSaturating(_longValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Long")]
    public long FixedToLong_Cast()
    {
        var result = 0L;
        for (var i = 0; i < Ops; i++)
            result = (long)_fixedValues[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Long")]
    public long FixedToLong_CreateSaturating()
    {
        var result = 0L;
        for (var i = 0; i < Ops; i++)
            result = long.CreateSaturating(_fixedValues[i]);
        return result;
    }

    #endregion

    #region ULong

    [Benchmark]
    [BenchmarkCategory("ULong")]
    public ChiFixed ULongToFixed_Cast()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = (ChiFixed)_ulongValues[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("ULong")]
    public ChiFixed ULongToFixed_CreateSaturating()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.CreateSaturating(_ulongValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("ULong")]
    public ulong FixedToULong_Cast()
    {
        var result = 0UL;
        for (var i = 0; i < Ops; i++)
            result = (ulong)_fixedValues[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("ULong")]
    public ulong FixedToULong_CreateSaturating()
    {
        var result = 0UL;
        for (var i = 0; i < Ops; i++)
            result = ulong.CreateSaturating(_fixedValues[i]);
        return result;
    }

    #endregion

    #region Decimal

    [Benchmark]
    [BenchmarkCategory("Decimal")]
    public ChiFixed DecimalToFixed_Cast()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = (ChiFixed)_decimalValues[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Decimal")]
    public ChiFixed DecimalToFixed_CreateSaturating()
    {
        var result = ChiFixed.Zero;
        for (var i = 0; i < Ops; i++)
            result = ChiFixed.CreateSaturating(_decimalValues[i]);
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Decimal")]
    public decimal FixedToDecimal_Cast()
    {
        var result = 0m;
        for (var i = 0; i < Ops; i++)
            result = (decimal)_fixedValues[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Decimal")]
    public decimal FixedToDecimal_CreateSaturating()
    {
        var result = 0m;
        for (var i = 0; i < Ops; i++)
            result = decimal.CreateSaturating(_fixedValues[i]);
        return result;
    }

    #endregion

    #region Baseline

    [Benchmark]
    [BenchmarkCategory("Baseline")]
    public decimal DoubleToDecimal()
    {
        var result = 0m;
        for (var i = 0; i < Ops; i++)
            result = (decimal)_doubleValues[i];
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Baseline")]
    public double DecimalToDouble()
    {
        var result = 0.0;
        for (var i = 0; i < Ops; i++)
            result = (double)_decimalValues[i];
        return result;
    }

    #endregion
}