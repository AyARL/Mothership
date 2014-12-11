using UnityEngine;
using System.Collections;
using Mothership;
using System.Linq;

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
            ClientSpawned spawned = message as ClientSpawned;
            if (spawned != null)
            {
                ClientDataOnServer client = serverManager.RegisteredClients.FirstOrDefault(c => c.NetworkPlayer == spawned.Player);
                if (client != null)
                {
                    client.Spawned = true;
                    if(serverManager.RegisteredClients.All(c => c.Spawned == true))
                    {
                        serverManager.ChangeState(serverManager.ServerGamePlayState);
                    }
                }
            }

            base.OnGameMessage(message);
        }

        public override void OnStateMessage(StateMessage message)
        {

        }
    }
}
