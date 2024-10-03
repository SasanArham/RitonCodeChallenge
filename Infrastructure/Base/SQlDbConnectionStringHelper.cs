using Microsoft.Extensions.Configuration;

namespace Infrastructure.Base
{
    internal static class SQlDbConnectionStringHelper
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            string connectionString;

            connectionString = GetFromEnviromentVariables();
            if (!string.IsNullOrEmpty(connectionString))
            {
                return connectionString;
            }

            connectionString = GetFromAppSettings(configuration);
            if (!string.IsNullOrEmpty(connectionString))
            {
                return connectionString;
            }
            throw new Exception("Could not find main db connectionstring");
        }

        private static string GetFromEnviromentVariables()
        {
            try
            {
                var ServerName = Environment.GetEnvironmentVariable("DATABASE_SERVER");
                if (string.IsNullOrEmpty(ServerName))
                {
                    return string.Empty;
                }

                var Port = Environment.GetEnvironmentVariable("DATABASE_PORT");

                var DatabaseName = Environment.GetEnvironmentVariable("DATABASE_NAME");
                if (string.IsNullOrEmpty(DatabaseName))
                {
                    return string.Empty;
                }

                var Username = Environment.GetEnvironmentVariable("DATABASE_USER");
                if (string.IsNullOrEmpty(Username))
                {
                    return string.Empty;
                }

                var Password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
                if (string.IsNullOrEmpty(Password))
                {
                    return string.Empty;
                }

                string connectionString = $"Server={ServerName},{Port};Database={DatabaseName};User Id={Username};Password={Password};TrustServerCertificate=Yes;MultipleActiveResultSets=true";
                return connectionString;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static string GetFromAppSettings(IConfiguration configuration)
        {
            try
            {
                var ServerName = configuration["MainDataBase:ServerName"];
                if (string.IsNullOrEmpty(ServerName))
                {
                    return string.Empty;
                }
                var Port = configuration["MainDataBase:Port"];

                var DatabaseName = configuration["MainDataBase:DbName"];
                if (string.IsNullOrEmpty(DatabaseName))
                {
                    return string.Empty;
                }

                var Username = configuration["MainDataBase:Username"];
                if (string.IsNullOrEmpty(Username))
                {
                    return string.Empty;
                }

                var Password = configuration["MainDataBase:Password"];
                if (string.IsNullOrEmpty(Password))
                {
                    return string.Empty;
                }

                string connectionString;
                if (string.IsNullOrEmpty(Port))
                {
                    connectionString = $"Server={ServerName};Database={DatabaseName};User Id={Username};Password={Password};TrustServerCertificate=Yes;MultipleActiveResultSets=true";
                }
                else
                {
                    connectionString = $"Server={ServerName},{Port};Database={DatabaseName};User Id={Username};Password={Password};TrustServerCertificate=Yes;MultipleActiveResultSets=true";
                }
                return connectionString;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
