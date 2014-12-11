using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Mothership;

namespace MothershipUI
{
    public class MatchTimer : MonoBehaviour
    {
        [SerializeField]
        private Text timeDisplay = null;

        private float startTime = 0f;
        private float duration = 0f;

        private void Awake()
        {
            ClientManager clientManager = RoleManager.roleManager as ClientManager;
            if(clientManager != null)
            {
                clientManager.OnMatchStarted = StartCountdown;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DisplayTime(Constants.GAME_MATCH_LENGTH);
        }

        private void StartCountdown(float messageDelay)
        {
            startTime = Time.time - messageDelay;
            StartCoroutine(Countdown());
        }

        private IEnumerator Countdown()
        {
            duration = Time.time - startTime;
            while(duration < Constants.GAME_MATCH_LENGTH)
            {
                DisplayTime(Constants.GAME_MATCH_LENGTH - duration);
                yield return new WaitForSeconds(1f);
                duration = Time.time - startTime;
            }
        }

        private void DisplayTime(float time)
        {
            int minutes, seconds;
            FormatTime(time, out minutes, out seconds);
            string sMinutes = minutes.ToString();
            string sSeconds = seconds.ToString();

            if (sMinutes.Length == 1)
            {
               sMinutes = sMinutes.Insert(0, "0");
            }

            if (sSeconds.Length == 1)
            {
               sSeconds = sSeconds.Insert(0, "0");
            }

            timeDisplay.text = string.Format("{0}:{1}", sMinutes, sSeconds);
        }

        private void FormatTime(float inTime, out int minutes, out int seconds)
        {
            minutes = (int)(inTime / 60f);
            seconds = (int)(inTime % 60f);
        }
    } 
}
