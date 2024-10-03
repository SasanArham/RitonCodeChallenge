using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Repository.Base
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {

            return services;
        }
    }
}
