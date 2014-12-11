using UnityEngine;
using System.Collections;

namespace MothershipStateMachine
{
    public class ClientGameEndState : ClientGameState
    {
        public ClientGameEndState(ClientManager manager)
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