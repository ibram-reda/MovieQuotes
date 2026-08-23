namespace MovieQuotes.Application.Features.Study;

public class StudyMaterialPhrase
{
    public int PhraseId { get; set; }
    // TODO: Add Content to hilight in phrse;
    public string PhraseText { get; set; } = string.Empty;
    public string ArPhraseTranslation { get; set; } = string.Empty;
    public string MovieCoverUrl { get; set; } = string.Empty;
    public string MovieTitle {get;set;}=string.Empty;
    public int Sequance { get; set; }
}

public class StudyMaterialDetails
{
    public int Id { get; set; }
    public string? PartOfSpeech { get; set; }
    public string? Content { get; set; }
    public string? ContentArabicTranslation { get; set; }
    public string? Origin { get; set; }
    public string? Definition { get; set; }

    public List<StudyMaterialPhrase> MaterialPhrases { get; set; } = [];
    public bool IsDraft { get; set; }
    public string Examples { get; set; } = string.Empty;
    public string Synonyms { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Pronunciation { get; set; } = string.Empty;
    public bool IsVulgar { get; set; }
    public string? Notes { get; set; }
    public string? Tags { get; set; }
}