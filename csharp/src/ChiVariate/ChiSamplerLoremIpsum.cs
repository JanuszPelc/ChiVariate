using System.Runtime.CompilerServices;
using System.Text;

namespace ChiVariate;

/// <summary>
///     Represents a sampler for generating Lorem Ipsum text using a deterministic random source.
/// </summary>
/// <typeparam name="TRng">The type of the random number generator.</typeparam>
public readonly ref struct ChiSamplerLoremIpsum<TRng>(ref TRng rng, float chaos, ReadOnlySpan<string> vocabulary)
    where TRng : struct, IChiRngSource<TRng>
{
    private readonly float _chaos = ValidateChaos(chaos);
    private readonly ReadOnlySpan<string> _vocabulary = vocabulary;
    private readonly ref TRng _rng = ref rng;
    private readonly float _averageWordsPerPhrase = CalculateAverageWordsPerPhrase(vocabulary);

    private static class ChaosPhases
    {
        public const float PhaseOneThreshold = 1.0f / 3.0f;
        public const float PhaseTwoThreshold = 2.0f / 3.0f;
        public const float PhaseThreeThreshold = 1.0f;

        public const int PhaseDriftLow = 3;
        public const int PhaseDriftHigh = 5;
        public const float ScrambleIntensity = 0.8f;
    }

    private static class PunctuationRules
    {
        public const float BaseCommaChance = 0.25f;
        public const int MinWordsBetweenCommas = 2;
        public const int MaxWordsWithoutComma = 3;
        public const int MinWordsBeforeComma = 2;
        public const int MinWordsAfterComma = 2;
        public const int OptimalCommaPosition = 3;
    }

    // ReSharper disable StaticMemberInGenericType
    private static class Defaults
    {
        public const int CanonicalParagraphSentences = 5;
        public const int RecentWordsMemory = 21;
        public const int RecentWordsMultiplier = 2;
        public const int MaxSelectionAttempts = 10;
        public const int MinTargetActualWords = 12;
        public const int MaxTargetActualWords = 25;
        public const int MinPhrasesPerSentence = 2;

        public static readonly int[] CanonicalSentenceLengths = [3, 5, 4, 4, 3];
        public static readonly Range SentenceCountRange = new(2, 4);
        public static readonly char[] WordSeparators = [' ', '-'];
    }
    // ReSharper restore StaticMemberInGenericType

    /// <summary>
    ///     Generates structured placeholder text with a tunable level of randomness.
    ///     Name is legally required to be DolorSit()
    /// </summary>
    /// <param name="paragraphs">The number of paragraphs to generate. Defaults to 1.</param>
    /// <param name="wordLimit">
    ///     Optional suggested word count per paragraph. When specified, each paragraph will contain
    ///     approximately this many words, ending at natural phrase boundaries.
    /// </param>
    /// <returns>A string containing the generated placeholder text.</returns>
    /// <remarks>
    ///     <para>
    ///         Fair warning: This sampler allocates like it's going out of style.
    ///         Definitely not for hot paths or performance-critical code!
    ///     </para>
    ///     <para>
    ///         But seriously: This placeholder text generator is fully deterministic when seeded.
    ///         That makes it ideal for reproducible tests, UI snapshots, or procedural content generation.
    ///     </para>
    /// </remarks>
    public string DolorSit(int paragraphs = 1, int? wordLimit = null)
    {
        ValidateParagraphs(paragraphs);
        if (wordLimit.HasValue)
            ValidateWordLimit(wordLimit.Value);
        return GenerateText(paragraphs, wordLimit);
    }

    private static void ValidateParagraphs(int paragraphs)
    {
        if (paragraphs < 1)
            throw new ArgumentOutOfRangeException(nameof(paragraphs), "Must generate at least 1 paragraph");
    }

    private static void ValidateWordLimit(int wordLimit)
    {
        if (wordLimit < 1)
            throw new ArgumentOutOfRangeException(nameof(wordLimit), "Word limit per paragraph must be at least 1");
    }

    private static float ValidateChaos(float chaos)
    {
        if (chaos is < 0.0f or > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(chaos), "Chaos must be between 0.0 and 1.0");

        return chaos;
    }

    private string GenerateText(int paragraphs, int? wordLimit)
    {
        var result = new StringBuilder();
        var recentWords = new Queue<string>();
        var globalWordPosition = 0;

        for (var paragraphIndex = 0; paragraphIndex < paragraphs; paragraphIndex++)
        {
            if (paragraphIndex > 0)
                result.AppendLine().AppendLine();

            var paragraph = GenerateParagraph(recentWords, ref globalWordPosition, wordLimit);
            result.Append(paragraph);
        }

        return result.ToString();
    }

    private string GenerateParagraph(Queue<string> recentWords, ref int globalWordPosition, int? wordLimit)
    {
        if (wordLimit.HasValue)
            return GenerateConstrainedParagraph(recentWords, ref globalWordPosition, wordLimit.Value);

        var sentenceCount = DetermineSentenceCount();
        var result = new StringBuilder();

        for (var sentenceIndex = 0; sentenceIndex < sentenceCount; sentenceIndex++)
        {
            if (sentenceIndex > 0)
                result.Append(' ');

            var sentence = GenerateSentence(recentWords, sentenceIndex, ref globalWordPosition);
            result.Append(sentence);
        }

        return result.ToString();
    }

    private int DetermineSentenceCount()
    {
        return _chaos == 0.0f
            ? Defaults.CanonicalParagraphSentences
            : _rng.Chance().PickBetween(Defaults.SentenceCountRange.Start.Value, Defaults.SentenceCountRange.End.Value);
    }

    private string GenerateSentence(Queue<string> recentWords, int sentenceIndex, ref int globalWordPosition)
    {
        var wordCount = DetermineWordCount(sentenceIndex);
        var result = new StringBuilder();
        var lastCommaPosition = -1;

        for (var wordIndex = 0; wordIndex < wordCount; wordIndex++)
        {
            if (wordIndex > 0)
                result.Append(' ');

            var selectedWord = SelectWord(_vocabulary, recentWords, globalWordPosition);
            var punctuatedWord = ApplyPunctuation(selectedWord, wordIndex, wordCount, lastCommaPosition);
            var capitalizedWord = ApplyCapitalization(punctuatedWord, wordIndex == 0);

            result.Append(capitalizedWord);

            if (punctuatedWord.EndsWith(','))
                lastCommaPosition = wordIndex;

            if (_chaos > 0.0f)
                TrackWordUsage(capitalizedWord, recentWords);

            globalWordPosition++;
        }

        result.Append('.');
        return result.ToString();
    }

    private int DetermineWordCount(int sentenceIndex)
    {
        var targetActualWords = _chaos == 0.0f
            ? Defaults.CanonicalSentenceLengths[sentenceIndex % Defaults.CanonicalSentenceLengths.Length] *
              _averageWordsPerPhrase
            : _rng.Chance().PickBetween(Defaults.MinTargetActualWords, Defaults.MaxTargetActualWords);

        return Math.Max(Defaults.MinPhrasesPerSentence, (int)(targetActualWords / _averageWordsPerPhrase));
    }

    private string SelectWord(ReadOnlySpan<string> vocabulary, Queue<string> recentWords, int globalPosition)
    {
        var canonicalWord = vocabulary[globalPosition % vocabulary.Length];

        string selectedWord;

        if (_chaos == 0.0f || _rng.Chance().NextSingle() >= _chaos)
        {
            selectedWord = canonicalWord;
        }
        else
        {
            selectedWord = null!;
            for (var attempt = 0; attempt < Defaults.MaxSelectionAttempts; attempt++)
            {
                var candidate = GenerateCandidateWord(vocabulary, globalPosition);
                if (!IsWordAcceptable(candidate, recentWords))
                    continue;

                selectedWord = candidate;
                break;
            }

            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            selectedWord ??= canonicalWord;
        }

        if (!(_chaos > 0.0f) || IsWordAcceptable(selectedWord, recentWords))
            return selectedWord;

        foreach (var alternative in vocabulary)
            if (IsWordAcceptable(alternative, recentWords))
                return alternative;

        return selectedWord;
    }

    private static string ApplyPunctuation(string word, int position, int totalWords, int lastCommaPosition)
    {
        if (ShouldAddComma(position, totalWords, lastCommaPosition))
            return word + ",";
        return word;
    }

    private static bool ShouldAddComma(int position, int totalWords, int lastCommaPosition)
    {
        if (position < PunctuationRules.MinWordsBeforeComma)
            return false;

        if (position >= totalWords - PunctuationRules.MinWordsAfterComma)
            return false;

        if (lastCommaPosition >= 0 && position - lastCommaPosition < PunctuationRules.MinWordsBetweenCommas)
            return false;

        var wordsSinceLastComma = lastCommaPosition >= 0 ? position - lastCommaPosition : position;
        if (wordsSinceLastComma >= PunctuationRules.MaxWordsWithoutComma)
        {
            var wordsRemaining = totalWords - position;
            if (wordsRemaining > PunctuationRules.MinWordsAfterComma)
                return true;
        }

        var distanceFromOptimal = Math.Abs(position - PunctuationRules.OptimalCommaPosition);
        var positionBonus = Math.Max(0, 1.0f - distanceFromOptimal * 0.1f);
        var commaChance = PunctuationRules.BaseCommaChance * (1.0f + positionBonus);

        var hash = position * 31 + totalWords * 17;
        var normalizedHash = hash % 100 / 100.0f;

        return normalizedHash < commaChance;
    }

    private string GenerateCandidateWord(ReadOnlySpan<string> vocabulary, int position)
    {
        return _chaos switch
        {
            < ChaosPhases.PhaseOneThreshold => SelectFromPhaseOne(vocabulary, position),
            < ChaosPhases.PhaseTwoThreshold => SelectFromPhaseTwo(vocabulary, position),
            _ => SelectFromPhaseThree(vocabulary)
        };
    }

    private string SelectFromPhaseOne(ReadOnlySpan<string> vocabulary, int position)
    {
        if (_rng.Chance().NextSingle() < _chaos)
            return vocabulary[_rng.Chance().Next(vocabulary.Length)];

        var baseIndex = position % vocabulary.Length;
        var drift = (int)(_chaos * ChaosPhases.PhaseDriftLow);
        var offset = _rng.Chance().Next(-drift, drift + 1);
        var finalIndex = (baseIndex + offset + vocabulary.Length) % vocabulary.Length;

        return vocabulary[finalIndex];
    }

    private string SelectFromPhaseTwo(ReadOnlySpan<string> vocabulary, int position)
    {
        var normalizedChaos = (_chaos - ChaosPhases.PhaseOneThreshold) /
                              (ChaosPhases.PhaseTwoThreshold - ChaosPhases.PhaseOneThreshold);

        if (_rng.Chance().NextSingle() < normalizedChaos)
            return vocabulary[_rng.Chance().Next(vocabulary.Length)];

        var baseIndex = position % vocabulary.Length;
        var drift = (int)(normalizedChaos * ChaosPhases.PhaseDriftHigh);
        var offset = _rng.Chance().Next(-drift, drift + 1);
        var finalIndex = (baseIndex + offset + vocabulary.Length) % vocabulary.Length;

        return vocabulary[finalIndex];
    }

    private string SelectFromPhaseThree(ReadOnlySpan<string> vocabulary)
    {
        var selectedWord = vocabulary[_rng.Chance().Next(vocabulary.Length)];
        var normalizedChaos = (_chaos - ChaosPhases.PhaseTwoThreshold) /
                              (ChaosPhases.PhaseThreeThreshold - ChaosPhases.PhaseTwoThreshold);

        return _rng.Chance().NextSingle() < normalizedChaos * ChaosPhases.ScrambleIntensity
            ? ScrambleWord(selectedWord)
            : selectedWord;
    }

    private static bool IsWordAcceptable(string candidate, Queue<string> recentWords)
    {
        var individualWords = candidate.Split(Defaults.WordSeparators, StringSplitOptions.RemoveEmptyEntries)
            .Select(word => word.ToLowerInvariant());

        var hasRecentIndividualWord = individualWords.Any(recentWords.Contains);
        var hasRecentPhrase = recentWords.Contains(candidate.ToLowerInvariant());

        return !hasRecentIndividualWord && !hasRecentPhrase;
    }

    private static void TrackWordUsage(string word, Queue<string> recentWords)
    {
        recentWords.Enqueue(word.ToLowerInvariant());

        var individualWords = word.Split(Defaults.WordSeparators, StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.ToLowerInvariant());

        foreach (var individualWord in individualWords)
            recentWords.Enqueue(individualWord);

        const int maxQueueSize = Defaults.RecentWordsMemory * Defaults.RecentWordsMultiplier;
        while (recentWords.Count > maxQueueSize)
            recentWords.Dequeue();
    }

    private static string ApplyCapitalization(string word, bool isFirstWord)
    {
        if (string.IsNullOrEmpty(word))
            return word;

        var words = word.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        for (var i = 0; i < words.Length; i++)
            words[i] = ShouldPreserveCaps(words[i])
                ? words[i]
                : ApplyStandardCapitalization(words[i], isFirstWord && i == 0);

        return string.Join(" ", words);
    }

    private static string ApplyStandardCapitalization(string word, bool shouldCapitalize)
    {
        return shouldCapitalize
            ? char.ToUpperInvariant(word[0]) + word.Substring(1).ToLowerInvariant()
            : word.ToLowerInvariant();
    }

    private static bool ShouldPreserveCaps(string word)
    {
        if (string.IsNullOrEmpty(word))
            return false;

        var uppercaseCount = word.Count(char.IsUpper);
        var letterCount = word.Count(char.IsLetter);

        return letterCount > 0 && (uppercaseCount == letterCount || uppercaseCount >= 2);
    }

    private string ScrambleWord(string word)
    {
        if (word.Length <= 2)
            return word;

        var chars = word.ToCharArray();

        for (var i = 1; i < chars.Length - 1; i++)
        {
            var swapIndex = _rng.Chance().Next(1, chars.Length - 1);
            (chars[i], chars[swapIndex]) = (chars[swapIndex], chars[i]);
        }

        return new string(chars);
    }

    private string GenerateConstrainedParagraph(Queue<string> recentWords, ref int globalWordPosition, int targetWords)
    {
        var result = new StringBuilder();
        var sentenceIndex = 0;
        var currentWordCount = 0;

        while (currentWordCount < targetWords)
        {
            if (sentenceIndex > 0)
                result.Append(' ');

            // Generate phrases one by one until we approach the target word count
            var phrases = new List<string>();
            var phraseWordCount = 0;

            while (phraseWordCount < targetWords - currentWordCount)
            {
                var selectedPhrase = SelectWord(_vocabulary, recentWords, globalWordPosition);
                var capitalizedPhrase =
                    ApplyCapitalization(selectedPhrase, phrases.Count == 0); // Skip punctuation for constrained mode

                var wordsInThisPhrase = CountWordsInText(capitalizedPhrase);

                // Check if adding this phrase would overshoot significantly
                if (phraseWordCount > 0 && phraseWordCount + wordsInThisPhrase > targetWords - currentWordCount + 2)
                    break;

                phrases.Add(capitalizedPhrase);
                phraseWordCount += wordsInThisPhrase;

                if (_chaos > 0.0f)
                    TrackWordUsage(capitalizedPhrase, recentWords);

                globalWordPosition++;
            }

            // Join phrases into a sentence and add period
            if (phrases.Count > 0)
            {
                var sentence = string.Join(" ", phrases) + ".";
                result.Append(sentence);
                currentWordCount += phraseWordCount;
            }

            sentenceIndex++;

            // Safety break to avoid infinite loop
            if (sentenceIndex > 10 || phrases.Count == 0)
                break;
        }

        return result.ToString();
    }

    private static int CountWordsInText(string text)
    {
        return text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private static float CalculateAverageWordsPerPhrase(ReadOnlySpan<string> vocabulary)
    {
        var totalWords = 0;
        foreach (var phrase in vocabulary)
            totalWords += phrase.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        return vocabulary.Length > 0 ? (float)totalWords / vocabulary.Length : 1.0f;
    }
}

/// <summary>
///     Provides the extension method for accessing the Lorem Ipsum generator.
/// </summary>
public static class ChiSamplerLoremIpsumExtensions
{
    /// <summary>
    ///     A "secret" feature that started as a joke and proved surprisingly useful.
    /// </summary>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="chaos">
    ///     A value from 0.0 to 1.0 controlling the level of randomness:
    ///     0.0 produces canonical text following the original sequence;
    ///     0.5 creates interesting, realistic variations (default);
    ///     1.0 results in full word salad with scrambled words.
    /// </param>
    /// <param name="vocabulary">
    ///     Optional comma-separated vocabulary list to generate custom-themed Ipsum.
    ///     Leave null for classic "Lorem Ipsum", or supply terms for a "Corporate Buzzword Ipsum",
    ///     "Sci-Fi Technobabble", or "Unit Test Mock Data".
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerLoremIpsum<TRng> LoremIpsum<TRng>(
        ref this TRng rng, float chaos = 0.5f, string? vocabulary = null)
        where TRng : struct, IChiRngSource<TRng>
    {
        ReadOnlySpan<string> vocabularySpan = string.IsNullOrWhiteSpace(vocabulary)
            ? CanonicalWords
            : vocabulary
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

        return new ChiSamplerLoremIpsum<TRng>(ref rng, chaos, vocabularySpan);
    }

    private static string[] CanonicalWords =>
    [
        // ReSharper disable StringLiteralTypo
        "Lorem", "ipsum", "dolor sit amet", "consectetur adipiscing elit",
        "sed do", "eiusmod tempor", "incididunt ut", "labore et dolore",
        "magna aliqua", "Ut enim", "ad minim veniam", "quis nostrud",
        "exercitation ullamco", "laboris nisi ut", "aliquip ex ea", "commodo consequat",
        "Duis aute", "irure dolor", "in reprehenderit", "in voluptate",
        "velit esse", "cillum dolore eu", "fugiat nulla pariatur", "Excepteur sint",
        "occaecat cupidatat", "non proident", "sunt in culpa", "qui officia",
        "deserunt mollit", "anim id", "est laborum"
        // ReSharper enable StringLiteralTypo
    ];
}