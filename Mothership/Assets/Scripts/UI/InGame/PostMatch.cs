using UnityEngine;
using System.Collections;
using Mothership;
using UnityEngine.UI;

namespace MothershipUI
{
    public class PostMatch : MonoBehaviour
    {
        [SerializeField]
        private GameObject screen = null;
        [SerializeField]
        private ScoreboardUI[] redScoreboard = null;
        [SerializeField]
        private ScoreboardUI[] blueScoreboard = null;

        [SerializeField]
        private Button exitButton = null;

        private void Awake()
        {
            ClientManager clientManager = RoleManager.roleManager as ClientManager;
            if (clientManager != null)
            {
                clientManager.OnMatchEnded += () => screen.SetActive(true);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        [System.Serializable]
        private class ScoreboardUI
        {
            [SerializeField]
            private Text name = null;
            public string Name { get { return name.text; } set { name.text = value; } }
            [SerializeField]
            private Text kills = null;
            public string Kills { get { return kills.text; } set { kills.text = value; } }
            [SerializeField]
            private Text deaths = null;
            public string Deaths { get { return deaths.text; } set { deaths.text = value; } }
            [SerializeField]
            private Text flags = null;
            public string Flags { get { return flags.text; } set { flags.text = value; } }
            [SerializeField]
            private Text exp = null;
            public string EXP { get { return exp.text; } set { exp.text = value; } }
        }
    } 
}
