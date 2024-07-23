using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace JobScraperBot.Services.Interfaces
{
    public interface IVacancyVisibilityService
    {
        Task HandleVacancyVisibilityAsync(ITelegramBotClient botClient, Update update);

        //void MarkVacancyAsVisible(Update update);
    }
}
