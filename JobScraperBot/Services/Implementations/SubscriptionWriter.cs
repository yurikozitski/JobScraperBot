using System.Globalization;
using AutoMapper;
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
        private readonly IUserSubscriptionsStorage subscriptionsStorage;
        private readonly IMapper mapper;

        public SubscriptionWriter(
            ISubscriptionRepository subscriptionRepository,
            IMapper mapper,
            IUserSubscriptionsStorage subscriptionsStorage)
        {
            this.subscriptionRepository = subscriptionRepository;
            this.mapper = mapper;
            this.subscriptionsStorage = subscriptionsStorage;
        }

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

            int dayIncrement = GetDayIncrement(interval);
            string intervalEng = GetIntervalEng(interval);
            string jobStack = GetStackName(userState.UserSettings.Stack);
            string jobGrade = GetGradeName(userState.UserSettings.Grade);
            string? jobKind = GetJobKind(userState.UserSettings.Type);
            var messagingDateOnly = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(dayIncrement));

            var subscription = new Subscription()
            {
                ChatId = chatId,
                SubscriptionSettings = new SubscriptionSettings()
                {
                    Stack = new WorkStack()
                    {
                        StackName = jobStack,
                    },
                    Grade = new Grade()
                    {
                        GradeName = jobGrade,
                    },
                    JobKind = jobKind != null ? new JobKind() { KindName = jobKind } : null,
                },
                MessageInterval = new MessageIntervalEntity()
                {
                    Interval = intervalEng,
                },
                Time = timeUtc,
                NextUpdate = messagingDateOnly,
            };

            var subscriptionInfo = this.mapper.Map<Subscription, SubscriptionInfo>(subscription);

            if (!this.subscriptionsStorage.Subscriptions.TryAdd(chatId, subscriptionInfo))
            {
                this.subscriptionsStorage.Subscriptions.Remove(chatId, out _);
                this.subscriptionsStorage.Subscriptions.TryAdd(chatId, subscriptionInfo);
            }

            await this.subscriptionRepository.AddAsync(subscription);
        }

        private static int GetDayIncrement(string s)
        {
            int dayIncrement = s switch
            {
                _ when s.Equals("щодня", StringComparison.InvariantCulture) => 1,
                _ when s.Equals("через день", StringComparison.InvariantCulture) => 2,
                _ when s.Equals("щотижня", StringComparison.InvariantCulture) => 7,
                _ => throw new FormatException($"Can't convert string: {s}")
            };

            return dayIncrement;
        }

        private static string GetIntervalEng(string s)
        {
            string intervalEng = s switch
            {
                _ when s.Equals("щодня", StringComparison.InvariantCulture) => "daily",
                _ when s.Equals("через день", StringComparison.InvariantCulture) => "once_in_two_days",
                _ when s.Equals("щотижня", StringComparison.InvariantCulture) => "weekly",
                _ => throw new FormatException($"Can't convert string: {s}")
            };

            return intervalEng;
        }

        private static string GetStackName(string s)
        {
            string stackName = s switch
            {
                _ when s.Equals(".NET", StringComparison.InvariantCulture) => ".net",
                _ when s.Equals("Front End", StringComparison.InvariantCulture) => "front_end",
                _ when s.Equals("Java", StringComparison.InvariantCulture) => "java",
                _ when s.Equals("Full Stack", StringComparison.InvariantCulture) => "full_stack",
                _ when s.Equals("Python", StringComparison.InvariantCulture) => "python",
                _ => throw new FormatException($"Can't convert string: {s}")
            };

            return stackName;
        }

        private static string GetGradeName(string s)
        {
            string gradeName = s switch
            {
                _ when s.Equals("Trainee/Intern", StringComparison.InvariantCulture) => "trainee_intern",
                _ when s.Equals("Junior", StringComparison.InvariantCulture) => "junior",
                _ when s.Equals("Middle", StringComparison.InvariantCulture) => "middle",
                _ when s.Equals("Senior", StringComparison.InvariantCulture) => "senior",
                _ when s.Equals("Team Lead", StringComparison.InvariantCulture) => "team_lead",
                _ when s.Equals("Head/Chief", StringComparison.InvariantCulture) => "head_chief",
                _ => throw new FormatException($"Can't convert string: {s}")
            };

            return gradeName;
        }

        private static string? GetJobKind(string? s)
        {
            string? jobKind = s switch
            {
                null => null,
                _ when s.Equals("В офісі", StringComparison.InvariantCulture) => "office",
                _ when s.Equals("Віддалено", StringComparison.InvariantCulture) => "remote",
                _ when s.Equals("Віддалено або в офісі", StringComparison.InvariantCulture) => "office_or_remote",
                _ => null,
            };

            return jobKind;
        }
    }
}
