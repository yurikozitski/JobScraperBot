using JobScraperBot.Services.Implementations;
using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using Moq;

namespace JobScraperBot.Tests
{
    public class MessageValidatorTests
    {
        [Theory]
        [InlineData(null, UserState.OnStart, false)]
        [InlineData("", UserState.OnGreeting, false)]
        [InlineData("somerandomstring", UserState.OnGreeting, true)]
        [InlineData("Full Stack", UserState.OnStackChoosing, true)]
        [InlineData("Trainee/Intern", UserState.OnGradeChoosing, true)]
        [InlineData("Віддалено або в офісі", UserState.OnTypeChoosing, true)]
        [InlineData("Налаштувати підписку", UserState.OnResultChoosing, true)]
        [InlineData("через день, 07:38", UserState.OnSubscriptionSetting, true)]
        [InlineData("/reset", UserState.OnSubscriptionSetting, true)]
        [InlineData("/confirm", UserState.OnSubscriptionSetting, true)]
        public void MessageValidator_IsMessageValid_ValidatesMessages(string message, UserState userState, bool expected)
        {
            // arrange
            var optionsProviderMock = new Mock<IOptionsProvider>();
            optionsProviderMock
                .Setup(x => x.Stacks)
                .Returns(new OptionsProvider().Stacks);
            optionsProviderMock
                .Setup(x => x.Levels)
                .Returns(new OptionsProvider().Levels);
            optionsProviderMock
                .Setup(x => x.ResultTypes)
                .Returns(new OptionsProvider().ResultTypes);
            optionsProviderMock
                .Setup(x => x.JobKinds)
                .Returns(new OptionsProvider().JobKinds);

            var messageValidator = new MessageValidator(optionsProviderMock.Object);

            // act
            bool isMessageValid = messageValidator.IsMessageValid(message, userState);

            // assert
            Assert.Equal(expected, isMessageValid);
        }
    }
}
