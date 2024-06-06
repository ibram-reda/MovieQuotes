namespace MovieQuotes.UI.Extensions;

using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection Services, Window window)
    {
        Services.AddSingleton<IFilesService>(x => new FilesService(window));
        Services.AddTransient<MainWindowViewModel>();
    }
}