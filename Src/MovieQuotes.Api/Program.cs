using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Features.Movies.Commands;
using MovieQuotes.Infrastructure;
using MovieQuotes.Infrastructure.Data;
using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Application.Features.Movies.Services;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.AI;

var builder = WebApplication.CreateBuilder(args);

var AngularOrigins = "_Angular";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AngularOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200",
                                              "https://localhost:4200");
                      });
});

var cs = builder.Configuration.GetConnectionString("DefaultConnection");
var hangfire = builder.Configuration.GetConnectionString("HangfireConnection");
builder.Services.AddDbContext<MovieQuotesDbContext>(op => op.UseMySQL(cs));

builder.Services.AddScoped<IMovieQUnitOfWork, MovieQUnitOfWork>();
var TmdbApiKey = builder.Configuration["TmdbApiKey"];
builder.Services.AddSingleton(new TmdbService(TmdbApiKey ?? ""));

var applicationHandlers = typeof(CreateMovieCommand).Assembly.GetTypes()
    .Where(type => type.IsClass && !type.IsAbstract &&
        (type.Name.EndsWith("Handler", StringComparison.Ordinal) ||
         type.Name.EndsWith("Handlers", StringComparison.Ordinal)));
foreach (var handler in applicationHandlers)
    builder.Services.AddTransient(handler);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<AppSettings>();

builder.Services.AddSingleton<LLMService>(sp =>
 {
     return new LLMService("", "");
 });




var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors(AngularOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
