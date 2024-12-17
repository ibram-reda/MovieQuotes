namespace MovieQuotes.Application.Operations.Commands;

using MediatR;
using MovieQuotes.Application.Models;
using MovieQuotes.Domain.Models;

public class AddStudyContentCommand : IRequest<OperationResult<bool>>
{
    public int PhraseId { get; set; }
    public SubtitlePhrase? Phrase { get; set; }

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
