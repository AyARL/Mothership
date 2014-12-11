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
            GamePlayStarted gameStarted = message as GamePlayStarted;
            if(gameStarted != null)
            {
                clientManager.ChangeState(clientManager.ClientGamePlayState, gameStarted);
            }
        }

        public override void OnStateMessage(StateMessage message)
        {
            OnEnterState enter = message as OnEnterState;
            if(enter != null)
            {
                if(clientManager.SpawnInGame())
                {
                    clientManager.NetworkManager.PlayerSpawned();
                }
            }
        }

    }
}