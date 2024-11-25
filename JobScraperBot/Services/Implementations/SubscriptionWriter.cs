using System.Globalization;
using JobScraperBot.DAL.Entities;
using JobScraperBot.DAL.Interfaces;
using JobScraperBot.Models;
using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;

namespace JobScraperBot.Services.Implementations
{
    public class SubscriptionWriter : ISubscriptionWriter
    {
        private readonly ISubscriptionRepository subscriptionRepository;

        public SubscriptionWriter(ISubscriptionRepository subscriptionRepository)
        {
            this.subscriptionRepository = subscriptionRepository;
        }

        //public async Task WriteSubscriptionAsync(long chatId, string sbscrptnText, IUserStateMachine userState)
        //{
        //    ArgumentNullException.ThrowIfNull(sbscrptnText);
        //    ArgumentNullException.ThrowIfNull(userState);

        //    if (userState.State != UserState.OnSubscriptionSetting)
        //        return;

        //    string path = Directory.GetCurrentDirectory() + "\\Subscriptions\\";

        //    if (!Directory.Exists(path))
        //        Directory.CreateDirectory(path);

        //    string[] sbscrptnTextArr = sbscrptnText.Split(',');

        //    TimeOnly time = TimeOnly.Parse(sbscrptnTextArr[1].Trim(), CultureInfo.InvariantCulture);
        //    var timeDifference = (DateTime.UtcNow - DateTime.Now).Hours;
        //    TimeOnly timeUtc = time.AddHours(timeDifference);

        //    string interval = sbscrptnTextArr[0].Trim();
        //    string sbscrptnTextUtc = interval + "," + timeUtc.ToString("HH':'mm");

        //    int dayIncrement = interval switch
        //    {
        //        _ when interval.Equals("щодня", StringComparison.InvariantCulture) => 1,
        //        _ when interval.Equals("через день", StringComparison.InvariantCulture) => 2,
        //        _ when interval.Equals("щотижня", StringComparison.InvariantCulture) => 7,
        //        _ => throw new FormatException($"Can't convert string: {interval}")
        //    };

        //    var messagingDateOnly = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(dayIncrement));

        //    await System.IO.File.WriteAllTextAsync(
        //        path + $"{chatId}_subscription.txt",
        //        sbscrptnTextUtc + "," + userState.UserSettings + "," + messagingDateOnly.ToString(CultureInfo.InvariantCulture));
        //}

        public async Task WriteSubscriptionAsync(long chatId, string sbscrptnText, IUserStateMachine userState)
        {
            ArgumentNullException.ThrowIfNull(sbscrptnText);
            ArgumentNullException.ThrowIfNull(userState);

            if (userState.State != UserState.OnSubscriptionSetting)
            {
                return;
            }

            string[] sbscrptnTextArr = sbscrptnText.Split(',');

            TimeOnly time = TimeOnly.Parse(sbscrptnTextArr[1].Trim(), CultureInfo.InvariantCulture);
            var timeDifference = (DateTime.UtcNow - DateTime.Now).Hours;
            TimeOnly timeUtc = time.AddHours(timeDifference);

            string interval = sbscrptnTextArr[0].Trim();
            string sbscrptnTextUtc = interval + "," + timeUtc.ToString("HH':'mm");

            int dayIncrement;
            string intervalEng;

            switch (interval)
            {
                case string s when s.Equals("щодня", StringComparison.InvariantCulture):
                    dayIncrement = 1;
                    intervalEng = "daily";
                    break;
                case string s when s.Equals("через день", StringComparison.InvariantCulture):
                    dayIncrement = 2;
                    intervalEng = "once_in_two_days";
                    break;
                case string s when s.Equals("щотижня", StringComparison.InvariantCulture):
                    dayIncrement = 7;
                    intervalEng = "weekly";
                    break;
                default:
                    throw new FormatException($"Can't convert string: {interval}");
            }

            string? jobType = userState.UserSettings.Type;

            string? jobKind = jobType switch
            {
                null => null,
                _ when jobType.Equals("В офісі", StringComparison.InvariantCulture) => "office",
                _ when jobType.Equals("Віддалено", StringComparison.InvariantCulture) => "remote",
                _ when jobType.Equals("Віддалено або в офісі", StringComparison.InvariantCulture) => "office_or_remote",
                _ => null,
            };

            var messagingDateOnly = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(dayIncrement));

            await this.subscriptionRepository.AddAsync(new Subscription()
            {
                ChatId = chatId,
                SubscriptionSettings = new SubscriptionSettings()
                {
                    Stack = new WorkStack()
                    {
                        StackName = userState.UserSettings.Stack,
                    },
                    Grade = new Grade()
                    {
                        GradeName = userState.UserSettings.Grade,
                    },
                    JobKind = jobKind != null ? new JobKind() { KindName = jobKind } : null,
                },
                MessageInterval = new MessageIntervalEntity()
                {
                    Interval = intervalEng,
                },
                Time = timeUtc,
                NextUpdate = messagingDateOnly,
            });
        }
    }
}
