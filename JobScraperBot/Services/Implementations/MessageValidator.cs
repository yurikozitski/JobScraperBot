using System.Text.RegularExpressions;
using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using Telegram.Bot.Types;

namespace JobScraperBot.Services.Implementations
{
    public class MessageValidator : IMessageValidator
    {
        private readonly IOptionsProvider optionsProvider;

        public MessageValidator(IOptionsProvider optionsProvider)
        {
            this.optionsProvider = optionsProvider;
        }

        public bool IsMessageValid(string message, UserState userState)
        {
            if (string.IsNullOrEmpty(message))
                return false;

            if (message == "/reset" ||
                message == "/confirm")
                return true;

            string subsRegPat = @"^(щодня|через день|щотижня),\s?[0-2][0-9]:[0-5][0-9]";
            Regex subsReg = new Regex(subsRegPat, RegexOptions.IgnoreCase);

            return userState switch
            {
                UserState.OnStackChoosing => this.optionsProvider.Stacks.ContainsValue(message),
                UserState.OnGradeChoosing => this.optionsProvider.Levels.ContainsValue(message),
                UserState.OnTypeChoosing => this.optionsProvider.JobKinds.ContainsValue(message),
                UserState.OnResultChoosing => this.optionsProvider.ResultTypes.ContainsValue(message),
                UserState.OnSubscriptionSetting => subsReg.IsMatch(message),
                _ => true,
            };
        }
    }
}
