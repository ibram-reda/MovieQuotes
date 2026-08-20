namespace MovieQuotes.UI.Services;

using System;
using System.Collections.Generic;
using System.Linq;

public sealed class SpellingErrorAnalyzer
{
    private static readonly HashSet<char> Vowels =
    ['a', 'e', 'i', 'o', 'u'];

    public SpellingAnalysis Analyze(
        string expected,
        string actual)
    {
        ArgumentNullException.ThrowIfNull(expected);
        ArgumentNullException.ThrowIfNull(actual);

        expected = Normalize(expected);
        actual = Normalize(actual);

        if (expected == actual)
        {
            return new SpellingAnalysis(
                Expected: expected,
                Actual: actual,
                IsCorrect: true,
                EditDistance: 0,
                Characters: CreateCorrectCharacters(expected),
                Changes: [],
                Errors: []);
        }

        var alignment = BuildAlignment(expected, actual);

        var errors = ClassifyErrors(expected, actual, alignment);

        return new SpellingAnalysis(
            Expected: expected,
            Actual: actual,
            IsCorrect: false,
            EditDistance: alignment.Distance,
            Characters: alignment.Characters,
            Changes: alignment.Changes,
            Errors: errors);
    }

    private static string Normalize(string value)
     => value.Trim().ToLowerInvariant();

    private static IReadOnlyList<SpellingCharacter>
        CreateCorrectCharacters(string word)
    {
        return word
            .Select(c =>
                new SpellingCharacter(
                    Expected: c,
                    Actual: c,
                    State: SpellingCharacterState.Correct))
            .ToList();
    }

    private static AlignmentResult BuildAlignment(
        string expected,
        string actual)
    {
        var matrix = BuildMatrix(expected, actual);

        var result = Backtrack(expected, actual, matrix);

        return new AlignmentResult(
            Distance: matrix[expected.Length, actual.Length],
            Characters: result.Characters,
            Changes: result.Changes);
    }

    private static int[,] BuildMatrix(
        string expected,
        string actual)
    {
        var rows = expected.Length + 1;
        var columns = actual.Length + 1;

        var matrix = new int[rows, columns];

        // Empty expected -> actual.
        for (var j = 0; j < columns; j++)
            matrix[0, j] = j;

        // Expected -> empty actual.
        for (var i = 0; i < rows; i++)
            matrix[i, 0] = i;

        for (var i = 1; i < rows; i++)
        {
            for (var j = 1; j < columns; j++)
            {
                var substitutionCost =
                    expected[i - 1] == actual[j - 1]
                        ? 0
                        : 1;

                var deletion =
                    matrix[i - 1, j] + 1;

                var insertion =
                    matrix[i, j - 1] + 1;

                var substitution =
                    matrix[i - 1, j - 1]
                    + substitutionCost;

                matrix[i, j] = Math.Min(
                    Math.Min(deletion, insertion), substitution);
            }
        }

        return matrix;
    }

