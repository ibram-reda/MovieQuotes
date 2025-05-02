namespace MovieQuotes.UI.Services;

using Microsoft.Extensions.DependencyInjection;
using MovieQuotes.UI.ViewModels;
using System;
using System.Threading.Tasks;

public class NavigationService
{
    public NavigationService()
    {

    }
    ViewModelBase? _OldViewModel = null;   
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

    public void NavigateTo<T>(object? initValue = null) where T : ViewModelBase 
    {
        _OldViewModel = CurrentViewModel;
        CurrentViewModel = App.Current?.Services?.GetRequiredService<T>()!;
        if (_OldViewModel is IDisposable vm) vm.Dispose();
        if (initValue is not null) 
            CurrentViewModel.Init(initValue);

    }

    public async Task NavigateToAsync<T>(object? initValue = null) where T : ViewModelBase
    {
        _OldViewModel = CurrentViewModel;
        CurrentViewModel = App.Current?.Services?.GetRequiredService<T>()!;
        if (_OldViewModel is IDisposable vm) vm.Dispose();
        if (initValue is not null)
            await CurrentViewModel.InitAsync(initValue);

    }

    public void NavigateTo(ViewModelBase viewModel)
    {
        if(CurrentViewModel == viewModel) return;
        _OldViewModel = CurrentViewModel;
        CurrentViewModel = viewModel;
        if (_OldViewModel is IDisposable vm) vm.Dispose();
    }

    public void GoBack(object? message = null)
    {
        if (_OldViewModel is null)
            return;

        _OldViewModel.ConsumeMessage(message);
        CurrentViewModel = _OldViewModel;

    }
}
