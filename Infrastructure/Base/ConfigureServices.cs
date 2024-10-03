using Application.Base;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MassTransit;
using System.Reflection;
using Application.Base.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Base
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMainDatabase(configuration);
            services.AddMassTransit(configuration);

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        private static IServiceCollection AddMainDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = SQlDbConnectionStringHelper.GetConnectionString(configuration);
            services.AddDbContext<DatabaseContext>(
                (sp, options) =>
                {
                    options.UseLazyLoadingProxies();
                    options.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName));

                });
            services.AddScoped<IDatabaseContext>(provider => provider.GetRequiredService<DatabaseContext>());
            return services;
        }

        private static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration)
        {
            var hostAddress = Environment.GetEnvironmentVariable("EVENT_BUS_HOST_ADDRESS") ?? configuration["EVENT_BUS:HOST_ADDRESS"];

            services.AddMassTransit(config =>
            {
                config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("contact-manager", false));
                var entryAssembly = Assembly.GetAssembly(typeof(CrudException));
                config.AddConsumers(entryAssembly);
                config.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.ConfigureEndpoints(ctx);
                    cfg.Host(hostAddress);
                });
                config.AddEntityFrameworkOutbox<DatabaseContext>(options =>
                {
                    options.QueryDelay = TimeSpan.FromSeconds(30);
                    options.UseSqlServer();
                    options.UseBusOutbox();
                });
            });

            return services;
        }
    }
}
