namespace MovieQuotes.UI.Features.Subtitles.EditeSubtitlePhrase;

using Avalonia.Controls;
using System.Threading.Tasks;

public partial class EditeSubtitlePhraseView : UserControl
{
    public EditeSubtitlePhraseView()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (this.DataContext is EditeSubtitlePhraseViewModel vm)
            await vm.InitAsync(null);
    }
}