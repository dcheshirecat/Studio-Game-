using System;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessBeloved.Dialogue
{
    /// <summary>
    /// Data structures for the JSON-driven dialogue system.
    /// Dialogue is authored in JSON and parsed at runtime.
    /// </summary>

    [Serializable]
    public class DialogueChapter
    {
        public string chapterId;
        public List<DialogueNode> nodes = new List<DialogueNode>();
    }

    [Serializable]
    public class DialogueNode
    {
        public string id;
        public string type;           // "line", "choice", "end", "card_draw", "minigame", "branch"
        public string speaker;         // character id or "narrator"
        public string text;
        public string portrait;        // expression name
        public string background;      // background image key
        public string music;           // music track key (starts new track)
        public string sfx;             // one-shot sound effect
        public string next;            // next node id (for "line" type)
        public string nextScene;       // scene to load (for "end" type)
        public List<DialogueChoice> choices;  // for "choice" type
        public DialogueBranch branch;  // for "branch" type (conditional)
        public string cardDrawType;    // for "card_draw" type: "random", "forced"
        public string forcedCard;      // specific card to force draw
        public string minigameId;      // for "minigame" type
        public string onWinNext;       // next node if minigame won
        public string onLoseNext;      // next node if minigame lost
        public List<DialogueEffect> effects; // effects to apply when this node plays
    }

    [Serializable]
    public class DialogueChoice
    {
        public string text;
        public string next;            // next node id
        public Dictionary<string, int> affinity; // character_id -> delta
        public List<DialogueEffect> effects;
        public string requiredFlag;    // flag that must be set to show this choice
        public string requiredSpell;   // spell that must be known
        public int requiredAffinity;   // minimum affinity with active route character
        public string requiredAffinityChar; // which character's affinity to check
    }

    [Serializable]
    public class DialogueBranch
    {
        public string flag;            // flag name to check
        public string ifTrue;          // node id if flag is true
        public string ifFalse;         // node id if flag is false
        public string affinityChar;    // alternative: check affinity
        public int affinityThreshold;
        public string ifAbove;         // node if affinity >= threshold
        public string ifBelow;         // node if affinity < threshold
    }

    [Serializable]
    public class DialogueEffect
    {
        public string type;            // "set_flag", "change_affinity", "learn_spell", "unlock_route", "record_card"
        public string target;          // flag name, character id, spell id, etc.
        public string value;           // value to set (int as string for affinity delta)
    }

    /// <summary>
    /// Wrapper for JSON parsing since Unity's JsonUtility doesn't handle dictionaries.
    /// The raw JSON is parsed manually.
    /// </summary>
    [Serializable]
    public class RawDialogueFile
    {
        // The JSON is a dictionary of node_id -> node_data
        // We parse it with a custom parser
    }
}
