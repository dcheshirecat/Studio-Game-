using UnityEngine;
using UnityEngine.UI;
using EndlessBeloved.Core;

namespace EndlessBeloved.UI
{
    /// <summary>
    /// Title screen / main menu. Auto-wires all UI references by name.
    /// </summary>
    public class MainMenuUI : AutoWireUI
    {
        private GameObject mainPanel;
        private GameObject saveSlotPanel;
        private GameObject settingsPanel;
        private Button newGameButton;
        private Button continueButton;
        private Button settingsButton;
        private Button backFromSlotsButton;
        private Button backFromSettingsButton;
        private Slider musicVolumeSlider;
        private Slider sfxVolumeSlider;
        private Text[] slotLabels = new Text[3];
        private Button[] slotButtons = new Button[3];

        private string characterSetupScene = "CharacterSetup";
        private string altarHomeScene = "AltarHome";
        private bool isLoadMode = false;

        private void Start()
        {
            // Auto-wire
            mainPanel = Find("MainPanel");
            saveSlotPanel = Find("SaveSlotPanel");
            settingsPanel = Find("SettingsPanel");
            newGameButton = FindBtn("NewGameButton");
            continueButton = FindBtn("ContinueButton");
            settingsButton = FindBtn("SettingsButton");
            backFromSlotsButton = FindBtn("BackButton");
            backFromSettingsButton = FindBtn("BackFromSettings");
            musicVolumeSlider = FindSlider("MusicSlider");
            sfxVolumeSlider = FindSlider("SFXSlider");

            for (int i = 0; i < 3; i++)
            {
                slotButtons[i] = FindBtn($"Slot{i}Button");
                slotLabels[i] = FindTxt($"Slot{i}Button");
            }

            // Setup
            if (mainPanel != null) mainPanel.SetActive(true);
            if (saveSlotPanel != null) saveSlotPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);

            bool hasSaves = false;
            for (int i = 0; i < GameState.MaxSlots; i++)
                if (SaveSystem.Instance != null && SaveSystem.Instance.SlotExists(i)) { hasSaves = true; break; }
            if (continueButton != null) continueButton.interactable = hasSaves;

            newGameButton?.onClick.AddListener(OnNewGame);
            continueButton?.onClick.AddListener(OnContinue);
            settingsButton?.onClick.AddListener(OnSettings);
            backFromSlotsButton?.onClick.AddListener(ShowMain);
            backFromSettingsButton?.onClick.AddListener(ShowMain);

            for (int i = 0; i < slotButtons.Length; i++)
            {
                int slot = i;
                slotButtons[i]?.onClick.AddListener(() => OnSlotSelected(slot));
            }

            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = PlayerPrefs.GetFloat("music_volume", 0.7f);
                musicVolumeSlider.onValueChanged.AddListener(v =>
                {
                    AudioManager.Instance?.SetMusicVolume(v);
                    PlayerPrefs.SetFloat("music_volume", v);
                });
            }
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = PlayerPrefs.GetFloat("sfx_volume", 1f);
                sfxVolumeSlider.onValueChanged.AddListener(v =>
                {
                    AudioManager.Instance?.SetSFXVolume(v);
                    PlayerPrefs.SetFloat("sfx_volume", v);
                });
            }

            AudioManager.Instance?.PlayMusic("title_theme");
        }

        private void OnNewGame() { isLoadMode = false; ShowSlotPanel(); }
        private void OnContinue() { isLoadMode = true; ShowSlotPanel(); }

        private void OnSettings()
        {
            if (mainPanel != null) mainPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(true);
        }

        private void ShowMain()
        {
            if (mainPanel != null) mainPanel.SetActive(true);
            if (saveSlotPanel != null) saveSlotPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        private void ShowSlotPanel()
        {
            if (mainPanel != null) mainPanel.SetActive(false);
            if (saveSlotPanel != null) saveSlotPanel.SetActive(true);

            for (int i = 0; i < 3; i++)
            {
                var data = SaveSystem.Instance?.PeekSlot(i);
                if (slotLabels[i] != null)
                {
                    slotLabels[i].text = data != null
                        ? $"Slot {i + 1}: {data.PlayerName}\nChapter {data.CurrentChapter} | Cycle {data.CycleNumber}"
                        : $"Slot {i + 1}: Empty";
                }
                if (slotButtons[i] != null)
                    slotButtons[i].interactable = !isLoadMode || data != null;
            }
        }

        private void OnSlotSelected(int slot)
        {
            GameState.Instance.CurrentSlot = slot;
            if (isLoadMode)
            {
                SaveSystem.Instance.LoadGame(slot);
                SceneFlowManager.Instance.LoadScene(altarHomeScene);
            }
            else
            {
                GameState.Instance.ResetAll();
                GameState.Instance.CurrentSlot = slot;
                GameState.Instance.RandomizeCharacterVariants();
                SceneFlowManager.Instance.LoadScene(characterSetupScene);
            }
        }
    }
}
