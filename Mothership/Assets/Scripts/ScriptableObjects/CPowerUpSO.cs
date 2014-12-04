using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mothership;

public class CPowerUpSO: ScriptableObject
{
    [ SerializeField ]
    private GameObject[] m_rggoItems;

    [ SerializeField ]
    private GameObject[] m_rggoWeapons;

    List< GameObject > m_liItems;
    public List< GameObject > Items
    { 
        get 
        { 
            UpdateItems(); 
            return m_liItems; 
        } 
    }

    Dictionary< string, GameObject > m_dictWeapons;
    public Dictionary< string, GameObject > Weapons
    { 
        get 
        { 
            UpdateWeapons(); 
            return m_dictWeapons; 
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

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               UpdateWeapons
    /////////////////////////////////////////////////////////////////////////////
    public void UpdateWeapons()
    {
        string strFunction = "CPowerUpSO::UpdateWeapons()";

        // Clear the objects list.
        m_dictWeapons = new Dictionary< string, GameObject >();

        // Add stuff to it.
        for ( int i = 0; i < m_rggoWeapons.Length; ++i )
        {
            // Get a handle on the item script
            CProjectile cProjectile = m_rggoWeapons[ i ].GetComponent< CProjectile >();
            if ( null == cProjectile )
            {
                Debug.LogError( string.Format( "{0} {1}: {2}", strFunction, ErrorStrings.ERROR_MISSING_COMPONENT, typeof( CProjectile ).ToString() ) );
                continue;
            }

            string strName = "";

            switch ( cProjectile.ProjectileType )
            {
                case CProjectile.EProjectileType.PROJECTILE_BULLET:

                    strName = Names.NAME_BULLET;

                    break;
                case CProjectile.EProjectileType.PROJECTILE_MISSILE:

                    strName = Names.NAME_MISSILE;

                    break;
                case CProjectile.EProjectileType.PROJECTILE_RAY:

                    strName = Names.NAME_RAY;

                    break;
                default:
                    Debug.LogError( string.Format( "{0} {1}: {2}", strFunction, ErrorStrings.ERROR_UNASSIGNED_TYPE, cProjectile.ProjectileType.ToString() ) );
                    continue;
            }

            // Add weapon to dictionary.
            m_dictWeapons.Add( strName, m_rggoWeapons[ i ] );
        }
    }
}
