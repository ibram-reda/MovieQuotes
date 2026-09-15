namespace MovieQuotes.AI;
public sealed record AiGenerationResult<T>(
    bool Success,
    T? Value,
    IReadOnlyList<string> Errors,
    int Attempts);
    
public sealed class StudyMaterialAiResponse
{
    public string Content { get; set; } = "";
    public string ContentType { get; set; } = "";
    public string? PartOfSpeech { get; set; }

    public string ContentArabicTranslation { get; set; } = "";
    public string Definition { get; set; } = "";
    public string ArPhraseTranslation { get; set; } = "";

    public string? Origin { get; set; }

    public List<string> Examples { get; set; } = [];
    public List<string> Synonyms { get; set; } = [];

    public string Level { get; set; } = "";
    public string Pronunciation { get; set; } = "";

    public bool IsVulgar { get; set; }

    public string? Notes { get; set; }

    public List<string> Tags { get; set; } = [];
}