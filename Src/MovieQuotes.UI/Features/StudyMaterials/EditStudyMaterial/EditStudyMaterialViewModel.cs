namespace MovieQuotes.UI.Features.StudyMaterials.EditStudyMaterial;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.StudyMaterials; 
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels.Dialogues;
using System;
using System.Threading.Tasks;


public partial class EditStudyMaterialViewModel : DialogueViewModelBase
{
    public override string Title => "Edit Study Material";
    public event Action<bool, OperationResult<StudyMaterial>>? OnSaved;

    public int StudyId { get; }
    

    [ObservableProperty] string content = string.Empty;
    [ObservableProperty] string arPhraseTranslation = string.Empty;
    [ObservableProperty] string contentArabicTranslation = string.Empty;
    [ObservableProperty] string definition = string.Empty;
    [ObservableProperty] string partOfSpeech = string.Empty;
    [ObservableProperty] string origin = string.Empty;
    [ObservableProperty] string notes = string.Empty;
    [ObservableProperty] bool isDraft = true;
    [ObservableProperty] string examples = string.Empty;
    [ObservableProperty] string synonyms = string.Empty;
    [ObservableProperty] string level = string.Empty;
    [ObservableProperty] string pronunciation = string.Empty;
    [ObservableProperty] bool isVulgar;

    public string[] AllowedPartOfSpeech { get; } =
        ["noun", "adjective", "verb", "idiom", "phrasal verb", "phrase", "exclamation", "conjunction", "adverb"];

    [System.Obsolete("For design-time use only")]
    public EditStudyMaterialViewModel()
    {
    }

    public EditStudyMaterialViewModel(StudyMaterial studyMaterial)
    {
        StudyId = studyMaterial.Id;
        Content = studyMaterial.Content ?? string.Empty;
        ArPhraseTranslation = studyMaterial.ArPhraseTranslation ?? string.Empty;
        ContentArabicTranslation = studyMaterial.ContentArabicTranslation ?? string.Empty;
        Definition = studyMaterial.Definition ?? string.Empty;
        PartOfSpeech = studyMaterial.PartOfSpeech ?? string.Empty;
        Origin = studyMaterial.Origin ?? string.Empty;
        Notes = studyMaterial.Notes ?? string.Empty;
        IsDraft = studyMaterial.IsDraft;
        Examples = studyMaterial.Examples;
        Synonyms = studyMaterial.Synonyms;
        Level = studyMaterial.Level;
        Pronunciation = studyMaterial.Pronunciation;
        IsVulgar = studyMaterial.IsVulgar;
    }

    [RelayCommand]
    private async Task Save()
    {
        var command = new EditStudyMaterialCommand
        {
            StudyMaterialId = StudyId,
            PartOfSpeech = PartOfSpeech,
            Content = Content,
            Definition = Definition,
            ContentArabicTranslation = ContentArabicTranslation,
            ArPhraseTranslation = ArPhraseTranslation,
            Origin = Origin,
            Notes = Notes,
            IsDraft = IsDraft,
            Examples = Examples,
            Synonyms = Synonyms,
            Level = Level,
            Pronunciation = Pronunciation,
            IsVulgar = IsVulgar
        };

        var result = await mediator.Send(command);
        if (result.IsError)
        {
            HandleErrors(result.Errors);
            return;
        }

        OnSaved?.Invoke(true, result);
        OnClose(true, result.Payload);
    }

    [RelayCommand]
    private void Canceled()
    {
        var result = new OperationResult<StudyMaterial>();
        result.AddError(MovieQuotes.Application.Common.Enums.ErrorCode.CanceledOperation, "Cancelled by user");
        OnSaved?.Invoke(false, result);
        OnClose(false, result);
    }


}
