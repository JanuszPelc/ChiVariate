using System.Reflection;
using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSeedTests;

/// <summary>
///     Guards the deliberate API symmetry between <see cref="ChiSeed" /> (64-bit) and its
///     32-bit counterpart <see cref="ChiHash" />: when an overload is added to one type's
///     surface, these tests fail until its counterpart exists on the other.
/// </summary>
public class ChiSeedSymmetryTests
{
    [Fact]
    public void Add_OverloadSignatures_MatchBetweenChiHashAndChiSeed()
    {
        var hashOverloads = GetSignatures(typeof(ChiHash), nameof(ChiHash.Add));
        var seedOverloads = GetSignatures(typeof(ChiSeed), nameof(ChiSeed.Add));

        seedOverloads.Should().Equal(hashOverloads);
    }

    [Fact]
    public void Combine_OverloadSignatures_MatchChiSeedScramble()
    {
        var combineOverloads = GetSignatures(typeof(ChiHash), nameof(ChiHash.Combine));
        var scrambleOverloads = GetSignatures(typeof(ChiSeed), nameof(ChiSeed.Scramble));

        scrambleOverloads.Should().Equal(combineOverloads);
    }

    private static string[] GetSignatures(Type type, string methodName)
    {
        return type
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(method => method.Name == methodName)
            .Select(method => string.Join(", ", method.GetParameters()
                .Select(parameter => parameter.ParameterType.ToString())))
            .OrderBy(signature => signature, StringComparer.Ordinal)
            .ToArray();
    }
}