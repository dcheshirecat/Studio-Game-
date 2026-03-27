using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EndlessBeloved.Core;

namespace EndlessBeloved.Dialogue
{
    /// <summary>
    /// Runs a loaded dialogue chapter node-by-node.
    /// Drives the DialogueBoxUI and ChoicePanelUI.
    /// </summary>
    public class DialogueRunner : MonoBehaviour
    {
        public static DialogueRunner Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float typewriterSpeed = 0.03f;
        [SerializeField] private float autoAdvanceDelay = 1.5f;

        private Dictionary<string, DialogueNode> currentNodes;
        private DialogueNode currentNode;
        private bool isTyping = false;
        private bool waitingForInput = false;
        private bool waitingForChoice = false;
        private Coroutine typewriterCoroutine;

        // Events for UI to listen to
        public event Action<string, string, string> OnDialogueLine;    // speaker, text, portrait
        public event Action<List<DialogueChoice>> OnChoicesPresented;
        public event Action OnDialogueCleared;
        public event Action<string> OnBackgroundChanged;
        public event Action<string> OnSpeakerChanged;
        public event Action OnChapterEnded;
        public event Action<string> OnCardDrawTriggered;
        public event Action<string> OnMinigameTriggered;
        public event Action<float> OnTypewriterProgress; // 0-1 progress

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        /// <summary>
        /// Load a chapter from a TextAsset and start at the given node.
        /// </summary>
        public void LoadChapter(TextAsset chapterJson, string startNodeId = "")
        {
            currentNodes = DialogueParser.Parse(chapterJson);
            if (currentNodes.Count == 0)
            {
                Debug.LogError("DialogueRunner: No nodes parsed from chapter");
                return;
            }

            if (string.IsNullOrEmpty(startNodeId))
                startNodeId = GameState.Instance.CurrentSceneId;

            if (!currentNodes.ContainsKey(startNodeId))
            {
                // Try to find first node
                foreach (var key in currentNodes.Keys)
                {
                    startNodeId = key;
                    break;
                }
            }

            AdvanceToNode(startNodeId);
        }

        /// <summary>
        /// Load from raw JSON string.
        /// </summary>
        public void LoadChapter(string json, string startNodeId)
        {
            currentNodes = DialogueParser.Parse(json);
            if (!string.IsNullOrEmpty(startNodeId) && currentNodes.ContainsKey(startNodeId))
                AdvanceToNode(startNodeId);
        }

        /// <summary>
        /// Player tapped the screen -- advance dialogue or complete typewriter.
        /// </summary>
        public void OnPlayerTap()
        {
            if (waitingForChoice) return; // Must pick a choice

            if (isTyping)
            {
                // Complete the typewriter immediately
                CompleteTypewriter();
                return;
            }

            if (waitingForInput && currentNode != null)
            {
                // Advance to next node
                if (!string.IsNullOrEmpty(currentNode.next))
                    AdvanceToNode(currentNode.next);
            }
        }

        /// <summary>
        /// Player selected a choice.
        /// </summary>
        public void OnChoiceSelected(int index)
        {
            if (!waitingForChoice || currentNode?.choices == null) return;
            if (index < 0 || index >= currentNode.choices.Count) return;

            var choice = currentNode.choices[index];

            // Apply affinity changes
            if (choice.affinity != null)
            {
                foreach (var kvp in choice.affinity)
                    GameState.Instance.ChangeAffinity(kvp.Key, kvp.Value);
            }

            // Apply effects
            ApplyEffects(choice.effects);

            waitingForChoice = false;

            if (!string.IsNullOrEmpty(choice.next))
                AdvanceToNode(choice.next);
        }

        private void AdvanceToNode(string nodeId)
        {
            if (!currentNodes.ContainsKey(nodeId))
            {
                Debug.LogWarning($"DialogueRunner: Node '{nodeId}' not found");
                OnChapterEnded?.Invoke();
                return;
            }

            currentNode = currentNodes[nodeId];
            GameState.Instance.CurrentSceneId = nodeId;

            // Apply node effects
            ApplyEffects(currentNode.effects);

            // Handle music/sfx
            if (!string.IsNullOrEmpty(currentNode.music))
                AudioManager.Instance?.PlayMusic(currentNode.music);
            if (!string.IsNullOrEmpty(currentNode.sfx))
                AudioManager.Instance?.PlaySFX(currentNode.sfx);

            // Handle background change
            if (!string.IsNullOrEmpty(currentNode.background))
                OnBackgroundChanged?.Invoke(currentNode.background);

            switch (currentNode.type)
            {
                case "line":
                    ShowLine();
                    break;

                case "choice":
                    ShowChoices();
                    break;

                case "end":
                    HandleEnd();
                    break;

                case "branch":
                    HandleBranch();
                    break;

                case "card_draw":
                    HandleCardDraw();
                    break;

                case "minigame":
                    HandleMinigame();
                    break;

                default:
                    ShowLine();
                    break;
            }
        }

        private void ShowLine()
        {
            waitingForInput = false;
            waitingForChoice = false;

            string parsedText = GameState.Instance.ParseDialogue(currentNode.text);
            string speaker = currentNode.speaker ?? "narrator";

            OnSpeakerChanged?.Invoke(speaker);
            OnDialogueLine?.Invoke(speaker, "", currentNode.portrait);

            // Start typewriter
            if (typewriterCoroutine != null)
                StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = StartCoroutine(TypewriterEffect(speaker, parsedText, currentNode.portrait));
        }

