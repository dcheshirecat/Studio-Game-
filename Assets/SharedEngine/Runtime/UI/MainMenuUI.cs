using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EndlessBeloved.Core;

namespace EndlessBeloved.UI
{
    /// <summary>
    /// Title screen / main menu. Handles new game, continue, settings.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject saveSlotPanel;
        [SerializeField] private GameObject settingsPanel;

        [Header("Main Buttons")]
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button settingsButton;

        [Header("Save Slots")]
        [SerializeField] private List<Button> slotButtons = new List<Button>();
        [SerializeField] private List<Text> slotLabels = new List<Text>();
        [SerializeField] private Button backFromSlotsButton;

        [Header("Settings")]
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider textSpeedSlider;
        [SerializeField] private Button backFromSettingsButton;

        [Header("Scenes")]
        [SerializeField] private string characterSetupScene = "CharacterSetup";
        [SerializeField] private string altarHomeScene = "AltarHome";

        private bool isLoadMode = false;

        private void Start()
        {
            mainPanel.SetActive(true);
            saveSlotPanel.SetActive(false);
            settingsPanel.SetActive(false);

            // Check if any saves exist
            bool hasSaves = false;
            for (int i = 0; i < GameState.MaxSlots; i++)
            {
                if (SaveSystem.Instance.SlotExists(i)) { hasSaves = true; break; }
            }
            continueButton.interactable = hasSaves;

            newGameButton.onClick.AddListener(OnNewGame);
            continueButton.onClick.AddListener(OnContinue);
            settingsButton.onClick.AddListener(OnSettings);
            backFromSlotsButton?.onClick.AddListener(ShowMain);
            backFromSettingsButton?.onClick.AddListener(ShowMain);

            for (int i = 0; i < slotButtons.Count; i++)
            {
                int slot = i;
                slotButtons[i].onClick.AddListener(() => OnSlotSelected(slot));
            }

            // Load saved settings
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
            if (textSpeedSlider != null)
            {
                textSpeedSlider.value = PlayerPrefs.GetFloat("text_speed", 30f);
                textSpeedSlider.onValueChanged.AddListener(v =>
                {
                    PlayerPrefs.SetFloat("text_speed", v);
                });
            }
        }

        private void OnNewGame()
        {
            isLoadMode = false;
            ShowSlotPanel();
        }

        private void OnContinue()
        {
            isLoadMode = true;
            ShowSlotPanel();
        }

        private void OnSettings()
        {
            mainPanel.SetActive(false);
            settingsPanel.SetActive(true);
        }

        private void ShowMain()
        {
            mainPanel.SetActive(true);
            saveSlotPanel.SetActive(false);
            settingsPanel.SetActive(false);
        }

        private void ShowSlotPanel()
        {
            mainPanel.SetActive(false);
            saveSlotPanel.SetActive(true);

            for (int i = 0; i < slotButtons.Count && i < GameState.MaxSlots; i++)
            {
                var data = SaveSystem.Instance.PeekSlot(i);
                if (data != null)
                {
                    slotLabels[i].text = $"Slot {i + 1}: {data.PlayerName}\n" +
                        $"Chapter {data.CurrentChapter} | Cycle {data.CycleNumber}\n" +
                        $"{data.SaveTimestamp}";
                    slotButtons[i].interactable = true;
                }
                else
                {
                    slotLabels[i].text = $"Slot {i + 1}: Empty";
                    slotButtons[i].interactable = !isLoadMode; // Can't load empty slot
                }
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
