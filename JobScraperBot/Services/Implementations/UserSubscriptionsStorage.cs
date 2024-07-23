using System.Collections.Concurrent;
using JobScraperBot.Models;
using JobScraperBot.Services.Interfaces;

namespace JobScraperBot.Services.Implementations
{
    internal class UserSubscriptionsStorage : IUserSubscriptionsStorage
    {
        public ConcurrentDictionary<long, SubscriptionInfo> Subscriptions { get; }

        public UserSubscriptionsStorage()
        {
            this.Subscriptions = new ConcurrentDictionary<long, SubscriptionInfo>();
        }
    }
}
