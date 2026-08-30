namespace MovieQuotes.UI.Features.StudyMaterials.CreateStudyMaterial;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.StudyMaterials; 
using MovieQuotes.UI.ViewModels.Dialogues;
using System;
using System.Threading.Tasks;

public partial class CreateStudyMaterialViewModel : DialogueViewModelBase
{
    public override string Title => "Create Study Material";
    public event Action<bool, OperationResult<StudyMaterial>>? OnSaved;

    public int PhraseId { get; init; }

    [ObservableProperty] private string phraseText = string.Empty;
    [ObservableProperty] private string arPhraseTranslation = string.Empty;
    [ObservableProperty] private string content = string.Empty;
    [ObservableProperty] private string definition = string.Empty;
    [ObservableProperty] private string contentArabicTranslation = string.Empty;
    [ObservableProperty] private string partOfSpeech = string.Empty;
    [ObservableProperty] private string origin = string.Empty;
    [ObservableProperty] private string notes = string.Empty;
    [ObservableProperty] private bool isDraft = true;
    [ObservableProperty] private string examples = string.Empty;
    [ObservableProperty] private string synonyms = string.Empty;
    [ObservableProperty] private string level = string.Empty;
    [ObservableProperty] private string pronunciation = string.Empty;
    [ObservableProperty] private bool isVulgar;

    public string[] AllowedPartOfSpeech { get; } =
        ["noun", "adjective", "verb", "idiom", "phrasal verb", "phrase", "exclamation", "conjunction", "adverb"];

    [Obsolete("For design-time use only")]
    public CreateStudyMaterialViewModel()
    {
    }

    public CreateStudyMaterialViewModel(int phraseId, string phraseText, string arPhraseTranslation)
    {
        PhraseId = phraseId;
        PhraseText = phraseText;
        ArPhraseTranslation = arPhraseTranslation;
    }

    [RelayCommand]
    private async Task Save()
    {
        ErrorMessages.Clear();
        var command = new AddNewStudyMaterialCommand
        {
            PhraseId = PhraseId,
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
