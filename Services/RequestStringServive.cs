using JobScraperBot.State;
using System.Text;

namespace JobScraperBot.Services
{
    internal static class RequestStringServive
    {
        private static string domain = "https://localhost:7055/jobs/find?";

        public static string GetRequestString(UserSettings userSettings)
        {
            ArgumentNullException.ThrowIfNull(userSettings);

            var requestSb = new StringBuilder(domain);

            _ = userSettings.Stack switch
            {
                _ when userSettings.Stack.Equals("Front End", StringComparison.InvariantCulture) => requestSb.Append("JobStack=JavaScriptFrontEnd"),
                _ when userSettings.Stack.Equals(".NET", StringComparison.InvariantCulture) => requestSb.Append("JobStack=CSharpDotNET"),
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

            return requestSb.ToString();

        }
    }
}