using UnityEngine;
using System.Collections;
using MothershipOS;
using MothershipUtility;
using MothershipStateMachine;

namespace Mothership
{
    [RequireComponent(typeof(NetworkView))]
    public abstract class NetworkManager : MonoBehaviour
    {
        protected const string gameTypeName = "Mothership";
        protected const string roleManagerObjectName = "RoleManager";

        public static NetworkManager networkManager = null;
        protected ServerManager serverManager = null;
        protected ClientManager clientManager = null;

        // Used for level loading to prevent message leaking
        protected int LastLevelPrefix { get; set; }

        protected virtual void Awake()
        {
            if (networkManager != null)
            {
                Destroy(gameObject);
            }
            else
            {
                networkManager = this;

                // use local master server
                //MasterServer.ipAddress = "127.0.0.1";
                //MasterServer.port = 23466;

                DontDestroyOnLoad(gameObject);
                networkView.group = 0;
            }
        }

        public void RemoveNetworkManager()
        {
            networkManager = null;
            Destroy(this);
        }

        private void OnLevelWasLoaded(int level)
        {
            Network.isMessageQueueRunning = true;
            Network.SetSendingEnabled(0, true);

            if (Network.isClient)
            {
                networkView.RPC("RPCClientLoadedLevel", RPCMode.Server, level);
            }

            if (level != 0)
            {
                if (Network.isClient)
                {
                    clientManager.SendGameMessage(new EnteredGame());
                }

                if (Network.isServer)
                {
                    serverManager.SendGameMessage(new EnteredGame());
                }
            }
        }

        #region RPCs

        #region Server->Client
        [RPC]
        private void RPCSendClientRegistrationData(int team, int teamOrder)
        {
            IAIBase.ETeam teamVal = (IAIBase.ETeam)team;
            clientManager.SendGameMessage(new RegistrationOnServer() { Team = teamVal, TeamOrder = teamOrder });
        }

        [RPC]
        private void RPCSendTeamData(string redTeamData, string blueTeamData)
        {
            TeamList redTeam = JsonUtility.ValidateJsonData<TeamList>(redTeamData);
            TeamList blueTeam = JsonUtility.ValidateJsonData<TeamList>(blueTeamData);

            clientManager.SendGameMessage(new UpdateTeamRoster() { RedTeam = redTeam, BlueTeam = blueTeam });
        }

        [RPC]
        private void RPCLoadLevel(int level, int levelPrefix)
        {
            LastLevelPrefix = levelPrefix;

            Network.SetSendingEnabled(0, false);
            Network.isMessageQueueRunning = false;

            Network.SetLevelPrefix(LastLevelPrefix);
            Application.LoadLevel(level);
        }

        [RPC]
        private void RPCMatchCountdown(NetworkMessageInfo info)
        {
            float delay = (float)(Network.time - info.timestamp);
            clientManager.SendGameMessage(new MatchCountdownStarted() { Delay = delay });
        }

        [RPC]
        private void RPCGamePlayStarted(NetworkMessageInfo info)
        {
            float delay = (float)(Network.time - info.timestamp);
            clientManager.SendGameMessage(new GamePlayStarted() { Delay = delay });
        }

        [RPC]
        private void RPCMatchExpired()
        {
            clientManager.SendGameMessage(new MatchExpired());
        }

        [RPC]
        private void RPCScoreUpdate( int iRedScore, int iBlueScore )
        {
            clientManager.SendGameMessage( new MsgScoreUpdate() { RedScore = iRedScore, BlueScore = iBlueScore });
        }

        [RPC]
        private void RPCUpdateClientStats( string updateMessage )
        {
            MsgClientStatsUpdate update = JsonUtility.ValidateJsonData<MsgClientStatsUpdate>(updateMessage);
            clientManager.SendGameMessage(update);
        }

        [RPC]
        private void RPCForwardToClients( string strMessage )
        {
            GameMessage cMessage = JsonUtility.ValidateJsonData<GameMessage>(strMessage);
            clientManager.SendGameMessage( cMessage );
        }
        #endregion

        #region Client->Server
        [RPC]
        private void RPCRegisterClient(string userString, string profileString, NetworkMessageInfo info)
        {
            User user = JsonUtility.ValidateJsonData<User>(userString);
            Profile profile = JsonUtility.ValidateJsonData<Profile>(profileString);

            serverManager.SendGameMessage(new RegisterClient() { User = user, Profile = profile, NetworkPlayer = info.sender });
        }

        [RPC]
        private void RPCClientReadyToPlay(NetworkMessageInfo info)
        {
            serverManager.SendGameMessage(new ClientReadyToPlay() { Player = info.sender });
        }

        [RPC]
        private void RPCClientLoadedLevel(int level, NetworkMessageInfo info)
        {
            serverManager.SendGameMessage(new ClientLoadedLevel() { Player = info.sender, Level = level });
            Debug.Log("client Loaded level");
        }

        [RPC]
        private void RPCPlayerSpawned(NetworkMessageInfo info)
        {
            serverManager.SendGameMessage(new ClientSpawned() { Player = info.sender });
        }
        #endregion

        #endregion
    }
    
}