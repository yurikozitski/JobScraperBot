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
                .ForMember(s => s.SubscriptionSettings, si => si.MapFrom(x => new SubscriptionSettings()
                {
                    Stack = new WorkStack() { StackName = x.UserSettings.Stack },
                    Grade = new Grade() { GradeName = x.UserSettings.Grade },
                    JobKind = x.UserSettings.Type != null ? new JobKind() { KindName = x.UserSettings.Type } : null,
                }))
                .ForMember(s => s.MessageInterval, si => si.MapFrom(x => new MessageIntervalEntity()
                {
                    Interval = GetInterval(x.MessageInterval),
                }))
                .ForMember(s => s.Time, si => si.MapFrom(x => x.Time))
                .ForMember(s => s.NextUpdate, si => si.MapFrom(x => DateOnly.FromDateTime(x.NextUpdate)));

            this.CreateMap<Subscription, SubscriptionInfo>()
                .ForCtorParam("chatId", s => s.MapFrom(x => x.ChatId))
                .ForCtorParam("userSettings", s => s.MapFrom(x => new UserSettings
                {
                    Stack = x.SubscriptionSettings.Stack.StackName,
                    Grade = x.SubscriptionSettings.Grade.GradeName,
                    Type = x.SubscriptionSettings.JobKind != null ? x.SubscriptionSettings.JobKind.KindName : null,
                }))
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
    }
}
