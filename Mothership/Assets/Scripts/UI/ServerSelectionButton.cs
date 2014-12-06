using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace MothershipUI
{
    public class ServerSelectionButton : MonoBehaviour
    {
        [SerializeField]
        private Text serverName = null;
        public string ServerName { get { return serverName.text; } set { serverName.text = value; } }

        public HostData host { get; private set; }

        [SerializeField]
        private Toggle toggle = null;
        public Toggle Toggle { get { return toggle; } }

        [SerializeField]
        private float rollOutTime = 0.5f;
        [SerializeField]
        private int foldedHeight = 35;
        [SerializeField]
        private int unfoldedHeight = 75;
        [SerializeField]
        private GameObject description = null;
        private LayoutElement layout = null;

        public void Initialise(HostData host)
        {
            this.host = host;
            ServerName = string.Format("{0} ({1} {2})", host.gameName, host.connectedPlayers - 1, host.connectedPlayers - 1 == 1 ? "player" : "players");

            description.GetComponentInChildren<Text>().text = host.comment;
            description.SetActive(false);

            layout = GetComponent<LayoutElement>();

            ToggleGroup group = GetComponentInParent<ToggleGroup>();
            if (group != null)
            {
                toggle.group = group;
            }
            toggle.onValueChanged.AddListener(OnSelectionChanged);
        }

        private void OnSelectionChanged(bool state)
        {
            StopAllCoroutines();
            if (state == true)
            {
                StartCoroutine(RollOut());
            }
            else
            {
                StartCoroutine(RollIn());
            }
        }

        private IEnumerator RollOut()
        {
            float step = (unfoldedHeight - layout.minHeight) / rollOutTime;
            float t = layout.minHeight;

            while (layout.minHeight < unfoldedHeight)
            {
                layout.minHeight = t;
                t += step * Time.deltaTime;
                yield return null;
            }

            layout.minHeight = unfoldedHeight;
            description.SetActive(true);
        }

        private IEnumerator RollIn()
        {
            float step = layout.minHeight / rollOutTime;
            float t = layout.minHeight;

            while (layout.minHeight > 0)
            {
                layout.minHeight = t;
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

            layout.minHeight = foldedHeight;
            description.SetActive(false);
        }
    }
}