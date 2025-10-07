namespace MovieQuotes.UI.ViewModels;

using CommunityToolkit.Mvvm.Input;
using MovieQuotes.Application.Features.MoviePhrases.Commands;
using MovieQuotes.Application.Features.Movies.Models;
using MovieQuotes.Application.Features.Movies.Queries;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class MovieInfoLoaded
{
    public MovieInfo Movie { get; set; } = new();
    
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;

}
internal partial class SubtitleAddingViewModel : ViewModelBase
{
    public override string Title => "Subtitles";
    public ObservableCollection<MovieInfo> MovieList { get; } = [];
    public ObservableCollection<MovieInfo> Running { get; } = [];
    public ObservableCollection<MovieInfoLoaded> Done { get; } = [];

    public SubtitleAddingViewModel()
    {
        LoadAllCommand.Execute(null);
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    async Task AddSubtitle(MovieInfo movie)
    {
        var cmd = new InsertPhrasesForMovieCommand()
        {
            MovieId = movie.Id,
        };
        MovieList.Remove(movie);
        Running.Add(movie);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await this.mediator.Send(cmd);
        stopwatch.Stop();


        Running.Remove(movie);
        if (result.IsSuccess)
        {
            var movieLoaded = new MovieInfoLoaded()
            {
                Movie = movie,
                Duration = stopwatch.Elapsed,
            };
            Done.Add(movieLoaded);
        }
        else
        {
            MovieList.Add(movie);
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false,IncludeCancelCommand =true)]
    async Task AddSubtitleAll(CancellationToken token)
    {
                 
        while(MovieList.FirstOrDefault() is MovieInfo movie)
        {
            await AddSubtitle(movie);
            if (token.IsCancellationRequested)
            {
                break;
            }
        }
    }

    [RelayCommand]
    async Task LoadAll()
    {
        var qury = new GetAllMoviesWithoutSubtitlesQuery();

        var result = await this.mediator.Send(qury);

        if (result.IsSuccess)
        {
            foreach (var m in result.Payload ?? [])
            {
                MovieList.Add(m);
            }
        }
    }
}
