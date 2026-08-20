namespace MovieQuotes.UI.ViewModels;

using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using MediatR;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels.Dialogues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract partial class PageViewModelBase : ViewModelBase
{   
    [ObservableProperty] bool showEditDialog = false;
    public bool IsDialogClosed => !ShowEditDialog;
    [ObservableProperty] DialogueViewModelBase? dialogue;

    [Obsolete("For design-time use only")]
    protected PageViewModelBase()
    {
        if(!Design.IsDesignMode)
            throw new Exception("this is allowed in designMode only");
    }

    protected PageViewModelBase(IMediator mediator, NavigationService navigation) : base(mediator, navigation)
    {
    }
}
