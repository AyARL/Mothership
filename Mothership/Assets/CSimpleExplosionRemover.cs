using UnityEngine;
using System.Collections;
using Mothership;

public class CSimpleExplosionRemover : MonoBehaviour 
{
    private const float ONE_SECOND = 1f;
    private float m_fInstantiationTime = 0;

	// Use this for initialization
	void Start () 
    {
        CAudioControl.CreateAndPlayAudio( transform.position, Audio.AUDIO_EFFECT_EXPLOSION, false, true, false, 1f );
        m_fInstantiationTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if ( m_fInstantiationTime <= Time.time - ONE_SECOND )
        { 
            if ( Network.isServer )
                Network.Destroy( gameObject );
        }
	}
}
