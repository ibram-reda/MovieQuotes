namespace MovieQuotes.UI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieQuotes.UI.Services;


public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private bool _RenderNavigationBar = true;
    [ObservableProperty] private string _WindowTitle = "";

    public MainWindowViewModel()
    {
        NavigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;
        NavigationService.NavigateTo<MoviesListViewModel>();

    }

    private void OnCurrentViewModelChanged(ViewModelBase obj)
    {
        this.OnPropertyChanged(nameof(CurrentViewModel));
        WindowTitle = $"{Title} : {CurrentViewModel.Title}";
        CurrentViewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "Title")
                WindowTitle = $"{Title} : {CurrentViewModel.Title}";

        };

        this.RenderNavigationBar = true;
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
    private void Study()
    {
        this.NavigationService.NavigateTo<StudyViewModel>();
    }

    [RelayCommand]
    private void Subtitle()
    {
        this.NavigationService.NavigateTo<SubtitleAddingViewModel>();
    }

    [RelayCommand]
    private void GetMovies()
    {
        this.NavigationService.NavigateTo<MoviesListViewModel>();
    }
    public ViewModelBase CurrentViewModel => NavigationService.CurrentViewModel;


    public override string Title => "Movie Quotes";
}