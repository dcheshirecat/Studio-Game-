using System;
using System.Collections.Generic;
using UnityEngine;
using EndlessBeloved.Core;

namespace EndlessBeloved.Altar
{
    /// <summary>
    /// Manages the altar scene: decorations, interactive elements, and visual state.
    /// </summary>
    public class AltarManager : MonoBehaviour
    {
        public static AltarManager Instance { get; private set; }

        [Header("Decoration Slots")]
        [SerializeField] private List<AltarSlot> slots = new List<AltarSlot>();
        [SerializeField] private List<AltarDecorationData> allDecorations = new List<AltarDecorationData>();

        public event Action<string> OnDecorationPlaced;
        public event Action OnAltarUpdated;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            RefreshAltar();
        }

        public void RefreshAltar()
        {
            var gs = GameState.Instance;
            foreach (var slot in slots)
            {
                if (slot.renderer != null)
                    slot.renderer.gameObject.SetActive(false);
            }

            foreach (string decoId in gs.AltarDecorations)
            {
                var deco = allDecorations.Find(d => d.decorationId == decoId);
                if (deco == null) continue;

                var slot = slots.Find(s => s.slotId == deco.preferredSlot && !s.isOccupied);
                if (slot == null) slot = slots.Find(s => !s.isOccupied);
                if (slot == null) continue;

                if (slot.renderer != null)
                {
                    slot.renderer.sprite = deco.sprite;
                    slot.renderer.gameObject.SetActive(true);
                    slot.isOccupied = true;
                    slot.currentDecoration = decoId;
                }
            }

            OnAltarUpdated?.Invoke();
        }

        public void PlaceDecoration(string decorationId, string slotId = "")
        {
            var gs = GameState.Instance;
            if (!gs.AltarDecorations.Contains(decorationId))
                gs.AltarDecorations.Add(decorationId);

            RefreshAltar();
            OnDecorationPlaced?.Invoke(decorationId);
        }

        public List<AltarDecorationData> GetOwnedDecorations()
        {
            var gs = GameState.Instance;
            return allDecorations.FindAll(d => gs.AltarDecorations.Contains(d.decorationId));
        }

        public List<AltarDecorationData> GetAvailableForPurchase()
        {
            var gs = GameState.Instance;
            return allDecorations.FindAll(d => !gs.AltarDecorations.Contains(d.decorationId) && d.isPurchasable);
        }
    }

    [Serializable]
    public class AltarSlot
    {
        public string slotId;
        public SpriteRenderer renderer;
        public Vector2 position;
        [HideInInspector] public bool isOccupied;
        [HideInInspector] public string currentDecoration;
    }

    [Serializable]
    public class AltarDecorationData
    {
        public string decorationId;
        public string displayName;
        public Sprite sprite;
        public string preferredSlot;
        public bool isPurchasable;
        public string iapProductId;     // for IAP items
        public string unlockCondition;  // for story-unlocked items
    }
}
