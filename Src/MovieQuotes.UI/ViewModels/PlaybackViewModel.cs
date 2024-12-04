namespace MovieQuotes.UI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MovieQuotes.Application.Operations.Queries;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;


public partial class PlaybackViewModel : ViewModelBase
{
    [ObservableProperty] private string searchText = "";
    [ObservableProperty] private int searchCount = 0;
    public ObservableCollection<string> Phrases { get; } = new();

    public override string Title => "Search for phrase";

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task search(CancellationToken token = default)
    {
        Phrases.Clear();
        var query = new SearchForPhraseQuery(SearchText) 
        { 
            ResultPerPage = 1000,
        };
        IsBusy = true;
        var result =  await this.mediator.Send(query, token);
        IsBusy = false;
        
        SearchCount = result.Count;
        foreach (var phrase in result?.Payload ?? []) 
        {  
            Phrases.Add( $"[{phrase.MovieName}] "+ phrase!.Text);
        } 
    }
     
}
