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
    protected void RPCSendTeamData(string redTeamData, string blueTeamData)
    {
        TeamList redTeam = JsonUtility.ValidateJsonData<TeamList>(redTeamData);
        TeamList blueTeam = JsonUtility.ValidateJsonData<TeamList>(blueTeamData);

        clientManager.SendGameMessage(new UpdateTeamRoster() { RedTeam = redTeam, BlueTeam = blueTeam });
    }
    #endregion

    #region Client->Server
    [RPC]
    protected void RPCRegisterClient(string userString, string profileString)
    {
        User user = JsonUtility.ValidateJsonData<User>(userString);
        Profile profile = JsonUtility.ValidateJsonData<Profile>(profileString);

        serverManager.SendGameMessage(new RegisterClient() { User = user, Profile = profile });
    }
    #endregion

    #endregion
}
