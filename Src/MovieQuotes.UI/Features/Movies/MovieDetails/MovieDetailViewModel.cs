namespace MovieQuotes.UI.Features.Movies.MovieDetails;

using Avalonia.Controls.Generators;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using MovieQuotes.Application.Features.MoviePhrases.Commands;
using MovieQuotes.Application.Features.MoviePhrases.Queries;
using MovieQuotes.Application.Features.Movies.Queries;
using MovieQuotes.UI.Features.Movies.WatchMovie; 
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;


internal partial class MovieDetailViewModel : PageViewModelBase
{
    private readonly GetMovieDetailsQueryHandler getMovieDetailsHandler;

    [System.Obsolete("For design-time use only")]
    public MovieDetailViewModel()
    {
    }

    public MovieDetailViewModel(GetMovieDetailsQueryHandler getMovieDetailsHandler, NavigationService nav) : base(nav)
    {         
        this.getMovieDetailsHandler = getMovieDetailsHandler;
    }


    public override string Title => $"Details of {MovieName}";

    [ObservableProperty] int _id;
    [NotifyPropertyChangedFor(nameof(Title))]
    [ObservableProperty] string movieName = "";
     
    [ObservableProperty] long movieLength = 0;
    [ObservableProperty] string moviePoster = "";
    [ObservableProperty] string backdrop = "";
    [ObservableProperty] string overview = ""; 

    [ObservableProperty] string releaseDate = "";
    [ObservableProperty] string genres = "";

    [ObservableProperty] float voteAverage = 0f;
    [ObservableProperty] int voteCount = 0;
    [ObservableProperty] bool isAdult = false; 
    
 

    public override async Task InitAsync(object? message)
    {
        if (message is int movieId)
            await LoadDataAsync(movieId);
    }

    async Task LoadDataAsync(int movieId)
    {
        var query = new GetMovieDetailsQuery { MovieId = movieId };

        var reuslt = await this.getMovieDetailsHandler.Handle(query, CancellationToken.None);

        if (reuslt.IsError)
            return;

        this.Id = reuslt.Payload!.Id;
        this.MovieName = reuslt.Payload.Title;  
        this.Overview = reuslt.Payload.Description ?? "";
        this.MoviePoster = reuslt.Payload.PosterUrl ?? "";
        this.Backdrop = reuslt.Payload.BackdropUrl ?? ""; 
        this.Genres = string.Join(", ", reuslt.Payload.Genres); 
        this.VoteAverage = reuslt.Payload.VoteAverage;
        this.VoteCount = reuslt.Payload.VoteCount;
        this.IsAdult = reuslt.Payload.IsAdult;
        this.ReleaseDate =  reuslt.Payload.Year?.ToString() ?? "";

    }


    [RelayCommand]
    private async Task WatchMovie()
    { 
        await this.NavigationService.NavigateToAsync<WatchMovieViewModel>(this.Id);
    }
}
