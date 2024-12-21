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
    async Task AddSubtitle(MovieInfo phrase)
    {
        var baseFolder = Path.GetDirectoryName(phrase.LocalPath);
        var subtitleFolder = Path.Combine(baseFolder ?? "", "subtitles");
        var enSubs = Directory.GetFiles(subtitleFolder).FirstOrDefault(f => f.EndsWith("en.srt"));
        var cmd = new InsertPhrasesForMovieCommand()
        {
            MovieId = phrase.Id,
            SubtitleLocation = enSubs
        };
        MovieList.Remove(phrase);
        Running.Add(phrase);
        var result = await this.mediator.Send(cmd);

        Running.Remove(phrase);
        if (result.IsSuccess)
        {
            Done.Add(phrase);
        }
        else
        {
            MovieList.Add(phrase);
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
