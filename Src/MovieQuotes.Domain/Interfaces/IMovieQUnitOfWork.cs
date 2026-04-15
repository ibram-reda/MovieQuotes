namespace MovieQuotes.Domain.Interfaces;

public interface IMovieQUnitOfWork : IAsyncDisposable
{
    IMovieRepository Movies { get; }
    ISubtitlePhraseRepository SubtitlePhrases { get; }
    IPhraseWordsRepository PhraseWords { get; }
    IWordRepository Words { get; }
    IStudyPhraseRepository StudyPhrases { get; }
    IStudyPhraseProgressRepository StudyPhraseProgress { get; }
    
    Task<int> SaveAsync(CancellationToken cancellationToken = default);
    Task<bool> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task<bool> CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task<bool> RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

 