using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EndlessBeloved.Therapeutic
{
    /// <summary>
    /// Achievement-style skill log UI. Shows all unlocked CBT/DBT skills
    /// with both in-game and real-life descriptions.
    /// Always accessible from the journal/menu.
    /// </summary>
    public class SkillLogUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject skillLogPanel;
        [SerializeField] private GameObject detailPanel;

        [Header("List")]
        [SerializeField] private Transform skillListParent;
        [SerializeField] private GameObject skillEntryPrefab;

        [Header("Detail View")]
        [SerializeField] private Text skillNameText;
        [SerializeField] private Text skillTypeText;
        [SerializeField] private Text storyDescText;
        [SerializeField] private Text realLifeDescText;
        [SerializeField] private Text quickTipText;
        [SerializeField] private Image skillIcon;

        [Header("Stats")]
        [SerializeField] private Text progressText;
        [SerializeField] private Slider progressBar;

        [Header("Filter")]
        [SerializeField] private Button allButton;
        [SerializeField] private Button cbtButton;
        [SerializeField] private Button dbtButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button detailCloseButton;

        private List<GameObject> spawnedEntries = new List<GameObject>();
        private SkillType? currentFilter = null;

        private void Start()
        {
            allButton?.onClick.AddListener(() => ShowFiltered(null));
            cbtButton?.onClick.AddListener(() => ShowFiltered(SkillType.CBT));
            dbtButton?.onClick.AddListener(() => ShowFiltered(SkillType.DBT));
            closeButton?.onClick.AddListener(Close);
            detailCloseButton?.onClick.AddListener(() => detailPanel.SetActive(false));

            skillLogPanel.SetActive(false);
            detailPanel.SetActive(false);

            // Listen for new unlocks
            if (SkillSystem.Instance != null)
                SkillSystem.Instance.OnSkillUnlocked += OnNewSkillUnlocked;
        }

        private void OnDestroy()
        {
            if (SkillSystem.Instance != null)
                SkillSystem.Instance.OnSkillUnlocked -= OnNewSkillUnlocked;
        }

        public void Open()
        {
            skillLogPanel.SetActive(true);
            detailPanel.SetActive(false);
            ShowFiltered(currentFilter);
            UpdateProgress();
        }

        public void Close()
        {
            skillLogPanel.SetActive(false);
        }

        private void ShowFiltered(SkillType? filter)
        {
            currentFilter = filter;
            foreach (var go in spawnedEntries) Destroy(go);
            spawnedEntries.Clear();

            if (SkillSystem.Instance == null) return;

            var skills = filter.HasValue
                ? SkillSystem.Instance.GetSkillsByType(filter.Value)
                : SkillSystem.Instance.GetUnlockedSkills();

            foreach (var skill in skills)
            {
                var go = Instantiate(skillEntryPrefab, skillListParent);
                spawnedEntries.Add(go);

                // Entry display
                var nameLabel = go.transform.Find("SkillName")?.GetComponent<Text>();
                if (nameLabel != null) nameLabel.text = skill.skillName;

                var typeLabel = go.transform.Find("SkillType")?.GetComponent<Text>();
                if (typeLabel != null) typeLabel.text = skill.type.ToString();

                var tipLabel = go.transform.Find("QuickTip")?.GetComponent<Text>();
                if (tipLabel != null) tipLabel.text = skill.quickTip;

                var icon = go.transform.Find("Icon")?.GetComponent<Image>();
                if (icon != null && skill.icon != null) icon.sprite = skill.icon;

                var bg = go.GetComponent<Image>();
                if (bg != null) bg.color = new Color(skill.themeColor.r, skill.themeColor.g, skill.themeColor.b, 0.3f);

                // Click to show detail
                var btn = go.GetComponent<Button>();
                if (btn == null) btn = go.AddComponent<Button>();
                var capturedSkill = skill;
                btn.onClick.AddListener(() => ShowDetail(capturedSkill));
            }
        }

        private void ShowDetail(SkillData skill)
        {
            detailPanel.SetActive(true);

            if (skillNameText != null) skillNameText.text = skill.skillName;
            if (skillTypeText != null)
                skillTypeText.text = $"{skill.type} - {skill.category.ToString().Replace("_", " ")}";
            if (storyDescText != null) storyDescText.text = skill.storyDescription;
            if (realLifeDescText != null) realLifeDescText.text = skill.realLifeDescription;
            if (quickTipText != null) quickTipText.text = skill.quickTip;
            if (skillIcon != null && skill.icon != null) skillIcon.sprite = skill.icon;
        }

        private void UpdateProgress()
        {
            if (SkillSystem.Instance == null) return;
            int unlocked = SkillSystem.Instance.UnlockedSkillCount;
            int total = SkillSystem.Instance.TotalSkillCount;

            if (progressText != null)
                progressText.text = $"Skills: {unlocked} / {total}";
            if (progressBar != null)
            {
                progressBar.maxValue = total;
                progressBar.value = unlocked;
            }
        }

        private void OnNewSkillUnlocked(SkillData skill)
        {
            // Could show a notification popup here
            if (skillLogPanel.activeSelf)
            {
                ShowFiltered(currentFilter);
                UpdateProgress();
            }
        }
    }
}
