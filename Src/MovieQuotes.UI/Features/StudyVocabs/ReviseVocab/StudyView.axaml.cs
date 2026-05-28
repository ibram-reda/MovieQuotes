namespace MovieQuotes.UI.Features.StudyVocabs.ReviseVocab;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform; 
using MovieQuotes.UI.ViewModels;

public partial class StudyView : UserControl
{
    public StudyView()
    {
        InitializeComponent();
    }

    private void ComboBox_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        (this.DataContext as StudyViewModel)?.GetPhrasesCommand.Execute(null);
    }

    private void StudyView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        (this.DataContext as StudyViewModel)?.InitCommand.Execute(null);
    }

    public async void TextBlock_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (sender is TextBlock textBlock)
        {
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            
            if (clipboard is not null)
                await clipboard.SetTextAsync(textBlock.Text); 
        }
    }

}