namespace MovieQuotes.UI.Features.Study.BrowseStudyMaterials;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MovieQuotes.Application.Features.Movies.Models;
using MovieQuotes.Application.Features.Movies.Queries;
using MovieQuotes.Application.Features.Study;
using MovieQuotes.Application.Features.Study.GetStudyMaterials;
using MovieQuotes.UI.Features.Study.EditStudyMaterial;
using MovieQuotes.UI.Features.Study.StudyMaterialDetails;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

public partial class BrowseStudyMaterialsViewModel : PageViewModelBase
{
    public ObservableCollection<StudyMaterial> Materials { get; } = [];
    public ObservableCollection<MovieWithStudyMaterialCount> Movies { get; } = [];
    public ObservableCollection<PartOfSpeechCountDto> PartOfSpeechCounts { get; } = [];

    [ObservableProperty]
    private MovieWithStudyMaterialCount? selectedMovie;

    [ObservableProperty]
    private string levelFilter = string.Empty;

    [ObservableProperty]
    private PartOfSpeechCountDto? selectedPartOfSpeech;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private int totalCount;

    [ObservableProperty]
    private uint currentPageNumber;

    [ObservableProperty]
    private uint resultPerPage = 20;

    public override string Title => "Browse Study Materials";

    [System.Obsolete("For design-time use only")]
    public BrowseStudyMaterialsViewModel()
    {
        Materials.Add(new StudyMaterial
        {
            Content = "Example vocabulary",
            Definition = "A sample study material",
            PartOfSpeech = "Noun",
            Level = "B1",
            Examples = "- This is an example."
        });
    }

    public BrowseStudyMaterialsViewModel(IMediator mediator, NavigationService navigation)
        : base(mediator, navigation)
    {
    }

    public override async Task InitAsync(object? initValue)
    {
        await this.LoadMaterials();
        await this.LoadMovies();
        await this.LoadPartOfSpeechCounts();
    }

    [RelayCommand]
    private async Task LoadMovies()
    {
        var query = new GetMoviesWithStudyMaterialQuery();
        var result = await mediator.Send(query);

        if (result.IsError)
        {
            HandleErrors(result.Errors);
            return;
        }

        Movies.Clear();
        foreach (var movie in result.Payload ?? [])
            Movies.Add(movie);
    }

    [RelayCommand]
    private async Task LoadPartOfSpeechCounts(int movieId=0)
    {
        var query = new GetPartOfSpeechCountsQuery(movieId);
        var result = await mediator.Send(query);

        if (result.IsError)
        {
            HandleErrors(result.Errors);
            return;
        }

        PartOfSpeechCounts.Clear();
        foreach (var partOfSpeech in result.Payload ?? [])
            PartOfSpeechCounts.Add(partOfSpeech);
    }

    [RelayCommand]
    private async Task LoadMaterials()
    {
        await LoadMaterialsAsync(CurrentPageNumber);
    }

    [RelayCommand]
    private void EditMaterial(StudyMaterial material)
    {
        if (material is null)
            return;

        var dialogue = new EditStudyMaterialViewModel(material);
        dialogue.OnSaved += async (isSaved, _) =>
        {
            if (isSaved)
                await LoadMaterialsAsync(CurrentPageNumber);
            this.ShowEditDialog = false;
        };
        this.Dialogue = dialogue;
        this.ShowEditDialog = true;
    }

    [RelayCommand]
    private async Task ShowMaterialDetails(StudyMaterial material)
    {
        if (material is null || material.Id <= 0)
            return;

        await NavigationService.NavigateToAsync<StudyMaterialDetailsViewModel>(material.Id);
    }

    private async Task LoadMaterialsAsync(uint pageNumber)
    {
        IsBusy = true;
        try
        {
            var query = new GetStudyMaterialsQuery
            {
                MovieId = SelectedMovie?.Id ?? 0,
                Level = LevelFilter,
                PartOfSpeech = SelectedPartOfSpeech?.PartOfSpeech,
                SearchText = SearchText,
                PageNumber = pageNumber,
                ResultPerPage = ResultPerPage
            };

            var result = await mediator.Send(query);
            if (result.IsError)
            {
                HandleErrors(result.Errors);
                return;
            }

            ErrorMessages.Clear();
            Materials.Clear();
            foreach (var material in result.Payload ?? [])
                Materials.Add(material);

            TotalCount = result.Count;
            CurrentPageNumber = result.CurrentPageNumber;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ApplyFilters()
    {
        CurrentPageNumber = 0;
        await LoadMaterialsAsync(CurrentPageNumber);
    }

    [RelayCommand]
    private async Task GoToPage(uint pageNumber)
    {
        if (IsBusy)
            return;

        await LoadMaterialsAsync(pageNumber);
    }

    

    [RelayCommand]
    private async Task ClearFilters()
    {
        SelectedMovie = null;
        LevelFilter = string.Empty;
        SelectedPartOfSpeech = null;
        SearchText = string.Empty;
        await ApplyFilters();
    }
    
    partial void OnSelectedMovieChanged(MovieWithStudyMaterialCount? value)
    =>OnSelectedMovieChangedAsync(value);
    async void OnSelectedMovieChangedAsync(MovieWithStudyMaterialCount? movie)
    {
        await LoadPartOfSpeechCountsCommand.ExecuteAsync(movie?.Id??0);
        await ApplyFiltersCommand.ExecuteAsync(null);
        
    }
}