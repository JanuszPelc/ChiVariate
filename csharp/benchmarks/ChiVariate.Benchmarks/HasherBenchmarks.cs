using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Benchmarks;

[MemoryDiagnoser]
[SimpleJob]
[MinIterationTime(500)]
[IterationCount(15)]
public class HasherBenchmarks
{
    private const int IterationCount = 1_000;
    private const string Text = "Apollo 440";
    private const double Value = Math.PI * Math.E * Math.Tau;

    [Benchmark(Description = "HashCode.Hash32", Baseline = true)]
    public bool HashCode_Build()
    {
        var sum = 0;
        for (var i = 0; i < IterationCount; i++)
        {
            var hash = new HashCode();
            hash.Add(Text);
            hash.Add(Value);
            sum += hash.ToHashCode();
        }

        return Consume(sum);
    }

    [Benchmark(Description = "ChiHash.Hash32")]
    public bool ChiHash_Build()
    {
        var sum = 0;
        for (var i = 0; i < IterationCount; i++)
            sum += new ChiHash().Add(Text).Add(Value).Hash;
        return Consume(sum);
    }

    [Benchmark(Description = "ChiSeed.Hash64")]
    public bool ChiSeed_Scramble()
    {
        var sum = 0L;
        for (var i = 0; i < IterationCount; i++)
            sum += ChiSeed.Scramble(Text, Value);
        return Consume(sum);
    }

    [Benchmark(Description = "ChiSeed.Generate64")]
    public bool ChiSeed_GenerateUnique()
    {
        var sum = 0L;
        for (var i = 0; i < IterationCount; i++)
            sum += ChiSeed.GenerateUnique();
        return Consume(sum);
    }

    [Benchmark(Description = "CryptoRng.Generate64")]
    public bool RandomNumberGenerator_FillBytes()
    {
        var sum = 0L;
        Span<byte> buffer = stackalloc byte[8];
        for (var i = 0; i < IterationCount; i++)
        {
            RandomNumberGenerator.Fill(buffer);
            sum += Unsafe.ReadUnaligned<long>(ref buffer[0]);
        }

        return Consume(sum);
    }

    private static bool Consume<T>(T value) where T : INumberBase<T>
    {
        if (value.GetHashCode() == Environment.TickCount)
            Console.WriteLine(value);
        return true;
    }
}

/*

// * Summary *

BenchmarkDotNet v0.14.0, macOS Sequoia 15.7.3 (24G419) [Darwin 24.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD
  Job-BHHXHU : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD

MinIterationTime=500ms  IterationCount=15  

| Method               | Mean     | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
|--------------------- |---------:|---------:|---------:|------:|--------:|----------:|------------:|
| HashCode.Hash32      | 11.81 μs | 0.023 μs | 0.021 μs |  1.00 |    0.00 |         - |          NA |
| ChiHash.Hash32       | 32.17 μs | 0.100 μs | 0.084 μs |  2.72 |    0.01 |         - |          NA |
| ChiSeed.Hash64       | 34.26 μs | 0.255 μs | 0.239 μs |  2.90 |    0.02 |         - |          NA |
| ChiSeed.Generate64   | 55.70 μs | 0.440 μs | 0.412 μs |  4.72 |    0.03 |         - |          NA |
| CryptoRng.Generate64 | 83.12 μs | 0.169 μs | 0.158 μs |  7.04 |    0.02 |         - |          NA |

// * Hints *
Outliers
  HasherBenchmarks.ChiHash.Hash32: MinIterationTime=500ms, IterationCount=15 -> 2 outliers were removed (32.50 μs, 32.55 μs)

// * Legends *
  Mean        : Arithmetic mean of all measurements
  Error       : Half of 99.9% confidence interval
  StdDev      : Standard deviation of all measurements
  Ratio       : Mean of the ratio distribution ([Current]/[Baseline])
  RatioSD     : Standard deviation of the ratio distribution ([Current]/[Baseline])
  Allocated   : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  Alloc Ratio : Allocated memory ratio distribution ([Current]/[Baseline])
  1 μs        : 1 Microsecond (0.000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
Run time: 00:01:26 (86.99 sec), executed benchmarks: 5

*/