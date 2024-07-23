using System.Text;
using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using Microsoft.Extensions.Configuration;

namespace JobScraperBot.Services.Implementations
{
    public class RequestStringServive : IRequestStringService
    {
        private readonly IConfiguration configuration;

        public RequestStringServive(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetRequestString(UserSettings userSettings)
        {
            ArgumentNullException.ThrowIfNull(userSettings);

            var requestSb = new StringBuilder(this.configuration["parsingApi"]);

            _ = userSettings.Stack switch
            {
                _ when userSettings.Stack.Equals("Front End", StringComparison.InvariantCulture) => requestSb.Append("JobStack=JavaScriptFrontEnd"),
                _ when userSettings.Stack.Equals(".NET", StringComparison.InvariantCulture) => requestSb.Append("JobStack=CSharpDotNET"),
                _ when userSettings.Stack.Equals("Full Stack", StringComparison.InvariantCulture) => requestSb.Append("JobStack=Fullstack"),
                _ when userSettings.Stack.Equals("Python", StringComparison.InvariantCulture) => requestSb.Append("JobStack=Python"),
                _ when userSettings.Stack.Equals("Java", StringComparison.InvariantCulture) => requestSb.Append("JobStack=Java"),
                _ => throw new ArgumentException($"Invalid value: {userSettings.Stack} for stack name"),
            };

            _ = userSettings.Grade switch
            {
                _ when userSettings.Grade.Equals("Trainee/Intern", StringComparison.InvariantCulture) => requestSb.Append("&Grade=TraineeIntern"),
                _ when userSettings.Grade.Equals("Junior", StringComparison.InvariantCulture) => requestSb.Append("&Grade=Junior"),
                _ when userSettings.Grade.Equals("Middle", StringComparison.InvariantCulture) => requestSb.Append("&Grade=Middle"),
                _ when userSettings.Grade.Equals("Senior", StringComparison.InvariantCulture) => requestSb.Append("&Grade=Senior"),
                _ when userSettings.Grade.Equals("Team Lead", StringComparison.InvariantCulture) => requestSb.Append("&Grade=TeamLead"),
                _ when userSettings.Grade.Equals("Head/Chief", StringComparison.InvariantCulture) => requestSb.Append("&Grade=HeadChief"),
                _ => throw new ArgumentException($"Invalid value: {userSettings.Grade} for grade name"),
            };

            if (!string.IsNullOrEmpty(userSettings.Type))
            {
                _ = userSettings.Type switch
                {
                    _ when userSettings.Type.Equals("В офісі", StringComparison.InvariantCulture) => requestSb.Append("&JobType=OnSite"),
                    _ when userSettings.Type.Equals("Віддалено", StringComparison.InvariantCulture) => requestSb.Append("&JobType=Remote"),
                    _ when userSettings.Type.Equals("Віддалено або в офісі", StringComparison.InvariantCulture) => requestSb.Append("&JobType=Remote&JobType=OnSite"),
                    _ => throw new ArgumentException($"Invalid value: {userSettings.Type} for job type name"),
                };
            }

            return requestSb.ToString();
        }
    }
}