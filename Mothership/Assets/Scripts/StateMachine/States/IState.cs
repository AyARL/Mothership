using UnityEngine;
using System.Collections;

namespace MothershipStateMachine
{
    // State Interface
    public interface IState
    {
        void OnGameMessage(GameMessage message);

        void OnStateMessage(StateMessage message);
    }
}