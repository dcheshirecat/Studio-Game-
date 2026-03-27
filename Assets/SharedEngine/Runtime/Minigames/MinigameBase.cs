using System;
using UnityEngine;
using EndlessBeloved.Core;

namespace EndlessBeloved.Minigames
{
    /// <summary>
    /// Abstract base class for all minigames. Subclass this for each minigame type.
    /// Handles scoring, time limits, and result reporting.
    /// </summary>
    public abstract class MinigameBase : MonoBehaviour
    {
        [Header("Minigame Settings")]
        public string minigameId;
        public string displayName;
        public float timeLimit = 60f;
        public int scoreToWin = 100;
        public string associatedCharacter; // affects this character's affinity

        protected int currentScore = 0;
        protected float timeRemaining;
        protected bool isPlaying = false;
        protected bool hasWon = false;

        public event Action<bool, int> OnMinigameComplete;  // won, score
        public event Action<int> OnScoreChanged;
        public event Action<float> OnTimeChanged;

        public virtual void StartMinigame()
        {
            currentScore = 0;
            timeRemaining = timeLimit;
            isPlaying = true;
            hasWon = false;

            // Check for spell buffs
            var gs = GameState.Instance;
            if (gs.HasFlag($"buff_{minigameId}"))
            {
                // Buff: reduce score needed or add time
                scoreToWin = Mathf.Max(scoreToWin - 20, 10);
                timeRemaining += 15f;
            }

            OnGameStart();
        }

        protected virtual void Update()
        {
            if (!isPlaying) return;

            if (timeLimit > 0)
            {
                timeRemaining -= Time.deltaTime;
                OnTimeChanged?.Invoke(timeRemaining);

                if (timeRemaining <= 0)
                {
                    timeRemaining = 0;
                    EndMinigame(currentScore >= scoreToWin);
                }
            }
        }

        protected void AddScore(int points)
        {
            currentScore += points;
            OnScoreChanged?.Invoke(currentScore);

            if (currentScore >= scoreToWin && timeLimit <= 0)
                EndMinigame(true);
        }

        protected void EndMinigame(bool won)
        {
            isPlaying = false;
            hasWon = won;

            // Apply affinity change based on result
            if (!string.IsNullOrEmpty(associatedCharacter))
            {
                int affinityDelta = won ? 5 : -2;
                GameState.Instance.ChangeAffinity(associatedCharacter, affinityDelta);
            }

            OnGameEnd(won);
            OnMinigameComplete?.Invoke(won, currentScore);
        }

        // ── Override these in subclasses ─────────────────────────────

        protected abstract void OnGameStart();
        protected abstract void OnGameEnd(bool won);

        // ── Getters ─────────────────────────────────────────────────

        public bool IsPlaying => isPlaying;
        public int CurrentScore => currentScore;
        public float TimeRemaining => timeRemaining;
        public bool HasWon => hasWon;
    }
}
