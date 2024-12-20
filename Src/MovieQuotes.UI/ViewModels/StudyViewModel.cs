namespace MovieQuotes.UI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using MovieQuotes.Application.Features.StudyPhrases.Models;
using MovieQuotes.Application.Features.StudyPhrases.Queries;
using MovieQuotes.Application.Features.VideoClips.Queries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public partial class StudyViewModel : ViewModelBase
{
    [ObservableProperty] int totalCountOfStudyPhrase = 0;
    [ObservableProperty] int readyPhrase = 0;
    [ObservableProperty] StudyPhrase? currentPlayingPhrase;

    private LibVLC MainLibVLC { get; }
    public MediaPlayer MainMediaPlayer { get; } 

    public Stack<StudyPhrase> PlayedPhrase { get; } = new();
    public LinkedList<StudyPhrase> Phrases { get; } = new();
    public override string Title => "Study";

    public StudyViewModel()
    {
        MainLibVLC = new();
        MainMediaPlayer = new(MainLibVLC)
        {
            EnableHardwareDecoding = true,
        }; ;

        MainMediaPlayer.EndReached += MainMediaPlayer_EndReached;
    }

    private void MainMediaPlayer_EndReached(object? sender, EventArgs e)
    {
    }

    [RelayCommand ]
    void Next()
    {
        if (this.CurrentPlayingPhrase is not null)
            this.PlayedPhrase.Push(CurrentPlayingPhrase);

        if(HasNext)
        {
            var phrase = Phrases.Last();
            this.Phrases.RemoveLast();
            PlayPhrase(phrase);
        }
        
    }

    bool HasNext => this.Phrases.Count > 0;
    bool HasPrevious => this.PlayedPhrase.Count > 0;

    [RelayCommand ]
    void Previous()
    {
        if (CurrentPlayingPhrase is not null)
            this.Phrases.AddLast(CurrentPlayingPhrase);
        if (HasPrevious)
        {
            var phrase = this.PlayedPhrase.Pop();
            PlayPhrase(phrase);
        }
    }

    void PlayPhrase(StudyPhrase phrase)
    {
        this.CurrentPlayingPhrase = phrase;
        var uri = new Uri(phrase.VideoLocation);
        Media media = new Media(this.MainLibVLC, uri);
        MainMediaPlayer.Media?.Dispose(); 
        var r = MainMediaPlayer.Play(media);

        this.OnPropertyChanged(nameof(HasNext));
        this.OnPropertyChanged(nameof(HasPrevious));
    }



    [RelayCommand]
    async Task GetPhrases()
    {
        var qry = new GetAllStudyPhrasesQuery();
        var result = await this.mediator.Send(qry);

        if (result.IsSuccess)
        {

            var lst = result.Payload?.DistinctBy(a => a.PhraseId);
            this.TotalCountOfStudyPhrase = lst.Count();
            foreach (var phrase in lst)
            {
                var q = new VideoClipQuery(phrase.PhraseId);
                var videoClipResult = await this.mediator.Send(q);
                if (videoClipResult.IsSuccess)
                {
                    ReadyPhrase++;
                    phrase.VideoLocation = videoClipResult.Payload!;
                    this.Phrases.AddLast(phrase);
                }
                else
                {
                    this.ErrorMessages.Add(videoClipResult.Errors.First().Message);
                }
            }
        }

        this.Next();

    }
}
