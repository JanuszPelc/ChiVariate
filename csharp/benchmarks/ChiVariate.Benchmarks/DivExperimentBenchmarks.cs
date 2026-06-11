using System.Numerics;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Benchmarks;

// Q31.32 ChiFixed division candidates, vs hardware floors and the committed impl.
// Verdict (M1, see results below): nothing beat Div_Schoolbook.
//
// Div_<Type> = raw-op floor (returns the raw type); Div_<Name> = Q31.32 candidate
// (returns ChiFixed). DivExperimentCore.DivSchoolbook mirrors FixedMath.Div.
// Throughput: each op is independent (overwritten), so divides overlap in the pipeline.
// GlobalSetup verifies every candidate bit-exact vs UInt128 before measuring.

[MemoryDiagnoser]
[SimpleJob]
[MinIterationTime(500)]
[IterationCount(15)]
public class DivExperimentBenchmarks
{
    private const int Ops = DivDataSet.Ops;

    private double[] _doubleValues = null!;
    private double[] _doubleValues2 = null!;
    private Int128[] _int128Values = null!;
    private Int128[] _int128Values2 = null!;
    private long[] _longValues = null!;
    private long[] _longValues2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        var data = DivDataSet.Generate();
        _doubleValues = data.DoubleA;
        _doubleValues2 = data.DoubleB;
        _longValues = data.LongA;
        _longValues2 = data.LongB;
        _int128Values = data.Int128A;
        _int128Values2 = data.Int128B;

        DivExperimentVerify.QuickGate(_longValues, _longValues2);
    }

    [Benchmark(Baseline = true)]
    public double Div_Double()
    {
        var result = 1.0;
        for (var i = 0; i < Ops; i++)
            result = _doubleValues[i] / _doubleValues2[i];
        return result;
    }

    [Benchmark]
    public long Div_Long()
    {
        var result = 1L;
        for (var i = 0; i < Ops; i++)
            result = _longValues[i] / _longValues2[i];
        return result;
    }

    [Benchmark]
    public Int128 Div_Int128()
    {
        var result = (Int128)1;
        for (var i = 0; i < Ops; i++)
            result = _int128Values[i] / _int128Values2[i];
        return result;
    }

    // Current ChiFixed division (FixedMath.Div), via DivExperimentCore.DivSchoolbook.
    [Benchmark]
    public ChiFixed Div_Schoolbook()
    {
        var result = ChiFixed.One;
        for (var i = 0; i < Ops; i++)
            result = new ChiFixed(DivExperimentCore.DivSchoolbook(_longValues[i], _longValues2[i]));
        return result;
    }

    [Benchmark]
    public ChiFixed Div_Wide128()
    {
        var result = ChiFixed.One;
        for (var i = 0; i < Ops; i++)
            result = new ChiFixed(DivExperimentCore.DivWide128(_longValues[i], _longValues2[i]));
        return result;
    }

    [Benchmark]
    public ChiFixed Div_Stratified()
    {
        var result = ChiFixed.One;
        for (var i = 0; i < Ops; i++)
            result = new ChiFixed(DivExperimentCore.DivStratified(_longValues[i], _longValues2[i]));
        return result;
    }

    [Benchmark]
    public ChiFixed Div_Hybrid()
    {
        var result = ChiFixed.One;
        for (var i = 0; i < Ops; i++)
            result = new ChiFixed(DivExperimentCore.DivHybrid(_longValues[i], _longValues2[i]));
        return result;
    }

    [Benchmark]
    public ChiFixed Div_MollerGranlund()
    {
        var result = ChiFixed.One;
        for (var i = 0; i < Ops; i++)
            result = new ChiFixed(DivExperimentCore.DivMollerGranlund(_longValues[i], _longValues2[i]));
        return result;
    }

    [Benchmark]
    public ChiFixed Div_NewtonRaphson()
    {
        var result = ChiFixed.One;
        for (var i = 0; i < Ops; i++)
            result = new ChiFixed(DivExperimentCore.DivNewtonRaphson(_longValues[i], _longValues2[i]));
        return result;
    }

    [Benchmark]
    public ChiFixed Div_Goldschmidt()
    {
        var result = ChiFixed.One;
        for (var i = 0; i < Ops; i++)
            result = new ChiFixed(DivExperimentCore.DivGoldschmidt(_longValues[i], _longValues2[i]));
        return result;
    }

    [Benchmark]
    public ChiFixed Div_FpHint()
    {
        var result = ChiFixed.One;
        for (var i = 0; i < Ops; i++)
            result = new ChiFixed(DivExperimentCore.DivFpHint(_longValues[i], _longValues2[i]));
        return result;
    }
}

