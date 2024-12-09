using System.Net;
using System.Text.Json;
using Azure;
using Castle.Core.Logging;
using FluentAssertions;
using JobScraperBot.DAL.Interfaces;
using JobScraperBot.DAL.Repositories;
using JobScraperBot.Exceptions;
using JobScraperBot.Services.Implementations;
using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using JobScraperBot.Tests.Helpers;
using JobsScraper.BLL.Models;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Moq;
using Moq.Protected;
using RichardSzalay.MockHttp;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace JobScraperBot.Tests
{
    public class VacancyServiceTests
    {
        private readonly Mock<ILogger<VacancyService>> loggerMock;
        private readonly Mock<ITelegramBotClient> botClientMock;

        public VacancyServiceTests()
        {
            this.loggerMock = new Mock<ILogger<VacancyService>>();
            this.botClientMock = new Mock<ITelegramBotClient>();
        }

        [Theory]
        [InlineData("testData.json")]
        [InlineData("")]
        public async Task GetVacanciesAsync_ValidData_ReturnsVacancyList(string testDataFilePath)
        {
            // Arrange
            string testDataUri = "http://gettestdata";
            string response = !string.IsNullOrEmpty(testDataFilePath) ? await System.IO.File.ReadAllTextAsync(testDataFilePath) : "[]";

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(testDataUri)
                    .Respond("application/text", response);

            var httpClient = new HttpClient(mockHttp);

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var requestStringServiceMock = new Mock<IRequestStringService>();
            requestStringServiceMock
                .Setup(x => x.GetRequestString(It.IsAny<UserSettings>()))
                .Returns(testDataUri);

            var hiddenVacancyRepoMock = new Mock<IHiddenVacancyRepository>();

            var vacancyService = new VacancyService(
                httpClientFactoryMock.Object,
                requestStringServiceMock.Object,
                hiddenVacancyRepoMock.Object,
                this.loggerMock.Object);

            // Act
            var vacancies = await vacancyService.GetVacanciesAsync(this.botClientMock.Object, It.IsAny<long>(), It.IsAny<UserSettings>());

            // Assert
            var expectedVacancies = JsonSerializer.Deserialize<IEnumerable<Vacancy>>(
                response,
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,
                });

            Assert.Equal(expectedVacancies?.Count(), vacancies.Count());
        }

        [Fact]
        public async Task GetVacanciesAsync_CantLoadData_ThrowsVacancyLoadException()
        {
            // Arrange
            string testDataUri = "http://gettestdata";

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(testDataUri)
                    .Throw(new HttpRequestException());

            var httpClient = new HttpClient(mockHttp);

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var requestStringServiceMock = new Mock<IRequestStringService>();
            requestStringServiceMock
                .Setup(x => x.GetRequestString(It.IsAny<UserSettings>()))
                .Returns(testDataUri);

            var hiddenVacancyRepoMock = new Mock<IHiddenVacancyRepository>();

            var vacancyService = new VacancyService(
                httpClientFactoryMock.Object,
                requestStringServiceMock.Object,
                hiddenVacancyRepoMock.Object,
                this.loggerMock.Object);

            // Act
            var result = vacancyService.GetVacanciesAsync;

            // Assert
            await Assert.ThrowsAnyAsync<VacancyLoadException>(async () => await result(this.botClientMock.Object, It.IsAny<long>(), It.IsAny<UserSettings>()));
        }

        [Fact]
        public async Task ShowVacanciesAsync_EmptyCollection_RespondsToTheUserOnce()
        {
            // Arrange
            long chatId = 678150967L;
            string testDataUri = "http://gettestdata";

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(testDataUri)
                    .Respond("application/text", It.IsAny<string>());

            var httpClient = new HttpClient(mockHttp);

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var requestStringServiceMock = new Mock<IRequestStringService>();
            requestStringServiceMock
                .Setup(x => x.GetRequestString(It.IsAny<UserSettings>()))
                .Returns(testDataUri);

            var hiddenVacancyRepoMock = new Mock<IHiddenVacancyRepository>();

            var vacancyService = new VacancyService(
                httpClientFactoryMock.Object,
                requestStringServiceMock.Object,
                hiddenVacancyRepoMock.Object,
                this.loggerMock.Object);

            // Act
            await vacancyService.ShowVacanciesAsync(this.botClientMock.Object, chatId, new List<Vacancy>());

            // Assert
            this.botClientMock.Invocations.Count.Should().Be(1);
        }

        [Fact]
        public async Task ShowVacanciesAsync_NotEmptyCollection_RespondsToTheUserExactTimes()
        {
            // Arrange
            long chatId = 678150967L;
            string testDataUri = "http://gettestdata";

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(testDataUri)
                    .Respond("application/text", It.IsAny<string>());

            var httpClient = new HttpClient(mockHttp);

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var requestStringServiceMock = new Mock<IRequestStringService>();
            requestStringServiceMock
                .Setup(x => x.GetRequestString(It.IsAny<UserSettings>()))
                .Returns(testDataUri);

            var hiddenVacancyRepoMock = new Mock<IHiddenVacancyRepository>();

            var vacancyService = new VacancyService(
                httpClientFactoryMock.Object,
                requestStringServiceMock.Object,
                hiddenVacancyRepoMock.Object,
                this.loggerMock.Object);

            var testVacancies = JsonSerializer.Deserialize<IEnumerable<Vacancy>>(
                await System.IO.File.ReadAllTextAsync("testData.json"),
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,
                });

            // Act
            await vacancyService.ShowVacanciesAsync(this.botClientMock.Object, chatId, testVacancies!);

            // Assert
            this.botClientMock.Invocations.Count.Should().Be(testVacancies?.Count() + 1);
        }
    }
}
