//using JobScraperBot.Services.Implementations;
//using JobScraperBot.Services.Interfaces;
//using JobScraperBot.State;
//using Moq;
//using Telegram.Bot;
//using Telegram.Bot.Types;

//namespace JobScraperBot.Tests
//{
//    public class MenuHandlerTests
//    {
//        private readonly Mock<ITelegramBotClient> botClientMock;
//        private readonly Mock<IUserStateMachine> userStateMachineMock;
//        private readonly Mock<IFileRemover> fileRemoverMock;
//        private readonly IUserSubscriptionsStorage userSubscriptionsStorage;

//        public MenuHandlerTests()
//        {
//            this.botClientMock = new Mock<ITelegramBotClient>();
//            this.userStateMachineMock = new Mock<IUserStateMachine>();
//            this.fileRemoverMock = new Mock<IFileRemover>();
//            this.userSubscriptionsStorage = new UserSubscriptionsStorage();
//        }

//        [Fact]
//        public async Task HandleMenuAsync_ResetCommand_ResetsUserState()
//        {
//            // Arrange
//            var message = new Message { Text = "/reset", Chat = new Chat { Id = 12345L } };
//            var menuHandler = new MenuHandler(this.userSubscriptionsStorage, this.fileRemoverMock.Object);

//            // Act
//            await menuHandler.HandleMenuAsync(this.botClientMock.Object, message, this.userStateMachineMock.Object);

//            // Assert
//            this.userStateMachineMock.Verify(x => x.Reset(), Times.Once);
//        }

//        [Fact]
//        public async Task HandleMenuAsync_ResetCommand_RemovesSubscriptionFromSubscriptionStorage()
//        {
//            // Arrange
//            long chatId = 12345L;
//            var message = new Message { Text = "/reset", Chat = new Chat { Id = chatId } };
//            this.userSubscriptionsStorage.Subscriptions.TryAdd(chatId, new Models.SubscriptionInfo(default, default!, default, default));
//            var menuHandler = new MenuHandler(this.userSubscriptionsStorage, this.fileRemoverMock.Object);

//            // Act
//            await menuHandler.HandleMenuAsync(this.botClientMock.Object, message, this.userStateMachineMock.Object);

//            // Assert
//            Assert.Empty(this.userSubscriptionsStorage.Subscriptions);
//        }

//        [Fact]
//        public async Task HandleMenuAsync_ResetCommand_RemovesFileWithHiddenVacancies()
//        {
//            // Arrange
//            var message = new Message { Text = "/reset", Chat = new Chat { Id = 12345L } };
//            string path = Directory.GetCurrentDirectory() + "\\HiddenVacancies" + $"\\{message.Chat.Id}_hidden.txt";
//            var menuHandler = new MenuHandler(this.userSubscriptionsStorage, this.fileRemoverMock.Object);

//            // Act
//            await menuHandler.HandleMenuAsync(this.botClientMock.Object, message, this.userStateMachineMock.Object);

//            // Assert
//            this.fileRemoverMock.Verify(x => x.RemoveFile(path), Times.Once);
//        }

//        [Fact]
//        public async Task HandleMenuAsync_ResetCommand_RemovesFileWithSubscription()
//        {
//            // Arrange
//            var message = new Message { Text = "/reset", Chat = new Chat { Id = 12345L } };
//            string path = Directory.GetCurrentDirectory() + "\\Subscriptions" + $"\\{message.Chat.Id}_subscription.txt";
//            var menuHandler = new MenuHandler(this.userSubscriptionsStorage, this.fileRemoverMock.Object);

//            // Act
//            await menuHandler.HandleMenuAsync(this.botClientMock.Object, message, this.userStateMachineMock.Object);

//            // Assert
//            this.fileRemoverMock.Verify(x => x.RemoveFile(path), Times.Once);
//        }

//        [Fact]
//        public async Task HandleMenuAsync_ConfirmCommand_SetsUserStateToPreviousToResultChoosing()
//        {
//            // Arrange
//            var message = new Message { Text = "/confirm", Chat = new Chat { Id = 12345L } };
//            var userStateMachine = new UserStateMachine(new UserSettings());
//            userStateMachine.SetState(UserState.OnTypeChoosing);
//            var menuHandler = new MenuHandler(this.userSubscriptionsStorage, this.fileRemoverMock.Object);

//            // Act
//            await menuHandler.HandleMenuAsync(this.botClientMock.Object, message, userStateMachine);

//            // Assert
//            Assert.Equal(UserState.OnTypeChoosing, userStateMachine.State);
//        }

//        [Fact]
//        public async Task HandleMenuAsync_ConfirmCommand_SetsUserStateToPreviousState()
//        {
//            // Arrange
//            var message = new Message { Text = "/confirm", Chat = new Chat { Id = 12345L } };
//            var userStateMachine = new UserStateMachine(new UserSettings());
//            userStateMachine.SetState(UserState.OnGreeting);
//            var menuHandler = new MenuHandler(this.userSubscriptionsStorage, this.fileRemoverMock.Object);

//            // Act
//            await menuHandler.HandleMenuAsync(this.botClientMock.Object, message, userStateMachine);

//            // Assert
//            Assert.Equal(UserState.OnStart, userStateMachine.State);
//        }

//        [Fact]
//        public async Task HandleMenuAsync_MessegeIsNull_ThrowsArgumentException()
//        {
//            // Arrange
//            Message message = null!;
//            var menuHandler = new MenuHandler(this.userSubscriptionsStorage, this.fileRemoverMock.Object);

//            // Act
//            var result = menuHandler.HandleMenuAsync;

//            // Assert
//            await Assert.ThrowsAnyAsync<ArgumentException>(async () => await result(this.botClientMock.Object, message, this.userStateMachineMock.Object));
//        }

//        [Theory]
//        [InlineData(null)]
//        [InlineData("")]
//        public async Task HandleMenuAsync_MessegeTextIsNullOrEmpty_ThrowsArgumentException(string messageText)
//        {
//            // Arrange
//            var message = new Message { Text = messageText, Chat = new Chat { Id = 12345L } };
//            var menuHandler = new MenuHandler(this.userSubscriptionsStorage, this.fileRemoverMock.Object);

//            // Act
//            var result = menuHandler.HandleMenuAsync;

//            // Assert
//            await Assert.ThrowsAnyAsync<ArgumentException>(async () => await result(this.botClientMock.Object, message, this.userStateMachineMock.Object));
//        }
//    }
//}
