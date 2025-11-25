using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MovieQuotes.UI.Features.StudyVocabs.ReviseVocab;

public partial class ExportStudiesView : UserControl
{

    public ExportStudiesView()
    {
        InitializeComponent();

    }

    private void ExportStudiesView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        (this.DataContext as ExportStudiesViewModel).Clipboard = (TopLevel.GetTopLevel(this) as Window)?.Clipboard;
    }
}