using AutoMapper;
using JobScraperBot.DAL.Entities;
using JobScraperBot.DAL.Interfaces;
using JobScraperBot.DAL.Repositories;
using JobScraperBot.Exceptions;
using JobScraperBot.Models;
using JobScraperBot.Services.Implementations;
using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using JobScraperBot.Tests.Helpers;
using Moq;

namespace JobScraperBot.Tests
{
    public class SubscriptionWriterTests
    {
        private readonly IUserSubscriptionsStorage userSubscriptionsStorage;
        private readonly IUserStateMachine userStateMachine;
        private readonly Mock<IMapper> mapperMock;

        public SubscriptionWriterTests()
        {
            this.userSubscriptionsStorage = new UserSubscriptionsStorage();

            this.userStateMachine = new UserStateMachine(new UserSettings()
            {
                Stack = ".NET",
                Grade = "Junior",
            });
            this.userStateMachine.SetState(UserState.OnSubscriptionSetting);

            this.mapperMock = new Mock<IMapper>();
            this.mapperMock.Setup(x => x.Map<Subscription, SubscriptionInfo>(It.IsAny<Subscription>()))
                .Returns(new SubscriptionInfo(
                    It.IsAny<long>(),
                    It.IsAny<UserSettings>(),
                    It.IsAny<MessageInterval>(),
                    It.IsAny<TimeOnly>()));
        }

        [Fact]
        public async Task SubscriptionWriter_ValidData_AddsRecordToDataBase()
        {
            // Arrange
            long chatId = 998150968L;

            using var contextFactory = new TestDbContextFactory();
            var subscriptionRepo = new SubscriptionDbRepository(contextFactory);

            var subscriptionWriter = new SubscriptionWriter(subscriptionRepo, this.mapperMock.Object, this.userSubscriptionsStorage);

            // Act
            await subscriptionWriter.WriteSubscriptionAsync(chatId, "щодня,18:00", this.userStateMachine);

            // Assert
            Assert.NotEmpty(contextFactory.CreateDbContext().Subscriptions.Where(x => x.ChatId == chatId));
        }

        [Fact]
        public async Task SubscriptionWriter_ValidData_AddsRecordToUserSubscriptionsStorage()
        {
            // Arrange
            long chatId = 998150968L;

            using var contextFactory = new TestDbContextFactory();
            var subscriptionRepo = new SubscriptionDbRepository(contextFactory);

            var subscriptionWriter = new SubscriptionWriter(subscriptionRepo, this.mapperMock.Object, this.userSubscriptionsStorage);

            // Act
            await subscriptionWriter.WriteSubscriptionAsync(chatId, "щодня,18:00", this.userStateMachine);

            // Assert
            Assert.NotEmpty(this.userSubscriptionsStorage.Subscriptions);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("щодня,18:00", null)]
        public async Task SubscriptionWriter_InValidData_ThrowsArgumentNullException(string sbscrptnText, IUserStateMachine userStateMachine)
        {
            // Arrange
            var subscriptionWriter = new SubscriptionWriter(
                It.IsAny<ISubscriptionRepository>(),
                this.mapperMock.Object,
                this.userSubscriptionsStorage);

            // Act
            var result = subscriptionWriter.WriteSubscriptionAsync;

            // Assert
            await Assert.ThrowsAnyAsync<ArgumentNullException>(async () => await result(It.IsAny<long>(), sbscrptnText, userStateMachine));
        }

        [Fact]
        public async Task SubscriptionWriter_CantReachDb_ThrowsFailedOperationException()
        {
            // Arrange
            var subRepoMock = new Mock<ISubscriptionRepository>();
            subRepoMock.Setup(x => x.AddAsync(It.IsAny<Subscription>()))
                .ThrowsAsync(new Exception());

            var subscriptionWriter = new SubscriptionWriter(subRepoMock.Object, this.mapperMock.Object, this.userSubscriptionsStorage);

            // Act
            var result = subscriptionWriter.WriteSubscriptionAsync;

            // Assert
            await Assert.ThrowsAnyAsync<FailedOperationException>(async () => await result(It.IsAny<long>(), "щодня,18:00", this.userStateMachine));
        }
    }
}
