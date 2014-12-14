using UnityEngine;
using System.Collections;

namespace Mothership
{
    public class GameResult
    {
        public int BlueScore { get; set; }
        public int RedScore { get; set; }

        public PlayerResult[] PlayerResults { get; set; }

        public class PlayerResult
        {
            public string PlayerName { get; set; }
            public IAIBase.ETeam Team { get; set; }
            public int Kills { get; set; }
            public int Deaths { get; set; }
            public int Flags { get; set; }
            public int EXP { get; set; }
        }
    }
}
