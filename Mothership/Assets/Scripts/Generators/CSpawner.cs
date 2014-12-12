using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mothership;

[ RequireComponent( typeof( NetworkView ) ) ]
public class CSpawner : MonoBehaviour {

    public enum ESpawnerType
    {
        TYPE_NONE,
        TYPE_NPC,
        TYPE_POWERUPS,
        TYPE_FLAG,
    }

    // Will hold a reference to all available spawnpoints.
    private static List< GameObject > m_liSpawnPoints = new List< GameObject >();
    public static List< GameObject > SpawnPoints { get { return m_liSpawnPoints; }  }

    // Reference to the PowerUp resource.
    private static CPowerUpSO m_ItemsResource = null;
    public static CPowerUpSO ItemsResource { get { return m_ItemsResource; } set { m_ItemsResource = value; } }

    // Reference to the npc resource.
    private static CNPCSO m_NPCResource = null;
    public static CNPCSO NPCResource { get { return m_NPCResource; } set { m_NPCResource = value; } }

    [ SerializeField ]
    private static GameObject m_goFlagPrefab = null;
    public static GameObject FlagPrefab { get { return m_goFlagPrefab; } }

    // We're going to use different spawn logic depending on the type of the spawner.
    [ SerializeField ]
    private ESpawnerType m_eSpawnerType = ESpawnerType.TYPE_NONE;
    public ESpawnerType SpawnerType { get { return m_eSpawnerType; } }

    // Dictates if this spawner belongs to a specific team.
    [ SerializeField ]
    private IAIBase.ETeam m_eTeam = IAIBase.ETeam.TEAM_NONE;
    public IAIBase.ETeam Team { get { return m_eTeam; } }

    // Will indicate if the spawner can spawn.
    [ SerializeField ]
    private bool m_bCanSpawn = true;
    public bool CanSpawn { get { return m_bCanSpawn; } }

    private bool IsRunningLocally { get { return !Network.isClient && !Network.isServer; } }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               Spawn
    /////////////////////////////////////////////////////////////////////////////
	private void Spawn ( GameObject goObject ) 
    {
        GameObject goInstantiatedObject = ( GameObject )Network.Instantiate( goObject, transform.position, Quaternion.identity, 0 );
        goInstantiatedObject.name = goObject.name;
	    StartCoroutine( InitiateSpawnCountDown() );
	}

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               InitiateSpawnCountDown
    /////////////////////////////////////////////////////////////////////////////
    private IEnumerator InitiateSpawnCountDown()
    {
        m_bCanSpawn = false;
        yield return new WaitForSeconds( 5 );
        m_bCanSpawn = true;
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               Awake
    /////////////////////////////////////////////////////////////////////////////
    void Awake ()
    {
        m_liSpawnPoints.Add( gameObject );
    }

	/////////////////////////////////////////////////////////////////////////////
    /// Function:               Update
    /////////////////////////////////////////////////////////////////////////////
	void Update () 
    {
        string strFunction = "CSpawner::Update()";

        if ( null == m_ItemsResource )
        {
            m_ItemsResource = Resources.Load< CPowerUpSO >( ResourcePacks.RESOURCE_CONTAINER_ITEMS );
            if ( null == m_ItemsResource )
            {
                Debug.LogError( string.Format("{0} {1}: {2}", strFunction, ErrorStrings.ERROR_CANNOT_LOAD_RESOURCE, ResourcePacks.RESOURCE_CONTAINER_ITEMS ) );
                return;
            }
        }

        if ( null == m_NPCResource )
        {
            m_NPCResource = Resources.Load< CNPCSO >( ResourcePacks.RESOURCE_CONTAINER_NPCS );
            if ( null == m_ItemsResource )
            {
                Debug.LogError( string.Format("{0} {1}: {2}", strFunction, ErrorStrings.ERROR_CANNOT_LOAD_RESOURCE, ResourcePacks.RESOURCE_CONTAINER_NPCS ) );
                return;
            }
        }

        if ( null == m_goFlagPrefab )
        {
            m_goFlagPrefab = m_NPCResource.Flag;
            if ( null == m_goFlagPrefab )
            {
                Debug.LogError( string.Format("{0} {1}: {2}", strFunction, ErrorStrings.ERROR_NULL_OBJECT, typeof( GameObject ).ToString() ) );
            }
        }
	}

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               SpawnNPC
    /////////////////////////////////////////////////////////////////////////////
    public static void SpawnNPC( IAIBase.ETeam eTeam, IAIBase.ENPCType eType )
    {
        foreach ( GameObject goObject in m_liSpawnPoints )
        {
            CSpawner cSpawner = goObject.GetComponent< CSpawner >();
            
            if ( cSpawner.SpawnerType != ESpawnerType.TYPE_NPC )
                continue;
   
            if ( cSpawner.Team == eTeam && true == cSpawner.CanSpawn )
            {
                cSpawner.Spawn( NPCResource.GetObjectByType( eTeam, eType ) );
                break;
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               SpawnPowerUp
    /////////////////////////////////////////////////////////////////////////////
    public static void SpawnPowerUp( CPowerUp.EItemType eType )
    {
        foreach ( GameObject goObject in m_liSpawnPoints )
        {
            CSpawner cSpawner = goObject.GetComponent< CSpawner >();
            
            if ( cSpawner.SpawnerType != ESpawnerType.TYPE_POWERUPS )
                continue;

            if ( cSpawner.Team == IAIBase.ETeam.TEAM_NONE && true == cSpawner.CanSpawn )
            {
                List< GameObject > liPowerUps = ItemsResource.Items;
                foreach ( GameObject goPowerUp in liPowerUps )
                {
                    CPowerUp cPowerUp = goPowerUp.GetComponent< CPowerUp >();
                    if ( cPowerUp.ItemType == eType )
                    {
                        cSpawner.Spawn( goPowerUp );
                    }
                }                
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               SpawnFlag
    /////////////////////////////////////////////////////////////////////////////
    public static void SpawnFlag()
    {
        List< GameObject > liSpawnLocations = new List< GameObject >(); 
        foreach ( GameObject goObject in m_liSpawnPoints )
        {
            CSpawner cSpawner = goObject.GetComponent< CSpawner >();
            
            if ( cSpawner.SpawnerType != ESpawnerType.TYPE_FLAG )
                continue;

            liSpawnLocations.Add( goObject );
        }

        CSpawner cSpawn = liSpawnLocations[ Random.Range( 0, liSpawnLocations.Count - 1 ) ].GetComponent< CSpawner >();
        cSpawn.Spawn( m_goFlagPrefab );
    }
}
