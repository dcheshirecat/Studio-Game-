using UnityEngine;
using UnityEngine.UI;
using EndlessBeloved.Core;

namespace EndlessBeloved.UI
{
    /// <summary>
    /// Character setup screen: name entry, pronoun selection, avatar customization.
    /// Shown at the start of a new game.
    /// </summary>
    public class CharacterSetupUI : MonoBehaviour
    {
        [Header("Name Entry")]
        [SerializeField] private InputField nameInput;
        [SerializeField] private Text namePromptText;

        [Header("Pronoun Selection")]
        [SerializeField] private GameObject pronounPanel;
        [SerializeField] private Button theyButton;
        [SerializeField] private Button sheButton;
        [SerializeField] private Button heButton;
        [SerializeField] private Text pronounLabel;

        [Header("Avatar")]
        [SerializeField] private Avatar.AvatarCustomization avatarCustomization;
        [SerializeField] private GameObject avatarPanel;

        [Header("Navigation")]
        [SerializeField] private Button nextButton;
        [SerializeField] private Button backButton;
        [SerializeField] private string nextScene = "DialogueScene";

        private int step = 0; // 0=name, 1=pronouns, 2=avatar

        private void Start()
        {
            namePromptText.text = "The cards ask your name.\nWhat do you tell them?";

            theyButton.onClick.AddListener(() => SelectPronouns("they", "them", "their"));
            sheButton.onClick.AddListener(() => SelectPronouns("she", "her", "her"));
            heButton.onClick.AddListener(() => SelectPronouns("he", "him", "his"));

            nextButton.onClick.AddListener(NextStep);
            backButton?.onClick.AddListener(PrevStep);

            ShowStep(0);
        }

        private void ShowStep(int s)
        {
            step = s;
            nameInput.gameObject.SetActive(s == 0);
            namePromptText.gameObject.SetActive(s == 0);
            pronounPanel.SetActive(s == 1);
            avatarPanel.SetActive(s == 2);
            if (backButton != null)
                backButton.interactable = s > 0;
        }

        private void SelectPronouns(string subject, string obj, string possessive)
        {
            GameState.Instance.SetPronouns(subject, obj, possessive);
            pronounLabel.text = $"{subject}/{obj}/{possessive}";

            // Visual feedback
            theyButton.GetComponent<Image>().color = subject == "they" ? Color.white : new Color(0.6f, 0.6f, 0.6f);
            sheButton.GetComponent<Image>().color = subject == "she" ? Color.white : new Color(0.6f, 0.6f, 0.6f);
            heButton.GetComponent<Image>().color = subject == "he" ? Color.white : new Color(0.6f, 0.6f, 0.6f);
        }

        private void NextStep()
        {
            if (step == 0)
            {
                string name = nameInput.text.Trim();
                if (string.IsNullOrEmpty(name))
                {
                    namePromptText.text = "The cards insist.\nThey need a name.";
                    return;
                }
                GameState.Instance.PlayerName = name;
                ShowStep(1);
            }
            else if (step == 1)
            {
                ShowStep(2);
                avatarCustomization?.Initialize();
            }
            else if (step == 2)
            {
                // Setup complete, start the game
                GameState.Instance.CurrentChapter = 1;
                GameState.Instance.CurrentSceneId = "prologue_01";
                SaveSystem.Instance.SaveGame();
                SceneFlowManager.Instance.LoadScene(nextScene);
            }
        }

        private void PrevStep()
        {
            if (step > 0)
                ShowStep(step - 1);
        }
    }
}
