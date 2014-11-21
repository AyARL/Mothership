using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class ServerNetworkManager : NetworkManager
{
    private string GameName { get; set; }
    private string GameDescription { get; set; }

    private ServerManager ServerManager { get; set; }

    // Used for level loading to prevent message leaking
    public int LastLevelPrefix { get; private set; }

    public UnityAction OnServerReady { get; set; }

    public void StartServer(string gameName, string gameDescription)
    {
        if (!Network.isServer && !Network.isClient)
        {
            GameName = gameName;
            GameDescription = gameDescription;
            NetworkConnectionError error = Network.InitializeServer(5, 25000, !Network.HavePublicAddress());
            if (error == NetworkConnectionError.NoError)
            {
                Network.maxConnections = 8;
                MasterServer.RegisterHost(gameTypeName, GameName, GameDescription);
            }
            else
            {
                Debug.Log("Failed to initialize server: " + error.ToString());
            }
        }
    }


    private void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        Debug.Log("Master Server Event: " + msEvent.ToString());

        switch (msEvent)
        {
            case MasterServerEvent.RegistrationSucceeded:
                InitialiseRoleManager();
                break;
        }

    }

    private void OnFailedToConnectToMasterServer(NetworkConnectionError info)
    {
        
    }

    private void InitialiseRoleManager()
    {
        GameObject roleManagerObj = new GameObject(roleManagerObjectName);
        ServerManager = roleManagerObj.AddComponent<ServerManager>();
        //ServerManager.Init(this);

        if(OnServerReady != null)
        {
            OnServerReady();
        }
    }

    private void OnPlayerConnected(NetworkPlayer newPlayer)
    {
        Debug.Log("Player Connected");
    }

    private void OnPlayerDisconnected(NetworkPlayer player)
    {
        
    }

    public void PreventFurtherConnections()
    {
        MasterServer.UnregisterHost(); // don't advertise on Master Server
        Network.maxConnections = Network.connections.Length; // allow connections equal to current count
    }
}
