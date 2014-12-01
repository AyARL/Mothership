using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CNPCSO : ScriptableObject
{
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
