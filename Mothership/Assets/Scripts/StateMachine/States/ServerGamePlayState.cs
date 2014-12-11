using UnityEngine;
using System.Collections;
using Mothership;

namespace MothershipStateMachine
{
    public class ServerGamePlayState : ServerGameState
    {
        public ServerGamePlayState(ServerManager manager)
            : base(manager)
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
