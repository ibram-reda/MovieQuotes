namespace MovieQuotes.Application.Features.Study;

public class StudyPhrase
{
    public int ProgressId {get;set;}
    public int StudyCardId { get; set; }

    /// <summary>
    /// the transcript of the phrase.
    /// </summary>
    public string PhraseText { get; set; } = string.Empty;
    /// <summary>
    /// the location of the video clip representing this phrase.
    /// </summary>
    public string VideoPath { get; set; } = string.Empty;

    /// <summary>
    /// what you learn from this subtitle phrase.
    /// </summary>
    public string? Content { get; set; }


    /// <summary>
    /// what is the English Definition of the content part.
    /// </summary>
    public string? Definition { get; set; }
    public string Examples { get; set; } = string.Empty;

    public string Synonyms { get; set; } = string.Empty;


    /// <summary>
    /// can be one of the following
    /// (noun,verb,idiom,phrasal verb...)
    /// </summary>
    public string? StudyType { get; set; }

    /// <summary>
    /// Gets or sets the origin of this content (optional).
    /// </summary>
    public string? Origin { get; set; }

    /// <summary>
    /// Gets or sets additional notes or comments associated with the object.
    /// </summary>
    public string? Notes { get; set; }

    public string Level { get; set; } = string.Empty;
    public string Pronunciation { get; set; } = string.Empty;

}