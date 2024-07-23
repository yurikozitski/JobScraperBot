using JobScraperBot.State;
using Telegram.Bot.Types.ReplyMarkups;

namespace JobScraperBot.Services.Interfaces
{
    public interface IResponseKeyboardService
    {
        KeyboardButton[][]? GetResponseButtons(UserState state);
    }
}