/// <summary>
///     Benchmark operand arrays, generated once from a fixed seed.
/// </summary>
file sealed class DivDataSet
{
    public const int Ops = 1_000;
    private const int Seed = 42;

    // Independent log-normal magnitudes with random signs: median 1 (even split below/
    // above 1), sigma 7 spans ChiFixed's full ~19-decade range — so the data hits the
    // sub-1 fast path, the large-operand Knuth path, and the saturation early-out alike.
    private const decimal LogMean = 0.0m; // median magnitude = 1
    private const decimal LogSigma = 7.0m; // magnitudes span ~1e-9 .. 1e9
    private const decimal MinAbsDivisor = 1e-9m; // keep the divisor off round-to-zero
    private const decimal MaxLogMagnitude = 21.4m; // ≈ ln(MaxValue): clamp so decimal Exp can't overflow

    public readonly double[] DoubleA = new double[Ops];
    public readonly double[] DoubleB = new double[Ops];
    public readonly Int128[] Int128A = new Int128[Ops];
    public readonly Int128[] Int128B = new Int128[Ops];
    public readonly long[] LongA = new long[Ops];
    public readonly long[] LongB = new long[Ops];

    public static DivDataSet Generate()
    {
        var data = new DivDataSet();
        var rng = new ChiRng(Seed);

        for (var i = 0; i < Ops; i++)
        {
            var v1 = NextSignedMagnitude(ref rng);
            var v2 = NextSignedMagnitude(ref rng);
            if (Math.Abs(v2) < MinAbsDivisor) v2 = 1.0m;

            data.DoubleA[i] = (double)v1;
            data.DoubleB[i] = (double)v2;
            data.LongA[i] = ((ChiFixed)v1).Raw;
            data.LongB[i] = ((ChiFixed)v2).Raw;
            data.Int128A[i] = data.LongA[i];
            data.Int128B[i] = data.LongB[i];
        }

        return data;

        static decimal NextSignedMagnitude(ref ChiRng rng)
        {
            // Clamp the log-magnitude before Exp: the sigma-7 tail (~±11) would overflow
            // decimal Exp, and anything past the range would just saturate on conversion.
            var sample = rng.Normal(LogMean, LogSigma).Sample();
            var logMagnitude = Math.Clamp(sample, -MaxLogMagnitude, MaxLogMagnitude);
            var magnitude = ChiMath.Exp(logMagnitude);
            return rng.Chance().FlipCoin() ? magnitude : -magnitude;
        }
    }
}

