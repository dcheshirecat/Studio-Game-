using System;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessBeloved.Characters
{
    /// <summary>
    /// ScriptableObject defining a single romance character.
    /// Create one asset per character in each app.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCharacter", menuName = "Endless Beloved/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [Header("Identity")]
        public string characterId;          // "oracle", "angel", etc.
        public string archetype;            // "The Oracle", "The Angel", etc.
        public string tagline;              // "Foresight & fate"

        [Header("Name Variants")]
        public string feminineName;
        public string masculineName;
        public string nonbinaryName;

        [Header("Portraits")]
        public Sprite defaultPortrait;
        public List<PortraitEntry> portraits = new List<PortraitEntry>();

        [Header("Route")]
        public bool isStarter = true;       // available from the start?
        public string unlockCondition;       // e.g. "complete_oracle_route"
        public int chapterCount = 8;
        public List<string> endingIds = new List<string>();

        [Header("Tarot Association")]
        public string associatedCard;        // card id linked to this character

        [Header("Personality")]
        [TextArea(3, 6)]
        public string shortBio;
        [TextArea(3, 6)]
        public string backstory;
        public List<string> personalityTraits = new List<string>();

        [Header("Dialogue Style")]
        public List<string> commonPhrases = new List<string>();
        public string speechStyle;

        [Header("Romance")]
        public int romanceThreshold = 61;    // affinity needed for romance scenes
        public int belovedThreshold = 81;    // affinity needed for "beloved" tier

        public string GetName(string variant)
        {
            return variant switch
            {
                "feminine" => feminineName,
                "masculine" => masculineName,
                "nonbinary" => nonbinaryName,
                _ => nonbinaryName
            };
        }

        public Sprite GetPortrait(string expression, string variant = "")
        {
            foreach (var entry in portraits)
            {
                if (entry.expression == expression &&
                    (string.IsNullOrEmpty(variant) || entry.variant == variant))
                    return entry.sprite;
            }
            return defaultPortrait;
        }
    }

    [Serializable]
    public class PortraitEntry
    {
        public string expression;   // "neutral", "happy", "sad", "angry", etc.
        public string variant;      // "feminine", "masculine", "nonbinary" or empty for shared
        public Sprite sprite;
    }
}
