using JobScraperBot.Services;
using JobScraperBot.Services.Implementations;
using JobScraperBot.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Telegram.Bot.Polling;

namespace JobScraperBot.Extensions
{
    internal static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddBotServices(this IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            return services.AddSingleton<IUserStateStorage, UserStateStorage>()
                    .AddSingleton<IUserStateService, UserStateService>()
                    .AddSingleton<IResponseMessageService, ResponseMessageService>()
                    .AddSingleton<IResponseKeyboardService, ResponseKeyboardService>()
                    .AddSingleton<IMessageValidator, MessageValidator>()
                    .AddSingleton<IOptionsProvider, OptionsProvider>()
                    .AddSingleton<IMenuHandler, MenuHandler>()
                    .AddSingleton<IVacancyVisibilityService, VacancyVisibilityService>()
                    .AddSingleton<IResultChoosingHandler, ResultChoosingHandler>()
                    .AddSingleton<ISubscriptionWriter, SubscriptionWriter>()
                    .AddSingleton<IUserSubscriptionsStorage, UserSubscriptionsStorage>()
                    .AddSingleton<ISubscriptionsService, SubscriptionsService>()
                    .AddTransient<IRequestStringService, RequestStringServive>()
                    .AddTransient<IVacancyService, VacancyService>()
                    .AddTransient<IUpdateHandler, UpdateHandler>()
                    .AddTransient<IConfiguration>(_ => configuration)
                    .AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders();
                        loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                        loggingBuilder.AddNLog(configuration);
                    })
                    .AddHttpClient();
        }
    }
}
