using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mothership;

public class CDroneAI : IAIBase {
	
    // Declares the drone's states.
	public enum EDroneState
	{
		DRONE_IDLE,
		DRONE_MOVING,
        DRONE_ATTACKING,
        DROME_DEAD,
	}
	
    // Drone state should be idle by default
    [ SerializeField ]
    private EDroneState m_eState = EDroneState.DRONE_IDLE;
    public EDroneState DroneState { get { return m_eState; } }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               Awake
    /////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        m_v3Target = GameObject.Find( "Target" ).transform.position;
    }
	
    /////////////////////////////////////////////////////////////////////////////
    /// Function:               Update
    /////////////////////////////////////////////////////////////////////////////
	void Update () 
	{
        // Call in the interface update function.
        base.Update();

        // Will check if we need to transition to a new state
        CheckForTransitions();

        // Will run the NPCs State machines logic.
        RunStates();
	}

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               RunStates
    /////////////////////////////////////////////////////////////////////////////
    private void RunStates()
    {
		m_fElapsedTime += Time.deltaTime;
		
		if (m_fElapsedTime > m_fOldTime)
		{
			switch ( m_eState )
			{
                case EDroneState.DRONE_IDLE:

				break;
				
			case EDroneState.DRONE_MOVING:
				m_fOldTime = m_fElapsedTime + 0.01f;

				if (m_fElapsedTime > m_fCheckTime)
				{
					m_fCheckTime = m_fElapsedTime + 1;
					SetTarget();
				}
				
				if ( m_liPath != null )
				{
					if ( m_bReachedNode )
					{
						m_bReachedNode = false;
						if ( m_iNodeIndex < m_liPath.Count )
							m_v3CurrNode = m_liPath[m_iNodeIndex];
					} 
                    else
						GoTo();
				}
				break;
			}
		}
    }
  
    /////////////////////////////////////////////////////////////////////////////
    /// Function:               CheckForTransitions
    /////////////////////////////////////////////////////////////////////////////
    private void CheckForTransitions()
    {
        // According to current state, we will check if we need to transition to
        //  a different state.
        switch ( m_eState )
        {
            case EDroneState.DRONE_IDLE:

                if ( null != m_v3Target )
                {
                    MoveOrder( m_v3Target );
                    m_eState = EDroneState.DRONE_MOVING;
                }

                break;
            case EDroneState.DRONE_MOVING:
                break;
        }
    }
}
