using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace ChiVariate.Tests.ChiSamplerUtlTests.LoremIpsum;

#pragma warning disable CS1591

public class DolorSitTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void DolorSit_WithFixedSeedAndZeroChaos_IsDeterministicAndCanonical()
    {
        // Arrange
        var rng1 = new ChiRng(42);
        var rng2 = new ChiRng(42);

        // Act
        var result1 = rng1.LoremIpsum(0.0f).DolorSit();
        var result2 = rng2.LoremIpsum(0.0f).DolorSit();
        testOutputHelper.WriteLine(result1);

        // Assert
        result1.Should().Be(result2, "because with the same seed, the output must be identical.");
        result1.Should().StartWith("Lorem ipsum dolor sit amet. Consectetur adipiscing elit sed do eiusmod tempor,",
            "because with zero chaos, it should follow the canonical sequence with each sentence starting from the beginning.");

        var sentences = result1.Split('.', StringSplitOptions.RemoveEmptyEntries);
        sentences.Should().HaveCount(5, "because zero chaos should produce exactly 5 sentences per paragraph");
        sentences[^1].Trim().Should().Be(
            "Duis aute irure dolor in reprehenderit",
            "because the last sentence should follow the canonical.");
    }

    [Fact]
    public void DolorSit_WithCorporateBuzzwordIpsum_UsesProvidedVocabulary()
    {
        // Arrange
        var rng = new ChiRng("Corporate Buzzword Ipsum");

        const string corporateBuzzwords2025 =
            """
            sustainable competitive advantages, strategic market penetration, compelling value propositions,
            comprehensive go-to-market strategies, validated product-market fit, optimized customer journeys,
            high-conversion funnel design, strategic touchpoint analysis, enhanced user experiences,

            leverage innovative solutions, synergize cross-functional teams, optimize operational efficiency,
            streamline business processes, disruptive market innovations, scalable growth strategies,
            transformational digital initiatives, accelerated innovation cycles, maximized ROI outcomes,
            enhanced customer experiences, streamlined operational workflows,

            AI-powered solutions, GenAI-driven insights, LLM-based architecture, RAG implementation strategy,
            agentic workflow optimization, multimodal AI capabilities, prompt engineering excellence,
            vector database infrastructure, retrieval-augmented intelligence, fine-tuning methodologies,
            model training pipelines, inference optimization techniques, advanced embedding models,
            transformer architecture deployment, intelligent AI agents, seamless copilot integration,

            cloud-native architecture solutions, data-driven strategic insights, customer-centric methodologies,
            agile transformation frameworks, digital-first strategies, omnichannel customer experiences,
            seamless system integration, robust infrastructure platforms, enterprise-grade solutions,
            """;

        // Act
        var result = rng.LoremIpsum(vocabulary: corporateBuzzwords2025).DolorSit(7, 27);
        testOutputHelper.WriteLine(result);

        // Assert
        var buzzwordList =
            corporateBuzzwords2025.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var foundCount = buzzwordList.Count(word =>
            result.Contains(word.Replace("-", " "), StringComparison.OrdinalIgnoreCase) ||
            result.Contains(word.Replace("-", ""), StringComparison.OrdinalIgnoreCase));

        (foundCount / (double)buzzwordList.Length).Should().BeGreaterThan(0.5,
            "because most buzzwords should appear in the generated text");

        result.Should().NotContain("Lorem", "because the default Latin vocabulary should be overridden");

        result.Should().MatchRegex(@"\b(leverage|optimize|streamline)\b",
            "because action-oriented buzzwords should appear");
        result.Should().MatchRegex(@"\b(AI-powered|data-driven|customer-centric)\b",
            "because compound buzzwords should be handled correctly");
    }

    [Fact]
    public void DolorSit_WithFixedSeedAndNonZeroChaos_IsDeterministic()
    {
        // Arrange
        var rng1 = new ChiRng(1337);
        var rng2 = new ChiRng(1337);

        // Act
        var result1 = rng1.LoremIpsum(0.25f).DolorSit(2);
        var result2 = rng2.LoremIpsum(0.25f).DolorSit(2);

        // Assert
        result1.Should().Be(result2, "because even with chaos, the same seed must produce the same 'random' text.");
    }

    [Fact]
    public void DolorSit_WithDifferentSeeds_ProducesDifferentText()
    {
        // Arrange
        var rng1 = new ChiRng(1);
        var rng2 = new ChiRng(2);

        // Act
        var result1 = rng1.LoremIpsum(0.1f).DolorSit();
        var result2 = rng2.LoremIpsum(0.1f).DolorSit();

        // Assert
        result1.Should().NotBe(result2, "because different seeds should produce different sequences of text.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void DolorSit_ReturnsCorrectNumberOfParagraphs(int paragraphCount)
    {
        // Arrange
        var rng = new ChiRng(paragraphCount);

        // Act
        var result = rng.LoremIpsum().DolorSit(paragraphCount);

        // Assert
        var paragraphs = result.Split(["\r\n\r\n", "\n\n"], StringSplitOptions.None);
        paragraphs.Should().HaveCount(paragraphCount);
    }

    [Fact]
    public void DolorSit_WithLowChaos_DiffersFromCanonical()
    {
        // Arrange
        var rng = new ChiRng("LowChaos");

        // Act
        var canonical = rng.LoremIpsum(0.0f).DolorSit();
        var chaotic = rng.LoremIpsum(0.1f).DolorSit();

        // Assert
        chaotic.Should().NotBe(canonical, "because even a small amount of chaos should alter the text.");
    }

    [Fact]
    public void DolorSit_WithHighChaos_ContainsScrambledWords()
    {
        // Arrange
        var rng = new ChiRng("HighChaos");

        // Act
        var result = rng.LoremIpsum(0.9f).DolorSit();
        var words = result.Split([' ', '.', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

        // Assert
        words.Should().Contain(w => w.Length > 2 && w.Any(char.IsLower),
            "because high chaos should produce scrambled, non-canonical words.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void DolorSit_WithInvalidParagraphCount_ThrowsArgumentOutOfRangeException(int count)
    {
        var rng = new ChiRng();
        var act = () => rng.LoremIpsum().DolorSit(count);
        act.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be("paragraphs");
    }

    [Fact]
    public void DolorSit_WithSingleCustomWord_StillProducesOutput()
    {
        // Arrange
        var rng = new ChiRng(999);

        // Act
        var result = rng.LoremIpsum(1.0f, "Pika").DolorSit();
        testOutputHelper.WriteLine(result);

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
        result.Should().Contain("pika");
        result.Should().Contain("pkia");
    }

    [Fact]
    public void DolorSit_ChaosProgression_HasGradualChanges()
    {
        var previousText = string.Empty;
        var similarities = new List<double>();

        for (var chaos = 0.0f; chaos <= 1.0f; chaos += 0.025f)
        {
            var rng = new ChiRng(42);
            var currentText = rng.LoremIpsum(chaos).DolorSit();

            if (chaos > 0.0f)
            {
                var similarity = CalculateTextSimilarity(previousText, currentText);
                similarities.Add(similarity);
                testOutputHelper.WriteLine($"Chaos {chaos:F2}: Similarity = {similarity:F3}");
            }

            previousText = currentText;
        }

        var lowChaos = similarities.Where((_, i) => i < 10).Average(); // 0.025-0.25
        var highChaos = similarities.Where((_, i) => i > 30).Average(); // 0.75-1.0

        lowChaos.Should().BeGreaterThan(0.7, "low chaos should maintain reasonable similarity to canonical");
        highChaos.Should().BeGreaterThan(0.5, "high chaos should still produce some recognizable patterns");

        var veryLowChaos = similarities.Take(5).Average(); // 0.025-0.125  
        var veryHighChaos = similarities.Skip(35).Average(); // 0.875-1.0

        veryLowChaos.Should().BeGreaterThan(veryHighChaos,
            "very low chaos should be more similar than very high chaos");

        return;

        static double CalculateTextSimilarity(string text1, string text2)
        {
            var distance = LevenshteinDistance(text1, text2);
            var maxLength = Math.Max(text1.Length, text2.Length);
            return 1.0 - (double)distance / maxLength;
        }

        static int LevenshteinDistance(string s1, string s2)
        {
            var matrix = new int[s1.Length + 1, s2.Length + 1];

            for (var i = 0; i <= s1.Length; i++) matrix[i, 0] = i;
            for (var j = 0; j <= s2.Length; j++) matrix[0, j] = j;

            for (var i = 1; i <= s1.Length; i++)
            for (var j = 1; j <= s2.Length; j++)
            {
                var cost = s1[i - 1] == s2[j - 1] ? 0 : 1;

                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, // deletion
                        matrix[i, j - 1] + 1), // insertion
                    matrix[i - 1, j - 1] + cost); // substitution
            }

            return matrix[s1.Length, s2.Length];
        }
    }

    [Fact]
    public void DolorSit_WordLimit_HasConsistentAccuracyAcrossRange()
    {
        var accuracies = new List<double>();
        var rng = new ChiRng(42);

        for (var wordLimit = 100; wordLimit >= 1; wordLimit -= 1)
        {
            var result = rng.LoremIpsum().DolorSit(1, wordLimit);
            var actualWords = CountWords(result);

            var accuracy = 1.0 - Math.Abs(actualWords - wordLimit) / (double)wordLimit;
            accuracies.Add(accuracy);

            testOutputHelper.WriteLine($"WordLimit {wordLimit}: Actual {actualWords}, Accuracy = {accuracy:F3}");
        }

        var highLimitsAccuracy = accuracies.Take(10).Average(); // 100-80 words
        var lowLimitsAccuracy = accuracies.Skip(30).Average(); // ~40-3 words

        lowLimitsAccuracy.Should().BeGreaterThan(0.7, "even small word limits should be reasonably accurate");
        Math.Abs(highLimitsAccuracy - lowLimitsAccuracy).Should().BeLessThan(0.3,
            "accuracy shouldn't degrade dramatically with smaller limits");

        return;

        static int CountWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            return text.Replace(".", "")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Length;
        }
    }
}