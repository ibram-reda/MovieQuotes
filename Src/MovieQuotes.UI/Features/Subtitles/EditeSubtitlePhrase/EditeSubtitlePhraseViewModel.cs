namespace MovieQuotes.UI.Features.Subtitles.EditeSubtitlePhrase;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MovieQuotes.Application.Features.MoviePhrases.Commands;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Application.Features.MoviePhrases.Queries;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;
using System;
using System.Threading.Tasks;

public partial class EditeSubtitlePhraseViewModel : ViewModelBase
{
    public override string Title => "Edite Subtitle Phrase";

    public event Action<Phrase?>? OnPhraseEdited;
    public int PhraseId { get; init; }
    [ObservableProperty] string phraseText = string.Empty;
    [ObservableProperty] TimeSpan startTime = TimeSpan.Zero;
    [ObservableProperty] TimeSpan endTime = TimeSpan.Zero;
    [ObservableProperty] string message;

    [System.Obsolete("For design-time use only")]
    public EditeSubtitlePhraseViewModel()
    {
    }

    public EditeSubtitlePhraseViewModel(int PhraseId):base(null,null)
    {
        this.mediator = GetService<IMediator>();
        this.NavigationService = GetService<NavigationService>();
        this.PhraseId = PhraseId;
    }
    public EditeSubtitlePhraseViewModel(IMediator mediator, NavigationService nav) : base(mediator, nav)
    {
        this.PhraseId = PhraseId;
    }

    public override async Task InitAsync(object? initValue)
    {
        var query = new GetSubtitlePhraseByIdQuery(this.PhraseId);
        var result = await mediator.Send(query);
        if (result.IsError)
        {
            foreach(var err in result.Errors)
                ErrorMessages.Add(err.Message);
            return;
        }

        this.PhraseText = result.Payload.Text;
        this.StartTime = result.Payload.StartTime;
        this.EndTime = result.Payload.EndTime;
    }

    [RelayCommand]
    async Task Save()
    {
        this.Message = string.Empty;
        var query = new EditPhraseCommand
        {
            PhraseId = this.PhraseId,
            PhraseText = this.PhraseText,
            StartTime = this.StartTime,
            EndTime = this.EndTime,
        };
        var result = await this.mediator.Send(query);
        if (result.IsError)
        {
            this.Message = "Failed to edit phrase:";
            foreach (var err in result.Errors)
                ErrorMessages.Add(err.Message);
            return;
        }
        this.Message = "Phrase edited successfully.";
        OnPhraseEdited?.Invoke(result.Payload);
    }
}
