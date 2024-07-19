using System.Globalization;
using JobScraperBot.Models;
using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using Telegram.Bot;

namespace JobScraperBot.Services.Implementations
{
    internal class SubscriptionsService : ISubscriptionsService
    {
        private readonly IUserSubscriptionsStorage subscriptionsStorage;
        //private readonly IRequestStringService requestStringService;
        //private readonly ITelegramBotClient telegramBotClient;
        private readonly IVacancyService vacancyService;

        public SubscriptionsService(
            IUserSubscriptionsStorage subscriptionsStorage,
            //IRequestStringService requestStringService,
            //ITelegramBotClient telegramBotClient,
            IVacancyService vacancyService)
        {
            this.subscriptionsStorage = subscriptionsStorage;
            //this.requestStringService = requestStringService;
            //this.telegramBotClient = telegramBotClient;
            this.vacancyService = vacancyService;
        }

        public Task ReadFromFilesAsync(CancellationToken token)
        {
            return Task.Run(async () =>
            {
                string path = Directory.GetCurrentDirectory() + "\\Subscriptions";

                while (true)
                {
                    if (token.IsCancellationRequested)
                        return;

                    if (Directory.Exists(path))
                    {
                        foreach (string filePath in Directory.EnumerateFiles(path, "*.txt"))
                        {
                            try
                            {
                                string subscription = await File.ReadAllTextAsync(filePath);
                                string[] subscriptionParams = subscription.Split(',');

                                string chatIdStr = Path.GetFileName(filePath).Split('_')[0].Trim();
                                string intervalStr = subscriptionParams[0].Trim();
                                string timeStr = subscriptionParams[1].Trim();

                                long chatId = long.Parse(chatIdStr);
                                MessageInterval messageInterval = intervalStr switch
                                {
                                    _ when intervalStr.Equals("щодня", StringComparison.InvariantCulture) => MessageInterval.Daily,
                                    _ when intervalStr.Equals("через день", StringComparison.InvariantCulture) => MessageInterval.OnceInTwoDays,
                                    _ when intervalStr.Equals("щотижня", StringComparison.InvariantCulture) => MessageInterval.Weekly,
                                    _ => throw new FormatException($"Can't convert string: {intervalStr}")
                                };
                                TimeOnly time = TimeOnly.Parse(timeStr, CultureInfo.InvariantCulture);
                                UserSettings userSettings = new UserSettings()
                                {
                                    Stack = subscriptionParams[2].Trim(),
                                    Grade = subscriptionParams[3].Trim(),
                                    Type = subscriptionParams[4].Trim(),
                                };

                                var subscriptionInfo = new SubscriptionInfo(chatId, userSettings, messageInterval, time);

                                if (!this.subscriptionsStorage.Subscriptions.Any(x => x.ChatId == subscriptionInfo.ChatId))
                                {
                                    this.subscriptionsStorage.Subscriptions.Add(subscriptionInfo);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }

                    await Task.Delay(5_000);
                }
            });
        }

        public Task SendMessagesAsync(CancellationToken token)
        {
            return Task.Run(async () =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                        return;

                    if (!this.subscriptionsStorage.Subscriptions.IsEmpty)
                    {
                        try
                        {
                            foreach (var subscriptionInfo in this.subscriptionsStorage.Subscriptions)
                            {
                                if (subscriptionInfo.MessageInterval == MessageInterval.Daily &&
                                ((subscriptionInfo.LastSent == null && TimeOnly.FromDateTime(DateTime.Now) > subscriptionInfo.Time) ||
                                (subscriptionInfo.LastSent != null && DateTime.Now.Subtract((DateTime)subscriptionInfo.LastSent) > new TimeSpan(1, 0, 0, 0))))
                                {
                                    var vacancies = await this.vacancyService.GetVacanciesAsync(
                                        new TelegramBotClient("7448548753:AAEkSnA2KdnzTExqwgz_sguLJ3UJo2pp4hU", default, token),
                                        subscriptionInfo.ChatId,
                                        subscriptionInfo.UserSettings);

                                    await this.vacancyService.ShowVacanciesAsync(
                                        new TelegramBotClient("7448548753:AAEkSnA2KdnzTExqwgz_sguLJ3UJo2pp4hU", default, token),
                                        subscriptionInfo.ChatId,
                                        vacancies);

                                    subscriptionInfo.LastSent = DateTime.Now;
                                }

                                if (subscriptionInfo.MessageInterval == MessageInterval.OnceInTwoDays &&
                                ((subscriptionInfo.LastSent == null && TimeOnly.FromDateTime(DateTime.Now) > subscriptionInfo.Time) ||
                                (subscriptionInfo.LastSent != null && DateTime.Now.Subtract((DateTime)subscriptionInfo.LastSent) > new TimeSpan(2, 0, 0, 0))))
                                {
                                    var vacancies = await this.vacancyService.GetVacanciesAsync(
                                        new TelegramBotClient("7448548753:AAEkSnA2KdnzTExqwgz_sguLJ3UJo2pp4hU", default, token),
                                        subscriptionInfo.ChatId,
                                        subscriptionInfo.UserSettings);

                                    await this.vacancyService.ShowVacanciesAsync(
                                        new TelegramBotClient("7448548753:AAEkSnA2KdnzTExqwgz_sguLJ3UJo2pp4hU", default, token),
                                        subscriptionInfo.ChatId,
                                        vacancies);

                                    subscriptionInfo.LastSent = DateTime.Now;
                                }

                                if (subscriptionInfo.MessageInterval == MessageInterval.Weekly &&
                                ((subscriptionInfo.LastSent == null && TimeOnly.FromDateTime(DateTime.Now) > subscriptionInfo.Time) ||
                                (subscriptionInfo.LastSent != null && DateTime.Now.Subtract((DateTime)subscriptionInfo.LastSent) > new TimeSpan(7, 0, 0, 0))))
                                {
                                    var vacancies = await this.vacancyService.GetVacanciesAsync(
                                        new TelegramBotClient("7448548753:AAEkSnA2KdnzTExqwgz_sguLJ3UJo2pp4hU", default, token),
                                        subscriptionInfo.ChatId,
                                        subscriptionInfo.UserSettings);

                                    await this.vacancyService.ShowVacanciesAsync(
                                        new TelegramBotClient("7448548753:AAEkSnA2KdnzTExqwgz_sguLJ3UJo2pp4hU", default, token),
                                        subscriptionInfo.ChatId,
                                        vacancies);

                                    subscriptionInfo.LastSent = DateTime.Now;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

                    await Task.Delay(1000);
                }
            });
        }
    }
}
