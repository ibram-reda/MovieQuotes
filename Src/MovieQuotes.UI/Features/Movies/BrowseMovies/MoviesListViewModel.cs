namespace MovieQuotes.UI.Features.Movies.BrowseMovies;

using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MovieQuotes.Application.Features.Movies.Commands;
using MovieQuotes.Application.Features.Movies.Models;
using MovieQuotes.Application.Features.Movies.Queries;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


public partial class MoviesListViewModel : PageViewModelBase
{
    private readonly IFilesService filesService;

    public ObservableCollection<MovieCardViewModel> DBMovies { get; } = new();
    public ObservableCollection<MovieCardViewModel> OutOfSyncMovies { get; } = new();
    [ObservableProperty] private string _BaseFolder = string.Empty;
    [ObservableProperty] string? filterText;
    [ObservableProperty] int totalCountOfMovieInDB = 0;

    public bool NeedToSync => OutOfSyncMovies.Any();

    public override string Title => "🎥 movie List";

    [System.Obsolete("For design time only")]
    public MoviesListViewModel()
    {
        this.OutOfSyncMovies.Add(new() { MovieTitle = "Inception", ReleaseYear = 2010, CoverUrl = "e:/inception.jpg", Tagline = "Your mind is the scene of the crime." });
        this.OutOfSyncMovies.Add(new() { MovieTitle = "Interstellar", ReleaseYear = 2014, CoverUrl = "e:/interstellar.jpg", Tagline = "Mankind was born on Earth. It was never meant to die here." });

        this.DBMovies.Add(new() { MovieTitle = "The Shawshank Redemption", ReleaseYear = 1994, CoverUrl = "e:/cover.jpg", Tagline = "Fear can hold you prisoner. Hope can set you free." });
        this.DBMovies.Add(new() { MovieTitle = "The Godfather", ReleaseYear = 1972, CoverUrl = "e:/cover2.jpg", Tagline = "An offer you can't refuse." });
        this.DBMovies.Add(new() { MovieTitle = "The Dark Knight", ReleaseYear = 2008, CoverUrl = "e:/cover3.jpg", Tagline = "Why So Serious?" });
        this.DBMovies.Add(new() { MovieTitle = "Pulp Fiction", ReleaseYear = 1994, CoverUrl = "e:/cover4.jpg", Tagline = "Just because you are a character doesn't mean you have character." });
    }

    public MoviesListViewModel(IMediator mediator, NavigationService nav, IFilesService filesService) : base(mediator, nav)
    {
        this.filesService = filesService;
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
                if (!await IsMovieInDBAsync(movie.FolderName))
                    OutOfSyncMovies.Add(new(movie));
            }
        }
    }



    private async Task<bool> IsMovieInDBAsync(string folderName)
    {
        var query = new IsMovieExistQuery { FolderName = folderName };
        var result = await this.mediator.Send(query);
        return result.Payload;
    }

    [ObservableProperty] int _prograss =0;
    [RelayCommand]
    private async Task AutoBendingMoviesInsert()
    {
        foreach(var m in OutOfSyncMovies)
        {
            var info = m.MovieInfo;
            var command = new CreateMovieCommand(info.BaseFolderDir, info.FolderName, info.Title, info.Year??0, info.LocalPath, info.Description, info.IMDBId, info.CoverUrl);
            var result = await mediator.Send(command);
        

            if (result.IsError)
            {
                foreach (var error in result.Errors)
                    ErrorMessages?.Add(error.Message);
                continue;
            }

            Prograss++;
            this.DBMovies.Add(new(result.Payload!));

        }
    }
    record MovieExportDto(string? IMDBId,string? Title ,int? Year ,string? Description);
    private static async Task<MovieInfo> ExtractMovieInfoAsync(IStorageFolder movieBaseFolder)
    {
        MovieInfo movie = new MovieInfo();
        movie.BaseFolderDir = (await movieBaseFolder.GetParentAsync())?.TryGetLocalPath()??"";
        movie.FolderName = movieBaseFolder.Name;
        movie.Title = GetTitleFromFolderName(movieBaseFolder.Name);
        movie.Year = GetYearFromFolderName(movieBaseFolder.Name);
        await foreach (var file in movieBaseFolder.GetItemsAsync())
        {
            if (file is IStorageFile info && info.Name =="info.json")
            {
                try
                {                    
                var p = file.TryGetLocalPath();
                var infotext = await File.ReadAllTextAsync(p);
                var minfo = JsonSerializer.Deserialize<MovieExportDto>(infotext);
                movie.IMDBId = minfo?.IMDBId;
                movie.Description = minfo?.Description;
                }
                finally
                {
                    
                }
            }
            if (file is IStorageFile movieFilePart)
            {

                var index = movieFilePart.Name.LastIndexOf('.');
                var extension = movieFilePart.Name.Substring(index).ToLower();
                var path = movieFilePart.Path.LocalPath; ;
                if (extension.EndsWith("srt"))
                    movie.SubtitlePath = path;
                if (extension.EndsWith("mp4") || 
                    extension.EndsWith("mkv") ||
                    extension.EndsWith("avi"))
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
        if (mediator is null)
        {
            return;
        }
        var result = await this.mediator.Send(query);
        IsBusy = false;
        if (result.IsError)
        {
            foreach (var error in result.Errors)
                ErrorMessages?.Add(error.Message);
            return;
        }
        DBMovies.Clear();
        TotalCountOfMovieInDB = (int)result.Count;
        foreach (var movie in result?.Payload ?? [])
        {
            DBMovies.Add(new(movie));
        }
    }



    public override void ConsumeMessage(object? message)
    {
        if (message is null) return;
        if (message is MovieInfo dbMovie)
        {
            var m = OutOfSyncMovies.FirstOrDefault(a => a.MovieTitle == dbMovie.Title);
            if (m is null)
                return;

            OutOfSyncMovies.Remove(m);
            DBMovies.Add(new(dbMovie));
        }
    }

    private static string GetTitleFromFolderName(string folderName)
    {
        var index = folderName.IndexOf('(');

        if (index < 0)
            return folderName;

        return folderName[..index].Trim();
    }

    private static int GetYearFromFolderName(string folderName)
    {
        Regex regex = new Regex(@"\(([0-9]{4})\)$");
        var x = regex.Match(folderName).Groups[1].Value;
        if (int.TryParse(x, out var res))
            return res;

        
        throw new ArgumentException($"can not get year form '{folderName}'", nameof(folderName));
    }
}