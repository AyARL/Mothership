using UnityEngine;
using System.Collections;
using MothershipStateMachine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

namespace Mothership
{
    public class ClientManager : RoleManager
    {
        public ClientNetworkManager NetworkManager { get; private set; }

        // States
        public ClientLobbyState ClientLobbyState { get; private set; }
        public ClientGameSetupState ClientGameSetupState { get; private set; }
        public ClientGamePlayState ClientGamePlayState { get; private set; }
        public ClientGameEndState ClientGameEndState { get; private set; }

        public IAIBase.ETeam team = IAIBase.ETeam.TEAM_NONE;
        public int teamOrder = -1;

        private PlayerPrefabResourceSO prefabResource = null;
        public ClientStats ClientStats { get; private set; }
        public PlayerController PlayerController { get; private set; }

        public TeamList[] TeamRoster { get; private set; }

        // Events
        public UnityAction<TeamList, TeamList> OnUpdateTeamRoster { get; set; } // Called when updated team list arives from server
        public UnityAction<float> OnMatchCountdownStarted { get; set; } // Passes the time value for network latency, so a timer can be started with an adjusted value
        public UnityAction<float> OnMatchStarted { get; set; }    // Passes the time value for network latency, so a timer can be started with an adjusted value
        public UnityAction<IAIBase.ETeam, int> OnTeamScoreChanged { get; set; } // Passes the team colour and their current score
        public UnityAction<string, IAIBase.ETeam> OnPlayerDied { get; set; }    // Passes name and team for the killing player
        public UnityAction OnPlayerRespawn { get; set; } // Called when server gives the player all clear to respawn
        public UnityAction OnMatchEnded { get; set; }   // Called when match has ended
        // Log Events
        public UnityAction<string, IAIBase.ETeam, string, IAIBase.ETeam> OnKillEvent { get; set; } // Passes the killing player name and team follwed by killed player name and team
        public UnityAction<string, IAIBase.ETeam, string> OnPlayerDrivenEvent { get; set; } // Passes player name, player team and message to be displayed 
        public UnityAction<string> OnGameDrivenEvent { get; set; } // Passes the message to be displayed
        //Player Stat Events
        public UnityAction<ClientStats> OnStatsChaned { get; set; }

        public override void Init(NetworkManager networkManager)
        {
            NetworkManager = networkManager as ClientNetworkManager;

            TeamRoster = new TeamList[2];
            OnUpdateTeamRoster += (red, blue) => { TeamRoster[0] = red; TeamRoster[1] = blue; };

            //Init all states
            ClientLobbyState = new ClientLobbyState(this);
            ClientGameSetupState = new ClientGameSetupState(this);
            ClientGamePlayState = new ClientGamePlayState(this);
            ClientGameEndState = new ClientGameEndState(this);

            ChangeState(ClientLobbyState);
        }

        public void UpdateTeamDetails(IAIBase.ETeam team, int teamOrder)
        {
            this.team = team;
            this.teamOrder = teamOrder;
        }

        public bool Spawn()
        {
            if (ClientStats == null)
            {
                ClientStats = new ClientStats() { CurrentHealth = Constants.DEFAULT_HEALTH_DRONE };
                if(OnStatsChaned != null)
                {
                    OnStatsChaned(ClientStats);
                }
            }
            GameObject spawnPoint;
            if (FindSpawnPoints(out spawnPoint))
            {
                switch (team)
                {
                    case IAIBase.ETeam.TEAM_RED:
                        PlayerController = Network.Instantiate(prefabResource.RedDrone, spawnPoint.transform.position, spawnPoint.transform.rotation, 0) as PlayerController;
                        return true;
                    case IAIBase.ETeam.TEAM_BLUE:
                        PlayerController = Network.Instantiate(prefabResource.BlueDrone, spawnPoint.transform.position, spawnPoint.transform.rotation, 0) as PlayerController;
                        return true;
                    default:
                        return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void Die()
        {
            Network.Destroy(PlayerController.gameObject);
        }

        public bool LoadPrefabs()
        {
            prefabResource = Resources.Load<PlayerPrefabResourceSO>("PlayerPrefabResource");
            if (prefabResource != null)
            {
                return true;
            }
            else
            {
                Debug.LogException(new System.NullReferenceException("Prefab Resource not loaded. Cannot create player object!"));
                throw new System.NullReferenceException("Prefab Resource not loaded. Cannot create player object!");
            }
        }

        private bool FindSpawnPoints(out GameObject spawnPoint)
        {
            GameObject group = GameObject.FindGameObjectWithTag("SpawnPoint");
            if (group != null)
            {
                var spawnPoints = group.transform.GetComponentsInChildren<SpawnPoint>().Where(s => s.Team == this.team).Select(s => s.gameObject).ToList();
                if (spawnPoints.Count > teamOrder)
                {
                    spawnPoint = spawnPoints[teamOrder];
                    return true;
                }
                else
                {
                    Debug.LogException(new System.IndexOutOfRangeException("Not enough spawn points"));
                    throw new System.IndexOutOfRangeException("Not enough spawn points");
                }

            }
            else
            {
                Debug.LogException(new System.NullReferenceException("Spawn Point Group not found"));
                throw new System.NullReferenceException("Spawn Point Group not found");
            }
        }

    }

}