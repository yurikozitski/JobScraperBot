using System.Globalization;
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

            string[] sbscrptnTextArr = sbscrptnText.Split(',');

            TimeOnly time = TimeOnly.Parse(sbscrptnTextArr[1].Trim(), CultureInfo.InvariantCulture);
            var timeDifference = (DateTime.UtcNow - DateTime.Now).Hours;
            TimeOnly timeUtc = time.AddHours(timeDifference);

            string sbscrptnTextUtc = sbscrptnTextArr[0].Trim() + "," + timeUtc.ToString("HH':'mm");

            await System.IO.File.WriteAllTextAsync(
                path + $"{chatId}_subscription.txt",
                sbscrptnTextUtc + "," + userState.UserSettings + "," + DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
        }
    }
}
