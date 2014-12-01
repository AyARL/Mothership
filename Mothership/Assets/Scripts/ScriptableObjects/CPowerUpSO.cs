using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CPowerUpSO: ScriptableObject
{
    [ SerializeField ]
    private GameObject[] m_rggoItems;

    List< GameObject > m_liItems;
    public List< GameObject > Items
    { 
        get 
        { 
            UpdateItems(); 
            return m_liItems; 
        } 
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               UpdateItems
    /////////////////////////////////////////////////////////////////////////////
    public void UpdateItems()
    {
        // Clear the objects list.
        m_liItems = new List< GameObject >();

        // Add stuff to it.
        for ( int i = 0; i < m_rggoItems.Length; ++i )
        {
            m_liItems.Add( m_rggoItems[ i ] );
        }
    }
}
