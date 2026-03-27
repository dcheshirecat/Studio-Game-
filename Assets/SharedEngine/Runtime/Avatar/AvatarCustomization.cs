using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EndlessBeloved.Core;

namespace EndlessBeloved.Avatar
{
    /// <summary>
    /// Manages avatar customization: skin tone, hair, eyes, outfit, accessories.
    /// Each category has a list of options the player cycles through.
    /// </summary>
    public class AvatarCustomization : MonoBehaviour
    {
        [Header("Option Lists")]
        public List<Sprite> skinTones = new List<Sprite>();
        public List<Sprite> hairStyles = new List<Sprite>();
        public List<Color> hairColors = new List<Color>();
        public List<Sprite> eyeColors = new List<Sprite>();
        public List<Sprite> outfits = new List<Sprite>();
        public List<Sprite> accessories = new List<Sprite>();

        [Header("Display References")]
        [SerializeField] private Image skinDisplay;
        [SerializeField] private Image hairDisplay;
        [SerializeField] private Image eyeDisplay;
        [SerializeField] private Image outfitDisplay;
        [SerializeField] private Image accessoryDisplay;

        public event Action OnAvatarChanged;

        private AvatarData Data => GameState.Instance.PlayerAvatar;

        public void Initialize()
        {
            RefreshDisplay();
        }

        // ── Cycling ─────────────────────────────────────────────────────

        public void NextSkinTone() { Data.SkinTone = Cycle(Data.SkinTone, skinTones.Count); RefreshDisplay(); }
        public void PrevSkinTone() { Data.SkinTone = CycleBack(Data.SkinTone, skinTones.Count); RefreshDisplay(); }

        public void NextHairStyle() { Data.HairStyle = Cycle(Data.HairStyle, hairStyles.Count); RefreshDisplay(); }
        public void PrevHairStyle() { Data.HairStyle = CycleBack(Data.HairStyle, hairStyles.Count); RefreshDisplay(); }

        public void NextHairColor() { Data.HairColor = Cycle(Data.HairColor, hairColors.Count); RefreshDisplay(); }
        public void PrevHairColor() { Data.HairColor = CycleBack(Data.HairColor, hairColors.Count); RefreshDisplay(); }

        public void NextEyeColor() { Data.EyeColor = Cycle(Data.EyeColor, eyeColors.Count); RefreshDisplay(); }
        public void PrevEyeColor() { Data.EyeColor = CycleBack(Data.EyeColor, eyeColors.Count); RefreshDisplay(); }

        public void NextOutfit() { Data.Outfit = Cycle(Data.Outfit, outfits.Count); RefreshDisplay(); }
        public void PrevOutfit() { Data.Outfit = CycleBack(Data.Outfit, outfits.Count); RefreshDisplay(); }

        public void NextAccessory() { Data.Accessory = Cycle(Data.Accessory, accessories.Count); RefreshDisplay(); }
        public void PrevAccessory() { Data.Accessory = CycleBack(Data.Accessory, accessories.Count); RefreshDisplay(); }

        // ── Display ─────────────────────────────────────────────────────

        public void RefreshDisplay()
        {
            if (skinDisplay != null && skinTones.Count > 0)
                skinDisplay.sprite = skinTones[Mathf.Clamp(Data.SkinTone, 0, skinTones.Count - 1)];

            if (hairDisplay != null && hairStyles.Count > 0)
            {
                hairDisplay.sprite = hairStyles[Mathf.Clamp(Data.HairStyle, 0, hairStyles.Count - 1)];
                if (hairColors.Count > 0)
                    hairDisplay.color = hairColors[Mathf.Clamp(Data.HairColor, 0, hairColors.Count - 1)];
            }

            if (eyeDisplay != null && eyeColors.Count > 0)
                eyeDisplay.sprite = eyeColors[Mathf.Clamp(Data.EyeColor, 0, eyeColors.Count - 1)];

            if (outfitDisplay != null && outfits.Count > 0)
                outfitDisplay.sprite = outfits[Mathf.Clamp(Data.Outfit, 0, outfits.Count - 1)];

            if (accessoryDisplay != null && accessories.Count > 0)
            {
                int idx = Mathf.Clamp(Data.Accessory, 0, accessories.Count - 1);
                accessoryDisplay.sprite = accessories[idx];
                accessoryDisplay.gameObject.SetActive(Data.Accessory > 0); // 0 = none
            }

            OnAvatarChanged?.Invoke();
        }

        /// <summary>
        /// Get a composite of all current avatar sprites for display elsewhere.
        /// </summary>
        public List<(string layer, Sprite sprite, Color tint)> GetAvatarLayers()
        {
            var layers = new List<(string, Sprite, Color)>();

            if (skinTones.Count > 0)
                layers.Add(("skin", skinTones[Mathf.Clamp(Data.SkinTone, 0, skinTones.Count - 1)], Color.white));
            if (eyeColors.Count > 0)
                layers.Add(("eyes", eyeColors[Mathf.Clamp(Data.EyeColor, 0, eyeColors.Count - 1)], Color.white));
            if (outfits.Count > 0)
                layers.Add(("outfit", outfits[Mathf.Clamp(Data.Outfit, 0, outfits.Count - 1)], Color.white));
            if (hairStyles.Count > 0)
            {
                Color hc = hairColors.Count > 0 ? hairColors[Mathf.Clamp(Data.HairColor, 0, hairColors.Count - 1)] : Color.white;
                layers.Add(("hair", hairStyles[Mathf.Clamp(Data.HairStyle, 0, hairStyles.Count - 1)], hc));
            }
            if (accessories.Count > 0 && Data.Accessory > 0)
                layers.Add(("accessory", accessories[Mathf.Clamp(Data.Accessory, 0, accessories.Count - 1)], Color.white));

            return layers;
        }

        private int Cycle(int current, int max) => max <= 0 ? 0 : (current + 1) % max;
        private int CycleBack(int current, int max) => max <= 0 ? 0 : (current - 1 + max) % max;
    }
}
