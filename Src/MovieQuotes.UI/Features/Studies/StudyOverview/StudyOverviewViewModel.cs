namespace MovieQuotes.UI.Features.Study;

using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MovieQuotes.Application.Features.Study;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;

public partial class StudyOverviewViewModel : PageViewModelBase
{
    public override string Title => "Study Overview";

    [ObservableProperty]
    private StudyOverview? studyOverview;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool hasData = false;

    public bool HasRecognitionDue => StudyOverview?.RecognitionCardsDueToday > 0;

    public bool HasContextRecallDue => StudyOverview?.ContextRecallCardsDueToday > 0;

    public StudyOverviewViewModel() : base()
    {
    }

    public StudyOverviewViewModel(IMediator mediator, NavigationService navigationService) 
        : base(mediator, navigationService)
    {
    }

    partial void OnStudyOverviewChanged(StudyOverview? value)
    {
        OnPropertyChanged(nameof(HasRecognitionDue));
        OnPropertyChanged(nameof(HasContextRecallDue));
    }

    public async override Task InitAsync(object? parameter = null)
    {
        await LoadStudyOverviewAsync();
    }

    [RelayCommand]
    private async Task LoadStudyOverviewAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = null;
            HasData = false;

            var query = new GetStudyOverviewQuery();
            var result = await mediator.Send(query);

            if (result.IsSuccess && result.Payload != null)
            {
                StudyOverview = result.Payload;
                HasData = true;
            }
            else if (result.IsError)
            {
                ErrorMessage = "Failed to load study overview. Please try again.";
                foreach (var error in result.Errors)
                {
                    ErrorMessages.Add($"{error.Code}: {error.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
            ErrorMessages.Add(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadStudyOverviewAsync();
    }

    [RelayCommand]
    private void StartRecognitionStudy()
    {
        NavigationService.NavigateTo<RecognitionViewModel>();
    }

    [RelayCommand]
    private void StartActiveRecallStudy()
    {
        NavigationService.NavigateTo<ActiveRecallViewModel>();
    }
}
