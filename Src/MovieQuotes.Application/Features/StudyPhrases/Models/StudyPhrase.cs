namespace MovieQuotes.Application.Features.StudyPhrases.Models;

public class StudyPhrase
{
    public int PhraseId { get; set; }
    public int StudyId { get; set; }

    /// <summary>
    /// name of the movie that this phrase is from.
    /// </summary>
    public string MovieName { get; set; } = string.Empty;

    /// <summary>
    /// start time of the phrase in the movie.
    /// </summary>
    public TimeSpan StartTime { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// End time of the phrase in the movie.
    /// </summary>
    public TimeSpan EndTime { get; set; } = TimeSpan.Zero;

    public DateTime NextReviewDate { get; set; } = DateTime.Now;

    /// <summary>
    /// the transcript of the phrase.
    /// </summary>
    public string PhraseText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Arabic translation of the phrase.
    /// </summary>
    public string? PhraseArTranslation { get; set; } = string.Empty;


    /// <summary>
    /// can be one of the following
    /// (noun,verb,idiom,phrasal verb...)
    /// </summary>
    public string? StudyType { get; set; }

    /// <summary>
    /// what you learn from this subtitle phrase.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets the Arabic translation of the content.
    /// </summary>
    public string? ArContentTranslation { get; set; } = string.Empty;

    /// <summary>
    /// what is the English Definition of the content part.
    /// </summary>
    public string? Translation { get; set; }

    /// <summary>
    /// the location of the video clip representing this phrase.
    /// </summary>
    public string VideoLocation { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the origin of this content (optional).
    /// </summary>
    public string? Origin { get; set; }

    /// <summary>
    /// Gets or sets additional notes or comments associated with the object.
    /// </summary>
    public string? Notes { get; set; }

}
