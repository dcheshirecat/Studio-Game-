using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EndlessBeloved.Core;

namespace EndlessBeloved.UI
{
    /// <summary>
    /// Character setup: name, pronouns, avatar. Auto-wires by name.
    /// </summary>
    public class CharacterSetupUI : AutoWireUI
    {
        private TMP_InputField nameInput;
        private TMP_Text namePromptText;
        private GameObject pronounPanel;
        private Button theyButton, sheButton, heButton;
        private TMP_Text pronounLabel;
        private GameObject avatarPanel;
        private Button nextButton, backButton;
        private string nextScene = "DialogueScene";
        private int step = 0;

        private void Start()
        {
            nameInput = FindInput("NameInput");
            namePromptText = FindTxt("NamePromptText");
            pronounPanel = Find("PronounPanel");
            theyButton = FindBtn("TheyButton");
            sheButton = FindBtn("SheButton");
            heButton = FindBtn("HeButton");
            pronounLabel = FindTxt("PronounLabel");
            avatarPanel = Find("AvatarPanel");
            nextButton = FindBtn("NextButton");
            backButton = FindBtn("BackButton");

            if (namePromptText != null)
                namePromptText.text = "The cards ask your name.\nWhat do you tell them?";

            theyButton?.onClick.AddListener(() => SelectPronouns("they", "them", "their"));
            sheButton?.onClick.AddListener(() => SelectPronouns("she", "her", "her"));
            heButton?.onClick.AddListener(() => SelectPronouns("he", "him", "his"));
            nextButton?.onClick.AddListener(NextStep);
            backButton?.onClick.AddListener(PrevStep);

            ShowStep(0);
        }

        private void ShowStep(int s)
        {
            step = s;
            if (nameInput != null) nameInput.gameObject.SetActive(s == 0);
            if (namePromptText != null) namePromptText.gameObject.SetActive(s == 0);
            if (pronounPanel != null) pronounPanel.SetActive(s == 1);
            if (avatarPanel != null) avatarPanel.SetActive(s == 2);
            if (backButton != null) backButton.interactable = s > 0;
        }

        private void SelectPronouns(string subject, string obj, string possessive)
        {
            GameState.Instance.SetPronouns(subject, obj, possessive);
            if (pronounLabel != null) pronounLabel.text = $"{subject}/{obj}/{possessive}";
        }

        private void NextStep()
        {
            if (step == 0)
            {
                string name = nameInput != null ? nameInput.text.Trim() : "";
                if (string.IsNullOrEmpty(name))
                {
                    if (namePromptText != null)
                        namePromptText.text = "The cards insist.\nThey need a name.";
                    return;
                }
                GameState.Instance.PlayerName = name;
                ShowStep(1);
            }
            else if (step == 1)
            {
                ShowStep(2);
            }
            else if (step == 2)
            {
                GameState.Instance.CurrentChapter = 1;
                GameState.Instance.CurrentSceneId = "prologue_01";
                SaveSystem.Instance?.SaveGame();
                SceneFlowManager.Instance.LoadScene(nextScene);
            }
        }

        private void PrevStep()
        {
            if (step > 0) ShowStep(step - 1);
        }
    }
}
