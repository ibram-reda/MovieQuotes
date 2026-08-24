namespace MovieQuotes.UI.Features.Study.StudyMaterialDetails;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using MediatR;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Application.Features.MoviePhrases.Queries;
using MovieQuotes.Application.Features.Study;
using MovieQuotes.Application.Features.Study.AddStudyMaterialToLearning;
using MovieQuotes.Application.Features.Study.AddPhraseToStudyMaterial;
using MovieQuotes.Application.Features.Study.GetStudyMaterial;
using MovieQuotes.Application.Features.Study.UnlinkPhraseFromStudyMaterial;
using MovieQuotes.Application.Features.VideoClips.Queries;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class StudyMaterialDetailsViewModel : PageViewModelBase, IDisposable
{
    private LibVLC MainLibVLC { get; } = null!;
    public MediaPlayer MainMediaPlayer { get; } = null!;

    public override string Title => Details?.Content ?? "Study Material Details";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Title))]
    [NotifyPropertyChangedFor(nameof(HasProgressRecords))]
    private StudyMaterialDetails? details;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddToStudyCommand))]
    private bool isBeingStudied;

    public bool HasProgressRecords => Details?.Progresses.Count > 0;

    public ObservableCollection<Phrase> MatchingPhrases { get; } = [];

    [Obsolete("For design-time use only")]
    public StudyMaterialDetailsViewModel()
    {
    }

    public StudyMaterialDetailsViewModel(IMediator mediator, NavigationService navigation)
        : base(mediator, navigation)
    {
        MainLibVLC = new("--no-video");
        MainMediaPlayer = new(MainLibVLC);
    }

    public override async Task InitAsync(object? initValue)
    {
        if (initValue is not int studyMaterialId || studyMaterialId <= 0)
        {
            ErrorMessages.Add("A valid study material id is required.");
            return;
        }

        IsBusy = true;
        try
        {
            await LoadDetailsAsync(studyMaterialId);
            await SearchMatchingPhrasesAsync(Details?.Content);
        }
        finally
        {
            IsBusy = false;
        }
        this.AddToStudyCommand.NotifyCanExecuteChanged();
    }

    private async Task LoadDetailsAsync(int studyMaterialId)
    {
        var result = await mediator.Send(new GetStudyMaterialQuery(studyMaterialId));
        if (result.IsError)
        {
            HandleErrors(result.Errors);
            return;
        }

        Details = result.Payload;
        IsBeingStudied = Details?.IsCurrentlyLerning == true;
    }

    [RelayCommand(CanExecute = nameof(CanAddToStudy))]
    private async Task AddToStudy()
    {
        if (Details is null || Details.Id <= 0)
            return;

        IsBusy = true;
        try
        {
            var result = await mediator.Send(new AddStudyMaterialToLearningCommand(Details.Id));
            if (result.IsError)
            {
                HandleErrors(result.Errors);
                return;
            }

            await LoadDetailsAsync(Details.Id);
        }
        finally
        {
            IsBusy = false;
            AddToStudyCommand.NotifyCanExecuteChanged();
        }
    }

    private bool CanAddToStudy()
        => !this.IsBusy && this.Details != null && !this.Details.IsCurrentlyLerning;

    [RelayCommand]
    private async Task AddPhrase(Phrase phrase)
    {
        if (phrase is null || Details is null || Details.Id <= 0)
            return;

        IsBusy = true;
        try
        {
            var result = await mediator.Send(new AddPhraseToStudyMaterialCommand(Details.Id, phrase.Id));
            if (result.IsError)
            {
                HandleErrors(result.Errors);
                return;
            }
            MatchingPhrases.Remove(phrase);
            await LoadDetailsAsync(Details.Id);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task UnlinkPhrase(StudyMaterialPhrase phrase)
    {
        if (phrase is null || Details is null || Details.Id <= 0)
            return;

        IsBusy = true;
        try
        {
            var result = await mediator.Send(new UnlinkPhraseFromStudyMaterialCommand(Details.Id, phrase.PhraseId));
            if (result.IsError)
            {
                HandleErrors(result.Errors);
                return;
            }

            var studyMaterialId = Details.Id;
            await LoadDetailsAsync(studyMaterialId);
            await SearchMatchingPhrasesAsync(Details?.Content);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task PlayPhrase(Phrase phrase)
    {
        if (phrase is null)
            return;

        if (string.IsNullOrWhiteSpace(phrase.VideoLocation))
        {
            //Generate the Clip
            var vc = new VideoClipQuery(phrase.Id);
            var r = await this.mediator.Send(vc);
            if (r.IsSuccess)
            {
                phrase.VideoLocation = r.Payload ??"";
            }
        }

        if(!File.Exists(phrase.VideoLocation)){
            ErrorMessages.Add("No video clip is available for this phrase.");
            return;

        }

        try
        {
            MainMediaPlayer.Media?.Dispose();
            MainMediaPlayer.Play(new Media(MainLibVLC, new Uri(phrase.VideoLocation)));
        }
        catch (Exception exception)
        {
            ErrorMessages.Add($"Unable to play the phrase: {exception.Message}");
        }
    }

    private async Task SearchMatchingPhrasesAsync(string? content)
    {
        MatchingPhrases.Clear();
        if (string.IsNullOrWhiteSpace(content))
            return;

        var result = await mediator.Send(new SearchForPhraseQuery(content.Trim())
        {
            ResultPerPage = 100
        });

        if (result.IsError)
        {
            HandleErrors(result.Errors);
            return;
        }

        var materialPhrases = Details?.MaterialPhrases ?? [];
        foreach (var phrase in result.Payload ?? [])
        {
            if(!materialPhrases.Any(a=>a.PhraseId == phrase.Id))
                MatchingPhrases.Add(phrase);
        }
    }

    public void Dispose()
    {
        MainMediaPlayer.Stop();
        MainMediaPlayer.Media?.Dispose();
        MainMediaPlayer.Dispose();
        MainLibVLC.Dispose();
    }
}
