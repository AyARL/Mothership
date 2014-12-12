using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mothership;
using MothershipStateMachine;
using MothershipReplication;

[ RequireComponent( typeof( NetworkView ) ) ]
public class IAIBase : MonoBehaviour 
{
    public enum ETeam
    {
        TEAM_NONE,
        TEAM_RED,
        TEAM_BLUE,
    }

    public enum ENPCType
    {
        TYPE_NONE,
        TYPE_DRONE,
        TYPE_TANK,
        TYPE_HEALER,
        TYPE_WARRIOR,
    }

    protected ENPCType m_eNPCType = ENPCType.TYPE_NONE;
    public ENPCType NPCType { get { return m_eNPCType; } }

    private static GameObject m_goFlagHolder = null;

    private static CPowerUpSO m_ItemsResource = null;
    public static CPowerUpSO ItemsResource { get { return m_ItemsResource; } set { m_ItemsResource = value; } }

    // A list holding all active red NPCs.
    protected static List< GameObject > m_liActiveReds = new List< GameObject >();

    // A list holding all active blue NPCs.
    protected static List< GameObject > m_liActiveBlues = new List< GameObject >();

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

    // Current target position.
    [ SerializeField ]
	protected Vector3 m_v3Target = Vector3.zero;
    public Vector3 TargetPosition { get { return m_v3Target; } }

    // We're going to hold a reference to the "gun" gameobject
    //  from which we're going to fire.
    [ SerializeField ]
    protected GameObject[] m_rggoGuns;

    [ SerializeField ]
    protected GameObject m_goFlagPrefab = null;
	
    // Will indicate if we reached the target node.
	protected bool m_bReachedNode = true;
    public bool ReachedNode { get { return m_bReachedNode; } }

    protected bool m_bReachedTarget = false;
    public bool ReachedTarget { get { return m_bReachedTarget; } }

    // Will flag that this NPC is being attacked.
    protected bool m_bIsBeingAttacked = false;

    protected bool m_bTargetInRange = false;

    protected bool m_bHasFlag = false;

    // Will hold ammo variables.
    protected Dictionary< string, uint > m_dictInventory = new Dictionary< string, uint >();

    // Will hold a reference to the prefabs we want to instantiate.
    protected Dictionary< string, GameObject > m_dictProjectilePrefabs = new Dictionary< string, GameObject >(); 

    // Will hold a handle on this object's animator
    protected Animator m_anAnimator;

    protected GameObject m_goFlag = null;

    protected string m_strAttackerName;
    public string AttackerName { get { return m_strAttackerName; } }

    protected Vector3 m_v3CurrNode;
	protected int m_iNodeIndex;
	protected List< Vector3 > m_liPath = new List<Vector3>();
	protected float m_fOldTime = 0;
	protected float m_fCheckTime = 0;
	protected float m_fElapsedTime = 0;

    bool m_bCanFireBullet = true;
    bool m_bCanFireMissile = true;
    bool m_bCanFireRay = true;

    protected Dictionary< int, AnimatorBoolProperty > m_dictAnimatorStates;

    protected bool IsRunningLocally { get { return !Network.isClient && !Network.isServer; } }

    // Replication variables start here.
    [SerializeField]
    private Transform m_trObservedTransform = null;
    
    [SerializeField]
    private bool m_bSendAnimationFlags = true;

    [SerializeField]
    private float pingMargin = 0.5f;

    protected Transform m_trClosestEnemy;

    private float clientPing;
    private CAIPayload[] m_rgBuffer = new CAIPayload[ 20 ];

