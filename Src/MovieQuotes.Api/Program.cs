using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Features.Movies.Commands;
using MovieQuotes.Infrastructure;

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

var cs = builder.Configuration.GetConnectionString("local");
builder.Services.AddDbContext<MovieQuotesDbContext>(op => op.UseSqlServer(cs));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateMovieCommand).Assembly));
// autom mapper
builder.Services.AddAutoMapper(typeof(Program), typeof(CreateMovieCommand));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




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
