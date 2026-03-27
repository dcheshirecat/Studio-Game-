using System.Collections.Generic;
using UnityEngine;

namespace EndlessBeloved.Characters
{
    /// <summary>
    /// ScriptableObject that holds references to all character data assets.
    /// One instance per app, populated in the inspector.
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Endless Beloved/Character Database")]
    public class CharacterDatabase : ScriptableObject
    {
        public List<CharacterData> characters = new List<CharacterData>();

        private Dictionary<string, CharacterData> _lookup;

        public void Initialize()
        {
            _lookup = new Dictionary<string, CharacterData>();
            foreach (var c in characters)
            {
                if (c != null && !string.IsNullOrEmpty(c.characterId))
                    _lookup[c.characterId] = c;
            }
        }

        public CharacterData GetCharacter(string characterId)
        {
            if (_lookup == null) Initialize();
            return _lookup.ContainsKey(characterId) ? _lookup[characterId] : null;
        }

        public string GetCharacterName(string characterId, string variant)
        {
            var c = GetCharacter(characterId);
            return c != null ? c.GetName(variant) : characterId;
        }

        public Sprite GetPortrait(string characterId, string expression, string variant = "")
        {
            var c = GetCharacter(characterId);
            return c?.GetPortrait(expression, variant);
        }

        public List<CharacterData> GetStarterCharacters()
        {
            return characters.FindAll(c => c.isStarter);
        }

        public List<CharacterData> GetUnlockableCharacters()
        {
            return characters.FindAll(c => !c.isStarter);
        }

        public List<CharacterData> GetAllUnlocked()
        {
            var gs = Core.GameState.Instance;
            return characters.FindAll(c => c.isStarter || gs.IsRouteUnlocked(c.characterId));
        }
    }
}
