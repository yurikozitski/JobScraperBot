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
            Console.ReadLine();
            cts.Cancel();
        }
    }
}
