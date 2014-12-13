using UnityEngine;
using System.Collections;
using MothershipOS;
using MothershipStateMachine;

namespace MothershipOS
{
    public class UserDataManager : MonoBehaviour
    {
        public static UserDataManager userData = null;

        public User User { get; set; }
        public Profile Profile { get; set; }
        public GameMessage Message { get; set; }

        // Use this for initialization
        void Start()
        {
            if (userData != null)
            {
                Destroy(userData.gameObject);
            }
            else
            {
                userData = this;
                DontDestroyOnLoad(userData.gameObject);
            }
        }

        public void Clear()
        {
            User = null;
            Profile = null;
            Message = null;
        }

    }
}
