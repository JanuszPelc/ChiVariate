using System.IO.Hashing;
using System.Text;
using BenchmarkDotNet.Attributes;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Benchmarks;

[MemoryDiagnoser]
public class HashStringBenchmarks
{
    private const string ShortString = "Hello";
    private const string MediumString = "Hello, World! This is a test string.";

    private const string LongString =
        "This is a significantly longer string designed to test hashing performance on larger inputs, " +
        "involving multiple characters and potentially different distributions of code units or code points " +
        "depending on the hashing method used.";

    private const string UnicodeString =
        "你好世界_こんにちは世界_👍_This contains mixed scripts and supplementary chars.";

    private const int MaxStackAllocSize = 512;

    [GlobalSetup]
    public void GlobalSetup()
    {
    }

    // --- ChiHash ---

    [Benchmark(Description = "ChiHash.Hash(string) [Short]")]
    public int ChiHash_String_Short()
    {
        return new ChiHash().Add(ShortString).Hash;
    }

    [Benchmark(Description = "ChiHash.Hash(string) [Medium]")]
    public int ChiHash_String_Medium()
    {
        return new ChiHash().Add(MediumString).Hash;
    }

    [Benchmark(Description = "ChiHash.Hash(string) [Long]")]
    public int ChiHash_String_Long()
    {
        return new ChiHash().Add(LongString).Hash;
    }

    [Benchmark(Description = "ChiHash.HashUnicode(string) [Unicode]")]
    public int ChiHash_UnicodeString()
    {
        return new ChiHash().Add(UnicodeString).Hash;
    }

    [Benchmark(Description = "HashCode.Combine(string) [Short]")]
    public int HashCodeCombine_String_Short()
    {
        return HashCode.Combine(ShortString);
    }

    [Benchmark(Description = "HashCode.Combine(string) [Medium]")]
    public int HashCodeCombine_String_Medium()
    {
        return HashCode.Combine(MediumString);
    }

    [Benchmark(Description = "HashCode.Combine(string) [Long]")]
    public int HashCodeCombine_String_Long()
    {
        return HashCode.Combine(LongString);
    }

    [Benchmark(Description = "HashCode.Combine(string) [Unicode]")]
    public int HashCodeCombine_UnicodeString()
    {
        return HashCode.Combine(UnicodeString);
    }

    [Benchmark(Description = "XxHash64(bytes) [Short UTF8] (incl conv)")]
    public ulong XxHash64_String_ShortBytes()
    {
        var byteCount = Encoding.UTF8.GetByteCount(ShortString);
        var buffer = byteCount <= MaxStackAllocSize
            ? stackalloc byte[byteCount]
            : new byte[byteCount];
        Encoding.UTF8.GetBytes(ShortString, buffer);
        return XxHash64.HashToUInt64(buffer);
    }

    [Benchmark(Description = "XxHash64(bytes) [Medium UTF8] (incl conv)")]
    public ulong XxHash64_String_MediumBytes()
    {
        var byteCount = Encoding.UTF8.GetByteCount(MediumString);
        var buffer = byteCount <= MaxStackAllocSize
            ? stackalloc byte[byteCount]
            : new byte[byteCount];
        Encoding.UTF8.GetBytes(MediumString, buffer);
        return XxHash64.HashToUInt64(buffer);
    }

    [Benchmark(Description = "XxHash64(bytes) [Long UTF8] (incl conv)")]
    public ulong XxHash64_String_LongBytes()
    {
        var byteCount = Encoding.UTF8.GetByteCount(LongString);
        var buffer = byteCount <= MaxStackAllocSize
            ? stackalloc byte[byteCount]
            : new byte[byteCount];
        Encoding.UTF8.GetBytes(LongString, buffer);
        return XxHash64.HashToUInt64(buffer);
    }

    [Benchmark(Description = "XxHash64(bytes) [Unicode UTF8] (incl conv)")]
    public ulong XxHash64_UnicodeStringBytes()
    {
        var byteCount = Encoding.UTF8.GetByteCount(UnicodeString);
        var buffer = byteCount <= MaxStackAllocSize
            ? stackalloc byte[byteCount]
            : new byte[byteCount];
        Encoding.UTF8.GetBytes(UnicodeString, buffer);
        return XxHash64.HashToUInt64(buffer);
    }

    // --- XxHash3 (Hashing Bytes with UTF8 Conversion) ---

    [Benchmark(Description = "XxHash3(bytes) [Short UTF8] (incl conv)")]
    public ulong XxHash3_String_ShortBytes()
    {
        var byteCount = Encoding.UTF8.GetByteCount(ShortString);
        var buffer = byteCount <= MaxStackAllocSize
            ? stackalloc byte[byteCount]
            : new byte[byteCount];
        Encoding.UTF8.GetBytes(ShortString, buffer);
        return XxHash3.HashToUInt64(buffer);
    }

    [Benchmark(Description = "XxHash3(bytes) [Medium UTF8] (incl conv)")]
    public ulong XxHash3_String_MediumBytes()
    {
        var byteCount = Encoding.UTF8.GetByteCount(MediumString);
        var buffer = byteCount <= MaxStackAllocSize
            ? stackalloc byte[byteCount]
            : new byte[byteCount];
        Encoding.UTF8.GetBytes(MediumString, buffer);
        return XxHash3.HashToUInt64(buffer);
    }

    [Benchmark(Description = "XxHash3(bytes) [Long UTF8] (incl conv)")]
    public ulong XxHash3_String_LongBytes()
    {
        var byteCount = Encoding.UTF8.GetByteCount(LongString);
        var buffer = byteCount <= MaxStackAllocSize
            ? stackalloc byte[byteCount]
            : new byte[byteCount];
        Encoding.UTF8.GetBytes(LongString, buffer);
        return XxHash3.HashToUInt64(buffer);
    }

    [Benchmark(Description = "XxHash3(bytes) [Unicode UTF8] (incl conv)")]
    public ulong XxHash3_UnicodeStringBytes()
    {
        var byteCount = Encoding.UTF8.GetByteCount(UnicodeString);
        var buffer = byteCount <= MaxStackAllocSize
            ? stackalloc byte[byteCount]
            : new byte[byteCount];
        Encoding.UTF8.GetBytes(UnicodeString, buffer);
        return XxHash3.HashToUInt64(buffer);
    }
}