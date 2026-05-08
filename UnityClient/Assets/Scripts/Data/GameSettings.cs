using UnityEngine;

namespace StreetFighter.Data
{
    /// <summary>
    /// Global gameplay configuration used by runtime managers.
    /// </summary>
    [CreateAssetMenu(fileName = "GameSettings", menuName = "StreetFighter/Data/GameSettings")]
    public sealed class GameSettings : ScriptableObject
    {
        [SerializeField]
        private float inputSmoothing = 0.08f;

        [SerializeField]
        private float cameraSensitivity = 12f;

        [SerializeField]
        private float mouseSensitivity = 1.2f;

        public float InputSmoothing => inputSmoothing;
        public float CameraSensitivity => cameraSensitivity;
        public float MouseSensitivity => mouseSensitivity;
    }
}
