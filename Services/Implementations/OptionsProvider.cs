using JobScraperBot.Services.Interfaces;
using JobsScraper.BLL.Enums;

namespace JobScraperBot.Services.Implementations
{
    internal class OptionsProvider : IOptionsProvider
    {
        public Dictionary<JobStacks, string> Stacks => new Dictionary<JobStacks, string>()
        {
            { JobStacks.CSharpDotNET, ".NET" },
            { JobStacks.JavaScriptFrontEnd, "Front End" },
        };

        public Dictionary<Grades, string> Levels => new Dictionary<Grades, string>()
        {
            { Grades.TraineeIntern, "Trainee/Intern" },
            { Grades.Junior, "Junior" },
            { Grades.Middle, "Middle" },
            { Grades.Senior, "Senior" },
            { Grades.TeamLead, "Team Lead" },
            { Grades.HeadChief, "Head/Chief" },
        };

        public Dictionary<JobTypes, string> JobKinds => new Dictionary<JobTypes, string>()
        {
            { JobTypes.OnSite, "В офісі" },
            { JobTypes.Remote, "Віддалено" },
            { JobTypes.Remote | JobTypes.OnSite, "Віддалено або в офісі" },
        };

        public Dictionary<string, string> ResultTypes => new Dictionary<string, string>()
        {
            { "now", "Отримати зараз" },
            { "with_subscription", "Налаштувати підписку" },
        };
    }
}
