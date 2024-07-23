using System.Collections.Concurrent;
using JobScraperBot.Models;

namespace JobScraperBot.Services.Interfaces
{
    public interface IUserSubscriptionsStorage
    {
        ConcurrentDictionary<long, SubscriptionInfo> Subscriptions { get; }
    }
}
