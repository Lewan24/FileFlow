using FileFlow.Application.Services;
using FileFlow.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FileFlow.Application;

public static class Extensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddTransient<RepoService>();
        services.AddTransient<IRepoService>(s => s.GetRequiredService<RepoService>());
        services.AddTransient<IRepoConfigService>(s => s.GetRequiredService<RepoService>());
        services.AddTransient<IRepoChangesService>(s => s.GetRequiredService<RepoService>());
        
        return services;
    }
}