using UnityEngine;
using System.Collections;

namespace Mothership
{
    public class TeamList
    {
        public IAIBase.ETeam TeamColour { get; private set; }
        public TeamListRecord[] TeamDisplayNames { get; private set; }

        public TeamList() { }

        public TeamList(IAIBase.ETeam colour, ClientDataOnServer[] players)
        {
            TeamColour = colour;
            TeamDisplayNames = new TeamListRecord[players.Length];
            int i = 0;
            foreach (ClientDataOnServer p in players)
            {
                TeamDisplayNames[i] = new TeamListRecord() { PlayerName = p.Profile.DisplayName, ReadyInLobby = p.ReadyToPlay, ReadyInMatch = p.LoadedLevel };
                ++i;
            }
        }
    }
    
    public class TeamListRecord
    {
        public string PlayerName { get; set; }
        public bool ReadyInLobby { get; set; }
        public bool ReadyInMatch { get; set; }
    }
}