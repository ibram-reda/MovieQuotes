namespace MovieQuotes.UI.Features.Settings;

using Avalonia.Platform.Storage;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;

public partial class SettingsViewModel : PageViewModelBase
{
     
    public SettingsService Settings { get; }
    private readonly IFilesService? filesService;
    private readonly INotificationService notification;

    public SettingsViewModel(IMediator mediator, NavigationService nav, SettingsService settings, IFilesService filesService, INotificationService notification) : base(mediator, nav)
    {
        Settings = settings;
        this.filesService = filesService;
        this.notification = notification;
    }

    public override string Title => "Settings";

    [RelayCommand]
    private async Task ChooseMoviesLibraryFolderAsync()
    {
        if (filesService is null)
            return;

        var folder = await filesService.OpenFolderAsync("Select movies library folder");
        var path = folder?.TryGetLocalPath();
        if (!string.IsNullOrWhiteSpace(path))
            Settings.Current.MoviesLibraryPath = path;
    }

    [RelayCommand]
    private async Task ChooseVideoCacheFolderAsync()
    {
        if (filesService is null)
            return;

        var folder = await filesService.OpenFolderAsync("Select video cache folder");
        var path = folder?.TryGetLocalPath();
        if (!string.IsNullOrWhiteSpace(path))
            Settings.Current.VideoCacheFolderPath = path;
    }

    [RelayCommand]
    async Task SaveSettingsAsync()
    {
        await Settings.SaveAsync(Settings.Current); 
        notification.ShowSuccess("Settings", "Settings saved successfully.");
    }

}