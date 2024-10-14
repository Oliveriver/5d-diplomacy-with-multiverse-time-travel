using Context;
using Factories;
using Mappers;
using Microsoft.EntityFrameworkCore;
using Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<GameContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddScoped<EntityMapper>();
builder.Services.AddScoped<ModelMapper>();
builder.Services.AddScoped<GameRepository>();
builder.Services.AddScoped<WorldRepository>();
builder.Services.AddScoped<MapFactory>();
builder.Services.AddScoped<DefaultWorldFactory>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(options => options
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
}

app.UseAuthorization();
app.MapControllers();
app.Run();