    private static AlignmentBacktrackResult Backtrack(
        string expected,
        string actual,
        int[,] matrix)
    {
        var characters = new List<SpellingCharacter>();
        var changes = new List<SpellingChange>();

        var i = expected.Length;
        var j = actual.Length;

        while (i > 0 || j > 0)
        {
            // --------------------------------------------------
            // Correct character
            // --------------------------------------------------

            if (i > 0 &&
                j > 0 &&
                expected[i - 1] == actual[j - 1] &&
                matrix[i, j] == matrix[i - 1, j - 1])
            {
                characters.Add(
                    new SpellingCharacter(
                        Expected: expected[i - 1],
                        Actual: actual[j - 1],
                        State: SpellingCharacterState.Correct));

                i--;
                j--;
                continue;
            }

            // --------------------------------------------------
            // Substitution
            // --------------------------------------------------

            if (i > 0 &&
                j > 0 &&
                matrix[i, j] == matrix[i - 1, j - 1] + 1)
            {
                var expectedChar = expected[i - 1];
                var actualChar = actual[j - 1];

                characters.Add(
                    new SpellingCharacter(
                        Expected: expectedChar,
                        Actual: actualChar,
                        State: SpellingCharacterState.Substituted));

                changes.Add(
                    new SpellingChange(
                        ExpectedIndex: i - 1,
                        ActualIndex: j - 1,
                        Expected: expectedChar.ToString(),
                        Actual: actualChar.ToString()));

                i--;
                j--;

                continue;
            }

            // --------------------------------------------------
            // Missing character
            //
            // Exists in expected but not in actual.
            // --------------------------------------------------

            if (i > 0 &&
                matrix[i, j] == matrix[i - 1, j] + 1)
            {
                var expectedChar = expected[i - 1];

                characters.Add(
                    new SpellingCharacter(
                        Expected: expectedChar,
                        Actual: null,
                        State: SpellingCharacterState.Missing));

                changes.Add(
                    new SpellingChange(
                        ExpectedIndex: i - 1,
                        ActualIndex: j,
                        Expected: expectedChar.ToString(),
                        Actual: string.Empty));

                i--;

                continue;
            }

            // --------------------------------------------------
            // Extra character
            //
            // Exists in actual but not in expected.
            // --------------------------------------------------

            if (j > 0 &&
                matrix[i, j] == matrix[i, j - 1] + 1)
            {
                var actualChar = actual[j - 1];

                characters.Add(
                    new SpellingCharacter(
                        Expected: null,
                        Actual: actualChar,
                        State: SpellingCharacterState.Extra));

                changes.Add(
                    new SpellingChange(
                        ExpectedIndex: i,
                        ActualIndex: j - 1,
                        Expected: string.Empty,
                        Actual: actualChar.ToString()));

                j--;

                continue;
            }

            throw new InvalidOperationException(  "Unable to reconstruct spelling alignment.");
        }

        characters.Reverse();
        changes.Reverse();

        return new AlignmentBacktrackResult(
            Characters: characters,
            Changes: changes);
    }

