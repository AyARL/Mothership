using UnityEngine;
using System.Collections;
using Mothership;
using UnityEngine.UI;

namespace MothershipUI
{
    public class DeathBanner : MonoBehaviour
    {
        [SerializeField]
        private GameObject deathBanner = null;
        [SerializeField]
        private Text playerName = null;
        [SerializeField]
        private Text counter = null;

        public void Start()
        {
            ClientManager clientManager = RoleManager.roleManager as ClientManager;
            if (clientManager != null)
            {
                clientManager.OnPlayerDied += Enable;
                clientManager.OnPlayerRespawn += Disable;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        public void Enable(string killerName, IAIBase.ETeam killerTeam)
        {
            deathBanner.SetActive(true);

            switch(killerTeam)
            {
                case IAIBase.ETeam.TEAM_RED:
                    playerName.color = Color.red;
                    break;
                case IAIBase.ETeam.TEAM_BLUE:
                    playerName.color = Color.blue;
                    break;
                default:
                    playerName.color = Color.white;
                    break;
            }
            playerName.text = killerName;
            StartCoroutine(Countdown());
        }

        public void Disable()
        {
            StopAllCoroutines();
            deathBanner.SetActive(false);
        }

        private IEnumerator Countdown()
        {
            int i = Constants.GAME_PLAYER_RESPAWN_COUNTDOWN;
            while(i > 0)
            {
                counter.text = i.ToString();
                yield return new WaitForSeconds(1f);
                i =- 1;
            }
        }
    }
}