        private IEnumerator TypewriterEffect(string speaker, string fullText, string portrait)
        {
            isTyping = true;
            string displayText = "";

            for (int i = 0; i < fullText.Length; i++)
            {
                displayText += fullText[i];
                OnDialogueLine?.Invoke(speaker, displayText, portrait);
                OnTypewriterProgress?.Invoke((float)(i + 1) / fullText.Length);

                // Skip whitespace delays
                if (fullText[i] == ' ') continue;

                // Pause longer on punctuation
                float delay = typewriterSpeed;
                if (".,;:!?".IndexOf(fullText[i]) >= 0)
                    delay *= 3f;
                if (fullText[i] == '\n')
                    delay *= 2f;

                yield return new WaitForSeconds(delay);
            }

            isTyping = false;
            waitingForInput = true;
            typewriterCoroutine = null;
        }

        private void CompleteTypewriter()
        {
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }
            isTyping = false;
            waitingForInput = true;

            string parsedText = GameState.Instance.ParseDialogue(currentNode.text);
            OnDialogueLine?.Invoke(currentNode.speaker, parsedText, currentNode.portrait);
            OnTypewriterProgress?.Invoke(1f);
        }

        private void ShowChoices()
        {
            // First show the line text if present
            if (!string.IsNullOrEmpty(currentNode.text))
            {
                string parsedText = GameState.Instance.ParseDialogue(currentNode.text);
                OnDialogueLine?.Invoke(currentNode.speaker, parsedText, currentNode.portrait);
            }

            // Filter choices by requirements
            var available = new List<DialogueChoice>();
            foreach (var choice in currentNode.choices)
            {
                if (!string.IsNullOrEmpty(choice.requiredFlag) &&
                    !GameState.Instance.HasFlag(choice.requiredFlag))
                    continue;
                if (!string.IsNullOrEmpty(choice.requiredSpell) &&
                    !GameState.Instance.KnowsSpell(choice.requiredSpell))
                    continue;
                if (!string.IsNullOrEmpty(choice.requiredAffinityChar) &&
                    choice.requiredAffinity > 0 &&
                    GameState.Instance.GetAffinity(choice.requiredAffinityChar) < choice.requiredAffinity)
                    continue;
                available.Add(choice);
            }

            waitingForChoice = true;
            OnChoicesPresented?.Invoke(available);
        }

        private void HandleEnd()
        {
            if (!string.IsNullOrEmpty(currentNode.nextScene))
            {
                SceneFlowManager.Instance?.LoadScene(currentNode.nextScene);
            }
            else
            {
                OnChapterEnded?.Invoke();
            }
        }

        private void HandleBranch()
        {
            var b = currentNode.branch;
            if (b == null)
            {
                if (!string.IsNullOrEmpty(currentNode.next))
                    AdvanceToNode(currentNode.next);
                return;
            }

            // Flag-based branch
            if (!string.IsNullOrEmpty(b.flag))
            {
                bool flagVal = GameState.Instance.HasFlag(b.flag);
                AdvanceToNode(flagVal ? b.ifTrue : b.ifFalse);
                return;
            }

            // Affinity-based branch
            if (!string.IsNullOrEmpty(b.affinityChar))
            {
                int aff = GameState.Instance.GetAffinity(b.affinityChar);
                AdvanceToNode(aff >= b.affinityThreshold ? b.ifAbove : b.ifBelow);
                return;
            }

            // Fallback
            if (!string.IsNullOrEmpty(currentNode.next))
                AdvanceToNode(currentNode.next);
        }

        private void HandleCardDraw()
        {
            OnCardDrawTriggered?.Invoke(currentNode.forcedCard);
            // After card draw UI completes, it should call ContinueAfterCardDraw()
        }

        public void ContinueAfterCardDraw(string drawnCardId)
        {
            GameState.Instance.RecordCardDraw(drawnCardId);
            if (!string.IsNullOrEmpty(currentNode.next))
                AdvanceToNode(currentNode.next);
        }

        private void HandleMinigame()
        {
            OnMinigameTriggered?.Invoke(currentNode.minigameId);
            // After minigame, call ContinueAfterMinigame(won)
        }

        public void ContinueAfterMinigame(bool won)
        {
            string next = won ? currentNode.onWinNext : currentNode.onLoseNext;
            if (string.IsNullOrEmpty(next)) next = currentNode.next;
            if (!string.IsNullOrEmpty(next))
                AdvanceToNode(next);
        }

        private void ApplyEffects(List<DialogueEffect> effects)
        {
            if (effects == null) return;
            var gs = GameState.Instance;

            foreach (var eff in effects)
            {
                switch (eff.type)
                {
                    case "set_flag":
                        gs.SetFlag(eff.target, eff.value ?? "true");
                        break;
                    case "change_affinity":
                        if (int.TryParse(eff.value, out int delta))
                            gs.ChangeAffinity(eff.target, delta);
                        break;
                    case "learn_spell":
                        gs.LearnSpell(eff.target);
                        break;
                    case "unlock_route":
                        gs.UnlockRoute(eff.target);
                        break;
                    case "record_card":
                        gs.RecordCardDraw(eff.target);
                        break;
                }
            }
        }

        // ── Getters ─────────────────────────────────────────────────────

        public bool IsWaitingForChoice => waitingForChoice;
        public bool IsTyping => isTyping;
        public DialogueNode CurrentNode => currentNode;

        public void SetTypewriterSpeed(float charsPerSecond)
        {
            typewriterSpeed = 1f / Mathf.Max(charsPerSecond, 1f);
        }
    }
}
