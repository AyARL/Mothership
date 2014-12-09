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
        private PlayerController controllerScript = null;
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
                    if (controllerScript.animatorStates.Any(s => s.Value.State == true))
                    {
                        var animState = controllerScript.animatorStates.First(s => s.Value.State == true);
                        animFlagIndex = animState.Key;
                        
                    }
                }
                stream.Serialize(ref animFlagIndex);

            }
            else    // Executed by everyone else receiving the data
            {
                stream.Serialize(ref position);
                stream.Serialize(ref rotation);
                stream.Serialize(ref animFlagIndex);

                //Shift buffer
                for(int i = stateBuffer.Length -1; i >= 1; i--)
                {
                    stateBuffer[i] = stateBuffer[i - 1];
                }

                // Insert latest data at the front of buffer
                stateBuffer[0] = new PlayerPayload() { Position = position, Rotation = rotation, ActiveAnimatorFlagIndex = animFlagIndex, Timestamp = (float)info.timestamp };
            }
        }

        // Update the Player basing on data received from the server
        private void Update()
        {
            if (!networkView.isMine && Network.connections.Length > 0) // If this is remote side receiving the data and connection exists
            {
                if (Network.isClient)
                {
                    clientPing = (Network.GetAveragePing(Network.connections[0]) / 100) + pingMargin;   // on client the only connection [0] is the server
                }
                else
                {
                    clientPing = (Network.GetAveragePing(Network.connections.First(c => c == networkView.owner)) / 100) + pingMargin; // on server find the ping to the client that owns the networkView
                }

                float interpolationTime = (float)Network.time - clientPing;

                // make sure there is at least one entry in the buffer
                if (stateBuffer[0] == null)
                {
                    stateBuffer[0] = new PlayerPayload() { Position = observedTransform.position, Rotation = observedTransform.rotation, ActiveAnimatorFlagIndex = -1, Timestamp = 0 };
                }

                // Interpolation
                if (stateBuffer[0].Timestamp > interpolationTime)   
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
                            controllerScript.CurrentAnimationFlag(bestTarget.ActiveAnimatorFlagIndex);

                            return;
                        }
                    }
                }
                else    
                {
                    //Extrapolation
                    float extrapolationTime = (interpolationTime - stateBuffer[0].Timestamp);

                    if(stateBuffer[0] != null && stateBuffer[1] != null)
                    {
                        PlayerPayload lastSample = stateBuffer[0];
                        PlayerPayload prevSample = stateBuffer[1];

                        float timeDiff = lastSample.Timestamp - prevSample.Timestamp;
                        float lerpTime = 0f;

                        if (timeDiff > 0.0001)
                        {
                            lerpTime = ((extrapolationTime - lastSample.Timestamp) / timeDiff);
                        }

                        Vector3 predictedPosition = lastSample.Position + prevSample.Position;

                        observedTransform.position = Vector3.Lerp(lastSample.Position, predictedPosition, lerpTime);
                        observedTransform.rotation = lastSample.Rotation;
                        controllerScript.CurrentAnimationFlag(lastSample.ActiveAnimatorFlagIndex);
                    }


                    // Updates to latest state - no extrapolation
                    //PlayerPayload latest = stateBuffer[0];
                    //observedTransform.position = Vector3.Lerp(observedTransform.position, latest.Position, 0.5f);
                    //observedTransform.rotation = Quaternion.Slerp(observedTransform.rotation, latest.Rotation, 0.5f);
                    //controllerScript.CurrentAnimationFlag(latest.ActiveAnimatorFlagIndex);


                }
            }
        }

    }
}
