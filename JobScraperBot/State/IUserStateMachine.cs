namespace JobScraperBot.State
{
    public interface IUserStateMachine : IStateMachine<UserState>
    {
        UserSettings UserSettings { get; }

        void Reset();

        void SetState(UserState state);
    }
}
