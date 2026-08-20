namespace MovieQuotes.UI.Services;

using System.Collections.Generic;


public enum SpellingErrorType
{
    Correct,
    MissingLetter,
    ExtraLetter,
    Substitution,
    VowelError,
    Transposition,
    InternalSequence,
    MultipleErrors
}

public enum SpellingCharacterState
{
    Correct,
    Missing,
    Extra,
    Substituted
}

public sealed record SpellingCharacter(
    char? Expected,
    char? Actual,
    SpellingCharacterState State);

public sealed record SpellingChange(
    int ExpectedIndex,
    int ActualIndex,
    string Expected,
    string Actual);

public sealed record SpellingError(
    SpellingErrorType Type,
    int ExpectedIndex,
    int ActualIndex,
    string ExpectedText,
    string ActualText);

public sealed record SpellingAnalysis(
    string Expected,
    string Actual,
    bool IsCorrect,
    int EditDistance,
    IReadOnlyList<SpellingCharacter> Characters,
    IReadOnlyList<SpellingChange> Changes,
    IReadOnlyList<SpellingError> Errors);