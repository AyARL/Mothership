using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System;
using Mothership;

[ RequireComponent( typeof( CStaticCoroutine ) ) ]
public class CAudioControl : MonoBehaviour {

    private static CAudioSO m_AudioResource = null;
    public static CAudioSO AudioResource { get { return m_AudioResource; } set { m_AudioResource = value; } }

    private static List< GameObject > m_liActiveAudioObjects = new List< GameObject >();

    private static Dictionary< string, List< AudioClip > > m_dAudioClipContainer = new Dictionary< string, List< AudioClip > >();

    private static Dictionary< int, float > m_dVolumeContainer = new Dictionary< int, float >();

    private List< string > m_liValidExtensions = new List< string > 
    { 
        Audio.FILE_TYPE_MP3,
        Audio.FILE_TYPE_WAV
    };

    private List< string > m_liRegexPatterns = new List< string > 
    { 
        Audio.AUDIO_EFFECT_GUNSHOT,
        Audio.AUDIO_MUSIC,
        Audio.AUDIO_EFFECT_MENU_SELECT,
        Audio.AUDIO_EFFECT_GAMEOVER,
        Audio.AUDIO_EFFECT_LEVEL_COMPLETED,
        Audio.AUDIO_EFFECT_EXPLOSION,
    };

    private static bool m_bMainMenuMode = false;

    private bool m_bAudioFilesLoaded = false;
    public bool AudioFilesLoaded { get { return m_bAudioFilesLoaded; } private set { m_bAudioFilesLoaded = value; }  }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               Awake
    /////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        // Reload the audio files.
        ReloadSounds();

