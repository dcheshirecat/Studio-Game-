using UnityEngine;
using UnityEngine.UI;
using EndlessBeloved.Core;

namespace EndlessBeloved.UI
{
    /// <summary>
    /// Age verification screen shown on first launch.
    /// Auto-wires all UI references by GameObject name -- no Inspector drag needed.
    /// </summary>
    public class AgeGateUI : MonoBehaviour
    {
        private GameObject ageGatePanel;
        private Button confirmButton;
        private Button denyButton;
        private Text warningText;
        private string nextScene = "TitleScreen";

        private const string AGE_VERIFIED_KEY = "age_verified";

        private void Start()
        {
            // Auto-wire by finding named objects in the scene
            AutoWire();

            // Skip if already verified
            if (PlayerPrefs.GetInt(AGE_VERIFIED_KEY, 0) == 1)
            {
                OnConfirmed();
                return;
            }

            if (ageGatePanel != null) ageGatePanel.SetActive(true);

            if (warningText != null)
            {
                warningText.text = "This game contains explicit adult content including sexual themes, " +
                    "mature language, and dark subject matter.\n\n" +
                    "You must be 18 years or older to continue.\n\n" +
                    "By pressing 'I am 18+' you confirm that you are of legal age " +
                    "in your jurisdiction to view adult content.";
            }

            if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirmed);
            if (denyButton != null) denyButton.onClick.AddListener(OnDenied);
        }

        private void AutoWire()
        {
            // Find by name in scene
            ageGatePanel = FindInScene("Background") ?? FindInScene("AgeGatePanel");
            confirmButton = FindButton("ConfirmButton");
            denyButton = FindButton("DenyButton");
            warningText = FindText("WarningText");
        }

        private void OnConfirmed()
        {
            PlayerPrefs.SetInt(AGE_VERIFIED_KEY, 1);
            PlayerPrefs.Save();
            SceneFlowManager.Instance.LoadScene(nextScene);
        }

        private void OnDenied()
        {
            if (warningText != null)
                warningText.text = "You must be 18 or older to play this game.\n\nThe application will now close.";

            if (confirmButton != null) confirmButton.gameObject.SetActive(false);
            if (denyButton != null) denyButton.gameObject.SetActive(false);

            Invoke(nameof(QuitApp), 2f);
        }

        private void QuitApp()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        // ── Auto-wire helpers ───────────────────────────────────────
        private GameObject FindInScene(string name)
        {
            var all = Resources.FindObjectsOfTypeAll<Transform>();
            foreach (var t in all)
                if (t.name == name && t.gameObject.scene.isLoaded)
                    return t.gameObject;
            return null;
        }

        private Button FindButton(string name)
        {
            var go = FindInScene(name);
            return go != null ? go.GetComponent<Button>() : null;
        }

        private Text FindText(string name)
        {
            var go = FindInScene(name);
            return go != null ? go.GetComponent<Text>() : null;
        }
    }
}
