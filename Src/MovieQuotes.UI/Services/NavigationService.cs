namespace MovieQuotes.UI.Services;

using Microsoft.Extensions.DependencyInjection;
using MovieQuotes.UI.ViewModels;
using System;

public class NavigationService
{
    public NavigationService()
    {

    }
    private ViewModelBase? _CurrentViewModel;
    public event Action<ViewModelBase>? CurrentViewModelChanged;
    public ViewModelBase CurrentViewModel
    {
        get => _CurrentViewModel!;
        set
        {
            _CurrentViewModel = value;
            CurrentViewModelChanged?.Invoke(_CurrentViewModel);
        }
    }

    public void NavigateTo<T>() where T : ViewModelBase 
    {
        CurrentViewModel = App.Current?.Services?.GetRequiredService<T>()!;
    }


}
