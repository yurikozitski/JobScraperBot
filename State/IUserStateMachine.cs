namespace JobScraperBot.State
{
    internal interface IUserStateMachine : IStateMachine<UserState>
    {
        UserSettings UserSettings { get; }
    }
}
