
namespace MovieQuotes.Application.Features.StudyPhrases.Commands;

using MediatR;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.StudyPhrases.Models;

public record ReviewStudyPhraseCommand(
    int StudyPhraseId,
    /// <summary>
    /// | Score | Meaning           |
    /// | ----- | ----------------- |
    /// | 0     | complete blackout |
    /// | 1     | wrong             |
    /// | 2     | almost remembered |
    /// | 3     | correct but hard  |
    /// | 4     | correct           |
    /// | 5     | very easy         |
    /// </summary>
    int Quality
) : IRequest<OperationResult<DateTime>>;

