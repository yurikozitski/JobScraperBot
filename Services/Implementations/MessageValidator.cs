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

            return userState switch
            {
                UserState.OnStackChoosing => this.optionsProvider.Stacks.ContainsValue(message.Text),
                UserState.OnGradeChoosing => this.optionsProvider.Levels.ContainsValue(message.Text),
                _ => true,
            };
        }
    }
}
