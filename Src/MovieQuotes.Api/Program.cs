using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Operations.Commands;
using MovieQuotes.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


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

app.UseAuthorization();

app.MapControllers();

app.Run();
