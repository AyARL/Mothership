using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CNPCSO : ScriptableObject
{
    [ SerializeField ]
    private GameObject m_goFlag;
    public GameObject Flag { get { return m_goFlag; } }

    [ SerializeField ]
    private GameObject[] m_rggoRedTeamAI;

    [ SerializeField ]
    private GameObject[] m_rggoBlueTeamAI;
 
    private List< GameObject > m_liRedTeamNPCs;
    public List< GameObject > RedTeamNPCs
    {
        get
        {
            UpdateRedTeamObjects();
            return m_liRedTeamNPCs;
        }
    }

    private List< GameObject > m_liBlueTeamNPCs;
    public List< GameObject > BlueTeamNPCs
    {
        get
        {
            UpdateBlueTeamObjects();
            return m_liBlueTeamNPCs;
        }
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               GetObjectByType
    /////////////////////////////////////////////////////////////////////////////
    public GameObject GetObjectByType( IAIBase.ETeam eTeam, IAIBase.ENPCType eType )
    {
        bool bMatchFound = false;

        if ( eTeam == IAIBase.ETeam.TEAM_BLUE )
        {
            UpdateBlueTeamObjects();

            foreach ( GameObject goObject in m_liBlueTeamNPCs )
            {
                switch ( eType )
                {
                    case IAIBase.ENPCType.TYPE_DRONE:

                        CDroneAI cDrone = goObject.GetComponent< CDroneAI >();
                        if ( null == cDrone )
                            continue;

                        bMatchFound = true;

                        break;
                    case IAIBase.ENPCType.TYPE_HEALER:
                        break;
                    case IAIBase.ENPCType.TYPE_TANK:

                        CTankAI cTank = goObject.GetComponent< CTankAI >();
                        if ( null == cTank )
                            continue;

                        bMatchFound = true;

                        break;
                    case IAIBase.ENPCType.TYPE_WARRIOR:

                        CWarriorAI cWarrior = goObject.GetComponent< CWarriorAI >();
                        if ( null == cWarrior )
                            continue;

                        bMatchFound = true;

                        break;
                }
                
                if ( true == bMatchFound )
                {
                    return goObject;
                }
            }
        }
        else if ( eTeam == IAIBase.ETeam.TEAM_RED )
        {
            UpdateRedTeamObjects();

            foreach ( GameObject goObject in m_liRedTeamNPCs )
            {
                switch ( eType )
                {
                    case IAIBase.ENPCType.TYPE_DRONE:

                        CDroneAI cDrone = goObject.GetComponent< CDroneAI >();
                        if ( null == cDrone )
                            continue;

                        bMatchFound = true;

                        break;
                    case IAIBase.ENPCType.TYPE_HEALER:
                        break;
                    case IAIBase.ENPCType.TYPE_TANK:

                        CTankAI cTank = goObject.GetComponent< CTankAI >();
                        if ( null == cTank )
                            continue;

                        bMatchFound = true;

                        break;
                    case IAIBase.ENPCType.TYPE_WARRIOR:

                        CWarriorAI cWarrior = goObject.GetComponent< CWarriorAI >();
                        if ( null == cWarrior )
                            continue;

                        bMatchFound = true;

                        break;
                }
                
                if ( true == bMatchFound )
                {
                    return goObject;
                }
            }
        }

        return null;
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               UpdateRedTeamObjects
    /////////////////////////////////////////////////////////////////////////////
    public void UpdateRedTeamObjects()
    {
        // Clear the objects list.
        m_liRedTeamNPCs = new List< GameObject >();

        // Add stuff to it.
        for ( int i = 0; i < m_rggoRedTeamAI.Length; ++i )
        {
            m_liRedTeamNPCs.Add( m_rggoRedTeamAI[ i ] );
        }
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               UpdateBlueTeamObjects
    /////////////////////////////////////////////////////////////////////////////
    public void UpdateBlueTeamObjects()
    {
        // Clear the objects list.
        m_liBlueTeamNPCs = new List< GameObject >();

        // Add stuff to it.
        for ( int i = 0; i < m_rggoBlueTeamAI.Length; ++i )
        {
            m_liBlueTeamNPCs.Add( m_rggoBlueTeamAI[ i ] );
        }
    }
}
