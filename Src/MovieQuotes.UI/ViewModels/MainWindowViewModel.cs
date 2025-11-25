namespace MovieQuotes.UI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MovieQuotes.UI.Features.Movies.BrowseMovies;
using MovieQuotes.UI.Features.Movies.CreateMovie;
using MovieQuotes.UI.Features.StudyVocabs.ReviseVocab;
using MovieQuotes.UI.Services;
using System;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private bool _RenderNavigationBar = true;
    [ObservableProperty] private string _WindowTitle = "";

    [System.Obsolete("For design-time use only")]
    public MainWindowViewModel()
    {
    }

    public MainWindowViewModel(IMediator mediator, NavigationService nav) : base(mediator, nav)
    {
        NavigationService.CurrentPageChanged += OnCurrentViewModelChanged;
        NavigationService.NavigateTo<MoviesListViewModel>();
    }

    private void OnCurrentViewModelChanged(PageViewModelBase obj)
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
    private void Navigate(string PageName)
    {
        switch (PageName)
        {
            case "InsertNewMovie":
                this.NavigationService.NavigateTo<NewMovieViewModel>();
                break;
            case "PlayBack":
                this.NavigationService.NavigateTo<PlaybackViewModel>();
                break;
            case "Study":
                this.NavigationService.NavigateTo<StudyViewModel>();
                break;
            case "Subtitle":
                this.NavigationService.NavigateTo<SubtitleAddingViewModel>();
                break;
            case "GetMovies":
                this.NavigationService.NavigateTo<MoviesListViewModel>();
                break;
            case "ResyncSubtitle":
                this.NavigationService.NavigateTo<ResyncSubtitleViewModel>();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public ViewModelBase CurrentViewModel => NavigationService.CurrentViewModel;


    public override string Title => "Movie Quotes";
}