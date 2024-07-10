using JobScraperBot;
using JobScraperBot.Services;
using JobScraperBot.Services.Implementations;
using JobScraperBot.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Polling;

var services = new ServiceCollection()
    .AddSingleton<IUserStateStorage, UserStateStorage>()
    .AddSingleton<IUserStateService, UserStateService>()
    .AddSingleton<IResponseMessageService, ResponseMessageService>()
    .AddSingleton<IResponseKeyboardService, ResponseKeyboardService>()
    .AddSingleton<IMessageValidator, MessageValidator>()
    .AddSingleton<IOptionsProvider, OptionsProvider>()
    .AddTransient<IRequestStringService, RequestStringServive>()
    .AddTransient<IVacancyService, VacancyService>()
    .AddTransient<IUpdateHandler, UpdateHandler>()
    .AddHttpClient();

using var serviceProvider = services.BuildServiceProvider();

var bot = new Bot(serviceProvider);
await bot.Run();
