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
            vm.InitAsync(null);
    }

    private void ScrollViewer_ScrollChanged(object? sender, Avalonia.Controls.ScrollChangedEventArgs e)
    {
        var offset = scrollViewer.Offset;
        var extent = scrollViewer.Extent;
        var viewport = scrollViewer.Viewport;

        // Check if scrolled to bottom (with a 1000-pixel tolerance)
        var isAtBottom = offset.Y >= extent.Height - viewport.Height - 1000;

        if (isAtBottom)
        {
            if (DataContext is MoviesListViewModel vm && vm is not null && !vm.IsBusy&& vm.IsInit)
                vm.GetAllMoviesCommand.Execute(null);
        }
    }
}