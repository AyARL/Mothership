using UnityEngine;
using System.Collections;
using System.Linq;
using MothershipOS;

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
                
                // Check team for this player
                int order = updateTeams.RedTeam.TeamDisplayNames.ToList().IndexOf(UserDataManager.userData.Profile.DisplayName);
                if(order > -1)
                {
                    manager.Team = updateTeams.RedTeam.TeamColour;
                    manager.TeamOrder = order;
                }
                else
                {
                    order = updateTeams.BlueTeam.TeamDisplayNames.ToList().IndexOf(UserDataManager.userData.Profile.DisplayName);
                    if (order > -1)
                    {
                        manager.Team = updateTeams.BlueTeam.TeamColour;
                        manager.TeamOrder = order;
                    }
                }

                return;
            }

            ClientReadyToPlay readyToPlay = message as ClientReadyToPlay;
            if(readyToPlay != null)
            {
                manager.NetworkManager.ReadyToPlay();
                return;
            }

            EnteredGame enteredGame = message as EnteredGame;
            if(enteredGame != null)
            {
                manager.ChangeState(manager.ClientGameSetupState);
            }
        }

        public void OnStateMessage(StateMessage message)
        {

        }
    }
}
