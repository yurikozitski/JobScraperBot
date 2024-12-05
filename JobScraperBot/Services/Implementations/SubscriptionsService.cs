using AutoMapper;
using JobScraperBot.DAL.Entities;
using JobScraperBot.DAL.Interfaces;
using JobScraperBot.Models;
using JobScraperBot.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace JobScraperBot.Services.Implementations
{
    public class SubscriptionsService : ISubscriptionsService
    {
        private readonly IUserSubscriptionsStorage subscriptionsStorage;
        private readonly ISubscriptionRepository subscriptionRepository;
        private readonly IVacancyService vacancyService;
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IMapper mapper;
        private readonly ILogger<SubscriptionsService> logger;

        public SubscriptionsService(
            IUserSubscriptionsStorage subscriptionsStorage,
            ISubscriptionRepository subscriptionsRepository,
            IVacancyService vacancyService,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IMapper mapper,
            ILogger<SubscriptionsService> logger)
        {
            this.subscriptionsStorage = subscriptionsStorage;
            this.subscriptionRepository = subscriptionsRepository;
            this.vacancyService = vacancyService;
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<IEnumerable<Subscription>> LoadSubscriptionsFromDataSourceAsync()
        {
            bool connectionSuccesful = false;
            IEnumerable<Subscription> subscriptions = default!;

            while (!connectionSuccesful)
            {
                try
                {
                    subscriptions = await this.subscriptionRepository.GetAllAsync();
                    connectionSuccesful = true;
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Error occured while loading user data from data source");
                    await Task.Delay(60_000);
                }
            }

            return subscriptions;
        }

        public void LoadSubscriptionsIntoMemory(IEnumerable<Subscription> subscriptions)
        {
            foreach (var subscription in subscriptions)
            {
                var subscriptionInfo = this.mapper.Map<Subscription, SubscriptionInfo>(subscription);

                if (!this.subscriptionsStorage.Subscriptions.ContainsKey(subscriptionInfo.ChatId))
                {
                    this.subscriptionsStorage.Subscriptions.TryAdd(subscriptionInfo.ChatId, subscriptionInfo);
                }
                else
                {
                    this.subscriptionsStorage.Subscriptions.Remove(subscriptionInfo.ChatId, out _);
                    this.subscriptionsStorage.Subscriptions.TryAdd(subscriptionInfo.ChatId, subscriptionInfo);
                }
            }
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

            await this.UpdateLastSentDateAsync(subscriptionInfo);
        }

        private async Task UpdateLastSentDateAsync(SubscriptionInfo subscriptionInfo)
        {
            int dayIncrement = subscriptionInfo.MessageInterval switch
            {
                MessageInterval.Daily => 1,
                MessageInterval.OnceInTwoDays => 2,
                MessageInterval.Weekly => 7,
                _ => throw new ArgumentOutOfRangeException(nameof(subscriptionInfo), $"{subscriptionInfo.MessageInterval} is not valid"),
            };

            subscriptionInfo.NextUpdate = new DateTime(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(dayIncrement)), subscriptionInfo.Time, DateTimeKind.Utc);

            var subscription = this.mapper.Map<SubscriptionInfo, Subscription>(subscriptionInfo);
            bool connectionSuccesful = false;

            while (!connectionSuccesful)
            {
                try
                {
                    await this.subscriptionRepository.UpdateDateAsync(subscription);
                    connectionSuccesful = true;
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Error occured while updating subscription in database");
                    await Task.Delay(60_000);
                }
            }

            this.subscriptionsStorage.Subscriptions.Remove(subscriptionInfo.ChatId, out _);
            this.subscriptionsStorage.Subscriptions.TryAdd(subscriptionInfo.ChatId, subscriptionInfo);
        }
    }
}
