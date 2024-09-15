namespace MovieQuotes.UI.ViewModels;

using CommunityToolkit.Mvvm.Input;
using MovieQuotes.Application.Models;
using MovieQuotes.Application.Operations.Queries;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


public partial class MoviesListViewModel : ViewModelBase
{
    public ObservableCollection<MovieInfo> MoviesInfo { get; } = new();

    public MoviesListViewModel()
    {
        GetAllMoviesCommand.Execute(this);
    }


    [RelayCommand]
    public async Task GetAllMovies()
    {
        var query = new GetAllMoviesQuery();
        var result = await this.mediator.Send(query);

        if (result.IsError)
        {
            foreach (var error in result.Errors)
                ErrorMessages?.Add(error.Message);
            return;
        }

        foreach (var movie in result?.Payload ?? []) 
        {
            MoviesInfo.Add(movie);
        }
    }
}