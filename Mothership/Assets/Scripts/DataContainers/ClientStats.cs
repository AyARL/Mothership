using UnityEngine;
using System.Collections;
using MothershipStateMachine;

namespace Mothership
{
    public class ClientStats
    {
        public float CurrentHealth { get; set; }
        public int KillCount { get; set; }
        public int DeathCount { get; set; }
        public int CaptureCount { get; set; }
        public int EXP { get; set; }

        public void UpdateStats(MsgClientStatsUpdate newData)
        {
            CurrentHealth = newData.CurrentHealth;
            KillCount = newData.KillCount;
            DeathCount = newData.DeathCount;
            CaptureCount = newData.CaptureCount;
            EXP = newData.EXP;
        }
    }
}
