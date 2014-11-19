using UnityEngine;
using System.Collections;

public class ServerNetworkManager : NetworkManager
{
    //TODO: Make this customizable when creating server
    private const string gameName = "Mothership001";
    public string GameName { get { return gameName; } }
    private const string gameDesc = "Test Mothership Server";
    public string GameDesc { get { return gameDesc; } }

    private ServerManager ServerManager { get; set; }

    // Used for level loading to prevent message leaking
    public int LastLevelPrefix { get; private set; }

    public System.Action<bool> OnServerReady { get; set; }

    public void StartServer()
    {
        if (!Network.isServer && !Network.isClient)
        {
            NetworkConnectionError error = Network.InitializeServer(5, 25000, !Network.HavePublicAddress());
            if (error == NetworkConnectionError.NoError)
            {
                MasterServer.RegisterHost(gameTypeName, gameName, gameDesc);
            }
            else
            {
                Debug.Log("Failed to initialize server: " + error.ToString());
            }
        }
    }

    private void OnServerInitialized()
    {
        Debug.Log("Server Ready - waiting for registraion");
        InitialiseRoleManager();
    }

    private void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        Debug.Log("Master Server Event: " + msEvent.ToString());

        switch (msEvent)
        {
            case MasterServerEvent.RegistrationSucceeded:
                
                break;
        }

    }

    private void OnFailedToConnectToMasterServer(NetworkConnectionError info)
    {
        if (OnServerReady != null)
        {
            OnServerReady(false);
        }
    }

    private void InitialiseRoleManager()
    {
        GameObject roleManagerObj = new GameObject(roleManagerObjectName);
        ServerManager = roleManagerObj.AddComponent<ServerManager>();
        //ServerManager.Init(this);
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
