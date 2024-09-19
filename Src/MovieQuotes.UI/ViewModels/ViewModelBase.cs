namespace MovieQuotes.UI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MovieQuotes.UI.Services;
using System;
using System.Collections.ObjectModel;


public abstract partial class ViewModelBase : ObservableObject
{
    abstract public string  Title { get; }
    private IMediator? _mediator;
    private NavigationService? _navigationService;
    protected IMediator mediator => this._mediator ??= GetService<IMediator>();
    protected NavigationService NavigationService => _navigationService ??= GetService<NavigationService>();
    protected ViewModelBase()
    {
        ErrorMessages = new ObservableCollection<string>();

    }

    [ObservableProperty]
    private ObservableCollection<string>? _errorMessages;
    [ObservableProperty] private bool _IsBusy = false;
     
    protected T GetService<T>() where T : class
    {
        return App.Current?.Services?.GetService<T>() ??
            throw new ArgumentException("Can not locate Services", nameof(T));
    }


    [RelayCommand]
    private void BackToWelcomeScreen()
    {
        this.NavigationService.NavigateTo<WelcomeScreenViewModel>();
    }

    public virtual void Init(object? initValue)
    { 
    }

    public virtual void ConsumeMessage(object? message)
    {        
    }
}
