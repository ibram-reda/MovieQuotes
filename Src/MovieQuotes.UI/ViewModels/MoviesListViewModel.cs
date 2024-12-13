namespace MovieQuotes.UI.ViewModels;

using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieQuotes.Application.Models;
using MovieQuotes.Application.Operations.Commands;
using MovieQuotes.Application.Operations.Queries;
using MovieQuotes.UI.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


public partial class MoviesListViewModel : ViewModelBase
{
    private readonly IFilesService filesService;

    public ObservableCollection<MovieInfo> DBMovies { get; } = new();
    public ObservableCollection<MovieInfo> OutOfSyncMovies { get; } = new();
    [ObservableProperty] private string _BaseFolder = string.Empty;
    [ObservableProperty] string? filterText;

    public bool NeedToSync => OutOfSyncMovies.Any();

    public override string Title => "🎥 movie List";

    public MoviesListViewModel()
    {
        this.filesService = this.GetService<IFilesService>();
        GetAllMoviesCommand.Execute(this);
        OutOfSyncMovies.CollectionChanged += (_, _) =>
            this.OnPropertyChanged(nameof(NeedToSync));

    }

    [RelayCommand]
    public async Task ChoseFolder()
    {
        OutOfSyncMovies.Clear();
        var folder = await this.filesService.OpenFolderAsync();
        if (folder is null)
            return;

        BaseFolder = folder.TryGetLocalPath() ?? string.Empty;
        var items = folder.GetItemsAsync();
        await foreach (var item in folder.GetItemsAsync())
        {
            if (item is IStorageFolder movieBaseFolder)
            {
                var movie = await ExtractMovieInfoAsync(movieBaseFolder);
                if (!IsMovieInDB(movie))
                    OutOfSyncMovies.Add(movie);
            }
        }
    }

    [RelayCommand]
    private void Select(MovieInfo selectedMovie)
    {
        this.NavigationService.NavigateTo<NewMovieViewModel>(selectedMovie);

    }

    private bool IsMovieInDB(MovieInfo movie)
    {
        return DBMovies.Any(a => a.Title == movie.Title);
    }
    private static async Task<MovieInfo> ExtractMovieInfoAsync(IStorageFolder movieBaseFolder)
    {
        MovieInfo movie = new MovieInfo();
        movie.Title = movieBaseFolder.Name;
        movie.Year = GetYearFromTitle(movie.Title);
        await foreach (var file in movieBaseFolder.GetItemsAsync())
        {
            if (file is IStorageFile movieFilePart)
            {
                var index = movieFilePart.Name.LastIndexOf('.');
                var extension = movieFilePart.Name.Substring(index);
                var path = movieFilePart.Path.LocalPath; ;
                if (extension.EndsWith("srt"))
                    movie.SubtitlePath = path;
                if (extension.EndsWith("mp4") || extension.EndsWith("mkv"))
                    movie.LocalPath = path;
                if (extension.EndsWith("jpg"))
                    movie.CoverUrl = path;
            }

            if (file is IStorageFolder subFolder && subFolder.Name == "subtitles")
            {
                await foreach (var subtitleFile in subFolder.GetItemsAsync())
                {
                    if (subtitleFile is IStorageFile subtitleFilePart && subtitleFilePart.Name.EndsWith("en.srt"))
                        movie.SubtitlePath = subtitleFilePart.Path.LocalPath;
                }
            }
        }


        return movie;
    }

    [RelayCommand]
    public async Task GetAllMovies()
    {
        var query = new GetAllMoviesQuery(FilterText);
        IsBusy = true;
        var result = await this.mediator.Send(query);
        IsBusy = false;
        if (result.IsError)
        {
            foreach (var error in result.Errors)
                ErrorMessages?.Add(error.Message);
            return;
        }
        DBMovies.Clear();
        foreach (var movie in result?.Payload ?? [])
        {
            DBMovies.Add(movie);
        }
    }

    [RelayCommand]
    private async Task DisplayMovieDetails(int movieId)
    {
        await this.NavigationService.NavigateToAsync<MovieDetailsViewModel>(movieId);
    }

    [RelayCommand]
    public async Task Sync()
    {
        var cleanDb = new CleanDatabaseCommand();
        IsBusy = true;
        var result = await this.mediator.Send(cleanDb);
        IsBusy = false;

        if (!result.IsError)
        {
            Console.WriteLine(result.Payload);
        }
    }
    public override void ConsumeMessage(object? message)
    {
        if (message is null) return;
        if (message is string s)
        {
            var m = OutOfSyncMovies.FirstOrDefault(a => a.Title == s);
            if (m is null)
                return;

            OutOfSyncMovies.Remove(m);
            DBMovies.Add(m);
        }
    }


    private static int GetYearFromTitle(string title)
    {
        Regex regex = new Regex(@"\(([0-9]{4})\)$");
        var x = regex.Match(title).Groups[1].Value;
        return int.Parse(x ?? "0");
    }
}