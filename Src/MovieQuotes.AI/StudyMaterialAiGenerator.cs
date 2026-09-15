namespace MovieQuotes.AI;

using System.Text.Json;
using Microsoft.Extensions.AI;

public sealed class StudyMaterialAiGenerator
{
  private readonly IChatClient _chatClient;

  public StudyMaterialAiGenerator(IChatClient chatClient)
  {
    _chatClient = chatClient;
  }

  public StudyMaterialAiGenerator()
  {
  }

  private static readonly JsonSerializerOptions JsonOptions = new()
  {
    PropertyNameCaseInsensitive = true,
    AllowTrailingCommas = true,
    ReadCommentHandling = JsonCommentHandling.Disallow
  };
  public async Task<AiGenerationResult<StudyMaterialAiResponse>>
      GenerateAsync(
          string phrase,
          string content,
          CancellationToken cancellationToken = default)
  {
    if (!phrase.Contains(
        content,
        StringComparison.OrdinalIgnoreCase))
    {
      throw new ArgumentException(
          "Content does not occur in the phrase.");
    }
    const int maxAttempts = 3;

    var systemPrompt = BuildSystemPrompt();
    var userPrompt = BuildUserPrompt(phrase, content);

    var errors = new List<string>();

    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
      try
      {
        var attempet = BuildAttemptPrompt(errors);
        var response = await _chatClient.GetResponseAsync<StudyMaterialAiResponse>(
          [
            new (ChatRole.System, systemPrompt),
            new(ChatRole.User, userPrompt),
          //  new(ChatRole.Assistant,attempet)

          ],
            cancellationToken: cancellationToken);



        var result = response.Result;

        if (result is null)
        {
          errors.Add("AI returned null JSON.");
          continue;
        }

        var validationErrors =
            StudyMaterialAiValidator.Validate(result, content);

        if (validationErrors.Count == 0)
        {
          return new AiGenerationResult<
              StudyMaterialAiResponse>(
              true,
              result,
              [],
              attempt);
        }

        errors.AddRange(validationErrors);
      }
      catch (JsonException ex)
      {
        errors.Add(
            $"Invalid JSON: {ex.Message}");
      }
      catch (Exception ex)
      {
        errors.Add(
            $"AI request failed: {ex.Message}");
      }
    }

    return new AiGenerationResult<StudyMaterialAiResponse>(
        false,
        null,
        errors,
        maxAttempts);
  }


  private static string BuildAttemptPrompt(IReadOnlyList<string> previousErrors)
  {
    if (previousErrors.Count == 0) return "";
    return $"""
        Your previous response was invalid.

        Validation errors:
        {string.Join("\n", previousErrors)}

        Generate the complete corrected JSON again.

        IMPORTANT:
        Return ONLY the corrected JSON.
        """;
  }
  private static string BuildSystemPrompt()
  {
    return """
        You are the AI English material generator.

        Your job is to analyze an English phrase from Movies and generate
        learning material for the specified target content.

        STRICT RULES:

        1. Return ONLY valid JSON.
        2. Do not use Markdown.
        3. Do not wrap the JSON in ```json.
        4. Do not add any text before or after the JSON.
        5. All JSON strings must use valid JSON escaping.
        6. Never return trailing commas.
        7. Use double quotes for JSON property names and strings.
        8. The response must match the requested schema exactly.
        9. Do not invent information when it is unknown. Use null where allowed.
        10. The "content" field must contain exactly the requested target content.

        CONTENT TYPE must be one of:
        - Word
        - PhrasalVerb
        - Idiom
        - Expression
        - Collocation
        - Slang

        PART OF SPEECH must be one of:
        - Noun
        - Verb
        - Adjective
        - Adverb
        - Preposition
        - Pronoun
        - Determiner
        - Conjunction
        - Interjection
        - null

        LEVEL must be one of:
        - A1
        - A2
        - B1
        - B2
        - C1
        - C2

        EXAMPLES:
        - Generate 2 or 3 examples.
        - Examples must be natural English.
        - Examples should demonstrate the same meaning as the target content.

        Origin:
        - origin is the Infinitive verb if the content is a verb
        - singular noun if the content is a plural noun
        - null otherwise

        TRANSLATIONS:
        - Translate naturally.
        - Do not add explanations to translations.

        DEFINITION:
        - Give a concise learner-friendly English definition.
        - Explain the meaning used in the provided phrase.

        NOTES:
        - Explain important contextual meaning or usage.
        - Keep it concise.

        Return exactly this JSON structure:

        {
          "content": "string",
          "contentType": "string",
          "partOfSpeech": "string",
          "contentArabicTranslation": "string",
          "definition": "string",
          "arPhraseTranslation": "string",
          "origin": "string | null",
          "examples": ["string", "string"],
          "synonyms": ["string", "string"],
          "level": "B2",
          "pronunciation": "string",
          "isVulgar": false,
          "notes": "string",
          "tags": ["string", "string"]
        }
        """;
  }
  private static string BuildUserPrompt(
    string phrase,
    string content)
  {
    return $"""
        PHRASE:
        {phrase}

        TARGET CONTENT:
        {content}
        """;
  }

}