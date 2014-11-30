using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ServerSetupGUI : MonoBehaviour
{
    [SerializeField]
    private InputField serverName = null;
    [SerializeField]
    private InputField serverDescription = null;
    [SerializeField]
    private Button startServerButton = null;
    [SerializeField]
    private Button backButton = null;

    [SerializeField]
    private MainMenuGUI mainMenu = null;
    [SerializeField]
    private WaitScreenGUI waitScreen = null;
    [SerializeField]
    private ServerLobbyGUI serverLobby = null;

    private ServerNetworkManager networkManager = null;
   

    private void Start()
    {
        serverName.characterValidation = InputField.CharacterValidation.Alphanumeric;
        InputField.SubmitEvent submitEvent = new InputField.SubmitEvent();
        submitEvent.AddListener(OnServerNameSet);
        serverName.onEndEdit = submitEvent;

        startServerButton.onClick.AddListener(StartServer);
    }

    public void EnableScreen()
    {
        serverName.interactable = true;
        serverDescription.interactable = true;
        backButton.interactable = true;
        OnServerNameSet(serverName.text);
    }

    public void DisableScreen()
    {
        foreach(Transform t in transform)
        {
            Selectable s = t.GetComponent<Selectable>();
            if(s != null)
            {
                s.interactable = false;
            }
        }
    }

    private void StartServer()
    {
        GameObject managerObj = GameObject.FindGameObjectWithTag("NetworkManager");
        if (managerObj == null)
        {
            managerObj = new GameObject("NetworkManager");
            managerObj.tag = "NetworkManager";
        }

        networkManager = managerObj.AddComponent<ServerNetworkManager>();

        mainMenu.DisableScreen();
        waitScreen.Enable("Waiting for Master Server");
        networkManager.OnServerReady += () => { waitScreen.Disable(); serverLobby.Enable(); };

        networkManager.StartServer(serverName.text, serverDescription.text);
    }

    private void OnServerNameSet(string name)
    {
        if(name.Length > 0)
        {
            startServerButton.interactable = true;
        }
    }

}
