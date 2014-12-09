using UnityEngine;
using System.Collections;
using Mothership;

public class CProjectile : MonoBehaviour {

    public enum EProjectileType
    {
        PROJECTILE_NONE,
        PROJECTILE_BULLET,
        PROJECTILE_MISSILE,
        PROJECTILE_RAY,
    }

    [ SerializeField ]
    private bool m_bIsActivated = false;
    public bool Activation { get { return m_bIsActivated; } set { m_bIsActivated = value; } }

    [ SerializeField ]
    private GameObject m_goInstantiator;
    public GameObject Instantiator { get { return m_goInstantiator; } set { m_goInstantiator = value; } }

    [ SerializeField ]
    private Vector3 m_v3Direction;
    public Vector3 Direction { get { return m_v3Direction; } set { m_v3Direction = value; } }

    [ SerializeField ]
    private EProjectileType m_eProjectileType = EProjectileType.PROJECTILE_NONE;
    public EProjectileType ProjectileType { get { return m_eProjectileType; } }

    private bool m_bLeavesTrail;

    private float m_fForce;

    private Vector3 m_v3InitialPosition;

	/////////////////////////////////////////////////////////////////////////////
    /// Function:               Start
    /////////////////////////////////////////////////////////////////////////////
	void Start () 
    {
        string strFunction = "CProjectile::Start()";

        // Initialize the projectile
	    m_v3InitialPosition = gameObject.transform.position;
        // Bullet will face the the same direction as it's path
        transform.forward = m_v3Direction;
        transform.rotation = Quaternion.LookRotation(transform.forward + transform.rotation.eulerAngles);

        // Ignore collisions with the firing object
        Physics.IgnoreCollision(collider, Instantiator.collider);

        switch ( m_eProjectileType )
        {
            case EProjectileType.PROJECTILE_BULLET:

                m_fForce = Constants.PROJECTILE_FORCE_BULLET;

                break;

            case EProjectileType.PROJECTILE_MISSILE:

                m_fForce = Constants.PROJECTILE_SPEED_MISSILE;

                break;

            case EProjectileType.PROJECTILE_RAY:

                m_fForce = Constants.PROJECTILE_SPEED_RAY;

                break;

            default:

                // Could not identify projectile type, report error.
                Debug.LogError( string.Format( "{0} {1}", strFunction, ErrorStrings.ERROR_UNASSIGNED_TYPE ) );

                break;
        }

       rigidbody.AddForce(m_v3Direction * m_fForce);
	}
	
	/////////////////////////////////////////////////////////////////////////////
    /// Function:               Update
    /////////////////////////////////////////////////////////////////////////////
	void Update () 
    {
        if ( false == m_bIsActivated )
            return;

	    // Check if the projectile needs to be destroyed
        if ( Vector3.Distance( m_v3InitialPosition, transform.position ) > Constants.DEFAULT_MAX_PROJECTILE_RANGE )
        {
            Destroy( gameObject );
        }

        //transform.Translate( m_v3Direction * m_fSpeed * Time.deltaTime );
	}

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               OnCollisionEnter
    /////////////////////////////////////////////////////////////////////////////
    void OnCollisionEnter( Collision cCollision )
    {
        if ( false == m_bIsActivated || cCollision.gameObject == m_goInstantiator )
            return;

        Destroy( gameObject );
    }
}
