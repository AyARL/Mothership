using UnityEngine;
using System.Collections;
using MothershipStateMachine;
using UnityEngine.Events;

public class ClientManager : RoleManager
{
    public ClientNetworkManager NetworkManager { get; private set; }

    // States
    public ClientLobbyState ClientLobbyState { get; private set; }

    // Events
    public UnityAction<TeamList, TeamList> OnUpdateTeamRoster { get; set; }

    public override void Init(NetworkManager networkManager)
    {
        NetworkManager = networkManager as ClientNetworkManager;

        ClientLobbyState = new ClientLobbyState(this);

        activeState = ClientLobbyState;
    }

}
