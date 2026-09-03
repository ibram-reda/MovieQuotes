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

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection Services, Window window)
    {
        var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddUserSecrets<Program>()
        .Build();
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
        var cs = configuration.GetConnectionString("DefaultConnection");
        var hangfire = configuration.GetConnectionString("HangfireConnection");
        var TmdbApiKey = configuration["TmdbApiKey"];
        Services.AddSingleton(new TmdbService(TmdbApiKey));
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
    }
}