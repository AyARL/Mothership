using UnityEngine;
using System.Collections;

namespace MothershipOS
{
    public class User
    {
        public int UserID { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
    }

    public class Profile
    {
        public int ProfileID { get; private set; }
        public int UserID { get; private set; }
        public string DisplayName { get; private set; }
        public int EXP { get; private set; }
        public int GamesPlayed { get; private set; }
        public int GamesWon { get; private set; }
        public int GamesLost { get; private set; }
        public System.DateTime LastLogin { get; private set; }
    }
}