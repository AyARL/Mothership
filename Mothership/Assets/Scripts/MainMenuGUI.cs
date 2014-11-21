using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;

public class MainMenuGUI : MonoBehaviour
{
    [SerializeField]
    private GameObject content = null;
    [SerializeField]
    private ServerSetupGUI serverSetupGUI = null;
    [SerializeField]
    private LayoutElement serverSetupLayout = null;
    [SerializeField]
    private float setupTargetHeight = 195f;
    [SerializeField]
    private float setupRolloutTime = 0.5f;
    [SerializeField]
    private GameObject connectButton = null;
    [SerializeField]
    private GameObject serverButton = null;

    public void Disable()
    {
        serverSetupGUI.Disable();
        serverSetupLayout.preferredHeight = 0;
        content.SetActive(false);
    }

    public void CreateNewServer()
    {
        connectButton.SetActive(false);
        serverButton.SetActive(false);
        StartCoroutine(RollOut(serverSetupLayout, setupTargetHeight, setupRolloutTime, serverSetupGUI.Enable));
    }

    public void CancelServerCreation()
    {
        connectButton.SetActive(true);
        serverButton.SetActive(true);
        serverSetupGUI.Disable();
        StartCoroutine(RollIn(serverSetupLayout, setupRolloutTime));
    }


    private IEnumerator RollOut(LayoutElement layout, float targetHeight, float rolloutTime, UnityAction onCompleted = null)
    {
        float step = (targetHeight - layout.preferredHeight) / rolloutTime;
        float t = layout.preferredHeight;

        while (layout.preferredHeight < targetHeight)
        {
            layout.preferredHeight = t;
            t += step * Time.deltaTime;
            yield return null;
        }

        layout.preferredHeight = targetHeight;

        if (onCompleted != null)
        {
            onCompleted();
        }
    }

    private IEnumerator RollIn(LayoutElement layout, float rollinTime, UnityAction onCompleted = null)
    {
        float step = layout.preferredHeight / rollinTime;
        float t = layout.preferredHeight;

        while (layout.preferredHeight > 0)
        {
            layout.preferredHeight = t;
            t -= step * Time.deltaTime;
            if (t > 0)
            {
                yield return null;
            }
            else
            {
                break;
            }
        }

        layout.preferredHeight = 0;

        if(onCompleted != null)
        {
            onCompleted();
        }
    }
}
