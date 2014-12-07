using UnityEngine;
using System.Collections;
using MothershipOS;
using MothershipUtility;
using MothershipStateMachine;

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
            DontDestroyOnLoad(gameObject);
            networkView.group = 0;
        }
    }

    public void RemoveNetworkManager()
    {
        networkManager = null;
        Destroy(this);
    }

    #region RPCs

    #region Server->Client
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
    #endregion

    #endregion
}
