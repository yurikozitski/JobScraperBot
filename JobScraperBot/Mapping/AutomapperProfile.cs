using AutoMapper;
using JobScraperBot.DAL.Entities;
using JobScraperBot.Models;
using JobScraperBot.State;

namespace JobScraperBot.Mapping
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            this.CreateMap<SubscriptionInfo, Subscription>()
                .ForMember(s => s.ChatId, si => si.MapFrom(x => x.ChatId))
                .ForMember(s => s.SubscriptionSettings, si => si.MapFrom(x => GetSubscriptionSettings(x.UserSettings)))
                .ForMember(s => s.MessageInterval, si => si.MapFrom(x => new MessageIntervalEntity()
                {
                    Interval = GetInterval(x.MessageInterval),
                }))
                .ForMember(s => s.Time, si => si.MapFrom(x => x.Time))
                .ForMember(s => s.NextUpdate, si => si.MapFrom(x => DateOnly.FromDateTime(x.NextUpdate)));

            this.CreateMap<Subscription, SubscriptionInfo>()
                .ForCtorParam("chatId", s => s.MapFrom(x => x.ChatId))
                .ForCtorParam("userSettings", s => s.MapFrom(x => GetUserSettings(x.SubscriptionSettings)))
                .ForCtorParam("messageInterval", s => s.MapFrom(x => GetIntervalEnum(x.MessageInterval.Interval)))
                .ForCtorParam("time", s => s.MapFrom(x => x.Time))
                .ForMember(si => si.NextUpdate, s => s.MapFrom(x => new DateTime(x.NextUpdate, x.Time, DateTimeKind.Utc)));
        }

        private static string GetInterval(MessageInterval interval)
        {
            string intervalString = interval switch
            {
                MessageInterval.Daily => "daily",
                MessageInterval.OnceInTwoDays => "once_in_two_days",
                MessageInterval.Weekly => "weekly",
                _ => throw new ArgumentException("Invalid interval", nameof(interval)),
            };

            return intervalString;
        }

        private static MessageInterval GetIntervalEnum(string intervalStr)
        {
            MessageInterval interval = intervalStr switch
            {
                "daily" => MessageInterval.Daily,
                "once_in_two_days" => MessageInterval.OnceInTwoDays,
                "weekly" => MessageInterval.Weekly,
                _ => throw new FormatException($"Can't convert string: {intervalStr}")
            };

            return interval;
        }

        private static SubscriptionSettings GetSubscriptionSettings(UserSettings userSettings)
        {
            ArgumentNullException.ThrowIfNull(userSettings, nameof(userSettings));

            string stackName = userSettings.Stack switch
            {
                _ when userSettings.Stack.Equals(".NET", StringComparison.InvariantCulture) => ".net",
                _ when userSettings.Stack.Equals("Front End", StringComparison.InvariantCulture) => "front_end",
                _ when userSettings.Stack.Equals("Java", StringComparison.InvariantCulture) => "java",
                _ when userSettings.Stack.Equals("Full Stack", StringComparison.InvariantCulture) => "full_stack",
                _ when userSettings.Stack.Equals("Python", StringComparison.InvariantCulture) => "python",
                _ => throw new FormatException($"Can't convert string: {userSettings.Stack}")
            };

            string gradeName = userSettings.Grade switch
            {
                _ when userSettings.Grade.Equals("Trainee/Intern", StringComparison.InvariantCulture) => "trainee_intern",
                _ when userSettings.Grade.Equals("Junior", StringComparison.InvariantCulture) => "junior",
                _ when userSettings.Grade.Equals("Middle", StringComparison.InvariantCulture) => "middle",
                _ when userSettings.Grade.Equals("Senior", StringComparison.InvariantCulture) => "senior",
                _ when userSettings.Grade.Equals("Team Lead", StringComparison.InvariantCulture) => "team_lead",
                _ when userSettings.Grade.Equals("Head/Chief", StringComparison.InvariantCulture) => "head_chief",
                _ => throw new FormatException($"Can't convert string: {userSettings.Grade}")
            };

            string? jobKind = userSettings.Type switch
            {
                null => null,
                _ when userSettings.Type.Equals("В офісі", StringComparison.InvariantCulture) => "office",
                _ when userSettings.Type.Equals("Віддалено", StringComparison.InvariantCulture) => "remote",
                _ when userSettings.Type.Equals("Віддалено або в офісі", StringComparison.InvariantCulture) => "office_or_remote",
                _ => null,
            };

            var subscriptionSettings = new SubscriptionSettings()
            {
                Stack = new WorkStack() { StackName = stackName },
                Grade = new Grade() { GradeName = gradeName },
                JobKind = jobKind != null ? new JobKind() { KindName = jobKind } : null,
            };

            return subscriptionSettings;
        }

        private static UserSettings GetUserSettings(SubscriptionSettings subscriptionSettings)
        {
            ArgumentNullException.ThrowIfNull(subscriptionSettings, nameof(subscriptionSettings));

            string stack = subscriptionSettings.Stack.StackName switch
            {
                _ when subscriptionSettings.Stack.StackName.Equals(".net", StringComparison.InvariantCulture) => ".NET",
                _ when subscriptionSettings.Stack.StackName.Equals("front_end", StringComparison.InvariantCulture) => "Front End",
                _ when subscriptionSettings.Stack.StackName.Equals("java", StringComparison.InvariantCulture) => "Java",
                _ when subscriptionSettings.Stack.StackName.Equals("full_stack", StringComparison.InvariantCulture) => "Full Stack",
                _ when subscriptionSettings.Stack.StackName.Equals("python", StringComparison.InvariantCulture) => "Python",
                _ => throw new FormatException($"Can't convert string: {subscriptionSettings.Stack.StackName}")
            };

            string grade = subscriptionSettings.Grade.GradeName switch
            {
                _ when subscriptionSettings.Grade.GradeName.Equals("trainee_intern", StringComparison.InvariantCulture) => "Trainee/Intern",
                _ when subscriptionSettings.Grade.GradeName.Equals("junior", StringComparison.InvariantCulture) => "Junior",
                _ when subscriptionSettings.Grade.GradeName.Equals("middle", StringComparison.InvariantCulture) => "Middle",
                _ when subscriptionSettings.Grade.GradeName.Equals("senior", StringComparison.InvariantCulture) => "Senior",
                _ when subscriptionSettings.Grade.GradeName.Equals("team_lead", StringComparison.InvariantCulture) => "Team Lead",
                _ when subscriptionSettings.Grade.GradeName.Equals("head_chief", StringComparison.InvariantCulture) => "Head/Chief",
                _ => throw new FormatException($"Can't convert string: {subscriptionSettings.Grade.GradeName}")
            };

            string? type = subscriptionSettings.JobKind?.KindName switch
            {
                null => null,
                _ when subscriptionSettings.JobKind.KindName.Equals("office", StringComparison.InvariantCulture) => "В офісі",
                _ when subscriptionSettings.JobKind.KindName.Equals("remote", StringComparison.InvariantCulture) => "Віддалено",
                _ when subscriptionSettings.JobKind.KindName.Equals("office_or_remote", StringComparison.InvariantCulture) => "Віддалено або в офісі",
                _ => throw new FormatException($"Can't convert string: {subscriptionSettings.JobKind.KindName}")
            };

            var userSettings = new UserSettings()
            {
                Stack = stack,
                Grade = grade,
                Type = type,
            };

            return userSettings;
        }
    }
}
