using JobsScraper.BLL.Enums;

namespace JobScraperBot.Services.Interfaces
{
    internal interface IOptionsProvider
    {
        Dictionary<JobStacks, string> Stacks { get; }

        Dictionary<Grades, string> Levels { get; }
    }
}
