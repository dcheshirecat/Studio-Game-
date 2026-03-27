using System.Collections.Generic;
using System;
using UnityEngine;
using EndlessBeloved.Core;

namespace EndlessBeloved.Altar
{
    /// <summary>
    /// Manages spell casting, ingredient tracking, and spell effects on game state.
    /// </summary>
    public class SpellSystem : MonoBehaviour
    {
        public static SpellSystem Instance { get; private set; }

        [SerializeField] private List<SpellData> allSpells = new List<SpellData>();

        public event Action<SpellData> OnSpellCast;
        public event Action<SpellData> OnSpellUnlocked;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public List<SpellData> GetAvailableSpells()
        {
            var gs = GameState.Instance;
            var available = new List<SpellData>();

            foreach (var spell in allSpells)
            {
                if (CanCastSpell(spell))
                    available.Add(spell);
            }
            return available;
        }

        public List<SpellData> GetLearnedSpells()
        {
            var gs = GameState.Instance;
            return allSpells.FindAll(s => gs.KnowsSpell(s.spellId));
        }

        public List<SpellData> GetSpellsByCategory(SpellCategory category)
        {
            return allSpells.FindAll(s => s.category == category);
        }

        public bool CanCastSpell(SpellData spell)
        {
            var gs = GameState.Instance;

            if (spell.requiredChapter > 0 && gs.CurrentChapter < spell.requiredChapter)
                return false;
            if (!string.IsNullOrEmpty(spell.requiredFlag) && !gs.HasFlag(spell.requiredFlag))
                return false;
            if (!string.IsNullOrEmpty(spell.requiredCard) && !gs.CardsUnlocked.Contains(spell.requiredCard))
                return false;
            if (spell.requiredAffinity > 0)
            {
                bool hasEnough = false;
                foreach (var kvp in gs.Affinity)
                    if (kvp.Value >= spell.requiredAffinity) { hasEnough = true; break; }
                if (!hasEnough) return false;
            }

            return true;
        }

        public void CastSpell(SpellData spell)
        {
            if (!CanCastSpell(spell))
            {
                Debug.LogWarning($"Cannot cast spell: {spell.spellId}");
                return;
            }

            var gs = GameState.Instance;
            gs.LearnSpell(spell.spellId);

            // Apply effects
            foreach (var effect in spell.effects)
            {
                switch (effect.type)
                {
                    case "set_flag":
                        gs.SetFlag(effect.target, effect.value);
                        break;
                    case "change_affinity":
                        if (int.TryParse(effect.value, out int delta))
                            gs.ChangeAffinity(effect.target, delta);
                        break;
                    case "unlock_route":
                        gs.UnlockRoute(effect.target);
                        break;
                    case "unlock_scene":
                    case "change_ending":
                    case "reveal_dialogue":
                        gs.SetFlag($"spell_{spell.spellId}_{effect.type}", effect.target);
                        break;
                    case "buff_minigame":
                        gs.SetFlag($"buff_{effect.target}", effect.value);
                        break;
                }
            }

            // Add altar decoration
            if (!string.IsNullOrEmpty(spell.altarDecoration) &&
                !gs.AltarDecorations.Contains(spell.altarDecoration))
            {
                gs.AltarDecorations.Add(spell.altarDecoration);
            }

            OnSpellCast?.Invoke(spell);
        }

        public SpellData GetSpellById(string spellId)
        {
            return allSpells.Find(s => s.spellId == spellId);
        }
    }
}
