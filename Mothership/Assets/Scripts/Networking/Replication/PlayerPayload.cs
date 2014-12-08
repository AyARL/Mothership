using UnityEngine;
using System.Collections;

namespace MothershipReplication
{
    public class PlayerPayload
    {
        public float Timestamp { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }

        public AnimatorState[] animatorStates { get; set; }
    }

    public class AnimatorState
    {
        public string StateName { get; set; }
        public bool StateValue { get; set; }
    }
}