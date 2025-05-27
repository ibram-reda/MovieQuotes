namespace MovieQuotes.UI.ViewModels.Dialogues;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieQuotes.Application.Features.StudyPhrases.Models;
using System;


public partial class PhraseEditDialogueViewModel : DialogueViewModelBase
{
    public int PhraseId { get; init; }
    [ObservableProperty] string phraseText = string.Empty;
    [ObservableProperty] TimeSpan startTime = TimeSpan.Zero;
    [ObservableProperty] TimeSpan endTime = TimeSpan.Zero;

    public PhraseEditDialogueViewModel(StudyPhrase? currentPlayingPhrase)
    {
        this.PhraseId = currentPlayingPhrase?.PhraseId ?? 0;
        this.PhraseText = currentPlayingPhrase?.PhraseText ?? string.Empty;
        this.StartTime = currentPlayingPhrase?.StartTime ?? TimeSpan.Zero;
        this.EndTime = currentPlayingPhrase?.EndTime ?? TimeSpan.Zero;
    }

    [RelayCommand]
    private void Save()
    {
        this.OnClose(true);
    }
}