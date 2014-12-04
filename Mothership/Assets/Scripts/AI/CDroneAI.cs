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
        DRONE_DEAD,
        DRONE_FLEE,
	}
	
    // Drone state should be idle by default
    [ SerializeField ]
    private EDroneState m_eState = EDroneState.DRONE_IDLE;
    public EDroneState DroneState { get { return m_eState; } }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               Start
    /////////////////////////////////////////////////////////////////////////////
    void Start ()
    {
        // Run the start function
        base.Start();

        // Drone initialization.
        m_fHealth = Constants.DEFAULT_HEALTH_DRONE;
        m_fSpeedMultiplier = Constants.DEFAULT_SPEED_DRONE;
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
        // The drone is the "scout" and generally is only interested in retrieving 
        //  the ray, reason why we're going to search for the closest powerup and
        //  set it as the target.
		m_fElapsedTime += Time.deltaTime;
		
		if (m_fElapsedTime > m_fOldTime)
		{
			switch ( m_eState )
			{
                case EDroneState.DRONE_IDLE:

                    // Run the Idle state.
                    RunIdleState();

				break;
				
			case EDroneState.DRONE_MOVING:
				
                    // Run movement logic.
                    RunMovingState();

				break;

            case EDroneState.DRONE_ATTACKING:

                    // Run Attack logic.
                    RunAttackState();

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

        // The drone will attempt to run away back to its base when its health is
        //  getting low.
        if ( m_fHealth < Constants.DEFAULT_HEALTH_DRONE / 2 )
        {
            m_v3Target = m_goHomeBase.transform.position;
            return;
        }

        // We don't want the drone to lazy about, so if it's not holding a ray gun
        //  powerup, we want it to head out and find it.
        if ( m_iItemId != -1 )
        {
            if ( m_iItemId == PowerUpIDs.ID_RAYGUN )
            {
                // We have the ray gun, tell the drone to go back home.
                m_v3Target = m_goHomeBase.transform.position;
                return;
            }
        }

        // We're not holding anything, find the closest powerup and go pick it up.
        GameObject goPowerup = CPowerUp.GetClosestPowerUp( transform );
        if ( null == goPowerup )
        {
            // We couldn't find any power ups, return.
            return;
        }

        // We managed to find a powerup, tell the drone to go get it.
        m_v3Target = goPowerup.transform.position;
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
                m_eState = EDroneState.DRONE_IDLE;
                m_v3Target = Vector3.zero;
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
    }
  
    /////////////////////////////////////////////////////////////////////////////
    /// Function:               CheckForTransitions
    /////////////////////////////////////////////////////////////////////////////
    private void CheckForTransitions()
    {
        // We need to constantly check if the NPC died.
        if ( m_fHealth <= 0 )
            m_eState = EDroneState.DRONE_DEAD;

        // Get a handle on the closest enemy and calculate distance from it.
        GameObject goClosestEnemy = GetClosestEnemy();
        float fDistance = Vector3.Distance( goClosestEnemy.transform.position, transform.position );

        // According to current state, we will check if we need to transition to
        //  a different state.
        switch ( m_eState )
        {
            case EDroneState.DRONE_IDLE:

                if ( true == m_bIsBeingAttacked )
                {
                    // Attempt to decide if we should attack or flee.
                    if ( m_fHealth < Constants.DEFAULT_HEALTH_DRONE / 2 )
                    {
                        // There's a 4 in 6 chance that the drone will try to run to home base.
                        if ( Random.Range( 0, 5 ) < 4 )
                        {
                            m_v3Target = m_goHomeBase.transform.position;
                            m_eState = EDroneState.DRONE_MOVING;
                            break;
                        }
                    }
                }

                if ( fDistance <= Constants.DEFAULT_ATTACK_RANGE )
                {
                    m_v3Target = Vector3.zero;
                    m_eState = EDroneState.DRONE_ATTACKING;
                    m_bTargetInRange = true;
                    break;
                }

                if ( Vector3.zero != m_v3Target )
                {
                    MoveOrder( m_v3Target );
                    m_eState = EDroneState.DRONE_MOVING;
                }

                break;
            case EDroneState.DRONE_MOVING:
                break;
                
            case EDroneState.DRONE_ATTACKING:

                StartCoroutine( AttackTarget( goClosestEnemy.transform ) );

                if ( fDistance > Constants.DEFAULT_ATTACK_RANGE + 20f )
                {
                    m_v3Target = goClosestEnemy.transform.position;
                    m_eState = EDroneState.DRONE_MOVING;
                    m_bTargetInRange = false;
                }

                break;
        }
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
                m_fHealth += 0.5f;
            }
            else if ( goCollisionObject.name == Names.NAME_BLUE_BASE && m_eTeam == ETeam.TEAM_BLUE )
            {
                m_fHealth += 0.5f;
            }
        } 
    }
}
