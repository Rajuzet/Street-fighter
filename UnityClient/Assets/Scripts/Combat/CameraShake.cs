using Cinemachine;
using UnityEngine;

namespace StreetFighter.Combat
{
    public class CameraShake : MonoBehaviour
    {
        private CinemachineBasicMultiChannelPerformer performer;

        public static void Shake(float magnitude)
        {
            var cam = Camera.main.GetComponent<CinemachineVirtualCamera>();
            var shake = cam.GetComponent<CinemachineBasicMultiChannelPerformer>();
            shake.GenerateImpulse(magnitude);
        }
    }
}
