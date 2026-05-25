namespace MovieQuotes.UI.Features.Movies.BrowseMovies;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MovieQuotes.Application.Features.Movies.Models;
using MovieQuotes.UI.Features.Movies.CreateMovie;
using MovieQuotes.UI.Features.Movies.MovieDetails;
using MovieQuotes.UI.Features.Movies.WatchMovie;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;
using System.Threading.Tasks;

public partial class MovieCardViewModel : ViewModelBase
{
    public override string Title => MovieTitle;
    [NotifyPropertyChangedFor(nameof(Title))]
    [ObservableProperty] string movieTitle = string.Empty;
    [ObservableProperty] string tagline = string.Empty;
    [ObservableProperty] string coverUrl = string.Empty;
    [ObservableProperty] int movieId;
    [ObservableProperty] int releaseYear;
    public MovieInfo MovieInfo { get; private set; }   


    [System.Obsolete("For design-time use only")]
    public MovieCardViewModel()
    {
    }

    public MovieCardViewModel(MovieInfo movieInfo) : base(null, null)
    {
        this.mediator = GetService<IMediator>();
        this.NavigationService = GetService<NavigationService>();
        this.Initialize(movieInfo);
    }
    public MovieCardViewModel(IMediator mediator, NavigationService nav, MovieInfo movieInfo) : base(mediator, nav)
    {
        this.Initialize(movieInfo);
    }
    private void Initialize(MovieInfo movieInfo)
    {
        this.MovieInfo = movieInfo;
        this.MovieId = movieInfo.Id;
        this.MovieTitle = movieInfo.Title;
        this.ReleaseYear = movieInfo.Year ?? 0;
        this.Tagline = movieInfo.Description ?? "";
        this.CoverUrl = movieInfo.CoverUrl ?? "";
    }
    [RelayCommand]
    private async Task DisplayMovieDetails()
    {
        if(MovieId <= 0)
        {
            this.NavigationService.NavigateTo<NewMovieViewModel>(this.MovieInfo);
            return;
        }
        await this.NavigationService.NavigateToAsync<MovieDetailViewModel>(this.MovieId);
    }
}
