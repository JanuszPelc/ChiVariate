using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace ChiVariate.Tests.TestInfrastructure;

#pragma warning disable CS1591

public class Histogram
{
    private readonly double _binWidth;
    private readonly bool _isDiscrete;
    private readonly double _maxBound;
    private readonly double _minBound;

    public readonly int BinCount;
    public readonly long[] Bins;
    public long OutOfBoundsSamples;
    public long TotalSamples;

    public Histogram(double minBound, double maxBound, int binCount, bool isDiscrete = false)
    {
        if (minBound >= maxBound)
            throw new ArgumentException("Min bound must be less than max bound.", nameof(minBound));
        if (binCount <= 0)
            throw new ArgumentException("Bin count must be positive.", nameof(binCount));

        _minBound = minBound;
        _maxBound = maxBound;
        _isDiscrete = isDiscrete;

        BinCount = binCount;
        Bins = new long[binCount];
        _binWidth = (maxBound - minBound) / binCount;
    }

    public void Generate<T, TRng, TSampler>(ref TRng rng, int sampleCount, TSampler sampler)
        where TRng : struct, IChiRngSource<TRng>
        where TSampler : IHistogramSamplerWithRange<T, TRng>
    {
        for (var i = 0; i < sampleCount; i++)
        {
            var nativeSample = sampler.NextSample(ref rng);
            var normalizedSample = sampler.Normalize(nativeSample);
            AddSample(normalizedSample);
        }
    }

    public void AddSample(double value)
    {
        if (value < _minBound || value >= _maxBound)
        {
            OutOfBoundsSamples++;
            return;
        }

        var binIndex = (int)((value - _minBound) / _binWidth);
        binIndex = Math.Clamp(binIndex, 0, BinCount - 1);
        Bins[binIndex]++;
        TotalSamples++;
    }

    public double GetBinWidth()
    {
        return _binWidth;
    }

    public double GetBinCenter(int binIndex)
    {
        if (binIndex < 0 || binIndex >= BinCount)
            throw new ArgumentOutOfRangeException(nameof(binIndex));

        return _minBound + (binIndex + 0.5) * _binWidth;
    }

    public double CalculateMean()
    {
        if (TotalSamples == 0)
            return 0;

        double sum = 0;
        for (var i = 0; i < BinCount; i++)
            sum += GetValueForBin(i) * Bins[i];

        return sum / TotalSamples;
    }

    public double CalculateStdDev(double mean)
    {
        if (TotalSamples <= 1) return 0;

        double sumOfSquares = 0;
        for (var i = 0; i < BinCount; i++)
        {
            var deviation = GetValueForBin(i) - mean;
            sumOfSquares += deviation * deviation * Bins[i];
        }

        return Math.Sqrt(sumOfSquares / (TotalSamples - 1));
    }

    public double CalculateMode()
    {
        if (TotalSamples == 0) return 0;

        long maxCount = 0;
        var peakBinIndex = -1;
        for (var i = 0; i < BinCount; i++)
            if (Bins[i] > maxCount)
            {
                maxCount = Bins[i];
                peakBinIndex = i;
            }

        if (peakBinIndex <= 0 || peakBinIndex >= BinCount - 1)
            return GetBinCenter(peakBinIndex);

        var yMinus1 = (double)Bins[peakBinIndex - 1];
        var y0 = (double)Bins[peakBinIndex];
        var yPlus1 = (double)Bins[peakBinIndex + 1];

        var numerator = yMinus1 - yPlus1;
        var denominator = yMinus1 - 2 * y0 + yPlus1;

        if (Math.Abs(denominator) < 1e-9)
            return GetBinCenter(peakBinIndex);

        var peakOffset = 0.5 * numerator / denominator;
        return GetBinCenter(peakBinIndex) + peakOffset * _binWidth;
    }

    public double CalculateMedian()
    {
        if (TotalSamples == 0) return 0;

        var medianIndex = TotalSamples / 2;
        long cumulativeCount = 0;
        for (var i = 0; i < BinCount; i++)
        {
            cumulativeCount += Bins[i];
            if (cumulativeCount < medianIndex)
                continue;

            var countInBin = Bins[i];
            var previousCount = cumulativeCount - countInBin;
            var fraction = (double)(medianIndex - previousCount) / countInBin;

            return GetBinCenter(i) - _binWidth / 2.0 + fraction * _binWidth;
        }

        return _maxBound;
    }

