#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace EndlessBeloved.Editor
{
    /// <summary>
    /// Generates simple placeholder app icons for Heal and Dark variants.
    /// Run from Unity menu: Endless Beloved > Generate App Icons
    /// </summary>
    public static class IconGenerator
    {
        private static readonly int[] IconSizes = { 48, 72, 96, 144, 192, 512 };

        [MenuItem("Endless Beloved/Generate App Icons")]
        public static void GenerateAll()
        {
            GenerateHealIcons();
            GenerateDarkIcons();
            AssetDatabase.Refresh();
            Debug.Log("App icons generated!");
        }

        public static void GenerateHealIcons()
        {
            // Heal: deep purple background with gold tarot star
            Color bg = new Color(0.15f, 0.05f, 0.25f);
            Color accent = new Color(1f, 0.84f, 0f);       // Gold
            Color secondary = new Color(0.6f, 0.4f, 0.8f); // Light purple
            GenerateIconSet("Heal", bg, accent, secondary, true);
        }

        public static void GenerateDarkIcons()
        {
            // Dark: near-black background with red/crimson accent
            Color bg = new Color(0.05f, 0.02f, 0.08f);
            Color accent = new Color(0.8f, 0.1f, 0.15f);   // Crimson
            Color secondary = new Color(0.4f, 0.1f, 0.3f);  // Dark magenta
            GenerateIconSet("Dark", bg, accent, secondary, false);
        }

        private static void GenerateIconSet(string variant, Color bg, Color accent, Color secondary, bool isHeal)
        {
            string dir = $"Assets/_Project/Art/Icons/{variant}";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            foreach (int size in IconSizes)
            {
                var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
                DrawIcon(tex, size, bg, accent, secondary, isHeal);

                byte[] png = tex.EncodeToPNG();
                string path = $"{dir}/icon_{size}.png";
                File.WriteAllBytes(path, png);
                Object.DestroyImmediate(tex);
            }

            AssetDatabase.Refresh();
        }

        private static void DrawIcon(Texture2D tex, int size, Color bg, Color accent, Color secondary, bool isHeal)
        {
            float center = size / 2f;
            float radius = size * 0.42f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - center;
                    float dy = y - center;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    // Background with subtle radial gradient
                    float gradientT = Mathf.Clamp01(dist / (size * 0.7f));
                    Color pixel = Color.Lerp(bg * 1.3f, bg * 0.7f, gradientT);

                    // Circular border
                    float borderDist = Mathf.Abs(dist - radius);
                    if (borderDist < size * 0.02f)
                    {
                        float t = 1f - borderDist / (size * 0.02f);
                        pixel = Color.Lerp(pixel, secondary, t * 0.8f);
                    }

                    // Inner circle ring
                    float innerRadius = size * 0.3f;
                    float innerDist = Mathf.Abs(dist - innerRadius);
                    if (innerDist < size * 0.015f)
                    {
                        float t = 1f - innerDist / (size * 0.015f);
                        pixel = Color.Lerp(pixel, secondary * 0.6f, t * 0.5f);
                    }

                    // Central symbol
                    if (isHeal)
                    {
                        // Star/pentagram shape for Heal
                        DrawStar(ref pixel, x, y, center, size * 0.18f, accent, 5);
                        // Small heart at center
                        DrawHeart(ref pixel, x, y, center, center, size * 0.06f, accent);
                    }
                    else
                    {
                        // Crescent moon for Dark
                        DrawCrescent(ref pixel, x, y, center, size * 0.2f, accent);
                        // Small dot at center
                        float dotDist = Mathf.Sqrt((x - center) * (x - center) + (y - center) * (y - center));
                        if (dotDist < size * 0.03f)
                            pixel = Color.Lerp(pixel, accent, 1f - dotDist / (size * 0.03f));
                    }

                    // Corner decorations (small dots)
                    float cornerSize = size * 0.04f;
                    float margin = size * 0.12f;
                    DrawCornerDot(ref pixel, x, y, margin, margin, cornerSize, secondary);
                    DrawCornerDot(ref pixel, x, y, size - margin, margin, cornerSize, secondary);
                    DrawCornerDot(ref pixel, x, y, margin, size - margin, cornerSize, secondary);
                    DrawCornerDot(ref pixel, x, y, size - margin, size - margin, cornerSize, secondary);

                    pixel.a = 1f;
                    tex.SetPixel(x, y, pixel);
                }
            }

            tex.Apply();
        }

        private static void DrawStar(ref Color pixel, int x, int y, float center, float r, Color color, int points)
        {
            float angle = Mathf.Atan2(y - center, x - center);
            float dist = Mathf.Sqrt((x - center) * (x - center) + (y - center) * (y - center));

            float starAngle = angle + Mathf.PI / 2f;
            float starR = r * (0.5f + 0.5f * Mathf.Cos(points * starAngle));

            if (dist < starR)
            {
                float t = 1f - dist / starR;
                pixel = Color.Lerp(pixel, color, t * 0.9f);
            }
        }

        private static void DrawHeart(ref Color pixel, int x, int y, float cx, float cy, float size, Color color)
        {
            float nx = (x - cx) / size;
            float ny = -(y - cy) / size;
            float heart = (nx * nx + ny * ny - 1);
            heart = heart * heart * heart - nx * nx * ny * ny * ny;
            if (heart < 0)
                pixel = Color.Lerp(pixel, color, 0.9f);
        }

        private static void DrawCrescent(ref Color pixel, int x, int y, float center, float r, Color color)
        {
            float dx = x - center;
            float dy = y - center;
            float dist1 = Mathf.Sqrt(dx * dx + dy * dy);

            float offset = r * 0.4f;
            float dx2 = x - (center + offset);
            float dist2 = Mathf.Sqrt(dx2 * dx2 + dy * dy);

            if (dist1 < r && dist2 > r * 0.85f)
            {
                float t = 1f - dist1 / r;
                pixel = Color.Lerp(pixel, color, t * 0.85f);
            }
        }

        private static void DrawCornerDot(ref Color pixel, int x, int y, float cx, float cy, float r, Color color)
        {
            float dist = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
            if (dist < r)
            {
                float t = 1f - dist / r;
                pixel = Color.Lerp(pixel, color, t * 0.6f);
            }
        }

        /// <summary>
        /// Apply icons for a specific variant. Called from BuildScript.
        /// </summary>
        public static void ApplyIcons(bool isHeal)
        {
            string variant = isHeal ? "Heal" : "Dark";
            string dir = $"Assets/_Project/Art/Icons/{variant}";

            // Generate if they don't exist
            if (!Directory.Exists(dir) || Directory.GetFiles(dir, "*.png").Length == 0)
            {
                if (isHeal) GenerateHealIcons();
                else GenerateDarkIcons();
            }

            // Load the 192px icon as the default
            string iconPath = $"{dir}/icon_192.png";
            if (!File.Exists(iconPath)) return;

            AssetDatabase.Refresh();
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            if (icon == null) return;

            // Set as default icon
            var icons = new Texture2D[] { icon };
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, icons);

            Debug.Log($"Applied {variant} icons from {dir}");
        }
    }
}
#endif
