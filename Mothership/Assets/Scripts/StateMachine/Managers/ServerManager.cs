using UnityEngine;
using System.Collections;
using MothershipStateMachine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace Mothership
{
    public class ServerManager : RoleManager
    {
        public ServerNetworkManager networkManager = null;

        private List<ClientDataOnServer> registeredClients;
        public IEnumerable<ClientDataOnServer> RegisteredClients { get { return registeredClients; } }

        // States
        public ServerLobbyState ServerLobbyState { get; private set; }
        public ServerGameSetupState ServerGameSetupState { get; private set; }
        public ServerGamePlayState ServerGamePlayState { get; private set; }
        public ServerGameEndState ServerGameEndState { get; private set; }

        // Events
        public UnityAction OnClientRegistered { get; set; }

        public override void Init(NetworkManager networkManager)
        {
            this.networkManager = networkManager as ServerNetworkManager;
            registeredClients = new List<ClientDataOnServer>();

            // Initialise all states
            ServerLobbyState = new ServerLobbyState(this);
            ServerGameSetupState = new ServerGameSetupState(this);
            ServerGamePlayState = new ServerGamePlayState(this);
            ServerGameEndState = new ServerGameEndState(this);

            ChangeState(ServerLobbyState);
        }

        public bool RegisterClient(RegisterClient registerMessage, out IAIBase.ETeam team, out int teamOrder)
        {
            if (registeredClients.Count < Constants.MAX_PLAYERS_IN_GAME)
            {
                team = GetTeamForNextClient(out teamOrder);

                ClientDataOnServer client = new ClientDataOnServer(registerMessage.User, registerMessage.Profile, registerMessage.NetworkPlayer, team);
                registeredClients.Add(client);

                if (OnClientRegistered != null)
                {
                    OnClientRegistered();
                }

                return true;
            }
            else
            {
                Debug.LogError("Maximum number of players reached, cannot register");
                team = IAIBase.ETeam.TEAM_NONE;
                teamOrder = -1;
                return false;
            }
        }

        private IAIBase.ETeam GetTeamForNextClient(out int teamOrder)
        {
            int redCount = registeredClients.Count(c => c.ClientTeam == IAIBase.ETeam.TEAM_RED);
            int blueCount = registeredClients.Count(c => c.ClientTeam == IAIBase.ETeam.TEAM_BLUE);

            IAIBase.ETeam team = redCount <= blueCount ? IAIBase.ETeam.TEAM_RED : IAIBase.ETeam.TEAM_BLUE;
            switch(team)
            {
                case IAIBase.ETeam.TEAM_RED:
                    teamOrder = redCount;
                    break;
                case IAIBase.ETeam.TEAM_BLUE:
                    teamOrder = blueCount;
                    break;
                default:
                    teamOrder = 0;
                    break;
            }
            return team;
        }

        public IEnumerable<ClientDataOnServer> GetTeam(IAIBase.ETeam teamColour)
        {
            return registeredClients.Where(c => c.ClientTeam == teamColour);
        }
    }
    
}