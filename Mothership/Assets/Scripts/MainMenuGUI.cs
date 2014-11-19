using UnityEngine;
using System.Collections;

public class MainMenuGUI : MonoBehaviour
{
    public void StartNewServer()
    {
        GameObject managerObj = GameObject.FindGameObjectWithTag("NetworkManager");
        ServerNetworkManager networkManager = managerObj.AddComponent<ServerNetworkManager>();
        networkManager.StartServer();
    }

    public void GetServerList()
    {

    }
}
