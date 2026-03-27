using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessBeloved.Core
{
    /// <summary>
    /// Handles save/load to JSON files. Supports multiple save slots
    /// and auto-save at chapter transitions.
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }

        private string SaveDirectory => Path.Combine(Application.persistentDataPath, "saves");

        public event Action<int> OnGameSaved;
        public event Action<int> OnGameLoaded;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Directory.CreateDirectory(SaveDirectory);
        }

        public bool SaveGame(int slot = -1)
        {
            if (slot < 0) slot = GameState.Instance.CurrentSlot;
            var gs = GameState.Instance;

            var data = new SaveData
            {
                PlayerName = gs.PlayerName,
                PronounSubject = gs.PronounSubject,
                PronounObject = gs.PronounObject,
                PronounPossessive = gs.PronounPossessive,
                Avatar = gs.PlayerAvatar,
                CurrentChapter = gs.CurrentChapter,
                CurrentSceneId = gs.CurrentSceneId,
                CycleNumber = gs.CycleNumber,
                CompletedEndings = gs.CompletedEndings,
                ActiveRoute = gs.ActiveRoute,
                UnlockedRoutes = gs.UnlockedRoutes,
                AltarDecorations = gs.AltarDecorations,
                LearnedSpells = gs.LearnedSpells,
                CardsDrawn = gs.CardsDrawn,
                CardsUnlocked = gs.CardsUnlocked,
                JudgementAppearances = gs.JudgementAppearances,
                SaveTimestamp = DateTime.Now.ToString("o")
            };

            // Serialize dictionaries as lists of pairs
            data.AffinityPairs = DictToList(gs.Affinity);
            data.VariantPairs = DictToList(gs.CharacterVariants);
            data.FlagPairs = new List<StringObjectPair>();
            foreach (var kvp in gs.StoryFlags)
                data.FlagPairs.Add(new StringObjectPair { Key = kvp.Key, Value = kvp.Value?.ToString() ?? "" });

            string json = JsonUtility.ToJson(data, true);
            string path = GetSlotPath(slot);

            try
            {
                File.WriteAllText(path, json);
                OnGameSaved?.Invoke(slot);
                Debug.Log($"Game saved to slot {slot}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Save failed: {e.Message}");
                return false;
            }
        }

        public bool LoadGame(int slot = -1)
        {
            if (slot < 0) slot = GameState.Instance.CurrentSlot;
            string path = GetSlotPath(slot);

            if (!File.Exists(path))
            {
                Debug.LogWarning($"No save file at slot {slot}");
                return false;
            }

            try
            {
                string json = File.ReadAllText(path);
                var data = JsonUtility.FromJson<SaveData>(json);
                var gs = GameState.Instance;

                gs.PlayerName = data.PlayerName;
                gs.PronounSubject = data.PronounSubject;
                gs.PronounObject = data.PronounObject;
                gs.PronounPossessive = data.PronounPossessive;
                gs.PlayerAvatar = data.Avatar ?? new AvatarData();
                gs.CurrentChapter = data.CurrentChapter;
                gs.CurrentSceneId = data.CurrentSceneId;
                gs.CycleNumber = data.CycleNumber;
                gs.CompletedEndings = data.CompletedEndings ?? new List<string>();
                gs.ActiveRoute = data.ActiveRoute;
                gs.UnlockedRoutes = data.UnlockedRoutes ?? new List<string> { "oracle", "angel", "keeper" };
                gs.AltarDecorations = data.AltarDecorations ?? new List<string>();
                gs.LearnedSpells = data.LearnedSpells ?? new List<string>();
                gs.CardsDrawn = data.CardsDrawn ?? new List<string>();
                gs.CardsUnlocked = data.CardsUnlocked ?? new List<string>();
                gs.JudgementAppearances = data.JudgementAppearances;
                gs.CurrentSlot = slot;

                // Restore dictionaries
                if (data.AffinityPairs != null)
                    foreach (var p in data.AffinityPairs)
                        gs.Affinity[p.Key] = int.Parse(p.Value);
                if (data.VariantPairs != null)
                    foreach (var p in data.VariantPairs)
                        gs.CharacterVariants[p.Key] = p.Value;
                if (data.FlagPairs != null)
                    foreach (var p in data.FlagPairs)
                        gs.StoryFlags[p.Key] = p.Value;

                OnGameLoaded?.Invoke(slot);
                Debug.Log($"Game loaded from slot {slot}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Load failed: {e.Message}");
                return false;
            }
        }

        public bool SlotExists(int slot)
        {
            return File.Exists(GetSlotPath(slot));
        }

        public SaveData PeekSlot(int slot)
        {
            string path = GetSlotPath(slot);
            if (!File.Exists(path)) return null;
            try
            {
                return JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
            }
            catch { return null; }
        }

        public void DeleteSlot(int slot)
        {
            string path = GetSlotPath(slot);
            if (File.Exists(path)) File.Delete(path);
        }

        public void AutoSave()
        {
            SaveGame(GameState.Instance.CurrentSlot);
        }

        private string GetSlotPath(int slot) => Path.Combine(SaveDirectory, $"slot_{slot}.json");

        private List<StringObjectPair> DictToList<T>(Dictionary<string, T> dict)
        {
            var list = new List<StringObjectPair>();
            foreach (var kvp in dict)
                list.Add(new StringObjectPair { Key = kvp.Key, Value = kvp.Value?.ToString() ?? "" });
            return list;
        }
    }

    [Serializable]
    public class SaveData
    {
        public string PlayerName;
        public string PronounSubject;
        public string PronounObject;
        public string PronounPossessive;
        public AvatarData Avatar;
        public int CurrentChapter;
        public string CurrentSceneId;
        public int CycleNumber;
        public List<string> CompletedEndings;
        public string ActiveRoute;
        public List<string> UnlockedRoutes;
        public List<string> AltarDecorations;
        public List<string> LearnedSpells;
        public List<string> CardsDrawn;
        public List<string> CardsUnlocked;
        public int JudgementAppearances;
        public string SaveTimestamp;
        public List<StringObjectPair> AffinityPairs;
        public List<StringObjectPair> VariantPairs;
        public List<StringObjectPair> FlagPairs;
    }

    [Serializable]
    public class StringObjectPair
    {
        public string Key;
        public string Value;
    }
}
