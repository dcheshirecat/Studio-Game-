#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace EndlessBeloved.Editor
{
    /// <summary>
    /// One-time setup script. Run once when starting a fresh clone.
    /// Creates necessary folders, placeholder assets, and initial scenes.
    /// </summary>
    public static class FirstTimeSetup
    {
        [MenuItem("Endless Beloved/First Time Setup")]
        public static void RunSetup()
        {
            Debug.Log("=== Endless Beloved: First Time Setup ===");

            CreateFolders();
            CreateInitialAssets();
            SceneGenerator.GenerateAllScenes();
            IconGenerator.GenerateAll();
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("=== Setup Complete! ===");
            Debug.Log("Next steps:");
            Debug.Log("1. Open Edit > Project Settings > Player to verify Android settings");
            Debug.Log("2. Build: Endless Beloved > Build Heal APK");
        }

        private static void CreateFolders()
        {
            string[] folders = new[]
            {
                "Assets/_Project/Data",
                "Assets/_Project/Data/Characters",
                "Assets/_Project/Data/Cards",
                "Assets/_Project/Data/Skills",
                "Assets/_Project/Data/Databases",
                "Assets/Resources/Story",
                "Assets/Resources/Backgrounds",
                "Assets/Resources/Characters",
                "Assets/Resources/Audio/Music",
                "Assets/Resources/Audio/SFX",
                "Assets/Resources/Tarot",
            };

            foreach (var folder in folders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    string parent = Path.GetDirectoryName(folder);
                    if (AssetDatabase.IsValidFolder(parent))
                    {
                        AssetDatabase.CreateFolder(parent, Path.GetFileName(folder));
                        Debug.Log($"Created folder: {folder}");
                    }
                }
            }
        }

        private static void CreateInitialAssets()
        {
            // Create placeholder Character Database if not exists
            string dbPath = "Assets/_Project/Data/Databases/CharacterDatabase.asset";
            if (!File.Exists(dbPath))
            {
                var db = ScriptableObject.CreateInstance<EndlessBeloved.Characters.CharacterDatabase>();
                AssetDatabase.CreateAsset(db, dbPath);
                Debug.Log($"Created: {dbPath}");
            }

            // Create placeholder Tarot Deck if not exists
            string deckPath = "Assets/_Project/Data/TarotDeck.asset";
            if (!File.Exists(deckPath))
            {
                var deck = ScriptableObject.CreateInstance<EndlessBeloved.Tarot.TarotDeck>();
                AssetDatabase.CreateAsset(deck, deckPath);
                Debug.Log($"Created: {deckPath}");
            }

            // Create placeholder oracle story JSON if not exists
            string storyPath = "Assets/Resources/Story/oracle_ch1.json";
            if (!File.Exists(storyPath))
            {
                string json = @"{
    ""start"": {
        ""type"": ""line"",
        ""speaker"": ""narrator"",
        ""text"": ""You stand at the entrance to the sanctuary. The morning air is thick with possibility."",
        ""next"": ""start_2""
    },
    ""start_2"": {
        ""type"": ""line"",
        ""speaker"": ""narrator"",
        ""text"": ""The Oracle awaits within."",
        ""next_scene"": ""AltarHome""
    }
}";
                File.WriteAllText(storyPath, json);
                Debug.Log($"Created: {storyPath}");
            }

            // Create placeholder backgrounds folder marker
            string bgMarkerPath = "Assets/Resources/Backgrounds/PLACEHOLDER.txt";
            if (!File.Exists(bgMarkerPath))
            {
                File.WriteAllText(bgMarkerPath, "Place background images here (1080x1920 PNG or JPG).");
                Debug.Log($"Created: {bgMarkerPath}");
            }
        }
    }
}
#endif
