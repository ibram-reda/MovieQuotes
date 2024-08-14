namespace MovieQuotes.UI.Extensions;

using Avalonia.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using MovieQuotes.Application.Operations.Commands;
using MovieQuotes.Infrastructure;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection Services, Window window)
    {
        Services.AddSingleton<IFilesService>(x => new FilesService(window));
        Services.AddSingleton<NavigationService>();


        Services.AddTransient<WelcomeScreenViewModel>();
        Services.AddTransient<NewMovieViewModel>();
        Services.AddTransient<PlaybackViewModel>();
        Services.AddTransient<MainWindowViewModel>();

        Services.AddAutoMapper(typeof(MainWindowViewModel));

        // add database
        var cs = "Server=localhost;Database=MovieQuotesDb;Trusted_Connection=True;TrustServerCertificate=True";
        Services.AddDbContext<MovieQuotesDbContext>(op => op.UseSqlServer(cs));

        Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateMovieCommand).Assembly));
    }
}