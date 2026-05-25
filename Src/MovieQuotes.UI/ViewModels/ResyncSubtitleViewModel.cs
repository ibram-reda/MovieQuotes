namespace MovieQuotes.UI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MovieQuotes.Application.Features.Movies.Commands;
using MovieQuotes.Application.Features.Movies.CommandsHandlers;
using MovieQuotes.Application.Features.SubtitleFiles.Commands;
using MovieQuotes.UI.Services;
using System;
using System.Text;
using System.Threading.Tasks;

partial class ResyncSubtitleViewModel : PageViewModelBase
{
    public override string Title => "Resync Subtitle";

    [ObservableProperty] string path = "";
    [ObservableProperty] string outFileName = "";
    [ObservableProperty] int timeShiftInMS = 0;

    [Obsolete("For design-time use only")]
    public ResyncSubtitleViewModel()
    {        
    }

    public ResyncSubtitleViewModel(IMediator mediator, NavigationService nav) : base(mediator, nav)
    {
        
    }

    [RelayCommand]
    private async Task Resync()
    {
        var cmd = new GenerateInfoFilesCommand();
        IsBusy = true;
        var result = await this.mediator.Send(cmd);
        IsBusy = false;

        if (result.IsError)
        {
            foreach (var err in result.Errors)
                this.ErrorMessages.Add(err.Message);
        }
    }
}
