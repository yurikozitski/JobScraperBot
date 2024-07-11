﻿using JobScraperBot.Services;
using JobScraperBot.Services.Implementations;
using JobScraperBot.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Polling;

namespace JobScraperBot.Extensions
{
    internal static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddBotServices(this IServiceCollection services)
        {
            return services.AddSingleton<IUserStateStorage, UserStateStorage>()
                    .AddSingleton<IUserStateService, UserStateService>()
                    .AddSingleton<IResponseMessageService, ResponseMessageService>()
                    .AddSingleton<IResponseKeyboardService, ResponseKeyboardService>()
                    .AddSingleton<IMessageValidator, MessageValidator>()
                    .AddSingleton<IOptionsProvider, OptionsProvider>()
                    .AddSingleton<IMenuHandler, MenuHandler>()
                    .AddTransient<IRequestStringService, RequestStringServive>()
                    .AddTransient<IVacancyService, VacancyService>()
                    .AddTransient<IUpdateHandler, UpdateHandler>()
                    .AddHttpClient();
        }
    }
}