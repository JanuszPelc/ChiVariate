using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace ChiVariate.Benchmarks;

internal static class Program
{
    public static void Main(string[] args)
    {
        var config = DefaultConfig.Instance
            .WithArtifactsPath(Path.Combine(AppContext.BaseDirectory, "BenchmarkDotNet.Artifacts"));
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
    }
}