using System;
using System.Collections.Generic;
using UnityEngine;
using EndlessBeloved.Core;

namespace EndlessBeloved.Therapeutic
{
    /// <summary>
    /// Manages therapeutic skill unlocks. Listens for story flags
    /// and unlocks corresponding CBT/DBT skills.
    /// Only active in the Heal version of the app.
    /// </summary>
    public class SkillSystem : MonoBehaviour
    {
        public static SkillSystem Instance { get; private set; }

        [SerializeField] private List<SkillData> allSkills = new List<SkillData>();
        [SerializeField] private bool isEnabled = true; // disable for dark fantasy version

        private List<string> unlockedSkillIds = new List<string>();

        public event Action<SkillData> OnSkillUnlocked;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            if (!isEnabled) return;

            // Listen for flag changes to auto-unlock skills
            if (GameState.Instance != null)
                GameState.Instance.OnFlagSet += CheckForSkillUnlock;

            // Load previously unlocked skills
            LoadUnlockedSkills();
        }

        private void OnDestroy()
        {
            if (GameState.Instance != null)
                GameState.Instance.OnFlagSet -= CheckForSkillUnlock;
        }

        private void CheckForSkillUnlock(string flagName, object value)
        {
            if (!isEnabled) return;

            foreach (var skill in allSkills)
            {
                if (skill.unlockFlag == flagName && !unlockedSkillIds.Contains(skill.skillId))
                {
                    UnlockSkill(skill);
                }
            }
        }

        public void UnlockSkill(SkillData skill)
        {
            if (unlockedSkillIds.Contains(skill.skillId)) return;

            unlockedSkillIds.Add(skill.skillId);
            SaveUnlockedSkills();
            OnSkillUnlocked?.Invoke(skill);
            Debug.Log($"Skill unlocked: {skill.skillName}");
        }

        public List<SkillData> GetUnlockedSkills()
        {
            return allSkills.FindAll(s => unlockedSkillIds.Contains(s.skillId));
        }

        public List<SkillData> GetSkillsByType(SkillType type)
        {
            return GetUnlockedSkills().FindAll(s => s.type == type);
        }

        public List<SkillData> GetSkillsByCategory(SkillCategory category)
        {
            return GetUnlockedSkills().FindAll(s => s.category == category);
        }

        public bool IsSkillUnlocked(string skillId)
        {
            return unlockedSkillIds.Contains(skillId);
        }

        public int TotalSkillCount => allSkills.Count;
        public int UnlockedSkillCount => unlockedSkillIds.Count;

        // ── Persistence ─────────────────────────────────────────────

        private void SaveUnlockedSkills()
        {
            string json = string.Join(",", unlockedSkillIds);
            PlayerPrefs.SetString("unlocked_skills", json);
            PlayerPrefs.Save();
        }

        private void LoadUnlockedSkills()
        {
            string saved = PlayerPrefs.GetString("unlocked_skills", "");
            if (!string.IsNullOrEmpty(saved))
            {
                unlockedSkillIds = new List<string>(saved.Split(','));
            }
        }
    }
}
