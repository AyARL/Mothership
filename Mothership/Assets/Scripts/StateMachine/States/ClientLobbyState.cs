using UnityEngine;
using System.Collections;
using System.Linq;
using MothershipOS;
using Mothership;

namespace MothershipStateMachine
{
    public class ClientLobbyState : IState
    {
        private ClientManager clientManager = null;

        public ClientLobbyState(ClientManager manager)
        {
            this.clientManager = manager;
        }

        public void OnGameMessage(GameMessage message)
        {
            RegistrationOnServer registration = message as RegistrationOnServer;
            if(registration != null)
            {
                clientManager.UpdateTeamDetails(registration.Team, registration.TeamOrder);
                return;
            }

            UpdateTeamRoster updateTeams = message as UpdateTeamRoster;
            if(updateTeams != null)
            {
                clientManager.OnUpdateTeamRoster(updateTeams.RedTeam, updateTeams.BlueTeam);
                return;
            }

            ClientReadyToPlay readyToPlay = message as ClientReadyToPlay;
            if(readyToPlay != null)
            {
                clientManager.NetworkManager.ReadyToPlay();
                return;
            }

            EnteredGame enteredGame = message as EnteredGame;
            if(enteredGame != null)
            {
                clientManager.ChangeState(clientManager.ClientGameSetupState);
            }
        }

        public void OnStateMessage(StateMessage message)
        {

        }
    }
}
