using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScraperBot.State
{
    internal interface IUserStateMachine : IStateMachine<UserState>
    {
        UserSettings UserSettings { get; }
    }
}
