using UnityEngine;
using System.Collections;

namespace MothershipStateMachine
{
    public abstract class StateMessage { }

    public class OnEnterState : StateMessage
    {
        public GameMessage Message { get; set; }    // Used to pass additional message when entering state
    }

    public class OnExitState : StateMessage { }

    public class OnUpdateState : StateMessage
    {
        public float DeltaTime { get; set; }
    }
}