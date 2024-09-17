namespace MovieQuotes.UI.ViewModels;

using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieQuotes.Application.Models;
using MovieQuotes.Application.Operations.Queries;
using MovieQuotes.UI.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;


public partial class MoviesListViewModel : ViewModelBase
{
    private readonly IFilesService filesService;
    
    public ObservableCollection<MovieInfo> DBMovies { get; } = new();
    public ObservableCollection<MovieInfo> OutOfSyncMovies { get; } = new();
    [ObservableProperty] private string _BaseFolder = string.Empty;

    public bool NeedToSync => OutOfSyncMovies.Any();

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
        await foreach (var file in movieBaseFolder.GetItemsAsync())
        {
            if (file is IStorageFile movieFilePart)
            {
                var index = movieFilePart.Name.LastIndexOf('.');
                var extension = movieFilePart.Name.Substring(index); 
                var path = movieFilePart.Path.LocalPath; ;
                if (extension.EndsWith("srt"))
                    movie.SubtitlePath = path;
                if(extension.EndsWith("mp4") || extension.EndsWith("mkv"))
                    movie.LocalPath = path;
                if (extension.EndsWith("jpg")) 
                    movie.CoverUrl = path;

            }
        }


        return movie;
    }

    [RelayCommand]
    public async Task GetAllMovies()
    {
        var query = new GetAllMoviesQuery();
        var result = await this.mediator.Send(query);

        if (result.IsError)
        {
            foreach (var error in result.Errors)
                ErrorMessages?.Add(error.Message);
            return;
        }

        foreach (var movie in result?.Payload ?? [])
        {
            DBMovies.Add(movie);
        }
    }



    public override void ConsumeMessage(object? message)
    {
        if(message is string s)
        {
            var m = OutOfSyncMovies.FirstOrDefault(a=>a.Title == s);
            if (m is  null)
                return;
            
            OutOfSyncMovies.Remove(m);
            DBMovies.Add(m);
        }
    }
}