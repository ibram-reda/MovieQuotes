using Xunit;
using Moq;
using MovieQuotes.Application.Features.StudyPhrases.Commands;
using MovieQuotes.Application.Features.StudyPhrases.CommandHandlers;
using MovieQuotes.Domain.Entities;
using MovieQuotes.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace MovieQuotes.Application.Tests.Features.StudyPhrases.CommandsHandlers
{
    public class ReviewStudyPhraseCommandHandlerTests
    {
        private readonly Mock<IStudyPhraseRepository> _mockRepository;
        private readonly ReviewStudyPhraseCommandHandler _handler;

        public ReviewStudyPhraseCommandHandlerTests()
        {
            _mockRepository = new Mock<IStudyPhraseRepository>();
            _handler = new ReviewStudyPhraseCommandHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_UpdatesStudyPhrase()
        {
            // Arrange
            var studyPhraseId = 1;
            var command = new ReviewStudyPhraseCommand { StudyPhraseId = studyPhraseId, Quality = 4 };
            var studyPhrase = new StudyPhrase { Id = studyPhraseId, ReviewCount = 0 };
            
            _mockRepository.Setup(r => r.GetByIdAsync(studyPhraseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(studyPhrase);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<StudyPhrase>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<StudyPhrase>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNullStudyPhrase_ThrowsException()
        {
            // Arrange
            var command = new ReviewStudyPhraseCommand { StudyPhraseId = 999 };
            _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((StudyPhrase)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Handle_WithCorrectAndIncorrectAnswers_UpdatesReviewData(bool isCorrect)
        {
            // Arrange
            var studyPhraseId = 1;
            var command = new ReviewStudyPhraseCommand { StudyPhraseId = studyPhraseId, IsCorrect = isCorrect };
            var studyPhrase = new StudyPhrase { Id = studyPhraseId, CorrectCount = 0, IncorrectCount = 0 };
            
            _mockRepository.Setup(r => r.GetByIdAsync(studyPhraseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(studyPhrase);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<StudyPhrase>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<StudyPhrase>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}