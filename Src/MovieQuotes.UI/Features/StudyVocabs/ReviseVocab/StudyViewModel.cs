namespace MovieQuotes.UI.Features.StudyVocabs.ReviseVocab;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using MediatR;
using MovieQuotes.Application.Features.MoviePhrases.Commands;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Application.Features.StudyPhrases.Commands;
using MovieQuotes.Application.Features.StudyPhrases.Models;
using MovieQuotes.Application.Features.StudyPhrases.Queries;
using MovieQuotes.Application.Features.VideoClips.Queries;
using MovieQuotes.UI.Extensions;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;
using MovieQuotes.UI.ViewModels.Dialogues;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class StudyViewModel : PageViewModelBase
{

    [NotifyCanExecuteChangedFor(nameof(AddReviewCommand))]
    [NotifyCanExecuteChangedFor(nameof(ReplayCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
    [NotifyCanExecuteChangedFor(nameof(ShowFolderCommand))]
    [NotifyCanExecuteChangedFor(nameof(EditPhraseCommand))]
    [ObservableProperty] StudyPhrase? currentPlayingPhrase;
    [NotifyCanExecuteChangedFor(nameof(EditPhraseCommand))]
    [ObservableProperty] bool showEditDialog = false;
    public bool IsDialogClosed => !ShowEditDialog;
    [ObservableProperty] StudyPhrasesGroupByMovie? selectedMovie;
    [ObservableProperty] DialogueViewModelBase? dialogue;

    [ObservableProperty] bool _ShowCompleteContent = false;
    [ObservableProperty] bool _ShowPhraseContent = false;


    [ObservableProperty] int _Quality = 0;

    public int DuePhrasesCount => this.Phrases.Count(p => p.NextReviewDate <= DateTime.Now);

    public List<string> QualityStrings { get; } = [
        "complete blackout",
        "wrong",
        "almost remembered",
        "correct but hard",
        "correct",
        "very easy"
    ];


    [NotifyCanExecuteChangedFor(nameof(NextCommand))]
    [NotifyCanExecuteChangedFor(nameof(PreviousCommand))]
    [ObservableProperty] int currentPlayingIndex;
    public ObservableCollection<StudyPhrase> Phrases { get; } = new();
    public ObservableCollection<StudyPhrasesGroupByMovie> Movies { get; } = [];

    private LibVLC MainLibVLC { get; }
    public MediaPlayer MainMediaPlayer { get; }
    private IFilesService _filesService;

    public override string Title => "Study";

    [System.Obsolete("For design-time use only")]
    public StudyViewModel()
    {
    }

    public StudyViewModel(IMediator mediator, NavigationService nav, IFilesService filesService) : base(mediator, nav)
    {
        MainLibVLC = new();
        MainMediaPlayer = new(MainLibVLC)
        {
            EnableHardwareDecoding = true,
            Volume = 100
        };
        this._filesService = filesService;
        this.Phrases.CollectionChanged += (s, e) =>
        {
            NextCommand.NotifyCanExecuteChanged();
            PreviousCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(DuePhrasesCount));
        };

    }


    [RelayCommand(CanExecute = nameof(NextCanExecute))]
    void Next()
    {
        if (!NextCanExecute()) return;
        this.MainMediaPlayer.Stop();
        SetCurrentPhraseIndex(++CurrentPlayingIndex);
        ShowCompleteContent = false;
        ShowPhraseContent = false;
    }

    [RelayCommand]
    void CompleteContent()
    {
        // If the phrase content is not shown yet, show it first. Otherwise, show the complete content and play the phrase.
        if (ShowPhraseContent == false)
        {
            ShowPhraseContent = true;
            return;
        }
        ShowCompleteContent = true;
        MainMediaPlayer.Play();
    }

    [RelayCommand]
    void ShowFolder() =>
        this._filesService.ExploreFile(CurrentPlayingPhrase?.VideoLocation ?? "");


    public bool NextCanExecute() => this.CurrentPlayingIndex < this.Phrases.Count - 1;
    public bool PreviousCanExecute() => this.CurrentPlayingIndex > 0;

    public bool HasPlayingPhrase => CurrentPlayingPhrase != null ;

    [RelayCommand(CanExecute = nameof(PreviousCanExecute))]
    void Previous()
    {
        if (!PreviousCanExecute()) return;
        this.MainMediaPlayer.Stop();
        SetCurrentPhraseIndex(--CurrentPlayingIndex);
        ShowCompleteContent = false;
        ShowPhraseContent = false;
    }

    void SetCurrentPhraseIndex(int index)
    {
        this.CurrentPlayingPhrase = null;
        if (index < 0 || index >= this.Phrases.Count) return;
        var phrase = this.Phrases[index];
        SetCurrentPhrase(phrase);
    }


    [RelayCommand]
    void OrderByReviewDate()
    {
        this.Phrases.Sort((a, b) =>
        {
            if (a.NextReviewDate == null && b.NextReviewDate == null) return 0;
            if (a.NextReviewDate == null) return 1;
            if (b.NextReviewDate == null) return -1;
            return DateTime.Compare(a.NextReviewDate, b.NextReviewDate);
        });
        this.SetCurrentPhraseIndex(0);
        ShowCompleteContent = false;
        ShowPhraseContent = false;
    }
    void SetCurrentPhrase(StudyPhrase phrase)
    {
        this.CurrentPlayingIndex = this.Phrases.IndexOf(phrase);
        this.CurrentPlayingPhrase = phrase;

        if (!File.Exists(phrase.VideoLocation))
        {
            Task.Run(async () => await GenerateVideoAsync(phrase)).Wait(); // try to generate the video if it doesn't exist
            if (!File.Exists(phrase.VideoLocation))
            {
                this.ErrorMessages.Add("Video file not found for phrase: " + phrase.PhraseText);
                return;
            }
        }
        var uri = new Uri(phrase.VideoLocation);
        Media media = new Media(this.MainLibVLC, uri);
        MainMediaPlayer.Media?.Dispose();
        MainMediaPlayer.Media = media;
    }

    [RelayCommand]
    public void EditPhrase()
    {
        this.Dialogue = new PhraseEditDialogueViewModel(CurrentPlayingPhrase);
        this.Dialogue.DialogueClosed += async (sender, args) =>
        {
            this.ShowEditDialog = false; // Close the dialog after handling the event
            if (!args.IsClosedSuccessfully)
                return; // User cancelled the dialog
            if (sender is not PhraseEditDialogueViewModel studyEditDialog)
                return; // Invalid sender type

            if (args.Pram is StudyPhrase UpdatedStudyPhrase)
            {
                var oldPhrase = this.Phrases.FirstOrDefault(x => x.StudyId == UpdatedStudyPhrase.StudyId);
                if (oldPhrase is not null)
                {
                    var index = this.Phrases.IndexOf(oldPhrase);
                    this.Phrases[index] = UpdatedStudyPhrase;
                }
            }
            if (args.Pram is Phrase newPhrase)
            {
                var oldPhrase = this.Phrases.FirstOrDefault(x => x.PhraseId == newPhrase.Id);
                oldPhrase!.PhraseText = newPhrase.Text;
                oldPhrase!.StartTime = newPhrase.StartTime;
                oldPhrase!.EndTime = newPhrase.EndTime;
                var index = this.Phrases.IndexOf(oldPhrase);
                Phrases.Remove(oldPhrase);
                Phrases.Insert(index, oldPhrase);
                SetCurrentPhraseIndex(index);
            }

            SetCurrentPhraseIndex(this.CurrentPlayingIndex);
            MainMediaPlayer.Play(); // Re-play the phrase after update
        };
        this.ShowEditDialog = true;
    }

    bool AddReviewCanExecute() => CurrentPlayingPhrase != null && CurrentPlayingPhrase.NextReviewDate <= DateTime.Now;

    [RelayCommand(CanExecute = nameof(AddReviewCanExecute))]
    async Task AddReview()
    {
        var command = new ReviewStudyPhraseCommand(this.CurrentPlayingPhrase!.StudyId, this.Quality);
        var result = await this.mediator.Send(command);
        if (result.IsError)
        {
            this.ErrorMessages.Add("Failed to add review: " + result.Errors.First().Message);
            return;
        }
        // Update the NextReviewDate of the current phrase in the UI
        var updatedPhrase = this.Phrases.FirstOrDefault(p => p.StudyId == this.CurrentPlayingPhrase!.StudyId);
        if (updatedPhrase != null)
        {
            updatedPhrase.NextReviewDate = result.Payload;
            // Refresh the current phrase to update the UI If you will play the same phrase again
        }
        Next();
        OnPropertyChanged(nameof(DuePhrasesCount));
    }

    [RelayCommand(CanExecute = nameof(HasPlayingPhrase))]
    async Task Delete()
    {
        var cmd = new DeleteStudyContentCommand(CurrentPlayingPhrase?.StudyId ?? 0);

        var result = await this.mediator.Send(cmd);
        if (result.IsError)
        {
            this.HandleErrors(result.Errors);
            return;
        }

        // remove the content from the playing list
        var p = this.Phrases.FirstOrDefault(a => a.StudyId == result.Payload);
        this.Phrases.Remove(p);
        SetCurrentPhraseIndex(CurrentPlayingIndex);

    }

    [RelayCommand]
    async Task Init()
    {
        var query = new GetAllStudyPhrasesGroupedQuery();
        var reslt = await this.mediator.Send(query);
        if (reslt.IsSuccess)
        {
            this.Movies.Clear();
            this.Movies.Add(new()
            {
                MovieName = "random phrases",
                StudyCount = reslt.Payload?.Sum(x => x.StudyCount) ?? 0,
            });
            foreach (var m in (reslt.Payload ?? []).OrderByDescending(a => a.StudyCount))
                Movies.Add(m);
        }

    }

    [RelayCommand(CanExecute = nameof(HasPlayingPhrase))]
    void Replay()
    {
        this.MainMediaPlayer.Stop();
        this.MainMediaPlayer.Play();
    }

    [RelayCommand]
    void SetQuality(object parm)
    {
        if (parm is not string str || !int.TryParse(str, out int quality))
            return;
        if (quality < 0 || quality > 5)
            return;
        this.Quality = quality;
    }

    [RelayCommand]
    void Escape()
    {
        if (ShowEditDialog)
            ShowEditDialog = false;
        if (ShowPhraseContent)
            ShowPhraseContent = false;
        if (ShowCompleteContent)
            ShowCompleteContent = false;
        if (MainMediaPlayer.IsPlaying)
            this.MainMediaPlayer.Stop();
    }

    [RelayCommand]
    void Export()
    {

        var vm = new ExportStudiesViewModel(this.Phrases.ToList());
        vm.OnSelectionChanged += (study) =>
        {
            var index = this.Phrases.IndexOf(study);
            SetCurrentPhraseIndex(index);
            MainMediaPlayer.Play();
        };
        this.Dialogue = vm;
        vm.DialogueClosed += (sender, args) =>
        {
            this.ShowEditDialog = false;
        };
        this.ShowPhraseContent = true; // need to show phrase content to make the export dialoge display the phrase text, can be refactored later to make it more elegant
        this.ShowEditDialog = true;
    }

    [RelayCommand]
    async Task GetPhrases()
    {
        this.Phrases.Clear();
        this.CurrentPlayingPhrase = null;

        var qry = new GetAllStudyPhrasesQuery();
        if (SelectedMovie is not null)
            qry.MovieId = SelectedMovie.MovieId;
        var result = await this.mediator.Send(qry);

        if (result.IsError)
        {
            this.ErrorMessages.Add("can't load phrases");
            return;
        }

        var lst = result.Payload;
        foreach (var phrase in lst)
        {
            if (string.IsNullOrWhiteSpace(phrase.VideoLocation))
                await GenerateVideoAsync(phrase);
            this.Phrases.Add(phrase);
        }
        this.SetCurrentPhraseIndex(0);
        ShowCompleteContent = false;
        ShowPhraseContent = false;
    }

    async Task GenerateVideoAsync(StudyPhrase phrase)
    {
        var q = new VideoClipQuery(phrase.PhraseId);
        var videoClipResult = await this.mediator.Send(q);
        if (videoClipResult.IsSuccess)
        {
            phrase.VideoLocation = videoClipResult.Payload!;
        }
        else
        {
            this.ErrorMessages.Add(videoClipResult.Errors.First().Message);
        }
    }
}
