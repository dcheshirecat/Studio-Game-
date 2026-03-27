using UnityEngine;
using UnityEngine.UI;
using EndlessBeloved.Core;

namespace EndlessBeloved.UI
{
    /// <summary>
    /// Age verification screen shown on first launch.
    /// Must be confirmed before accessing any content.
    /// </summary>
    public class AgeGateUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject ageGatePanel;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button denyButton;
        [SerializeField] private Text warningText;
        [SerializeField] private string nextScene = "TitleScreen";

        private const string AGE_VERIFIED_KEY = "age_verified";

        private void Start()
        {
            // Skip if already verified
            if (PlayerPrefs.GetInt(AGE_VERIFIED_KEY, 0) == 1)
            {
                OnConfirmed();
                return;
            }

            ageGatePanel.SetActive(true);

            if (warningText != null)
            {
                warningText.text = "This game contains explicit adult content including sexual themes, " +
                    "mature language, and dark subject matter.\n\n" +
                    "You must be 18 years or older to continue.\n\n" +
                    "By pressing 'I am 18+' you confirm that you are of legal age " +
                    "in your jurisdiction to view adult content.";
            }

            confirmButton.onClick.AddListener(OnConfirmed);
            denyButton.onClick.AddListener(OnDenied);
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

            confirmButton.gameObject.SetActive(false);
            denyButton.gameObject.SetActive(false);

            Invoke(nameof(QuitApp), 2f);
        }

        private void QuitApp()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
