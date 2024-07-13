using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace JobScraperBot.Services.Interfaces
{
    internal interface IVacancyVisibilityService
    {
        Task HandleVacancyVisibilityAsync(Update update);

        //void MarkVacancyAsVisible(Update update);
    }
}
