using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application;
using Application.GenreDetection;
using Application.Interfaces;
using Infrastructure.AI;

namespace IoC;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTypingAdventureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IAiClient>(sp => new AiClient(configuration));
        services.AddTransient<GenreDetector>();
        services.AddTransient<GameEngine>();

        return services;
    }
}
