using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mothership;

public class CTankAI : IAIBase {

    // Declares the drone's states.
	public enum ETankState
	{
		TANK_IDLE,
		TANK_MOVING,
        TANK_ATTACKING,
        TANK_DEAD,
	}
	
    // Drone state should be idle by default
    [ SerializeField ]
    private ETankState m_eState = ETankState.TANK_IDLE;
    public ETankState DroneState { get { return m_eState; } }

    private GameObject m_goClosestEnemy;

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               Start
    /////////////////////////////////////////////////////////////////////////////
    void Start ()
    {
        // Run the start function
        base.Start();

        // Drone initialization.
        m_fHealth = Constants.DEFAULT_HEALTH_TANK;
        m_fSpeedMultiplier = Constants.DEFAULT_SPEED_TANK;
        m_eNPCType = ENPCType.TYPE_TANK;

        m_dictInventory[ Names.NAME_BULLET ] = 1000;
        m_dictInventory[ Names.NAME_MISSILE ] = 3;
        m_dictInventory[ Names.NAME_RAY ] = 1;
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
                case ETankState.TANK_IDLE:

                    // Run the Idle state.
                    RunIdleState();

				break;
				
			case ETankState.TANK_MOVING:
				
                    // Run movement logic.
                    RunMovingState();

				break;

            case ETankState.TANK_ATTACKING:

                    // Run Attack logic.
                    RunAttackState();

                break;

                case ETankState.TANK_DEAD:

                    // The tank is dead, we have to clean up and get rid of it.
                    Die();

                    break;
			}
		}
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               RunIdleState
    /////////////////////////////////////////////////////////////////////////////
    private void RunIdleState()
    {
        // Error reporting...
        string strFunction = "CDroneAI::RunIdleState()";

        // Check if the moving animation is on and disable it if it is.
        if ( true == m_anAnimator.GetBool( AnimatorValues.ANIMATOR_IS_MOVING ) )
            m_anAnimator.SetBool( AnimatorValues.ANIMATOR_IS_MOVING, false );

        // We only want to set the target if it's unset.
        if ( m_v3Target != Vector3.zero )
            return;

        // The drone will attempt to find a health pack if its health is low.
        //  But ensure we don't do that if we're holding the flag.
        if ( m_fHealth < Constants.DEFAULT_HEALTH_TANK / 2 && false == m_bHasFlag )
        {
            // Find the closest powerup and go pick it up.
            GameObject goPowerup = CPowerUp.GetClosestPowerUp( transform );
            if ( null == goPowerup )
            {
                // Find the flag.
                m_goFlag = FindFlag();

                // We couldn't find any power ups, we want the drone to go for the flag.
                m_v3Target = m_goFlag.transform.position;
                return;
            }
        }

        m_goFlag = FindFlag();

        // We don't want the drone to lazy about, so we send it after the flag if it doesn't hold it.
        if ( false == m_bHasFlag )
        {
            m_v3Target = m_goFlag.transform.position;
        }
        else
        {
            // We're holding the flag, we want the character to head home.
            m_v3Target = m_goHomeBase.transform.position;
        }

    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               RunMovingState
    /////////////////////////////////////////////////////////////////////////////
    private void RunMovingState()
    {
        // Enable the moving animation if it's not on.
        if ( false == m_anAnimator.GetBool( AnimatorValues.ANIMATOR_IS_MOVING ) )
            m_anAnimator.SetBool( AnimatorValues.ANIMATOR_IS_MOVING, true );

        m_fOldTime = m_fElapsedTime + 0.01f;

		if (m_fElapsedTime > m_fCheckTime)
		{
			m_fCheckTime = m_fElapsedTime + 1;
			SetTarget();
		}
				
		if ( m_liPath != null )
		{
			if ( true == m_bReachedNode )
			{
				m_bReachedNode = false;
			    m_v3CurrNode = m_liPath[ m_iNodeIndex ];
			} 
            else if ( true == m_bReachedTarget )
            {
                // We reached our destination, switch to idle and clear the target
                //  vector.
                m_bReachedTarget = false;
                //m_bReachedNode = false;
                m_eState = ETankState.TANK_IDLE;
                m_v3Target = Vector3.zero;
                return;
            }
            else
				GoTo();
		}
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               RunAttackState
    /////////////////////////////////////////////////////////////////////////////
    private void RunAttackState()
    {
        // Get a handle on the closest enemy and calculate distance from it.
        // Enable the moving animation if it's not on.
        if ( true == m_anAnimator.GetBool( AnimatorValues.ANIMATOR_IS_MOVING ) )
            m_anAnimator.SetBool( AnimatorValues.ANIMATOR_IS_MOVING, false );

        Vector3 v3Direction = m_goClosestEnemy.transform.position - transform.position;

        transform.rotation = Quaternion.LookRotation( v3Direction.normalized );
    }
  
    /////////////////////////////////////////////////////////////////////////////
    /// Function:               CheckForTransitions
    /////////////////////////////////////////////////////////////////////////////
    private void CheckForTransitions()
    {
        // We need to constantly check if the NPC died.
        if ( m_fHealth <= 0 )
        { 
            m_eState = ETankState.TANK_DEAD;
            return;
        }

        // The drone will rush to the homebase while it's holding the flag.
        if ( true == m_bHasFlag )
        {
            if ( m_v3Target != m_goHomeBase.transform.position )
            { 
                // Ensure that we're headed towards our base.
                m_v3Target = m_goHomeBase.transform.position;

                MoveOrder( m_v3Target );
                m_eState = ETankState.TANK_MOVING;
            }
            return;
        }

        // Get a handle on the closest enemy and calculate distance from it.
        m_goClosestEnemy = GetClosestEnemy();
        if ( null != m_goClosestEnemy )
        { 
            float fDistance = Vector3.Distance( m_goClosestEnemy.transform.position, transform.position );

            if ( fDistance <= Constants.DEFAULT_ATTACK_RANGE )
            {
                m_v3Target = Vector3.zero;
                m_eState = ETankState.TANK_ATTACKING;
                m_bTargetInRange = true;
            }
        }
        else
        {
            m_bTargetInRange = false;
            m_bIsBeingAttacked = false;
        }

        // According to current state, we will check if we need to transition to
        //  a different state.
        switch ( m_eState )
        {
            case ETankState.TANK_IDLE:

                if ( true == m_bIsBeingAttacked )
                {
                    // Attempt to decide if we should attack or flee.
                    if ( m_fHealth < Constants.DEFAULT_HEALTH_TANK / 2 )
                    {
                        // There's a 4 in 6 chance that the drone will try to run to home base.
                        if ( Random.Range( 0, 5 ) < 4 )
                        {
                            m_v3Target = m_goHomeBase.transform.position;
                            m_eState = ETankState.TANK_MOVING;
                            break;
                        }
                    }
                }

                if ( Vector3.zero != m_v3Target )
                {
                    MoveOrder( m_v3Target );
                    m_eState = ETankState.TANK_MOVING;
                }

                break;
            case ETankState.TANK_MOVING:
                break;
                
            case ETankState.TANK_ATTACKING:

                if ( null == m_goClosestEnemy )
                {
                    // The enemy has been destroyed, switch back to idle.
                    m_eState = ETankState.TANK_IDLE;
                    m_v3Target = Vector3.zero;
                    m_bTargetInRange = false;
                    m_bIsBeingAttacked = false;
                    break;
                }

                StartCoroutine( AttackTarget( m_goClosestEnemy.transform ) );

                float fDistance = Vector3.Distance( m_goClosestEnemy.transform.position, transform.position );

                if ( fDistance > Constants.DEFAULT_ATTACK_RANGE + 20f )
                {
                    m_v3Target = m_goClosestEnemy.transform.position;
                    m_eState = ETankState.TANK_MOVING;
                    m_bTargetInRange = false;
                }

                break;
        }
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               OnCollisionEnter
    /////////////////////////////////////////////////////////////////////////////
    void OnCollisionEnter( Collision cCollision )
    {
        // Run the base IAIBase collision logic.
        base.OnCollisionEnter( cCollision );
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               OnTriggerEnter
    /////////////////////////////////////////////////////////////////////////////
    void OnTriggerEnter( Collider cCollider ) 
    {
        // Run the base IAIBase collision logic.
        base.OnTriggerEnter( cCollider );
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               OnCollisionEnter
    /////////////////////////////////////////////////////////////////////////////
    void OnCollisionStay( Collision cCollision )
    {
        // Error reporting
        string strFunction = "CDroneAI::OnCollisionEnter()";

        // Get a handle on the gameobject we collided with.
        GameObject goCollisionObject = cCollision.gameObject;
        
        // Check if we collided with a base, and act accordingly depending on the
        //  team.
        if ( goCollisionObject.tag == Tags.TAG_BASE )
        {
            if ( goCollisionObject.name == Names.NAME_RED_BASE && m_eTeam == ETeam.TEAM_RED )
            {
                CollidedWithBase();
            }
            else if ( goCollisionObject.name == Names.NAME_BLUE_BASE && m_eTeam == ETeam.TEAM_BLUE )
            {
                CollidedWithBase();
            }
        } 
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               CollidedWithBase
    /////////////////////////////////////////////////////////////////////////////
    private void CollidedWithBase()
    {
        m_fHealth += 0.5f;

        if ( true == m_bHasFlag )
        {
            m_bHasFlag = false;
            m_v3Target = Vector3.zero;
            m_eState = ETankState.TANK_IDLE;
            CSpawner.SpawnFlag();
        }
    }
}
