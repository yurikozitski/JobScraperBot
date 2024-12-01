﻿using System.Globalization;
using JobScraperBot.Models;
using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace JobScraperBot.Services.Implementations
{
    public class SubscriptionsService : ISubscriptionsService
    {
        private static readonly string Path = Directory.GetCurrentDirectory() + "\\Subscriptions";

        private readonly IUserSubscriptionsStorage subscriptionsStorage;
        private readonly IVacancyService vacancyService;
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<SubscriptionsService> logger;

        public SubscriptionsService(
            IUserSubscriptionsStorage subscriptionsStorage,
            IVacancyService vacancyService,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<SubscriptionsService> logger)
        {
            this.subscriptionsStorage = subscriptionsStorage;
            this.vacancyService = vacancyService;
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        public async Task StartUpLoadAsync() => await this.LoadSubscriptionsAsync(Path);

        public Task ReadFromFilesAsync(CancellationToken token)
        {
            return Task.Run(async () =>
            {
                using var watcher = new FileSystemWatcher(Path);

                watcher.EnableRaisingEvents = true;

                watcher.Changed += async (o, e) => await this.LoadSubscriptionsAsync(Path);
                watcher.Created += async (o, e) => await this.LoadSubscriptionsAsync(Path);

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
                        foreach (var subscriptionInfo in this.subscriptionsStorage.Subscriptions.Values)
                        {
                            try
                            {
                                if (DateTime.UtcNow > subscriptionInfo.NextUpdate)
                                {
                                    await this.SendVacanciesAsync(this.configuration!["botToken"]!, subscriptionInfo, token);
                                }
                            }
                            catch (Exception ex)
                            {
                                this.logger.LogError(ex, "Can't send vacancies from subscription");
                            }
                        }
                    }

                    await Task.Delay(1000, token);
                }
            });
        }

        private async Task LoadSubscriptionsAsync(string path)
        {
            if (Directory.Exists(path))
            {
                foreach (string filePath in Directory.EnumerateFiles(path, "*.txt"))
                {
                    try
                    {
                        string subscription = await File.ReadAllTextAsync(filePath);
                        string[] subscriptionParams = subscription.Split(',');

                        string chatIdStr = System.IO.Path.GetFileName(filePath).Split('_')[0].Trim();
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
                            NextUpdate = new DateTime(DateOnly.Parse(subscriptionParams[5].Trim(), CultureInfo.InvariantCulture), time, DateTimeKind.Utc),
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
                        this.logger.LogError(ex, "Error occured while loading subscriptions from files");
                    }
                }
            }
        }

        private async Task SendVacanciesAsync(string token, SubscriptionInfo subscriptionInfo, CancellationToken cancellationToken)
        {
            var vacancies = await this.vacancyService.GetVacanciesAsync(
                new TelegramBotClient(token, this.httpClientFactory.CreateClient(), cancellationToken),
                subscriptionInfo.ChatId,
                subscriptionInfo.UserSettings);

            await this.vacancyService.ShowVacanciesAsync(
                new TelegramBotClient(token, this.httpClientFactory.CreateClient(), cancellationToken),
                subscriptionInfo.ChatId,
                vacancies);

            await UpdateLastSentDateAsync(subscriptionInfo);
        }

#pragma warning disable SA1204 // Static elements should appear before instance elements
        private static async Task UpdateLastSentDateAsync(SubscriptionInfo subscriptionInfo)
        {
            string path = Path + $"\\{subscriptionInfo.ChatId}_subscription.txt";
            string subscription = await File.ReadAllTextAsync(path);
            string[] subscriptionParams = subscription.Split(',');
            int dayIncrement = subscriptionInfo.MessageInterval switch
            {
                MessageInterval.Daily => 1,
                MessageInterval.OnceInTwoDays => 2,
                MessageInterval.Weekly => 7,
                _ => throw new ArgumentOutOfRangeException(nameof(subscriptionInfo), $"{subscriptionInfo.MessageInterval} is not valid"),
            };
            subscriptionParams[subscriptionParams.Length - 1] = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(dayIncrement)).ToString(CultureInfo.InvariantCulture);
            string newSubscription = string.Join(",", subscriptionParams);

            await File.WriteAllTextAsync(path, newSubscription);
        }
    }
}
