using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using JobsScraper.BLL.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace JobScraperBot.Services.Implementations
{
    internal class ResponseKeyboardService : IResponseKeyboardService
    {
        private readonly IOptionsProvider optionsProvider;

        public ResponseKeyboardService(IOptionsProvider optionsProvider)
        {
            this.optionsProvider = optionsProvider;
        }

        public KeyboardButton[][]? GetResponseButtons(UserState state)
        {
            return state switch
            {
                //UserState.OnStart => null,
                UserState.OnStackChoosing => new KeyboardButton[][]
                {
                    new KeyboardButton[] { this.optionsProvider.Stacks[JobStacks.CSharpDotNET] },
                    new KeyboardButton[] { this.optionsProvider.Stacks[JobStacks.JavaScriptFrontEnd] },
                },
                UserState.OnGradeChoosing => new KeyboardButton[][]
                {
                    new KeyboardButton[] { this.optionsProvider.Levels[Grades.TraineeIntern] },
                    new KeyboardButton[] { this.optionsProvider.Levels[Grades.Junior] },
                    new KeyboardButton[] { this.optionsProvider.Levels[Grades.Middle] },
                    new KeyboardButton[] { this.optionsProvider.Levels[Grades.Senior] },
                    new KeyboardButton[] { this.optionsProvider.Levels[Grades.TeamLead] },
                    new KeyboardButton[] { this.optionsProvider.Levels[Grades.HeadChief] },
                },
                UserState.OnTypeChoosing => new KeyboardButton[][]
                {
                    new KeyboardButton[] { this.optionsProvider.JobKinds[JobTypes.Remote] },
                    new KeyboardButton[] { this.optionsProvider.JobKinds[JobTypes.OnSite] },
                    new KeyboardButton[] { this.optionsProvider.JobKinds[JobTypes.OnSite | JobTypes.Remote] },
                },
                UserState.OnResultChoosing => new KeyboardButton[][]
                {
                    new KeyboardButton[] { this.optionsProvider.ResultTypes["now"] },
                    new KeyboardButton[] { this.optionsProvider.ResultTypes["with_subscription"] },
                },
                //UserState.OnEnd => null,
                _ => null
            };
        }
    }
}
