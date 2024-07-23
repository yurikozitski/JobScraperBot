using JobsScraper.BLL.Enums;

namespace JobScraperBot.Services.Interfaces
{
    public interface IOptionsProvider
    {
        Dictionary<JobStacks, string> Stacks { get; }

        Dictionary<Grades, string> Levels { get; }

        Dictionary<JobTypes, string> JobKinds { get; }

        Dictionary<string, string> ResultTypes { get; }
    }
}
