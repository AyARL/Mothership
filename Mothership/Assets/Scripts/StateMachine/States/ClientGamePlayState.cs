using UnityEngine;
using System.Collections;
using Mothership;

namespace MothershipStateMachine
{
    public class ClientGamePlayState : ClientGameState
    {
        public ClientGamePlayState(ClientManager manager)
            : base(manager)
        {

        }

        public override void OnGameMessage(GameMessage message)
        {

            MatchExpired expired = message as MatchExpired;
            if(expired != null)
            {
                clientManager.ChangeState(clientManager.ClientGameEndState);
                return;
            }

            base.OnGameMessage(message);
        }

        public override void OnStateMessage(StateMessage message)
        {
            
        }
    }
}
