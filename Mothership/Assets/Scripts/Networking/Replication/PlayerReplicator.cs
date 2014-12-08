using UnityEngine;
using System.Collections;

namespace MothershipReplication
{
public class PlayerReplicator : MonoBehaviour
{
    [SerializeField]
    private Transform observedTransform = null;
    [SerializeField]
    private Animator observedAnimator = null;
    [SerializeField]
    private PlayerController receiver = null;
    [SerializeField]
    private float pingMargin = 0.5f;

    private float clientPing;
    private PlayerPayload[] stateBuffer = new PlayerPayload[20];

}
}
