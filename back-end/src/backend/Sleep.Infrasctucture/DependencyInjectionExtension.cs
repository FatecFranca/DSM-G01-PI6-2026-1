using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sleep.Application.Services.LoggedUser;
using Sleep.Application.Tokens;
using Sleep.Domain.Repositories;
using Sleep.Domain.Repositories.SleepAnalysis;
using Sleep.Domain.Repositories.SleepRecord;
using Sleep.Domain.Repositories.User;
using Sleep.Domain.Security.Cryptography;
using Sleep.Infrasctructure.DataAccess;
using Sleep.Infrasctructure.DataAccess.Repositories.SleepAnalysis;
using Sleep.Infrasctructure.DataAccess.Repositories.SleepRecord;
using Sleep.Infrasctructure.DataAccess.Repositories.User;
using Sleep.Infrasctructure.Extensions;
using Sleep.Infrasctructure.Messaging.Extensions;
using Sleep.Infrasctructure.Security.Cryptography;
using Sleep.Infrasctructure.Security.Token.Access;
using Sleep.Infrasctructure.Services.LoggedUser;
using Sleep.Infrasctucture.DataAccess;
using System.Reflection;


namespace Sleep.Infrasctucture
{
    public static class DependencyInjectionExtension
    {
        public static void AddInfrasctructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddTokens(services, configuration);
            AddLoggedUser(services);
            AddRepositories(services);
            AddPasswordEncrypter(services, configuration);
            if (configuration.IsUnitTestEnviroment())
                return;

            var connectionString = configuration.ConnectionString();
            AddDbContext(services, connectionString!);
            AddFluentMigration(services, connectionString!);
            RabbitMqExtensions.AddRabbitMq(services, configuration);
        }
        private static void AddDbContext(IServiceCollection services, string connectionString)
        {
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 45));

            services.AddDbContext<SleepDbContext>(dbContextOp =>
            {
                dbContextOp.UseMySql(connectionString, serverVersion);
            });
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepositoryReadOnly, UserRepository>();
            services.AddScoped<IUserRepositoryWriteOnly, UserRepository>();
            services.AddScoped<ISleepRecordRepositoryReadOnly, SleepRecordRepository>();
            services.AddScoped<ISleepRecordRepositoryWriteOnly, SleepRecordRepository>();
            services.AddScoped<ISleepAnalysisRepositoryReadOnly, SleepAnalysisRepository>();
            services.AddScoped<ISleepAnalysisRepositoryWriteOnly, SleepAnalysisRepository>();
        }

        private static void AddTokens(IServiceCollection services, IConfiguration configuration)
        {
            var expirationTime = configuration.GetValue<uint>("Settings:Jwt:ExpirationTimeMinutes");
            var signingKey = configuration.GetValue<string>("Settings:Jwt:SigningKey");

            services.AddScoped<IAccessTokenGenerator>(opt => new JwtTokenGenerator(expirationTime, signingKey!));
            services.AddScoped<IAccessTokenValidator>(opt => new JwtTokenValidator(signingKey!));
        }
        private static void AddLoggedUser(IServiceCollection services) => services.AddScoped<ILoggedUser, LoggedUser>();
        private static void AddFluentMigration(IServiceCollection services, string connectionString)
        {
            services.AddFluentMigratorCore().ConfigureRunner(options =>
            {
                options.AddMySql5()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(Assembly.Load("Sleep.Infrasctructure")).For.All();
            });
        }

        private static void AddPasswordEncrypter(IServiceCollection services, IConfiguration configuration)
        {
            var salt = configuration.GetValue<string>("Settings:Password:Salt");
            services.AddScoped<IPasswordEncrypt>(option => new Sha512Encrypter(salt!));
        }
    }
}
