namespace MovieQuotes.UI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieQuotes.Application.Operations.Queries;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;


public partial class PlaybackViewModel : ViewModelBase
{
    [ObservableProperty] private string searchText = "";
    public ObservableCollection<string> Phrases { get; } = new();

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task search(CancellationToken token = default)
    {
        Phrases.Clear();
        var query = new SearchForPhraseQuery(SearchText);
        var result = await this.mediator.Send(query, token);
        if (result.IsError)
        {
            foreach (var error in result.Errors)
                ErrorMessages?.Add(error.Message);
            return;
        }

        foreach (var p in result.Payload ?? [])
        {
            Phrases.Add(p.Text);
        }
    }

    [RelayCommand]
    private void BackToWelcomeScreen()
    {
        this.NavigationService.NavigateTo<WelcomeScreenViewModel>();
    }
}
