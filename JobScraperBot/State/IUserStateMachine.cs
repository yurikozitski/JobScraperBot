namespace JobScraperBot.State
{
    internal interface IUserStateMachine : IStateMachine<UserState>
    {
        UserSettings UserSettings { get; }

        void Reset();

        void SetState(UserState state);
    }
}
