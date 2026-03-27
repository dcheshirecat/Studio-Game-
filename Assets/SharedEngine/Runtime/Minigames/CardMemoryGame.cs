using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EndlessBeloved.Minigames
{
    /// <summary>
    /// Memory match minigame: flip cards to find matching pairs.
    /// Associated with the Oracle character.
    /// </summary>
    public class CardMemoryGame : MinigameBase
    {
        [Header("Memory Game")]
        [SerializeField] private Transform gridParent;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private int gridSize = 4; // 4x4 = 8 pairs
        [SerializeField] private List<Sprite> cardFaces = new List<Sprite>();
        [SerializeField] private Sprite cardBackSprite;

        private List<MemoryCard> cards = new List<MemoryCard>();
        private MemoryCard firstFlipped;
        private MemoryCard secondFlipped;
        private int matchesFound = 0;
        private int totalPairs;
        private bool canFlip = true;

        protected override void OnGameStart()
        {
            matchesFound = 0;
            firstFlipped = null;
            secondFlipped = null;
            canFlip = true;
            GenerateGrid();
        }

        protected override void OnGameEnd(bool won)
        {
            canFlip = false;
        }

        private void GenerateGrid()
        {
            // Clear existing
            foreach (var card in cards)
            {
                if (card != null && card.gameObject != null)
                    Destroy(card.gameObject);
            }
            cards.Clear();

            totalPairs = (gridSize * gridSize) / 2;
            var faceIndices = new List<int>();

            // Create pairs
            for (int i = 0; i < totalPairs; i++)
            {
                int faceIdx = i % cardFaces.Count;
                faceIndices.Add(faceIdx);
                faceIndices.Add(faceIdx);
            }

            // Shuffle
            for (int i = faceIndices.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (faceIndices[i], faceIndices[j]) = (faceIndices[j], faceIndices[i]);
            }

            // Create card objects
            for (int i = 0; i < faceIndices.Count; i++)
            {
                if (cardPrefab == null || gridParent == null) break;

                var go = Instantiate(cardPrefab, gridParent);
                var mc = go.GetComponent<MemoryCard>();
                if (mc == null) mc = go.AddComponent<MemoryCard>();

                mc.Initialize(faceIndices[i],
                    cardFaces.Count > 0 ? cardFaces[faceIndices[i]] : null,
                    cardBackSprite);
                mc.OnCardClicked += HandleCardClicked;
                cards.Add(mc);
            }
        }

        private void HandleCardClicked(MemoryCard card)
        {
            if (!canFlip || !isPlaying) return;
            if (card.IsMatched || card.IsFlipped) return;

            card.Flip();

            if (firstFlipped == null)
            {
                firstFlipped = card;
            }
            else if (secondFlipped == null)
            {
                secondFlipped = card;
                canFlip = false;
                StartCoroutine(CheckMatch());
            }
        }

        private IEnumerator CheckMatch()
        {
            yield return new WaitForSeconds(0.8f);

            if (firstFlipped.FaceIndex == secondFlipped.FaceIndex)
            {
                // Match found
                firstFlipped.SetMatched();
                secondFlipped.SetMatched();
                matchesFound++;
                AddScore(25);

                Core.AudioManager.Instance?.PlaySFX("card_match");

                if (matchesFound >= totalPairs)
                    EndMinigame(true);
            }
            else
            {
                // No match -- flip back
                firstFlipped.FlipBack();
                secondFlipped.FlipBack();

                Core.AudioManager.Instance?.PlaySFX("card_flip");
            }

            firstFlipped = null;
            secondFlipped = null;
            canFlip = true;
        }
    }

    /// <summary>
    /// Individual card in the memory game.
    /// </summary>
    public class MemoryCard : MonoBehaviour
    {
        public int FaceIndex { get; private set; }
        public bool IsFlipped { get; private set; }
        public bool IsMatched { get; private set; }

        public event System.Action<MemoryCard> OnCardClicked;

        private Image image;
        private Sprite faceSprite;
        private Sprite backSprite;
        private Button button;

        public void Initialize(int faceIndex, Sprite face, Sprite back)
        {
            FaceIndex = faceIndex;
            faceSprite = face;
            backSprite = back;
            IsFlipped = false;
            IsMatched = false;

            image = GetComponent<Image>();
            if (image == null) image = gameObject.AddComponent<Image>();

            button = GetComponent<Button>();
            if (button == null) button = gameObject.AddComponent<Button>();

            button.onClick.AddListener(() => OnCardClicked?.Invoke(this));
            ShowBack();
        }

        public void Flip()
        {
            IsFlipped = true;
            if (image != null && faceSprite != null)
                image.sprite = faceSprite;
        }

        public void FlipBack()
        {
            IsFlipped = false;
            ShowBack();
        }

        public void SetMatched()
        {
            IsMatched = true;
            if (image != null)
                image.color = new Color(1f, 1f, 1f, 0.5f);
        }

        private void ShowBack()
        {
            if (image != null && backSprite != null)
                image.sprite = backSprite;
        }
    }
}
