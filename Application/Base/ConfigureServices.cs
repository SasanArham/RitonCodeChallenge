using Application.Modules.ContactManagement.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Base
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            services.AddScoped<IPersonImporter, PersonImporter>();

            return services;
        }
    }
}
