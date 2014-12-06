using UnityEngine;
using System.Collections;
using MothershipOS;

namespace MothershipStateMachine
{
    public abstract class GameMessage { }

    public class RegisterClient : GameMessage
    {
        public User User { get; set; }
        public Profile Profile { get; set; }
    }

    public class UpdateTeamRoster : GameMessage
    {
        public TeamList RedTeam { get; set; }
        public TeamList BlueTeam { get; set; }
    }
}
