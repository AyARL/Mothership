using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MothershipUI
{
    public class WaitScreenGUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject contents = null;
        [SerializeField]
        private Text message = null;
        [SerializeField]
        private GameObject loadingIndicator = null;

        public void Enable(string message = "Processing...")
        {
            contents.SetActive(true);
            this.message.text = message;
            StartCoroutine(AnimateIndicator());
        }

        public void Disable()
        {
            StopAllCoroutines();
            contents.SetActive(false);
        }

        public void SetMessage(string message)
        {
            this.message.text = message;
        }

        private IEnumerator AnimateIndicator()
        {
            var anims = loadingIndicator.GetComponentsInChildren<Animation>();
            foreach (Animation a in anims)
            {
                a.Play();
                yield return new WaitForSeconds(0.10f);


            }
        }

        private void StopIndicatorAnimation()
        {
            var anims = loadingIndicator.GetComponentsInChildren<Animation>();
            foreach (Animation a in anims)
            {
                a.Stop();
            }
        }
    }
    
}