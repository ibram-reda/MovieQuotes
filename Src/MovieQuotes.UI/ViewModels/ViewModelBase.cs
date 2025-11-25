namespace MovieQuotes.UI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MovieQuotes.UI.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

public abstract partial class ViewModelBase : ObservableObject
{
    abstract public string Title { get; }
    private IMediator? _mediator;
    private NavigationService? _navigationService;
    protected IMediator mediator { get; set; }
    protected NavigationService NavigationService { get; set; }

    public ObservableCollection<string> ErrorMessages { get; } = [];
    [ObservableProperty] bool _IsBusy = false;

    [System.Obsolete("For design-time use only")]
    protected ViewModelBase()
    {
    }

    [RelayCommand]
    void ClearErrors()
    {
        this.ErrorMessages.Clear();
    }
    protected ViewModelBase(IMediator mediator, NavigationService navigation)
    {
        this.mediator = mediator;
        this.NavigationService = navigation;
    }
    protected T GetService<T>() where T : class
    {
        return App.Current?.Services?.GetService<T>() ??
            throw new ArgumentException("Can not locate Services", nameof(T));
    }

    [RelayCommand]
    private void BackToPrevious()
    {
        this.NavigationService.GoBack();
    }
    public virtual void Init(object? initValue)
    {
    }

    public virtual async Task InitAsync(object? initValue)
    {
    }

    public virtual void ConsumeMessage(object? message)
    {
    }
}
