namespace MovieQuotes.UI.Extensions;

using Hangfire;
using Hangfire.MySql;
using Avalonia.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieQuotes.Application.Features.Movies.Commands;
using MovieQuotes.Infrastructure;
using MovieQuotes.Infrastructure.Data;
using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Infrastructure.Repositories;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;
using System.Linq;
using Avalonia.Controls.Templates;
using MovieQuotes.UI.Views;
using Avalonia.Controls.Notifications;
using Microsoft.Extensions.Configuration;
using MovieQuotes.Application.Features.Movies.Services;
using System.IO;
using System;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.AI;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection Services, Window window)
    {
        var settingsDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MovieQuotes");
        var settingsFilePath = Path.Combine(settingsDirectory, "appsettings.json");
        var configuration = new ConfigurationBuilder()
        .AddJsonFile(settingsFilePath, optional: false, reloadOnChange: true)
        .AddUserSecrets<Program>()
        .Build();
        // Add SettingsService
        Services.AddSingleton<SettingsService>(sp =>
        { 
            return new SettingsService(settingsFilePath, configuration);
        });
        Services.AddSingleton<IFilesService>(x => new FilesService(window));
        Services.AddSingleton<NavigationService>();
        Services.AddTransient<IDataTemplate, ViewLocator>();
        Services.AddSingleton<INotificationService, NotificationService>();
        Services.AddSingleton<MediaService>();
        Services.AddScoped<WindowNotificationManager>(x => ((MainWindow)window).manger);


        var viewModelsTypes = typeof(ViewModelBase).Assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(ViewModelBase)))
            .Where(t => !t.IsAbstract);
        foreach (var type in viewModelsTypes)
        {
            Services.AddTransient(type);
        }

        Services.AddScoped<IMovieQUnitOfWork, MovieQUnitOfWork>();
        Services.AddScoped<IStudySessionRepository, StudySessionRepository>();

        Services.AddLogging();

        // add database
        var cs = configuration["ConnectionString"];
        var hangfire = configuration.GetConnectionString("HangfireConnection");
        var TmdbApiKey = configuration["TmdbApiKey"];
        Services.AddDbContext<MovieQuotesDbContext>(op => op.UseMySQL(cs), ServiceLifetime.Transient);

        Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateMovieCommand).Assembly));

        // Add Hangfire services
        Services.AddHangfire(hangfireConfig => hangfireConfig
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseStorage(new MySqlStorage(hangfire, new MySqlStorageOptions
            {

            })));

        Services.AddHangfireServer();

        

        Services.AddSingleton<AppSettings>(sp =>
        {
            var settingsService = sp.GetRequiredService<SettingsService>();
            return settingsService.Current.ToAppSettings();
        });

        Services.AddSingleton<LLMService>(sp =>
        {
            var appSettings = sp.GetRequiredService<AppSettings>();
            return new LLMService(appSettings.OllamaApiUrl, appSettings.OllamaModelName);
        });

        Services.AddSingleton<TmdbService>(sp =>
        {
            var appSettings = sp.GetRequiredService<AppSettings>();
            return new TmdbService(appSettings.TmdbApiKey);
        });

    }
}