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

/*

// * Summary *

BenchmarkDotNet v0.14.0, macOS Sequoia 15.7.3 (24G419) [Darwin 24.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD
  Job-MAIOCM : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD

MinIterationTime=500ms  IterationCount=15

| Method                          | Mean      | Error     | StdDev    | Allocated |
|-------------------------------- |----------:|----------:|----------:|----------:|
| DoubleToDecimal                 |  4.195 us | 0.0074 us | 0.0070 us |         - |
| DecimalToDouble                 |  1.113 us | 0.0007 us | 0.0006 us |         - |
|                                 |           |           |           |           |
| DecimalToFixed_Cast             |  6.222 us | 0.0558 us | 0.0522 us |         - |
| DecimalToFixed_CreateSaturating |  6.279 us | 0.0532 us | 0.0497 us |         - |
| FixedToDecimal_Cast             |  4.048 us | 0.0194 us | 0.0182 us |         - |
| FixedToDecimal_CreateSaturating |  4.034 us | 0.0225 us | 0.0211 us |         - |
|                                 |           |           |           |           |
| DoubleToFixed_Cast              |  2.391 us | 0.0019 us | 0.0018 us |         - |
| DoubleToFixed_CreateSaturating  |  2.400 us | 0.0174 us | 0.0145 us |         - |
| FixedToDouble_Cast              |  2.354 us | 0.0020 us | 0.0015 us |         - |
| FixedToDouble_CreateSaturating  |  2.352 us | 0.0013 us | 0.0012 us |         - |
| DoubleToFixed_ViaDecimal        | 11.162 us | 0.0187 us | 0.0156 us |         - |
| FixedToDouble_ViaDecimal        |  4.220 us | 0.0087 us | 0.0077 us |         - |
|                                 |           |           |           |           |
| FloatToFixed_Cast               |  2.431 us | 0.0014 us | 0.0012 us |         - |
| FloatToFixed_CreateSaturating   |  2.431 us | 0.0015 us | 0.0013 us |         - |
| FixedToFloat_Cast               |  2.391 us | 0.0017 us | 0.0015 us |         - |
| FixedToFloat_CreateSaturating   |  2.391 us | 0.0020 us | 0.0018 us |         - |
|                                 |           |           |           |           |
| IntToFixed_Cast                 |  1.727 us | 0.0010 us | 0.0008 us |         - |
| IntToFixed_CreateSaturating     |  1.730 us | 0.0007 us | 0.0007 us |         - |
| FixedToInt_Cast                 |  2.064 us | 0.0066 us | 0.0058 us |         - |
| FixedToInt_CreateSaturating     |  1.769 us | 0.0152 us | 0.0135 us |         - |
|                                 |           |           |           |           |
| LongToFixed_Cast                |  1.333 us | 0.0007 us | 0.0006 us |         - |
| LongToFixed_CreateSaturating    |  1.334 us | 0.0010 us | 0.0009 us |         - |
| FixedToLong_Cast                |  1.590 us | 0.0110 us | 0.0098 us |         - |
| FixedToLong_CreateSaturating    |  1.575 us | 0.0110 us | 0.0098 us |         - |
|                                 |           |           |           |           |
| UIntToFixed_Cast                |  1.738 us | 0.0011 us | 0.0010 us |         - |
| UIntToFixed_CreateSaturating    |  1.752 us | 0.0087 us | 0.0073 us |         - |
| FixedToUInt_Cast                |  1.683 us | 0.0026 us | 0.0024 us |         - |
| FixedToUInt_CreateSaturating    |  1.738 us | 0.0027 us | 0.0025 us |         - |
|                                 |           |           |           |           |
| ULongToFixed_Cast               |  1.233 us | 0.0022 us | 0.0020 us |         - |
| ULongToFixed_CreateSaturating   |  1.275 us | 0.0016 us | 0.0015 us |         - |
| FixedToULong_Cast               |  1.715 us | 0.0035 us | 0.0027 us |         - |
| FixedToULong_CreateSaturating   |  1.714 us | 0.0023 us | 0.0018 us |         - |

// * Hints *
Outliers
  FixedConversionBenchmarks.FixedToDecimal_Cast: MinIterationTime=500ms, IterationCount=15            -> 1 outlier  was  detected (4.01 us)
  FixedConversionBenchmarks.DoubleToFixed_CreateSaturating: MinIterationTime=500ms, IterationCount=15 -> 2 outliers were removed (2.47 us, 2.48 us)
  FixedConversionBenchmarks.FixedToDouble_Cast: MinIterationTime=500ms, IterationCount=15             -> 3 outliers were removed (2.36 us..2.37 us)
  FixedConversionBenchmarks.DoubleToFixed_ViaDecimal: MinIterationTime=500ms, IterationCount=15       -> 2 outliers were removed (11.25 us, 11.26 us)
  FixedConversionBenchmarks.FixedToDouble_ViaDecimal: MinIterationTime=500ms, IterationCount=15       -> 1 outlier  was  removed (4.27 us)
  FixedConversionBenchmarks.FloatToFixed_Cast: MinIterationTime=500ms, IterationCount=15              -> 2 outliers were removed (2.44 us, 2.44 us)
  FixedConversionBenchmarks.FloatToFixed_CreateSaturating: MinIterationTime=500ms, IterationCount=15  -> 1 outlier  was  removed (2.44 us)
  FixedConversionBenchmarks.FixedToFloat_Cast: MinIterationTime=500ms, IterationCount=15              -> 1 outlier  was  removed (2.40 us)
  FixedConversionBenchmarks.FixedToFloat_CreateSaturating: MinIterationTime=500ms, IterationCount=15  -> 1 outlier  was  removed (2.43 us)
  FixedConversionBenchmarks.IntToFixed_Cast: MinIterationTime=500ms, IterationCount=15                -> 3 outliers were removed (1.73 us..1.75 us)
  FixedConversionBenchmarks.IntToFixed_CreateSaturating: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  removed (1.73 us)
  FixedConversionBenchmarks.FixedToInt_Cast: MinIterationTime=500ms, IterationCount=15                -> 1 outlier  was  removed (2.08 us)
  FixedConversionBenchmarks.FixedToInt_CreateSaturating: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  removed (1.84 us)
  FixedConversionBenchmarks.LongToFixed_Cast: MinIterationTime=500ms, IterationCount=15               -> 1 outlier  was  removed (1.34 us)
  FixedConversionBenchmarks.FixedToLong_Cast: MinIterationTime=500ms, IterationCount=15               -> 1 outlier  was  removed (1.62 us)
  FixedConversionBenchmarks.FixedToLong_CreateSaturating: MinIterationTime=500ms, IterationCount=15   -> 1 outlier  was  removed (1.61 us)
  FixedConversionBenchmarks.UIntToFixed_CreateSaturating: MinIterationTime=500ms, IterationCount=15   -> 2 outliers were removed (1.77 us, 1.78 us)
  FixedConversionBenchmarks.FixedToULong_Cast: MinIterationTime=500ms, IterationCount=15              -> 3 outliers were removed (1.74 us..1.74 us)
  FixedConversionBenchmarks.FixedToULong_CreateSaturating: MinIterationTime=500ms, IterationCount=15  -> 3 outliers were removed (1.76 us..1.82 us)

// * Legends *
  Mean      : Arithmetic mean of all measurements
  Error     : Half of 99.9% confidence interval
  StdDev    : Standard deviation of all measurements
  Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 us      : 1 Microsecond (0.000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
Run time: 00:10:25 (625.22 sec), executed benchmarks: 32

Global total time: 00:10:31 (631.49 sec), executed benchmarks: 32
// * Artifacts cleanup *
Artifacts cleanup is finished

Process finished with exit code 0.

*/