using System.Collections.Concurrent;
using JobScraperBot.Models;
using JobScraperBot.Services.Interfaces;

namespace JobScraperBot.Services.Implementations
{
    internal class UserSubscriptionsStorage : IUserSubscriptionsStorage
    {
        public ConcurrentBag<SubscriptionInfo> Subscriptions { get; }

        public UserSubscriptionsStorage()
        {
            this.Subscriptions = new ConcurrentBag<SubscriptionInfo>();
        }
    }
}
