using Comments.API.Interfaces;
using Microsoft.Extensions.Options;
using Ratbags.Comments.API.Interfaces;
using Ratbags.Comments.API.Models;
using Ratbags.Comments.API.Repositories;
using Ratbags.Comments.API.Services;

namespace Ratbags.Comments.API.ServiceExtensions;

public static class DIServiceExtension
{
    public static IServiceCollection AddDIServiceExtension(this IServiceCollection services)
    {
        services.AddScoped<ICommentsRepository, CommentsRepository>();
        services.AddScoped<ICommentsService, CommentsService>();

        // expose appSettings base as IOptions<T> singleton
        services.AddSingleton(x => x.GetRequiredService<IOptions<AppSettings>>().Value);

        return services;
    }
}
