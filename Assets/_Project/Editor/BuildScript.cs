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
            Debug.Log("=== BuildScript: Building HEAL variant ===");
            SceneGenerator.GenerateAllScenes();
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

        // ── Dark (Fantasy) variant ────────────────────────────────────
        [MenuItem("Endless Beloved/Build Dark APK")]
        public static void BuildDark()
        {
            Debug.Log("=== BuildScript: Building DARK variant ===");
            SceneGenerator.GenerateAllScenes();
            IconGenerator.ApplyIcons(isHeal: false);
            BuildVariant(
                buildName: "EndlessBelovedDark",
                outputPath: "build/Android/EndlessBelovedDark.apk",
                bundleId: "com.endlessbeloved.dark",
                productName: "Endless, Beloved",
                defineSymbols: "DARK_VERSION",
                isHeal: false
            );
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

        // ── Core build logic ─────────────────────────────────────────
        private static void BuildVariant(
            string buildName,
            string outputPath,
            string bundleId,
            string productName,
            string defineSymbols,
            bool isHeal)
        {
            // Set variant-specific settings
            PlayerSettings.applicationIdentifier = bundleId;
            PlayerSettings.productName = productName;

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

            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
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
    }
}
#endif
