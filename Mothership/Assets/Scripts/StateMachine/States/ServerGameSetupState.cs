using UnityEngine;
using System.Collections;
using Mothership;
using System.Linq;
using System.Collections.Generic;

namespace MothershipStateMachine
{
    public class ServerGameSetupState : ServerGameState
    {
        public ServerGameSetupState(ServerManager manager)
            : base(manager)
        {

        }

        public override void OnGameMessage(GameMessage message)
        {
            ClientLoadedLevel loaded = message as ClientLoadedLevel;
            if(loaded != null)
            {
                ClientDataOnServer client = serverManager.RegisteredClients.FirstOrDefault(c => c.NetworkPlayer == loaded.Player);
                if (client != null)
                {
                    client.LoadedLevel = true;

                    IEnumerable<ClientDataOnServer> redTeam = serverManager.GetTeam(IAIBase.ETeam.TEAM_RED);
                    TeamList redList = new TeamList(IAIBase.ETeam.TEAM_RED, redTeam.ToArray());

                    IEnumerable<ClientDataOnServer> blueTeam = serverManager.GetTeam(IAIBase.ETeam.TEAM_BLUE);
                    TeamList blueList = new TeamList(IAIBase.ETeam.TEAM_BLUE, blueTeam.ToArray());

                    serverManager.networkManager.SendTeamSetupUpdate(redList, blueList);

                    if (serverManager.RegisteredClients.All(c => c.LoadedLevel == true))
                    {
                        serverManager.CountdownToMatch();
                        serverManager.networkManager.StartMatchCountdown();
                    }
                }
                return;
            }

            MatchCountdownEnded countdownEnded = message as MatchCountdownEnded;
            if(countdownEnded != null)
            {
                serverManager.ChangeState(serverManager.ServerGamePlayState);
                return;
            }

            base.OnGameMessage(message);
        }

        public override void OnStateMessage(StateMessage message)
        {

        }
    }
}