        // We finished loading the Audio files, set files loaded flag to true.
        AudioFilesLoaded = true;
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               AddClipToDictionary
    /////////////////////////////////////////////////////////////////////////////
    IEnumerator AddClipToDictionary( AudioClip aClip )
    {
        // Error reporting.
        string strFunctionName = "CAudioControl::AddClipToDictionary()";

        // Will be used as a dictionary key if an audio file can be matched to a pattern.
        string strDictionaryKey = "";

        // Yield if the clip isn't ready yet.
        while( false == aClip.isReadyToPlay )
            yield return null;

        // Clip is ready, we need to identify it.
        foreach ( string strPattern in m_liRegexPatterns )
        {
            // Attempt to find the pattern within the clip name.
            Regex regex = new Regex( @strPattern );
            Match match = regex.Match( aClip.name );

            if ( true == match.Success )
            {
                // We found a match, set the pattern which should be unique to each
                //  sound type as the dictionary key.
                strDictionaryKey = strPattern;
                break;
            }
        }

        // Check if we have set a dictionary key,
        if ( true == string.IsNullOrEmpty( strDictionaryKey ) )
        {
            // Report that we couldn't find a pattern match.
            Debug.LogError( string.Format( "{0} {1} " + ErrorStrings.ERROR_UNMATCHED_AUDIO_CLIP, strFunctionName, aClip.name ) );

        }

        // Check if the audio dictionary contains the provided audio name/clip pairing
        //  and add them if it doesn't.
        if ( m_dAudioClipContainer.ContainsKey( strDictionaryKey ) )
        {
            // Add the clip.
            m_dAudioClipContainer[ strDictionaryKey ].Add( aClip );
        }
        else
        {
            // Create clip list.
            List< AudioClip > liClipList = new List< AudioClip >();

            // Add the clip to the clip list.
            liClipList.Add( aClip );

            // Add the list to the dictionary.
            m_dAudioClipContainer.Add( strDictionaryKey, liClipList );
        }
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               CreateAndPlayAudio
    /////////////////////////////////////////////////////////////////////////////
    public static int CreateAndPlayAudio( Vector3 v3Position, string strAudioName, bool bLoop, bool bPlayOnAwake, bool bFadeIn, float fVolume )
    {
        // This function will create a GameObject using the provided parameters and will add an
        //  AudioSource component to it which we will configure to suit our needs. The GameObject
        //  will be destroyed once we're done with it.

        string strFunctionName = "CAudioControl::CreateAndPlayAudio()";

        // Check if we have a collection available for the provided audio name.
        if ( false == m_dAudioClipContainer.ContainsKey( strAudioName ) )
        {
            Debug.LogError( string.Format( "{0} {1} " + ErrorStrings.ERROR_UNRECOGNIZED_NAME, strFunctionName, strAudioName ) );
        }

        // Get a list of audio clips available to us.
        List< AudioClip > liAudioClips = m_dAudioClipContainer[ strAudioName ];

        // Attempt to select a random clip from the list.
        AudioClip aClip = liAudioClips.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

        GameObject goAudioClipObject = new GameObject( strAudioName );

        goAudioClipObject.tag = Tags.TAG_AUDIO;

        // Set the position of the object.
        goAudioClipObject.transform.position = v3Position;

        // Add the Audio Source component
        goAudioClipObject.AddComponent< AudioSource >();

        // Add the Clip Info object.
        goAudioClipObject.AddComponent< CAudioClip >();

        // Retrieve the Audio source component. We will use this reference to set up values, etc.
        AudioSource asSource = goAudioClipObject.GetComponent< AudioSource >();

        // Retrieve the Clip information object.
        CAudioClip cClipInfo = goAudioClipObject.GetComponent< CAudioClip >();

        // Set up the audio source.
        asSource.playOnAwake = bPlayOnAwake;
        asSource.loop = bLoop;
        asSource.clip = aClip;
        asSource.volume = 0f;

        // Check if we want to fade in the sound.
        if ( true == bFadeIn )
        {
             CStaticCoroutine.DoCoroutine( FadeIn( asSource, fVolume ) );
        }
        else
        { 
            asSource.volume = fVolume;
        }

        // Play the clip and destroy the game object once we're done with it.
        asSource.Play();

        if ( false == bLoop )
        {
            Destroy( goAudioClipObject, aClip.length );
        }
        else
        {
            m_liActiveAudioObjects.Add( goAudioClipObject );
            return cClipInfo.ClipId;
        }

        // Return an invalid id.
        return -1;
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               ReloadSounds
    /////////////////////////////////////////////////////////////////////////////
    private void ReloadSounds()
    {
        string strFunctionName = "CAudioControl::ReloadSounds()";

        // This function will clear the existing dictionary and reload all sounds.
        m_dAudioClipContainer.Clear();

        if ( null == m_AudioResource )
        {
            m_AudioResource = Resources.Load< CAudioSO >( ResourcePacks.RESOURCE_CONTAINER_AUDIO_OBJECTS );
            if ( null == m_AudioResource )
            {
                Debug.LogError( string.Format("{0} {1} " + ErrorStrings.ERROR_AUDIO_FAILED_RELOAD, strFunctionName, ResourcePacks.RESOURCE_CONTAINER_AUDIO_OBJECTS ) );
                return;
            }
        }

        // Loop through available clips and add them to the dictionary.
        foreach ( AudioClip aClip in m_AudioResource.AudioObjects )
        {
            StartCoroutine( AddClipToDictionary( aClip ) );
        }

        // Get a handle on the Audio directory.
        //DirectoryInfo directoryInfo = new DirectoryInfo( CAudio.PATH_AUDIO );

        // Holy crap I'm using a lambda. P.S. We're verifying directory contents for valid audio files.
        //FileInfo[] rgFileInfo = directoryInfo.GetFiles().Where( x => IsValidAudioType( Path.GetExtension( x.Name ) ) ).ToArray();
        
        // Loop through the files we found and attempt to load them into the dictionary.
        //foreach ( FileInfo fileInfo in rgFileInfo )
        //{
            // Attempt to load the clip to dictionary.
        //    StartCoroutine( AddClipToDictionary( fileInfo.FullName ) );
        //}

    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               IsValidAudioType
    /////////////////////////////////////////////////////////////////////////////
    private bool IsValidAudioType( string strFile )
    {
        // Will verify if the file provided has the correct extension.
        bool bIsValidAudioType = false;

        if ( true == m_liValidExtensions.Contains( strFile ) )
            bIsValidAudioType = true;

        return bIsValidAudioType;
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               SoundIsPlaying
    /////////////////////////////////////////////////////////////////////////////
    public static List< GameObject > SoundIsPlaying( string strAudioName )
    {
        // Check if there is a spawned object playing the provided audio.
        GameObject[] rggoAudioObjects = GameObject.FindGameObjectsWithTag( Tags.TAG_AUDIO );

        // Return all game objects of interest.
        List< GameObject > liObjectsOfInterest = rggoAudioObjects.Where( x => x.name == strAudioName ).ToList();
        
        return liObjectsOfInterest;
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               StopSound
    /////////////////////////////////////////////////////////////////////////////
    public static void StopSound( int iSourceID, bool bFadeOut = true )
    {
        // The below GameObject will contain the Audio Object.
        GameObject goAudioObject = null;

        // Loop through all active audio objects and retrieve the object that
        //  matches the provided id.
        foreach ( GameObject goAudio in m_liActiveAudioObjects )
        {
            if ( goAudio == null )
            {
                m_liActiveAudioObjects.Remove( goAudio );
                return;
            }

            // Attempt to retrieve the audio clip object.
            CAudioClip aClip = goAudio.GetComponent< CAudioClip >();
            if ( aClip == null )
                return;

            if ( aClip.ClipId == iSourceID )
            {
                // We found a match.
                goAudioObject = goAudio;
            }
        }

        // Check if we managed to find the gameobject.
        if ( goAudioObject == null )
            return;

        // Retrieve the clip information object.
        CAudioClip cClipInfo = goAudioObject.GetComponent< CAudioClip >();

        // Check if it's marked for deletion.
        if ( true == cClipInfo.MarkedForDestruction )
        {
            // Take out the object and try stopping the sound again.
            m_liActiveAudioObjects.Remove( goAudioObject );

            StopSound( iSourceID, bFadeOut );

            // No point in going forward, we can exit the function.
            return;
        }
        else
        {
            // Take out the object and try stopping the sound again.
            m_liActiveAudioObjects.Remove( goAudioObject );

            // Ensure we don't reprocess this clip.
            cClipInfo.MarkedForDestruction = true;
        }
        
        // Retrieve the audio source for the fadeout functionality.
        AudioSource asSource = goAudioObject.GetComponent< AudioSource >();

        if ( true == bFadeOut )
        { 
            CStaticCoroutine.DoCoroutine( FadeOut( asSource ) );
        }
        else
        {
            asSource.volume = 0;
        }
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               FadeIn
    /////////////////////////////////////////////////////////////////////////////
    static IEnumerator FadeIn( AudioSource asSource, float fVol )
    {
        // Slowly raise the volume.
        while ( asSource != null && asSource.volume < fVol )
        {
            asSource.volume += Audio.AUDIO_FADE_VARIABLE * Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               FadeOut
    /////////////////////////////////////////////////////////////////////////////
    static IEnumerator FadeOut( AudioSource asSource )
    {
        // Reduce volume to create a fade out effect.
        while ( asSource != null && asSource.volume > 0 )
        {
            asSource.volume -= Audio.AUDIO_FADE_VARIABLE * Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               ClearContainers
    /////////////////////////////////////////////////////////////////////////////
    public static void ClearContainers()
    {
        m_liActiveAudioObjects.Clear();
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               SetVolume
    /////////////////////////////////////////////////////////////////////////////
    public static void SetVolume( int iAudioID, float fVol )
    {
        // The below GameObject will contain the Audio Object.
        GameObject goAudioObject = null;

        // Loop through all active audio objects and retrieve the object that
        //  matches the provided id.
        foreach ( GameObject goAudio in m_liActiveAudioObjects )
        {
            if ( goAudio == null )
            {
                m_liActiveAudioObjects.Remove( goAudio );
                return;
            }

            // Attempt to retrieve the audio clip object.
            CAudioClip aClip = goAudio.GetComponent< CAudioClip >();
            if ( aClip == null )
                return;

            if ( aClip.ClipId == iAudioID )
            {
                // We found a match.
                goAudioObject = goAudio;
            }
        }

        // Check if we managed to find the gameobject.
        if ( goAudioObject == null )
            return;

        // Retrieve the audio source component.
        AudioSource asAudio = goAudioObject.GetComponent< AudioSource >();
        if ( null == asAudio )
            return;

        // Set the new volume
        asAudio.volume = fVol;
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               SetVolume
    /////////////////////////////////////////////////////////////////////////////
    public static void SetVolume( string strAudio, float fVol )
    {
        // The below GameObject will contain the Audio Object.
        GameObject goAudioObject = null;

        // Loop through all active audio objects and retrieve the object that
        //  matches the provided id.
        foreach ( GameObject goAudio in m_liActiveAudioObjects )
        {
            if ( goAudio == null )
            {
                m_liActiveAudioObjects.Remove( goAudio );
                return;
            }

            // Check if we find a match.
            if ( goAudio.name == strAudio )
            {
                goAudioObject = goAudio;
            }
        }

        // Check if we managed to find the gameobject.
        if ( goAudioObject == null )
            return;

        // Retrieve the audio source component.
        AudioSource asAudio = goAudioObject.GetComponent< AudioSource >();
        if ( null == asAudio )
            return;

        // Set the new volume
        asAudio.volume = fVol;
    }

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               ToggleMainMenuMode
    /////////////////////////////////////////////////////////////////////////////
    public static void ToggleMainMenuMode()
    {
        // Toggle the main menu mode flag.
        m_bMainMenuMode = !m_bMainMenuMode;

        // Application expects us to be in main menu mode, we need to lower the volume.
        //  and store the initial volume in a dictionary.
        if ( true == m_bMainMenuMode )
        {
            foreach ( GameObject goAudio in m_liActiveAudioObjects )
            {
                if ( goAudio == null )
                {
                    m_liActiveAudioObjects.Remove( goAudio );
                    return;
                }

                // Get handle on audio clip component.
                CAudioClip aClip = goAudio.GetComponent< CAudioClip >();
                if ( null == aClip )
                    return;

                // Get the clip ID;
                int iAudioID = aClip.ClipId;

                // Get handle on audio source.
                AudioSource asAudio = goAudio.GetComponent< AudioSource >();
                if ( null == asAudio )
                    return;

                // Keep track of the initial volume.
                m_dVolumeContainer.Add( iAudioID, asAudio.volume );

                // Lower volume.
                asAudio.volume = 0.01f;
            }
        }
        else
        {
            // Return normal volume to all sounds.
            foreach ( GameObject goAudio in m_liActiveAudioObjects )
            {
                if ( goAudio == null )
                {
                    m_liActiveAudioObjects.Remove( goAudio );
                    return;
                }

                // Get handle on audio clip component.
                CAudioClip aClip = goAudio.GetComponent< CAudioClip >();
                if ( null == aClip )
                    return;

                float fVolume = 0;

                // Check if the volume container contains the clip key.
                if ( null != aClip && true == m_dVolumeContainer.ContainsKey( aClip.ClipId ) )
                {
                    // Get the initial volume for this audio object.
                    fVolume = m_dVolumeContainer[ aClip.ClipId ];
                }

                // Get handle on audio source.
                AudioSource asAudio = goAudio.GetComponent< AudioSource >();
                if ( null == asAudio )
                    return;

                // Turn volume back up.
                asAudio.volume = fVolume;
            }

            // Clear out the volume container
            m_dVolumeContainer.Clear();
        }
    }
}
