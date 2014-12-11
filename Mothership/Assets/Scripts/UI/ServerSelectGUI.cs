using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using Mothership;

namespace MothershipUI
{
    public class ServerSelectGUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject content = null;
        [SerializeField]
        private WaitScreenGUI waitScreen = null;
        [SerializeField]
        private ProfileGUI profileScreen = null;
        [SerializeField]
        private ClientLobbyGUI clientLobby = null;

        [SerializeField]
        private ServerSelectionButton serverButtonPrefab = null;
        [SerializeField]
        private GameObject scrollContent = null;

        private List<ServerSelectionButton> serverButtons;

        [SerializeField]
        private Button backButton = null;
        [SerializeField]
        private Button connectButton = null;

        private ClientNetworkManager networkManager = null;

        private void Start()
        {
            backButton.onClick.AddListener(() => BackToProfile());
            connectButton.onClick.AddListener(Connect);
            connectButton.interactable = false;
            serverButtons = new List<ServerSelectionButton>();
        }

        public void EnableScreen()
        {
            networkManager = NetworkManager.networkManager as ClientNetworkManager;
            if(networkManager == null)
            {
                CreateNetworkManager();
            }

            waitScreen.Enable("Fetching Server List from Master Server");
            networkManager.FetchServerList(OnServerListResponse);

            content.SetActive(true);
        }

        public void DisableScreen()
        {
            content.SetActive(false);
        }

        private void CreateNetworkManager()
        {
            GameObject managerObj = GameObject.FindGameObjectWithTag("NetworkManager");
            if (managerObj == null)
            {
                managerObj = new GameObject("NetworkManager");
                managerObj.tag = "NetworkManager";
            }

            networkManager = managerObj.AddComponent<ClientNetworkManager>();
        }

        private void OnServerListResponse(bool response, HostData[] hosts)
        {
            if(response == true)
            {
                PopulateServerList(hosts);
                waitScreen.Disable();
            }
            else
            {
                StartCoroutine(BackToProfile(2, "No servers found. Returning to Profile Screen"));
            }
        }

        private void PopulateServerList(HostData[] hosts)
        {
            foreach(HostData host in hosts)
            {
                ServerSelectionButton button = Instantiate(serverButtonPrefab) as ServerSelectionButton;
                button.transform.SetParent(scrollContent.transform, false);

                button.Initialise(host);
                button.Toggle.onValueChanged.AddListener(SelectionChanged);
                serverButtons.Add(button);
            }
        }

        private void SelectionChanged(bool state)
        {
            if(state == true)
            {
                connectButton.interactable = true;
            }
            else
            {
                connectButton.interactable = false;
            }
        }

        private void Connect()
        {
            HostData host = serverButtons.First(b => b.Toggle.isOn).host;
            waitScreen.Enable("Connecting to server");
            networkManager.ConnectedToServer += Connected;
            networkManager.ConnectToServer(host);
        }

        private void Connected()
        {
            waitScreen.Disable();
            DisableScreen();
            clientLobby.EnableScreen();
        }

        private IEnumerator BackToProfile(float secondsDelay = 0f, string delayMessage = "")
        {
            if(secondsDelay > 0f)
            {
                waitScreen.Enable(delayMessage);
                yield return new WaitForSeconds(secondsDelay);
                waitScreen.Disable();
            }
            DisableScreen(); 
            profileScreen.EnableScreen();
        }
    }
}
