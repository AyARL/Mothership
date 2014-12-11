using UnityEngine;
using System.Collections;
using MothershipStateMachine;

namespace Mothership
{
    public abstract class RoleManager : MonoBehaviour
    {
        public static RoleManager roleManager = null;

        protected IState activeState = null;

        protected virtual void Awake()
        {
            if (roleManager != null)
            {
                Destroy(gameObject);
            }
            else
            {
                roleManager = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public abstract void Init(NetworkManager networkManager);

        public void SendGameMessage(GameMessage message)
        {
            activeState.OnGameMessage(message);
        }

        protected void SendStateMessage(StateMessage message)
        {
            activeState.OnStateMessage(message);
        }

        public void ChangeState(IState newState)
        {
            if (activeState != null)
            {
                SendStateMessage(new OnExitState());
            }

            activeState = newState;
            SendStateMessage(new OnEnterState());
        }

        public void ChangeState(IState newState, GameMessage enterMessage)
        {
            if (activeState != null)
            {
                SendStateMessage(new OnExitState());
            }

            activeState = newState;
            SendStateMessage(new OnEnterState() { Message = enterMessage });
        }

        protected void OnDestroy()
        {
            roleManager = null;
        }
    }
    
}