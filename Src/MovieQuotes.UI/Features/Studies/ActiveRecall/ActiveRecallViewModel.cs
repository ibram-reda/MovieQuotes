namespace MovieQuotes.UI.Features.Study;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using Microsoft.Extensions.Logging;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Application.Features.Study;
using MovieQuotes.Application.Features.MoviePhrases.CommandsHandlers;
using MovieQuotes.Application.Features.MoviePhrases.QueriesHandlers;
using MovieQuotes.Application.Features.StudyMaterials;
using MovieQuotes.Domain.Models;
using MovieQuotes.UI.Features.StudyMaterials.EditStudyMaterial;
using MovieQuotes.UI.Features.Subtitles.EditeSubtitlePhrase;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;
using MovieQuotes.UI.ViewModels.Dialogues;

public partial class ActiveRecallViewModel : PageViewModelBase
{
    private readonly SpellingErrorAnalyzer _spellingAnalyzer;
    private readonly GetStudiesQueryHandler getStudiesHandler;
    private readonly UpdateStudyProgressCommandHandler updateStudyProgressHandler;
    private readonly GetStudyMaterialQueryHandler getStudyMaterialHandler;
    private readonly EditStudyMaterialCommandHandler editStudyMaterialHandler;
    private readonly GetSubtitlePhraseByIdQueryHandler getSubtitlePhraseHandler;
    private readonly EditPhraseCommandHandler editPhraseHandler;
    private MediaService MediaService { get; }
    [ObservableProperty] SpellingAnalysis? _spellingAnalysis;
    public bool ShowResult =>
        HasChecked && SpellingAnalysis != null;


    [ObservableProperty] int _TotalPhrases;
    [ObservableProperty] StudyPhrase? _CurrentPhrase;
    [ObservableProperty] int _CurrentPhraseIndex;
    [ObservableProperty] bool _IsAnswerVisible;
    [ObservableProperty] bool _IsAnswerCorrect = true;
    [ObservableProperty] bool _IsHintVisible;
    [ObservableProperty] bool _hasChecked;
    [ObservableProperty] string _QuestionText = string.Empty;
    [ObservableProperty] string _AnswerText = string.Empty;
    [ObservableProperty] string _AnswerInput = string.Empty;

    ObservableCollection<StudyPhrase> Phrases { get; } = new();
    private int _wrongAnswerAttemptsByPhrase = 0;
    private int _correctAnswers;
    private int _incorrectAnswers;

    public string? ErrorPattern
    {
        get
        {
            var error = SpellingAnalysis?
                .Errors
                .FirstOrDefault();

            return error?.Type switch
            {
                SpellingErrorType.MissingLetter =>
                    "Missing letter",

                SpellingErrorType.ExtraLetter =>
                    "Extra letter",

                SpellingErrorType.Substitution =>
                    "Letter substitution",

                SpellingErrorType.VowelError =>
                    "Vowel error",

                SpellingErrorType.Transposition =>
                    "Letter order",

                SpellingErrorType.InternalSequence =>
                    "Internal sequence",

                SpellingErrorType.MultipleErrors =>
                    "Multiple errors",

                _ => null
            };
        }
    }




#pragma warning disable
    public ActiveRecallViewModel()
    {
        // test if this constructor is called at runtime, if so throw an exception
        if (!Design.IsDesignMode)
        {
            throw new Exception("ActiveRecallViewModel should not be instantiated without parameters.");
        }
        // For design time only
        TotalPhrases = 0;
        CurrentPhraseIndex = 0;
        IsAnswerVisible = false;
        AnswerText = string.Empty;
        AnswerInput = string.Empty;


    }
    private readonly ILogger<ActiveRecallViewModel> logger;
#pragma warning restore
    public ActiveRecallViewModel(
        GetStudiesQueryHandler getStudiesHandler,
        UpdateStudyProgressCommandHandler updateStudyProgressHandler,
        GetStudyMaterialQueryHandler getStudyMaterialHandler,
        EditStudyMaterialCommandHandler editStudyMaterialHandler,
        GetSubtitlePhraseByIdQueryHandler getSubtitlePhraseHandler,
        EditPhraseCommandHandler editPhraseHandler,
        NavigationService nav,
        MediaService mediaService,
        ILogger<ActiveRecallViewModel> logger) : base(nav)
    {
        this.getStudiesHandler = getStudiesHandler;
        this.updateStudyProgressHandler = updateStudyProgressHandler;
        this.getStudyMaterialHandler = getStudyMaterialHandler;
        this.editStudyMaterialHandler = editStudyMaterialHandler;
        this.getSubtitlePhraseHandler = getSubtitlePhraseHandler;
        this.editPhraseHandler = editPhraseHandler;
        MediaService = mediaService;
        _spellingAnalyzer = new SpellingErrorAnalyzer();
        this.logger = logger;
    }



