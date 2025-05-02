namespace MovieQuotes.UI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieQuotes.Application.Features.SubtitleFiles.Commands;
using System;
using System.Text;
using System.Threading.Tasks;

partial class ResyncSubtitleViewModel : ViewModelBase
{
    public override string Title => "Resync Subtitle";

    [ObservableProperty] string path = "";
    [ObservableProperty] string outFileName = "";
    [ObservableProperty] int timeShiftInMS = 0;




    [RelayCommand]
    private async Task Resync()
    {
        var cmd = new SubtitleResyncCommand()
        {
            SubtitleFilePath = this.Path.Replace("\"",""),
            OutPutFileName = this.OutFileName,
            TimeShift = this.TimeShiftInMS,
            InputFileEncoding = Encoding.GetEncoding(1256)
        };
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