    private List< GameObject > m_liEnemyPlayers = new List< GameObject >();

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               Start
    /////////////////////////////////////////////////////////////////////////////
    protected void Start()
    {
        // Error reporting.
        string strFunction = "IAIBase::Start()";

        // Load the items resource.
        if ( null == m_ItemsResource )
        {
            m_ItemsResource = Resources.Load< CPowerUpSO >( ResourcePacks.RESOURCE_CONTAINER_ITEMS );
            if ( null == m_ItemsResource )
            {
                Debug.LogError( string.Format("{0} {1} " + ErrorStrings.ERROR_AUDIO_FAILED_RELOAD, strFunction, ResourcePacks.RESOURCE_CONTAINER_ITEMS ) );
                return;
            }
        }

        // Load in the projectile prefabs.
        m_dictProjectilePrefabs = m_ItemsResource.Weapons;

        // Add this NPC to the correct list depending on its team.
        switch ( m_eTeam )
        {
            case ETeam.TEAM_BLUE:

                m_liActiveBlues.Add( gameObject );

                break;

            case ETeam.TEAM_RED:

                m_liActiveReds.Add( gameObject );

                break;
            default:

                // Unassigned NPC detected, report the issue.
                Debug.LogError( string.Format( "{0} {1}: {2}", strFunction, ErrorStrings.ERROR_UNASSIGNED_NPC, transform.position ) );

                return;
        }

        // Initialize inventory stock.
        m_dictInventory.Add( Names.NAME_BULLET, 500 );
        m_dictInventory.Add( Names.NAME_MISSILE, 0 );
        m_dictInventory.Add( Names.NAME_RAY, 0 );

        // Get a handle on the NPC's animator.
        m_anAnimator = gameObject.GetComponent< Animator >();
        if ( null == m_anAnimator )
        {
            // Shit happened
            Debug.LogError( string.Format( "{0} {1}: {2}", strFunction, ErrorStrings.ERROR_MISSING_COMPONENT, typeof( Animator ).ToString() ) );
            return;
        }

        m_dictAnimatorStates = new Dictionary< int, AnimatorBoolProperty >();
        m_dictAnimatorStates.Add( 0, new AnimatorBoolProperty() { Name = AnimatorValues.ANIMATOR_IS_MOVING, State = false }); // Moving
    }

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
                Debug.LogError( string.Format( "{0} {1}: {2}", strFunction, ErrorStrings.ERROR_NULL_OBJECT, strBaseName ) );
                return;
            }

            // Find the flag.
            if ( null == m_goFlag ) 
                m_goFlag = FindFlag( m_eTeam );

            m_liEnemyPlayers = GetEnemyPlayers( m_eTeam ); 

            // The server will send messages to the clients to inform them
            //  of AI position and state.
            UpdateClients();
        }
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               FindFlag
    /////////////////////////////////////////////////////////////////////////////
    protected GameObject FindFlag( ETeam eTeam = ETeam.TEAM_NONE )
    {
        string strFunction = "IAIBase::CDroneAI()";

        GameObject goObject = null;

        goObject = GameObject.Find( Names.NAME_FLAG );
        if ( null == goObject )
        {
            // Someone is holding the flag, go towards him.
            goObject = m_goFlagHolder;
            //Debug.LogError( string.Format( "{0} {1}: {2}", strFunction, ErrorStrings.ERROR_NULL_OBJECT, typeof( GameObject ).ToString() ) );
        }

        return goObject;
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               GoTo
    /////////////////////////////////////////////////////////////////////////////
	protected void GoTo()
	{
        if ( m_v3Target == Vector3.zero )
            return;

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
    /// Function:               OnCollisionEnter
    /////////////////////////////////////////////////////////////////////////////
    protected void OnCollisionEnter( Collision cCollision ) 
    {
        // Get a handle on the gameObject with which we collided. 
        GameObject goObject = cCollision.gameObject;
        
        // Check if we collided with a base, and act accordingly depending on the
        //  team.
        if ( goObject.tag == Tags.TAG_BASE )
        {
            if ( goObject.name == Names.NAME_RED_BASE && m_eTeam == ETeam.TEAM_RED )
            {
                CollidedWithBase();
            }
            else if ( goObject.name == Names.NAME_BLUE_BASE && m_eTeam == ETeam.TEAM_BLUE )
            {
                CollidedWithBase();
            }
        }

        // Depending on the name of the object, react accordingly.
        switch ( goObject.tag )
        {
            case Tags.TAG_WEAPON:

                // Get the name of the attacker.
                CProjectile cProjectile = goObject.GetComponent< CProjectile >();
                m_strAttackerName = cProjectile.Instantiator.gameObject.name;

                // Reduce the NPCs health depending on the type of projectile.
                if ( goObject.name == Names.NAME_MISSILE + "(Clone)" )
                    m_fHealth -= Constants.PROJECTILE_DAMAGE_MISSILE;

                else if ( goObject.name == Names.NAME_BULLET + "(Clone)" )
                    m_fHealth -= Constants.PROJECTILE_DAMAGE_BULLET;

                else if ( goObject.name == Names.NAME_RAY + "(Clone)" )
                    m_fHealth -= Constants.PROJECTILE_DAMAGE_RAY;

                // We've been attacked by an enemy, flag this fact.
                m_bIsBeingAttacked = true;
                
                break;
        }
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               OnTriggerEnter
    /////////////////////////////////////////////////////////////////////////////
    protected void OnTriggerEnter( Collider cCollider ) 
    {
        string strFunction = "IAIBase::OnTriggerEnter()";

        // Get a handle on the gameObject with which we collided. 
        GameObject goObject = cCollider.gameObject;

        // Depending on the name of the object, react accordingly.
        switch ( goObject.tag )
        {
            case Tags.TAG_POWERUP:

                // Attempt to get a handle on the object's powerup script.
                CPowerUp cPowerUp = goObject.GetComponent< CPowerUp >();
                if ( null == cPowerUp )
                {
                    // We failed to get a handle, this shouldn't happen - report the error.
                    Debug.LogError( string.Format( "{0} {1}: {2}", strFunction, ErrorStrings.ERROR_MISSING_COMPONENT, typeof( CPowerUp ).ToString() ) );
                    return;
                }

                // Increase inventory stocks or health depending on the type of powerup.
                if ( goObject.name == Names.NAME_MISSILE )
                    m_dictInventory[ Names.NAME_MISSILE ] += 3;

                else if ( goObject.name == Names.NAME_BULLET )
                    m_dictInventory[ Names.NAME_MISSILE ] += 50;

                else if ( goObject.name == Names.NAME_RAY )
                    m_dictInventory[ Names.NAME_RAY ] += 1;

                else if ( goObject.name == Names.NAME_HEALTH )
                    m_fHealth += 50;

                cPowerUp.PickupPowerUp();

                break;
        }

        // Check if we collided with the flag.
        if ( cCollider.name == Names.NAME_FLAG )
        {
            // Set the flag's parent.
            m_goFlagHolder = gameObject;
            m_bReachedTarget = true;
            Network.Destroy( cCollider.gameObject );
            m_bHasFlag = true;

            // Send message to the server manager.
            //ServerManager cServer = RoleManager.roleManager as ServerManager;
            //cServer.SendGameMessage( new FlagPickedUp() { PlayerName = gameObject.name } );
        }
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               GetEnemyPlayers
    /////////////////////////////////////////////////////////////////////////////
    protected List< GameObject > GetEnemyPlayers( ETeam eMyTeam )
    {
        List< PlayerController > liControllers = PlayerController.PlayerControllers;
        List< GameObject > liEnemyPlayers = new List< GameObject >();
        foreach ( PlayerController cPlayerController in liControllers )
        {
            switch ( eMyTeam )
            {
                case ETeam.TEAM_BLUE:
                    
                    if ( cPlayerController.gameObject.name == Names.NAME_PLAYER_RED_DRONE + "(Clone)" )
                        liEnemyPlayers.Add( cPlayerController.gameObject );

                    break;
                case ETeam.TEAM_RED:

                    if ( cPlayerController.gameObject.name == Names.NAME_PLAYER_BLUE_DRONE + "(Clone)" )
                        liEnemyPlayers.Add( cPlayerController.gameObject );

                    break;
            }
        }

        return liEnemyPlayers;
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               GetClosestEnemy
    /////////////////////////////////////////////////////////////////////////////
    protected Transform GetClosestEnemy()
    {
        // Loop through the enemy list and get a reference to the closest enemy.
        List< GameObject > liEnemies = new List< GameObject >();

        if ( m_eTeam == ETeam.TEAM_BLUE )
        {
            liEnemies = m_liActiveReds;
        }

        else if ( m_eTeam == ETeam.TEAM_RED )
        {
            liEnemies = m_liActiveBlues;
        }

        float fLowestDistance = -1;
        Transform trClosestEnemy = null;

        m_liEnemyPlayers= GetEnemyPlayers( m_eTeam );

        int iCounter = 0;
        foreach ( GameObject goPlayer in m_liEnemyPlayers )
        {
            float fDistance = Vector3.Distance( goPlayer.transform.position, transform.position );
            // Check if the variable hasn't been set yet.
            if ( fLowestDistance == -1 )
            { 
                fLowestDistance = fDistance ;
                trClosestEnemy = goPlayer.transform;
                continue;
            }

            // Current enemy is closer than previous enemies.
            if ( fLowestDistance > fDistance )
            {
                fLowestDistance = fDistance;
                trClosestEnemy = goPlayer.transform;
            }
        }

        foreach ( GameObject goEnemy in liEnemies )
        {
            float fDistance = Vector3.Distance( goEnemy.transform.position, transform.position );
            // Check if the variable hasn't been set yet.
            if ( fLowestDistance == -1 )
            { 
                fLowestDistance = fDistance ;
                trClosestEnemy = goEnemy.transform;
                continue;
            }

            // Current enemy is closer than previous enemies.
            if ( fLowestDistance > fDistance )
            {
                fLowestDistance = fDistance;
                trClosestEnemy = goEnemy.transform;
            }
        }

        return trClosestEnemy;
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               AttackTarget
    /////////////////////////////////////////////////////////////////////////////
    protected IEnumerator AttackTarget( Transform trEnemy )
    {
        while ( true == m_bTargetInRange )
        {
            // We're going to supply this vector as a firing position.
            Vector3 v3GunPosition = m_rggoGuns[ Random.Range( 0, m_rggoGuns.Length - 1 ) ].transform.position;

            // Check if we have any bullets in our inventory and fire.
            if ( m_dictInventory[ Names.NAME_BULLET ] > 0 && true == m_bCanFireBullet )
            { 
                m_bCanFireBullet = false;
                
                if ( !IsRunningLocally )
                {
                    networkView.RPC( RPCFunctions.RPC_FIRE, RPCMode.All, Names.NAME_BULLET, v3GunPosition, transform.forward );
                }
                else
                {
                    Fire( Names.NAME_BULLET, v3GunPosition, transform.forward );
                }

                yield return new WaitForSeconds( Constants.PROJECTILE_DELAY_BULLET );
                m_bCanFireBullet = true;
            }

            // Check if we have any missiles in our inventory and fire.
            if ( m_dictInventory[ Names.NAME_MISSILE ] > 0 && true == m_bCanFireMissile )
            { 
                m_bCanFireMissile = false;

                if ( !IsRunningLocally )
                {
                    networkView.RPC( RPCFunctions.RPC_FIRE, RPCMode.All, Names.NAME_MISSILE, v3GunPosition, transform.forward );
                }
                else
                {
                    Fire( Names.NAME_MISSILE, v3GunPosition, transform.forward );
                }

                yield return new WaitForSeconds( Constants.PROJECTILE_DELAY_MISSILE );
                m_bCanFireMissile = true;
            }

            // Check if we have any rays in our inventory and fire.
            if ( m_dictInventory[ Names.NAME_RAY ] > 0 && true == m_bCanFireRay )
            { 
                m_bCanFireRay = false;
                
                if ( !IsRunningLocally )
                {
                    networkView.RPC( RPCFunctions.RPC_FIRE, RPCMode.All, Names.NAME_RAY, v3GunPosition, transform.forward );
                }
                else
                {
                    Fire( Names.NAME_RAY, v3GunPosition, transform.forward );
                }

                yield return new WaitForSeconds( Constants.PROJECTILE_DELAY_RAY );
                m_bCanFireRay = true;
            }

            yield return null;
        }
    } 

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               Die
    /////////////////////////////////////////////////////////////////////////////
    protected void Die( bool bInstantiateFlag = true )
    {
        string strFunction = "IAIBase::Die()";

        switch ( m_eTeam )
        {
            case ETeam.TEAM_BLUE:

                m_liActiveBlues.Remove( gameObject );

                break;

            case ETeam.TEAM_RED:

                m_liActiveReds.Remove( gameObject );

                break;
            default:

                // Unassigned NPC detected, report the issue.
                Debug.LogError( string.Format( "{0} {1}: {2}", strFunction, ErrorStrings.ERROR_UNASSIGNED_NPC, transform.position ) );

                return;
        }

        // We need to drop the flag if we're holding it.
        if ( true == m_bHasFlag && true == bInstantiateFlag )
        { 
            GameObject goFlag = (GameObject)Network.Instantiate( m_goFlagPrefab, transform.position, Quaternion.identity, 0 );
            goFlag.name = Names.NAME_FLAG;
            m_goFlagHolder = null;
        }

        // Inform the server who killed this AI character.
        //ServerManager cServer = RoleManager.roleManager as ServerManager;
        //cServer.SendGameMessage( new AIPlayerKilled() { PlayerName = gameObject.name, KillerName = m_strAttackerName } );

        Network.Destroy( gameObject );
        CSpawner.SpawnNPC( m_eTeam, m_eNPCType );
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               CollidedWithBase
    /////////////////////////////////////////////////////////////////////////////
    protected void CollidedWithBase()
    {
        m_fHealth += 0.5f;

        if ( true == m_bHasFlag )
        {
            // Inform the server that we delivered the flag to the base.
            //ServerManager cServer = RoleManager.roleManager as ServerManager;
            //cServer.SendGameMessage( new FlagDelievered() { PlayerName = gameObject.name } );
            Die( false );
            CSpawner.SpawnFlag();
        }
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               Fire
    /////////////////////////////////////////////////////////////////////////////
    [RPC]
    protected void Fire( string strProjectileName, Vector3 position, Vector3 direction )
    {
        string strFunction = "IAIBase::Fire()";

        GameObject goProjectile;
        if( m_dictProjectilePrefabs.TryGetValue( strProjectileName, out goProjectile ) )
        {
            CProjectile cProjectile = goProjectile.GetComponent< CProjectile >();
            if ( null == cProjectile )
            {
                Debug.LogError( string.Format( "{0} {1}: {2}", strFunction, ErrorStrings.ERROR_MISSING_COMPONENT, typeof( CProjectile ).ToString() ) );
                return;
            }

            cProjectile.Direction = direction;
            cProjectile.Instantiator = gameObject;
            cProjectile.Activation = true;
            cProjectile.FiringPosition = position;
            goProjectile.name = strProjectileName;

             // Select a gun position.
            Vector3 v3GunPosition = m_rggoGuns[ Random.Range( 0, m_rggoGuns.Length - 1 ) ].transform.position;

            Instantiate( goProjectile, v3GunPosition, goProjectile.transform.rotation );

            m_dictInventory[ strProjectileName ]--;
        }
        else
        {
            Debug.LogError( "Unrecognised projectile name: " + strProjectileName );
        }
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               OnSerializeNetworkView
    /////////////////////////////////////////////////////////////////////////////
    private void OnSerializeNetworkView( BitStream stream, NetworkMessageInfo info )
    {
        Vector3 v3Position = m_trObservedTransform.position;
        Quaternion qRotation = m_trObservedTransform.rotation;
        int iAnimFlagIndex = -1;

        if ( stream.isWriting )    // Executed by owner of the network view
        {
            stream.Serialize( ref v3Position );
            stream.Serialize( ref qRotation );

            if ( m_bSendAnimationFlags == true )
            {
                if ( m_dictAnimatorStates.Any( s => s.Value.State == true ) )
                {
                    var animState = m_dictAnimatorStates.First( s => s.Value.State == true );
                    iAnimFlagIndex = animState.Key;

                }
            }
            stream.Serialize( ref iAnimFlagIndex );

        }
        else    // Executed by everyone else receiving the data
        {
            stream.Serialize( ref v3Position );
            stream.Serialize( ref qRotation );
            stream.Serialize( ref iAnimFlagIndex );

            // Shift buffer
            for ( int i = m_rgBuffer.Length - 1; i >= 1; i-- )
            {
                m_rgBuffer[i] = m_rgBuffer[i - 1];
            }

            // Insert latest data at the front of buffer
            m_rgBuffer[ 0 ] = new CAIPayload() { Position = v3Position, Rotation = qRotation, ActiveAnimatorFlagIndex = iAnimFlagIndex, Timestamp = (float)info.timestamp };
        }
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               SetAnimation
    /////////////////////////////////////////////////////////////////////////////
    public void SetAnimation( int index )
    {
        if( index >= 0 )
        {
            m_dictAnimatorStates[ index ].State = true;
            m_anAnimator.SetBool( m_dictAnimatorStates[ index ].Name, true );

            var otherFlags = m_dictAnimatorStates.Where( s => s.Key != index );
            foreach( var flag in otherFlags )
            {
                m_dictAnimatorStates[ flag.Key ].State = false;
                m_anAnimator.SetBool( flag.Value.Name, false );
            }
        }
        else
        {
            foreach ( var flag in m_dictAnimatorStates )
            {
                m_dictAnimatorStates[ flag.Key ].State = false;
                m_anAnimator.SetBool( flag.Value.Name, false );
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               UpdateClients
    /////////////////////////////////////////////////////////////////////////////
    protected void UpdateClients()
    {
        if (!networkView.isMine && Network.connections.Length > 0) // If this is remote side receiving the data and connection exists
        {
            if ( Network.isServer )
            {
                clientPing = (Network.GetAveragePing(Network.connections[0]) / 100) + pingMargin;   // on client the only connection [0] is the server

                float interpolationTime = (float)Network.time - clientPing;

                // make sure there is at least one entry in the buffer
                if (m_rgBuffer[0] == null)
                {
                    m_rgBuffer[0] = new CAIPayload() { Position = m_trObservedTransform.position, Rotation = m_trObservedTransform.rotation, ActiveAnimatorFlagIndex = -1, Timestamp = 0 };
                }

                // Interpolation
                if (m_rgBuffer[0].Timestamp > interpolationTime)
                {
                    for (int i = 0; i < m_rgBuffer.Length; i++)
                    {
                        if (m_rgBuffer[i] == null)
                        {
                            continue;
                        }

                        // Find best fitting state or use the last one available
                        if (m_rgBuffer[i].Timestamp <= interpolationTime || i == m_rgBuffer.Length - 1)
                        {
                            CAIPayload bestTarget = m_rgBuffer[Mathf.Max(i - 1, 0)];
                            CAIPayload bestStart = m_rgBuffer[i];

                            float timeDiff = bestTarget.Timestamp - bestStart.Timestamp;
                            float lerpTime = 0f;

                            if (timeDiff > 0.0001)
                            {
                                lerpTime = ((interpolationTime - bestStart.Timestamp) / timeDiff);
                            }

                            m_trObservedTransform.position = Vector3.Lerp(bestStart.Position, bestTarget.Position, lerpTime);
                            m_trObservedTransform.rotation = Quaternion.Slerp(bestStart.Rotation, bestTarget.Rotation, lerpTime);
                            //controllerScript.CurrentAnimationFlag(bestTarget.ActiveAnimatorFlagIndex);

                            return;
                        }
                    }
                }
                else
                {
                    //Extrapolation
                    float extrapolationTime = (interpolationTime - m_rgBuffer[0].Timestamp);

                    if (m_rgBuffer[0] != null && m_rgBuffer[1] != null)
                    {
                        CAIPayload lastSample = m_rgBuffer[0];
                        CAIPayload prevSample = m_rgBuffer[1];

                        float timeDiff = lastSample.Timestamp - prevSample.Timestamp;
                        float lerpTime = 0f;

                        if (timeDiff > 0.0001)
                        {
                            lerpTime = ((extrapolationTime - lastSample.Timestamp) / timeDiff);
                        }

                        Vector3 predictedPosition = lastSample.Position + prevSample.Position;

                        m_trObservedTransform.position = Vector3.Lerp(lastSample.Position, predictedPosition, lerpTime);
                        m_trObservedTransform.rotation = lastSample.Rotation;
                        //controllerScript.CurrentAnimationFlag(lastSample.ActiveAnimatorFlagIndex);
                    }
                }
            }

            else if ( Network.isClient )
            {
                CAIPayload latest = m_rgBuffer[0];
                m_trObservedTransform.position = latest.Position;
                m_trObservedTransform.rotation = latest.Rotation;
                //m_trObservedTransform.CurrentAnimationFlag(latest.ActiveAnimatorFlagIndex);
            }
        }
    }
}
