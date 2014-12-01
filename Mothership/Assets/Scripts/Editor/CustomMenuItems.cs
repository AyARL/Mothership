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

    [ MenuItem( DefaultPaths.SO_RESOURCES_PATH + DefaultPaths.SO_NPC ) ]
    public static void CreateNPCSO()
    {
        ScriptableObjectUtility.CreateResource< CNPCSO >( ResourcePacks.RESOURCE_CONTAINER_NPCS );
    }

    [ MenuItem( DefaultPaths.SO_RESOURCES_PATH + DefaultPaths.SO_ITEMS ) ]
    public static void CreateItemsSO()
    {
        ScriptableObjectUtility.CreateResource< CPowerUpSO >( ResourcePacks.RESOURCE_CONTAINER_ITEMS );
    }
}
