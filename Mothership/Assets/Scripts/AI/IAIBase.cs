using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mothership;

public class IAIBase : MonoBehaviour 
{
    // The speed of this lovely drone.
    [ SerializeField ]
	protected float m_fSpeed;
    public float Speed { get { return m_fSpeed; } }

    // Rotation speed.
    [ SerializeField ]
    protected float m_fRotationSpeed;
    public float RotationSpeed { get { return m_fRotationSpeed; } }

    // The Health.
    [ SerializeField ]
    protected float m_fHealth = 100;
    public float Health { get { return m_fHealth; } }

    [ SerializeField ]
	protected bool m_bShowPath = false;
	
    // Will indicate if we reached the target node.
	protected bool m_bReachedNode = true;
    public bool ReachedNode { get { return m_bReachedNode; } }

    // Current target position.
    [ SerializeField ]
	protected Vector3 m_v3Target = Vector3.zero;
    public Vector3 TargetPosition { get { return m_v3Target; } }

    protected Vector3 m_v3CurrNode;
	protected int m_iNodeIndex;
	protected List< Vector3 > m_liPath = new List<Vector3>();
	protected float m_fOldTime = 0;
	protected float m_fCheckTime = 0;
	protected float m_fElapsedTime = 0;

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               Update
    /////////////////////////////////////////////////////////////////////////////
    protected void Update()
    {
        // Update the speeds.
        m_fSpeed = Time.deltaTime * Constants.DEFAULT_NPC_SPEED;
        m_fRotationSpeed = Time.deltaTime * Constants.DEFAULT_NPC_SPEED;
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               GoTo
    /////////////////////////////////////////////////////////////////////////////
	protected void GoTo()
	{
        // Check if we want to show this character's path in the editor.
		if ( true == m_bShowPath )
		{
			for ( int i = 0; i < m_liPath.Count - 1; ++i )
			{
				Debug.DrawLine( ( Vector3 )m_liPath[ i ], ( Vector3 )m_liPath[ i + 1 ], Color.white, 0.01f );
			}
		}
		
        // Declare the
		Vector3 v3NewPos = transform.position;
		float fXdistance = v3NewPos.x - m_v3CurrNode.x;
        float fYdistance = v3NewPos.z - m_v3CurrNode.z;

		if ( fXdistance < 0 ) 
            fXdistance -= fXdistance * 2;

		if ( fYdistance < 0 ) 
            fYdistance -= fYdistance * 2;
	
        //if ( ( fXdistance < 0.1 && fYdistance < 0.1 ) && m_v3Target == m_v3CurrNode )
        //{
        //    m_eState = EDroneState.DRONE_IDLE;
        //}

		else if ( fXdistance < 0.1 && fYdistance < 0.1 )
		{
			m_iNodeIndex++;
			m_bReachedNode = true;
		}

		Vector3 v3Motion = m_v3CurrNode - v3NewPos;
		v3Motion.Normalize();

        transform.rotation = Quaternion.Slerp( transform.rotation, Quaternion.LookRotation( v3Motion ), m_fRotationSpeed );

		v3NewPos += v3Motion * m_fSpeed;
		
		transform.position = v3NewPos;
	}
	
    /////////////////////////////////////////////////////////////////////////////
    /// Function:               SetTarget
    /////////////////////////////////////////////////////////////////////////////
	protected void SetTarget()
	{
		m_liPath = CNodeController.FindPath( transform.position, m_v3Target );
		m_iNodeIndex = 0;
		//m_bReachedNode = true;
	}
	
    /////////////////////////////////////////////////////////////////////////////
    /// Function:               MoveOrder
    /////////////////////////////////////////////////////////////////////////////
	protected void MoveOrder( Vector3 v3Pos )
	{
		m_v3Target = v3Pos;
		SetTarget();
		//m_eState = EDroneState.DRONE_MOVING;
	}
}
