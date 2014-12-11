using UnityEngine;
using System.Collections;
using Mothership;

namespace MothershipStateMachine
{
    public class ServerGameSetupState : ServerGameState
    {
        public ServerGameSetupState(ServerManager manager) : base(manager)
        {
            
        }

        public override void OnGameMessage(GameMessage message)
        {
            base.OnGameMessage(message);
        }

        public override void OnStateMessage(StateMessage message)
        {

        }
    } 
}
