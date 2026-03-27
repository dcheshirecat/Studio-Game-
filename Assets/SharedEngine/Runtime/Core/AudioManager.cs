using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessBeloved.Core
{
    /// <summary>
    /// Manages music and SFX playback with crossfade support.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float musicVolume = 0.7f;
        [SerializeField] private float sfxVolume = 1.0f;
        [SerializeField] private float crossfadeDuration = 1.5f;

        private AudioSource musicSourceA;
        private AudioSource musicSourceB;
        private AudioSource sfxSource;
        private bool usingSourceA = true;

        private Dictionary<string, AudioClip> musicCache = new Dictionary<string, AudioClip>();
        private Dictionary<string, AudioClip> sfxCache = new Dictionary<string, AudioClip>();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            musicSourceA = gameObject.AddComponent<AudioSource>();
            musicSourceA.loop = true;
            musicSourceA.playOnAwake = false;

            musicSourceB = gameObject.AddComponent<AudioSource>();
            musicSourceB.loop = true;
            musicSourceB.playOnAwake = false;

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        public void PlayMusic(string trackName)
        {
            var clip = LoadMusic(trackName);
            if (clip == null) return;

            var current = usingSourceA ? musicSourceA : musicSourceB;
            if (current.clip == clip && current.isPlaying) return;

            StartCoroutine(CrossfadeMusic(clip));
        }

        public void StopMusic(float fadeOut = 1f)
        {
            StartCoroutine(FadeOutCurrent(fadeOut));
        }

        public void PlaySFX(string sfxName)
        {
            var clip = LoadSFX(sfxName);
            if (clip != null)
                sfxSource.PlayOneShot(clip, sfxVolume);
        }

        public void SetMusicVolume(float vol)
        {
            musicVolume = Mathf.Clamp01(vol);
            var current = usingSourceA ? musicSourceA : musicSourceB;
            current.volume = musicVolume;
        }

        public void SetSFXVolume(float vol)
        {
            sfxVolume = Mathf.Clamp01(vol);
        }

        private IEnumerator CrossfadeMusic(AudioClip newClip)
        {
            var outgoing = usingSourceA ? musicSourceA : musicSourceB;
            var incoming = usingSourceA ? musicSourceB : musicSourceA;
            usingSourceA = !usingSourceA;

            incoming.clip = newClip;
            incoming.volume = 0f;
            incoming.Play();

            float elapsed = 0f;
            while (elapsed < crossfadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / crossfadeDuration;
                outgoing.volume = Mathf.Lerp(musicVolume, 0f, t);
                incoming.volume = Mathf.Lerp(0f, musicVolume, t);
                yield return null;
            }

            outgoing.Stop();
            outgoing.clip = null;
            incoming.volume = musicVolume;
        }

        private IEnumerator FadeOutCurrent(float duration)
        {
            var current = usingSourceA ? musicSourceA : musicSourceB;
            float startVol = current.volume;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                current.volume = Mathf.Lerp(startVol, 0f, elapsed / duration);
                yield return null;
            }
            current.Stop();
            current.clip = null;
        }

        private AudioClip LoadMusic(string name)
        {
            if (musicCache.TryGetValue(name, out var cached)) return cached;
            var clip = Resources.Load<AudioClip>($"Audio/Music/{name}");
            if (clip != null) musicCache[name] = clip;
            else Debug.LogWarning($"Music not found: Audio/Music/{name}");
            return clip;
        }

        private AudioClip LoadSFX(string name)
        {
            if (sfxCache.TryGetValue(name, out var cached)) return cached;
            var clip = Resources.Load<AudioClip>($"Audio/SFX/{name}");
            if (clip != null) sfxCache[name] = clip;
            else Debug.LogWarning($"SFX not found: Audio/SFX/{name}");
            return clip;
        }
    }
}
