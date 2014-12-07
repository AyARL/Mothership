using UnityEngine;
using System.Collections;

namespace MothershipStateMachine
{
    public class ClientLobbyState : IState
    {
        private ClientManager manager = null;

        public ClientLobbyState(ClientManager manager)
        {
            this.manager = manager;
        }

        public void OnGameMessage(GameMessage message)
        {
            UpdateTeamRoster updateTeams = message as UpdateTeamRoster;
            if(updateTeams != null)
            {
                manager.OnUpdateTeamRoster(updateTeams.RedTeam, updateTeams.BlueTeam);
                return;
            }

            ClientReadyToPlay readyToPlay = message as ClientReadyToPlay;
            if(readyToPlay != null)
            {
                manager.NetworkManager.ReadyToPlay();
                return;
            }
        }

        public void OnStateMessage(StateMessage message)
        {

        }
    }
}
