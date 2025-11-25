namespace MovieQuotes.UI.Features.Movies.BrowseMovies;

using Avalonia.Controls; 

public partial class MoviesListView : UserControl
{
    public MoviesListView()
    {
        InitializeComponent();
    }

    private void MoviesListView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is MoviesListViewModel vm && vm is not null)
            vm.GetAllMoviesCommand.Execute(null);
    }
}