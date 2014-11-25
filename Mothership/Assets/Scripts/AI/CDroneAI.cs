using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mothership;

public class CDroneAI : MonoBehaviour {
	
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

    // The speed of this lovely drone.
    [ SerializeField ]
	private float m_fSpeed;
    public float Speed { get { return m_fSpeed; } }

    // The Health.
    [ SerializeField ]
    private float m_fHealth = 100;
    public float Health { get { return m_fHealth; } }

	public bool DebugMode;
	
    // Will indicate if we reached the target node.
	private bool m_bReachedNode = true;
    public bool ReachedNode { get { return m_bReachedNode; } }

    // Current target position.
    [ SerializeField ]
	private Vector3 m_v3Target = Vector3.zero;
    public Vector3 TargetPosition { get { return m_v3Target; } }

    // Target Node.
	Vector3 m_v3CurrNode;
	int m_iNodeIndex;
	List< Vector3 > m_liPath = new List<Vector3>();
	float m_fOldTime = 0;
	float m_fCheckTime = 0;
	float m_fElapsedTime = 0;

    void Awake()
    {
        m_v3Target = GameObject.Find( "Target" ).transform.position;
    }
	
    /////////////////////////////////////////////////////////////////////////////
    /// Function:               Update
    /////////////////////////////////////////////////////////////////////////////
	void Update () 
	{
        // Will run the NPCs State machines logic.
        RunStates();
	}

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               RunStates
    /////////////////////////////////////////////////////////////////////////////
    private void RunStates()
    {
        // Set the NPCs speed.
		m_fSpeed = Time.deltaTime * Constants.DEFAULT_NPC_SPEED;
		m_fElapsedTime += Time.deltaTime;
		
		if (m_fElapsedTime > m_fOldTime)
		{
			switch ( m_eState )
			{
			case EDroneState.DRONE_IDLE:

                MoveOrder( m_v3Target );

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
    /// Function:               GoTo
    /////////////////////////////////////////////////////////////////////////////
	void GoTo()
	{
		if ( DebugMode )
		{
			for ( int i=0; i<m_liPath.Count-1; ++i )
			{
				Debug.DrawLine( ( Vector3 )m_liPath[ i ], ( Vector3 )m_liPath[ i + 1 ], Color.white, 0.01f );
			}
		}
		
		Vector3 v3NewPos = transform.position;
		float fXdistance = v3NewPos.x - m_v3CurrNode.x;
        float fYdistance = v3NewPos.z - m_v3CurrNode.z;

		if ( fXdistance < 0 ) 
            fXdistance -= fXdistance * 2;

		if ( fYdistance < 0 ) 
            fYdistance -= fYdistance * 2;
	
		if ( ( fXdistance < 0.1 && fYdistance < 0.1 ) && m_v3Target == m_v3CurrNode )
		{
			m_eState = EDroneState.DRONE_IDLE;
		}

		else if ( fXdistance < 0.1 && fYdistance < 0.1 )
		{
			m_iNodeIndex++;
			m_bReachedNode = true;
		}

		Vector3 v3Motion = m_v3CurrNode - v3NewPos;
		v3Motion.Normalize();
		v3NewPos += v3Motion * m_fSpeed;
		
		transform.position = v3NewPos;
	}
	
    /////////////////////////////////////////////////////////////////////////////
    /// Function:               SetTarget
    /////////////////////////////////////////////////////////////////////////////
	private void SetTarget()
	{
		m_liPath = CNodeController.FindPath( transform.position, m_v3Target );
		m_iNodeIndex = 0;
		m_bReachedNode = true;
	}
	
    /////////////////////////////////////////////////////////////////////////////
    /// Function:               MoveOrder
    /////////////////////////////////////////////////////////////////////////////
	public void MoveOrder( Vector3 v3Pos )
	{
		m_v3Target = v3Pos;
		SetTarget();
		m_eState = EDroneState.DRONE_MOVING;
	}
}
