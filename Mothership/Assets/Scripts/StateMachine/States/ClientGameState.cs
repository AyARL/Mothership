using UnityEngine;
using System.Collections;
using Mothership;

namespace MothershipStateMachine
{
    /// <summary>
    /// Base Game State - only process messages here that will be aplicable to all game states
    /// </summary>
    public abstract class ClientGameState : IState
    {
        protected ClientManager clientManager = null;

        public ClientGameState(ClientManager manager)
        {
            clientManager = manager;
        }

        public virtual void OnGameMessage(GameMessage message)
        {
            // TODO: Process disconnect
        }

        public virtual void OnStateMessage(StateMessage message)
        {
            
        }
        
    }
}
