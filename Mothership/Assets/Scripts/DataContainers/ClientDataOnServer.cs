using UnityEngine;
using System.Collections;
using MothershipOS;

namespace Mothership
{
    public class ClientDataOnServer
    {
        public User User { get; private set; }
        public Profile Profile { get; private set; }
        public NetworkPlayer NetworkPlayer { get; private set; }
        public IAIBase.ETeam ClientTeam { get; private set; }

        public bool ReadyToPlay { get; set; }
        public bool LoadedLevel { get; set; }

        public float CurrentHealth { get; set; }
        public int KillCount { get; set; }
        public int DeathCount { get; set; }
        public int CaptureCount { get; set; }
        public int EXP { get; set; }

        public bool HasFlag { get; set; }

        public ClientDataOnServer(User user, Profile profile, NetworkPlayer networkPlayer, IAIBase.ETeam team)
        {
            User = user;
            Profile = profile;
            NetworkPlayer = networkPlayer;
            ClientTeam = team;

            CurrentHealth = Constants.DEFAULT_HEALTH_DRONE;
        }
    } 
}
