using UnityEngine;
using System.Collections;
using Mothership;
using UnityEngine.UI;

namespace MothershipUI
{
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField]
        private Text currentHealth = null;
        [SerializeField]
        private RectTransform healthBar = null;
        private float maxHealthBarSize = 0f;
        [SerializeField]
        private Text killCounter = null;
        [SerializeField]
        private Text deathCounter = null;
        [SerializeField]
        private Text captureCounter = null;
        [SerializeField]
        private Text expCounter = null;

        private void Awake()
        {
            ClientManager clientManager = RoleManager.roleManager as ClientManager;
            if (clientManager != null)
            {
                clientManager.OnStatsChanged += UpdateStats;
                maxHealthBarSize = healthBar.rect.width;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

        }

        private void UpdateStats(ClientStats newData)
        {
            currentHealth.text = newData.CurrentHealth.ToString();
            float healthPercentage = newData.CurrentHealth / Constants.DEFAULT_HEALTH_DRONE;
            healthBar.sizeDelta = new Vector2(maxHealthBarSize * healthPercentage, healthBar.rect.height / 2f);

            killCounter.text = newData.KillCount.ToString();
            deathCounter.text = newData.DeathCount.ToString();
            captureCounter.text = newData.CaptureCount.ToString();
            expCounter.text = newData.EXP.ToString();

        }
    }
    
}