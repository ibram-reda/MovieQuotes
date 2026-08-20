namespace MovieQuotes.UI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MovieQuotes.UI.Features.Movies.BrowseMovies;
using MovieQuotes.UI.Features.Movies.CreateMovie;
using MovieQuotes.UI.Features.StudyVocabs.ReviseVocab;
using MovieQuotes.UI.Services;
using System;
using Avalonia;
using Avalonia.Styling;
using MovieQuotes.UI.Features.Settings;
using MovieQuotes.UI.Features.Study;
using MovieQuotes.UI.Features.Study.BrowseStudyMaterials;
using MovieQuotes.UI.Features.Movies.WatchMovie;

public partial class MainWindowViewModel : ViewModelBase
{
    private const double ExpandedNavigationWidth = 220;
    private const double CollapsedNavigationWidth = 70;

    [ObservableProperty] private bool _RenderNavigationBar = true;
    [ObservableProperty] private bool _isNavigationMenuOpen = true;
    [ObservableProperty] private double _navigationMenuWidth = ExpandedNavigationWidth;
    [ObservableProperty] private string _WindowTitle = "";
    [ObservableProperty] bool _isDarkMode = true;

    partial void OnIsDarkModeChanged(bool value)
    {
        if (Application.Current is { } app)
        {
            app.RequestedThemeVariant = value ? ThemeVariant.Dark : ThemeVariant.Light;
        }
    }

    partial void OnIsNavigationMenuOpenChanged(bool value)
    {
        NavigationMenuWidth = value ? ExpandedNavigationWidth : CollapsedNavigationWidth;
    }

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
        if(CurrentViewModel is WatchMovieViewModel){
            NavigationMenuWidth = 0;
        }else
        {
            NavigationMenuWidth = ExpandedNavigationWidth;
        }  

        this.RenderNavigationBar = true;
    }

    [RelayCommand]
    private void ToggleNavigationMenu()
    {
        IsNavigationMenuOpen = !IsNavigationMenuOpen;
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
            case "BrowseStudyMaterials":
                this.NavigationService.NavigateTo<BrowseStudyMaterialsViewModel>();
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
            case "Settings":
                this.NavigationService.NavigateTo<SettingsViewModel>();
                break;
            case "ActiveRecall":
                this.NavigationService.NavigateTo<ActiveRecallViewModel>();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public PageViewModelBase CurrentViewModel => NavigationService.CurrentViewModel;


    public override string Title => "Movie Quotes";
}