using System;
using System.Collections.Generic;
using UnityEngine;
using EndlessBeloved.Core;

namespace EndlessBeloved.Tarot
{
    /// <summary>
    /// Manages the tarot deck: card draws, daily readings, and unlock conditions.
    /// </summary>
    [CreateAssetMenu(fileName = "TarotDeck", menuName = "Endless Beloved/Tarot Deck")]
    public class TarotDeck : ScriptableObject
    {
        public List<CardData> allCards = new List<CardData>();

        /// <summary>
        /// Get all cards currently available based on game state.
        /// </summary>
        public List<CardData> GetAvailableCards()
        {
            var gs = GameState.Instance;
            var available = new List<CardData>();

            foreach (var card in allCards)
            {
                if (IsCardAvailable(card, gs))
                    available.Add(card);
            }
            return available;
        }

        /// <summary>
        /// Draw a random card from the available pool. Returns (card, isReversed).
        /// </summary>
        public (CardData card, bool reversed) DrawRandom()
        {
            var available = GetAvailableCards();
            if (available.Count == 0)
            {
                Debug.LogWarning("TarotDeck: No cards available to draw");
                return (null, false);
            }

            var card = available[UnityEngine.Random.Range(0, available.Count)];
            bool reversed = UnityEngine.Random.value < 0.35f; // 35% chance reversed

            string drawId = reversed ? $"{card.cardId}_reversed" : card.cardId;
            GameState.Instance.RecordCardDraw(drawId);

            return (card, reversed);
        }

        /// <summary>
        /// Draw a specific card (forced by story).
        /// </summary>
        public (CardData card, bool reversed) DrawSpecific(string cardId, bool forceReversed = false)
        {
            var card = allCards.Find(c => c.cardId == cardId);
            if (card == null)
            {
                Debug.LogWarning($"TarotDeck: Card '{cardId}' not found");
                return (null, false);
            }

            bool reversed = forceReversed || UnityEngine.Random.value < 0.35f;
            string drawId = reversed ? $"{card.cardId}_reversed" : card.cardId;
            GameState.Instance.RecordCardDraw(drawId);

            return (card, reversed);
        }

        /// <summary>
        /// Perform a daily reading (3-card spread: past, present, future).
        /// </summary>
        public List<(CardData card, bool reversed, string position)> DailyReading()
        {
            var available = GetAvailableCards();
            var reading = new List<(CardData, bool, string)>();
            string[] positions = { "Past", "Present", "Future" };
            var used = new HashSet<int>();

            for (int i = 0; i < 3 && i < available.Count; i++)
            {
                int idx;
                do { idx = UnityEngine.Random.Range(0, available.Count); }
                while (used.Contains(idx));
                used.Add(idx);

                bool reversed = UnityEngine.Random.value < 0.35f;
                reading.Add((available[idx], reversed, positions[i]));

                string drawId = reversed ? $"{available[idx].cardId}_reversed" : available[idx].cardId;
                GameState.Instance.RecordCardDraw(drawId);
            }

            return reading;
        }

        private bool IsCardAvailable(CardData card, GameState gs)
        {
            if (string.IsNullOrEmpty(card.unlockCondition))
                return true;

            string cond = card.unlockCondition;

            // Cycle conditions: "cycle_2", "cycle_3"
            if (cond.StartsWith("cycle_"))
            {
                if (int.TryParse(cond.Substring(6), out int reqCycle))
                    return gs.CycleNumber >= reqCycle;
            }

            // Affinity conditions: "affinity_61", "affinity_angel_40"
            if (cond.StartsWith("affinity_"))
            {
                string rest = cond.Substring(9);
                // Check if it's character-specific
                foreach (var kvp in gs.Affinity)
                {
                    if (rest.StartsWith(kvp.Key + "_"))
                    {
                        if (int.TryParse(rest.Substring(kvp.Key.Length + 1), out int threshold))
                            return kvp.Value >= threshold;
                    }
                }
                // Generic: any character at threshold
                if (int.TryParse(rest, out int genericThreshold))
                {
                    foreach (var kvp in gs.Affinity)
                        if (kvp.Value >= genericThreshold) return true;
                    return false;
                }
            }

            // Chapter conditions: "chapter_4"
            if (cond.StartsWith("chapter_"))
            {
                if (int.TryParse(cond.Substring(8), out int reqChapter))
                    return gs.CurrentChapter >= reqChapter;
            }

            // Special conditions
            if (cond == "after_tower")
                return gs.CardsDrawn.Exists(c => c.StartsWith("tower"));

            // Flag conditions
            if (cond.EndsWith("_trigger"))
                return gs.HasFlag(cond);

            return gs.HasFlag(cond);
        }

        public CardData GetCardById(string cardId)
        {
            return allCards.Find(c => c.cardId == cardId);
        }
    }
}
