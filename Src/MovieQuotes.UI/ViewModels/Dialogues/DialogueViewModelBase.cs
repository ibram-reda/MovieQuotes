namespace MovieQuotes.UI.ViewModels.Dialogues;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MovieQuotes.UI.Services;
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

public abstract partial class DialogueViewModelBase :  ViewModelBase
{
    public DialogueViewModelBase():base(null,null)
    {
        this.mediator= GetService<IMediator>();
        this.NavigationService= GetService<NavigationService>();
    }
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


}
