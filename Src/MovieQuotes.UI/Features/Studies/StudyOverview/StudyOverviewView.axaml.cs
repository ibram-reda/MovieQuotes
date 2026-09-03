namespace MovieQuotes.UI.Features.Study;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MovieQuotes.UI.Features.Study;


public partial class StudyOverviewView : UserControl
{
    public StudyOverviewView()
    {
        InitializeComponent();
    } 

    public void UserControl_Loaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (this.DataContext is StudyOverviewViewModel vm)
        {
            vm.InitAsync();
        }
    }
}
