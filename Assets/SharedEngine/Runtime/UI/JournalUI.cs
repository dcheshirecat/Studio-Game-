using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EndlessBeloved.Core;
using EndlessBeloved.Tarot;

namespace EndlessBeloved.UI
{
    /// <summary>
    /// Journal / lore tracker UI. Shows story progress, collected cards,
    /// character affinity, and learned spells.
    /// </summary>
    public class JournalUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject journalPanel;
        [SerializeField] private GameObject progressTab;
        [SerializeField] private GameObject cardsTab;
        [SerializeField] private GameObject characterTab;
        [SerializeField] private GameObject spellsTab;

        [Header("Tab Buttons")]
        [SerializeField] private Button progressTabBtn;
        [SerializeField] private Button cardsTabBtn;
        [SerializeField] private Button characterTabBtn;
        [SerializeField] private Button spellsTabBtn;
        [SerializeField] private Button closeButton;

        [Header("Progress")]
        [SerializeField] private TMP_Text chapterText;
        [SerializeField] private TMP_Text cycleText;
        [SerializeField] private TMP_Text endingsText;

        [Header("Cards")]
        [SerializeField] private Transform cardGridParent;
        [SerializeField] private GameObject cardEntryPrefab;
        [SerializeField] private TarotDeck tarotDeck;

        [Header("Characters")]
        [SerializeField] private Transform characterListParent;
        [SerializeField] private GameObject characterEntryPrefab;

        [Header("Spells")]
        [SerializeField] private Transform spellListParent;
        [SerializeField] private GameObject spellEntryPrefab;

        private void Start()
        {
            progressTabBtn?.onClick.AddListener(() => ShowTab(progressTab));
            cardsTabBtn?.onClick.AddListener(() => ShowTab(cardsTab));
            characterTabBtn?.onClick.AddListener(() => ShowTab(characterTab));
            spellsTabBtn?.onClick.AddListener(() => ShowTab(spellsTab));
            closeButton?.onClick.AddListener(Close);

            journalPanel.SetActive(false);
        }

        public void Open()
        {
            journalPanel.SetActive(true);
            RefreshAll();
            ShowTab(progressTab);
        }

        public void Close()
        {
            journalPanel.SetActive(false);
        }

        private void ShowTab(GameObject tab)
        {
            progressTab.SetActive(tab == progressTab);
            cardsTab.SetActive(tab == cardsTab);
            characterTab.SetActive(tab == characterTab);
            spellsTab.SetActive(tab == spellsTab);
        }

        private void RefreshAll()
        {
            var gs = GameState.Instance;

            // Progress
            if (chapterText != null)
                chapterText.text = $"Chapter {gs.CurrentChapter}";
            if (cycleText != null)
                cycleText.text = $"Cycle {gs.CycleNumber}";
            if (endingsText != null)
                endingsText.text = $"Endings found: {gs.CompletedEndings.Count}";

            // Cards
            RefreshCards();

            // Characters
            RefreshCharacters();

            // Spells
            RefreshSpells();
        }

        private void RefreshCards()
        {
            if (cardGridParent == null || cardEntryPrefab == null || tarotDeck == null) return;

            // Clear
            foreach (Transform child in cardGridParent)
                Destroy(child.gameObject);

            var gs = GameState.Instance;
            foreach (var card in tarotDeck.allCards)
            {
                var go = Instantiate(cardEntryPrefab, cardGridParent);
                bool unlocked = gs.CardsUnlocked.Contains(card.cardId);

                var img = go.GetComponentInChildren<Image>();
                if (img != null)
                {
                    img.sprite = unlocked ? card.cardArt : card.cardBack;
                    img.color = unlocked ? Color.white : new Color(0.3f, 0.3f, 0.3f);
                }

                var label = go.GetComponentInChildren<Text>();
                if (label != null)
                    label.text = unlocked ? card.cardName : "???";
            }
        }

        private void RefreshCharacters()
        {
            if (characterListParent == null || characterEntryPrefab == null) return;

            foreach (Transform child in characterListParent)
                Destroy(child.gameObject);

            var gs = GameState.Instance;
            string[] charIds = { "oracle", "angel", "keeper", "wanderer", "apprentice", "weaver" };

            foreach (string id in charIds)
            {
                var go = Instantiate(characterEntryPrefab, characterListParent);
                int aff = gs.GetAffinity(id);
                string tier = gs.GetAffinityTier(id);
                bool unlocked = gs.IsRouteUnlocked(id);

                var label = go.GetComponentInChildren<Text>();
                if (label != null)
                {
                    if (unlocked)
                        label.text = $"{id.ToUpper()}\nAffinity: {aff}/100 ({tier})";
                    else
                        label.text = $"{id.ToUpper()}\n[LOCKED]";
                }
            }
        }

        private void RefreshSpells()
        {
            if (spellListParent == null || spellEntryPrefab == null) return;

            foreach (Transform child in spellListParent)
                Destroy(child.gameObject);

            var gs = GameState.Instance;
            foreach (string spellId in gs.LearnedSpells)
            {
                var go = Instantiate(spellEntryPrefab, spellListParent);
                var label = go.GetComponentInChildren<Text>();
                if (label != null)
                    label.text = spellId.Replace("_", " ");
            }
        }
    }
}
