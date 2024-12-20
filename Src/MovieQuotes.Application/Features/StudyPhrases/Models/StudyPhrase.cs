namespace MovieQuotes.Application.Features.StudyPhrases.Models;
 
public class StudyPhrase
{
    public int PhraseId { get; set; }
    public string PhraseText { get; set; } = string.Empty;
    /// <summary>
    /// can be one of the following
    /// (noun,verb,idiom,phrasal verb...)
    /// </summary>
    public string? StudyType { get;   set; }

    /// <summary>
    /// what you learn from this subtitle phrase.
    /// </summary>
    public string? Content { get;   set; }

    /// <summary>
    /// what is the translation of the content part.
    /// </summary>
    public string? Translation { get;   set; }

    public string VideoLocation { get; set; } = string.Empty;
}
