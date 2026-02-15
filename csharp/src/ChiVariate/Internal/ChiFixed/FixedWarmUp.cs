// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ChiVariate.Internal.ChiFixed;

/// <summary>
///     Forces initialization of all ChiFixed lookup tables.
/// </summary>
internal static class FixedWarmUp
{
    private static readonly Type[] TableTypes =
    [
        typeof(CordicTables),
        typeof(TableSinCos),
        typeof(TableAtan),
        typeof(TableLn),
        typeof(TaylorExp),
        typeof(NewtonSqrt),
        typeof(NewtonCbrt),
        typeof(FixedMath),
        typeof(Pow10Round)
    ];

    internal static Dictionary<Type, TimeSpan> Run()
    {
        var result = new Dictionary<Type, TimeSpan>(TableTypes.Length);

        foreach (var type in TableTypes)
        {
            var sw = Stopwatch.StartNew();
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            sw.Stop();
            result[type] = sw.Elapsed;
        }

        return result;
    }
}