using Comments.API.Interfaces;
using Ratbags.Comments.API.Interfaces;
using Ratbags.Comments.API.Repositories;
using Ratbags.Comments.API.Services;

namespace Ratbags.Comments.API.ServiceExtensions;

public static class DIServiceExtension
{
    public static IServiceCollection AddDIServiceExtension(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<IService, Service>();
        services.AddScoped<IRepository, Repository>();

        return services;
    }
}
