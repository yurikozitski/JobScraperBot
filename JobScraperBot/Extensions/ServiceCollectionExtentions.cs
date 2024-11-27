using AutoMapper;
using JobScraperBot.DAL;
using JobScraperBot.DAL.Interfaces;
using JobScraperBot.DAL.Repositories;
using JobScraperBot.Mapping;
using JobScraperBot.Services;
using JobScraperBot.Services.Implementations;
using JobScraperBot.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Telegram.Bot.Polling;

namespace JobScraperBot.Extensions
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddBotServices(this IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutomapperProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();

            return services
                    .AddDbContext<JobScraperBotContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("LocalDb")))
                    .AddDbContextFactory<JobScraperBotContext>()
                    .AddSingleton<IUserStateStorage, UserStateStorage>()
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
                    .AddSingleton<IFileRemover, FileRemover>()
                    .AddTransient<IRequestStringService, RequestStringServive>()
                    .AddTransient<IVacancyService, VacancyService>()
                    .AddTransient<IUpdateHandler, UpdateHandler>()
                    .AddTransient<ISubscriptionRepository, SubscriptionDbRepository>()
                    .AddTransient<IHiddenVacancyRepository, HiddenVacancyDbRepository>()
                    .AddTransient<IConfiguration>(_ => configuration)
                    .AddSingleton(mapper)
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
