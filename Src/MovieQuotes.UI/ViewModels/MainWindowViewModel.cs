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
using MovieQuotes.UI.Models;
using System.Collections.Generic;

public partial class MainWindowViewModel : ViewModelBase
{
    private const double ExpandedNavigationWidth = 220;
    private const double CollapsedNavigationWidth = 70;

    [ObservableProperty] private bool _RenderNavigationBar = true;
    [ObservableProperty] private bool _isNavigationMenuOpen = true;
    [ObservableProperty] private double _navigationMenuWidth = ExpandedNavigationWidth;
    [ObservableProperty] private string _WindowTitle = "";
    [ObservableProperty] bool _isDarkMode = true;

    public IReadOnlyList<NavigationItem> NavigationItems { get; } =
    [
        new("add_square_regular", "Add New Movie", typeof(NewMovieViewModel)),
        new("movies_and_tv_regular", "Browse Movies", typeof(MoviesListViewModel)),
        new("search_square_regular", "Search & Playback", typeof(PlaybackViewModel)),
        new("document_one_page_regular", "Study", typeof(StudyViewModel)),
        new("document_one_page_regular", "Study Materials", typeof(BrowseStudyMaterialsViewModel)),
        new("text_font_regular", "Subtitle", typeof(SubtitleAddingViewModel)),
        new("headset_regular", "Active Recall", typeof(ActiveRecallViewModel)),
        new("text_font_regular", "ResyncSubtitle", typeof(ResyncSubtitleViewModel)),
        new("settings_regular", "Settings", typeof(SettingsViewModel))
    ];

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
        foreach (var item in NavigationItems)
            item.IsActive = item.Matches(obj);
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
    private void Navigate(NavigationItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        NavigationService.NavigateTo(item.PageType);
    }

    public PageViewModelBase CurrentViewModel => NavigationService.CurrentViewModel;


    public override string Title => "Movie Quotes";
}