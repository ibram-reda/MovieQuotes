namespace MovieQuotes.UI.Features.StudyMaterials.BrowseStudyMaterials;

using System.Threading.Tasks;
using Avalonia.Controls;

public partial class BrowseStudyMaterialsView : UserControl
{
    public BrowseStudyMaterialsView()
    {
        InitializeComponent();
    }

    private async void BrowseStudyMaterialsView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is BrowseStudyMaterialsViewModel viewModel)
        {
             await viewModel.InitAsync(null);           
        }
    }
}