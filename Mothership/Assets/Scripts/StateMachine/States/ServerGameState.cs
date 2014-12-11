using UnityEngine;
using System.Collections;

namespace MothershipStateMachine
{
    /// <summary>
    /// Base Game State - only process messages here that will be aplicable to all game states
    /// </summary>
    public abstract class ServerGameState : IState
    {
        protected ServerManager serverManager = null;

        public ServerGameState(ServerManager manager)
        {
            serverManager = manager;
        }

        public virtual void OnGameMessage(GameMessage message)
        {
            // TODO: common tasks for client disconnections
        }

        public virtual void OnStateMessage(StateMessage message)
        {

        }
    }
}
