using UnityEngine;
using System.Collections;

namespace MothershipOS
{
    public class PlayerGameStats
    {
        public int GameID { get; private set; }
        public string DatePlayed { get; private set; }
        public string Winner { get; private set; }
        public string Team { get; private set; }
        public int EXPEarned { get; private set; }
    }
}