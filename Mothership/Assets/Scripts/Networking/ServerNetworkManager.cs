using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using MothershipUtility;
using MothershipStateMachine;
using System.Linq;
using MothershipOS;

namespace Mothership
{
    public class ServerNetworkManager : NetworkManager
    {
        // Going to use the below to teamlist variables to figure out how many AI NPCs are needed.
        private TeamList RedTeam { get; set; }
        private TeamList BlueTeam { get; set; }
        private string GameName { get; set; }
        private string GameDescription { get; set; }

        public UnityAction OnServerReady { get; set; }

        public void StartServer(string gameName, string gameDescription)
        {
            if (!Network.isServer && !Network.isClient)
            {
                GameName = gameName;
                GameDescription = gameDescription;
                NetworkConnectionError error = Network.InitializeServer(8, 25000, !Network.HavePublicAddress());
                if (error == NetworkConnectionError.NoError)
                {
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
                    if (serverManager == null)
                    {
                        InitialiseRoleManager();
                    }
                    break;
            }

        }

        private void OnFailedToConnectToMasterServer(NetworkConnectionError info)
        {
            Debug.LogError(info.ToString());
        }

        private void InitialiseRoleManager()
        {
            GameObject roleManagerObj = new GameObject(roleManagerObjectName);
            serverManager = roleManagerObj.AddComponent<ServerManager>();
            serverManager.Init(this);

            if (OnServerReady != null)
            {
                OnServerReady();
            }
        }

        private void OnPlayerConnected(NetworkPlayer newPlayer)
        {
            Debug.Log("Player Connected");
            // Use for reinstating disconnected players
        }

        private void OnPlayerDisconnected(NetworkPlayer player)
        {
            serverManager.SendGameMessage(new ClientDisconnected() { NetworkPlayer = player });
        }

        public void SendClientRegistration(NetworkPlayer player, IAIBase.ETeam team, int teamOrder)
        {
            int teamInt = (int)team;
            networkView.RPC("RPCSendClientRegistrationData", player, teamInt, teamOrder);
        }

        public void SendTeamSetupUpdate(TeamList redTeam, TeamList blueTeam)
        {
            string redTeamString = JsonUtility.SerializeToJson<TeamList>(redTeam);
            string blueTeamString = JsonUtility.SerializeToJson<TeamList>(blueTeam);

            RedTeam = redTeam;
            BlueTeam = blueTeam;

            networkView.RPC("RPCSendTeamData", RPCMode.Others, redTeamString, blueTeamString);
        }

        public void StartMatch()
        {
            LastLevelPrefix += 1;
            Network.RemoveRPCsInGroup(networkView.group);
            networkView.RPC("RPCLoadLevel", RPCMode.AllBuffered, 1, LastLevelPrefix);
        }

        private void PreventFurtherConnections()
        {
            MasterServer.UnregisterHost(); // don't advertise on Master Server
            Network.maxConnections = -1; // allow connections equal to current count
        }

        public void StartMatchCountdown()
        {
            networkView.RPC("RPCMatchCountdown", RPCMode.Others);
        }

        public void GamePlayStarted()
        {
            networkView.RPC("RPCGamePlayStarted", RPCMode.Others);
            // Spawn the flag.
            CSpawner.SpawnFlag();

            // Get the number of required red AI characters.
            int iNumberOfRequiredNPCs = Constants.GAME_MAX_PLAYERS_PER_TEAM - RedTeam.TeamDisplayNames.Length;
            for ( int i = 0; i < iNumberOfRequiredNPCs; ++i )
            {
                CSpawner.SpawnNPC( IAIBase.ETeam.TEAM_RED, IAIBase.ENPCType.TYPE_DRONE );
            }

            // Get the number of required blue AI characters.
            iNumberOfRequiredNPCs = Constants.GAME_MAX_PLAYERS_PER_TEAM - BlueTeam.TeamDisplayNames.Length;
            for ( int i = 0; i < iNumberOfRequiredNPCs; ++i )
            {
                CSpawner.SpawnNPC( IAIBase.ETeam.TEAM_BLUE, IAIBase.ENPCType.TYPE_DRONE );
            }    
        }

        public void MatchExpired()
        {
            networkView.RPC("RPCMatchExpired", RPCMode.Others);
        }

        public void UpdateScore( int iRedScore, int iBlueScore )
        {
            networkView.RPC( RPCFunctions.RPC_UPDATE_SCORE , RPCMode.Others, iRedScore, iBlueScore );
        }

        public void UpdateClientStats( ClientDataOnServer clientData )
        {
            networkView.RPC( RPCFunctions.RPC_UPDATE_CLIENT_STATS , clientData.NetworkPlayer, clientData );
        }

        public void ForwardMessage( GameMessage cMessage )
        {
            string strSerializedMessage = JsonUtility.SerializeToJson<GameMessage>(UserDataManager.userData.Message);
            networkView.RPC( RPCFunctions.RPC_FORWARD_TO_CLIENTS, RPCMode.Others, strSerializedMessage );
        }

    }
    
}