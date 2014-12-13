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
            MatchCountdownStarted countdownStarted = message as MatchCountdownStarted;
            if(countdownStarted != null)
            {
                if(clientManager.OnMatchCountdownStarted != null)
                {
                    clientManager.OnMatchCountdownStarted(countdownStarted.Delay);
                }
            }

            GamePlayStarted gameStarted = message as GamePlayStarted;
            if(gameStarted != null)
            {
                if (clientManager.LoadPrefabs() && clientManager.Spawn())
                {
                    //clientManager.NetworkManager.PlayerSpawned();
                    clientManager.ChangeState(clientManager.ClientGamePlayState, gameStarted);
                }
            }
        }

        public override void OnStateMessage(StateMessage message)
        {

        }

    }
}