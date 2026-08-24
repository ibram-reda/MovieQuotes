namespace MovieQuotes.Application.Tests.Features.Study.AddStudyMaterialToLearning;

using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using MovieQuotes.Application.Common.Enums;
using MovieQuotes.Application.Features.Study.AddStudyMaterialToLearning;
using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Domain.Models;
using Xunit;

public class AddStudyMaterialToLearningCommandHandlerTests
{
    [Fact]
    public async Task Handle_StudyMaterialNotFound_ReturnsNotFoundError()
    {
        var studyMaterials = new Mock<IStudyMaterialRepository>(MockBehavior.Strict);
        studyMaterials.Setup(repository => repository.GetByIdAsync(1)).ReturnsAsync((StudyMaterial?)null);
        var studyCards = new Mock<IStudyCardRepository>(MockBehavior.Strict);
        var unitOfWork = CreateUnitOfWork(studyMaterials, studyCards);

        var result = await CreateHandler(unitOfWork).Handle(new AddStudyMaterialToLearningCommand(1), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, error => error.Code == ErrorCode.NotFound);
        studyCards.Verify(repository => repository.AddAsync(It.IsAny<StudyCard>()), Times.Never);
        unitOfWork.Verify(work => work.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_StudyMaterialAlreadyHasCard_ReturnsInvalidInputError()
    {
        var material = Activator.CreateInstance(typeof(StudyMaterial), true) as StudyMaterial
            ?? throw new InvalidOperationException("Cannot create StudyMaterial instance for test.");
        material.StudyCards.Add(StudyCard.Create(1));
        var studyMaterials = new Mock<IStudyMaterialRepository>(MockBehavior.Strict);
        studyMaterials.Setup(repository => repository.GetByIdAsync(1)).ReturnsAsync(material);
        var studyCards = new Mock<IStudyCardRepository>(MockBehavior.Strict);
        var unitOfWork = CreateUnitOfWork(studyMaterials, studyCards);

        var result = await CreateHandler(unitOfWork).Handle(new AddStudyMaterialToLearningCommand(1), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, error => error.Code == ErrorCode.InvalidInput);
        studyCards.Verify(repository => repository.AddAsync(It.IsAny<StudyCard>()), Times.Never);
        unitOfWork.Verify(work => work.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_StudyMaterialWithoutCard_AddsCardAndSaves()
    {
        var material = Activator.CreateInstance(typeof(StudyMaterial), true) as StudyMaterial
            ?? throw new InvalidOperationException("Cannot create StudyMaterial instance for test.");
        var studyMaterials = new Mock<IStudyMaterialRepository>(MockBehavior.Strict);
        studyMaterials.Setup(repository => repository.GetByIdAsync(1)).ReturnsAsync(material);
        var studyCards = new Mock<IStudyCardRepository>(MockBehavior.Strict);
        studyCards.Setup(repository => repository.AddAsync(It.IsAny<StudyCard>())).Returns(Task.CompletedTask);
        var unitOfWork = CreateUnitOfWork(studyMaterials, studyCards);
        unitOfWork.Setup(work => work.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await CreateHandler(unitOfWork).Handle(new AddStudyMaterialToLearningCommand(1), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Payload);
        studyCards.Verify(repository => repository.AddAsync(It.Is<StudyCard>(card => card.StudyMaterialId == material.Id)), Times.Once);
        unitOfWork.Verify(work => work.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static AddStudyMaterialToLearningCommandHandler CreateHandler(Mock<IMovieQUnitOfWork> unitOfWork)
        => new(unitOfWork.Object);

    private static Mock<IMovieQUnitOfWork> CreateUnitOfWork(
        Mock<IStudyMaterialRepository> studyMaterials,
        Mock<IStudyCardRepository> studyCards)
    {
        var unitOfWork = new Mock<IMovieQUnitOfWork>(MockBehavior.Strict);
        unitOfWork.SetupGet(work => work.StudyMaterials).Returns(studyMaterials.Object);
        unitOfWork.SetupGet(work => work.StudyCards).Returns(studyCards.Object);
        return unitOfWork;
    }
}