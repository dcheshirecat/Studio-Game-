using System.Collections.Generic;
using UnityEngine;

namespace EndlessBeloved.Altar
{
    public enum SpellCategory
    {
        CandleMagic,
        SigilCrafting,
        Offerings,
        IntentionSetting,
        TarotSpellwork,
        HerbCombining,
        MoonPhaseRitual,
        ElementalMagic,
        BindingProtection,
        ShadowWork
    }

    /// <summary>
    /// ScriptableObject defining a single spell/practice.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSpell", menuName = "Endless Beloved/Spell Data")]
    public class SpellData : ScriptableObject
    {
        [Header("Identity")]
        public string spellId;
        public string spellName;
        public SpellCategory category;
        [TextArea(2, 4)]
        public string description;

        [Header("Requirements")]
        public int requiredChapter;
        public string requiredFlag;
        public string requiredCard;          // must have drawn this card
        public int requiredAffinity;          // with any character
        public List<string> requiredIngredients = new List<string>();

        [Header("Effects")]
        [TextArea(2, 4)]
        public string storyEffect;           // description of what it does narratively
        public List<SpellEffect> effects = new List<SpellEffect>();

        [Header("Altar Visual")]
        public string altarDecoration;        // decoration id this spell adds
        public Sprite altarVisualOverride;    // visual change on the altar

        [Header("Presentation")]
        public Sprite icon;
        public Color glowColor = Color.white;
        [TextArea(2, 4)]
        public string ritualInstructions;     // flavor text for the casting UI
    }

    [System.Serializable]
    public class SpellEffect
    {
        public string type;     // "unlock_scene", "change_ending", "buff_minigame", "reveal_dialogue", "set_flag", "change_affinity"
        public string target;
        public string value;
    }
}
