using JobScraperBot.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace JobScraperBot
{
    internal class Bot
    {
        private readonly ServiceProvider serviceProvider;

        public Bot(ServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task Run()
        {
            using var cts = new CancellationTokenSource();
            var bot = new TelegramBotClient("7448548753:AAEkSnA2KdnzTExqwgz_sguLJ3UJo2pp4hU", cancellationToken: cts.Token);

            bot.StartReceiving(this.serviceProvider.GetService<IUpdateHandler>()!.HandleUpdateAsync, this.serviceProvider.GetService<IUpdateHandler>()!.HandleErrorAsync);

            var me = await bot.GetMeAsync();
            Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");

            var subscriptionService = this.serviceProvider.GetService<ISubscriptionsService>();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            subscriptionService?.ReadFromFilesAsync(cts.Token);
            subscriptionService?.SendMessagesAsync(cts.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            Console.ReadLine();
            cts.Cancel();
        }
    }
}
