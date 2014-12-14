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

    public class MatchCountdownStarted : GameMessage
    {
        public float Delay { get; set; }
    }

    public class MatchCountdownEnded : GameMessage { }

    public class ClientSpawned : GameMessage
    {
        public NetworkPlayer Player { get; set; }
    }

    public class GamePlayStarted : GameMessage 
    {
        public float Delay { get; set; }
    }

    public class MsgFlagPickedUp : GameMessage 
    {
        public string PlayerName { get; set; }
        public IAIBase.ETeam PlayerTeam { get; set; }
    }

    public class MsgFlagDelivered : GameMessage
    {
        public string PlayerName { get; set; }
        public IAIBase.ETeam PlayerTeam { get; set; }
    }

    public class PlayerTakenDamage : GameMessage
    {
        public NetworkPlayer Player { get; set; }
        public int Damage { get; set; }
        public string Attacker { get; set; }
        public IAIBase.ETeam AttackerTeam { get; set; }
    }

    public class MsgPlayerDied : GameMessage
    {
        public string PlayerName { get; set; }
        public IAIBase.ETeam PlayerTeam { get; set; }

        public string KillerName { get; set; }
        public IAIBase.ETeam KillerTeam { get; set; }
    }

    public class PlayerRespawn : GameMessage
    {
        public NetworkPlayer Player { get; set; }
    }

    public class MsgScoreUpdate : GameMessage
    {
        public int RedScore { get; set; }
        public int BlueScore { get; set; }
    }

    public class MsgClientStatsUpdate : GameMessage
    {
        public string UserName { get; set; }
        public IAIBase.ETeam Team { get; set; }
        public float CurrentHealth { get; set; }
        public int KillCount { get; set; }
        public int DeathCount { get; set; }
        public int CaptureCount { get; set; }
        public int EXP { get; set; }  
    }

    public class MsgDamageClient : GameMessage
    {
        public string UserName { get; set; }
        public float Damage { get; set; }
    }

    public class MatchExpired : GameMessage { }

    public class GameResultReceived : GameMessage
    {
        public GameResult Result { get; set; }
    }
}
