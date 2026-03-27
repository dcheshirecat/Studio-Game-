using UnityEngine;

namespace EndlessBeloved.Therapeutic
{
    public enum SkillType
    {
        CBT,    // Cognitive Behavioral Therapy
        DBT     // Dialectical Behavior Therapy
    }

    public enum SkillCategory
    {
        // CBT categories
        CognitiveRestructuring,
        BehavioralActivation,
        ExposureTherapy,
        ProblemSolving,

        // DBT categories
        Mindfulness,
        DistressTolerance,
        EmotionRegulation,
        InterpersonalEffectiveness
    }

    /// <summary>
    /// ScriptableObject for a single CBT/DBT skill.
    /// Unlocked through story moments, displayed as achievements.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSkill", menuName = "Endless Beloved/Therapeutic Skill")]
    public class SkillData : ScriptableObject
    {
        [Header("Identity")]
        public string skillId;
        public string skillName;
        public SkillType type;
        public SkillCategory category;

        [Header("In-Game Description")]
        [TextArea(3, 6)]
        public string storyDescription;     // how the character learned it

        [Header("Real-Life Description")]
        [TextArea(3, 8)]
        public string realLifeDescription;  // how the player can use it
        [TextArea(2, 4)]
        public string quickTip;             // short actionable summary

        [Header("Unlock")]
        public string unlockFlag;           // story flag that triggers unlock
        public string unlockScene;          // which scene/moment unlocks it
        public string associatedCharacter;  // which character teaches it

        [Header("Presentation")]
        public Sprite icon;
        public Color themeColor = new Color(0.4f, 0.7f, 0.5f); // healing green
    }
}
