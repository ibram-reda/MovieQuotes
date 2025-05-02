namespace MovieQuotes.UI.ViewModels.Dialogues;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

public class DialogueCloseEventArgs : EventArgs
{
    public DialogueCloseEventArgs(bool isClosedSuccessfully )
    {
        IsClosedSuccessfully = isClosedSuccessfully;
    }

    public bool IsClosedSuccessfully { get; }
}

public abstract partial class DialogueViewModelBase : ObservableObject
{
    public event EventHandler<DialogueCloseEventArgs>? DialogueClosed;

    protected virtual void OnClose(bool status)
    {
        this.DialogueClosed?.Invoke(this, new(status));
    }

    [RelayCommand]
    private void Cancel()
    {
        this.OnClose(false);
    }
}
