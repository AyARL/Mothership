using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CAudioSO : ScriptableObject
{
    [ SerializeField ]
    private AudioClip[] m_rggoAudioObjects;

    List< AudioClip > m_liAudioObjects;
    public List< AudioClip > AudioObjects 
    { 
        get 
        { 
            UpdateAudioObjects(); 
            return m_liAudioObjects; 
        } 
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               UpdateAudioObjects
    /////////////////////////////////////////////////////////////////////////////
    public void UpdateAudioObjects()
    {
        // Clear the audio objects list.
        m_liAudioObjects = new List< AudioClip >();

        // Add stuff to it.
        for ( int i = 0; i < m_rggoAudioObjects.Length; ++i )
        {
            m_liAudioObjects.Add( m_rggoAudioObjects[ i ] );
        }
    }
}
