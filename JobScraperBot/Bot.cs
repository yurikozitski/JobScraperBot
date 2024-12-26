using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using JobScraperBot.Services.Interfaces;
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
#pragma warning disable SA1009 // Closing parenthesis should be spaced correctly
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
            var subscriptionService = this.serviceProvider.GetService<ISubscriptionsService>();
            var userStateService = this.serviceProvider.GetService<IUserStateService>();

            Console.WriteLine("Loading data...");

            var subscriptions = await subscriptionService!.LoadSubscriptionsFromDataSourceAsync();
            userStateService!.LoadUserSettingsIntoMemory(subscriptions);
            subscriptionService.LoadSubscriptionsIntoMemory(subscriptions);

            var config = this.serviceProvider.GetService<IConfiguration>();
            using var cts = new CancellationTokenSource();

            var bot = new TelegramBotClient(config!["botToken"]!, cancellationToken: cts.Token);
            bot.StartReceiving(this.serviceProvider.GetService<IUpdateHandler>()!.HandleUpdateAsync, this.serviceProvider.GetService<IUpdateHandler>()!.HandleErrorAsync);

            User me = default!;
            try
            {
                me = await bot.GetMe();
            }
            catch { }

            subscriptionService.SendMessagesAsync(cts.Token);

            Console.WriteLine($"@{me?.Username ?? "jobgatherer_bot"} is running... Press Enter to terminate");

            Console.ReadLine();

            //Костиль для деплоя на бесплатному сервісі
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                string url = "http://localhost:10000/";

                // Create and start the HTTP listener
                HttpListener listener = new HttpListener();
                listener.Prefixes.Add(url);
                listener.Start();
                Console.WriteLine($"Listening for connections on {url}");

                while (true)
                {
                    // Wait for an incoming request
                    HttpListenerContext context = await listener.GetContextAsync();
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    // Create the response
                    string responseString = "<html><body>Hello, World!</body></html>";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);

                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    await output.WriteAsync(buffer, 0, buffer.Length);
                    output.Close();
                }
            }

            await cts.CancelAsync();
        }
    }
}
