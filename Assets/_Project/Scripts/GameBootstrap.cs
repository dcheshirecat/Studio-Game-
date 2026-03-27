using UnityEngine;
using EndlessBeloved.Core;
using EndlessBeloved.Dialogue;

namespace EndlessBeloved
{
    /// <summary>
    /// Entry point for the game. Creates all singleton managers
    /// and starts the game flow (AgeGate -> Title -> Game).
    /// Attach this to a single GameObject in your first scene.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Prefab References (optional)")]
        [SerializeField] private GameObject gameStateOverride;

        private void Awake()
        {
            // Ensure singletons exist
            EnsureSingleton<GameState>("GameState");
            EnsureSingleton<SaveSystem>("SaveSystem");
            EnsureSingleton<SceneFlowManager>("SceneFlowManager");
            EnsureSingleton<AudioManager>("AudioManager");

            var runner = FindObjectOfType<DialogueRunner>();
            if (runner == null)
            {
                var go = new GameObject("DialogueRunner");
                go.AddComponent<DialogueRunner>();
                DontDestroyOnLoad(go);
            }

            // Apply saved settings
            float musicVol = PlayerPrefs.GetFloat("music_volume", 0.7f);
            float sfxVol = PlayerPrefs.GetFloat("sfx_volume", 1.0f);
            AudioManager.Instance?.SetMusicVolume(musicVol);
            AudioManager.Instance?.SetSFXVolume(sfxVol);

            // Set target framerate for mobile
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void EnsureSingleton<T>(string name) where T : MonoBehaviour
        {
            if (FindObjectOfType<T>() == null)
            {
                var go = new GameObject(name);
                go.AddComponent<T>();
                DontDestroyOnLoad(go);
            }
        }
    }
}
