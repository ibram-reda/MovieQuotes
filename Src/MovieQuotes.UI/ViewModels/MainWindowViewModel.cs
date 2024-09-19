namespace MovieQuotes.UI.ViewModels;

using Avalonia.Controls.Chrome;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieQuotes.UI.Services;


public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private bool _RenderNavigationBar = false;
    [ObservableProperty] private string _WindowTitle = "";

    public MainWindowViewModel()
    {
        NavigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;
        NavigationService.NavigateTo<WelcomeScreenViewModel>();

    }

    private void OnCurrentViewModelChanged(ViewModelBase obj)
    {
        this.OnPropertyChanged(nameof(CurrentViewModel));
        WindowTitle = $"{Title} : {CurrentViewModel.Title}";
        RenderNavigationBar = CurrentViewModel is not WelcomeScreenViewModel;
    }

    [RelayCommand]
    private void InsertNewMovie()
    {
        this.NavigationService.NavigateTo<NewMovieViewModel>();
    }

    [RelayCommand]
    private void PlayBack()
    {
        this.NavigationService.NavigateTo<PlaybackViewModel>();
    }

    [RelayCommand]
    private void GetMovies()
    {
        this.NavigationService.NavigateTo<MoviesListViewModel>();
    }
    public ViewModelBase CurrentViewModel => NavigationService.CurrentViewModel;


    public override string Title => "Movie Quotes";
}