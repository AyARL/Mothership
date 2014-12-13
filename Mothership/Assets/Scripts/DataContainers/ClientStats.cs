using UnityEngine;
using System.Collections;

namespace Mothership
{
    public class ClientStats
    {
        public float CurrentHealth { get; set; }
        public int KillCount { get; set; }
        public int DeathCount { get; set; }
        public int CaptureCount { get; set; }
        public int EXP { get; set; }
    }
}
