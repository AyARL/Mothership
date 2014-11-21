using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public abstract class NetworkManager : MonoBehaviour
{
    protected const string gameTypeName = "Mothership";
    protected const string roleManagerObjectName = "RoleManager";
}