    private static List<SpellingError> ClassifyErrors(
        string expected,
        string actual,
        AlignmentResult alignment)
    {
        var errors = new List<SpellingError>();

        // --------------------------------------------------
        // Transposition
        //
        // Example:
        //
        // receive
        // recieve
        //    ^^
        // --------------------------------------------------

        if (IsSimpleTransposition(
            expected,
            actual,
            out var transpositionIndex))
        {
            var expectedText =  
                expected.Substring( transpositionIndex, 2);

            var actualText =
                actual.Substring( transpositionIndex, 2);

            errors.Add(
                new SpellingError(
                    Type:
                        SpellingErrorType.Transposition,

                    ExpectedIndex:
                        transpositionIndex,

                    ActualIndex:
                        transpositionIndex,

                    ExpectedText:
                        expectedText,

                    ActualText:
                        actualText));
        }

        // --------------------------------------------------
        // Individual changes
        // --------------------------------------------------

        foreach (var change in alignment.Changes)
        {
            // Missing
            if (change.Expected.Length == 1 &&
                change.Actual.Length == 0)
            {
                errors.Add(
                    new SpellingError(
                        Type:
                            SpellingErrorType.MissingLetter,

                        ExpectedIndex:
                            change.ExpectedIndex,

                        ActualIndex:
                            change.ActualIndex,

                        ExpectedText:
                            change.Expected,

                        ActualText:
                            string.Empty));

                continue;
            }

            // Extra
            if (change.Expected.Length == 0 &&
                change.Actual.Length == 1)
            {
                errors.Add(
                    new SpellingError(
                        Type:
                            SpellingErrorType.ExtraLetter,

                        ExpectedIndex:
                            change.ExpectedIndex,

                        ActualIndex:
                            change.ActualIndex,

                        ExpectedText:
                            string.Empty,

                        ActualText:
                            change.Actual));

                continue;
            }

            // Substitution
            if (change.Expected.Length == 1 &&
                change.Actual.Length == 1)
            {
                var expectedChar =
                    change.Expected[0];

                var actualChar =
                    change.Actual[0];

                if (IsVowel(expectedChar) &&
                    IsVowel(actualChar))
                {
                    errors.Add(
                        new SpellingError(
                            Type:
                                SpellingErrorType.VowelError,

                            ExpectedIndex:
                                change.ExpectedIndex,

                            ActualIndex:
                                change.ActualIndex,

                            ExpectedText:
                                change.Expected,

                            ActualText:
                                change.Actual));
                }
                else
                {
                    errors.Add(
                        new SpellingError(
                            Type:
                                SpellingErrorType.Substitution,

                            ExpectedIndex:
                                change.ExpectedIndex,

                            ActualIndex:
                                change.ActualIndex,

                            ExpectedText:
                                change.Expected,

                            ActualText:
                                change.Actual));
                }
            }
        }

        // --------------------------------------------------
        // Internal sequence
        // --------------------------------------------------

        var substitutions =
            alignment.Changes
                .Where(change =>
                    change.Expected.Length == 1 &&
                    change.Actual.Length == 1)
                .OrderBy(change =>
                    change.ExpectedIndex)
                .ToList();

        if (substitutions.Count >= 2 &&
            AreInternallyGrouped(substitutions))
        {
            var first =
                substitutions.First();

            var last =
                substitutions.Last();

            var expectedStart =
                first.ExpectedIndex;

            var expectedLength =
                last.ExpectedIndex -
                first.ExpectedIndex +
                1;

            var actualStart =
                first.ActualIndex;

            var actualLength =
                last.ActualIndex -
                first.ActualIndex +
                1;

            var expectedSequence =
                expected.Substring(
                    expectedStart,
                    expectedLength);

            var actualSequence =
                actual.Substring(
                    actualStart,
                    actualLength);

            errors.Add(
                new SpellingError(
                    Type:
                        SpellingErrorType.InternalSequence,

                    ExpectedIndex:
                        expectedStart,

                    ActualIndex:
                        actualStart,

                    ExpectedText:
                        expectedSequence,

                    ActualText:
                        actualSequence));
        }

        // --------------------------------------------------
        // Multiple errors
        // --------------------------------------------------

        var primaryErrorTypes =
            errors
                .Where(error =>
                    error.Type !=
                        SpellingErrorType.VowelError &&
                    error.Type !=
                        SpellingErrorType.InternalSequence)
                .Select(error => error.Type)
                .Distinct()
                .ToList();

        if (primaryErrorTypes.Count > 1)
        {
            errors.Add(
                new SpellingError(
                    Type:
                        SpellingErrorType.MultipleErrors,

                    ExpectedIndex: -1,
                    ActualIndex: -1,

                    ExpectedText:
                        expected,

                    ActualText:
                        actual));
        }

        return errors;
    }

    private static bool IsSimpleTransposition(
        string expected,
        string actual,
        out int index)
    {
        index = -1;

        if (expected.Length != actual.Length)
            return false;

        var differences =
            new List<int>();

        for (var i = 0;
             i < expected.Length;
             i++)
        {
            if (expected[i] != actual[i])
                differences.Add(i);
        }

        if (differences.Count != 2)
            return false;

        var first =
            differences[0];

        var second =
            differences[1];

        if (second != first + 1)
            return false;

        if (expected[first] != actual[second])
            return false;

        if (expected[second] != actual[first])
            return false;

        index = first;

        return true;
    }

    private static bool AreInternallyGrouped(
        IReadOnlyList<SpellingChange> changes)
    {
        if (changes.Count < 2)
            return false;

        for (var i = 1;
             i < changes.Count;
             i++)
        {
            var previous =
                changes[i - 1];

            var current =
                changes[i];

            if (current.ExpectedIndex >
                previous.ExpectedIndex + 1)
            {
                return false;
            }

            if (current.ActualIndex >
                previous.ActualIndex + 1)
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsVowel(char character)
    {
        return Vowels.Contains(
            char.ToLowerInvariant(character));
    }

    private sealed record AlignmentResult(
        int Distance,
        IReadOnlyList<SpellingCharacter> Characters,
        IReadOnlyList<SpellingChange> Changes);

    private sealed record AlignmentBacktrackResult(
        IReadOnlyList<SpellingCharacter> Characters,
        IReadOnlyList<SpellingChange> Changes);
}