namespace MovieQuotes.Application.Features.StudyPhrases.Commands;

using MediatR;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.StudyPhrases.Models;

public class CreateStudyPhraseCommand : IRequest<OperationResult<StudyPhrase>>
{
    /// <summary>
    /// Phrase Id
    /// </summary>
    public int PhraseId { get; set; }

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
}
