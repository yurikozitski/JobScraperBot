using JobScraperBot.DAL.Interfaces;
using JobScraperBot.DAL.Repositories;
using JobScraperBot.Exceptions;
using JobScraperBot.Services.Implementations;
using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using JobScraperBot.Tests.Helpers;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace JobScraperBot.Tests
{
    public class MenuHandlerTests
    {
        private readonly Mock<ITelegramBotClient> botClientMock;
        private readonly Mock<IUserStateMachine> userStateMachineMock;
        private readonly IUserSubscriptionsStorage userSubscriptionsStorage;

        public MenuHandlerTests()
        {
            this.botClientMock = new Mock<ITelegramBotClient>();
            this.userStateMachineMock = new Mock<IUserStateMachine>();
            this.userSubscriptionsStorage = new UserSubscriptionsStorage();
        }

        [Fact]
        public async Task HandleMenuAsync_ResetCommand_ResetsUserState()
        {
            // Arrange
            var message = new Message { Text = "/reset", Chat = new Chat { Id = 578150968L } };

            var subRepoMock = new Mock<ISubscriptionRepository>();
            var hiddenVacRepoMock = new Mock<IHiddenVacancyRepository>();

            var menuHandler = new MenuHandler(this.userSubscriptionsStorage, subRepoMock.Object, hiddenVacRepoMock.Object);

            // Act
            await menuHandler.HandleMenuAsync(this.botClientMock.Object, message, this.userStateMachineMock.Object);

            // Assert
            this.userStateMachineMock.Verify(x => x.Reset(), Times.Once);
        }

        [Fact]
        public async Task HandleMenuAsync_ResetCommand_RemovesSubscriptionFromSubscriptionStorage()
        {
            // Arrange
            long chatId = 578150968L;
            var message = new Message { Text = "/reset", Chat = new Chat { Id = chatId } };
            this.userSubscriptionsStorage.Subscriptions.TryAdd(chatId, new Models.SubscriptionInfo(default, default!, default, default));

            var subRepoMock = new Mock<ISubscriptionRepository>();
            var hiddenVacRepoMock = new Mock<IHiddenVacancyRepository>();

            var menuHandler = new MenuHandler(this.userSubscriptionsStorage, subRepoMock.Object, hiddenVacRepoMock.Object);

            // Act
            await menuHandler.HandleMenuAsync(this.botClientMock.Object, message, this.userStateMachineMock.Object);

            // Assert
            Assert.Empty(this.userSubscriptionsStorage.Subscriptions);
        }

        [Fact]
        public async Task HandleMenuAsync_ResetCommand_RemovesHiddenVacancies()
        {
            // Arrange
            long chatId = 578150968L;
            var message = new Message { Text = "/reset", Chat = new Chat { Id = chatId } };

            using var contextFactory = new TestDbContextFactory();
            var subscriptionRepo = new SubscriptionDbRepository(contextFactory);
            var hiddenVacRepo = new HiddenVacancyDbRepository(contextFactory);

            var menuHandler = new MenuHandler(this.userSubscriptionsStorage, subscriptionRepo, hiddenVacRepo);

            // Act
            await menuHandler.HandleMenuAsync(this.botClientMock.Object, message, this.userStateMachineMock.Object);

            // Assert
            using var context = contextFactory.CreateDbContext();
            Assert.Empty(context.HiddenVacancies.Where(x => x.ChatId == chatId));
        }

        [Fact]
        public async Task HandleMenuAsync_ResetCommand_RemovesSubscriptions()
        {
            // Arrange
            long chatId = 578150968L;
            var message = new Message { Text = "/reset", Chat = new Chat { Id = chatId } };

            using var contextFactory = new TestDbContextFactory();
            var subscriptionRepo = new SubscriptionDbRepository(contextFactory);
            var hiddenVacRepo = new HiddenVacancyDbRepository(contextFactory);

            var menuHandler = new MenuHandler(this.userSubscriptionsStorage, subscriptionRepo, hiddenVacRepo);

            // Act
            await menuHandler.HandleMenuAsync(this.botClientMock.Object, message, this.userStateMachineMock.Object);

            // Assert
            using var context = contextFactory.CreateDbContext();
            Assert.Empty(context.Subscriptions.Where(x => x.ChatId == chatId));
        }

        [Fact]
        public async Task HandleMenuAsync_ConfirmCommand_SetsUserStateToPreviousToResultChoosing()
        {
            // Arrange
            var message = new Message { Text = "/confirm", Chat = new Chat { Id = 578150968L } };
            var userStateMachine = new UserStateMachine(new UserSettings());
            userStateMachine.SetState(UserState.OnTypeChoosing);

            var subRepoMock = new Mock<ISubscriptionRepository>();
            var hiddenVacRepoMock = new Mock<IHiddenVacancyRepository>();

            var menuHandler = new MenuHandler(this.userSubscriptionsStorage, subRepoMock.Object, hiddenVacRepoMock.Object);

            // Act
            await menuHandler.HandleMenuAsync(this.botClientMock.Object, message, userStateMachine);

            // Assert
            Assert.Equal(UserState.OnTypeChoosing, userStateMachine.State);
        }

        [Fact]
        public async Task HandleMenuAsync_ConfirmCommand_SetsUserStateToPreviousState()
        {
            // Arrange
            var message = new Message { Text = "/confirm", Chat = new Chat { Id = 578150968L } };
            var userStateMachine = new UserStateMachine(new UserSettings());
            userStateMachine.SetState(UserState.OnGreeting);

            var subRepoMock = new Mock<ISubscriptionRepository>();
            var hiddenVacRepoMock = new Mock<IHiddenVacancyRepository>();

            var menuHandler = new MenuHandler(this.userSubscriptionsStorage, subRepoMock.Object, hiddenVacRepoMock.Object);

            // Act
            await menuHandler.HandleMenuAsync(this.botClientMock.Object, message, userStateMachine);

            // Assert
            Assert.Equal(UserState.OnStart, userStateMachine.State);
        }

        [Fact]
        public async Task HandleMenuAsync_MessegeIsNull_ThrowsArgumentException()
        {
            // Arrange
            Message message = null!;

            var subRepoMock = new Mock<ISubscriptionRepository>();
            var hiddenVacRepoMock = new Mock<IHiddenVacancyRepository>();

            var menuHandler = new MenuHandler(this.userSubscriptionsStorage, subRepoMock.Object, hiddenVacRepoMock.Object);

            // Act
            var result = menuHandler.HandleMenuAsync;

            // Assert
            await Assert.ThrowsAnyAsync<ArgumentException>(async () => await result(this.botClientMock.Object, message, this.userStateMachineMock.Object));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task HandleMenuAsync_MessegeTextIsNullOrEmpty_ThrowsArgumentException(string messageText)
        {
            // Arrange
            var message = new Message { Text = messageText, Chat = new Chat { Id = 578150968L } };

            var subRepoMock = new Mock<ISubscriptionRepository>();
            var hiddenVacRepoMock = new Mock<IHiddenVacancyRepository>();

            var menuHandler = new MenuHandler(this.userSubscriptionsStorage, subRepoMock.Object, hiddenVacRepoMock.Object);

            // Act
            var result = menuHandler.HandleMenuAsync;

            // Assert
            await Assert.ThrowsAnyAsync<ArgumentException>(async () => await result(this.botClientMock.Object, message, this.userStateMachineMock.Object));
        }

        [Fact]
        public async Task HandleMenuAsync_CantReachDb_ThrowsFailedOperationException()
        {
            // Arrange
            var message = new Message { Text = "/reset", Chat = new Chat { Id = 578150968L } };

            var subRepoMock = new Mock<ISubscriptionRepository>();
            var hiddenVacRepoMock = new Mock<IHiddenVacancyRepository>();

            subRepoMock.Setup(x => x.DeleteByChatIdAsync(578150968L))
                .ThrowsAsync(new Exception());

            var menuHandler = new MenuHandler(this.userSubscriptionsStorage, subRepoMock.Object, hiddenVacRepoMock.Object);

            // Act
            var result = menuHandler.HandleMenuAsync;

            // Assert
            await Assert.ThrowsAnyAsync<FailedOperationException>(async () => await result(this.botClientMock.Object, message, this.userStateMachineMock.Object));
        }
    }
}
