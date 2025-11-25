using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MovieQuotes.UI.Features.StudyVocabs.EditVocab;

public partial class UpdateStudyVocabView : UserControl
{
    public UpdateStudyVocabView()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

        var vm = this.DataContext as UpdateStudyVocabViewModel;
        await vm?.InitAsync(null);
    }
}