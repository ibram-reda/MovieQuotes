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
        var result =  await this.mediator.Send(query, token);
         
        foreach (var phrase in result?.Payload ?? []) 
        {  
            Phrases.Add(phrase!.Text);
        } 
    }
     
}
