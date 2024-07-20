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
        private readonly IVacancyService vacancyService;

        public SubscriptionsService(
            IUserSubscriptionsStorage subscriptionsStorage,
            IVacancyService vacancyService)
        {
            this.subscriptionsStorage = subscriptionsStorage;
            this.vacancyService = vacancyService;
        }

        public Task ReadFromFilesAsync(CancellationToken token)
        {
            return Task.Run(async () =>
            {
                string path = Directory.GetCurrentDirectory() + "\\Subscriptions";
                using var watcher = new FileSystemWatcher(path);

                await this.LoadSubscriptions(path);

                watcher.EnableRaisingEvents = true;

                watcher.Changed += async (o, e) => await this.LoadSubscriptions(path);
                watcher.Created += async (o, e) => await this.LoadSubscriptions(path);

                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(1000, token);
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
                            //foreach (var subscriptionInfo in this.subscriptionsStorage.Subscriptions.Values)
                            //{
                            //    if (subscriptionInfo.MessageInterval == MessageInterval.Daily &&
                            //    ((subscriptionInfo.LastSent == null && TimeOnly.FromDateTime(DateTime.UtcNow) > subscriptionInfo.Time) ||
                            //    (subscriptionInfo.LastSent != null && DateTime.UtcNow.Subtract((DateTime)subscriptionInfo.LastSent) > new TimeSpan(1, 0, 0, 0))))
                            //    {
                            //        await this.SendVacanciesAsync("7448548753:AAEkSnA2KdnzTExqwgz_sguLJ3UJo2pp4hU", subscriptionInfo, token);
                            //    }

                            //    if (subscriptionInfo.MessageInterval == MessageInterval.OnceInTwoDays &&
                            //    ((subscriptionInfo.LastSent == null && TimeOnly.FromDateTime(DateTime.UtcNow) > subscriptionInfo.Time) ||
                            //    (subscriptionInfo.LastSent != null && DateTime.UtcNow.Subtract((DateTime)subscriptionInfo.LastSent) > new TimeSpan(2, 0, 0, 0))))
                            //    {
                            //        await this.SendVacanciesAsync("7448548753:AAEkSnA2KdnzTExqwgz_sguLJ3UJo2pp4hU", subscriptionInfo, token);
                            //    }

                            //    if (subscriptionInfo.MessageInterval == MessageInterval.Weekly &&
                            //    ((subscriptionInfo.LastSent == null && TimeOnly.FromDateTime(DateTime.UtcNow) > subscriptionInfo.Time) ||
                            //    (subscriptionInfo.LastSent != null && DateTime.UtcNow.Subtract((DateTime)subscriptionInfo.LastSent) > new TimeSpan(7, 0, 0, 0))))
                            //    {
                            //        await this.SendVacanciesAsync("7448548753:AAEkSnA2KdnzTExqwgz_sguLJ3UJo2pp4hU", subscriptionInfo, token);
                            //    }
                            //}
                            foreach (var subscriptionInfo in this.subscriptionsStorage.Subscriptions.Values)
                            {
                                if (subscriptionInfo.LastSent.Day != DateTime.UtcNow.Day &&
                                TimeOnly.FromDateTime(DateTime.UtcNow) > subscriptionInfo.Time)
                                {
                                    if (subscriptionInfo.MessageInterval == MessageInterval.Daily &&
                                    subscriptionInfo.LastSent.AddDays(1).Day == DateTime.UtcNow.Day)
                                    {
                                        await this.SendVacanciesAsync("7448548753:AAEkSnA2KdnzTExqwgz_sguLJ3UJo2pp4hU", subscriptionInfo, token);
                                    }

                                    if (subscriptionInfo.MessageInterval == MessageInterval.OnceInTwoDays &&
                                    subscriptionInfo.LastSent.AddDays(2).Day == DateTime.UtcNow.Day)
                                    {
                                        await this.SendVacanciesAsync("7448548753:AAEkSnA2KdnzTExqwgz_sguLJ3UJo2pp4hU", subscriptionInfo, token);
                                    }

                                    if (subscriptionInfo.MessageInterval == MessageInterval.Weekly &&
                                    subscriptionInfo.LastSent.AddDays(7).Day == DateTime.UtcNow.Day)
                                    {
                                        await this.SendVacanciesAsync("7448548753:AAEkSnA2KdnzTExqwgz_sguLJ3UJo2pp4hU", subscriptionInfo, token);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

                    await Task.Delay(1000, token);
                }
            });
        }

        private async Task LoadSubscriptions(string path)
        {
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

                        var subscriptionInfo = new SubscriptionInfo(chatId, userSettings, messageInterval, time)
                        {
                            LastSent = DateTime.Parse(subscriptionParams[5].Trim(), CultureInfo.InvariantCulture),
                        };

                        if (!this.subscriptionsStorage.Subscriptions.ContainsKey(chatId))
                        {
                            this.subscriptionsStorage.Subscriptions.TryAdd(chatId, subscriptionInfo);
                        }
                        else
                        {
                            this.subscriptionsStorage.Subscriptions.TryGetValue(chatId, out SubscriptionInfo? previousValue);
                            this.subscriptionsStorage.Subscriptions.TryUpdate(chatId, subscriptionInfo, previousValue!);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        private async Task SendVacanciesAsync(string token, SubscriptionInfo subscriptionInfo, CancellationToken cancellationToken)
        {
            var vacancies = await this.vacancyService.GetVacanciesAsync(
                new TelegramBotClient(token, default, cancellationToken),
                subscriptionInfo.ChatId,
                subscriptionInfo.UserSettings);

            await this.vacancyService.ShowVacanciesAsync(
                new TelegramBotClient(token, default, cancellationToken),
                subscriptionInfo.ChatId,
                vacancies);

            await UpdateLastSentDate(subscriptionInfo.ChatId);
        }

#pragma warning disable SA1204 // Static elements should appear before instance elements
        private static async Task UpdateLastSentDate(long chatId)
        {
            string path = Directory.GetCurrentDirectory() + "\\Subscriptions" + $"\\{chatId}_subscription.txt";
            string subscription = await File.ReadAllTextAsync(path);
            string[] subscriptionParams = subscription.Split(',');
            subscriptionParams[subscriptionParams.Length - 1] = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            string newSubscription = string.Join(",", subscriptionParams);

            await File.WriteAllTextAsync(path, newSubscription);
        }
    }
}
