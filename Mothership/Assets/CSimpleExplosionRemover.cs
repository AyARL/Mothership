using UnityEngine;
using System.Collections;
using Mothership;

public class CSimpleExplosionRemover : MonoBehaviour 
{
    private const float ONE_SECOND = 1f;
    private float m_fInstantiationTime = 0;
    private bool IsRunningLocally { get { return !Network.isClient && !Network.isServer; } }

	// Use this for initialization
	void Start () 
    {
        m_fInstantiationTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if ( m_fInstantiationTime <= Time.time - ONE_SECOND )
        { 
            if ( !IsRunningLocally )
            {
                networkView.RPC( RPCFunctions.RPC_DESTROY_EXPLOSION, RPCMode.All );
            }
            else
            {
                RPCDestroyExplosion();
            }
        }
	}

    [RPC]
    private void RPCDestroyExplosion()
    {
        Destroy( gameObject );
    }
}
