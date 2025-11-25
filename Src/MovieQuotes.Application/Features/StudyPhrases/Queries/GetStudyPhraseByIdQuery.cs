namespace MovieQuotes.Application.Features.StudyPhrases.Queries;

using MediatR;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.StudyPhrases.Models;


/// <summary>
/// Represents a request to retrieve a study phrase by its unique study identifier.
/// </summary>
/// <remarks>Use this query to obtain a specific <see cref="StudyPhrase"/> associated with the given study ID.
/// This type is typically used with a mediator pattern to encapsulate the retrieval operation.</remarks>
public class GetStudyPhraseByIdQuery : IRequest<OperationResult<StudyPhrase>>
{
    public GetStudyPhraseByIdQuery(int id)
    {
        this.StudyId = id;
    }
    /// <summary>
    /// Gets the unique identifier for the study.
    /// </summary>
    public int StudyId { get; private set; }
}
