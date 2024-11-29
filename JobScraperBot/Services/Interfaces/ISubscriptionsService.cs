using JobScraperBot.DAL.Entities;

namespace JobScraperBot.Services.Interfaces
{
    public interface ISubscriptionsService
    {
        Task<IEnumerable<Subscription>> LoadSubscriptionsFromDataSourceAsync();

        void LoadSubscriptionsIntoMemory(IEnumerable<Subscription> subscriptions);

        Task SendMessagesAsync(CancellationToken token);
    }
}
