using JobScraperBot;
using JobScraperBot.Extensions;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection().AddBotServices();

using var serviceProvider = services.BuildServiceProvider();

var bot = new Bot(serviceProvider);
await bot.Run();
