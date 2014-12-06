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
            }
        }

        public void OnStateMessage(StateMessage message)
        {

        }
    }
}
