namespace MovieQuotes.UI;

using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Styling; 
using Microsoft.Extensions.DependencyInjection;
using MovieQuotes.UI.Extensions;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;
using MovieQuotes.UI.Views;


public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {

        // Register all the services needed for the application to run
        var collection = new ServiceCollection();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            // add services
            collection.AddCommonServices(desktop.MainWindow);
            // Creates a ServiceProvider containing services from the provided IServiceCollection
            Services = collection.BuildServiceProvider();


            // // Get the SettingsService from the service provider
            var settingsService = Services.GetRequiredService<SettingsService>();
            RequestedThemeVariant = settingsService.Current.IsDarkMode ? ThemeVariant.Dark : ThemeVariant.Light;

            // set the dataContext 
            desktop.MainWindow.DataContext = Services.GetRequiredService<MainWindowViewModel>();
            
        }
        base.OnFrameworkInitializationCompleted();
    }

    public new static App? Current => Application.Current as App;

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
    /// </summary>
    public IServiceProvider? Services { get; private set; }
}