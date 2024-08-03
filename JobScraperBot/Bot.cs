﻿using JobScraperBot.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace JobScraperBot
#pragma warning disable S2486 // Generic exceptions should not be ignored
#pragma warning disable SA1501 // Statement should not be on a single line
#pragma warning disable S108 // Nested blocks of code should not be left empty
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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

            User me = default!;
            try
            {
                me = await bot.GetMeAsync();
            }
            catch { }

            var subscriptionService = this.serviceProvider.GetService<ISubscriptionsService>();
            var userStateService = this.serviceProvider.GetService<IUserStateService>();
            await subscriptionService!.StartUpLoadAsync();
            await userStateService!.LoadUserSettings();
            subscriptionService.ReadFromFilesAsync(cts.Token);
            subscriptionService.SendMessagesAsync(cts.Token);
            Console.WriteLine($"@{me?.Username ?? "jobgatherer_bot"} is running... Press Enter to terminate");

            Console.ReadLine();
            cts.Cancel();
        }
    }
}
