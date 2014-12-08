using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace MothershipReplication
{
    public class PlayerReplicator : MonoBehaviour
    {
        [SerializeField]
        private Transform observedTransform = null;
        [SerializeField]
        private bool sendAnimationFlags = true;
        [SerializeField]
        private PlayerController receiver = null;
        [SerializeField]
        private float pingMargin = 0.5f;

        private float clientPing;
        private PlayerPayload[] stateBuffer = new PlayerPayload[20];

        private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
        {
            Vector3 position = observedTransform.position;
            Quaternion rotation = observedTransform.rotation;
            int animFlagIndex = -1;

            if(stream.isWriting)    // Executed by owner of the network view
            {
                stream.Serialize(ref position);
                stream.Serialize(ref rotation);

                if (sendAnimationFlags == true)
                {
                    if (receiver.animatorStates.Any(s => s.Value.State == true))
                    {
                        var animState = receiver.animatorStates.First(s => s.Value.State == true);
                        animFlagIndex = animState.Key;
                        stream.Serialize(ref animFlagIndex);
                    }
                }

            }
            else    // Executed by everyone else receiving the data
            {
                stream.Serialize(ref position);
                stream.Serialize(ref rotation);
                stream.Serialize(ref animFlagIndex);

                receiver.inPosition = position;
                receiver.inRotation = rotation;

                receiver.LerpToTarget();
                Debug.Log(animFlagIndex);
                receiver.CurrentAnimationFlag(animFlagIndex);

                //Shift buffer
                for(int i = stateBuffer.Length -1; i >= 1; i--)
                {
                    stateBuffer[i] = stateBuffer[i - 1];
                }

                // Insert latest data at the front of buffer
                stateBuffer[0] = new PlayerPayload() { Position = position, Rotation = rotation, Timestamp = (float)info.timestamp };
            }
        }

        private void Update()
        {
            if (!networkView.isMine && Network.connections.Length > 0) // If this is remote side receiving the data and connection exists
            {
                clientPing = (Network.GetAveragePing(Network.connections[0]) / 100) + pingMargin;   // true on clients only
                float interpolationTime = (float)Network.time - clientPing;

                // make sure there is at leas one entry in the buffer
                if (stateBuffer[0] == null)
                {
                    stateBuffer[0] = new PlayerPayload() { Position = observedTransform.position, Rotation = observedTransform.rotation, Timestamp = 0 };
                }

                if (stateBuffer[0].Timestamp > interpolationTime)   // lag
                {
                    for (int i = 0; i < stateBuffer.Length; i++)
                    {
                        if (stateBuffer[i] == null)
                        {
                            continue;
                        }

                        // Find best fitting state or use the last one available
                        if (stateBuffer[i].Timestamp <= interpolationTime || i == stateBuffer.Length - 1)
                        {
                            PlayerPayload bestTarget = stateBuffer[Mathf.Max(i - 1, 0)];
                            PlayerPayload bestStart = stateBuffer[i];

                            float timeDiff = bestTarget.Timestamp - bestStart.Timestamp;
                            float lerpTime = 0f;

                            if (timeDiff > 0.0001)
                            {
                                lerpTime = ((interpolationTime - bestStart.Timestamp) / timeDiff);
                            }

                            observedTransform.position = Vector3.Lerp(bestStart.Position, bestTarget.Position, lerpTime);
                            observedTransform.rotation = Quaternion.Slerp(bestStart.Rotation, bestTarget.Rotation, lerpTime);

                            return;
                        }
                    }
                }
                else    // no lag
                {
                    PlayerPayload latest = stateBuffer[0];
                    observedTransform.position = Vector3.Lerp(observedTransform.position, latest.Position, 0.5f);
                    observedTransform.rotation = Quaternion.Slerp(observedTransform.rotation, latest.Rotation, 0.5f);
                }
            }
        }

    }
}
