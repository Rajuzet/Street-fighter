using System;
using System.Collections.Generic;
using UnityEngine;

namespace StreetFighter.Audio
{
    /// <summary>
    /// Manages runtime audio playback and audio bank lookup.
    /// </summary>
    public class AudioManager : MonoBehaviour, IAudioService
    {
        [SerializeField]
        private AudioSource musicSource = null;

        [SerializeField]
        private AudioSource sfxSourcePrefab = null;

        [SerializeField]
        private Transform audioPoolParent = null;

        private AudioBank audioBank;
        private readonly Dictionary<string, AudioClip> clipCache = new();

        /// <inheritdoc />
        public void Initialize(AudioBank bank)
        {
            audioBank = bank ?? throw new ArgumentNullException(nameof(bank));
            PreloadClips();

            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
            }

            if (audioPoolParent == null)
            {
                var poolObject = new GameObject("AudioPool");
                poolObject.transform.SetParent(transform, false);
                audioPoolParent = poolObject.transform;
            }
        }

        /// <inheritdoc />
        public void PlaySound(string clipKey, Vector3 position, float volume = 1f)
        {
            if (!clipCache.TryGetValue(clipKey, out var clip) || clip == null)
            {
                Debug.LogWarning($"Audio clip '{clipKey}' not found in audio bank.");
                return;
            }

            var source = Instantiate(sfxSourcePrefab, position, Quaternion.identity, audioPoolParent);
            source.clip = clip;
            source.volume = volume;
            source.spatialBlend = 1f;
            source.Play();
            Destroy(source.gameObject, clip.length + 0.1f);
        }

        /// <inheritdoc />
        public void PlayMusic(string clipKey, float volume = 1f)
        {
            if (!clipCache.TryGetValue(clipKey, out var clip) || clip == null)
            {
                Debug.LogWarning($"Music clip '{clipKey}' not found in audio bank.");
                return;
            }

            musicSource.clip = clip;
            musicSource.volume = volume;
            musicSource.loop = true;
            musicSource.Play();
        }

        /// <inheritdoc />
        public void StopMusic()
        {
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
            }
        }

        private void PreloadClips()
        {
            clipCache.Clear();
            foreach (var entry in audioBank.Entries)
            {
                if (entry.AudioClip == null)
                {
                    continue;
                }

                if (!clipCache.ContainsKey(entry.Key))
                {
                    clipCache.Add(entry.Key, entry.AudioClip);
                }
            }
        }
    }
}
