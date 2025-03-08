using Context;
using Controllers;
using Factories;
using Mappers;
using Repositories;
using System.Text.Json.Serialization;
using Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var provider = builder.Configuration["Provider"];
switch (provider)
{
    case "Sqlite":
        {
            builder.Services.AddDbContext<GameContext, SqliteGameContext>();
            break;
        }
    case "SqlServer":
        {
            builder.Services.AddDbContext<GameContext, SqlServerGameContext>();
            break;
        }
    default:
        {
            throw new ArgumentException($"Invalid provider: {provider}");
        }
}

builder.Services.AddSingleton<WebSocketConnectionManager>();
builder.Services.AddScoped<EntityMapper>();
builder.Services.AddScoped<ModelMapper>();
builder.Services.AddScoped<GameRepository>();
builder.Services.AddScoped<WorldRepository>();
builder.Services.AddSingleton<RegionMapFactory>();
builder.Services.AddSingleton<DefaultWorldFactory>();

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
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(Constants.WebSocketKeepAliveInterval)
});
app.MapControllers();
app.Run();
