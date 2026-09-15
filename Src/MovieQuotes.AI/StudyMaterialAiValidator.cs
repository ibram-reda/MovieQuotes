namespace MovieQuotes.AI;
public static class StudyMaterialAiValidator
{
  private static readonly HashSet<string> ContentTypes =
  [
      "Word",
        "PhrasalVerb",
        "Idiom",
        "Expression",
        "Collocation",
        "Slang"
  ];

  private static readonly HashSet<string> PartsOfSpeech =
  [
      "Noun",
        "Verb",
        "Adjective",
        "Adverb",
        "Preposition",
        "Pronoun",
        "Determiner",
        "Conjunction",
        "Interjection"
  ];

  private static readonly HashSet<string> Levels =
  [
      "A1",
        "A2",
        "B1",
        "B2",
        "C1",
        "C2"
  ];

  public static List<string> Validate(
      StudyMaterialAiResponse result,
      string expectedContent)
  {
    var errors = new List<string>();

    if (string.IsNullOrWhiteSpace(result.Content))
      errors.Add("Content is required.");

    if (!string.Equals(
            result.Content,
            expectedContent,
            StringComparison.Ordinal))
    {
      errors.Add(
          $"Content must exactly equal '{expectedContent}'.");
    }

    if (!ContentTypes.Contains(result.ContentType))
      errors.Add(
          $"Invalid ContentType: '{result.ContentType}'.");

    if (result.PartOfSpeech is not null &&
        !PartsOfSpeech.Contains(result.PartOfSpeech))
    {
      errors.Add(
          $"Invalid PartOfSpeech: '{result.PartOfSpeech}'.");
    }

    if (string.IsNullOrWhiteSpace(
            result.ContentArabicTranslation))
    {
      errors.Add(
          "ContentArabicTranslation is required.");
    }

    if (string.IsNullOrWhiteSpace(result.Definition))
      errors.Add("Definition is required.");

    if (string.IsNullOrWhiteSpace(
            result.ArPhraseTranslation))
    {
      errors.Add("ArPhraseTranslation is required.");
    }

    if (!Levels.Contains(result.Level))
      errors.Add($"Invalid Level: '{result.Level}'.");

    if (result.Examples.Count is < 2 or > 3)
      errors.Add("Examples must contain 2 or 3 items.");

    if (result.Synonyms.Count > 5)
      errors.Add("Too many synonyms.");

    if (result.Tags.Count > 10)
      errors.Add("Too many tags.");

    return errors;
  }
}
