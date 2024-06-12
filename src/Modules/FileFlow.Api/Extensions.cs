using FileFlow.Application;
using Microsoft.Extensions.DependencyInjection;

namespace FileFlow.Api;

public static class Extensions
{
    public static IServiceCollection AddFileFlowApi(this IServiceCollection services)
    {
        services.AddApplicationLayer();
        
        return services;
    }
}