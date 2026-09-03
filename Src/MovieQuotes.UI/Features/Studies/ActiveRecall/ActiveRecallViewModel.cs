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
using MediatR;
using Microsoft.Extensions.Logging;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Application.Features.Study;
using MovieQuotes.Domain.Models;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;
using MovieQuotes.UI.ViewModels.Dialogues;

public partial class ActiveRecallViewModel : PageViewModelBase
{
    private readonly SpellingErrorAnalyzer _spellingAnalyzer;
    private MediaService MediaService {get;}
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
    public ActiveRecallViewModel(IMediator mediator, NavigationService nav, MediaService mediaService,ILogger<ActiveRecallViewModel> logger) : base(mediator, nav)
    {
        MediaService = mediaService; 
        _spellingAnalyzer = new SpellingErrorAnalyzer();
        this.logger = logger;
    }



    public override string Title => "Active Recall";

    

    [RelayCommand]
    async Task Init()
    {
        var result = await this.mediator.Send(new GetStudiesQuery(studyType: StudyType.ContextRecall));
        TotalPhrases = result.Count;
        if (result.IsError)
        {
            this.HandleErrors(result.Errors);
            return;
        }
        Phrases.Clear();
        foreach (var phrase in result.Payload ??[])
        {
            Phrases.Add(phrase);
        }

        if (Phrases.Count > 0)
        {
            CurrentPhrase = Phrases[0];
        }
    }

    [RelayCommand]
    async Task CheckAnswer()
    {
        if (CurrentPhrase is null)
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
            this.MediaService.PlayWrongAnswerSound();
            return;
        }

        // corret go next  
        var update = UpdateProgressForCurrentPhraseAsync();
        this.MediaService.PlayCorrectAnswerSound();
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
            Reset();
            OnPropertyChanged(nameof(CurrentPhrase));
            PlayAudio();
            _wrongAnswerAttemptsByPhrase++;
            return;
        }
        if (CurrentPhraseIndex < Phrases.Count - 1)
        {
            CurrentPhraseIndex++;
            CurrentPhrase = Phrases[CurrentPhraseIndex];
        }
        else
        {
            // Handle end of phrases, e.g., show a message or reset
            CurrentPhraseIndex = 0;
            CurrentPhrase = Phrases[CurrentPhraseIndex];
            _wrongAnswerAttemptsByPhrase = 0;
        }

    }

    partial void OnCurrentPhraseChanged(StudyPhrase? value)
    {
        string hiddenContent = new string(value?.Content?.Select(c =>
            char.IsAsciiLetter(c) ? '_' : c
        ).ToArray());
        QuestionText = value?.PhraseText?.Replace("\n", " ").Replace(value.Content ?? "", hiddenContent) ?? string.Empty;
        AnswerText = value?.Content ?? string.Empty;
        Reset();
        PlayAudio();
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
        IsAnswerVisible = false;
        AnswerInput = string.Empty;
        IsHintVisible = false;
        HasChecked = false;
        IsAnswerCorrect = false;
        SpellingAnalysis = null;
    }

    private async Task UpdateProgressForCurrentPhraseAsync()
    {
        if (CurrentPhrase is null || CurrentPhrase.ProgressId <= 0)
            return;

        
        var reviewQuality = GetReviewQuality(_wrongAnswerAttemptsByPhrase);
        var result = await mediator.Send(new UpdateStudyProgressQuery(CurrentPhrase.ProgressId, (int)reviewQuality));

        if (result.IsError)
        {
            HandleErrors(result.Errors);
        }
    }

    private static ReviewQuality GetReviewQuality( int wrongAttempts)
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