#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace EndlessBeloved.Editor
{
    /// <summary>
    /// Auto-generates all game scenes with proper UI hierarchy.
    /// Run from Unity menu: Endless Beloved > Generate All Scenes
    /// </summary>
    public static class SceneGenerator
    {
        private static readonly Color DarkBg = new Color(0.1f, 0.04f, 0.18f);
        private static readonly Color DarkAccent = new Color(0.53f, 0.27f, 0.8f);
        private static readonly Color DarkPanel = new Color(0.05f, 0.02f, 0.1f, 0.95f);
        private static readonly Color Gold = new Color(1f, 0.84f, 0f);

        [MenuItem("Endless Beloved/Generate All Scenes")]
        public static void GenerateAllScenes()
        {
            string scenePath = "Assets/_Project/Scenes";
            if (!AssetDatabase.IsValidFolder(scenePath))
                AssetDatabase.CreateFolder("Assets/_Project", "Scenes");

            GenerateAgeGateScene(scenePath);
            GenerateTitleScreenScene(scenePath);
            GenerateCharacterSetupScene(scenePath);
            GenerateDialogueScene(scenePath);
            GenerateAltarHomeScene(scenePath);
            GenerateRouteSelectionScene(scenePath);
            GenerateMinigameScene(scenePath);

            // Add all scenes to build settings
            var scenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene($"{scenePath}/AgeGate.unity", true),
                new EditorBuildSettingsScene($"{scenePath}/TitleScreen.unity", true),
                new EditorBuildSettingsScene($"{scenePath}/CharacterSetup.unity", true),
                new EditorBuildSettingsScene($"{scenePath}/DialogueScene.unity", true),
                new EditorBuildSettingsScene($"{scenePath}/AltarHome.unity", true),
                new EditorBuildSettingsScene($"{scenePath}/RouteSelection.unity", true),
                new EditorBuildSettingsScene($"{scenePath}/MinigameScene.unity", true),
            };
            EditorBuildSettings.scenes = scenes;

            Debug.Log("All scenes generated and added to Build Settings!");
        }

        // ═══════════════════════════════════════════════════════════════
        // AGE GATE
        // ═══════════════════════════════════════════════════════════════
        static void GenerateAgeGateScene(string path)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera(DarkBg);
            CreateEventSystem();
            var bootstrap = CreateEmpty("GameBootstrap");
            bootstrap.AddComponent<GameBootstrap>();

            var canvas = CreateCanvas("AgeGateCanvas");

            // Background panel
            var bg = CreatePanel(canvas.transform, "Background", DarkBg);
            bg.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            bg.GetComponent<RectTransform>().anchorMax = Vector2.one;

            // Title
            var title = CreateText(canvas.transform, "TitleText", "ENDLESS, BELOVED",
                48, Gold, TextAnchor.MiddleCenter);
            SetAnchors(title, 0.1f, 0.75f, 0.9f, 0.85f);

            // Warning text
            var warning = CreateText(canvas.transform, "WarningText", "",
                22, Color.white, TextAnchor.MiddleCenter);
            SetAnchors(warning, 0.1f, 0.35f, 0.9f, 0.7f);

            // Confirm button
            var confirmBtn = CreateButton(canvas.transform, "ConfirmButton", "I am 18+",
                DarkAccent, Color.white);
            SetAnchors(confirmBtn, 0.15f, 0.18f, 0.85f, 0.28f);

            // Deny button
            var denyBtn = CreateButton(canvas.transform, "DenyButton", "I am under 18",
                new Color(0.3f, 0.3f, 0.3f), Color.white);
            SetAnchors(denyBtn, 0.25f, 0.08f, 0.75f, 0.16f);

            // AgeGateUI component
            var ageGatePanel = bg;
            var ageGateUI = canvas.AddComponent<UI.AgeGateUI>();

            EditorSceneManager.SaveScene(scene, $"{path}/AgeGate.unity");
            Debug.Log("Generated: AgeGate.unity");
        }

        // ═══════════════════════════════════════════════════════════════
        // TITLE SCREEN
        // ═══════════════════════════════════════════════════════════════
        static void GenerateTitleScreenScene(string path)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera(DarkBg);
            CreateEventSystem();

            var canvas = CreateCanvas("TitleCanvas");

            // Main panel
            var mainPanel = CreatePanel(canvas.transform, "MainPanel", new Color(0, 0, 0, 0));

            var title = CreateText(mainPanel.transform, "GameTitle", "ENDLESS,\nBELOVED",
                64, Gold, TextAnchor.MiddleCenter);
            SetAnchors(title, 0.1f, 0.6f, 0.9f, 0.9f);

            var subtitle = CreateText(mainPanel.transform, "Subtitle",
                "A Dark Fantasy Tarot Visual Novel", 20, new Color(0.7f, 0.6f, 0.8f),
                TextAnchor.MiddleCenter);
            SetAnchors(subtitle, 0.1f, 0.55f, 0.9f, 0.6f);

            var newGameBtn = CreateButton(mainPanel.transform, "NewGameButton", "New Game",
                DarkAccent, Color.white);
            SetAnchors(newGameBtn, 0.2f, 0.35f, 0.8f, 0.43f);

            var continueBtn = CreateButton(mainPanel.transform, "ContinueButton", "Continue",
                new Color(0.3f, 0.15f, 0.5f), Color.white);
            SetAnchors(continueBtn, 0.2f, 0.25f, 0.8f, 0.33f);

            var settingsBtn = CreateButton(mainPanel.transform, "SettingsButton", "Settings",
                new Color(0.2f, 0.1f, 0.35f), Color.white);
            SetAnchors(settingsBtn, 0.2f, 0.15f, 0.8f, 0.23f);

            // Save slot panel (hidden)
            var slotPanel = CreatePanel(canvas.transform, "SaveSlotPanel", DarkPanel);
            slotPanel.SetActive(false);

            for (int i = 0; i < 3; i++)
            {
                var slotBtn = CreateButton(slotPanel.transform, $"Slot{i}Button", $"Slot {i + 1}: Empty",
                    new Color(0.2f, 0.1f, 0.35f), Color.white);
                SetAnchors(slotBtn, 0.1f, 0.6f - i * 0.15f, 0.9f, 0.7f - i * 0.15f);
            }

            var backBtn = CreateButton(slotPanel.transform, "BackButton", "Back",
                new Color(0.3f, 0.3f, 0.3f), Color.white);
            SetAnchors(backBtn, 0.3f, 0.1f, 0.7f, 0.18f);

            // Settings panel (hidden)
            var settingsPanel = CreatePanel(canvas.transform, "SettingsPanel", DarkPanel);
            settingsPanel.SetActive(false);

            var musicLabel = CreateText(settingsPanel.transform, "MusicLabel", "Music Volume",
                20, Color.white, TextAnchor.MiddleLeft);
            SetAnchors(musicLabel, 0.1f, 0.7f, 0.4f, 0.76f);

            var sfxLabel = CreateText(settingsPanel.transform, "SFXLabel", "SFX Volume",
                20, Color.white, TextAnchor.MiddleLeft);
            SetAnchors(sfxLabel, 0.1f, 0.55f, 0.4f, 0.61f);

            var backSettings = CreateButton(settingsPanel.transform, "BackFromSettings", "Back",
                new Color(0.3f, 0.3f, 0.3f), Color.white);
            SetAnchors(backSettings, 0.3f, 0.1f, 0.7f, 0.18f);

            // MainMenuUI component
            canvas.AddComponent<UI.MainMenuUI>();

            EditorSceneManager.SaveScene(scene, $"{path}/TitleScreen.unity");
            Debug.Log("Generated: TitleScreen.unity");
        }

        // ═══════════════════════════════════════════════════════════════
        // CHARACTER SETUP
        // ═══════════════════════════════════════════════════════════════
        static void GenerateCharacterSetupScene(string path)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera(DarkBg);
            CreateEventSystem();

            var canvas = CreateCanvas("SetupCanvas");

            // Name prompt
            var namePrompt = CreateText(canvas.transform, "NamePromptText",
                "The cards ask your name.\nWhat do you tell them?",
                24, Color.white, TextAnchor.MiddleCenter);
            SetAnchors(namePrompt, 0.1f, 0.6f, 0.9f, 0.75f);

            // Name input
            var inputGo = CreateEmpty("NameInput", canvas.transform);
            var inputRect = inputGo.AddComponent<RectTransform>();
            SetAnchors(inputGo, 0.15f, 0.45f, 0.85f, 0.55f);
            var inputImg = inputGo.AddComponent<Image>();
            inputImg.color = new Color(0.15f, 0.08f, 0.25f);
            var input = inputGo.AddComponent<TMP_InputField>();
            var inputText = CreateText(inputGo.transform, "InputText", "", 28, Color.white, TextAnchor.MiddleCenter);
            var placeholder = CreateText(inputGo.transform, "Placeholder", "Enter your name...",
                28, new Color(0.5f, 0.5f, 0.5f), TextAnchor.MiddleCenter);
            input.textComponent = inputText.GetComponent<TMP_Text>();
            input.placeholder = placeholder.GetComponent<TMP_Text>();

            // Pronoun panel (hidden initially)
            var pronounPanel = CreatePanel(canvas.transform, "PronounPanel", new Color(0, 0, 0, 0));
            pronounPanel.SetActive(false);
            var pronounLabel = CreateText(pronounPanel.transform, "PronounLabel", "they/them/their",
                20, Gold, TextAnchor.MiddleCenter);
            SetAnchors(pronounLabel, 0.2f, 0.65f, 0.8f, 0.72f);

            var theyBtn = CreateButton(pronounPanel.transform, "TheyButton", "They/Them",
                DarkAccent, Color.white);
            SetAnchors(theyBtn, 0.1f, 0.45f, 0.45f, 0.55f);
            var sheBtn = CreateButton(pronounPanel.transform, "SheButton", "She/Her",
                DarkAccent, Color.white);
            SetAnchors(sheBtn, 0.55f, 0.45f, 0.9f, 0.55f);
            var heBtn = CreateButton(pronounPanel.transform, "HeButton", "He/Him",
                DarkAccent, Color.white);
            SetAnchors(heBtn, 0.3f, 0.33f, 0.7f, 0.43f);

            // Avatar panel (hidden initially)
            var avatarPanel = CreatePanel(canvas.transform, "AvatarPanel", new Color(0, 0, 0, 0));
            avatarPanel.SetActive(false);

            // Navigation
            var nextBtn = CreateButton(canvas.transform, "NextButton", "Next",
                DarkAccent, Color.white);
            SetAnchors(nextBtn, 0.3f, 0.08f, 0.7f, 0.16f);

            var backBtn = CreateButton(canvas.transform, "BackButton", "Back",
                new Color(0.3f, 0.3f, 0.3f), Color.white);
            SetAnchors(backBtn, 0.05f, 0.08f, 0.28f, 0.16f);

            // CharacterSetupUI component
            canvas.AddComponent<UI.CharacterSetupUI>();

            EditorSceneManager.SaveScene(scene, $"{path}/CharacterSetup.unity");
            Debug.Log("Generated: CharacterSetup.unity");
        }

        // ═══════════════════════════════════════════════════════════════
        // DIALOGUE SCENE
        // ═══════════════════════════════════════════════════════════════
        static void GenerateDialogueScene(string path)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera(DarkBg);
            CreateEventSystem();

            // DialogueRunner
            var runner = CreateEmpty("DialogueRunner");
            runner.AddComponent<Dialogue.DialogueRunner>();

            var canvas = CreateCanvas("DialogueCanvas");

            // Background image (full screen)
            var bgImage = CreateEmpty("BackgroundImage", canvas.transform);
            bgImage.AddComponent<RectTransform>();
            SetAnchors(bgImage, 0, 0, 1, 1);
            var bgImg = bgImage.AddComponent<Image>();
            bgImg.color = DarkBg;

            // Portrait
            var portrait = CreateEmpty("PortraitImage", canvas.transform);
            portrait.AddComponent<RectTransform>();
            SetAnchors(portrait, 0.15f, 0.25f, 0.85f, 0.75f);
            var portImg = portrait.AddComponent<Image>();
            portImg.color = new Color(1, 1, 1, 0);

            // Dialogue panel
            var dialoguePanel = CreatePanel(canvas.transform, "DialoguePanel", DarkPanel);
            SetAnchors(dialoguePanel, 0, 0, 1, 0.35f);
            var dialoguePanelRect = dialoguePanel.GetComponent<RectTransform>();

            var speakerName = CreateText(dialoguePanel.transform, "SpeakerNameText", "",
                22, Gold, TextAnchor.MiddleLeft);
            SetAnchors(speakerName, 0.05f, 0.8f, 0.6f, 0.95f);

            var dialogueText = CreateText(dialoguePanel.transform, "DialogueText", "",
                20, Color.white, TextAnchor.UpperLeft);
            SetAnchors(dialogueText, 0.05f, 0.1f, 0.95f, 0.78f);

            var tapIndicator = CreateText(dialoguePanel.transform, "TapIndicator", "tap to continue >>",
                16, new Color(0.6f, 0.6f, 0.6f), TextAnchor.MiddleRight);
            SetAnchors(tapIndicator, 0.6f, 0.02f, 0.95f, 0.1f);

            // Choice panel (hidden)
            var choicePanel = CreatePanel(canvas.transform, "ChoicePanel", new Color(0, 0, 0, 0.5f));
            choicePanel.SetActive(false);
            SetAnchors(choicePanel, 0.05f, 0.02f, 0.95f, 0.33f);

            // Choice button prefab template
            var choicePrefab = CreateButton(choicePanel.transform, "ChoiceButtonPrefab",
                "Choice text", new Color(0.15f, 0.08f, 0.25f, 0.9f), Color.white);
            SetAnchors(choicePrefab, 0, 0.7f, 1, 0.95f);
            choicePrefab.SetActive(false);

            // DialogueBoxUI component
            canvas.AddComponent<UI.DialogueBoxUI>();

            EditorSceneManager.SaveScene(scene, $"{path}/DialogueScene.unity");
            Debug.Log("Generated: DialogueScene.unity");
        }

        // ═══════════════════════════════════════════════════════════════
        // ALTAR HOME
        // ═══════════════════════════════════════════════════════════════
        static void GenerateAltarHomeScene(string path)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera(DarkBg);
            CreateEventSystem();

            var canvas = CreateCanvas("AltarCanvas");

            // Header
            var playerName = CreateText(canvas.transform, "PlayerNameText", "Player",
                24, Gold, TextAnchor.MiddleLeft);
            SetAnchors(playerName, 0.05f, 0.92f, 0.5f, 0.98f);

            var chapterText = CreateText(canvas.transform, "ChapterText", "Chapter 1",
                18, Color.white, TextAnchor.MiddleRight);
            SetAnchors(chapterText, 0.5f, 0.94f, 0.95f, 0.98f);

            var cycleText = CreateText(canvas.transform, "CycleText", "Cycle 1",
                16, new Color(0.7f, 0.6f, 0.8f), TextAnchor.MiddleRight);
            SetAnchors(cycleText, 0.5f, 0.9f, 0.95f, 0.94f);

            // Navigation buttons
            string[] buttons = { "Routes", "Daily Reading", "Spells", "Journal", "Save" };
            float startY = 0.7f;
            for (int i = 0; i < buttons.Length; i++)
            {
                var btn = CreateButton(canvas.transform, $"{buttons[i].Replace(" ", "")}Button",
                    buttons[i], DarkAccent, Color.white);
                SetAnchors(btn, 0.15f, startY - i * 0.12f, 0.85f, startY - i * 0.12f + 0.09f);
            }

            // AltarHomeUI component
            canvas.AddComponent<UI.AltarHomeUI>();

            EditorSceneManager.SaveScene(scene, $"{path}/AltarHome.unity");
            Debug.Log("Generated: AltarHome.unity");
        }

        // ═══════════════════════════════════════════════════════════════
        // ROUTE SELECTION
        // ═══════════════════════════════════════════════════════════════
        static void GenerateRouteSelectionScene(string path)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera(DarkBg);
            CreateEventSystem();

            var canvas = CreateCanvas("RouteCanvas");

            var title = CreateText(canvas.transform, "Title", "Choose Your Path",
                36, Gold, TextAnchor.MiddleCenter);
            SetAnchors(title, 0.1f, 0.88f, 0.9f, 0.96f);

            // Route list will be populated dynamically by RouteSelectionUI
            var scrollView = CreateEmpty("RouteList", canvas.transform);
            scrollView.AddComponent<RectTransform>();
            SetAnchors(scrollView, 0.05f, 0.1f, 0.95f, 0.85f);

            // Route entry prefab template
            var routePrefab = CreatePanel(scrollView.transform, "RouteEntryPrefab",
                new Color(0.15f, 0.08f, 0.25f, 0.8f));
            SetAnchors(routePrefab, 0, 0.85f, 1, 1);
            var nameText = CreateText(routePrefab.transform, "NameText", "The Oracle\nHope",
                20, Color.white, TextAnchor.MiddleLeft);
            SetAnchors(nameText, 0.3f, 0.1f, 0.9f, 0.6f);
            var tagText = CreateText(routePrefab.transform, "TaglineText", "Foresight & fate",
                16, new Color(0.7f, 0.6f, 0.8f), TextAnchor.MiddleLeft);
            SetAnchors(tagText, 0.3f, 0.6f, 0.9f, 0.9f);
            routePrefab.SetActive(false);

            var backBtn = CreateButton(canvas.transform, "BackButton", "Back",
                new Color(0.3f, 0.3f, 0.3f), Color.white);
            SetAnchors(backBtn, 0.3f, 0.02f, 0.7f, 0.08f);

            // RouteSelectionUI component
            canvas.AddComponent<UI.RouteSelectionUI>();

            EditorSceneManager.SaveScene(scene, $"{path}/RouteSelection.unity");
            Debug.Log("Generated: RouteSelection.unity");
        }

        // ═══════════════════════════════════════════════════════════════
        // MINIGAME SCENE
        // ═══════════════════════════════════════════════════════════════
        static void GenerateMinigameScene(string path)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera(DarkBg);
            CreateEventSystem();

            var canvas = CreateCanvas("MinigameCanvas");

            var title = CreateText(canvas.transform, "MinigameTitle", "Card Memory",
                32, Gold, TextAnchor.MiddleCenter);
            SetAnchors(title, 0.1f, 0.9f, 0.9f, 0.97f);

            var scoreText = CreateText(canvas.transform, "ScoreText", "Score: 0",
                20, Color.white, TextAnchor.MiddleLeft);
            SetAnchors(scoreText, 0.05f, 0.84f, 0.4f, 0.9f);

            var timeText = CreateText(canvas.transform, "TimeText", "Time: 60",
                20, Color.white, TextAnchor.MiddleRight);
            SetAnchors(timeText, 0.6f, 0.84f, 0.95f, 0.9f);

            // Card grid parent
            var grid = CreateEmpty("CardGrid", canvas.transform);
            grid.AddComponent<RectTransform>();
            SetAnchors(grid, 0.05f, 0.1f, 0.95f, 0.82f);
            var layout = grid.AddComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(150, 200);
            layout.spacing = new Vector2(15, 15);
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = 4;
            layout.childAlignment = TextAnchor.MiddleCenter;

            // Card prefab
            var cardPrefab = CreatePanel(grid.transform, "CardPrefab",
                new Color(0.2f, 0.1f, 0.35f));
            cardPrefab.AddComponent<Button>();
            cardPrefab.SetActive(false);

            EditorSceneManager.SaveScene(scene, $"{path}/MinigameScene.unity");
            Debug.Log("Generated: MinigameScene.unity");
        }

        // ═══════════════════════════════════════════════════════════════
        // HELPERS
        // ═══════════════════════════════════════════════════════════════

        static GameObject CreateCamera(Color bgColor)
        {
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            var cam = go.AddComponent<Camera>();
            cam.backgroundColor = bgColor;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.orthographic = true;
            cam.orthographicSize = 5;
            go.transform.position = new Vector3(0, 0, -10);
            go.AddComponent<AudioListener>();
            return go;
        }

        static GameObject CreateEventSystem()
        {
            var go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            go.AddComponent<StandaloneInputModule>();
            return go;
        }

        static GameObject CreateCanvas(string name)
        {
            var go = new GameObject(name);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;

            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            go.AddComponent<GraphicRaycaster>();
            return go;
        }

        static GameObject CreatePanel(Transform parent, string name, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = color;
            return go;
        }

        static GameObject CreateText(Transform parent, string name, string text,
            int fontSize, Color color, TextAnchor alignment)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var t = go.AddComponent<TextMeshProUGUI>();
            t.text = text;
            t.fontSize = fontSize;
            t.color = color;
            t.alignment = TextAnchorToTMPAlignment(alignment);
            t.enableWordWrapping = true;
            return go;
        }

        static TextAlignmentOptions TextAnchorToTMPAlignment(TextAnchor anchor)
        {
            return anchor switch
            {
                TextAnchor.UpperLeft => TextAlignmentOptions.Left,
                TextAnchor.UpperCenter => TextAlignmentOptions.Center,
                TextAnchor.UpperRight => TextAlignmentOptions.Right,
                TextAnchor.MiddleLeft => TextAlignmentOptions.Left,
                TextAnchor.MiddleCenter => TextAlignmentOptions.Center,
                TextAnchor.MiddleRight => TextAlignmentOptions.Right,
                TextAnchor.LowerLeft => TextAlignmentOptions.Left,
                TextAnchor.LowerCenter => TextAlignmentOptions.Center,
                TextAnchor.LowerRight => TextAlignmentOptions.Right,
                _ => TextAlignmentOptions.Center
            };
        }

        static GameObject CreateButton(Transform parent, string name, string label,
            Color bgColor, Color textColor)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>();
            var img = go.AddComponent<Image>();
            img.color = bgColor;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            var textGo = CreateText(go.transform, "Text", label, 22, textColor, TextAnchor.MiddleCenter);
            return go;
        }

        static GameObject CreateEmpty(string name, Transform parent = null)
        {
            var go = new GameObject(name);
            if (parent != null) go.transform.SetParent(parent, false);
            return go;
        }

        static void SetAnchors(GameObject go, float minX, float minY, float maxX, float maxY)
        {
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(minX, minY);
            rect.anchorMax = new Vector2(maxX, maxY);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
#endif
