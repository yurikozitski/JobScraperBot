using JobScraperBot.State;
using Telegram.Bot.Types;

namespace JobScraperBot.Services.Interfaces
{
    public interface IMessageValidator
    {
        bool IsMessageValid(string message, UserState userState);
    }
}
