using System.Text.RegularExpressions;
using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using Telegram.Bot.Types;

namespace JobScraperBot.Services.Implementations
{
    internal class MessageValidator : IMessageValidator
    {
        private readonly IOptionsProvider optionsProvider;

        public MessageValidator(IOptionsProvider optionsProvider)
        {
            this.optionsProvider = optionsProvider;
        }

        public bool IsMessageValid(Message message, UserState userState)
        {
            if (message == null || message.Text == null)
                return false;

            if (message.Text == "/reset" ||
                message.Text == "/confirm")
                return true;

            string subsRegPat = @"^(щодня|через день|щотижня),\s?[0-2][0-9]:[0-5][0-9]";
            Regex subsReg = new Regex(subsRegPat, RegexOptions.IgnoreCase);

            return userState switch
            {
                UserState.OnStackChoosing => this.optionsProvider.Stacks.ContainsValue(message.Text),
                UserState.OnGradeChoosing => this.optionsProvider.Levels.ContainsValue(message.Text),
                UserState.OnTypeChoosing => this.optionsProvider.JobKinds.ContainsValue(message.Text),
                UserState.OnResultChoosing => this.optionsProvider.ResultTypes.ContainsValue(message.Text),
                UserState.OnSubscriptionSetting => subsReg.IsMatch(message.Text),
                _ => true,
            };
        }
    }
}
