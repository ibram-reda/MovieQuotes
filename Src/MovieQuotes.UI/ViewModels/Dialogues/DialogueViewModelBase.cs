namespace MovieQuotes.UI.ViewModels.Dialogues;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;

public class DialogueCloseEventArgs : EventArgs
{
    public DialogueCloseEventArgs(bool isClosedSuccessfully,object? pram = null )
    {
        IsClosedSuccessfully = isClosedSuccessfully;
        Pram = pram;
    }

    public bool IsClosedSuccessfully { get; }
    public object? Pram { get; }
}

public abstract partial class DialogueViewModelBase : ObservableObject
{
    public event EventHandler<DialogueCloseEventArgs>? DialogueClosed;

    protected virtual void OnClose(bool status,object? pram = null)
    {
        this.DialogueClosed?.Invoke(this, new(status,pram));
    }

    [RelayCommand]
    private void Cancel()
    {
        this.OnClose(false);
    }

    protected T GetService<T>() where T : class
    {
        return MovieQuotes.UI.App.Current?.Services?.GetService<T>() ??
            throw new ArgumentException("Can not locate Services", nameof(T));
    }
}
