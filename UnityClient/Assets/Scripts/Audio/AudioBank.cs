using System.Collections.Generic;
using UnityEngine;

namespace StreetFighter.Audio
{
    /// <summary>
    /// Collection of audio clip references available for runtime playback.
    /// </summary>
    [CreateAssetMenu(fileName = "AudioBank", menuName = "StreetFighter/Audio/AudioBank")]
    public sealed class AudioBank : ScriptableObject
    {
        [SerializeField]
        private List<AudioEntry> entries = new();

        public IReadOnlyList<AudioEntry> Entries => entries;

        [System.Serializable]
        public sealed class AudioEntry
        {
            public string Key;
            public AudioClip AudioClip;
        }
    }
}
