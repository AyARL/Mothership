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
                clientManager.OnHealthChanged += UpdateHealth;
                clientManager.OnKillCountChanged += (count) => killCounter.text = count.ToString();
                clientManager.OnDeathCountChanged += (count) => deathCounter.text = count.ToString();
                clientManager.OnCaptureCountChanged += (count) => captureCounter.text = count.ToString();
                clientManager.OnEXPValueChanged += (count) => expCounter.text = count.ToString();

                maxHealthBarSize = healthBar.rect.width;
                UpdateHealth((int)(Constants.DEFAULT_HEALTH_DRONE));
            }
            else
            {
                Destroy(gameObject);
                return;
            }

        }

        private void UpdateHealth(int newHPValue)
        {
            currentHealth.text = newHPValue.ToString();
            float healthPercentage = newHPValue / Constants.DEFAULT_HEALTH_DRONE;
            healthBar.sizeDelta = new Vector2(maxHealthBarSize * healthPercentage, healthBar.rect.height);
        }
    }
    
}