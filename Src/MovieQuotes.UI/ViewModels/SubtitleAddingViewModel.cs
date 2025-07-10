namespace MovieQuotes.UI.ViewModels;

using CommunityToolkit.Mvvm.Input;
using MovieQuotes.Application.Features.MoviePhrases.Commands;
using MovieQuotes.Application.Features.Movies.Models;
using MovieQuotes.Application.Features.Movies.Queries;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

internal partial class SubtitleAddingViewModel : ViewModelBase
{
    public override string Title => "Subtitles";
    public ObservableCollection<MovieInfo> MovieList { get; } = [];
    public ObservableCollection<MovieInfo> Running { get; } = [];
    public ObservableCollection<MovieInfo> Done { get; } = [];

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
        var result = await this.mediator.Send(cmd);

        Running.Remove(movie);
        if (result.IsSuccess)
        {
            Done.Add(movie);
        }
        else
        {
            MovieList.Add(movie);
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
