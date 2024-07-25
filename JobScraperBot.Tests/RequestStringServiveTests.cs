using JobScraperBot.Services.Implementations;
using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using JobsScraper.BLL.Enums;
using Microsoft.Extensions.Configuration;
using Moq;
using Telegram.Bot.Types;

namespace JobScraperBot.Tests
{
    public class RequestStringServiveTests
    {
#pragma warning disable S3263 // Static fields should appear in the order they must be initialized
        private static IOptionsProvider optionsProvider = new OptionsProvider();
        private static IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();
#pragma warning restore S3263 // Static fields should appear in the order they must be initialized

        private static Dictionary<string, string> configData = new Dictionary<string, string>
        {
            { "parsingApi", "https://parsingapi/jobs/find?" },
        };

        [Theory]
        [MemberData(nameof(UserSettingsCollectionValid))]
        public void GetRequestString_ValidData_ReturnsRequestString(UserSettings userSettings, string expected)
        {
            // Arrange
            var requestStringBuilder = new RequestStringServive(configuration, optionsProvider);

            // Act
            bool isEqual = string.Equals(requestStringBuilder.GetRequestString(userSettings), expected, StringComparison.InvariantCulture);

            // Assert
            Assert.True(isEqual);
        }

        [Theory]
        [MemberData(nameof(UserSettingsCollectionInValid))]
        public void GetRequestString_InValidData_ThrowsArgumentException(UserSettings userSettings)
        {
            // Arrange
            var requestStringBuilder = new RequestStringServive(configuration, optionsProvider);

            // Act
            var result = requestStringBuilder.GetRequestString;

            // Assert
            Assert.ThrowsAny<ArgumentException>(() => result(userSettings));
        }

        public static IEnumerable<object[]> UserSettingsCollectionValid =>
            new List<object[]>()
            {
                new object[]
                {
                    new UserSettings()
                    {
                        Stack = optionsProvider.Stacks[JobStacks.CSharpDotNET],
                        Grade = optionsProvider.Levels[Grades.Junior],
                        Type = string.Empty,
                    },
                    configuration["parsingApi"]! + "JobStack=CSharpDotNET" + "&Grade=Junior",
                },
                new object[]
                {
                    new UserSettings()
                    {
                        Stack = optionsProvider.Stacks[JobStacks.Java],
                        Grade = optionsProvider.Levels[Grades.Middle],
                        Type = string.Empty,
                    },
                    configuration["parsingApi"]! + "JobStack=Java" + "&Grade=Middle",
                },
                new object[]
                {
                    new UserSettings()
                    {
                        Stack = optionsProvider.Stacks[JobStacks.JavaScriptFrontEnd],
                        Grade = optionsProvider.Levels[Grades.Senior],
                        Type = optionsProvider.JobKinds[JobTypes.Remote],
                    },
                    configuration["parsingApi"]! + "JobStack=JavaScriptFrontEnd" + "&Grade=Senior" + "&JobType=Remote",
                },
            };

        public static IEnumerable<object[]> UserSettingsCollectionInValid =>
            new List<object[]>()
            {
                new object[]
                {
                    new UserSettings()
                    {
                        Stack = string.Empty,
                        Grade = optionsProvider.Levels[Grades.Junior],
                        Type = null,
                    },
                },
                new object[]
                {
                    new UserSettings()
                    {
                        Stack = optionsProvider.Stacks[JobStacks.Java],
                        Grade = null!,
                        Type = string.Empty,
                    },
                },
                new object[]
                {
                    new UserSettings()
                    {
                        Stack = null!,
                        Grade = optionsProvider.Levels[Grades.Senior],
                        Type = "random string",
                    },
                },
                new object[]
                {
                    null!,
                },
            };
    }
}
