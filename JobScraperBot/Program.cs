using JobScraperBot;
using JobScraperBot.Extensions;
using Microsoft.Extensions.DependencyInjection;
using NLog;

var logger = LogManager.GetCurrentClassLogger();

try
{
    var services = new ServiceCollection().AddBotServices();

    using var serviceProvider = services.BuildServiceProvider();

    var bot = new Bot(serviceProvider);
    await bot.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    //loger shutdown
    LogManager.Shutdown();
}
