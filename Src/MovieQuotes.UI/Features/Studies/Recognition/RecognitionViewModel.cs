namespace MovieQuotes.UI.Features.Study;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MovieQuotes.Application.Features.Study;
using MovieQuotes.Domain.Models;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;

public partial class RecognitionViewModel : PageViewModelBase
{
    public override string Title => "Recognition Study";

    [ObservableProperty]
    private StudyPhrase? currentPhrase;

    [ObservableProperty]
    private int currentPhraseIndex = 0;

    [ObservableProperty]
    private int totalPhrases = 0;

    [ObservableProperty]
    private bool isAnswerVisible = false;

    [ObservableProperty]
    private bool isAnswerCorrect = false;

    [ObservableProperty]
    private bool hasAnswered = false;

    [ObservableProperty]
    private string? selectedAnswer;

    [ObservableProperty]
    private string? errorMessage;

    private readonly MediaService mediaService;

    public ObservableCollection<string> AnswerOptions { get; } = new();

    private ObservableCollection<StudyPhrase> phrases = new();
    private int answeredCount = 0;
    private int correctCount = 0;

    [Obsolete("For design-time use only")]
    public RecognitionViewModel() : base()
    {
        if (!Design.IsDesignMode)
            throw new Exception("this is allowed in designMode only");
    }

    public RecognitionViewModel(IMediator mediator, NavigationService navigationService, MediaService mediaService)
        : base(mediator, navigationService)
    {
        this.mediaService = mediaService;
    }

    public async override Task InitAsync(object? parameter = null)
    {
        await LoadPhrasesAsync();
    }

    [RelayCommand]
    private async Task LoadPhrasesAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = null;
            phrases.Clear();
            AnswerOptions.Clear();
            CurrentPhraseIndex = 0;
            answeredCount = 0;
            correctCount = 0;
            HasAnswered = false;
            IsAnswerVisible = false;
            SelectedAnswer = null;

            var query = new GetStudiesQuery(StudyType.Recognition);
            var result = await mediator.Send(query);

            if (result.IsSuccess && result.Payload != null)
            {
                foreach (var phrase in result.Payload)
                {
                    phrases.Add(phrase);
                }

                TotalPhrases = phrases.Count;

                if (TotalPhrases > 0)
                {
                    LoadCurrentPhrase();
                }
                else
                {
                    ErrorMessage = "No recognition exercises available. Please create study materials first.";
                }
            }
            else if (result.IsError)
            {
                ErrorMessage = "Failed to load recognition exercises.";
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

    private void LoadCurrentPhrase()
    {
        if (CurrentPhraseIndex < phrases.Count)
        {
            CurrentPhrase = phrases[CurrentPhraseIndex];
            PopulateAnswerOptions();
            HasAnswered = false;
            IsAnswerVisible = false;
            SelectedAnswer = null;
        }
    }

    private void PopulateAnswerOptions()
    {
        AnswerOptions.Clear();

        if (CurrentPhrase == null)
            return;

        var correctAnswer = CurrentPhrase.Content ?? string.Empty;

        // Get up to 3 random incorrect options from other phrases
        var incorrectOptions = new List<string>();
        var shuffledPhrases = new List<StudyPhrase>(phrases);

        // Shuffle and select incorrect answers
        Random random = new Random();
        for (int i = 0; i < shuffledPhrases.Count && incorrectOptions.Count < 3; i++)
        {
            int randomIndex = random.Next(shuffledPhrases.Count);
            var phrase = shuffledPhrases[randomIndex];

            if (phrase.Content != correctAnswer && !incorrectOptions.Contains(phrase.Content ?? string.Empty))
            {
                incorrectOptions.Add(phrase.Content ?? string.Empty);
            }
        }

        // Combine options
        var allOptions = new List<string> { correctAnswer };
        allOptions.AddRange(incorrectOptions.Take(3));

        // Shuffle options
        for (int i = allOptions.Count - 1; i > 0; i--)
        {
            int randomIndex = random.Next(i + 1);
            var temp = allOptions[i];
            allOptions[i] = allOptions[randomIndex];
            allOptions[randomIndex] = temp;
        }

        // Populate observable collection
        foreach (var option in allOptions)
        {
            AnswerOptions.Add(option);
        }
    }

    [RelayCommand]
    private void SelectAnswer(string? answer)
    {
        if (string.IsNullOrEmpty(answer) || CurrentPhrase == null || HasAnswered)
            return;

        SelectedAnswer = answer;
        IsAnswerCorrect = answer == CurrentPhrase.Content;
        IsAnswerVisible = true;
        HasAnswered = true;

        
        if (IsAnswerCorrect)
        {
           correctCount++;
           this.mediaService?.PlayCorrectAnswerSound();
        }
        else
           this.mediaService?.PlayWrongAnswerSound();
    }

    [RelayCommand]
    private async Task ReviewAgainAsync()
    {
        await ReviewCurrentPhraseAsync(ReviewQuality.CompleteFailure);
    }

    [RelayCommand]
    private async Task ReviewHardAsync()
    {
        await ReviewCurrentPhraseAsync(ReviewQuality.Difficult);
    }

    [RelayCommand]
    private async Task ReviewGoodAsync()
    {
        await ReviewCurrentPhraseAsync(ReviewQuality.Good);
    }

    [RelayCommand]
    private async Task ReviewPerfectAsync()
    {
        await ReviewCurrentPhraseAsync(ReviewQuality.Perfect);
    }

    private async Task ReviewCurrentPhraseAsync(ReviewQuality reviewQuality)
    {
        if (CurrentPhrase is null || !HasAnswered || IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            if (CurrentPhrase.ProgressId > 0)
            {
                var result = await mediator.Send(new UpdateStudyProgressQuery(
                    CurrentPhrase.ProgressId,
                    (int)reviewQuality));

                if (result.IsError)
                {
                    HandleErrors(result.Errors);
                    ErrorMessage = "Failed to update study progress.";
                    return;
                }
            }

            answeredCount++;
            if (reviewQuality >= ReviewQuality.Correct)
                correctCount++;

            if (CurrentPhraseIndex < TotalPhrases - 1)
            {
                CurrentPhraseIndex++;
                LoadCurrentPhrase();
            }
            else
            {
                CurrentPhrase = null;
                ErrorMessage = $"Study session complete! You got {correctCount} out of {answeredCount} correct ({(double)correctCount / answeredCount * 100:F1}%)";
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
    private async Task RestartAsync()
    {
        await LoadPhrasesAsync();
    }
}
