namespace MovieQuotes.UI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using MovieQuotes.Application.Features.MoviePhrases.Commands;
using MovieQuotes.Application.Features.StudyPhrases.Commands;
using MovieQuotes.Application.Features.StudyPhrases.Models;
using MovieQuotes.Application.Features.StudyPhrases.Queries;
using MovieQuotes.Application.Features.VideoClips.Queries;
using MovieQuotes.UI.Extensions;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels.Dialogues;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class StudyViewModel : ViewModelBase
{

    [ObservableProperty] StudyPhrase? currentPlayingPhrase;
    [NotifyCanExecuteChangedFor(nameof(EditPhraseCommand))]
    [ObservableProperty] bool showEditDialog = false;
    public bool IsDialogClosed => !ShowEditDialog;
    [ObservableProperty] StudyPhrasesGroupByMovie? selectedMovie;
    [ObservableProperty] DialogueViewModelBase? dialogue;

    [ObservableProperty] int currentPlayingIndex;
    public ObservableCollection<StudyPhrase> Phrases { get; } = new();
    public ObservableCollection<StudyPhrasesGroupByMovie> Movies { get; } = [];

    private LibVLC MainLibVLC { get; }
    public MediaPlayer MainMediaPlayer { get; }
    private IFilesService _filesService;

    public override string Title => "Study";

    public StudyViewModel()
    {
        MainLibVLC = new();
        MainMediaPlayer = new(MainLibVLC)
        {
            EnableHardwareDecoding = true,
            Volume = 100
        };
        this._filesService = this.GetService<IFilesService>();
    }


    [RelayCommand]
    void Next()
    {
        if (!HasNext) return;
        var phrase = Phrases[++CurrentPlayingIndex];
        PlayPhrase(phrase);
    }

    [RelayCommand]
    void ShowFolder() =>
        this._filesService.ExploreFile(CurrentPlayingPhrase?.VideoLocation ?? "");


    bool HasNext => this.CurrentPlayingIndex < this.Phrases.Count - 1;
    bool HasPrevious => this.CurrentPlayingIndex > 0;

    [RelayCommand]
    void Previous()
    {
        if (!HasPrevious) return;
        var phrase = this.Phrases[--CurrentPlayingIndex];
        PlayPhrase(phrase);
    }

    void PlayPhrase(int index)
    {
        this.CurrentPlayingPhrase = null;
        if (index < 0 || index >= this.Phrases.Count) return;
        var phrase = this.Phrases[index];
        PlayPhrase(phrase);
    }


    [RelayCommand]
    void Shuffle()
    {
        this.Phrases.Shuffle();
        PlayPhrase(0);
    }
    void PlayPhrase(StudyPhrase phrase)
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
        var r = MainMediaPlayer.Play(media);

        this.OnPropertyChanged(nameof(HasNext));
        this.OnPropertyChanged(nameof(HasPrevious));
    }

    [RelayCommand(CanExecute = nameof(IsDialogClosed))]
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

            this.IsBusy = true;
            if (studyEditDialog.RequiredPhraseEdit)
            {
                // Update the phrase in the data source
                var updateCommand = new EditPhraseCommand()
                {
                    PhraseId = studyEditDialog.PhraseId,
                    PhraseText = studyEditDialog.PhraseText,
                    StartTime = studyEditDialog.StartTime,
                    EndTime = studyEditDialog.EndTime
                };

                var result = await this.mediator.Send(updateCommand);

                if (result.IsError)
                    this.ErrorMessages.Add("Failed to update phrase: " + result.Errors.First().Message);

                var viewPhrase = this.Phrases.FirstOrDefault(p => p.PhraseId == studyEditDialog.PhraseId);
                if (viewPhrase is not null && result.Payload is not null)
                {
                    viewPhrase.VideoLocation = result.Payload.VideoLocation;
                    viewPhrase.PhraseText = result.Payload.Text;
                    viewPhrase.StartTime = result.Payload.StartTime;
                    viewPhrase.EndTime = result.Payload.EndTime;
                }
            }

            if (studyEditDialog.RequiredContentEdit)
            {
                var editContentCommand = new EditStudyContentCommand()
                {
                    StudyId = studyEditDialog.StudyId,
                    Content = studyEditDialog.Content,
                    Translation = studyEditDialog.Translation,
                    StudyType = studyEditDialog.StudyType
                };

                var contentResult = await this.mediator.Send(editContentCommand);
                if (contentResult.IsError)
                    this.ErrorMessages.Add("Failed to update content: " + contentResult.Errors.First().Message);

                // Update the current playing phrase with the new content
                var viewPhrase = this.Phrases.FirstOrDefault(p => p.StudyId == studyEditDialog.StudyId);

                if (viewPhrase != null)
                {
                    viewPhrase.Content = studyEditDialog.Content;
                    viewPhrase.Translation = studyEditDialog.Translation;
                    viewPhrase.StudyType = studyEditDialog.StudyType;
                }
            }

            this.IsBusy = false;

            PlayPhrase(this.CurrentPlayingIndex); // Re-play the phrase after update
        };
        this.ShowEditDialog = true;
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
            foreach (var m in reslt.Payload ?? [])
                Movies.Add(m);
        }

    }

    [RelayCommand]
    void Replay()
    {
        this.MainMediaPlayer.Stop();
        this.MainMediaPlayer.Play();
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
        PlayPhrase(0); // Play the first phrase by default if available
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
