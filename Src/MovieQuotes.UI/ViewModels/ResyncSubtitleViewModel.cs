namespace MovieQuotes.UI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieQuotes.Application.Features.AI.GenerateAISuggestionForMaterials;
using MovieQuotes.Application.Features.SubtitleFiles.Commands;
using MovieQuotes.UI.Services;
using System;
using System.Text;
using System.Threading.Tasks;

partial class ResyncSubtitleViewModel : PageViewModelBase
{
    private readonly GenerateAISuggestionForMaterialsCommandHandler generateSuggestionHandler;
    public override string Title => "Resync Subtitle";

    [ObservableProperty] string path = "";
    [ObservableProperty] string outFileName = "";
    [ObservableProperty] int timeShiftInMS = 0;

    [Obsolete("For design-time use only")]
    public ResyncSubtitleViewModel()
    {        
    }

    public ResyncSubtitleViewModel(
        GenerateAISuggestionForMaterialsCommandHandler generateSuggestionHandler,
        NavigationService nav) : base(nav)
    {
        this.generateSuggestionHandler = generateSuggestionHandler;
    }

    [RelayCommand]
    private async Task Resync()
    {
        var cmd = new GenerateAISuggestionForMaterialsCommand();
        IsBusy = true;
        var result = await this.generateSuggestionHandler.Handle(cmd, CancellationToken.None);
        IsBusy = false;

        if (result.IsError)
        {
            foreach (var err in result.Errors)
                this.ErrorMessages.Add(err.Message);
        }
    }
}
