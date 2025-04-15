using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NumberGuessingGame;
using NumberGuessingGame.Core;
using NumberGuessingGame.Data;
using NumberGuessingGame.UI;
using System.IO.Abstractions;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddSingleton<IRandomNumberGenerator, SystemRandomNumberGenerator>();
        services.AddTransient<IHighScoreData, HighScoreData>();
        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddTransient<IGameTimer, GameTimer>();
        services.AddTransient<IGameEngine, GameEngine>();
        services.AddTransient<IGameUI, GameUI>();
    })
    .Build();

using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    services.GetRequiredService<IGameUI>().LaunchGame();
}
catch (Exception ex)
{
    Console.WriteLine($"An error has occurred: {ex.Message}");
    Console.ReadLine();
}