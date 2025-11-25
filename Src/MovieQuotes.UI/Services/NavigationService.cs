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
    PageViewModelBase? _OldPage = null;   
    private PageViewModelBase? _CurrentPage;
    public event Action<PageViewModelBase>? CurrentPageChanged;
    public PageViewModelBase CurrentViewModel
    {
        get => _CurrentPage!;
        set
        {
            _CurrentPage = value;
            CurrentPageChanged?.Invoke(_CurrentPage);
        }
    }

    public void NavigateTo<T>(object? initValue = null) where T : PageViewModelBase
    {
        _OldPage = CurrentViewModel;
        CurrentViewModel = App.Current?.Services?.GetRequiredService<T>()!;
        if (_OldPage is IDisposable vm) vm.Dispose();
        if (initValue is not null) 
            CurrentViewModel.Init(initValue);

    }

    public async Task NavigateToAsync<T>(object? initValue = null) where T : PageViewModelBase
    {
        _OldPage = CurrentViewModel;
        CurrentViewModel = App.Current?.Services?.GetRequiredService<T>()!;
        if (_OldPage is IDisposable vm) vm.Dispose();
        if (initValue is not null)
            await CurrentViewModel.InitAsync(initValue);

    }

    public void NavigateTo(PageViewModelBase viewModel)
    {
        if(CurrentViewModel == viewModel) return;
        _OldPage = CurrentViewModel;
        CurrentViewModel = viewModel;
        if (_OldPage is IDisposable vm) vm.Dispose();
    }

    public void GoBack(object? message = null)
    {
        if (_OldPage is null)
            return;

        _OldPage.ConsumeMessage(message);
        CurrentViewModel = _OldPage;

    }
}
