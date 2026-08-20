namespace MovieQuotes.UI.Features.Settings;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;

public partial class SettingsViewModel : PageViewModelBase
{
     

    public SettingsViewModel(IMediator mediator, NavigationService nav) : base(mediator, nav)
    { 
    }

    public override string Title => "Settings";
}