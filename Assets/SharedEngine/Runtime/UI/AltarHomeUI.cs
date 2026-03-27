using UnityEngine;
using UnityEngine.UI;
using EndlessBeloved.Core;

namespace EndlessBeloved.UI
{
    /// <summary>
    /// Altar home screen: the hub between story chapters.
    /// Player can access routes, daily reading, altar, spells, journal.
    /// </summary>
    public class AltarHomeUI : MonoBehaviour
    {
        [Header("Navigation Buttons")]
        [SerializeField] private Button routeSelectionButton;
        [SerializeField] private Button dailyReadingButton;
        [SerializeField] private Button altarButton;
        [SerializeField] private Button spellsButton;
        [SerializeField] private Button journalButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button saveButton;

        [Header("Status Display")]
        [SerializeField] private Text playerNameText;
        [SerializeField] private Text chapterText;
        [SerializeField] private Text cycleText;

        [Header("Sub-panels")]
        [SerializeField] private CardDrawUI cardDrawUI;
        [SerializeField] private JournalUI journalUI;

        [Header("Scenes")]
        [SerializeField] private string routeSelectionScene = "RouteSelection";
        [SerializeField] private string spellCastingScene = "SpellCasting";
        [SerializeField] private string settingsScene = "TitleScreen";

        private void Start()
        {
            RefreshDisplay();

            routeSelectionButton?.onClick.AddListener(() =>
                SceneFlowManager.Instance.LoadScene(routeSelectionScene));

            dailyReadingButton?.onClick.AddListener(OnDailyReading);
            journalButton?.onClick.AddListener(() => journalUI?.Open());
            saveButton?.onClick.AddListener(OnSave);

            spellsButton?.onClick.AddListener(() =>
                SceneFlowManager.Instance.LoadScene(spellCastingScene));

            // Check if daily reading is available (once per real-world day)
            string lastReading = PlayerPrefs.GetString("last_daily_reading", "");
            string today = System.DateTime.Now.ToString("yyyy-MM-dd");
            if (dailyReadingButton != null)
                dailyReadingButton.interactable = (lastReading != today);

            AudioManager.Instance?.PlayMusic("altar_ambient");
        }

        private void RefreshDisplay()
        {
            var gs = GameState.Instance;
            if (playerNameText != null) playerNameText.text = gs.PlayerName;
            if (chapterText != null) chapterText.text = $"Chapter {gs.CurrentChapter}";
            if (cycleText != null) cycleText.text = $"Cycle {gs.CycleNumber}";
        }

        private void OnDailyReading()
        {
            if (cardDrawUI != null)
            {
                cardDrawUI.gameObject.SetActive(true);
                cardDrawUI.OpenForDailyReading();

                // Record that we did today's reading
                PlayerPrefs.SetString("last_daily_reading",
                    System.DateTime.Now.ToString("yyyy-MM-dd"));
                PlayerPrefs.Save();

                if (dailyReadingButton != null)
                    dailyReadingButton.interactable = false;
            }
        }

        private void OnSave()
        {
            SaveSystem.Instance.SaveGame();
            AudioManager.Instance?.PlaySFX("save_confirm");
        }
    }
}
