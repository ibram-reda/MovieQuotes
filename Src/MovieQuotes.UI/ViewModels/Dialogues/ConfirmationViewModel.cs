namespace MovieQuotes.UI.ViewModels.Dialogues;

using CommunityToolkit.Mvvm.Input;

public partial class ConfirmationViewModel : DialogueViewModelBase
{
    private readonly string title;

    public ConfirmationViewModel(string title, string message)
    {
        this.title = title;
        Message = message;
    }

    public override string Title => title;
    public string Message { get; }

    [RelayCommand]
    private void Confirm()
    {
        OnClose(true);
    }

    [RelayCommand]
    private void Decline()
    {
        OnClose(false);
    }
}