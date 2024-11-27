using JobScraperBot.State;

namespace JobScraperBot.Models
{
    public class SubscriptionInfo
    {
        public long ChatId { get; }

        public UserSettings UserSettings { get; }

        public MessageInterval MessageInterval { get; }

        public TimeOnly Time { get; }

        public DateTime NextUpdate { get; set; }

        public SubscriptionInfo()
        {
        }

        public SubscriptionInfo(long chatId, UserSettings userSettings, MessageInterval messageInterval, TimeOnly time)
        {
            this.ChatId = chatId;
            this.UserSettings = userSettings;
            this.MessageInterval = messageInterval;
            this.Time = time;
        }
    }
}
