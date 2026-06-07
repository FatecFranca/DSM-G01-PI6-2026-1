using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Sleep.Application.Services.AutoMapper;
using Sleep.Application.Services.Cryptography;
using Sleep.Application.UseCases.Auth.Login;
using Sleep.Application.UseCases.Sleep.Anlysis;
using Sleep.Application.UseCases.Sleep.Anlysis.Messaging;
using Sleep.Application.UseCases.Sleep.Create;
using Sleep.Application.UseCases.Sleep.Get.SleepHistory;
using Sleep.Application.UseCases.Insights.Get;
using Sleep.Application.UseCases.Sleep.Get.Analysis;
using Sleep.Application.UseCases.Sleep.Get.ById;
using Sleep.Application.UseCases.User.Profile.Get;
using Sleep.Application.UseCases.User.Profile.Update;
using Sleep.Application.UseCases.User.Register;
using Sleep.Communication.Events;
using Sleep.Domain.Messaging;

namespace Sleep.Application
{
    public static class DependencyInjectionExtesion
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            AddMessagingHandlers(services);
            AddAutomapper(services);
            AddUseCases(services);
            AddPasswordEncrypter(services, configuration);
        }

        private static void AddAutomapper(IServiceCollection services)
        {
            services.AddScoped(option => new MapperConfiguration(option =>
            {
                option.AddProfile(new AutoMapping());
            }, NullLoggerFactory.Instance).CreateMapper());
        }

        private static void AddUseCases(IServiceCollection services)
        {
            services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
            services.AddScoped<IDoLoginUseCase, DoLoginUseCase>();
            services.AddScoped<ICreateSleepRecordUseCase, CreateSleepRecordUseCase>();
            services.AddScoped<IGetSleepHistoryUseCase, GetSleepHistoryUseCase>();
            services.AddScoped<IDoSleepAnalysisUseCase, DoSleepAnalysisUseCase>();
            services.AddScoped<IGetUserProfileUseCase, GetUserProfileUseCase>();
            services.AddScoped<IUpdateUserProfileUseCase, UpdateUserProfileUseCase>();
            services.AddScoped<IGetSleepRecordByIdUseCase, GetSleepRecordByIdUseCase>();
            services.AddScoped<IGetSleepAnalysisUseCase, GetSleepAnalysisUseCase>();
            services.AddScoped<IGetInsightsUseCase, GetInsightsUseCase>();
        }

        private static void AddPasswordEncrypter(IServiceCollection services, IConfiguration configuration)
        {
            var salt = configuration.GetValue<string>("Settings:Password:Salt");
            services.AddScoped(option => new PasswordEncrypter(salt!));
        }

        private static void AddMessagingHandlers(IServiceCollection services)
        {
            services.AddScoped<IMessageHandler<SleepAnalysisResponseMessage>, SleepAnalysisResponseHandler>();
        }
    }   
}