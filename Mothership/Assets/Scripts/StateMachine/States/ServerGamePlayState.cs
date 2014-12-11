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
            MatchExpired expired = message as MatchExpired;
            if(expired != null)
            {
                serverManager.ChangeState(serverManager.ServerGameEndState);
                return;
            }

            base.OnGameMessage(message);
        }

        public override void OnStateMessage(StateMessage message)
        {
            OnEnterState enter = message as OnEnterState;
            if(enter != null)
            {
                serverManager.StartGameTimer();
                serverManager.networkManager.GamePlayStarted();
            }
        }
    }
}
