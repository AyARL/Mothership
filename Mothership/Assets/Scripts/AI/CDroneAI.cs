using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mothership;
using MothershipStateMachine;

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
        m_eNPCType = ENPCType.TYPE_DRONE;
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               Update
    /////////////////////////////////////////////////////////////////////////////
	void Update () 
	{
        // Call in the interface update function.
        base.Update();

        if ( Network.isServer )
        { 
            // Will check if we need to transition to a new state
            CheckForTransitions();

            // Will run the NPCs State machines logic.
            RunStates();
        }
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

                case EDroneState.DRONE_DEAD:

                    // The drone is dead, we have to clean up and get rid of it.
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

        // Set the animator to the idle state.
        SetAnimation( AnimatorValues.ANIMATOR_INDEX_IDLE );

        // We only want to set the target if it's unset.
        if ( m_v3Target != Vector3.zero )
            return;

        // The drone will attempt to find a health pack if its health is low.
        //  But ensure we don't do that if we're holding the flag.
        if ( m_fHealth < Constants.DEFAULT_HEALTH_DRONE / 2 && false == m_bHasFlag )
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
            SetAnimation( AnimatorValues.ANIMATOR_INDEX_MOVING );

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
                m_eState = EDroneState.DRONE_IDLE;
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
        // We want the animator to play the idle animation while attacking.
        SetAnimation( AnimatorValues.ANIMATOR_INDEX_IDLE );

        // Get the direction towards the closest enemy and rotate the NPC to face him.
        Vector3 v3Direction = m_trClosestEnemy.transform.position - transform.position;

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
            m_eState = EDroneState.DRONE_DEAD;
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
                m_eState = EDroneState.DRONE_MOVING;
            }
            return;
        }

        // Get a handle on the closest enemy and calculate distance from it.
        m_trClosestEnemy = GetClosestEnemy();
        if ( null != m_trClosestEnemy )
        { 
            float fDistance = Vector3.Distance( m_trClosestEnemy.transform.position, transform.position );

            if ( fDistance <= Constants.DEFAULT_ATTACK_RANGE )
            {
                m_v3Target = Vector3.zero;
                m_eState = EDroneState.DRONE_ATTACKING;
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

                if ( Vector3.zero != m_v3Target )
                {
                    MoveOrder( m_v3Target );
                    m_eState = EDroneState.DRONE_MOVING;
                }

                break;
            case EDroneState.DRONE_MOVING:
                break;
                
            case EDroneState.DRONE_ATTACKING:

                if ( null == m_trClosestEnemy )
                {
                    // The enemy has been destroyed, switch back to idle.
                    m_eState = EDroneState.DRONE_IDLE;
                    m_v3Target = Vector3.zero;
                    m_bTargetInRange = false;
                    m_bIsBeingAttacked = false;
                    break;
                }

                StartCoroutine( AttackTarget( m_trClosestEnemy.transform ) );

                float fDistance = Vector3.Distance( m_trClosestEnemy.transform.position, transform.position );

                if ( fDistance > Constants.DEFAULT_ATTACK_RANGE + 20f )
                {
                    m_v3Target = m_trClosestEnemy.transform.position;
                    m_eState = EDroneState.DRONE_MOVING;
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
}
