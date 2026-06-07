using Dapper;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace Sleep.Infrasctructure.Migrations
{
    public static class DatabaseMigration
    {
        public static void Migrate(string connectionString, IServiceProvider serviceProvider)
        {
            EnsureDatabaseCreated(connectionString);
            MigrationDatabase(serviceProvider);
        }
        public static void EnsureDatabaseCreated(string connectionString)
        {
            var connectionStringBuilder = new MySqlConnectionStringBuilder(connectionString);
            var databaseName = connectionStringBuilder.Database;

            connectionStringBuilder.Remove("Database");
            using var dbConnection = new MySqlConnection(connectionStringBuilder.ConnectionString);

            var parameters = new DynamicParameters();
            parameters.Add("name", databaseName);

            var records = dbConnection.Query("SELECT * FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @name", parameters);

            if (!records.Any())
                dbConnection.Execute($"CREATE DATABASE {databaseName}");
        }

        public static void MigrationDatabase(IServiceProvider servProvider)
        {
            var runner = servProvider.GetRequiredService<IMigrationRunner>();
            runner.ListMigrations();
            runner.MigrateUp();
        }
    }
}
