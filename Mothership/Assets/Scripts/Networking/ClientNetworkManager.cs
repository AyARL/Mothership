using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using MothershipUtility;
using MothershipOS;
using MothershipStateMachine;

namespace Mothership
{
    public class ClientNetworkManager : NetworkManager
    {
        private float timeOut = 5.0f;

        public UnityAction<bool, HostData[]> OnHostListFetchCompleted { get; set; }
        private HostData[] availableHosts;

        public UnityAction ConnectedToServer { get; set; }
        public HostData ServerHostData { get; private set; }

        public void FetchServerList(UnityAction<bool, HostData[]> onFetchCompleted)
        {
            OnHostListFetchCompleted += onFetchCompleted;
            StartCoroutine(FetchHostsFromMasterServer());
        }

        private IEnumerator FetchHostsFromMasterServer()
        {
            MasterServer.ClearHostList();
            MasterServer.RequestHostList(gameTypeName);
            float timeOutTime = Time.time + timeOut;

            while (Time.time < timeOutTime)
            {
                availableHosts = MasterServer.PollHostList();
                if (availableHosts.Length > 0)
                {
                    if (OnHostListFetchCompleted != null)
                    {
                        OnHostListFetchCompleted(true, availableHosts);
                    }

                    yield break;
                }

                yield return new WaitForSeconds(1f);
            }

            Debug.Log("Timed out waiting for host list");
            if (OnHostListFetchCompleted != null)
            {
                OnHostListFetchCompleted(false, null);
            }
        }

        public void ConnectToServer(HostData host)
        {
            NetworkConnectionError error = Network.Connect(host);
            if (error == NetworkConnectionError.NoError)
            {
                ServerHostData = host;
            }
        }

        private void OnConnectedToServer()
        {
            InitialiseRoleManager();
            if (ConnectedToServer != null)
            {
                ConnectedToServer();
            }
        }

        private void InitialiseRoleManager()
        {
            GameObject roleManagerObj = new GameObject(roleManagerObjectName);
            clientManager = roleManagerObj.AddComponent<ClientManager>();
            clientManager.Init(this);
        }

        public void RegisterOnServer()
        {
            string serializedUser = JsonUtility.SerializeToJson<User>(UserDataManager.userData.User);
            string serializedProfile = JsonUtility.SerializeToJson<Profile>(UserDataManager.userData.Profile);
            networkView.RPC("RPCRegisterClient", RPCMode.Server, serializedUser, serializedProfile);
        }

        public void ReadyToPlay()
        {
            networkView.RPC("RPCClientReadyToPlay", RPCMode.Server);
        }


    }
    
}