using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EndlessBeloved.Core;

namespace EndlessBeloved.UI
{
    /// <summary>
    /// Route selection screen. Auto-wires by name. Creates route buttons dynamically.
    /// </summary>
    public class RouteSelectionUI : AutoWireUI
    {
        private Transform routeListParent;
        private Button backButton;
        private string dialogueScene = "DialogueScene";
        private string altarScene = "AltarHome";

        private readonly string[] archetypes = { "oracle", "angel", "keeper", "wanderer", "apprentice", "weaver" };
        private readonly string[] archetypeNames = { "The Oracle", "The Angel", "The Keeper", "The Wanderer", "The Apprentice", "The Weaver" };
        private readonly string[] taglines = { "Foresight & fate", "Grace & ruin", "Memory & loss", "Freedom & longing", "Power & becoming", "Threads & endings" };

        private List<GameObject> spawnedEntries = new List<GameObject>();

        private void Start()
        {
            routeListParent = Find("RouteList")?.transform;
            backButton = FindBtn("BackButton");

            backButton?.onClick.AddListener(() => SceneFlowManager.Instance.LoadScene(altarScene));

            BuildRouteList();
        }

        private void BuildRouteList()
        {
            foreach (var go in spawnedEntries) Destroy(go);
            spawnedEntries.Clear();

            var gs = GameState.Instance;
            Transform parent = routeListParent ?? transform;

            for (int i = 0; i < archetypes.Length; i++)
            {
                string charId = archetypes[i];
                bool unlocked = gs.IsRouteUnlocked(charId);
                int affinity = gs.GetAffinity(charId);
                string tier = gs.GetAffinityTier(charId);

                // Create route entry
                var entry = new GameObject($"Route_{charId}");
                entry.transform.SetParent(parent, false);
                var rect = entry.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(0, 160);

                var img = entry.AddComponent<Image>();
                img.color = unlocked
                    ? new Color(0.15f, 0.08f, 0.25f, 0.8f)
                    : new Color(0.1f, 0.1f, 0.1f, 0.5f);

                // Name text
                var nameGo = new GameObject("NameText");
                nameGo.transform.SetParent(entry.transform, false);
                var nameRect = nameGo.AddComponent<RectTransform>();
                nameRect.anchorMin = new Vector2(0.05f, 0.5f);
                nameRect.anchorMax = new Vector2(0.7f, 0.9f);
                nameRect.offsetMin = Vector2.zero;
                nameRect.offsetMax = Vector2.zero;
                var nameText = nameGo.AddComponent<Text>();
                nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                nameText.fontSize = 26;
                nameText.color = unlocked ? Color.white : new Color(0.4f, 0.4f, 0.4f);
                nameText.text = unlocked ? archetypeNames[i] : $"{archetypeNames[i]} [LOCKED]";
                nameText.alignment = TextAnchor.MiddleLeft;

                // Tagline
                var tagGo = new GameObject("TaglineText");
                tagGo.transform.SetParent(entry.transform, false);
                var tagRect = tagGo.AddComponent<RectTransform>();
                tagRect.anchorMin = new Vector2(0.05f, 0.1f);
                tagRect.anchorMax = new Vector2(0.7f, 0.5f);
                tagRect.offsetMin = Vector2.zero;
                tagRect.offsetMax = Vector2.zero;
                var tagText = tagGo.AddComponent<Text>();
                tagText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                tagText.fontSize = 18;
                tagText.color = unlocked ? new Color(0.7f, 0.6f, 0.8f) : new Color(0.3f, 0.3f, 0.3f);
                tagText.text = unlocked ? $"{taglines[i]}\nAffinity: {affinity}/100 ({tier})" : "???";
                tagText.alignment = TextAnchor.MiddleLeft;

                // Button
                var btn = entry.AddComponent<Button>();
                btn.targetGraphic = img;
                btn.interactable = unlocked;
                string id = charId;
                btn.onClick.AddListener(() => SelectRoute(id));

                spawnedEntries.Add(entry);
            }

            // Add vertical layout
            var layout = parent.gameObject.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                layout = parent.gameObject.AddComponent<VerticalLayoutGroup>();
                layout.spacing = 15;
                layout.childForceExpandWidth = true;
                layout.childForceExpandHeight = false;
                layout.padding = new RectOffset(10, 10, 10, 10);
            }

            var fitter = parent.gameObject.GetComponent<ContentSizeFitter>();
            if (fitter == null)
            {
                fitter = parent.gameObject.AddComponent<ContentSizeFitter>();
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
        }

        private void SelectRoute(string characterId)
        {
            var gs = GameState.Instance;
            gs.ActiveRoute = characterId;
            gs.CurrentSceneId = "oracle_ch1_01"; // Default to first story node
            SaveSystem.Instance?.SaveGame();
            SceneFlowManager.Instance.LoadScene(dialogueScene);
        }
    }
}
