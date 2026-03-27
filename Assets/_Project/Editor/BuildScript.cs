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
    /// Called from GitHub Actions via: -executeMethod EndlessBeloved.Editor.BuildScript.BuildAndroid
    /// </summary>
    public static class BuildScript
    {
        private static readonly string BuildPath = "build/Android/EndlessBelovedHeal.apk";

        [MenuItem("Endless Beloved/Build Android APK")]
        public static void BuildAndroid()
        {
            Debug.Log("=== BuildScript: Starting Android build ===");

            // Step 1: Generate all scenes
            Debug.Log("Step 1: Generating all scenes...");
            SceneGenerator.GenerateAllScenes();

            // Step 2: Collect scenes from build settings
            var scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();

            Debug.Log($"Step 2: Building with {scenes.Length} scenes:");
            foreach (var scene in scenes)
                Debug.Log($"  - {scene}");

            // Step 3: Ensure output directory exists
            var buildDir = System.IO.Path.GetDirectoryName(BuildPath);
            if (!System.IO.Directory.Exists(buildDir))
                System.IO.Directory.CreateDirectory(buildDir);

            // Step 4: Configure build options
            var buildOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = BuildPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            // Step 5: Build
            Debug.Log("Step 3: Starting Android build...");
            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            BuildSummary summary = report.summary;

            switch (summary.result)
            {
                case BuildResult.Succeeded:
                    Debug.Log($"Build succeeded: {summary.totalSize / (1024 * 1024):F1} MB, time: {summary.totalTime}");
                    break;
                case BuildResult.Failed:
                    Debug.LogError($"Build failed with {summary.totalErrors} error(s)");
                    // Log individual errors
                    foreach (var step in report.steps)
                    {
                        foreach (var msg in step.messages)
                        {
                            if (msg.type == LogType.Error)
                                Debug.LogError($"  [{step.name}] {msg.content}");
                        }
                    }
                    EditorApplication.Exit(1);
                    break;
                case BuildResult.Cancelled:
                    Debug.LogWarning("Build was cancelled");
                    EditorApplication.Exit(2);
                    break;
                default:
                    Debug.LogWarning($"Build result: {summary.result}");
                    EditorApplication.Exit(3);
                    break;
            }
        }
    }
}
#endif
