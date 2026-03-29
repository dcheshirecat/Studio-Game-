using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EndlessBeloved.UI
{
    /// <summary>
    /// Base class that provides auto-wiring helpers for finding UI elements by name.
    /// All UI scripts inherit from this to avoid needing manual Inspector drag-and-drop.
    /// </summary>
    public abstract class AutoWireUI : MonoBehaviour
    {
        protected GameObject Find(string name)
        {
            // First try children of this object
            var t = transform.Find(name);
            if (t != null) return t.gameObject;

            // Then search all canvas children
            var canvases = FindObjectsOfType<Canvas>();
            foreach (var canvas in canvases)
            {
                var result = FindDeep(canvas.transform, name);
                if (result != null) return result.gameObject;
            }

            // Finally search entire scene
            var all = FindObjectsOfType<Transform>();
            foreach (var tr in all)
                if (tr.name == name) return tr.gameObject;

            Debug.LogWarning($"AutoWireUI: Could not find '{name}' in scene");
            return null;
        }

        protected Button FindBtn(string name)
        {
            var go = Find(name);
            return go != null ? go.GetComponent<Button>() ?? go.GetComponentInChildren<Button>() : null;
        }

        protected TMP_Text FindTxt(string name)
        {
            var go = Find(name);
            return go != null ? go.GetComponent<TMP_Text>() ?? go.GetComponentInChildren<TMP_Text>() : null;
        }

        protected Image FindImg(string name)
        {
            var go = Find(name);
            return go != null ? go.GetComponent<Image>() ?? go.GetComponentInChildren<Image>() : null;
        }

        protected TMP_InputField FindInput(string name)
        {
            var go = Find(name);
            return go != null ? go.GetComponent<TMP_InputField>() ?? go.GetComponentInChildren<TMP_InputField>() : null;
        }

        protected Slider FindSlider(string name)
        {
            var go = Find(name);
            return go != null ? go.GetComponent<Slider>() ?? go.GetComponentInChildren<Slider>() : null;
        }

        protected T FindComponent<T>(string name) where T : Component
        {
            var go = Find(name);
            return go != null ? go.GetComponent<T>() : null;
        }

        private Transform FindDeep(Transform parent, string name)
        {
            if (parent.name == name) return parent;
            for (int i = 0; i < parent.childCount; i++)
            {
                var result = FindDeep(parent.GetChild(i), name);
                if (result != null) return result;
            }
            return null;
        }
    }
}
