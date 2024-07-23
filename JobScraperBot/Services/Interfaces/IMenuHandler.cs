using JobScraperBot.State;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace JobScraperBot.Services.Interfaces
{
    internal interface IMenuHandler
    {
        Task HandleMenuAsync(ITelegramBotClient botClient, Message message, IUserStateMachine currentUserState);
    }
}
