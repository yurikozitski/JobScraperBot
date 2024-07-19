using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;

namespace JobScraperBot.Services.Implementations
{
    internal class SubscriptionWriter : ISubscriptionWriter
    {
        public async Task WriteSubscriptionAsync(long chatId, string sbscrptnText, IUserStateMachine userState)
        {
            ArgumentNullException.ThrowIfNull(sbscrptnText);
            ArgumentNullException.ThrowIfNull(userState);

            if (userState.State != UserState.OnSubscriptionSetting)
                return;

            string path = Directory.GetCurrentDirectory() + "\\Subscriptions\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            await System.IO.File.WriteAllTextAsync(path + $"{chatId}_subscription.txt", sbscrptnText + "," + userState.UserSettings);
        }
    }
}