    public override string Title => "Active Recall";



    [RelayCommand]
    async Task Init()
    {
        var result = await getStudiesHandler.Handle(new GetStudiesQuery(studyType: StudyType.ContextRecall), CancellationToken.None);
        if (result.IsError)
        {
            this.HandleErrors(result.Errors);
            return;
        }
        Phrases.Clear();
        CurrentPhraseIndex = 0;
        _correctAnswers = 0;
        _incorrectAnswers = 0;
        foreach (var phrase in result.Payload ?? [])
        {
            Phrases.Add(phrase);
        }
        TotalPhrases = Phrases.Count;

        if (Phrases.Count > 0)
        {
            CurrentPhrase = Phrases[0];
        }
    }

    [RelayCommand]
    async Task CheckAnswer()
    {
        if (CurrentPhrase is null || HasChecked)
            return;

        string expected = CurrentPhrase.Content ?? string.Empty;
        string actual = AnswerInput;
        var analysis = new SpellingErrorAnalyzer().Analyze(expected, actual);
        SpellingAnalysis = analysis;
        HasChecked = true;
        IsAnswerVisible = true;
        IsAnswerCorrect = analysis.IsCorrect;

        if (!IsAnswerCorrect)
        {
            this.MediaService?.PlayWrongAnswerSound();
            _wrongAnswerAttemptsByPhrase++;
            return;
        }

        var reviewQuality = GetReviewQuality(_wrongAnswerAttemptsByPhrase);
        if (reviewQuality is ReviewQuality.Perfect or ReviewQuality.Correct)
            _correctAnswers++;
        else
            _incorrectAnswers++;

        var update = UpdateProgressForCurrentPhraseAsync(reviewQuality);
        this.MediaService?.PlayCorrectAnswerSound();
        await update;
    }
    [RelayCommand]
    void UndoAnswer()
    {
        Reset();
    }


    [RelayCommand]
    async Task ShowNextPhrase()
    {
        IsAnswerVisible = false;
        AnswerInput = string.Empty;
        if (!IsAnswerCorrect)
        {
            // if the previouse answer is not correct redo the same qustion
            IsAnswerVisible = false;
            AnswerInput = string.Empty;
            HasChecked = false;
            IsHintVisible = false;
            OnPropertyChanged(nameof(CurrentPhrase));
            PlayAudio();
            return;
        }
        if (CurrentPhraseIndex < Phrases.Count - 1)
        {
            CurrentPhraseIndex++;
            CurrentPhrase = Phrases[CurrentPhraseIndex];
        }
        else
        {
            await NavigationService.NavigateToAsync<StudySummaryViewModel>(
                new StudySummaryViewModel.StudySummaryParameters
                {
                    TotalPhrases = TotalPhrases,
                    CorrectAnswers = _correctAnswers,
                    IncorrectAnswers = _incorrectAnswers
                });
        }

    }

    [RelayCommand]
    private void EditMaterial()
    {
        if (CurrentPhrase is null)
            return;

        var dialogue = new EditStudyMaterialViewModel(
            CurrentPhrase.MaterialId,
            getStudyMaterialHandler,
            editStudyMaterialHandler);
        dialogue.OnSaved += async (isSaved, result) =>
        {
            if (isSaved && result.Payload is Application.Features.StudyMaterials.StudyMaterial material)
            {
                // Refflect on UI
                CurrentPhrase.Content = material.Content;
                CurrentPhrase.Definition = material.Definition;
                CurrentPhrase.Examples = material.Examples;
                var EditedPhrase = CurrentPhrase;
                CurrentPhrase = null;
                OnPropertyChanged(nameof(CurrentPhrase));
                CurrentPhrase = EditedPhrase;
                OnPropertyChanged(nameof(CurrentPhrase));
            }
            this.ShowEditDialog = false;
        };
        this.Dialogue = dialogue;
        this.ShowEditDialog = true;
    }

