using UnityEngine;
using System.Collections;
using Mothership;
using MothershipUI;

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
            GameResultReceived result = message as GameResultReceived;
            if(result != null)
            {
                if(clientManager.OnGameResultReceived != null)
                {
                    clientManager.OnGameResultReceived(result.Result);
                }
            }

            ExitMatch exit = message as ExitMatch;
            if(exit != null)
            {
                Network.Disconnect();
                ScreenDispatch.screenToOpen = ScreenDispatch.ScreenTarget.Profile;
                Application.LoadLevel(0);
            }

            base.OnGameMessage(message);
        }

        public override void OnStateMessage(StateMessage message)
        {
            OnEnterState enter = message as OnEnterState;
            if(enter != null)
            {
                clientManager.Die();
                if(clientManager.OnMatchEnded != null)
                {
                    clientManager.OnMatchEnded();
                }
            }
        }
    }

}