namespace MovieQuotes.UI.ViewModels.Dialogues;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieQuotes.Application.Features.StudyPhrases.Models;
using MovieQuotes.UI.Features.StudyVocabs.EditVocab;
using MovieQuotes.UI.Features.Subtitles.EditeSubtitlePhrase;
using System;

public partial class PhraseEditDialogueViewModel : DialogueViewModelBase
{
    [ObservableProperty] EditeSubtitlePhraseViewModel editeSubtitlePhraseViewModel;
    [ObservableProperty] UpdateStudyVocabViewModel updateStudyVocabViewModel;

    public PhraseEditDialogueViewModel(StudyPhrase? studyPhrase)
    {
        this.editeSubtitlePhraseViewModel = new EditeSubtitlePhraseViewModel(studyPhrase?.PhraseId??0);
        this.updateStudyVocabViewModel = new UpdateStudyVocabViewModel(studyPhrase?.StudyId ?? 0);

        UpdateStudyVocabViewModel.OnSaved += (isSaved, result) => 
           this.OnClose(isSaved, result); 

        EditeSubtitlePhraseViewModel.OnPhraseEdited += (phrase) =>
              this.OnClose(phrase != null, phrase);
    }

    public override string Title =>  "Edit Phrase";

    [RelayCommand]
    private void Save()
    { 
        this.OnClose(false);
    } 
}