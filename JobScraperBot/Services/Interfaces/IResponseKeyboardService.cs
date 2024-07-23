using JobScraperBot.State;
using Telegram.Bot.Types.ReplyMarkups;

namespace JobScraperBot.Services.Interfaces
{
    internal interface IResponseKeyboardService
    {
        KeyboardButton[][]? GetResponseButtons(UserState state);
    }
}
