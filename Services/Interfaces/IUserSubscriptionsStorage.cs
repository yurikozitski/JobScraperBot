using System.Collections.Concurrent;
using JobScraperBot.Models;

namespace JobScraperBot.Services.Interfaces
{
    internal interface IUserSubscriptionsStorage
    {
        ConcurrentDictionary<long, SubscriptionInfo> Subscriptions { get; }
    }
}
