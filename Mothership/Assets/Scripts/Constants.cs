﻿using UnityEngine;
using System.Collections;

namespace Mothership
{ 
    public class Constants 
    {
        public const string GAME_MOTHERSHIP = "MotherShip";

        // NPC related constants.
        public const float DEFAULT_SPEED_TANK = 5f;
        public const float DEFAULT_SPEED_REPAIRBOT = 10f;
        public const float DEFAULT_SPEED_WARRIOR = 15.0f;
        public const float DEFAULT_SPEED_DRONE = 20.0f;

        public const float DEFAULT_HEALTH_REPAIRBOT = 50f;
        public const float DEFAULT_HEALTH_DRONE = 100f;
        public const float DEFAULT_HEALTH_WARRIOR = 200f;
        public const float DEFAULT_HEALTH_TANK = 400f;
        
        public const float DEFAULT_ATTACK_RANGE = 40f;
        public const float DEFAULT_MAX_PROJECTILE_RANGE = 50f;

        // Weapons related constants.
        public const float PROJECTILE_DAMAGE_BULLET = 5f;
        public const float PROJECTILE_DAMAGE_MISSILE = 25f;
        public const float PROJECTILE_DAMAGE_RAY = 100f;

        public const float PROJECTILE_SPEED_BULLET = 20f;
        public const float PROJECTILE_SPEED_MISSILE = 15f;
        public const float PROJECTILE_SPEED_RAY = 30f;

        public const float PROJECTILE_DELAY_BULLET = 0.5f;
        public const float PROJECTILE_DELAY_MISSILE = 2f;
        public const float PROJECTILE_DELAY_RAY = 5f;
    }

    public class DefaultPaths
    {
        // Editor Custom Menu Items paths.
        // Will be used by the custom menu items script for resource creation.
        public const string SO_RESOURCES_PATH = "MotherShip/Resources/";
        public const string SO_AUDIO = "Audio";
        public const string SO_NPC = "NPCs";
        public const string SO_ITEMS = "Items";
        
        // Asset paths
        public const string PATH_RESOURCES = "Resources/";

        // Default audio path
        public const string PATH_AUDIO = "./Assets/Audio";
    }

    public class AnimatorValues
    {
        // Animator float names.

        // Animator trigger names.

        // Animator boolean names.
        public const string ANIMATOR_IS_MOVING = "bIsMoving";

    }

    public class Controls
    {
        // Control related constants
        public const string CONTROL_AXIS_HORIZONTAL = "Horizontal";
        public const string CONTROL_AXIS_VERTICAL = "Vertical";
        public const string CONTROL_MOUSE_X = "Mouse X";
        public const string CONTROL_MOUSE_Y = "Mouse Y";

        public const int CONTROL_MOUSE_LEFT_BUTTON = 0;
        public const int CONTROL_MOUSE_RIGHT_BUTTON = 1;
    }

    public class Audio
    {
        // Default regex patterns.
        public const string AUDIO_MUSIC = "Music";
        public const string AUDIO_EFFECT_GAMEOVER = "LevelFailed";
        public const string AUDIO_EFFECT_MENU_SELECT = "Menu_Select";
        public const string AUDIO_EFFECT_LEVEL_COMPLETED = "LevelCompleted";

        // Valid file extensions.
        public const string FILE_TYPE_MP3 = ".mp3";
        public const string FILE_TYPE_WAV = ".wav";

        // Audio altering variables.
        public const float AUDIO_FADE_VARIABLE = 0.3f;
    }

    public class Tags
    {
        // List of commonly used tags.
        public const string TAG_PLAYER = "Player";
        public const string TAG_WALL = "Wall";
        public const string TAG_AUDIO_CONTROLLER = "AudioController";
        public const string TAG_AUDIO = "Audio";
        public const string TAG_NODE = "Node";
        public const string TAG_POWERUP = "PowerUp";
        public const string TAG_WEAPON = "Weapon";
        public const string TAG_BASE = "Base";
    }

    public class Names
    {
        // List of commonly used names.
        public const string NAME_NODE = "Node";
        public const string NAME_RED_BASE = "RedBase";
        public const string NAME_BLUE_BASE = "BlueBase";
        public const string NAME_BULLET = "Bullet";
        public const string NAME_MISSILE = "Missile";
        public const string NAME_RAY = "Ray";
        public const string NAME_POWER_UP = "PowerUp";
        public const string NAME_HEALTH = "Health";
        public const string NAME_MOTHERSHIP_RED = "MothershipRed";
        public const string NAME_MOTHERSHIP_BLUE = "MothershipBlue";
    }

    public class ErrorStrings
    {
        // Declare error messages.
        public const string ERROR_UNHANDLED_DEVICE = "is an unsupported device type";
        public const string ERROR_UNRECOGNIZED_VALUE = "Provided enum value is unrecognized";
        public const string ERROR_NULL_OBJECT = "Failed to find object";
        public const string ERROR_MISSING_COMPONENT = "Missing component";
        public const string ERROR_UNRECOGNIZED_NAME = "Provided name is not handled by current function.";
        public const string ERROR_UNMATCHED_AUDIO_CLIP = "Unable to match provided audio file to available patterns.";
        public const string ERROR_AUDIO_FILES_NOT_LOADED = "Audio Controller has indicated that it hasn't finished loading all audio files.";
        public const string ERROR_AUDIO_FAILED_RELOAD = "Could not load audio resources.";

        // Path finding errors.
        public const string ERROR_PATHFINDING_NO_VALID_PATH = "Could not find a valid path to the target.";

        // AI Errors
        public const string ERROR_UNASSIGNED_NPC = "Unassigned NPC detected, please assign NPC to a team.";

        // Projectile Errors.
        public const string ERROR_UNASSIGNED_TYPE = "Projectile type is unassigned.";
    }

    public class ResourcePacks
    {
        public const string RESOURCE_CONTAINER_AUDIO_OBJECTS = "AudioResource";
        public const string RESOURCE_CONTAINER_NPCS = "NPCResource";
        public const string RESOURCE_CONTAINER_ITEMS = "ItemsResource";
    }

    public class PowerUpIDs
    {
        // Powerup/Item IDs.
        public const uint ID_RAYGUN = 1;
    }
}