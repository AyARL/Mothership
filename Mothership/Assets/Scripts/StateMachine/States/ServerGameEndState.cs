using UnityEngine;
using System.Collections;
using Mothership;

namespace MothershipStateMachine
{
    public class ServerGameEndState : ServerGameState
    {
        int blueScore = 0;
        int redScore = 0;

        public ServerGameEndState(ServerManager manager) : base(manager)
        {

        }

        public override void OnGameMessage(GameMessage message)
        {
            base.OnGameMessage(message);
        }

        public override void OnStateMessage(StateMessage message)
        {
            OnEnterState enter = message as OnEnterState;
            if(enter != null)
            {
                serverManager.networkManager.MatchExpired();

                MsgScoreUpdate score = enter.Message as MsgScoreUpdate;
                if(score != null)
                {
                    blueScore = score.BlueScore;
                    redScore = score.RedScore;
                }

                return;
            }
        }
    }
    
}