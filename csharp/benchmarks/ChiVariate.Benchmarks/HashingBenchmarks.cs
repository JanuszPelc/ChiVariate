using BenchmarkDotNet.Attributes;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Benchmarks;

[MemoryDiagnoser]
public class HashingBenchmarks
{
    private const string Text = "Level 42";
    private double _doubleValue;
    private int _intValue;
    private long _longValue;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _doubleValue = Math.PI * Math.E;
        _intValue = 123456789;
        _longValue = 9876543210;
    }

    [Benchmark(Description = "HashCode.Build", Baseline = true)]
    public int HashCode()
    {
        var hash = new HashCode();
        hash.Add(_intValue++);
        hash.Add(_doubleValue++);
        hash.Add(_longValue++);
        hash.Add(Text);
        return hash.ToHashCode();
    }

    [Benchmark(Description = "ChiHash.Build")]
    public int ChiHash()
    {
        return new ChiHash().Add(_intValue++).Add(_doubleValue++).Add(_longValue++).Add(Text).Hash;
    }

    [Benchmark(Description = "ChiSeed.Scramble")]
    public long ChiSeed_Scramble()
    {
        return ChiSeed.Scramble(Text, _longValue++);
    }

    [Benchmark(Description = "ChiSeed.GenerateUnique")]
    public long ChiSeed_GenerateUnique()
    {
        return ChiSeed.GenerateUnique() ^ _longValue++;
    }
}