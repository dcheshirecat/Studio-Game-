using UnityEngine;

namespace EndlessBeloved.Tarot
{
    /// <summary>
    /// ScriptableObject for a single tarot card.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCard", menuName = "Endless Beloved/Card Data")]
    public class CardData : ScriptableObject
    {
        [Header("Identity")]
        public string cardId;
        public string cardName;
        public int arcanaNumber;

        [Header("Meanings")]
        [TextArea(2, 4)]
        public string uprightMeaning;
        [TextArea(2, 4)]
        public string reversedMeaning;

        [Header("Story")]
        public string associatedCharacter;  // character_id or empty
        [TextArea(2, 4)]
        public string storyNote;
        public string unlockCondition;       // e.g. "cycle_2", "affinity_61", "chapter_4"

        [Header("Art")]
        public Sprite cardArt;
        public Sprite cardBack;
    }
}
