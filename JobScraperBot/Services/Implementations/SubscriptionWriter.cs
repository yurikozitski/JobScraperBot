using System.Globalization;
using JobScraperBot.Models;
using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;

namespace JobScraperBot.Services.Implementations
{
    public class SubscriptionWriter : ISubscriptionWriter
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

            string interval = sbscrptnTextArr[0].Trim();
            string sbscrptnTextUtc = interval + "," + timeUtc.ToString("HH':'mm");

            int dayIncrement = interval switch
            {
                _ when interval.Equals("щодня", StringComparison.InvariantCulture) => 1,
                _ when interval.Equals("через день", StringComparison.InvariantCulture) => 2,
                _ when interval.Equals("щотижня", StringComparison.InvariantCulture) => 7,
                _ => throw new FormatException($"Can't convert string: {interval}")
            };

            var messagingDateOnly = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(dayIncrement));

            await System.IO.File.WriteAllTextAsync(
                path + $"{chatId}_subscription.txt",
                sbscrptnTextUtc + "," + userState.UserSettings + "," + messagingDateOnly.ToString(CultureInfo.InvariantCulture));
        }
    }
}
