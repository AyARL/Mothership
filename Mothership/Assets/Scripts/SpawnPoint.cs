using UnityEngine;
using System.Collections;

namespace Mothership
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private IAIBase.ETeam team = IAIBase.ETeam.TEAM_NONE;
        public IAIBase.ETeam Team { get { return team; } }

        float size = 3f;

        private void OnDrawGizmos()
        {
            switch (team)
            {
                case IAIBase.ETeam.TEAM_BLUE:
                    Gizmos.color = Color.blue;
                    break;
                case IAIBase.ETeam.TEAM_RED:
                    Gizmos.color = Color.red;
                    break;
                case IAIBase.ETeam.TEAM_NONE:
                    Gizmos.color = Color.gray;
                    break;
            }

            Gizmos.DrawWireCube(transform.position + new Vector3(0f, size / 2f, 0f), new Vector3(3f, 3f, 3f));
        }
    }
    
}