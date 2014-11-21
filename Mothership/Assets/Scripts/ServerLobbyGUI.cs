using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ServerLobbyGUI : MonoBehaviour
{
    [SerializeField]
    private GameObject content = null;
    [SerializeField]
    private GameObject clientList = null;
    [SerializeField]
    private Text clientLabelPrefab = null;
    private Text[] clientLabels = null;

    public void Enable()
    {
        content.SetActive(true);

        clientLabels = new Text[Network.maxConnections];
        for(int i = 0; i < clientLabels.Length; i++)
        {
            Text t = Instantiate(clientLabelPrefab) as Text;
            t.transform.SetParent(clientList.transform);
        }
    }

    public void Disable()
    {
        content.SetActive(false);
    }

    public void AddClient()
    {

    }
}
