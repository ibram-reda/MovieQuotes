namespace MovieQuotes.UI.Features.StudyVocabs.EditVocab;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Application.Features.StudyPhrases.Models;
using MovieQuotes.Application.Features.StudyPhrases.Queries;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;
using System;
using System.ComponentModel;
using System.Threading.Tasks;


public partial class UpdateStudyVocabViewModel : ViewModelBase
{
    public override string Title => "Update study Vocab";
    public event Action<bool, StudyPhrase?>? OnSaved;

    public int StudyId { get; init; }

    private StudyPhrase? dbPhrase = null;

    public bool NeedUpdate => IsContentChanged();

    [ObservableProperty] string content = string.Empty;
    [ObservableProperty] string arPhraseTranslateion;
    [ObservableProperty] string arContentTranslation = string.Empty;
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
    public UpdateStudyVocabViewModel()
    {
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(Content)
            or nameof(Translation)
            or nameof(StudyType)
            or nameof(ArContentTranslation)
            or nameof(Origin)
            or nameof(Notes)
            or nameof(ArPhraseTranslateion)
            or nameof(IsDraft)
            or nameof(Examples)
            or nameof(Synonyms)
            or nameof(Level)
            or nameof(Pronunciation)
            )
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(NeedUpdate)));
            SaveCommand.NotifyCanExecuteChanged();
        }
        base.OnPropertyChanged(e);
    }
    public UpdateStudyVocabViewModel(int studyId) : base(null, null)
    {
        this.mediator = GetService<IMediator>();
        this.NavigationService = GetService<NavigationService>();

        StudyId = studyId;
    }
    public UpdateStudyVocabViewModel(IMediator mediator, NavigationService nav, int studyId) : base(mediator, nav)
    {
        StudyId = studyId;
    }

    public override async Task InitAsync(object? initValue)
    {
        if (this.dbPhrase is not null)
            return;
        var query = new GetStudyPhraseByIdQuery(this.StudyId);
        var result = await mediator.Send(query);

        if (result.IsError)
        {
            foreach (var err in result.Errors)
                this.ErrorMessages.Add(err.Message);
            return;
        }

        if (result?.Payload is null)
        {
            this.ErrorMessages.Add("Study phrase not found.");
            return;
        }
        this.dbPhrase = result.Payload;
        this.Content = result.Payload.Content;
        this.Translation = result.Payload.Translation;
        this.StudyType = result.Payload.StudyType;
        this.ArContentTranslation = result.Payload.ArContentTranslation;
        this.Origin = result.Payload.Origin;
        this.Notes = result.Payload.Notes;
        this.ArPhraseTranslateion = result.Payload.PhraseArTranslation;
        this.IsDraft = result.Payload.IsDraft;
        this.Examples = result.Payload.Examples;
        this.Synonyms = result.Payload.Synonyms;
        this.Level = result.Payload.Level;
        this.Pronunciation = result.Payload.Pronunciation;
    }

    bool IsContentChanged()
    {
        if (dbPhrase is null)
            return false;
        var res = this.Content != dbPhrase.Content ||
               this.Translation != dbPhrase.Translation ||
               this.StudyType != dbPhrase.StudyType ||
               this.ArContentTranslation != dbPhrase.ArContentTranslation ||
               this.Origin != dbPhrase.Origin ||
               this.Notes != dbPhrase.Notes ||
               this.ArPhraseTranslateion != dbPhrase.PhraseArTranslation ||
               this.IsDraft != dbPhrase.IsDraft ||
               this.Examples != dbPhrase.Examples ||
               this.Synonyms != dbPhrase.Synonyms||
               this.Level != dbPhrase.Level ||
               this.Pronunciation != dbPhrase.Pronunciation;

        return res;
    }

    [RelayCommand(CanExecute = nameof(NeedUpdate))]
    private async Task Save()
    {
        var cmd = new Application.Features.StudyPhrases.Commands.EditStudyContentCommand
        {
            StudyId = this.StudyId,
            Content = this.Content,
            Translation = this.Translation,
            StudyType = this.StudyType,
            ArContentTranslation = this.ArContentTranslation,
            ArPhraseTranslation = this.ArPhraseTranslateion,
            Origin = this.Origin,
            Notes = this.Notes,
            IsDraft = this.IsDraft,
            Examples = PutDashInStartingLines(this.Examples),
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
        dbPhrase = result.Payload!;
        this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(NeedUpdate)));
        OnSaved?.Invoke(result.IsSuccess, result.Payload);
    }

    string PutDashInStartingLines(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;
        var lines = text.Split(Environment.NewLine);
        for (int i = 0; i < lines.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(lines[i]) && !lines[i].TrimStart().StartsWith("-"))
            {
                lines[i] = "- " + lines[i].TrimStart();
            }
        }
        return string.Join(Environment.NewLine, lines);
    }

    [RelayCommand]
    private void Canceled()
    {
        var res = new OperationResult<StudyPhrase>();
        res.AddError(Application.Common.Enums.ErrorCode.CanceledOperation, "Cancelled by user");
        OnSaved?.Invoke(false, null);
    }
}
