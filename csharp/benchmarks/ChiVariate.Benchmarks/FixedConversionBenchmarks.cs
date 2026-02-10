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
  Job-BHHXHU : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD

MinIterationTime=500ms  IterationCount=15  

| Method                          | Mean     | Error     | StdDev    | Allocated |
|-------------------------------- |---------:|----------:|----------:|----------:|
| DoubleToDecimal                 | 4.173 μs | 0.0165 μs | 0.0146 μs |         - |
| DecimalToDouble                 | 1.256 μs | 0.0026 μs | 0.0025 μs |         - |
|                                 |          |           |           |           |
| DecimalToFixed_Cast             | 6.479 μs | 0.0905 μs | 0.0847 μs |         - |
| DecimalToFixed_CreateSaturating | 6.358 μs | 0.0709 μs | 0.0663 μs |         - |
| FixedToDecimal_Cast             | 4.053 μs | 0.0236 μs | 0.0197 μs |         - |
| FixedToDecimal_CreateSaturating | 3.957 μs | 0.0213 μs | 0.0189 μs |         - |
|                                 |          |           |           |           |
| DoubleToFixed_Cast              | 2.381 μs | 0.0063 μs | 0.0059 μs |         - |
| DoubleToFixed_CreateSaturating  | 2.380 μs | 0.0053 μs | 0.0049 μs |         - |
| FixedToDouble_Cast              | 2.341 μs | 0.0050 μs | 0.0047 μs |         - |
| FixedToDouble_CreateSaturating  | 2.361 μs | 0.0167 μs | 0.0156 μs |         - |
|                                 |          |           |           |           |
| FloatToFixed_Cast               | 2.420 μs | 0.0056 μs | 0.0052 μs |         - |
| FloatToFixed_CreateSaturating   | 2.421 μs | 0.0050 μs | 0.0047 μs |         - |
| FixedToFloat_Cast               | 2.381 μs | 0.0055 μs | 0.0052 μs |         - |
| FixedToFloat_CreateSaturating   | 2.380 μs | 0.0051 μs | 0.0048 μs |         - |
|                                 |          |           |           |           |
| IntToFixed_Cast                 | 1.748 μs | 0.0038 μs | 0.0034 μs |         - |
| IntToFixed_CreateSaturating     | 1.746 μs | 0.0031 μs | 0.0029 μs |         - |
| FixedToInt_Cast                 | 2.133 μs | 0.0286 μs | 0.0267 μs |         - |
| FixedToInt_CreateSaturating     | 1.779 μs | 0.0267 μs | 0.0250 μs |         - |
|                                 |          |           |           |           |
| LongToFixed_Cast                | 1.328 μs | 0.0029 μs | 0.0026 μs |         - |
| LongToFixed_CreateSaturating    | 1.346 μs | 0.0041 μs | 0.0036 μs |         - |
| FixedToLong_Cast                | 1.704 μs | 0.0374 μs | 0.0350 μs |         - |
| FixedToLong_CreateSaturating    | 1.653 μs | 0.0212 μs | 0.0198 μs |         - |
|                                 |          |           |           |           |
| UIntToFixed_Cast                | 1.731 μs | 0.0016 μs | 0.0015 μs |         - |
| UIntToFixed_CreateSaturating    | 1.794 μs | 0.0153 μs | 0.0143 μs |         - |
| FixedToUInt_Cast                | 1.706 μs | 0.0034 μs | 0.0030 μs |         - |
| FixedToUInt_CreateSaturating    | 1.710 μs | 0.0087 μs | 0.0077 μs |         - |
|                                 |          |           |           |           |
| ULongToFixed_Cast               | 1.218 μs | 0.0012 μs | 0.0010 μs |         - |
| ULongToFixed_CreateSaturating   | 1.258 μs | 0.0010 μs | 0.0009 μs |         - |
| FixedToULong_Cast               | 1.680 μs | 0.0100 μs | 0.0094 μs |         - |
| FixedToULong_CreateSaturating   | 1.731 μs | 0.0028 μs | 0.0025 μs |         - |

// * Hints *
Outliers
  FixedConversionBenchmarks.DoubleToDecimal: MinIterationTime=500ms, IterationCount=15                 -> 1 outlier  was  removed (4.22 μs)
  FixedConversionBenchmarks.FixedToDecimal_Cast: MinIterationTime=500ms, IterationCount=15             -> 2 outliers were removed (4.11 μs, 4.12 μs)
  FixedConversionBenchmarks.FixedToDecimal_CreateSaturating: MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  removed (4.02 μs)
  FixedConversionBenchmarks.IntToFixed_Cast: MinIterationTime=500ms, IterationCount=15                 -> 1 outlier  was  removed, 2 outliers were detected (1.74 μs, 1.76 μs)
  FixedConversionBenchmarks.LongToFixed_Cast: MinIterationTime=500ms, IterationCount=15                -> 1 outlier  was  removed (1.34 μs)
  FixedConversionBenchmarks.LongToFixed_CreateSaturating: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  removed (1.56 μs)
  FixedConversionBenchmarks.FixedToLong_Cast: MinIterationTime=500ms, IterationCount=15                -> 1 outlier  was  detected (1.62 μs)
  FixedConversionBenchmarks.FixedToUInt_Cast: MinIterationTime=500ms, IterationCount=15                -> 1 outlier  was  removed (1.72 μs)
  FixedConversionBenchmarks.FixedToUInt_CreateSaturating: MinIterationTime=500ms, IterationCount=15    -> 1 outlier  was  removed (1.78 μs)
  FixedConversionBenchmarks.ULongToFixed_Cast: MinIterationTime=500ms, IterationCount=15               -> 3 outliers were removed (1.24 μs..1.26 μs)
  FixedConversionBenchmarks.ULongToFixed_CreateSaturating: MinIterationTime=500ms, IterationCount=15   -> 1 outlier  was  removed (1.27 μs)
  FixedConversionBenchmarks.FixedToULong_CreateSaturating: MinIterationTime=500ms, IterationCount=15   -> 1 outlier  was  removed (1.74 μs)

// * Legends *
  Mean      : Arithmetic mean of all measurements
  Error     : Half of 99.9% confidence interval
  StdDev    : Standard deviation of all measurements
  Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 μs      : 1 Microsecond (0.000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
Run time: 00:09:32 (572.44 sec), executed benchmarks: 30

*/