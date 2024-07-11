using JobScraperBot;
using JobScraperBot.Extensions;
using JobScraperBot.Services;
using JobScraperBot.Services.Implementations;
using JobScraperBot.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Polling;

var services = new ServiceCollection().AddBotServices();

using var serviceProvider = services.BuildServiceProvider();

var bot = new Bot(serviceProvider);
await bot.Run();
