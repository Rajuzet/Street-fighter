using UnityEngine;

namespace StreetFighter.Save
{
    /// <summary>
    /// ScriptableObject that stores save configuration for the runtime save system.
    /// </summary>
    [CreateAssetMenu(fileName = "SaveSettings", menuName = "StreetFighter/Save/SaveSettings")]
    public sealed class SaveSettings : ScriptableObject
    {
        [SerializeField]
        private string filePrefix = "streetfighter";

        /// <summary>
        /// File prefix used for saved JSON files.
        /// </summary>
        public string FilePrefix => filePrefix;
    }
}
