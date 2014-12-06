using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MothershipStateMachine
{
    public class ServerLobbyState : IState
    {
        private ServerManager manager = null;

        public ServerLobbyState(ServerManager manager)
        {
            this.manager = manager;
        }

        public void OnGameMessage(GameMessage message)
        {
            RegisterClient registerClient = message as RegisterClient;
            if(registerClient != null)
            {
                if(manager.RegisterClient(registerClient))
                {
                    IEnumerable<ClientDataOnServer> redTeam = manager.GetTeam(ClientDataOnServer.Team.RedTeam);
                    TeamList redList = new TeamList(ClientDataOnServer.Team.RedTeam, redTeam.Select(c => c.Profile.DisplayName).ToArray());

                    IEnumerable<ClientDataOnServer> blueTeam = manager.GetTeam(ClientDataOnServer.Team.BlueTeam);
                    TeamList blueList = new TeamList(ClientDataOnServer.Team.BlueTeam, blueTeam.Select(c => c.Profile.DisplayName).ToArray());

                    manager.networkManager.SendTeamSetupUpdate(redList, blueList);
                }
            }
        }

        public void OnStateMessage(StateMessage message)
        {

        }        
    }
}
