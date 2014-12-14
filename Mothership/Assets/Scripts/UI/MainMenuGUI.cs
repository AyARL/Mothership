using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;

namespace MothershipUI
{
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
        private GameObject loginButton = null;
        [SerializeField]
        private GameObject registerButton = null;
        [SerializeField]
        private GameObject orLabel = null;
        [SerializeField]
        private GameObject serverButton = null;
        [SerializeField]
        private Button quit = null;


        public void EnableScreen()
        {
            content.SetActive(true);
            serverSetupGUI.DisableScreen();
            serverSetupLayout.preferredHeight = 0;
            quit.onClick.AddListener(Application.Quit);
        }

        public void DisableScreen()
        {
            serverSetupGUI.DisableScreen();
            serverSetupLayout.preferredHeight = 0;
            content.SetActive(false);
        }

        public void CreateNewServer()
        {
            loginButton.SetActive(false);
            registerButton.SetActive(false);
            orLabel.SetActive(false);
            serverButton.SetActive(false);
            quit.gameObject.SetActive(false);
            StartCoroutine(RollOut(serverSetupLayout, setupTargetHeight, setupRolloutTime, serverSetupGUI.EnableScreen));
        }

        public void CancelServerCreation()
        {
            loginButton.SetActive(true);
            registerButton.SetActive(true);
            orLabel.SetActive(true);
            serverButton.SetActive(true);
            quit.gameObject.SetActive(true);
            serverSetupGUI.DisableScreen();
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

            if (onCompleted != null)
            {
                onCompleted();
            }
        }
    }
}
