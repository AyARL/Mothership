using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Mothership;

namespace MothershipUI
{
    public class FlagMarker : MonoBehaviour
    {
        [SerializeField]
        private Image flagImage = null;
        [SerializeField]
        private Color capturedColor = Color.white;
        [SerializeField]
        private Color noFlagColor = Color.white;

        private void Awake()
        {
            ClientManager clientManager = RoleManager.roleManager as ClientManager;
            if (clientManager != null)
            {
                clientManager.CapturedFlag += CapturedFlag;
                clientManager.DroppedFlag += DroppedFlag;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void CapturedFlag()
        {
            flagImage.color = capturedColor;
        }

        private void DroppedFlag()
        {
            flagImage.color = noFlagColor;
        }
    } 
}
