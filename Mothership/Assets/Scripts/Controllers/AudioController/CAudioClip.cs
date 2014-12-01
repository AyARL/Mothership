using UnityEngine;
using System.Collections;

public class CAudioClip : MonoBehaviour {

    private static int m_iAvailableClipID = 0;

    [ SerializeField ]
    private int m_iClipID = ++m_iAvailableClipID;
    public int ClipId { get { return m_iClipID; } private set { m_iClipID =value; } }

    private bool m_bMarkedForDestruction = false;
    public bool MarkedForDestruction { get { return m_bMarkedForDestruction; } set { m_bMarkedForDestruction = value; } }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               Update
    /////////////////////////////////////////////////////////////////////////////
    void Update()
    {
        // The script will destroy its gameobject once the AudioSource volume reaches 0;
        if ( gameObject.GetComponent< AudioSource >().volume <= 0 )
        {
            Destroy( gameObject );
        }
    }
}
