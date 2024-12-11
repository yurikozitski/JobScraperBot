using JobScraperBot.Services.Implementations;
using JobScraperBot.State;

namespace JobScraperBot.Tests
{
    public class ResultChoosingHandlerTests
    {
        [Fact]
        public void HandleResult_StateOnResultChoosing_MovesToTheNextState()
        {
            // Arrange
            string message = "Отримати зараз";
            var userStateMachine = new UserStateMachine(new UserSettings());
            userStateMachine.SetState(UserState.OnResultChoosing);
            var resultChoosingHandler = new ResultChoosingHandler();

            // Act
            resultChoosingHandler.HandleResult(message, userStateMachine);

            // Assert
            Assert.Equal(UserState.OnResultChoosing + 1, userStateMachine.State);
        }

        [Theory]
        [InlineData(UserState.OnGreeting)]
        [InlineData(UserState.OnGradeChoosing)]
        [InlineData(UserState.OnStackChoosing)]
        public void HandleResult_StateNotOnResultChoosing_RemainsInCurrentState(UserState userState)
        {
            // Arrange
            string message = "Отримати зараз";
            var userStateMachine = new UserStateMachine(new UserSettings());
            userStateMachine.SetState(userState);
            var resultChoosingHandler = new ResultChoosingHandler();

            // Act
            resultChoosingHandler.HandleResult(message, userStateMachine);

            // Assert
            Assert.Equal(userState, userStateMachine.State);
        }

        [Theory]
        [MemberData(nameof(InvalidArguments))]
        public void HandleResult_NullArguments_ThrowsArgumentNullException(string message, IUserStateMachine userState)
        {
            // Arrange
            var resultChoosingHandler = new ResultChoosingHandler();

            // Act
            var result = resultChoosingHandler.HandleResult;

            // Assert
            Assert.Throws<ArgumentNullException>(() => result(message, userState));
        }

        public static IEnumerable<object[]> InvalidArguments =>
            new List<object[]>()
            {
                new object[]
                {
                    null!,
                    new UserStateMachine(new UserSettings()),
                },
                new object[]
                {
                    "Отримати зараз",
                    null!,
                },
                new object[]
                {
                    null!,
                    null!,
                },
            };
    }
}
