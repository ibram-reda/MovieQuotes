namespace MovieQuotes.UI.Features.Study.BrowseStudyMaterials;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MovieQuotes.Application.Features.Study;
using MovieQuotes.Application.Features.Study.GetStudyMaterials;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

public partial class BrowseStudyMaterialsViewModel : PageViewModelBase
{
    public ObservableCollection<StudyMaterial> Materials { get; } = [];

    [ObservableProperty]
    private string movieIdFilter = string.Empty;

    [ObservableProperty]
    private string levelFilter = string.Empty;

    [ObservableProperty]
    private string partOfSpeechFilter = string.Empty;

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

    [RelayCommand]
    private async Task LoadMaterials()
    {
        await LoadMaterialsAsync(CurrentPageNumber);
    }

    private async Task LoadMaterialsAsync(uint pageNumber)
    {
        IsBusy = true;
        try
        {
            if (!int.TryParse(MovieIdFilter, out var movieId))
                movieId = 0;

            var query = new GetStudyMaterialsQuery
            {
                MovieId = movieId,
                Level = LevelFilter,
                PartOfSpeech = PartOfSpeechFilter,
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
        MovieIdFilter = string.Empty;
        LevelFilter = string.Empty;
        PartOfSpeechFilter = string.Empty;
        SearchText = string.Empty;
        await ApplyFilters();
    }
}