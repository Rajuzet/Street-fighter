using UnityEngine;

namespace StreetFighter.Data
{
    /// <summary>
    /// Defines character archetype and customization defaults.
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterProfileData", menuName = "StreetFighter/Data/CharacterProfileData")]
    public sealed class CharacterProfileData : ScriptableObject
    {
        [SerializeField]
        private string profileName = "Default";

        [SerializeField]
        private float movementSpeed = 6f;

        [SerializeField]
        private float sprintSpeed = 9f;

        [SerializeField]
        private float jumpHeight = 1.8f;

        [SerializeField]
        private float gravity = -24f;

        public string ProfileName => profileName;
        public float MovementSpeed => movementSpeed;
        public float SprintSpeed => sprintSpeed;
        public float JumpHeight => jumpHeight;
        public float Gravity => gravity;
    }
}
