using UnityEngine;
using System.Collections;
using Mothership;
using UnityEngine.UI;

namespace MothershipUI
{
    public class TeamScores : MonoBehaviour
    {
        [SerializeField]
        private Text blueTeamScore = null;
        [SerializeField]
        private Text redTeamScore = null;

        private void Awake()
        {
            ClientManager clientManager = RoleManager.roleManager as ClientManager;
            if (clientManager != null)
            {
                clientManager.OnTeamScoreChanged += UpdateScore;
                blueTeamScore.text = "0";
                redTeamScore.text = "0";
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void UpdateScore(IAIBase.ETeam team, int score)
        {
            switch(team)
            {
                case IAIBase.ETeam.TEAM_BLUE:
                    blueTeamScore.text = score.ToString();
                    return;
                case IAIBase.ETeam.TEAM_RED:
                    redTeamScore.text = score.ToString();
                    return;
                default:
                    return;
            }
        }
        
    }
}
