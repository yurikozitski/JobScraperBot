using JobScraperBot.State;
using Telegram.Bot.Types;

namespace JobScraperBot.Services.Interfaces
{
    internal interface IResultChoosingHandler
    {
        void HandleResult(string message, IUserStateMachine userState);
    }
}
