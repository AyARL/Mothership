using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mothership;

public class CPowerUp : MonoBehaviour {

    // Added this bit of functionality in case we have enough time to add different
    //  types of powerups.

    public enum EItemType
    {
        TYPE_NONE,
        TYPE_RAYGUN,
    };
    
    public enum EItemAttributes
    {
        ATT_CONSUMABLE,
        ATT_CARRYABLE,
    };

    // The Id of this item, we're going to use this to identify items.
    [ SerializeField ]
    private int m_iItemID;
    public int ItemId { get { return m_iItemID; } }

    // Holds a static list of all active powerups.
    private static List< GameObject > m_liPowerUpList = new List< GameObject >();

    // Holds a list of this item's attributes.
    private List< EItemAttributes > m_liAttributes = new List< EItemAttributes >();
    public List<EItemAttributes> Attributes { get { return m_liAttributes; } }

    // The item's type. For now all powerups will spawn as rayguns
    [ SerializeField ]
    private EItemType m_eItemType = EItemType.TYPE_RAYGUN;
    public EItemType ItemType { get { return m_eItemType; } }

	/////////////////////////////////////////////////////////////////////////////
    /// Function:               Start
    /////////////////////////////////////////////////////////////////////////////
	void Start () 
    {
        // Add self to the static powerup list.
	    m_liPowerUpList.Add( gameObject );

        // By default all items are carryable, so we add the carryable attribute.
        m_liAttributes.Add( EItemAttributes.ATT_CARRYABLE );
	}
	
	/////////////////////////////////////////////////////////////////////////////
    /// Function:               Update
    /////////////////////////////////////////////////////////////////////////////
	void Update () {
	
	}

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               PickupPowerUp
    /////////////////////////////////////////////////////////////////////////////
    public void PickupPowerUp()
    {
        // Remove the powerup from the active list and destroy it.
        m_liPowerUpList.Remove( this.gameObject );
        Destroy( this.gameObject );
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               GetClosestPowerUp
    /////////////////////////////////////////////////////////////////////////////
    public static GameObject GetClosestPowerUp( Transform trCaller )
    {
        // This function will return the closest power up to the caller.
        if ( null == trCaller )
                return null;

        // Declare relevant variables.
        GameObject goClosestPowerUp = null;
        float fLowestDistance = -1;
        float fDistance = -1;

        // Loop through the powerup list and find the closest one to the caller
        //  so we can send it back to him.
        foreach ( GameObject goPowerUp in m_liPowerUpList )
        {
            fDistance = Vector3.Distance( goPowerUp.transform.position, trCaller.position );

            if ( fLowestDistance == -1 )
            {
                fLowestDistance = fDistance;
                goClosestPowerUp = goPowerUp;
            }
            else
            {
                if ( fLowestDistance > fDistance )
                {
                    fLowestDistance = fDistance;
                    goClosestPowerUp = goPowerUp;
                }
            }
        }

        return goClosestPowerUp;
    }
    
    /////////////////////////////////////////////////////////////////////////////
    /// Function:               HasAttribute
    /////////////////////////////////////////////////////////////////////////////
    public bool HasAttribute( EItemAttributes eAtt )
    {
        // Check if this item has the provided attribute and return true if it does.
        if ( true == m_liAttributes.Contains( eAtt ) )
            return true;

        return false;
    }
}
