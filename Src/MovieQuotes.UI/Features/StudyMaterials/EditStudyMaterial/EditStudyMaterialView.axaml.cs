namespace MovieQuotes.UI.Features.StudyMaterials.EditStudyMaterial;

using Avalonia.Controls;

public partial class EditStudyMaterialView : UserControl
{
    public EditStudyMaterialView()
    {
        InitializeComponent();
    }
    private async void EditStudtyView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is EditStudyMaterialViewModel viewModel)
        {
            await viewModel.InitAsync(null);
        }
    }

    private void AcceptAiSuggetion(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if(e.Key != Avalonia.Input.Key.Tab || sender is not TextBox input)
            return;

        if(!string.IsNullOrEmpty(input.Text))
            return;

        // Accepte the AI Suggition by tab control
        input.Text = input.PlaceholderText;       
        e.Handled = true;
    }
}