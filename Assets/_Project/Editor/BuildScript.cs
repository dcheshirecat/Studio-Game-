#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System;
using System.Linq;

namespace EndlessBeloved.Editor
{
    /// <summary>
    /// CI/CD build script. Generates all scenes, then builds for Android.
    /// Supports two app variants: Heal (therapeutic) and Dark (fantasy).
    ///
    /// Called from GitHub Actions via:
    ///   -executeMethod EndlessBeloved.Editor.BuildScript.BuildHeal
    ///   -executeMethod EndlessBeloved.Editor.BuildScript.BuildDark
    ///   -executeMethod EndlessBeloved.Editor.BuildScript.BuildAll
    /// </summary>
    public static class BuildScript
    {
        // ── Heal (Therapeutic) variant ─────────────────────────────────
        [MenuItem("Endless Beloved/Build Heal APK")]
        public static void BuildHeal()
        {
            try
            {
                Debug.Log("=== BuildScript: Building HEAL variant ===");
                Debug.Log("[BuildScript] Calling SceneGenerator.GenerateAllScenes()");
                SceneGenerator.GenerateAllScenes();
                Debug.Log("[BuildScript] SceneGenerator completed");
                Debug.Log("[BuildScript] Calling IconGenerator.ApplyIcons(true)");
                IconGenerator.ApplyIcons(isHeal: true);
                Debug.Log("[BuildScript] IconGenerator completed");
                BuildVariant(
                    buildName: "EndlessBelovedHeal",
                    outputPath: "build/Android/EndlessBelovedHeal.apk",
                    bundleId: "com.endlessbeloved.heal",
                    productName: "Endless, Beloved: Heal",
                    defineSymbols: "HEAL_VERSION",
                    isHeal: true
                );
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BuildScript] Exception in BuildHeal: {ex.GetType().Name}: {ex.Message}");
                Debug.LogError($"[BuildScript] Stack trace: {ex.StackTrace}");
                EditorApplication.Exit(1);
            }
        }

