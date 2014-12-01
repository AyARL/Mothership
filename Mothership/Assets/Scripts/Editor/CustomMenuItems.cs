using UnityEngine;
using UnityEditor;
using System.Collections;
using Mothership;

public static class CustomMenuItems
{
    [ MenuItem( DefaultPaths.SO_RESOURCES_PATH + DefaultPaths.SO_AUDIO ) ]
    public static void CreateAudioSO()
    {
        ScriptableObjectUtility.CreateResource< CAudioSO >( ResourcePacks.RESOURCE_CONTAINER_AUDIO_OBJECTS );
    }
}
