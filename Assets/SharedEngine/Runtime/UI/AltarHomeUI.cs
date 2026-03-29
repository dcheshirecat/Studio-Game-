using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EndlessBeloved.Core;

namespace EndlessBeloved.UI
{
    /// <summary>
    /// Altar home screen hub. Auto-wires by name.
    /// </summary>
    public class AltarHomeUI : AutoWireUI
    {
        private Button routeSelectionButton;
        private Button dailyReadingButton;
        private Button spellsButton;
        private Button journalButton;
        private Button saveButton;
        private TMP_Text playerNameText;
        private TMP_Text chapterText;
        private TMP_Text cycleText;

        private void Start()
        {
            routeSelectionButton = FindBtn("RoutesButton");
            dailyReadingButton = FindBtn("DailyReadingButton");
            spellsButton = FindBtn("SpellsButton");
            journalButton = FindBtn("JournalButton");
            saveButton = FindBtn("SaveButton");
            playerNameText = FindTxt("PlayerNameText");
            chapterText = FindTxt("ChapterText");
            cycleText = FindTxt("CycleText");

            RefreshDisplay();

            routeSelectionButton?.onClick.AddListener(() =>
                SceneFlowManager.Instance.LoadScene("RouteSelection"));
            dailyReadingButton?.onClick.AddListener(OnDailyReading);
            journalButton?.onClick.AddListener(() =>
            {
                var journal = FindObjectOfType<JournalUI>();
                if (journal != null) journal.Open();
            });
            saveButton?.onClick.AddListener(OnSave);
            spellsButton?.onClick.AddListener(() =>
                Debug.Log("Spell casting coming in Phase 2"));

            // Check daily reading availability
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
            Debug.Log("Daily reading coming in Phase 2");
            PlayerPrefs.SetString("last_daily_reading",
                System.DateTime.Now.ToString("yyyy-MM-dd"));
            PlayerPrefs.Save();
            if (dailyReadingButton != null)
                dailyReadingButton.interactable = false;
        }

        private void OnSave()
        {
            SaveSystem.Instance?.SaveGame();
            AudioManager.Instance?.PlaySFX("save_confirm");
        }
    }
}
