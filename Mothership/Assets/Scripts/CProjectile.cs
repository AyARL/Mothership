using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mothership;

public class CProjectile : MonoBehaviour {

    public enum EProjectileType
    {
        PROJECTILE_NONE,
        PROJECTILE_BULLET,
        PROJECTILE_MISSILE,
        PROJECTILE_RAY,
    }

    private List< string > m_liProjectileNames = new List< string >
    {
        Names.NAME_BULLET + "(Clone)",
        Names.NAME_MISSILE + "(Clone)",
        Names.NAME_RAY + "(Clone)",
    };

    [ SerializeField ]
    private bool m_bIsActivated = false;
    public bool Activation { get { return m_bIsActivated; } set { m_bIsActivated = value; } }

    [ SerializeField ]
    private GameObject m_goInstantiator;
    public GameObject Instantiator { get { return m_goInstantiator; } set { m_goInstantiator = value; } }

    [ SerializeField ]
    private Vector3 m_v3Direction;
    public Vector3 Direction { get { return m_v3Direction; } set { m_v3Direction = value; } }

    [SerializeField]
    private Vector3 firingPosition;
    public Vector3 FiringPosition { get { return firingPosition; } set { firingPosition = value; } }
    private Vector3 replicationError = Vector3.zero;
    private float startTime = 0f;
    private float travelTime = 0f;
    [SerializeField]
    private AnimationCurve adjustmentCurve = null;

    [ SerializeField ]
    private EProjectileType m_eProjectileType = EProjectileType.PROJECTILE_NONE;
    public EProjectileType ProjectileType { get { return m_eProjectileType; } }

    private bool m_bLeavesTrail;

    private float m_fSpeed;
    private float m_fDamage;
    public float Damage { get { return m_fDamage; } }

    private Vector3 m_v3InitialPosition;

	/////////////////////////////////////////////////////////////////////////////
    /// Function:               Start
    /////////////////////////////////////////////////////////////////////////////
	void Start () 
    {
        string strFunction = "CProjectile::Start()";

        // Initialize the projectile
	    m_v3InitialPosition = gameObject.transform.position;
        
        // Calculate replication related values
        replicationError = FiringPosition - m_v3InitialPosition;
        startTime = Time.time;
        travelTime = Vector3.Distance(m_v3InitialPosition, firingPosition + m_v3Direction * Constants.DEFAULT_MAX_PROJECTILE_RANGE * 0.5f) / Constants.PROJECTILE_SPEED_BULLET;

        // Ignore collisions with the firing object
        Physics.IgnoreCollision(collider, Instantiator.collider);

        switch ( m_eProjectileType )
        {
            case EProjectileType.PROJECTILE_BULLET:

                m_fSpeed = Constants.PROJECTILE_SPEED_BULLET;
                m_fDamage = Constants.PROJECTILE_DAMAGE_BULLET;

                break;

            case EProjectileType.PROJECTILE_MISSILE:

                m_fSpeed = Constants.PROJECTILE_SPEED_MISSILE;
                m_fDamage = Constants.PROJECTILE_DAMAGE_MISSILE;

                break;

            case EProjectileType.PROJECTILE_RAY:

                m_fSpeed = Constants.PROJECTILE_SPEED_RAY;
                m_fDamage = Constants.PROJECTILE_DAMAGE_RAY;

                break;

            default:

                // Could not identify projectile type, report error.
                Debug.LogError( string.Format( "{0} {1}", strFunction, ErrorStrings.ERROR_UNASSIGNED_TYPE ) );

                break;
        }
	}
	
	/////////////////////////////////////////////////////////////////////////////
    /// Function:               Update
    /////////////////////////////////////////////////////////////////////////////
	void FixedUpdate () 
    {
        if ( false == m_bIsActivated )
            return;

	    // Check if the projectile needs to be destroyed
        if ( Vector3.Distance( m_v3InitialPosition, transform.position ) > Constants.DEFAULT_MAX_PROJECTILE_RANGE )
        {
            Destroy( gameObject );
        }

        float elapsedTime = Time.time - startTime;
        float normalisedTime = elapsedTime / travelTime;
        float curveValue = adjustmentCurve.Evaluate(normalisedTime);

        Vector3 adjustment = replicationError * curveValue;

        Vector3 newPos = m_v3InitialPosition + m_v3Direction * m_fSpeed * elapsedTime;
        newPos += adjustment;

        transform.Translate(newPos - transform.position);

        // Draw some debug stuff
        Debug.DrawLine(FiringPosition, FiringPosition + m_v3Direction * Constants.DEFAULT_MAX_PROJECTILE_RANGE, Color.green);
        Debug.DrawLine(transform.position, transform.position + newPos, Color.yellow);

	}

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               OnCollisionEnter
    /////////////////////////////////////////////////////////////////////////////
    void OnCollisionEnter( Collision cCollision )
    {
        if ( false == m_bIsActivated || true == m_liProjectileNames.Contains( cCollision.gameObject.name ) )
            return;

        Destroy( gameObject );
    }
}
