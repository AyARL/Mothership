using UnityEngine;
using System.Collections;
using Mothership;
using UnityEngine.UI;
using MothershipStateMachine;

namespace MothershipUI
{
    public class PostMatch : MonoBehaviour
    {
        [SerializeField]
        private GameObject screen = null;
        [SerializeField]
        private Text winnerTeam = null;
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
                clientManager.OnGameResultReceived += ProcessResults;

                exitButton.onClick.AddListener(() => clientManager.SendGameMessage(new ExitMatch()));
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void ProcessResults(GameResult result)
        {
            ShowWinnerText(result);
            int redI = 0;
            int blueI = 0;
            foreach(GameResult.PlayerResult player in result.PlayerResults)
            {
                ScoreboardUI ui = null;
                switch(player.Team)
                {
                    case IAIBase.ETeam.TEAM_BLUE:
                        ui = redScoreboard[redI];
                        ++redI;
                        break;

                    case IAIBase.ETeam.TEAM_RED:
                        ui = blueScoreboard[blueI];
                        ++blueI;
                        break;
                }

                if(ui != null)
                {
                    ui.Name = player.PlayerName;
                    ui.Kills = player.Kills.ToString();
                    ui.Deaths = player.Deaths.ToString();
                    ui.Flags = player.Flags.ToString();
                    ui.EXP = player.EXP.ToString();
                }
            }
        }

        private void ShowWinnerText(GameResult result)
        {
            if (result.RedScore > result.BlueScore)
            {
                winnerTeam.color = Color.red;
                winnerTeam.text = "Red Team";
            }
            else if (result.BlueScore > result.RedScore)
            {
                winnerTeam.color = Color.blue;
                winnerTeam.text = "Blue Team";
            }
            else
            {
                winnerTeam.color = Color.white;
                winnerTeam.text = "It's a Draw. No one";
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
