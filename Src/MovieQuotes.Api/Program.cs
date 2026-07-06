using Hangfire;
using Hangfire.MySql;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Features.Movies.Commands;
using MovieQuotes.Infrastructure;
using MovieQuotes.Infrastructure.Data;
using MovieQuotes.Domain.Interfaces;
using MovieQuotes.Application.Features.Movies.Services;

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
builder.Services.AddSingleton(new TmdbService(TmdbApiKey??""));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateMovieCommand).Assembly));
// Add Hangfire services
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseStorage(new MySqlStorage(hangfire, new MySqlStorageOptions
    {
         
    })));

builder.Services.AddHangfireServer();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard("/hangfire", new DashboardOptions ());

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors(AngularOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
