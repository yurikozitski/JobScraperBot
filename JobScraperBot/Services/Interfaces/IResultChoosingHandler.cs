using JobScraperBot.State;
using Telegram.Bot.Types;

namespace JobScraperBot.Services.Interfaces
{
    public interface IResultChoosingHandler
    {
        void HandleResult(string message, IUserStateMachine userState);
    }
}
