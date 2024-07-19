using System.Collections.Concurrent;
using JobScraperBot.Models;

namespace JobScraperBot.Services.Interfaces
{
    internal interface IUserSubscriptionsStorage
    {
        ConcurrentBag<SubscriptionInfo> Subscriptions { get; }
    }
}
