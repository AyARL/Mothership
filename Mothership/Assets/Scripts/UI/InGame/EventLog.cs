using UnityEngine;
using System.Collections;
using Mothership;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace MothershipUI
{
    public class EventLog : MonoBehaviour
    {
        [SerializeField]
        private GameObject playerLogMessagePrefab = null;
        [SerializeField]
        private GameObject gameLogMessagePrefab = null;
        [SerializeField]
        private GameObject killMessagePrefab = null;
        [SerializeField]
        private GameObject logParent = null;

        private List<GameObject> logMessages = null;

        private void Awake()
        {
            ClientManager clientManager = RoleManager.roleManager as ClientManager;
            if (clientManager != null)
            {
                clientManager.OnPlayerDrivenEvent += ShowPlayerDrivenMessage;
                clientManager.OnGameDrivenEvent += ShowGameDrivenMessage;
                clientManager.OnKillEvent += ShowKillMessage;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            logMessages = new List<GameObject>();
        }

        private void ShowPlayerDrivenMessage(string playerName, IAIBase.ETeam playerTeam, string message)
        {
            GameObject go = Instantiate(playerLogMessagePrefab) as GameObject;
            go.transform.SetParent(logParent.transform);

            Text[] textComponents = go.GetComponentsInChildren<Text>();

            SetTeamColourOnText(playerTeam, textComponents[0]);
            textComponents[0].text = playerName;
            textComponents[1].text = message;

            AddToLogList(go);
        }

        private void ShowGameDrivenMessage(string message)
        {
            GameObject go = Instantiate(gameLogMessagePrefab) as GameObject;
            go.transform.SetParent(logParent.transform);

            Text text = go.GetComponentInChildren<Text>();
            text.text = message;

            AddToLogList(go);
        }

        private void ShowKillMessage(string killerName, IAIBase.ETeam killerTeam, string victimName, IAIBase.ETeam victimTeam)
        {
            GameObject go = Instantiate(killMessagePrefab) as GameObject;
            go.transform.SetParent(logParent.transform);

            Text[] textComponents = go.GetComponentsInChildren<Text>();

            SetTeamColourOnText(killerTeam, textComponents[0]);
            textComponents[0].text = killerName;
            SetTeamColourOnText(victimTeam, textComponents[2]);
            textComponents[2].text = victimName;

            AddToLogList(go);
        }


        private void SetTeamColourOnText(IAIBase.ETeam playerTeam, Text textComponent)
        {
            switch (playerTeam)
            {
                case IAIBase.ETeam.TEAM_RED:
                    textComponent.color = Color.red;
                    break;
                case IAIBase.ETeam.TEAM_BLUE:
                    textComponent.color = Color.blue;
                    break;
                default:
                    textComponent.color = Color.white;
                    break;
            }

        }


        private void AddToLogList(GameObject go)
        {
            logMessages.Add(go);
        }

    }
}