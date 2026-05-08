using UnityEngine;

namespace StreetFighter.Audio
{
    /// <summary>
    /// Defines the contract for audio playback systems.
    /// </summary>
    public interface IAudioService
    {
        void Initialize(AudioBank bank);
        void PlaySound(string clipKey, Vector3 position, float volume = 1f);
        void PlayMusic(string clipKey, float volume = 1f);
        void StopMusic();
    }
}
