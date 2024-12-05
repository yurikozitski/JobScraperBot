using JobScraperBot.DAL.Entities;
using JobScraperBot.DAL.Interfaces;
using System.Globalization;

namespace JobScraperBot.DAL.Repositories
{
    public class SubscriptionFileRepository : ISubscriptionRepository
    {
        private readonly string path = Directory.GetCurrentDirectory() + "\\Subscriptions\\";

        public async Task AddAsync(Subscription entity)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string mesInt = entity.MessageInterval.Interval switch
            {
                _ when entity.MessageInterval.Interval.Equals("daily", StringComparison.InvariantCulture) => "щодня",
                _ when entity.MessageInterval.Interval.Equals("once_in_two_days", StringComparison.InvariantCulture) => "через день",
                _ when entity.MessageInterval.Interval.Equals("weekly", StringComparison.InvariantCulture) => "щотижня",
                _ => throw new FormatException($"Can't convert string: {entity.MessageInterval.Interval}")
            };

            string mesTime = entity.Time.ToString(CultureInfo.InvariantCulture);
            string sbscrptnTextUtc = mesInt + "," + mesTime;

            await System.IO.File.WriteAllTextAsync(
               this.path + $"{entity.ChatId}_subscription.txt",
               sbscrptnTextUtc + "," 
               + $"{entity.SubscriptionSettings.Stack}, {entity.SubscriptionSettings.Grade}, {entity.SubscriptionSettings.JobKind}" 
               + "," + entity.NextUpdate.ToString(CultureInfo.InvariantCulture));
        }

        public Task<IEnumerable<Subscription>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteByChatIdAsync(long chatId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateDateAsync(Subscription subscription)
        {
            throw new NotImplementedException();
        }
    }
}
