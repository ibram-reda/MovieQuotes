namespace MovieQuotes.UI.Features.StudyMaterials.BrowseStudyMaterials;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MovieQuotes.Application.Features.Movies.Models;
using MovieQuotes.Application.Features.Movies.Queries;
using MovieQuotes.Application.Features.StudyMaterials; 
using MovieQuotes.UI.Features.StudyMaterials.EditStudyMaterial;
using MovieQuotes.UI.Features.StudyMaterials.StudyMaterialDetails;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;
using MovieQuotes.UI.ViewModels.Dialogues;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

public sealed record StudyMaterialSortOption(StudyMaterialSortOrder Value, string Label);

public partial class BrowseStudyMaterialsViewModel : PageViewModelBase
{
    public ObservableCollection<StudyMaterial> Materials { get; } = [];
    public ObservableCollection<MovieWithStudyMaterialCount> Movies { get; } = [];
    public ObservableCollection<LevelFilter> Levels { get; } = [];
    public ObservableCollection<PartOfSpeechCountDto> PartOfSpeechCounts { get; } = [];
    public IReadOnlyList<StudyMaterialSortOption> SortOptions { get; } =
    [
        new(StudyMaterialSortOrder.ModifiedDate, "Modified date"),
        new(StudyMaterialSortOrder.CreatedDate, "Created date"),
        new(StudyMaterialSortOrder.Alphabetical, "Alphabetical")
    ];

    [ObservableProperty]
    private MovieWithStudyMaterialCount? selectedMovie;

    [ObservableProperty]
    private LevelFilter? selectedLevel;

    [ObservableProperty]
    private PartOfSpeechCountDto? selectedPartOfSpeech;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool draftOnly;

    [ObservableProperty]
    private bool vulgarOnly;

    [ObservableProperty]
    private StudyMaterialSortOption sortOrder = new(StudyMaterialSortOrder.ModifiedDate, "Modified date");

    [ObservableProperty]
    private int totalCount;

    [ObservableProperty]
    private uint currentPageNumber;

    [ObservableProperty]
    private uint resultPerPage = 20;
    private readonly INotificationService notificationService;
    private bool isInitialized;

    public override string Title => "Browse Study Materials";

    [System.Obsolete("For design-time use only")]
    public BrowseStudyMaterialsViewModel()
    {
        Materials.Add(new StudyMaterial
        {
            Content = "vocabulary",
            Definition = "A sample study material",
            PartOfSpeech = "Noun",
            Level = "B1",
            Examples = "- This is an example."
        });
        Materials.Add(new StudyMaterial
        {
            Content = "vocabulary",
            Definition = "A sample study material",
            PartOfSpeech = "Noun",
            Level = "B1",
            Examples = "- This is an example."
        });
        Materials.Add(new StudyMaterial
        {
            Content = "vocabulary",
            Definition = "A sample study material",
            PartOfSpeech = "Noun",
            Level = "B1",
            Examples = "- This is an example."
        });
        Materials.Add(new StudyMaterial
        {
            Content = "vocabulary",
            Definition = "A sample study material",
            PartOfSpeech = "Noun",
            Level = "B1",
            Examples = "- This is an example."
        });
    }

    public BrowseStudyMaterialsViewModel(IMediator mediator, NavigationService navigation, INotificationService notificationService)
        : base(mediator, navigation)
    {
        this.notificationService = notificationService;
    }

    public override async Task InitAsync(object? initValue)
    {
        if (!isInitialized)
        {
            await this.LoadMovies();
            await this.LoadLevels();
            await this.LoadPartOfSpeechCounts();
            isInitialized = true;
        }
        await this.ApplyFilters();
    }
 
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
        var totalMovie = new MovieWithStudyMaterialCount(){Title="Any Movies"};
        Movies.Add(totalMovie);
        foreach (var movie in result.Payload ?? [])
            Movies.Add(movie); 
        SelectedMovie = totalMovie;
    }

    private async Task LoadLevels()
    {
        var result = await mediator.Send(new GetStudyMaterialLevelsQuery());

        if (result.IsError)
        {
            HandleErrors(result.Errors);
            return;
        }

        Levels.Clear();
        foreach (var level in result.Payload ?? [])
            Levels.Add(level);
    }
 
    private async Task LoadPartOfSpeechCounts(int movieId = 0)
    {
        var query = new GetPartOfSpeechCountsQuery(movieId);
        var result = await mediator.Send(query);

        if (result.IsError)
        {
            HandleErrors(result.Errors);
            return;
        }

        PartOfSpeechCounts.Clear();
        var total = new PartOfSpeechCountDto(){ PartOfSpeech="Any type"};
        PartOfSpeechCounts.Add(total);
        foreach (var partOfSpeech in result.Payload ?? [])
            PartOfSpeechCounts.Add(partOfSpeech); 
        SelectedPartOfSpeech=total; 
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
    private void DeleteMaterial(StudyMaterial material)
    {
        if (material is null)
            return;

        var confirmationDialogue = new ConfirmationViewModel(
            "Delete study material?",
            $"Are you sure you want to delete '{material.Content}' material? This action cannot be undone.");
        confirmationDialogue.DialogueClosed += async (_, args) =>
        {
            if (args.IsClosedSuccessfully)
            {
                var result = await mediator.Send(new DeleteStudyMaterialCommand(material.Id));
                if (result.IsError)
                {
                    HandleErrors(result.Errors);
                    return;
                }
                else
                {
                    this.notificationService.ShowSuccess("Deleted!", "Your Material Have Been Deleted");
                    await ApplyFiltersCommand.ExecuteAsync(null);
                }
            }
            this.ShowEditDialog = false;
        };

        this.Dialogue = confirmationDialogue;
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
                Level = SelectedLevel?.Level ?? string.Empty,
                PartOfSpeech = SelectedPartOfSpeech?.PartOfSpeech,
                SearchText = SearchText,
                DraftOnly = DraftOnly,
                VulgarOnly = VulgarOnly,
                SortOrder = SortOrder.Value,
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
        SelectedMovie = Movies.FirstOrDefault();
        SelectedLevel = Levels.FirstOrDefault();
        SelectedPartOfSpeech = PartOfSpeechCounts.FirstOrDefault();
        SearchText = string.Empty;
        DraftOnly = false;
        VulgarOnly = false;
        await ApplyFilters();
    }


}