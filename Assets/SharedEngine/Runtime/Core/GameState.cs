using System;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessBeloved.Core
{
    /// <summary>
    /// Central game state singleton. Holds all persistent data: player info,
    /// story progress, affinity scores, cards drawn, flags, and cycle state.
    /// Accessed globally via GameState.Instance.
    /// </summary>
    public class GameState : MonoBehaviour
    {
        public static GameState Instance { get; private set; }

        // ── Player ──────────────────────────────────────────────────────
        [Header("Player")]
        public string PlayerName = "";
        public string PronounSubject = "they";
        public string PronounObject = "them";
        public string PronounPossessive = "their";
        public AvatarData PlayerAvatar = new AvatarData();

        // ── Story Progress ──────────────────────────────────────────────
        [Header("Story Progress")]
        public int CurrentChapter = 0;
        public string CurrentSceneId = "prologue_01";
        public int CycleNumber = 1;
        public List<string> CompletedEndings = new List<string>();
        public Dictionary<string, object> StoryFlags = new Dictionary<string, object>();

        // ── Affinity ────────────────────────────────────────────────────
        [Header("Affinity")]
        public Dictionary<string, int> Affinity = new Dictionary<string, int>
        {
            { "oracle", 0 },
            { "angel", 0 },
            { "keeper", 0 },
            { "wanderer", 0 },
            { "apprentice", 0 },
            { "weaver", 0 }
        };

        // ── Character Variants ──────────────────────────────────────────
        [Header("Character Variants")]
        public Dictionary<string, string> CharacterVariants = new Dictionary<string, string>
        {
            { "oracle", "feminine" },
            { "angel", "masculine" },
            { "keeper", "nonbinary" },
            { "wanderer", "feminine" },
            { "apprentice", "masculine" },
            { "weaver", "nonbinary" }
        };

        // ── Tarot ───────────────────────────────────────────────────────
        [Header("Tarot")]
        public List<string> CardsDrawn = new List<string>();
        public List<string> CardsUnlocked = new List<string>();
        public int JudgementAppearances = 0;

        // ── Routes ──────────────────────────────────────────────────────
        [Header("Routes")]
        public List<string> UnlockedRoutes = new List<string> { "oracle", "angel", "keeper" };
        public string ActiveRoute = "";

        // ── Altar ───────────────────────────────────────────────────────
        [Header("Altar")]
        public List<string> AltarDecorations = new List<string>();
        public List<string> LearnedSpells = new List<string>();

        // ── Save ────────────────────────────────────────────────────────
        [Header("Save")]
        public int CurrentSlot = 0;
        public const int MaxSlots = 3;

        // ── Events ──────────────────────────────────────────────────────
        public event Action<string, int> OnAffinityChanged;
        public event Action<int> OnChapterChanged;
        public event Action<string, object> OnFlagSet;
        public event Action<int> OnCycleStarted;
        public event Action<string> OnRouteUnlocked;
        public event Action<string> OnSpellLearned;

        // ─────────────────────────────────────────────────────────────────

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // ── Affinity ────────────────────────────────────────────────────

        public void ChangeAffinity(string characterId, int delta)
        {
            if (!Affinity.ContainsKey(characterId))
            {
                Debug.LogWarning($"Unknown character_id: {characterId}");
                return;
            }
            Affinity[characterId] = Mathf.Clamp(Affinity[characterId] + delta, 0, 100);
            OnAffinityChanged?.Invoke(characterId, Affinity[characterId]);
        }

        public int GetAffinity(string characterId)
        {
            return Affinity.ContainsKey(characterId) ? Affinity[characterId] : 0;
        }

        public string GetAffinityTier(string characterId)
        {
            int score = GetAffinity(characterId);
            if (score <= 30) return "guarded";
            if (score <= 60) return "opening";
            if (score <= 80) return "intimate";
            return "beloved";
        }

        // ── Story Flags ─────────────────────────────────────────────────

        public void SetFlag(string flagName, object value = null)
        {
            value ??= true;
            StoryFlags[flagName] = value;
            OnFlagSet?.Invoke(flagName, value);
        }

        public object GetFlag(string flagName, object defaultValue = null)
        {
            return StoryFlags.ContainsKey(flagName) ? StoryFlags[flagName] : defaultValue ?? false;
        }

        public bool HasFlag(string flagName)
        {
            return StoryFlags.ContainsKey(flagName) && StoryFlags[flagName] is bool b && b;
        }

        // ── Tarot ───────────────────────────────────────────────────────

        public void RecordCardDraw(string cardId)
        {
            CardsDrawn.Add(cardId);
            if (!CardsUnlocked.Contains(cardId))
                CardsUnlocked.Add(cardId);
            if (cardId == "judgement" || cardId == "judgement_reversed")
                JudgementAppearances++;
        }

        // ── Pronouns ────────────────────────────────────────────────────

        public void SetPronouns(string subject, string obj, string possessive)
        {
            PronounSubject = subject;
            PronounObject = obj;
            PronounPossessive = possessive;
        }

        /// <summary>
        /// Replaces {they}, {them}, {their}, {name}, and character name tokens in text.
        /// </summary>
        public string ParseDialogue(string text, CharacterDatabase charDb = null)
        {
            text = text.Replace("{they}", PronounSubject);
            text = text.Replace("{them}", PronounObject);
            text = text.Replace("{their}", PronounPossessive);
            text = text.Replace("{name}", PlayerName);
            text = text.Replace("{They}", CapFirst(PronounSubject));
            text = text.Replace("{Them}", CapFirst(PronounObject));
            text = text.Replace("{Their}", CapFirst(PronounPossessive));

            if (charDb != null)
            {
                foreach (var kvp in CharacterVariants)
                {
                    string charName = charDb.GetCharacterName(kvp.Key, kvp.Value);
                    text = text.Replace($"{{{kvp.Key}}}", charName);
                }
            }
            return text;
        }

        private string CapFirst(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        // ── Routes ──────────────────────────────────────────────────────

        public bool IsRouteUnlocked(string routeId)
        {
            return UnlockedRoutes.Contains(routeId);
        }

        public void UnlockRoute(string routeId)
        {
            if (!UnlockedRoutes.Contains(routeId))
            {
                UnlockedRoutes.Add(routeId);
                OnRouteUnlocked?.Invoke(routeId);
            }
        }

        // ── Spells ──────────────────────────────────────────────────────

        public void LearnSpell(string spellId)
        {
            if (!LearnedSpells.Contains(spellId))
            {
                LearnedSpells.Add(spellId);
                OnSpellLearned?.Invoke(spellId);
            }
        }

        public bool KnowsSpell(string spellId)
        {
            return LearnedSpells.Contains(spellId);
        }

        // ── Character Variants ──────────────────────────────────────────

        public void RandomizeCharacterVariants()
        {
            string[] options = { "feminine", "masculine", "nonbinary" };
            var keys = new List<string>(CharacterVariants.Keys);
            foreach (string key in keys)
            {
                CharacterVariants[key] = options[UnityEngine.Random.Range(0, options.Length)];
            }
        }

        // ── Cycle ───────────────────────────────────────────────────────

        public void StartNewCycle()
        {
            CycleNumber++;
            CurrentChapter = 0;
            CurrentSceneId = "prologue_01";
            // Preserve: CompletedEndings, CardsUnlocked, UnlockedRoutes, LearnedSpells
            // Reset: Affinity, StoryFlags, CardsDrawn
            var keys = new List<string>(Affinity.Keys);
            foreach (string key in keys)
                Affinity[key] = 0;
            StoryFlags.Clear();
            CardsDrawn.Clear();
            JudgementAppearances = 0;
            OnCycleStarted?.Invoke(CycleNumber);
        }

        // ── Reset ───────────────────────────────────────────────────────

        public void ResetAll()
        {
            PlayerName = "";
            PronounSubject = "they";
            PronounObject = "them";
            PronounPossessive = "their";
            PlayerAvatar = new AvatarData();
            CurrentChapter = 0;
            CurrentSceneId = "prologue_01";
            CycleNumber = 1;
            CompletedEndings.Clear();
            StoryFlags.Clear();
            var keys = new List<string>(Affinity.Keys);
            foreach (string key in keys)
                Affinity[key] = 0;
            CharacterVariants = new Dictionary<string, string>
            {
                { "oracle", "feminine" }, { "angel", "masculine" }, { "keeper", "nonbinary" },
                { "wanderer", "feminine" }, { "apprentice", "masculine" }, { "weaver", "nonbinary" }
            };
            CardsDrawn.Clear();
            CardsUnlocked.Clear();
            JudgementAppearances = 0;
            UnlockedRoutes = new List<string> { "oracle", "angel", "keeper" };
            ActiveRoute = "";
            AltarDecorations.Clear();
            LearnedSpells.Clear();
        }
    }

    [Serializable]
    public class AvatarData
    {
        public int SkinTone = 0;
        public int HairStyle = 0;
        public int HairColor = 0;
        public int EyeColor = 0;
        public int Outfit = 0;
        public int Accessory = 0;
    }
}
