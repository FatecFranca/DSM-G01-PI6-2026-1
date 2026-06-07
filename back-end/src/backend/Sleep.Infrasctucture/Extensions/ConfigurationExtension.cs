using Microsoft.Extensions.Configuration;

namespace Sleep.Infrasctructure.Extensions
{
    public static class ConfigurationExtension
    {
        public static string ConnectionString(this IConfiguration configuration)
        {
            var host = Environment.GetEnvironmentVariable("PI_DB_HOST");
            var port = Environment.GetEnvironmentVariable("PI_DB_PORT") ?? "3306";
            var database = Environment.GetEnvironmentVariable("PI_DB_NAME");
            var user = Environment.GetEnvironmentVariable("DB_USER");
            var password = Environment.GetEnvironmentVariable("DB_PASS");

            return $"Host={host};Port={port};Database={database};Username={user};Password={password}";
        }


        public static bool IsUnitTestEnviroment(this IConfiguration configuration)
        {
            return configuration.GetValue<bool>("InMemoryTest");
        }
    }
}
