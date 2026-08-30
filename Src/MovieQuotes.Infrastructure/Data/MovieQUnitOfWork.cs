namespace MovieQuotes.Infrastructure.Data;

using Microsoft.EntityFrameworkCore.Storage;
using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Infrastructure.Repositories;

public class MovieQUnitOfWork : IMovieQUnitOfWork
{
    private readonly MovieQuotesDbContext _context;
    private IDbContextTransaction? _transaction;

    private IMovieRepository? _movieRepository;
    private IStudyMaterialRepository? _studyMaterialRepository;
    private IStudyCardRepository? _studyCardRepository;
    private ICardProgress? _cardProgress;
    private ISubtitlePhraseRepository? _subtitlePhraseRepository;
    private IPhraseWordsRepository? _phraseWordsRepository;
    private IWordRepository? _wordRepository; 
    private IGenreRepository? _genreRepository;
    private IStudySessionRepository? _studySessionRepository;

    public MovieQUnitOfWork(MovieQuotesDbContext context)
    {
        _context = context;
    }

    public IMovieRepository Movies => _movieRepository ??= new MovieRepository(_context);
    public IStudyCardRepository StudyCards => _studyCardRepository ??= new StudyCardRepository(_context);
    public ICardProgress CardProgress => _cardProgress ??= new CardProgressRepository(_context);
    public IStudyMaterialRepository StudyMaterials => _studyMaterialRepository ??= new StudyMaterialRepository(_context);
    public ISubtitlePhraseRepository SubtitlePhrases => _subtitlePhraseRepository ??= new SubtitlePhraseRepository(_context);

    public IPhraseWordsRepository PhraseWords => _phraseWordsRepository ??= new PhraseWordsRepository(_context);

    public IWordRepository Words => _wordRepository ??= new WordRepository(_context);

    public IGenreRepository Genres => _genreRepository ??= new GenreRepository(_context);
    public IStudySessionRepository StudySessions => _studySessionRepository ??= new StudySessionRepository(_context);

    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync();
        return _transaction != null;
    }

    public async Task<bool> CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
            return false;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync();
            return true;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
        return false;
    }

    public async Task<bool> RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
            return false;

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
            return true;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
        return false;
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
        }
        await _context.DisposeAsync();
    }
}
