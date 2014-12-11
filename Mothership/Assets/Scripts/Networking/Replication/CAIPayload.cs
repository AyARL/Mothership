using UnityEngine;
using System.Collections;

namespace MothershipReplication
{
    public class CAIPayload
    {
        public float Timestamp { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public int ActiveAnimatorFlagIndex { get; set; }
    }
}
