using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Benchmarks;

[MemoryDiagnoser]
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