using JobScraperBot.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace JobScraperBot.Services
{
    internal static class ResponseKeyboardService
    {
        public static KeyboardButton[][]? GetResponseButtons(UserState state)
        {
            return state switch
            {
                UserState.OnStart => null,
                UserState.OnStackChoosing => new KeyboardButton[][]
                {
                    new KeyboardButton[] { ".NET" },
                    new KeyboardButton[] { "Front End" },
                },
                UserState.OnGradeChoosing => new KeyboardButton[][]
                {
                    new KeyboardButton[] { "Trainee/Intern" },
                    new KeyboardButton[] { "Junior" },
                    new KeyboardButton[] { "Middle" },
                    new KeyboardButton[] { "Senior" },
                    new KeyboardButton[] { "Team Lead" },
                    new KeyboardButton[] { "Head/Chief" },
                },
                UserState.OnEnd => null,
                _ => null
            };
        }
    }
}
