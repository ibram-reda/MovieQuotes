using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MovieQuotes.UI.Features.Study;

namespace MovieQuotes.UI.Features.Study;

public partial class RecognitionView : UserControl
{
    public RecognitionView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void UserControl_Loaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (this.DataContext is RecognitionViewModel vm)
        {
            vm.InitAsync();
        }
    }
}
