using UnityEngine;
using System.Collections;
using MothershipStateMachine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public class ServerManager : RoleManager
{
    public ServerNetworkManager networkManager = null;

    private List<ClientDataOnServer> registeredClients;
    public IEnumerable<ClientDataOnServer> RegisteredClients { get { return registeredClients; } }

    // States
    public ServerLobbyState ServerLobbyState { get; set; }

    // Events
    public UnityAction OnClientRegistered { get; set; }

    public override void Init(NetworkManager networkManager)
    {
        this.networkManager = networkManager as ServerNetworkManager;

        registeredClients = new List<ClientDataOnServer>();

        // Initialise all states
        ServerLobbyState = new ServerLobbyState(this);
        
        ChangeState(ServerLobbyState);
    }

    public bool RegisterClient(RegisterClient registerMessage)
    {
        if (registeredClients.Count < 8)
        {
            ClientDataOnServer client = new ClientDataOnServer(registerMessage.User, registerMessage.Profile, registerMessage.NetworkPlayer, GetTeamForNextClient());
            registeredClients.Add(client);

            if(OnClientRegistered != null)
            {
                OnClientRegistered();
            }

            return true;
        }
        else
        {
            Debug.LogError("Maximum number of players reached, cannot register");
            return false;
        }
    }

    private IAIBase.ETeam GetTeamForNextClient()
    {
        int redCount = registeredClients.Count(c => c.ClientTeam == IAIBase.ETeam.TEAM_RED);
        int blueCount = registeredClients.Count(c => c.ClientTeam == IAIBase.ETeam.TEAM_BLUE);

        return redCount <= blueCount ? IAIBase.ETeam.TEAM_RED : IAIBase.ETeam.TEAM_BLUE;
    }

    public IEnumerable<ClientDataOnServer> GetTeam(IAIBase.ETeam teamColour)
    {
        return registeredClients.Where(c => c.ClientTeam == teamColour);
    }
}
