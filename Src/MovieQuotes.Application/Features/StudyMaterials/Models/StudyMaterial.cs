namespace MovieQuotes.Application.Features.StudyMaterials;

public class StudyMaterial
{
    public int Id { get; set; }
    public string? PartOfSpeech { get; set; }
    public string? Content { get; set; }
    public string? ContentArabicTranslation { get; set; }
    public string? Origin { get; set; }
    public string? Definition { get; set; }
    public string? ArPhraseTranslation { get; set; }
    public string? SrcPhrase { get; set; }

    public bool IsDraft { get; set; }
    public string Examples { get; set; } = string.Empty;
    public string Synonyms { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Pronunciation { get; set; } = string.Empty;
    public bool IsVulgar { get; set; }
    public string? Notes { get; set; }
    public string? Tags { get; set; }
}