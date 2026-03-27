using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EndlessBeloved.Dialogue;
using EndlessBeloved.Characters;

namespace EndlessBeloved.UI
{
    /// <summary>
    /// Visual dialogue box: shows speaker name, text, portrait, and choices.
    /// Listens to DialogueRunner events and renders them.
    /// </summary>
    public class DialogueBoxUI : MonoBehaviour
    {
        [Header("Dialogue Display")]
        [SerializeField] private Text speakerNameText;
        [SerializeField] private Text dialogueText;
        [SerializeField] private Image portraitImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private GameObject tapIndicator; // "tap to continue" icon

        [Header("Choice Display")]
        [SerializeField] private GameObject choicePanel;
        [SerializeField] private Transform choiceButtonParent;
        [SerializeField] private GameObject choiceButtonPrefab;

        [Header("Data")]
        [SerializeField] private CharacterDatabase characterDatabase;

        [Header("Theme")]
        [SerializeField] private Color narratorNameColor = new Color(0.8f, 0.8f, 0.8f);
        [SerializeField] private Color characterNameColor = new Color(1f, 0.84f, 0f); // gold

        private DialogueRunner runner;
        private List<GameObject> spawnedChoiceButtons = new List<GameObject>();

        private void Start()
        {
            runner = DialogueRunner.Instance;
            if (runner == null)
            {
                Debug.LogError("DialogueBoxUI: No DialogueRunner found");
                return;
            }

            runner.OnDialogueLine += HandleDialogueLine;
            runner.OnChoicesPresented += HandleChoices;
            runner.OnBackgroundChanged += HandleBackground;
            runner.OnChapterEnded += HandleChapterEnd;

            choicePanel.SetActive(false);
            if (tapIndicator != null) tapIndicator.SetActive(false);

            if (characterDatabase != null)
                characterDatabase.Initialize();
        }

        private void OnDestroy()
        {
            if (runner != null)
            {
                runner.OnDialogueLine -= HandleDialogueLine;
                runner.OnChoicesPresented -= HandleChoices;
                runner.OnBackgroundChanged -= HandleBackground;
                runner.OnChapterEnded -= HandleChapterEnd;
            }
        }

        private void Update()
        {
            // Tap anywhere to advance (except when choices are showing)
            if (Input.GetMouseButtonDown(0) && !runner.IsWaitingForChoice)
            {
                // Check we're not tapping a UI button
                if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    runner.OnPlayerTap();
            }

            // Show/hide tap indicator
            if (tapIndicator != null)
                tapIndicator.SetActive(!runner.IsTyping && !runner.IsWaitingForChoice);
        }

        private void HandleDialogueLine(string speaker, string text, string portrait)
        {
            dialoguePanel.SetActive(true);
            choicePanel.SetActive(false);

            // Speaker name
            if (speaker == "narrator")
            {
                speakerNameText.text = "";
                speakerNameText.gameObject.SetActive(false);
            }
            else
            {
                string displayName = speaker;
                if (characterDatabase != null)
                {
                    var gs = Core.GameState.Instance;
                    string variant = gs.CharacterVariants.ContainsKey(speaker) ? gs.CharacterVariants[speaker] : "";
                    string charName = characterDatabase.GetCharacterName(speaker, variant);
                    if (!string.IsNullOrEmpty(charName))
                        displayName = charName;
                }
                speakerNameText.text = displayName;
                speakerNameText.color = characterNameColor;
                speakerNameText.gameObject.SetActive(true);
            }

            // Dialogue text
            dialogueText.text = text;

            // Portrait
            if (portraitImage != null)
            {
                if (speaker != "narrator" && characterDatabase != null)
                {
                    var gs = Core.GameState.Instance;
                    string variant = gs.CharacterVariants.ContainsKey(speaker) ? gs.CharacterVariants[speaker] : "";
                    Sprite portraitSprite = characterDatabase.GetPortrait(speaker, portrait ?? "neutral", variant);
                    if (portraitSprite != null)
                    {
                        portraitImage.sprite = portraitSprite;
                        portraitImage.gameObject.SetActive(true);
                    }
                    else
                    {
                        portraitImage.gameObject.SetActive(false);
                    }
                }
                else
                {
                    portraitImage.gameObject.SetActive(false);
                }
            }
        }

        private void HandleChoices(List<DialogueChoice> choices)
        {
            choicePanel.SetActive(true);

            // Clear old buttons
            foreach (var btn in spawnedChoiceButtons)
                Destroy(btn);
            spawnedChoiceButtons.Clear();

            // Spawn choice buttons
            for (int i = 0; i < choices.Count; i++)
            {
                int index = i;
                var go = Instantiate(choiceButtonPrefab, choiceButtonParent);
                go.SetActive(true);

                var btnText = go.GetComponentInChildren<Text>();
                if (btnText != null) btnText.text = choices[i].text;

                var btn = go.GetComponent<Button>();
                if (btn != null) btn.onClick.AddListener(() => OnChoiceClicked(index));

                spawnedChoiceButtons.Add(go);
            }
        }

        private void OnChoiceClicked(int index)
        {
            choicePanel.SetActive(false);
            runner.OnChoiceSelected(index);
            Core.AudioManager.Instance?.PlaySFX("choice_select");
        }

        private void HandleBackground(string backgroundKey)
        {
            if (backgroundImage == null) return;
            var sprite = Resources.Load<Sprite>($"Backgrounds/{backgroundKey}");
            if (sprite != null)
                backgroundImage.sprite = sprite;
        }

        private void HandleChapterEnd()
        {
            dialoguePanel.SetActive(false);
            choicePanel.SetActive(false);
        }
    }
}
