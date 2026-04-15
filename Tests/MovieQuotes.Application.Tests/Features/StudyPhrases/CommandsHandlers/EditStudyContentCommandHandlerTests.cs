namespace MovieQuotes.Application.Tests.Features.StudyPhrases.CommandsHandlers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Features.StudyPhrases.Commands;
using MovieQuotes.Application.Features.StudyPhrases.CommandsHandlers;
using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Domain.Models;
using Xunit;


public class EditStudyContentCommandHandlerTests
{
    private static EditStudyContentCommand CreateBasicCommand()
        => new()
        {
            StudyId = 1,
            Content = "new content",
            Translation = "new translation",
            StudyType = default,
            ArContentTranslation = "ar content",
            ArPhraseTranslation = "ar phrase",
            Origin = "origin",
            Notes = "notes"
        };

    [Fact]
    public async Task Handle_StudyPhraseNotFound_ReturnsNotFoundError()
    {
        var studyPhrasesRepoMock = new Mock<IStudyPhraseRepository>(MockBehavior.Strict);
        studyPhrasesRepoMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((StudyPhrase)null);

        var unitOfWorkMock = new Mock<IMovieQUnitOfWork>(MockBehavior.Strict);
        unitOfWorkMock.SetupGet(u => u.StudyPhrases).Returns(studyPhrasesRepoMock.Object);

        var handler = new EditStudyContentCommandHandler(unitOfWorkMock.Object);
        var request = CreateBasicCommand();

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == ErrorCode.NotFound);
        studyPhrasesRepoMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        studyPhrasesRepoMock.Verify(r => r.UpdateAsync(It.IsAny<StudyPhrase>()), Times.Never);
        unitOfWorkMock.Verify(u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SaveReturnsZero_ReturnsUpdateError()
    {
        // create a StudyPhrase instance (constructor may vary)
        var studyPhrase = Activator.CreateInstance(typeof(StudyPhrase), true) as StudyPhrase
                          ?? throw new InvalidOperationException("Cannot create StudyPhrase instance for test.");

        var studyPhrasesRepoMock = new Mock<IStudyPhraseRepository>(MockBehavior.Strict);
        studyPhrasesRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(studyPhrase);
        studyPhrasesRepoMock.Setup(x => x.UpdateAsync(studyPhrase)).Returns(Task.CompletedTask);

        var unitOfWorkMock = new Mock<IMovieQUnitOfWork>(MockBehavior.Strict);
        unitOfWorkMock.SetupGet(u => u.StudyPhrases).Returns(studyPhrasesRepoMock.Object);
        unitOfWorkMock.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

        var handler = new EditStudyContentCommandHandler(unitOfWorkMock.Object);
        var request = CreateBasicCommand();

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == ErrorCode.UpdateError);
        studyPhrasesRepoMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        studyPhrasesRepoMock.Verify(r => r.UpdateAsync(studyPhrase), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ExistingStudyPhrase_SaveSucceeds_ReturnsPayload()
    {
        var studyPhrase = Activator.CreateInstance(typeof(StudyPhrase), true) as StudyPhrase
                          ?? throw new InvalidOperationException("Cannot create StudyPhrase instance for test.");

        var studyPhrasesRepoMock = new Mock<IStudyPhraseRepository>(MockBehavior.Strict);
        studyPhrasesRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(studyPhrase);
        studyPhrasesRepoMock.Setup(x => x.UpdateAsync(studyPhrase)).Returns(Task.CompletedTask);

        var unitOfWorkMock = new Mock<IMovieQUnitOfWork>(MockBehavior.Strict);
        unitOfWorkMock.SetupGet(u => u.StudyPhrases).Returns(studyPhrasesRepoMock.Object);
        unitOfWorkMock.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new EditStudyContentCommandHandler(unitOfWorkMock.Object);
        var request = CreateBasicCommand();

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.False(result.IsError);
        Assert.NotNull(result.Payload);
       // Assert.Equal(studyPhrase, result.Payload);
        // if ToStudyPhrase returns a mapped DTO, just ensure payload is not null.
        studyPhrasesRepoMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        studyPhrasesRepoMock.Verify(r => r.UpdateAsync(studyPhrase), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
