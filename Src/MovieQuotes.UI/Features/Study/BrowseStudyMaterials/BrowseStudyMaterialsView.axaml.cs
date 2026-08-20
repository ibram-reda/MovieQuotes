namespace MovieQuotes.UI.Features.Study.BrowseStudyMaterials;

using Avalonia.Controls;

public partial class BrowseStudyMaterialsView : UserControl
{
    public BrowseStudyMaterialsView()
    {
        InitializeComponent();
    }

    private void BrowseStudyMaterialsView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is BrowseStudyMaterialsViewModel viewModel)
            viewModel.LoadMaterialsCommand.Execute(null);
    }
}