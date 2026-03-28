using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EndlessBeloved.Dialogue;
using EndlessBeloved.Core;

namespace EndlessBeloved.UI
{
    /// <summary>
    /// Dialogue display UI. Auto-wires by name. Also loads chapter JSON automatically.
    /// </summary>
    public class DialogueBoxUI : AutoWireUI
    {
        private TMP_Text speakerNameText;
        private TMP_Text dialogueText;
        private Image portraitImage;
        private Image backgroundImage;
        private GameObject dialoguePanel;
        private GameObject tapIndicator;
        private GameObject choicePanel;
        private GameObject choiceButtonPrefab;

        private DialogueRunner runner;
        private List<GameObject> spawnedChoiceButtons = new List<GameObject>();

        private void Start()
        {
            // Auto-wire
            speakerNameText = FindTxt("SpeakerNameText");
            dialogueText = FindTxt("DialogueText");
            portraitImage = FindImg("PortraitImage");
            backgroundImage = FindImg("BackgroundImage");
            dialoguePanel = Find("DialoguePanel");
            tapIndicator = Find("TapIndicator");
            choicePanel = Find("ChoicePanel");
            choiceButtonPrefab = Find("ChoiceButtonPrefab");

            // Setup runner
            runner = FindObjectOfType<DialogueRunner>();
            if (runner == null)
            {
                var go = new GameObject("DialogueRunner");
                runner = go.AddComponent<DialogueRunner>();
            }

            runner.OnDialogueLine += HandleDialogueLine;
            runner.OnChoicesPresented += HandleChoices;
            runner.OnBackgroundChanged += HandleBackground;
            runner.OnChapterEnded += HandleChapterEnd;

            if (choicePanel != null) choicePanel.SetActive(false);
            if (tapIndicator != null) tapIndicator.SetActive(false);

            // Auto-load chapter based on game state
            LoadCurrentChapter();
        }

        private void LoadCurrentChapter()
        {
            var gs = GameState.Instance;
            string route = gs.ActiveRoute;
            if (string.IsNullOrEmpty(route)) route = "oracle";
            int chapter = gs.CurrentChapter > 0 ? gs.CurrentChapter : 1;

            // Try to load chapter JSON from Resources/Story/
            string[] pathsToTry = new[]
            {
                $"Story/{route}_ch{chapter}",
                $"Story/{route}_ch1",
                $"Story/oracle_ch1"
            };

            TextAsset jsonAsset = null;
            foreach (string path in pathsToTry)
            {
                jsonAsset = Resources.Load<TextAsset>(path);
                if (jsonAsset != null)
                {
                    Debug.Log($"DialogueBoxUI: Loaded story from {path}");
                    break;
                }
            }

            if (jsonAsset != null)
            {
                runner.LoadChapter(jsonAsset, gs.CurrentSceneId);
            }
            else
            {
                Debug.LogError($"Could not load chapter JSON for route={route} chapter={chapter}. Checked: {string.Join(", ", pathsToTry)}");
            }
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
            if (runner == null) return;

            // Tap to advance
            if (Input.GetMouseButtonDown(0) && !runner.IsWaitingForChoice)
            {
                if (UnityEngine.EventSystems.EventSystem.current == null ||
                    !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    runner.OnPlayerTap();
            }

            if (tapIndicator != null)
                tapIndicator.SetActive(!runner.IsTyping && !runner.IsWaitingForChoice);
        }

        private void HandleDialogueLine(string speaker, string text, string portrait)
        {
            if (dialoguePanel != null) dialoguePanel.SetActive(true);
            if (choicePanel != null) choicePanel.SetActive(false);

            if (speakerNameText != null)
            {
                if (speaker == "narrator")
                {
                    speakerNameText.text = "";
                    speakerNameText.gameObject.SetActive(false);
                }
                else
                {
                    var gs = GameState.Instance;
                    string variant = gs.CharacterVariants.ContainsKey(speaker)
                        ? gs.CharacterVariants[speaker] : "";
                    speakerNameText.text = speaker.ToUpper();
                    speakerNameText.color = new Color(1f, 0.84f, 0f);
                    speakerNameText.gameObject.SetActive(true);
                }
            }

            if (dialogueText != null)
                dialogueText.text = text;

            if (portraitImage != null)
                portraitImage.gameObject.SetActive(speaker != "narrator");
        }

        private void HandleChoices(List<DialogueChoice> choices)
        {
            if (choicePanel != null) choicePanel.SetActive(true);

            foreach (var btn in spawnedChoiceButtons) Destroy(btn);
            spawnedChoiceButtons.Clear();

            for (int i = 0; i < choices.Count; i++)
            {
                int index = i;
                GameObject go;

                if (choiceButtonPrefab != null)
                {
                    go = Instantiate(choiceButtonPrefab, choicePanel.transform);
                    go.SetActive(true);
                }
                else
                {
                    go = new GameObject($"Choice_{i}");
                    go.transform.SetParent(choicePanel.transform, false);
                    var rect = go.AddComponent<RectTransform>();
                    rect.anchorMin = new Vector2(0, 1f - (i + 1) * 0.25f);
                    rect.anchorMax = new Vector2(1, 1f - i * 0.25f);
                    rect.offsetMin = Vector2.zero;
                    rect.offsetMax = Vector2.zero;
                    var img = go.AddComponent<Image>();
                    img.color = new Color(0.15f, 0.08f, 0.25f, 0.9f);
                    go.AddComponent<Button>();
                    var textGo = new GameObject("Text");
                    textGo.transform.SetParent(go.transform, false);
                    var textRect = textGo.AddComponent<RectTransform>();
                    textRect.anchorMin = Vector2.zero;
                    textRect.anchorMax = Vector2.one;
                    textRect.offsetMin = new Vector2(10, 5);
                    textRect.offsetMax = new Vector2(-10, -5);
                    var t = textGo.AddComponent<Text>();
                    t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                    t.alignment = TextAnchor.MiddleLeft;
                    t.color = Color.white;
                    t.fontSize = 20;
                }

                var btnText = go.GetComponentInChildren<Text>();
                if (btnText != null) btnText.text = choices[i].text;

                var btn = go.GetComponent<Button>();
                btn?.onClick.AddListener(() => OnChoiceClicked(index));

                spawnedChoiceButtons.Add(go);
            }
        }

        private void OnChoiceClicked(int index)
        {
            if (choicePanel != null) choicePanel.SetActive(false);
            runner.OnChoiceSelected(index);
            AudioManager.Instance?.PlaySFX("choice_select");
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
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            if (choicePanel != null) choicePanel.SetActive(false);
        }
    }
}
