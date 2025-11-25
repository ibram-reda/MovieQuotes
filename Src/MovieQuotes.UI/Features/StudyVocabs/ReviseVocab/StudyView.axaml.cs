namespace MovieQuotes.UI.Features.StudyVocabs.ReviseVocab;

using Avalonia.Controls;
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
        (this.DataContext as StudyViewModel)?.InitCommand   .Execute(null);
    }
}