        // ── Dark (Fantasy) variant ────────────────────────────────────
        [MenuItem("Endless Beloved/Build Dark APK")]
        public static void BuildDark()
        {
            try
            {
                Debug.Log("=== BuildScript: Building DARK variant ===");
                Debug.Log("[BuildScript] Calling SceneGenerator.GenerateAllScenes()");
                SceneGenerator.GenerateAllScenes();
                Debug.Log("[BuildScript] SceneGenerator completed");
                Debug.Log("[BuildScript] Calling IconGenerator.ApplyIcons(false)");
                IconGenerator.ApplyIcons(isHeal: false);
                Debug.Log("[BuildScript] IconGenerator completed");
                BuildVariant(
                    buildName: "EndlessBelovedDark",
                    outputPath: "build/Android/EndlessBelovedDark.apk",
                    bundleId: "com.endlessbeloved.dark",
                    productName: "Endless, Beloved",
                    defineSymbols: "DARK_VERSION",
                    isHeal: false
                );
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BuildScript] Exception in BuildDark: {ex.GetType().Name}: {ex.Message}");
                Debug.LogError($"[BuildScript] Stack trace: {ex.StackTrace}");
                EditorApplication.Exit(1);
            }
        }

        // ── Build Both ───────────────────────────────────────────────
        [MenuItem("Endless Beloved/Build All APKs")]
        public static void BuildAll()
        {
            BuildHeal();
            BuildDark();
        }

        // ── Legacy entry point (builds Heal by default for CI compat) ─
        [MenuItem("Endless Beloved/Build Android APK")]
        public static void BuildAndroid()
        {
            BuildHeal();
        }

        // ── Quick test build (skips scene generation) ─
        [MenuItem("Endless Beloved/Quick Build Test")]
        public static void QuickBuildTest()
        {
            try
            {
                Debug.Log("=== Quick Build Test (no scene generation) ===");
                IconGenerator.ApplyIcons(isHeal: true);
                BuildVariant(
                    buildName: "EndlessBelovedHeal",
                    outputPath: "build/Android/EndlessBelovedHeal.apk",
                    bundleId: "com.endlessbeloved.heal",
                    productName: "Endless, Beloved: Heal",
                    defineSymbols: "HEAL_VERSION",
                    isHeal: true
                );
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BuildScript] Exception in QuickBuildTest: {ex.GetType().Name}: {ex.Message}");
                Debug.LogError($"Stack trace: {ex.StackTrace}");
                EditorApplication.Exit(1);
            }
        }

        // ── Minimal test (no icons, just build) ─
        [MenuItem("Endless Beloved/Minimal Build Test")]
        public static void MinimalBuildTest()
        {
            try
            {
                Debug.Log("=== Minimal Build Test ===");
                Debug.Log("Step 1: Getting scenes from build settings");
                
                var scenes = EditorBuildSettings.scenes
                    .Where(s => s.enabled)
                    .Select(s => s.path)
                    .ToArray();

                Debug.Log($"Found {scenes.Length} scenes");
                foreach (var scene in scenes)
                    Debug.Log($"  - {scene}");

                Debug.Log("Step 2: Creating build directory");
                string outputPath = "build/Android/Test.apk";
                var buildDir = System.IO.Path.GetDirectoryName(outputPath);
                if (!System.IO.Directory.Exists(buildDir))
                    System.IO.Directory.CreateDirectory(buildDir);

                Debug.Log("Step 3: Creating BuildPlayerOptions");
                var buildOptions = new BuildPlayerOptions
                {
                    scenes = scenes,
                    locationPathName = outputPath,
                    target = BuildTarget.Android,
                    options = BuildOptions.None
                };

                Debug.Log("Step 4: Calling BuildPipeline.BuildPlayer");
                BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
                Debug.Log("Step 5: BuildPipeline.BuildPlayer returned");
                
                BuildSummary summary = report.summary;
                Debug.Log($"Build result: {summary.result}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BuildScript] Exception in MinimalBuildTest: {ex.GetType().Name}: {ex.Message}");
                Debug.LogError($"Stack trace: {ex.StackTrace}");
                EditorApplication.Exit(1);
            }
        }

        // ── Core build logic ─────────────────────────────────────────
        private static void BuildVariant(
            string buildName,
            string outputPath,
            string bundleId,
            string productName,
            string defineSymbols,
            bool isHeal)
        {
            try
            {
                Debug.Log($"[BuildScript] Starting build: {buildName}");

                // Set variant-specific settings
                PlayerSettings.applicationIdentifier = bundleId;
                PlayerSettings.productName = productName;

                // Configure Android signing (use default debug keystore)
                PlayerSettings.Android.keyaliasName = "androiddebugkey";
                PlayerSettings.Android.keystoreName = "";
                PlayerSettings.Android.keyaliasPass = "android";
                PlayerSettings.Android.keystorePass = "android";
                PlayerSettings.Android.useCustomKeystore = false;

                // Target SDK 34 for Google Play compatibility
                // Cast to int because Unity 2022.3 enum may not have AndroidApiLevel35
                PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel34;

                // Set scripting define symbols for this variant
                var currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(
                    BuildTargetGroup.Android);
                // Remove old variant defines, add new one
                var defines = currentDefines
                    .Split(';')
                    .Where(d => d != "HEAL_VERSION" && d != "DARK_VERSION" && !string.IsNullOrEmpty(d))
                    .ToList();
                defines.Add(defineSymbols);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(
                    BuildTargetGroup.Android, string.Join(";", defines));

                // Splash screen color based on variant
                if (isHeal)
                {
                    PlayerSettings.SplashScreen.backgroundColor =
                        new Color(0.1f, 0.04f, 0.18f); // Deep purple
                }
                else
                {
                    PlayerSettings.SplashScreen.backgroundColor =
                        new Color(0.02f, 0.02f, 0.05f); // Near black
                }

                // Collect scenes from build settings
                var scenes = EditorBuildSettings.scenes
                    .Where(s => s.enabled)
                    .Select(s => s.path)
                    .ToArray();

                Debug.Log($"Building {buildName} with {scenes.Length} scenes:");
                foreach (var scene in scenes)
                    Debug.Log($"  - {scene}");

                // Ensure output directory exists
                var buildDir = System.IO.Path.GetDirectoryName(outputPath);
                if (!System.IO.Directory.Exists(buildDir))
                    System.IO.Directory.CreateDirectory(buildDir);

                // Build
                var buildOptions = new BuildPlayerOptions
                {
                    scenes = scenes,
                    locationPathName = outputPath,
                    target = BuildTarget.Android,
                    options = BuildOptions.None
                };

                Debug.Log($"[BuildScript] Starting BuildPipeline.BuildPlayer for {buildName}");
                BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
                Debug.Log($"[BuildScript] BuildPipeline.BuildPlayer completed for {buildName}");
                
                BuildSummary summary = report.summary;

                switch (summary.result)
                {
                    case BuildResult.Succeeded:
                        Debug.Log($"{buildName} build succeeded: {summary.totalSize / (1024 * 1024):F1} MB, time: {summary.totalTime}");
                        break;
                    case BuildResult.Failed:
                        Debug.LogError($"{buildName} build failed with {summary.totalErrors} error(s)");
                        foreach (var step in report.steps)
                            foreach (var msg in step.messages)
                                if (msg.type == LogType.Error)
                                    Debug.LogError($"  [{step.name}] {msg.content}");
                        EditorApplication.Exit(1);
                        break;
                    case BuildResult.Cancelled:
                        Debug.LogWarning($"{buildName} build was cancelled");
                        EditorApplication.Exit(2);
                        break;
                    default:
                        Debug.LogWarning($"{buildName} build result: {summary.result}");
                        EditorApplication.Exit(3);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BuildScript] Exception during {buildName} build: {ex.GetType().Name}: {ex.Message}");
                Debug.LogError($"[BuildScript] Stack trace: {ex.StackTrace}");
                EditorApplication.Exit(1);
            }
        }
    }
}
#endif
