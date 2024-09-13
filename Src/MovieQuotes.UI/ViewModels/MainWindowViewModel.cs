namespace MovieQuotes.UI.ViewModels;

using MovieQuotes.UI.Services;


public partial class MainWindowViewModel : ViewModelBase
{

    public MainWindowViewModel()
    {
        NavigationService.NavigateTo<WelcomeScreenViewModel>();

        NavigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;
    }

    private void OnCurrentViewModelChanged(ViewModelBase obj)
    {
        this.OnPropertyChanged(nameof(CurrentViewModel));
    }


    public ViewModelBase CurrentViewModel => NavigationService.CurrentViewModel;
}