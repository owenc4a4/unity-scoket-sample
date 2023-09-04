using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Reporting;

namespace App
{
    public class BuildT
    {
        private static bool useAAB = false;
        private static bool export = false;


        [MenuItem("App/Build/Build")]
        public static void Build()
        {
            export = false;
            Build(EditorUserBuildSettings.activeBuildTarget);
        }

        [MenuItem("App/Build/BuildDev")]
        public static void BuildDev()
        {
            export = false;
            Build(EditorUserBuildSettings.activeBuildTarget, true);
        }

        [MenuItem("App/Build/BuildDev&Run")]
        public static void BuildDevAndRun()
        {
            export = false;
            Build(EditorUserBuildSettings.activeBuildTarget, true, true);
        }

        [MenuItem("App/Build/Android/BuildAPK")]
        public static void BuildAndroid()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                Debug.LogError("Not Android");
                return;
            }

            export = false;
            useAAB = false;
            Build(BuildTarget.Android);
        }

        [MenuItem("App/Build/Android/BuildAAB")]
        public static void BuildAAB()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                Debug.LogError("Not Android");
                return;
            }

            useAAB = true;
            export = false;
            Build(BuildTarget.Android);
            useAAB = false;
        }

        [MenuItem("App/Build/Android/Export")]
        public static void Export()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                Debug.LogError("Not Android");
                return;
            }

            //forceAAB = true;
            export = true;
            Build(BuildTarget.Android);
            export = false;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
        }

        public static void Build(BuildTarget target, bool isDev = false, bool isRun = false)
        {
            Debug.Log("start build + " + target);

            PreBuild(isDev);

            // TODO support other platform
            var path = "";
            var file = "";
            switch (target) {
                case BuildTarget.Android:
                    path = "../Build/Android";
                    file = "/build";
                    if (isDev) {
                        file += "_dev";
                    }

                    if (useAAB) {
                        file += ".aab";
                    } else {
                        file += ".apk";
                    }

                    if (export) {
                        file = "prj";
                    }

                    break;
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneWindows:
                    path = "../Build/Win";
                    file = "/app.exe";
                break;
                default:
                    throw new Exception("no target");
            }

            FileUtil.DeleteFileOrDirectory(path);
            Directory.CreateDirectory(path);
            path = path + file;

            // Set Build Setting
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = EditorBuildSettings.scenes.Where(x => x.enabled).Select(x => x.path).ToArray();
            buildPlayerOptions.locationPathName = path;
            buildPlayerOptions.target = target;
            //buildPlayerOptions.targetGroup = group;
            var options = BuildOptions.None;

            if (isDev) {
                //options |= BuildOptions.Development;
                options |= BuildOptions.Development | BuildOptions.ConnectWithProfiler;
                buildPlayerOptions.extraScriptingDefines = new  string[] {"IS_DEV"};
            } else {
                //options |= BuildOptions.CleanBuildCache;
            }

            if (isRun) {
                options |= BuildOptions.AutoRunPlayer;
            }

            #if UNITY_ANDROID
            Debug.Log("Im android");
            if (useAAB) {
                EditorUserBuildSettings.buildAppBundle = true;
            } else {
                EditorUserBuildSettings.buildAppBundle = false;
            }
            #endif

            buildPlayerOptions.options = options;

            Debug.Log("build begin");
            // Build Package
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.LogError("Build failed");
            }

            PostBuild(isDev);
        }

        public static void PreBuild(bool isDev)
        {

        }

        private static void PostBuild(bool isDev)
        {

        }

        public static void SetGradle()
        {


        }

    }
}
