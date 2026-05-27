using Xunit;
using Moq;
using MovieQuotes.Application.Features.StudyPhrases.CommandsHandlers; 
using MovieQuotes.Application.Features.StudyPhrases.Commands;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MovieQuotes.Application.Tests.Features.StudyPhrases.CommandsHandlers
{
    public class ReviewStudyPhraseCommandHandlerTests
    {
        private readonly Mock<IStudyPhraseRepository> _mockRepository;
        private readonly Mock<IMovieQUnitOfWork> _mockUnitOfWork;
        private readonly ReviewStudyPhraseCommandHandler _handler;
        

        public ReviewStudyPhraseCommandHandlerTests()
        {
            _mockRepository = new Mock<IStudyPhraseRepository>();
            _mockUnitOfWork = new Mock<IMovieQUnitOfWork>();
            _mockUnitOfWork.SetupGet(u => u.StudyPhrases).Returns(_mockRepository.Object);
            _handler = new ReviewStudyPhraseCommandHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_UpdatesStudyPhrase()
        {
            // // Arrange
            // var studyPhraseId = 1;
            // var command = new ReviewStudyPhraseCommand ( studyPhraseId,  4 );
            // var progress = new Mock<StudyPhraseProgress>();
            // progress.SetupGet(p => p.Repetition).Returns(3);
            
            // var studyPhrase = new Mock<StudyPhrase>();
            // studyPhrase.SetupGet(sp => sp.Progress).Returns(progress.Object);
            
            // _mockRepository.Setup(r => r.GetByIdAsync(studyPhraseId))
            //     .ReturnsAsync(studyPhrase.Object);
            // _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<StudyPhrase>()))
            //     .Returns(Task.CompletedTask);

            // // Act
            // await _handler.Handle(command, CancellationToken.None);

            // // Assert
            // _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<StudyPhrase>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNullStudyPhrase_ReturnsError()
        {
            // Arrange
            var command = new ReviewStudyPhraseCommand ( 999 ,  4 );

            _mockUnitOfWork.Setup(u => u.StudyPhraseProgress.GetProgressForPhrase(999))
                .ReturnsAsync((StudyPhraseProgress)null); 

            // Act & Assert
            var result = await _handler.Handle(command, CancellationToken.None);
            Assert.False(result.IsSuccess);
            Assert.Contains(result.Errors, e => e.Code == MovieQuotes.Application.Common.Enums.ErrorCode.NotFound);
        }

         
    }
}