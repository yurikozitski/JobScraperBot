using System.Text;
using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using JobsScraper.BLL.Enums;
using Microsoft.Extensions.Configuration;

namespace JobScraperBot.Services.Implementations
{
    public class RequestStringServive : IRequestStringService
    {
        private readonly IConfiguration configuration;
        private readonly IOptionsProvider optionsProvider;

        public RequestStringServive(IConfiguration configuration, IOptionsProvider optionsProvider)
        {
            this.configuration = configuration;
            this.optionsProvider = optionsProvider;
        }

        public string GetRequestString(UserSettings userSettings)
        {
            ArgumentNullException.ThrowIfNull(userSettings);

            var requestSb = new StringBuilder(this.configuration["parsingApi"]);

            _ = userSettings.Stack switch
            {
                null => throw new ArgumentNullException(nameof(userSettings), $"{nameof(userSettings.Stack)} was null"),
                _ when userSettings.Stack.Equals(this.optionsProvider.Stacks[JobStacks.JavaScriptFrontEnd], StringComparison.InvariantCulture) => requestSb.Append("JobStack=JavaScriptFrontEnd"),
                _ when userSettings.Stack.Equals(this.optionsProvider.Stacks[JobStacks.CSharpDotNET], StringComparison.InvariantCulture) => requestSb.Append("JobStack=CSharpDotNET"),
                _ when userSettings.Stack.Equals(this.optionsProvider.Stacks[JobStacks.Fullstack], StringComparison.InvariantCulture) => requestSb.Append("JobStack=Fullstack"),
                _ when userSettings.Stack.Equals(this.optionsProvider.Stacks[JobStacks.Python], StringComparison.InvariantCulture) => requestSb.Append("JobStack=Python"),
                _ when userSettings.Stack.Equals(this.optionsProvider.Stacks[JobStacks.Java], StringComparison.InvariantCulture) => requestSb.Append("JobStack=Java"),
                _ => throw new ArgumentException($"Invalid value: {userSettings.Stack} for stack name"),
            };

            _ = userSettings.Grade switch
            {
                null => throw new ArgumentNullException(nameof(userSettings), $"{nameof(userSettings.Grade)} was null"),
                _ when userSettings.Grade.Equals(this.optionsProvider.Levels[Grades.TraineeIntern], StringComparison.InvariantCulture) => requestSb.Append("&Grade=TraineeIntern"),
                _ when userSettings.Grade.Equals(this.optionsProvider.Levels[Grades.Junior], StringComparison.InvariantCulture) => requestSb.Append("&Grade=Junior"),
                _ when userSettings.Grade.Equals(this.optionsProvider.Levels[Grades.Middle], StringComparison.InvariantCulture) => requestSb.Append("&Grade=Middle"),
                _ when userSettings.Grade.Equals(this.optionsProvider.Levels[Grades.Senior], StringComparison.InvariantCulture) => requestSb.Append("&Grade=Senior"),
                _ when userSettings.Grade.Equals(this.optionsProvider.Levels[Grades.TeamLead], StringComparison.InvariantCulture) => requestSb.Append("&Grade=TeamLead"),
                _ when userSettings.Grade.Equals(this.optionsProvider.Levels[Grades.HeadChief], StringComparison.InvariantCulture) => requestSb.Append("&Grade=HeadChief"),
                _ => throw new ArgumentException($"Invalid value: {userSettings.Grade} for grade name"),
            };

            if (!string.IsNullOrEmpty(userSettings.Type))
            {
                _ = userSettings.Type switch
                {
                    _ when userSettings.Type.Equals(this.optionsProvider.JobKinds[JobTypes.OnSite], StringComparison.InvariantCulture) => requestSb.Append("&JobType=OnSite"),
                    _ when userSettings.Type.Equals(this.optionsProvider.JobKinds[JobTypes.Remote], StringComparison.InvariantCulture) => requestSb.Append("&JobType=Remote"),
                    _ when userSettings.Type.Equals(this.optionsProvider.JobKinds[JobTypes.OnSite | JobTypes.Remote], StringComparison.InvariantCulture) => requestSb.Append("&JobType=Remote&JobType=OnSite"),
                    _ => throw new ArgumentException($"Invalid value: {userSettings.Type} for job type name"),
                };
            }

            return requestSb.ToString();
        }
    }
}