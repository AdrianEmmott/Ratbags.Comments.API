using Comments.API.Interfaces;
using Ratbags.Comments.API.Interfaces;
using Ratbags.Comments.API.Repositories;
using Ratbags.Comments.API.Services;

namespace Ratbags.Cmments.API.IOC
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            // Register services
            services.AddScoped<ICommentsService, CommentsService>();
            services.AddScoped<ICommentsRepository, CommentsRepository>();

            return services;
        }
    }
}
