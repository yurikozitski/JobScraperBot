using JobScraperBot.DAL.Interfaces;
using JobScraperBot.DAL.Repositories;
using JobScraperBot.Services.Implementations;
using JobScraperBot.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace JobScraperBot.Tests
{
    public class VacancyVisibilityServiceTests
    {
        private readonly Mock<ITelegramBotClient> botClientMock;

        public VacancyVisibilityServiceTests()
        {
            this.botClientMock = new Mock<ITelegramBotClient>();
        }

        [Fact]
        public async Task HandleVacancyVisibilityAsync_ValidData_AddsRecordToHiddenVacancies()
        {
            // Arrange
            long chatId = 578150968L;
            string testLink = "https://www.sometestlink.com";

            var message = new Message { Text = default, Chat = new Chat { Id = chatId } };

            var update = new Update()
            {
                CallbackQuery = new CallbackQuery()
                {
                    Data = testLink,
                    Message = message,
                },
            };

            using var contextFactory = new TestDbContextFactory();
            var hiddenVacRepo = new HiddenVacancyDbRepository(contextFactory);

            var vacancyVisibilityService = new VacancyVisibilityService(hiddenVacRepo);

            // Act
            await vacancyVisibilityService.HandleVacancyVisibilityAsync(this.botClientMock.Object, update);

            // Assert
            using var context = contextFactory.CreateDbContext();
            Assert.NotEmpty(context.HiddenVacancies.Where(x => x.ChatId == chatId && x.Link == testLink));
        }

        [Fact]
        public async Task HandleVacancyVisibilityAsync_ValidData_RemovesRecordFromHiddenVacanciesIfAlreadyExists()
        {
            // Arrange
            long chatId = 578150968L;

            using var contextFactory = new TestDbContextFactory();
            using var context = contextFactory.CreateDbContext();

            string testLink = (await context.HiddenVacancies.FirstOrDefaultAsync(x => x.ChatId == chatId))!.Link;

            var message = new Message { Text = default, Chat = new Chat { Id = chatId } };

            var update = new Update()
            {
                CallbackQuery = new CallbackQuery()
                {
                    Data = testLink,
                    Message = message,
                },
            };

            var hiddenVacRepo = new HiddenVacancyDbRepository(contextFactory);
            var vacancyVisibilityService = new VacancyVisibilityService(hiddenVacRepo);

            // Act
            await vacancyVisibilityService.HandleVacancyVisibilityAsync(this.botClientMock.Object, update);

            // Assert
            Assert.Empty(context.HiddenVacancies.Where(x => x.ChatId == chatId && x.Link == testLink));
        }

        [Theory]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(false, false, true)]
        public async Task HandleVacancyVisibilityAsync_InValidData_ThrowsArgumentNullException(bool isUpdateNull, bool isCallbackQueryNull, bool isDataNull)
        {
            // Arrange
            long chatId = 578150968L;
            string testLink = "https://www.sometestlink.com";

            var message = new Message { Text = default, Chat = new Chat { Id = chatId } };

            Update? update = null;

            if (!isUpdateNull)
            {
                update = new Update();
                if (!isCallbackQueryNull)
                {
                    update.CallbackQuery = new CallbackQuery
                    {
                        Message = message,
                    };

                    if (!isDataNull)
                    {
                        update.CallbackQuery.Data = testLink;
                    }
                }
            }

            var hiddenVacRepoMock = new Mock<IHiddenVacancyRepository>();
            var vacancyVisibilityService = new VacancyVisibilityService(hiddenVacRepoMock.Object);

            // Act
            var result = vacancyVisibilityService.HandleVacancyVisibilityAsync;

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await result(this.botClientMock.Object, update!));
        }
    }
}
