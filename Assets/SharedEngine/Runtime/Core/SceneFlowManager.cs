using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EndlessBeloved.Core
{
    /// <summary>
    /// Manages scene transitions with fade effects. Singleton that persists across scenes.
    /// </summary>
    public class SceneFlowManager : MonoBehaviour
    {
        public static SceneFlowManager Instance { get; private set; }

        [Header("Transition Settings")]
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private Color fadeColor = Color.black;

        private Canvas fadeCanvas;
        private Image fadeImage;
        private bool isTransitioning = false;

        public event Action<string> OnSceneLoadStarted;
        public event Action<string> OnSceneLoadCompleted;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateFadeCanvas();
        }

        private void CreateFadeCanvas()
        {
            var go = new GameObject("FadeCanvas");
            go.transform.SetParent(transform);
            fadeCanvas = go.AddComponent<Canvas>();
            fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            fadeCanvas.sortingOrder = 9999;

            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);

            go.AddComponent<GraphicRaycaster>();

            var imgGo = new GameObject("FadeImage");
            imgGo.transform.SetParent(go.transform, false);
            fadeImage = imgGo.AddComponent<Image>();
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
            fadeImage.raycastTarget = false;

            var rect = fadeImage.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        /// <summary>
        /// Transition to a scene by name with a fade effect.
        /// </summary>
        public void LoadScene(string sceneName, float customFadeDuration = -1f)
        {
            if (isTransitioning) return;
            StartCoroutine(TransitionCoroutine(sceneName, customFadeDuration > 0 ? customFadeDuration : fadeDuration));
        }

        /// <summary>
        /// Transition to a scene by build index.
        /// </summary>
        public void LoadScene(int sceneIndex, float customFadeDuration = -1f)
        {
            if (isTransitioning) return;
            string sceneName = SceneManager.GetSceneByBuildIndex(sceneIndex).name;
            LoadScene(sceneName, customFadeDuration);
        }

        private IEnumerator TransitionCoroutine(string sceneName, float duration)
        {
            isTransitioning = true;
            fadeImage.raycastTarget = true;
            OnSceneLoadStarted?.Invoke(sceneName);

            // Fade out
            yield return StartCoroutine(Fade(0f, 1f, duration));

            // Load scene
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            while (!op.isDone)
                yield return null;

            // Auto-save on chapter transitions
            if (SaveSystem.Instance != null)
                SaveSystem.Instance.AutoSave();

            // Fade in
            yield return StartCoroutine(Fade(1f, 0f, duration));

            fadeImage.raycastTarget = false;
            isTransitioning = false;
            OnSceneLoadCompleted?.Invoke(sceneName);
        }

        private IEnumerator Fade(float from, float to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float alpha = Mathf.Lerp(from, to, t);
                fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
                yield return null;
            }
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, to);
        }

        /// <summary>
        /// Quick flash effect (fade out then in rapidly).
        /// </summary>
        public void Flash(float duration = 0.3f)
        {
            if (!isTransitioning)
                StartCoroutine(FlashCoroutine(duration));
        }

        private IEnumerator FlashCoroutine(float duration)
        {
            yield return StartCoroutine(Fade(0f, 1f, duration * 0.3f));
            yield return StartCoroutine(Fade(1f, 0f, duration * 0.7f));
        }

        public bool IsTransitioning => isTransitioning;
    }
}
