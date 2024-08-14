using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MovieQuotes.UI.Services;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;

namespace MovieQuotes.UI.ViewModels;

public partial class ViewModelBase : ObservableObject
{
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

    protected T GetService<T>() where T : class
    {
        return App.Current?.Services?.GetService<T>() ??
            throw new ArgumentException("Can not locate Services", nameof(T));
    }
     
}