    public object Clone()
    {
        var newHistogram = new Histogram(_minBound, _maxBound, BinCount, _isDiscrete)
        {
            OutOfBoundsSamples = OutOfBoundsSamples,
            TotalSamples = TotalSamples
        };
        Array.Copy(Bins, newHistogram.Bins, BinCount);
        return newHistogram;
    }

    // ====================================================================
    // Statistical & Debug Methods
    // ====================================================================

    private double CalculateSkewness(double mean, double stdDev)
    {
        if (TotalSamples <= 2 || stdDev == 0) return 0;

        double sumOfCubes = 0;
        for (var i = 0; i < BinCount; i++)
        {
            var deviation = GetBinCenter(i) - mean;
            sumOfCubes += Math.Pow(deviation, 3) * Bins[i];
        }

        var m3 = sumOfCubes / TotalSamples;
        return m3 / Math.Pow(stdDev, 3);
    }

    private double CalculateKurtosis(double mean, double stdDev)
    {
        if (TotalSamples <= 3 || stdDev == 0) return 0;

        double sumOfQuads = 0;
        for (var i = 0; i < BinCount; i++)
        {
            var deviation = GetBinCenter(i) - mean;
            sumOfQuads += Math.Pow(deviation, 4) * Bins[i];
        }

        var m4 = sumOfQuads / TotalSamples;
        return m4 / Math.Pow(stdDev, 4) - 3.0;
    }

    private double GetValueForBin(int binIndex)
    {
        return _isDiscrete ? _minBound + binIndex : GetBinCenter(binIndex);
    }

    public void DebugPrint(ITestOutputHelper output, [CallerMemberName] string title = null!)
    {
        const int width = 68;

        if (TotalSamples == 0)
        {
            output.WriteLine($"--- {title} (No Data) ---");
            return;
        }

        var mean = CalculateMean();
        var stdDev = CalculateStdDev(mean);
        var skewness = CalculateSkewness(mean, stdDev);
        var kurtosis = CalculateKurtosis(mean, stdDev);

        output.WriteLine($"--- {title} ---");
        output.WriteLine($"Samples: {TotalSamples:N0}, Bins: {BinCount:N0}, Range: [{_minBound:F2}, {_maxBound:F2})");
        output.WriteLine($"Mean:    {mean:F4}, StdDev: {stdDev:F4}");
        output.WriteLine($"Skew:    {skewness:F4}, Kurtosis: {kurtosis:F4} (Excess)");
        output.WriteLine(new string('-', width));

        var maxBinCount = 0L;
        foreach (var bin in Bins)
            if (bin > maxBinCount)
                maxBinCount = bin;

        if (maxBinCount == 0) return;

        char[] blocks = ['█', '▉', '▊', '▋', '▌', '▍', '▎', '▏'];

        for (var i = 0; i < BinCount; i++)
        {
            const int barChartWidth = width - 12;

            var binCount = Bins[i];
            var normalizedLength = (double)binCount / maxBinCount * barChartWidth;
            var fullBlocks = (int)normalizedLength;
            var fractionalPart = normalizedLength - fullBlocks;
            var fractionalIndex = (int)(fractionalPart * blocks.Length);
            var fractionalChar = fullBlocks < barChartWidth && fractionalIndex > 0
                ? blocks[fractionalIndex - 1].ToString()
                : "";

            var valueLabel = GetValueForBin(i);
            var bar = new string('█', fullBlocks) + fractionalChar;

            output.WriteLine($"[{valueLabel,6:F2}] | {bar}");
        }

        output.WriteLine(new string('-', width));
    }
}

public interface IHistogramSampler<out T, TRng>
    where TRng : struct, IChiRngSource<TRng>
{
    T NextSample(ref TRng rng);
}

public interface IHistogramSamplerWithRange<T, TRng> : IHistogramSampler<T, TRng>
    where TRng : struct, IChiRngSource<TRng>
{
    double Normalize(T value);
}