    [RelayCommand]
    private void EditeSubtitlePhrase()
    {
        if (CurrentPhrase is null)
            return;

        var dialouge = new EditeSubtitlePhraseViewModel(
            CurrentPhrase.PhraseId,
            getSubtitlePhraseHandler,
            editPhraseHandler);
        dialouge.OnPhraseEdited += (phrase) =>
        {
            CurrentPhrase.PhraseText = phrase.Text;
            this.ShowEditDialog = false;
            var EditedPhrase = CurrentPhrase;
            CurrentPhrase = null;
            OnPropertyChanged(nameof(CurrentPhrase));
            CurrentPhrase = EditedPhrase;
            OnPropertyChanged(nameof(CurrentPhrase));
        };
        this.Dialogue = dialouge;
        this.ShowEditDialog = true;
    }

    partial void OnCurrentPhraseChanged(StudyPhrase? value)
    {
        if(value is null) return;
        string hiddenContent = new string(value?.Content?.Select(c =>
            char.IsAsciiLetter(c) ? '_' : c
        ).ToArray());
        var phrase = GetCleanPhrases(value?.PhraseText);
        QuestionText = phrase.Replace(value.Content ?? "", hiddenContent, StringComparison.OrdinalIgnoreCase) ?? string.Empty;
        AnswerText = value?.Content ?? string.Empty;
        Reset();
        PlayAudio();
    }

    /// <summary>
    /// Cleans subtitle text by combining multiple lines into a single phrase.
    /// If the subtitle contains dialogue lines prefixed with a hyphen,
    /// the original text is preserved to maintain speaker separation.
    /// </summary>
    /// <param name="subtitleText">The subtitle text to clean.</param>
    /// <returns>
    /// The cleaned subtitle text, or the original text when speaker-separated
    /// dialogue lines are detected.
    /// </returns>
    string GetCleanPhrases(string subtitleText)
    {
        if (string.IsNullOrWhiteSpace(subtitleText))
            return string.Empty;

        var phrases = subtitleText.Split('\n');

        if (phrases.Any(p => p.TrimStart().StartsWith("-")))
            return subtitleText;

        return string.Join(" ", phrases.Select(p => p.Trim()));
    }

    [RelayCommand]
    void PlayAudio()
    {
        if (!string.IsNullOrEmpty(CurrentPhrase?.VideoPath))
        {
            this.MediaService.PlayAudio(CurrentPhrase.VideoPath);
        }
    }
    [RelayCommand]
    void ShowHint()
    {
        IsHintVisible = true;
    }

    void Reset()
    {
        _wrongAnswerAttemptsByPhrase = 0;
        IsAnswerVisible = false;
        AnswerInput = string.Empty;
        IsHintVisible = false;
        HasChecked = false;
        IsAnswerCorrect = false;
        SpellingAnalysis = null;
    }

    private async Task UpdateProgressForCurrentPhraseAsync(ReviewQuality reviewQuality)
    {
        if (CurrentPhrase is null || CurrentPhrase.ProgressId <= 0)
            return;

        var result = await updateStudyProgressHandler.Handle(
            new UpdateStudyProgressCommand(CurrentPhrase.ProgressId, (int)reviewQuality),
            CancellationToken.None);

        if (result.IsError)
        {
            HandleErrors(result.Errors);
        }
    }

    private static ReviewQuality GetReviewQuality(int wrongAttempts)
    {

        return wrongAttempts switch
        {
            0 => ReviewQuality.Perfect,
            1 => ReviewQuality.Correct,
            2 => ReviewQuality.Difficult,
            _ => ReviewQuality.CompleteFailure
        };

    }



}