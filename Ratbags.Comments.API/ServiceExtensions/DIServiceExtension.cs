using Comments.API.Interfaces;
using Ratbags.Comments.API.Interfaces;
using Ratbags.Comments.API.Repositories;
using Ratbags.Comments.API.Services;

namespace Ratbags.Comments.API.ServiceExtensions;

public static class DIServiceExtension
{
    public static IServiceCollection AddDIServiceExtension(this IServiceCollection services)
    {
        services.AddScoped<IRepository, Repository>();
        services.AddScoped<IService, Service>();

        return services;
    }
}
