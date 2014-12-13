using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Mothership;
using System.Linq;

namespace MothershipUI
{
    public class PreGameBoard : MonoBehaviour
    {
        [SerializeField]
        private Text waitingText = null;
        [SerializeField]
        private Text countdownText = null;
        [SerializeField]
        private Text[] redTeamSlots = null;
        [SerializeField]
        private Text[] blueTeamSlots = null;

        [SerializeField]
        private Color notReadyColor = Color.gray;
        [SerializeField]
        private Color readyColor = Color.black;

        private ClientManager clientManager = null;

        private void Start()
        {
            ClientManager clientManager = RoleManager.roleManager as ClientManager;
            if (clientManager == null)
            {
                Destroy(gameObject);
                return;
            }

            clientManager.OnUpdateTeamRoster += UpdateRoster;
            UpdateRoster(clientManager.TeamRoster[0], clientManager.TeamRoster[1]);
            clientManager.OnMatchCountdownStarted += MatchTimer;
            clientManager.OnMatchStarted += (delay) => gameObject.SetActive(false);
        }

        private void UpdateRoster(TeamList team1, TeamList team2)
        {
            UpdateTeam(team1);
            UpdateTeam(team2);
        }

        private void UpdateTeam(TeamList team)
        {
            int i = 0;
            switch (team.TeamColour)
            {
                case IAIBase.ETeam.TEAM_BLUE:
                    foreach (TeamListRecord player in team.TeamDisplayNames)
                    {
                        if (player.ReadyInMatch)
                        {
                            blueTeamSlots[i].color = readyColor;
                        }
                        blueTeamSlots[i].text = player.PlayerName;
                        i++;
                    }

                    // Continue looping through the display and fill remaining lisings with placeholder text
                    for (; i < blueTeamSlots.Length; i++)
                    {
                        blueTeamSlots[i].color = readyColor;
                        blueTeamSlots[i].text = "[AI]";
                    }

                    break;

                case IAIBase.ETeam.TEAM_RED:
                    foreach (TeamListRecord player in team.TeamDisplayNames)
                    {
                        redTeamSlots[i].text = player.PlayerName;
                        if (player.ReadyInMatch)
                        {
                            redTeamSlots[i].color = readyColor;
                        }
                        i++;
                    }

                    // Continue looping through the display and fill remaining lisings with placeholder text
                    for (; i < redTeamSlots.Length; i++)
                    {
                        redTeamSlots[i].text = "[AI]";
                        redTeamSlots[i].color = readyColor;
                    }

                    break;
            }
        }

        private void MatchTimer(float delay)
        {
            StartCoroutine(CountdownToMatch(delay));
        }

        private IEnumerator CountdownToMatch(float delay)
        {
            waitingText.gameObject.SetActive(false);
            countdownText.transform.parent.gameObject.SetActive(true);

            float endTime = (Time.time - delay) + Constants.GAME_PREMATCH_COUNTDOWN;
            int i = (int)Constants.GAME_PREMATCH_COUNTDOWN;
            while(Time.time < endTime)
            {
                countdownText.text = i.ToString();
                yield return new WaitForSeconds(1f);
                i -= 1;
            }
        }
    } 
}
