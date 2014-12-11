using UnityEngine;
using System.Collections;
using Mothership;

namespace MothershipStateMachine
{
    public class ClientGameSetupState : ClientGameState
    {
        public ClientGameSetupState(ClientManager manager)
            : base(manager)
        {
            
        }

        public override void OnGameMessage(GameMessage message)
        {

        }

        public override void OnStateMessage(StateMessage message)
        {
            OnEnterState enter = message as OnEnterState;
            if(enter != null)
            {
                if(clientManager.SpawnInGame())
                {

                }
            }
        }

    }
}