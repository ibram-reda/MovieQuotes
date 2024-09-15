namespace MovieQuotes.UI.Extensions;

using Avalonia.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieQuotes.Application.Operations.Commands;
using MovieQuotes.Infrastructure;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;
using System.Linq;
using System.Reflection;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection Services, Window window)
    {
        Services.AddSingleton<IFilesService>(x => new FilesService(window));
        Services.AddSingleton<NavigationService>();


        var viewModelsTypes = typeof(ViewModelBase).Assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(ViewModelBase)))
            .Where(t => !t.IsAbstract);
        foreach (var type in viewModelsTypes)
        {
            Services.AddTransient(type);
        } 

        Services.AddLogging();

        Services.AddAutoMapper(typeof(MainWindowViewModel));

        // add database
        var cs = "Server=localhost;Database=MovieQuotesDb;Trusted_Connection=True;TrustServerCertificate=True";
        Services.AddDbContext<MovieQuotesDbContext>(op => op.UseSqlServer(cs));

        Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateMovieCommand).Assembly));
    }
}