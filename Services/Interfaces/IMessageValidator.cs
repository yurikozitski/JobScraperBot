using JobScraperBot.State;
using Telegram.Bot.Types;

namespace JobScraperBot.Services.Interfaces
{
    internal interface IMessageValidator
    {
        bool IsMessageValid(Message message, UserState userState);
    }
}
