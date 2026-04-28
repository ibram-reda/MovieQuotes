namespace MovieQuotes.UI.Features.StudyVocabs.CreateVoab;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.StudyPhrases.Commands;
using MovieQuotes.Application.Features.StudyPhrases.Models;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;
using MovieQuotes.UI.ViewModels.Dialogues;
using System;
using System.Threading.Tasks;


internal partial class CreateStudyVocabViewModel : DialogueViewModelBase
{
    public override string Title => "Create New Vocab";
    public event Action<bool,OperationResult<StudyPhrase>>? OnSaved;

    public int PhraseId { get; init; }

    [ObservableProperty] string phraseText = string.Empty;

    [ObservableProperty] string content = string.Empty;
    [ObservableProperty] string arContentTranslation = string.Empty;
    [ObservableProperty] string arPhraseTranslation = string.Empty;

    [ObservableProperty] string translation = string.Empty;
    [ObservableProperty] string studyType = string.Empty;
    [ObservableProperty] string origin = string.Empty;
    [ObservableProperty] string notes = string.Empty;
    [ObservableProperty] bool isDraft = true;
    [ObservableProperty] string examples = string.Empty;
    [ObservableProperty] string synonyms = string.Empty;
    [ObservableProperty] string level = string.Empty;
    [ObservableProperty] string pronunciation = string.Empty;
 

    public string[] AllowedType { get; } = ["noun", "adjective", "verb", "idiom", "phrasal verb", "phrase", "exclamation", "conjunction", "adverb"];

    [System.Obsolete("For design-time use only")]
    public CreateStudyVocabViewModel()
    {        
    }
     
    public CreateStudyVocabViewModel( int phraseId,string phraseText,string arText)
    {        
        PhraseId = phraseId;
        PhraseText = phraseText;
        ArPhraseTranslation = arText;
    }

    [RelayCommand]
    private async Task Save()
    {
        var cmd = new CreateStudyPhraseCommand
        {
            PhraseId = this.PhraseId,
            Content = this.Content,
            Translation = this.Translation,
            StudyType = this.StudyType,
            ArContentTranslation = this.ArContentTranslation,
            ArPhraseTranslation = this.ArPhraseTranslation,
            Origin = this.Origin,
            Notes = this.Notes,
            IsDraft = this.IsDraft,
            Examples = this.Examples,   
            Synonyms = this.Synonyms,
            Level = this.Level,
            Pronunciation = this.Pronunciation
        };

        var result = await mediator.Send(cmd);

        if (result.IsError)
        {
            foreach (var err in result.Errors)
                this.ErrorMessages.Add(err.Message);
        }

        OnSaved?.Invoke(result.IsSuccess,result);
        this.OnClose(result.IsSuccess);
    }

    [RelayCommand]
    private void Canceled()
    {
        var res = new OperationResult<StudyPhrase>();
        res.AddError( Application.Common.Enums.ErrorCode.CanceledOperation, "Cancelled by user");
        OnSaved?.Invoke(false,res);
        this.OnClose(false,res);
    }

}
