using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MovieQuotes.UI.Services;

namespace MovieQuotes.UI.ViewModels;


public partial class WelcomeScreenViewModel : ViewModelBase
{
    private readonly NavigationService navigation;

    public WelcomeScreenViewModel()
    {
        this.navigation = App.Current!.Services!.GetRequiredService<NavigationService>();
    }
    [RelayCommand]
    private void InsertNewMovie()
    { 
        this.navigation.NavigateTo<NewMovieViewModel>();
    } 

    [RelayCommand]
    private void PlayBack()
    {
        this.navigation.NavigateTo<PlaybackViewModel>();
    }

    [RelayCommand]
    private void GetMovies()
    {
        this.navigation.NavigateTo<MoviesListViewModel>();
    }

}