namespace MovieQuotes.UI.Extensions;

using Avalonia.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieQuotes.Application.Features.Movies.Commands;
using MovieQuotes.Infrastructure;
using MovieQuotes.Infrastructure.Data;
using MovieQuotes.Domain.Interfaces;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;
using System.Linq;
using Avalonia.Controls.Templates;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection Services, Window window)
    {
        Services.AddSingleton<IFilesService>(x => new FilesService(window));
        Services.AddSingleton<NavigationService>();
        Services.AddTransient<IDataTemplate, ViewLocator>();


        var viewModelsTypes = typeof(ViewModelBase).Assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(ViewModelBase)))
            .Where(t => !t.IsAbstract);
        foreach (var type in viewModelsTypes)
        {
            Services.AddTransient(type);
        }

        Services.AddScoped<IMovieQUnitOfWork, MovieQUnitOfWork>();

        Services.AddLogging(); 

        // add database
        var cs = "Server=localhost;Database=MovieQuotesDb;uid=root;pwd=root;";
        Services.AddDbContext<MovieQuotesDbContext>(op => op.UseMySQL(cs), ServiceLifetime.Transient);

        Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateMovieCommand).Assembly));
    }
}