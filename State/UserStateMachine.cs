namespace JobScraperBot.State
{
    internal class UserStateMachine : IUserStateMachine
    {
        public UserState State { get; private set; }

        public UserSettings UserSettings { get; }

        public UserStateMachine(UserSettings userSettings)
        {
            this.UserSettings = userSettings;
        }

        public bool MoveNext()
        {
            if (this.State == UserState.OnEnd)
                return false;
            this.State++;
            return true;
        }
    }
}
