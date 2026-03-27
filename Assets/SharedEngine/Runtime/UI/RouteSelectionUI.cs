using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EndlessBeloved.Core;
using EndlessBeloved.Characters;

namespace EndlessBeloved.UI
{
    /// <summary>
    /// Route selection screen: pick which character route to play.
    /// Shows unlocked and locked routes.
    /// </summary>
    public class RouteSelectionUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterDatabase characterDatabase;
        [SerializeField] private Transform routeListParent;
        [SerializeField] private GameObject routeEntryPrefab;
        [SerializeField] private Button backButton;
        [SerializeField] private string dialogueScene = "DialogueScene";
        [SerializeField] private string altarScene = "AltarHome";

        [Header("Route Entry Layout")]
        [SerializeField] private Color unlockedColor = Color.white;
        [SerializeField] private Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

        private List<GameObject> spawnedEntries = new List<GameObject>();

        private void Start()
        {
            backButton?.onClick.AddListener(() => SceneFlowManager.Instance.LoadScene(altarScene));
            Refresh();
        }

        public void Refresh()
        {
            foreach (var go in spawnedEntries) Destroy(go);
            spawnedEntries.Clear();

            if (characterDatabase == null) return;
            characterDatabase.Initialize();

            var gs = GameState.Instance;

            foreach (var character in characterDatabase.characters)
            {
                var go = Instantiate(routeEntryPrefab, routeListParent);
                spawnedEntries.Add(go);

                bool unlocked = character.isStarter || gs.IsRouteUnlocked(character.characterId);
                string variant = gs.CharacterVariants.ContainsKey(character.characterId)
                    ? gs.CharacterVariants[character.characterId] : "nonbinary";

                // Portrait
                var img = go.transform.Find("Portrait")?.GetComponent<Image>();
                if (img != null)
                {
                    img.sprite = character.GetPortrait("neutral", variant);
                    img.color = unlocked ? unlockedColor : lockedColor;
                }

                // Name
                var nameText = go.transform.Find("NameText")?.GetComponent<Text>();
                if (nameText != null)
                {
                    nameText.text = unlocked
                        ? $"{character.archetype}\n{character.GetName(variant)}"
                        : $"{character.archetype}\n[Locked]";
                }

                // Tagline
                var tagText = go.transform.Find("TaglineText")?.GetComponent<Text>();
                if (tagText != null)
                    tagText.text = unlocked ? character.tagline : "???";

                // Affinity bar
                var affBar = go.transform.Find("AffinityBar")?.GetComponent<Slider>();
                if (affBar != null)
                {
                    affBar.maxValue = 100;
                    affBar.value = unlocked ? gs.GetAffinity(character.characterId) : 0;
                    affBar.gameObject.SetActive(unlocked);
                }

                // Button
                var btn = go.GetComponent<Button>();
                if (btn != null)
                {
                    btn.interactable = unlocked;
                    string charId = character.characterId;
                    btn.onClick.AddListener(() => SelectRoute(charId));
                }
            }
        }

        private void SelectRoute(string characterId)
        {
            var gs = GameState.Instance;
            gs.ActiveRoute = characterId;
            gs.CurrentSceneId = $"{characterId}_ch{gs.CurrentChapter}_01";
            SaveSystem.Instance.SaveGame();
            SceneFlowManager.Instance.LoadScene(dialogueScene);
        }
    }
}
