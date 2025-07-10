namespace MovieQuotes.UI.ViewModels.Dialogues;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieQuotes.Application.Features.StudyPhrases.Models;
using System;

public partial class PhraseEditDialogueViewModel : DialogueViewModelBase
{
    readonly private StudyPhrase? OriginalPhrase = null;
    public int PhraseId { get; init; }
    public int StudyId { get; init; }
    [ObservableProperty] string phraseText = string.Empty;
    [ObservableProperty] TimeSpan startTime = TimeSpan.Zero;
    [ObservableProperty] TimeSpan endTime = TimeSpan.Zero;

    [ObservableProperty] string content = string.Empty;
    [ObservableProperty] string translation = string.Empty;
    [ObservableProperty] string studyType = string.Empty;

    public string[] AllowedType { get; } = ["noun", "adjective", "verb", "idiom", "phrasal verb", "phrase", "exclamation", "conjunction", "adverb"];


    public bool RequiredPhraseEdit { get; private set; } = false;
    public bool RequiredContentEdit { get; private set; } = false;

    public PhraseEditDialogueViewModel(StudyPhrase? studyPhrase)
    {
        OriginalPhrase = studyPhrase;

        this.StudyId = studyPhrase?.StudyId ?? 0;
        this.PhraseId = studyPhrase?.PhraseId ?? 0;
        this.PhraseText = studyPhrase?.PhraseText ?? string.Empty;
        this.StartTime = studyPhrase?.StartTime ?? TimeSpan.Zero;
        this.EndTime = studyPhrase?.EndTime ?? TimeSpan.Zero;

        this.Content = studyPhrase?.Content ?? string.Empty;
        this.Translation = studyPhrase?.Translation ?? string.Empty;
        this.StudyType = studyPhrase?.StudyType ?? string.Empty;
    }

    [RelayCommand]
    private void Save()
    {
        var NeedToEdit = this.IsEditRequired();
        this.OnClose(NeedToEdit);
    }

    private bool IsEditRequired()
    {
        if (this.OriginalPhrase is null)
            return false;
        this.RequiredContentEdit =
            (this.Content != this.OriginalPhrase.Content) ||
            (this.Translation != this.OriginalPhrase.Translation) ||
            (this.StudyType != this.OriginalPhrase.StudyType);

        this.RequiredPhraseEdit =
            (this.PhraseText != this.OriginalPhrase.PhraseText) ||
            (this.StartTime != this.OriginalPhrase.StartTime) ||
            (this.EndTime != this.OriginalPhrase.EndTime);

        return this.RequiredPhraseEdit || this.RequiredContentEdit;
    }
}