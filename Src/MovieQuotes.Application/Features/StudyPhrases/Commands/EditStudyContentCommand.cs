namespace MovieQuotes.Application.Features.StudyPhrases.Commands;

using MediatR;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.StudyPhrases.Models;

public class EditStudyContentCommand : IRequest<OperationResult<StudyPhrase>>
{
    /// <summary>
    /// study Id
    /// </summary>
    public int StudyId { get; set; }
    /// <summary>
    /// can be one of the following
    /// (Word,Idiom,PhrasalVerb...)
    /// </summary>
    public string? StudyType { get; set; }
    /// <summary>
    /// what you learn from this subtitle phrase.
    /// </summary>
    public string? Content { get; set; }
    /// <summary>
    /// what is the translation of the content part.
    /// </summary>
    public string? Translation { get; set; }

    /// <summary>
    /// Gets or sets the Arabic translation of the content.
    /// </summary>
    public string? ArContentTranslation { get; set; }


    /// <summary>
    /// Gets or sets the Arabic translation for the associated Phrase.
    /// </summary>
    public string? ArPhraseTranslation { get; set; }


    public string? Origin { get; set; }
    public string? Notes { get; set; }

    public bool IsDraft { get; set; } = false;

    public string Examples { get; set; } = string.Empty;

    public string Synonyms {get;set;} = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Pronunciation { get; set; } = string.Empty;

}
