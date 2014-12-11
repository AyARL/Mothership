using UnityEngine;
using System.Collections;
using MothershipOS;
using Mothership;

namespace MothershipStateMachine
{
    public abstract class GameMessage { }

    public class RegisterClient : GameMessage
    {
        public User User { get; set; }
        public Profile Profile { get; set; }
        public NetworkPlayer NetworkPlayer { get; set; }
    }

    public class ClientDisconnected : GameMessage
    {
        public NetworkPlayer NetworkPlayer { get; set; }
    }

    public class RegistrationOnServer : GameMessage
    {
        public IAIBase.ETeam Team { get; set; }
        public int TeamOrder { get; set; }
    }

    public class UpdateTeamRoster : GameMessage
    {
        public TeamList RedTeam { get; set; }
        public TeamList BlueTeam { get; set; }
    }

    public class ClientReadyToPlay : GameMessage
    {
        public NetworkPlayer Player { get; set; }
    }

    public class ClientLoadedLevel : GameMessage
    {
        public NetworkPlayer Player { get; set; }
        public int Level { get; set; }
    }

    public class EnteredGame : GameMessage { }

    public class ClientSpawned : GameMessage
    {
        public NetworkPlayer Player { get; set; }
    }

    public class GamePlayStarted : GameMessage { }

    public class FlagPickedUp : GameMessage 
    {
        public string PlayerName { get; set; }
    }

    public class FlagDelievered : GameMessage
    {
        public string PlayerName { get; set; }
    }

    public class AIPlayerKilled : GameMessage
    {
        public string PlayerName { get; set; }
        public string KillerName { get; set; }
    }
}
