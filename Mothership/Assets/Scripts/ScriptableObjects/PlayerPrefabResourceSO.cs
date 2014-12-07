using UnityEngine;
using System.Collections;

public class PlayerPrefabResourceSO : ScriptableObject
{
    [SerializeField]
    private PlayerController redDrone = null;
    public PlayerController RedDrone { get { return redDrone; } }

    [SerializeField]
    private PlayerController blueDrone = null;
    public PlayerController BlueDrone { get { return blueDrone; } }
}
