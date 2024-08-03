using JobScraperBot.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace JobScraperBot
{
    public class Bot
    {
        private readonly ServiceProvider serviceProvider;

        public Bot(ServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task Run()
        {
            var config = this.serviceProvider.GetService<IConfiguration>();

            using var cts = new CancellationTokenSource();
            var bot = new TelegramBotClient(config!["botToken"]!, cancellationToken: cts.Token);

            bot.StartReceiving(this.serviceProvider.GetService<IUpdateHandler>()!.HandleUpdateAsync, this.serviceProvider.GetService<IUpdateHandler>()!.HandleErrorAsync);

            var me = await bot.GetMeAsync();
            Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");

            var subscriptionService = this.serviceProvider.GetService<ISubscriptionsService>();
            var userStateService = this.serviceProvider.GetService<IUserStateService>();
            await subscriptionService!.StartUpLoadAsync();
            await userStateService!.LoadUserSettings();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            subscriptionService.ReadFromFilesAsync(cts.Token);
            subscriptionService.SendMessagesAsync(cts.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            Console.ReadLine();
            cts.Cancel();
        }
    }
}
