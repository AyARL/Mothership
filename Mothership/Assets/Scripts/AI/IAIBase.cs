using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mothership;

public class IAIBase : MonoBehaviour 
{
    public enum ETeam
    {
        TEAM_NONE,
        TEAM_RED,
        TEAM_BLUE,
    }

    [ SerializeField ]
    protected ETeam m_eTeam = ETeam.TEAM_NONE;
    public ETeam Team { get { return m_eTeam; } }

    [ SerializeField ]
    protected GameObject m_goHomeBase = null;
    public GameObject HomeBase { get { return m_goHomeBase; } }

    // The speed of this lovely drone.
    [ SerializeField ]
	protected float m_fSpeed;
    public float Speed { get { return m_fSpeed; } }

    // The speed multiplier.
    protected float m_fSpeedMultiplier = 0;
    public float SpeedMultiplier { get { return m_fSpeedMultiplier; } }

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

    protected bool m_bReachedTarget = false;
    public bool ReachedTarget { get { return m_bReachedTarget; } }

    // Current target position.
    [ SerializeField ]
	protected Vector3 m_v3Target = Vector3.zero;
    public Vector3 TargetPosition { get { return m_v3Target; } }

    // Reference to the held item (if any).
    [ SerializeField ]
    protected CPowerUp m_cItem = null;
    public CPowerUp HeldItem { get { return m_cItem; } }

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
        // For error reporting.
        string strFunction = "IAIBase::Update()";

        // Update the speeds.
        m_fSpeed = Time.deltaTime * m_fSpeedMultiplier;
        m_fRotationSpeed = Time.deltaTime * m_fSpeedMultiplier;

        // Ensure that this NPC knows where his homebase is according to his team.
        if ( null == m_goHomeBase )
        {
            string strBaseName;

            switch ( m_eTeam )
            {
                case ETeam.TEAM_BLUE:

                    strBaseName = Names.NAME_BLUE_BASE;
                    
                    break;
                case ETeam.TEAM_RED:

                    strBaseName = Names.NAME_RED_BASE;

                    break;
                default:

                    // This NPC doesn't have a valid team assigned, report the issue.
                    Debug.LogError( string.Format( "{0} {1}", strFunction, ErrorStrings.ERROR_UNASSIGNED_NPC ) );
                    return;
            }

            // Attempt to assign the homebase for this NPC.
            m_goHomeBase = GameObject.Find( strBaseName );
            if ( null == m_goHomeBase )
            {
                // We didn't manage to find the base, report the issue.
                Debug.LogError( string.Format( "{0} {1}: {}", strFunction, ErrorStrings.ERROR_NULL_OBJECT, strBaseName ) );
                return;
            }

        }
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
		
		Vector3 v3NewPos = transform.position;
        float fDistance = 1f;

		if ( Vector3.Distance( transform.position, m_v3CurrNode ) < fDistance && m_v3CurrNode != m_v3Target )
		{
			m_iNodeIndex++;
			m_bReachedNode = true;
		}
        else if ( Vector3.Distance( transform.position, m_v3CurrNode ) < fDistance && m_v3CurrNode == m_v3Target )
        {
            m_bReachedTarget = true;
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
	}
	
    /////////////////////////////////////////////////////////////////////////////
    /// Function:               MoveOrder
    /////////////////////////////////////////////////////////////////////////////
	protected void MoveOrder( Vector3 v3Pos )
	{
		m_v3Target = v3Pos;
		SetTarget();
	}

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               SetTeam
    /////////////////////////////////////////////////////////////////////////////
    protected void SetTeam( ETeam eTeam )
    {
        // Set the team for this NPC.
        m_eTeam = eTeam;
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               OnTriggerEnter
    /////////////////////////////////////////////////////////////////////////////
    protected void OnTriggerEnter( Collider cCollider ) 
    {
        string strFunction = "IAIBase::OnCollisionEnter()";

        // Get a handle on the gameObject with which we collided. 
        GameObject goObject = cCollider.gameObject;

        // Depending on the name of the object, react accordingly.
        switch ( goObject.name )
        {
            case Names.NAME_MISSILE:

                // We've been hit by a missile, damage the NPC.
                m_fHealth -= Constants.DAMAGE_MISSILE;

                break;
            case Names.NAME_BULLET:

                // We've been hit by a bullet, damage the NPC.
                m_fHealth -= Constants.DAMAGE_BULLET;

                break;
            case Names.NAME_RAY:

                // We've been hit by a ray guy, damage the NPC.
                m_fHealth -= Constants.DAMAGE_RAYGUN;

                break;
            case Names.NAME_POWER_UP:

                // Attempt to get a handle on the object's powerup script.
                CPowerUp cPowerUp = goObject.GetComponent< CPowerUp >();
                if ( null == cPowerUp )
                {
                    // We failed to get a handle, this shouldn't happen - report the error.
                    Debug.LogError( string.Format( "{0} {1}: {2}", strFunction, ErrorStrings.ERROR_MISSING_COMPONENT, typeof( CPowerUp ).ToString() ) );
                    return;
                }

                // Assign the held item and indicate that we picked up the power up, this
                //  will destroy the powerup.
                m_cItem = cPowerUp;
                //cPowerUp.PickupPowerUp();

                break;
        }
    }
}
