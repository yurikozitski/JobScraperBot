using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (State == UserState.OnEnd)
                return false;
            State++;
            return true;
        }
    }
}
