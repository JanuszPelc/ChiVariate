using System.IO.Hashing;
using BenchmarkDotNet.Attributes;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Benchmarks;

[MemoryDiagnoser]
public class HashNumberBenchmarks
{
    private double _doubleValue = Math.PI * Math.E;
    private int _intValue = 123456789;
    private long _longValue = 987654321012345L;

    [GlobalSetup]
    public void GlobalSetup()
    {
    }

    // --- ChiHash ---

    [Benchmark(Description = "ChiHash.Hash(int)")]
    public int ChiHash_Int()
    {
        return ChiHashObsolete.Hash(_intValue++);
    }

    [Benchmark(Description = "ChiHash.Hash<long>(long)")]
    public int ChiHash_Generic_Long()
    {
        return ChiHashObsolete.Hash(_longValue++);
    }

    [Benchmark(Description = "ChiHash.Hash<int, long, double>")]
    public int ChiHash_Generic_Multi()
    {
        return ChiHashObsolete.Hash(_intValue++, _longValue, _doubleValue);
    }

    // --- HashCode.Combine ---

    [Benchmark(Description = "HashCode.Combine(int)")]
    public int HashCodeCombine_Int()
    {
        return HashCode.Combine(_intValue++);
    }

    [Benchmark(Description = "HashCode.Combine(long)")]
    public int HashCodeCombine_Long()
    {
        return HashCode.Combine(_longValue++);
    }

    [Benchmark(Description = "HashCode.Combine<int, long, double>")]
    public int HashCodeCombine_Multi()
    {
        return HashCode.Combine(_intValue++, _longValue, _doubleValue);
    }

    // --- XxHash64 (Hashing Bytes with Conversion) ---

    [Benchmark(Description = "XxHash64(bytes) [int] (incl conv)")]
    public ulong XxHash64_IntBytes()
    {
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        BitConverter.TryWriteBytes(buffer, _intValue++);
        return XxHash64.HashToUInt64(buffer);
    }

    [Benchmark(Description = "XxHash64(bytes) [long] (incl conv)")]
    public ulong XxHash64_LongBytes()
    {
        Span<byte> buffer = stackalloc byte[sizeof(long)];
        BitConverter.TryWriteBytes(buffer, _longValue++);
        return XxHash64.HashToUInt64(buffer);
    }

    [Benchmark(Description = "XxHash64(bytes) [double] (incl conv)")]
    public ulong XxHash64_DoubleBytes()
    {
        Span<byte> buffer = stackalloc byte[sizeof(double)];
        BitConverter.TryWriteBytes(buffer, _doubleValue++);
        return XxHash64.HashToUInt64(buffer);
    }

    // --- XxHash3 (Hashing Bytes with Conversion) ---

    [Benchmark(Description = "XxHash3(bytes) [int] (incl conv)")]
    public ulong XxHash3_IntBytes()
    {
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        BitConverter.TryWriteBytes(buffer, _intValue++);
        return XxHash3.HashToUInt64(buffer);
    }

    [Benchmark(Description = "XxHash3(bytes) [long] (incl conv)")]
    public ulong XxHash3_LongBytes()
    {
        Span<byte> buffer = stackalloc byte[sizeof(long)];
        BitConverter.TryWriteBytes(buffer, _longValue++);
        return XxHash3.HashToUInt64(buffer);
    }

    [Benchmark(Description = "XxHash3(bytes) [double] (incl conv)")]
    public ulong XxHash3_DoubleBytes()
    {
        Span<byte> buffer = stackalloc byte[sizeof(double)];
        BitConverter.TryWriteBytes(buffer, _doubleValue++);
        return XxHash3.HashToUInt64(buffer);
    }
}