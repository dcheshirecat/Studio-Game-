using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EndlessBeloved.Core;
using EndlessBeloved.Tarot;

namespace EndlessBeloved.UI
{
    /// <summary>
    /// Card draw UI: swipe/tap to draw a card, reveal animation, meaning display.
    /// Used for daily readings and story-triggered draws.
    /// </summary>
    public class CardDrawUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TarotDeck tarotDeck;
        [SerializeField] private Image cardImage;
        [SerializeField] private Image cardBackImage;
        [SerializeField] private TMP_Text cardNameText;
        [SerializeField] private TMP_Text cardMeaningText;
        [SerializeField] private TMP_Text positionText; // "Past", "Present", "Future" for readings
        [SerializeField] private GameObject drawPanel;
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private Button drawButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button closeButton;

        [Header("Animation")]
        [SerializeField] private float flipDuration = 0.6f;
        [SerializeField] private RectTransform cardTransform;

        [Header("Reading Mode")]
        [SerializeField] private GameObject readingPanel;
        [SerializeField] private Image[] readingCardImages = new Image[3];
        [SerializeField] private TMP_Text[] readingCardNames = new TMP_Text[3];
        [SerializeField] private TMP_Text[] readingPositionLabels = new TMP_Text[3];

        private CardData drawnCard;
        private bool isReversed;
        private System.Action<string> onDrawComplete;

        private void Start()
        {
            drawButton?.onClick.AddListener(DrawCard);
            continueButton?.onClick.AddListener(OnContinue);
            closeButton?.onClick.AddListener(Close);
            resultPanel.SetActive(false);
            readingPanel?.SetActive(false);
        }

        /// <summary>
        /// Open for a single story-triggered draw.
        /// </summary>
        public void OpenForStoryDraw(string forcedCardId, System.Action<string> callback)
        {
            onDrawComplete = callback;
            drawPanel.SetActive(true);
            resultPanel.SetActive(false);

            if (!string.IsNullOrEmpty(forcedCardId))
            {
                // Auto-draw the forced card
                var result = tarotDeck.DrawSpecific(forcedCardId);
                ShowResult(result.card, result.reversed);
            }
        }

        /// <summary>
        /// Open for a daily reading (3-card spread).
        /// </summary>
        public void OpenForDailyReading()
        {
            var reading = tarotDeck.DailyReading();
            readingPanel?.SetActive(true);
            drawPanel.SetActive(false);

            for (int i = 0; i < 3 && i < reading.Count; i++)
            {
                var (card, reversed, position) = reading[i];
                if (readingCardImages[i] != null)
                {
                    readingCardImages[i].sprite = card.cardArt;
                    if (reversed)
                        readingCardImages[i].rectTransform.localRotation = Quaternion.Euler(0, 0, 180);
                    else
                        readingCardImages[i].rectTransform.localRotation = Quaternion.identity;
                }
                if (readingCardNames[i] != null)
                    readingCardNames[i].text = reversed ? $"{card.cardName} (Reversed)" : card.cardName;
                if (readingPositionLabels[i] != null)
                    readingPositionLabels[i].text = position;
            }
        }

        private void DrawCard()
        {
            var result = tarotDeck.DrawRandom();
            if (result.card == null) return;
            StartCoroutine(FlipAnimation(result.card, result.reversed));
        }

        private IEnumerator FlipAnimation(CardData card, bool reversed)
        {
            drawButton.interactable = false;
            AudioManager.Instance?.PlaySFX("card_flip");

            // Shrink horizontally (simulate flip)
            float half = flipDuration / 2f;
            float elapsed = 0f;

            while (elapsed < half)
            {
                elapsed += Time.deltaTime;
                float scale = Mathf.Lerp(1f, 0f, elapsed / half);
                if (cardTransform != null)
                    cardTransform.localScale = new Vector3(scale, 1f, 1f);
                yield return null;
            }

            // Switch to card face
            ShowResult(card, reversed);

            // Expand back
            elapsed = 0f;
            while (elapsed < half)
            {
                elapsed += Time.deltaTime;
                float scale = Mathf.Lerp(0f, 1f, elapsed / half);
                if (cardTransform != null)
                    cardTransform.localScale = new Vector3(scale, 1f, 1f);
                yield return null;
            }

            if (cardTransform != null)
                cardTransform.localScale = Vector3.one;

            AudioManager.Instance?.PlaySFX("card_reveal");
        }

        private void ShowResult(CardData card, bool reversed)
        {
            drawnCard = card;
            isReversed = reversed;

            drawPanel.SetActive(false);
            resultPanel.SetActive(true);

            if (cardImage != null)
            {
                cardImage.sprite = card.cardArt;
                cardImage.rectTransform.localRotation = reversed
                    ? Quaternion.Euler(0, 0, 180)
                    : Quaternion.identity;
            }

            if (cardNameText != null)
                cardNameText.text = reversed ? $"{card.cardName} (Reversed)" : card.cardName;

            if (cardMeaningText != null)
                cardMeaningText.text = reversed ? card.reversedMeaning : card.uprightMeaning;
        }

        private void OnContinue()
        {
            string drawId = drawnCard != null
                ? (isReversed ? $"{drawnCard.cardId}_reversed" : drawnCard.cardId)
                : "";
            onDrawComplete?.Invoke(drawId);
            Close();
        }

        public void Close()
        {
            drawPanel.SetActive(false);
            resultPanel.SetActive(false);
            readingPanel?.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