/// <summary>
///     Q31.32 division candidates. Every method computes the bit-exact truncated
///     quotient floor((|a| &lt;&lt; 32) / |b|) with sign and saturation handling
///     identical to FixedMath.Div — only the route differs.
/// </summary>
file static class DivExperimentCore
{
    // floor((sqrt(2) - 1/2) * 2^64): seed V0 = K - 2d, the minimax line C - 2D for
    // 1/D on [0.5, 1) with C = (3 + 2√2)/2 ≈ 2.9142 (~3.5 correct bits).
    private const ulong NewtonSeedK = 0xEA09E667F3BCC908UL;

    // Möller–Granlund — division-free. Table-seeded exact reciprocal (Algorithm 2)
    // then one 2/1 division step (Algorithm 4). "Improved division by invariant
    // integers", Möller & Granlund, IEEE TC 2011.

    // table[i] = floor((2^19 - 3*2^8) / (256 + i)) — 11-bit seed, 512 bytes.
    private static readonly ushort[] RecipTable = CreateRecipTable();

    // Schoolbook — verbatim copy of FixedMath.Div (the committed implementation).
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long DivSchoolbook(long a, long b)
    {
        if (b == 0) throw new DivideByZeroException();

        var negative = (a ^ b) < 0;
        var uA = (ulong)(a < 0 ? -a : a);
        var uB = (ulong)(b < 0 ? -b : b);

        // Decompose: (uA << 32) / uB = (uA/uB) << 32 + ((uA%uB) << 32) / uB
        var q1 = uA / uB;
        var r1 = uA - q1 * uB;

        if (q1 > uint.MaxValue)
            return negative ? long.MinValue : long.MaxValue;

        // Fractional part: (r1 << 32) / uB, result < 2^32
        ulong q2;
        var r1Hi = r1 >> 32;
        if (r1Hi == 0)
        {
            // r1 << 32 fits in 64 bits — plain division
            q2 = (r1 << 32) / uB;
        }
        else
        {
            // Single-digit Knuth: quotient fits in 32 bits
            var shift = BitOperations.LeadingZeroCount(uB);
            var d = uB << shift;
            var nHi = (r1Hi << shift) | (shift == 0 ? 0 : (r1 << 32) >> (64 - shift));
            var nLo = r1 << (32 + shift);

            var dHi = d >> 32;
            var dLo = d & 0xFFFF_FFFF;
            var rem = (nHi << 32) | (nLo >> 32);

            q2 = rem / dHi;
            var rr = rem - q2 * dHi;
            while (q2 >= 0x1_0000_0000UL || q2 * dLo > ((rr << 32) | (nLo & 0xFFFF_FFFF)))
            {
                q2--;
                rr += dHi;
                if (rr >= 0x1_0000_0000UL) break;
            }
        }

        var quotient = (q1 << 32) | q2;
        if (quotient > long.MaxValue)
            return negative ? long.MinValue : long.MaxValue;
        return negative ? -(long)quotient : (long)quotient;
    }

    // Wide128 — one UInt128 division, no decomposition.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long DivWide128(long a, long b)
    {
        if (b == 0) throw new DivideByZeroException();

        var negative = (a ^ b) < 0;
        var uA = (ulong)(a < 0 ? -a : a);
        var uB = (ulong)(b < 0 ? -b : b);

        var q = ((UInt128)uA << 32) / uB;
        if (q > (UInt128)long.MaxValue)
            return negative ? long.MinValue : long.MaxValue;
        return negative ? -(long)(ulong)q : (long)(ulong)q;
    }

    // Stratified — |a| < 1 fast path (numerator fits 64 bits → one hardware udiv);
    // everything else falls through to the schoolbook tail.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long DivStratified(long a, long b)
    {
        if (b == 0) throw new DivideByZeroException();

        var negative = (a ^ b) < 0;
        var uA = (ulong)(a < 0 ? -a : a);
        var uB = (ulong)(b < 0 ? -b : b);

        if (uA >> 32 != 0) return SchoolbookTail(uA, uB, negative);

        // The quotient can still overflow long when the divisor is tiny
        // (e.g. 0.9 / 2^-32), so the saturation check stays.
        var q = (uA << 32) / uB;
        if (q > long.MaxValue)
            return negative ? long.MinValue : long.MaxValue;
        return negative ? -(long)q : (long)q;
    }

    // Schoolbook body after sign decomposition (uA ≥ 2^32 when called from DivStratified).
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long SchoolbookTail(ulong uA, ulong uB, bool negative)
    {
        var q1 = uA / uB;
        var r1 = uA - q1 * uB;

        if (q1 > uint.MaxValue)
            return negative ? long.MinValue : long.MaxValue;

        ulong q2;
        var r1Hi = r1 >> 32;
        if (r1Hi == 0)
        {
            q2 = (r1 << 32) / uB;
        }
        else
        {
            var shift = BitOperations.LeadingZeroCount(uB);
            var d = uB << shift;
            var nHi = (r1Hi << shift) | (shift == 0 ? 0 : (r1 << 32) >> (64 - shift));
            var nLo = r1 << (32 + shift);

            var dHi = d >> 32;
            var dLo = d & 0xFFFF_FFFF;
            var rem = (nHi << 32) | (nLo >> 32);

            q2 = rem / dHi;
            var rr = rem - q2 * dHi;
            while (q2 >= 0x1_0000_0000UL || q2 * dLo > ((rr << 32) | (nLo & 0xFFFF_FFFF)))
            {
                q2--;
                rr += dHi;
                if (rr >= 0x1_0000_0000UL) break;
            }
        }

        var quotient = (q1 << 32) | q2;
        if (quotient > long.MaxValue)
            return negative ? long.MinValue : long.MaxValue;
        return negative ? -(long)quotient : (long)quotient;
    }

    // Hybrid — schoolbook, but the single-digit Knuth block becomes one UInt128
    // division on the rare r1 ≥ 2^32 path. Tests whether Knuth earns its ~30 lines.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long DivHybrid(long a, long b)
    {
        if (b == 0) throw new DivideByZeroException();

        var negative = (a ^ b) < 0;
        var uA = (ulong)(a < 0 ? -a : a);
        var uB = (ulong)(b < 0 ? -b : b);

        var q1 = uA / uB;
        var r1 = uA - q1 * uB;

        if (q1 > uint.MaxValue)
            return negative ? long.MinValue : long.MaxValue;

        // r1 < uB, so the 96-bit (r1 << 32) / uB quotient always fits in 32 bits.
        var q2 = r1 >> 32 == 0
            ? (r1 << 32) / uB
            : (ulong)(((UInt128)r1 << 32) / uB);

        var quotient = (q1 << 32) | q2;
        if (quotient > long.MaxValue)
            return negative ? long.MinValue : long.MaxValue;
        return negative ? -(long)quotient : (long)quotient;
    }

    private static ushort[] CreateRecipTable()
    {
        var table = new ushort[256];
        for (var i = 0; i < 256; i++)
            table[i] = (ushort)(((1u << 19) - 3 * (1u << 8)) / (uint)(256 + i));
        return table;
    }

    // Exact reciprocal v = floor((2^128-1)/d) - 2^64 for normalized d (bit 63 set).
    // M-G Algorithm 2: 11-bit table seed refined to 21 → 34 → 65 bits, all mod 2^64.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong Reciprocal2By1(ulong d)
    {
        var d9 = d >> 55;
        var v0 = (ulong)RecipTable[(int)(d9 - 256)];
        var d40 = (d >> 24) + 1;
        var v1 = (v0 << 11) - ((v0 * v0 * d40) >> 40) - 1;
        var v2 = (v1 << 13) + ((v1 * ((1UL << 60) - v1 * d40)) >> 47);
        var d0 = d & 1;
        var d63 = (d >> 1) + d0; // ceil(d / 2)
        var e = ((v2 >> 1) & (0UL - d0)) - v2 * d63;
        var v3 = (Math.BigMul(v2, e, out _) >> 1) + (v2 << 31);
        var pHi = Math.BigMul(v3, d, out var pLo);
        pLo += d;
        pHi += d + (pLo < d ? 1UL : 0UL);
        return v3 - pHi;
    }

    // 2/1 division by a precomputed exact reciprocal (M-G Algorithm 4 /
    // GMP udiv_qrnnd_preinv). Pre: d normalized, u1 < d, v exact. Returns the quotient.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong UdivQrnnd(ulong u1, ulong u0, ulong d, ulong v)
    {
        var pHi = Math.BigMul(v, u1, out var pLo);
        var qLo = pLo + u0;
        var qHi = pHi + u1 + 1 + (qLo < pLo ? 1UL : 0UL);

        var r = u0 - qHi * d;

        // First adjustment fires ~50/50 ("inherently unpredictable" per the paper),
        // so keep it branch-free; the second is rare, a predicted branch is fine.
        var over = r > qLo ? 1UL : 0UL;
        qHi -= over;
        r += d & (0UL - over);

        if (r >= d)
            qHi++;

        return qHi;
    }

    // |a| << t (t = 32 + shift) as 128 bits: hi returned, lo via out.
    // Caller guarantees the quotient fits, i.e. < 2^64.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong Numerator96(ulong uA, int t, out ulong nLo)
    {
        if (t >= 64)
        {
            nLo = 0;
            return uA << (t - 64);
        }

        nLo = uA << t;
        return uA >> (64 - t); // t ∈ [32, 63] here, so the shift count is in [1, 32]
    }

    // True when floor((uA << 32)/uB) ≥ 2^63 (result saturates):
    // q ≥ 2^63 ⟺ uA ≥ uB·2^31, impossible once uB ≥ 2^33.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool SaturatesQ63(ulong uA, ulong uB)
    {
        return uB >> 33 == 0 && uA >= uB << 31;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long DivMollerGranlund(long a, long b)
    {
        if (b == 0) throw new DivideByZeroException();

        var negative = (a ^ b) < 0;
        var uA = (ulong)(a < 0 ? -a : a);
        var uB = (ulong)(b < 0 ? -b : b);

        if (SaturatesQ63(uA, uB))
            return negative ? long.MinValue : long.MaxValue;

        var s = BitOperations.LeadingZeroCount(uB);
        var d = uB << s;
        var v = Reciprocal2By1(d);
        var nHi = Numerator96(uA, 32 + s, out var nLo);
        var q = UdivQrnnd(nHi, nLo, d, v);
        return negative ? -(long)q : (long)q;
    }

    // Newton–Raphson — same Algorithm-4 backend, but a table-free linear seed
    // (~3.5 bits) refined by 5 quadratic steps + an exactness net. A/B vs
    // Div_MollerGranlund prices the 512-byte table.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong SeedV(ulong d)
    {
        var twoD = d << 1; // 2d - 2^64 (bit 63 of d is set)
        return twoD <= NewtonSeedK ? NewtonSeedK - twoD : 0UL;
    }

    // One reciprocal step in V-form (V = y - 1, Q0.64): V' = V + f + hi(V·f),
    // signed f, with saturation guards at the V → 2^64-1 and V → 0 edges.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong ApplyReciprocalStep(ulong v, long f)
    {
        if (f >= 0)
        {
            var e = (ulong)f;
            var inc = e + Math.BigMul(v, e, out _);
            var next = v + inc;
            return next < v ? ulong.MaxValue : next;
        }

        var m = (ulong)-f;
        var dec = m + Math.BigMul(v, m, out _);
        var prev = v - dec;
        return prev > v ? 0UL : prev;
    }

    // Bounded ±1 net: nudge an almost-exact estimate to v = floor((2^128-1)/d) - 2^64,
    // i.e. hi(v·d) ≤ ~d < hi((v+1)·d). One mulhi per step; ~0-3 steps in practice.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong ExactReciprocal(ulong d, ulong v)
    {
        var notD = ~d;
        while (Math.BigMul(v, d, out _) > notD)
            v--;
        while (v != ulong.MaxValue && Math.BigMul(v + 1, d, out _) <= notD)
            v++;
        return v;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong ReciprocalNewton(ulong d)
    {
        var v = SeedV(d);
        // 5 quadratic steps (3.5 → 112 bits; 4 would stall at ~56). f = 1 - D·y, signed.
        for (var k = 0; k < 5; k++)
        {
            var f = (long)(0UL - d - Math.BigMul(d, v, out _));
            v = ApplyReciprocalStep(v, f);
        }

        return ExactReciprocal(d, v);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long DivNewtonRaphson(long a, long b)
    {
        if (b == 0) throw new DivideByZeroException();

        var negative = (a ^ b) < 0;
        var uA = (ulong)(a < 0 ? -a : a);
        var uB = (ulong)(b < 0 ? -b : b);

        if (SaturatesQ63(uA, uB))
            return negative ? long.MinValue : long.MaxValue;

        var s = BitOperations.LeadingZeroCount(uB);
        var d = uB << s;
        var v = ReciprocalNewton(d);
        var nHi = Numerator96(uA, 32 + s, out var nLo);
        var q = UdivQrnnd(nHi, nLo, d, v);
        return negative ? -(long)q : (long)q;
    }

    // Goldschmidt — Newton's multiply count, but f² and V·f are independent per
    // step (ILP over self-correction). Truncation noise accumulates linearly
    // (the classic caveat); the exactness net absorbs it.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong ReciprocalGoldschmidt(ulong d)
    {
        var v = SeedV(d);
        var f = (long)(0UL - d - Math.BigMul(d, v, out _)); // f1 = 1 - D·y0
        for (var k = 0; k < 5; k++)
        {
            var fNext = Math.BigMul(f, f, out _); // f² — independent of the V update
            v = ApplyReciprocalStep(v, f);
            f = fNext;
        }

        return ExactReciprocal(d, v);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long DivGoldschmidt(long a, long b)
    {
        if (b == 0) throw new DivideByZeroException();

        var negative = (a ^ b) < 0;
        var uA = (ulong)(a < 0 ? -a : a);
        var uB = (ulong)(b < 0 ? -b : b);

        if (SaturatesQ63(uA, uB))
            return negative ? long.MinValue : long.MaxValue;

        var s = BitOperations.LeadingZeroCount(uB);
        var d = uB << s;
        var v = ReciprocalGoldschmidt(d);
        var nHi = Numerator96(uA, 32 + s, out var nLo);
        var q = UdivQrnnd(nHi, nLo, d, v);
        return negative ? -(long)q : (long)q;
    }

    // FpHint — double divide as a quotient hint, then exact integer correction
    // (two FP passes + a ±1 sweep on the true 128-bit remainder). The result is
    // exact, so it's deterministic — but it puts doubles in a ChiFixed path
    // (policy call). Same shape as Monniaux & Pain (arXiv:2207.08420, CompCert).
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long DivFpHint(long a, long b)
    {
        if (b == 0) throw new DivideByZeroException();

        var negative = (a ^ b) < 0;
        var uA = (ulong)(a < 0 ? -a : a);
        var uB = (ulong)(b < 0 ? -b : b);

        if (SaturatesQ63(uA, uB))
            return negative ? long.MinValue : long.MaxValue;

        var dB = (double)uB;
        var qHat = (ulong)(uA * 4294967296.0 / dB);

        var n = (UInt128)uA << 32;
        var p = (UInt128)qHat * uB;

        if (n >= p)
        {
            var delta = (ulong)((double)(n - p) / dB);
            qHat += delta;
        }
        else
        {
            var delta = (ulong)((double)(p - n) / dB);
            qHat = delta > qHat ? 0UL : qHat - delta;
        }

        var p2 = (UInt128)qHat * uB;
        while (p2 > n)
        {
            qHat--;
            p2 -= uB;
        }

        var r = n - p2;
        while (r >= uB)
        {
            qHat++;
            r -= uB;
        }

        return negative ? -(long)qHat : (long)qHat;
    }
}

/// <summary>
///     Differential verification: every candidate against a UInt128 reference
///     (and the committed ChiFixed operator). QuickGate runs in GlobalSetup before
///     every benchmark and throws on the first mismatch, so no candidate is ever
///     measured unless it is bit-exact against the reference.
/// </summary>
file static class DivExperimentVerify
{
    private static readonly (string Name, Func<long, long, long> Div)[] Candidates =
    [
        ("Schoolbook", DivExperimentCore.DivSchoolbook),
        ("Wide128", DivExperimentCore.DivWide128),
        ("Stratified", DivExperimentCore.DivStratified),
        ("Hybrid", DivExperimentCore.DivHybrid),
        ("MollerGranlund", DivExperimentCore.DivMollerGranlund),
        ("NewtonRaphson", DivExperimentCore.DivNewtonRaphson),
        ("Goldschmidt", DivExperimentCore.DivGoldschmidt),
        ("FpHint", DivExperimentCore.DivFpHint)
    ];

    public static void QuickGate(long[] a, long[] b)
    {
        for (var i = 0; i < a.Length; i++)
            CheckAll(a[i], b[i]);

        var edges = EdgeValues();
        foreach (var x in edges)
        foreach (var y in edges)
            if (y != 0)
                CheckAll(x, y);

        var state = 0x9E3779B97F4A7C15UL;
        for (var i = 0; i < 100_000; i++)
        {
            var x = (long)SplitMix64(ref state);
            var y = (long)SplitMix64(ref state);
            if (y == 0) continue;
            CheckAll(x, y);
        }
    }

    private static void CheckAll(long a, long b)
    {
        var expected = Reference(a, b);

        var viaOperator = (new ChiFixed(a) / new ChiFixed(b)).Raw;
        if (viaOperator != expected)
            throw Mismatch("ChiFixed operator /", a, b, expected, viaOperator);

        foreach (var (name, div) in Candidates)
        {
            var actual = div(a, b);
            if (actual != expected)
                throw Mismatch(name, a, b, expected, actual);
        }
    }

    private static long Reference(long a, long b)
    {
        var negative = (a ^ b) < 0;
        var uA = (ulong)(a < 0 ? -a : a);
        var uB = (ulong)(b < 0 ? -b : b);

        var q = ((UInt128)uA << 32) / uB;
        if (q > (UInt128)long.MaxValue)
            return negative ? long.MinValue : long.MaxValue;
        return negative ? -(long)(ulong)q : (long)(ulong)q;
    }

    private static long[] EdgeValues()
    {
        var list = new List<long> { 0, long.MaxValue, long.MinValue, long.MaxValue - 1, long.MinValue + 1 };
        for (var k = 0; k < 63; k++)
        {
            var p = 1L << k;
            list.Add(p);
            list.Add(-p);
            list.Add(p - 1);
            list.Add(-(p - 1));
            list.Add(p + 1);
            list.Add(-(p + 1));
        }

        return list.Distinct().ToArray();
    }

    private static ulong SplitMix64(ref ulong state)
    {
        state += 0x9E3779B97F4A7C15UL;
        var z = state;
        z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
        z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
        return z ^ (z >> 31);
    }

    private static InvalidOperationException Mismatch(string name, long a, long b, long expected, long actual)
    {
        return new InvalidOperationException(
            $"Div mismatch [{name}]: a=0x{a:X16} ({a}), b=0x{b:X16} ({b}), " +
            $"expected=0x{expected:X16}, actual=0x{actual:X16}");
    }
}

/*

// * Summary *

BenchmarkDotNet v0.14.0, macOS Sequoia 15.7.7 (24G720) [Darwin 24.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD
  Job-KAPEUR : .NET 9.0.11 (9.0.1125.51716), Arm64 RyuJIT AdvSIMD

MinIterationTime=500ms  IterationCount=15

| Method             | Mean        | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------------- |------------:|---------:|---------:|------:|--------:|----------:|------------:|
| Div_Double         |    669.6 ns |  0.51 ns |  0.43 ns |  1.00 |    0.00 |         - |          NA |
| Div_Long           |    940.8 ns |  0.29 ns |  0.24 ns |  1.40 |    0.00 |         - |          NA |
| Div_Int128         |  2,674.3 ns |  1.84 ns |  1.72 ns |  3.99 |    0.00 |         - |          NA |
| Div_Schoolbook     |  2,961.9 ns |  0.87 ns |  0.72 ns |  4.42 |    0.00 |         - |          NA |
| Div_Wide128        | 11,627.2 ns | 54.85 ns | 45.80 ns | 17.36 |    0.07 |         - |          NA |
| Div_Stratified     |  3,187.9 ns |  2.57 ns |  2.40 ns |  4.76 |    0.00 |         - |          NA |
| Div_Hybrid         |  5,588.3 ns | 18.11 ns | 16.05 ns |  8.35 |    0.02 |         - |          NA |
| Div_MollerGranlund |  6,790.7 ns |  3.82 ns |  3.39 ns | 10.14 |    0.01 |         - |          NA |
| Div_NewtonRaphson  |  9,453.1 ns | 32.50 ns | 27.14 ns | 14.12 |    0.04 |         - |          NA |
| Div_Goldschmidt    |  8,579.5 ns | 27.13 ns | 22.65 ns | 12.81 |    0.03 |         - |          NA |
| Div_FpHint         |  4,334.9 ns |  4.21 ns |  3.73 ns |  6.47 |    0.01 |         - |          NA |

// * Hints *
Outliers
  DivExperimentBenchmarks.Div_Double: MinIterationTime=500ms, IterationCount=15         -> 2 outliers were removed (676.23 ns, 680.57 ns)
  DivExperimentBenchmarks.Div_Long: MinIterationTime=500ms, IterationCount=15           -> 2 outliers were removed (943.70 ns, 950.15 ns)
  DivExperimentBenchmarks.Div_Schoolbook: MinIterationTime=500ms, IterationCount=15     -> 2 outliers were removed (2.97 us, 2.98 us)
  DivExperimentBenchmarks.Div_Wide128: MinIterationTime=500ms, IterationCount=15        -> 2 outliers were removed, 3 outliers were detected (11.53 us, 11.86 us, 11.87 us)
  DivExperimentBenchmarks.Div_Hybrid: MinIterationTime=500ms, IterationCount=15         -> 1 outlier  was  removed (5.65 us)
  DivExperimentBenchmarks.Div_MollerGranlund: MinIterationTime=500ms, IterationCount=15 -> 1 outlier  was  removed (6.82 us)
  DivExperimentBenchmarks.Div_NewtonRaphson: MinIterationTime=500ms, IterationCount=15  -> 2 outliers were removed (9.57 us, 9.59 us)
  DivExperimentBenchmarks.Div_Goldschmidt: MinIterationTime=500ms, IterationCount=15    -> 2 outliers were removed (8.69 us, 8.80 us)
  DivExperimentBenchmarks.Div_FpHint: MinIterationTime=500ms, IterationCount=15         -> 1 outlier  was  removed (4.36 us)

// * Legends *
  Mean        : Arithmetic mean of all measurements
  Error       : Half of 99.9% confidence interval
  StdDev      : Standard deviation of all measurements
  Ratio       : Mean of the ratio distribution ([Current]/[Baseline])
  RatioSD     : Standard deviation of the ratio distribution ([Current]/[Baseline])
  Allocated   : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  Alloc Ratio : Allocated memory ratio distribution ([Current]/[Baseline])
  1 ns        : 1 Nanosecond (0.000000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
Run time: 00:03:32 (212.67 sec), executed benchmarks: 11

Global total time: 00:03:39 (219.05 sec), executed benchmarks: 11
// * Artifacts cleanup *
Artifacts cleanup is finished

Process finished with exit code 0.

*/