namespace MovieQuotes.Domain.Interfaces;

public interface IMovieQUnitOfWork : IAsyncDisposable
{
    IMovieRepository Movies { get; }
    ISubtitlePhraseRepository SubtitlePhrases { get; }
    IStudyMaterialRepository StudyMaterials { get; }
    IStudyCardRepository StudyCards { get; }
    ICardProgress CardProgress { get; }
    IPhraseWordsRepository PhraseWords { get; }
    IWordRepository Words { get; } 
    IGenreRepository Genres { get; }
    IStudySessionRepository StudySessions { get; }

    Task<int> SaveAsync(CancellationToken cancellationToken = default);
    Task<bool> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task<bool> CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task<bool> RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

