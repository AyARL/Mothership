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
        public bool Spawned { get; set; }

        public ClientDataOnServer(User user, Profile profile, NetworkPlayer networkPlayer, IAIBase.ETeam team)
        {
            User = user;
            Profile = profile;
            NetworkPlayer = networkPlayer;
            ClientTeam = team;

            ReadyToPlay = false;
        }
    } 
}
