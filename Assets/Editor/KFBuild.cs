using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

// PS Vita build (adapted from Borderlands' BOLBuild). Every build applies the PS Vita settings below, so they live in
// code rather than in hand-edited project settings.
// Batch: Unity.exe -batchmode -quit -projectPath <path> -buildTarget PSP2 -executeMethod KFBuild.BuildVita -logFile <log>
public static class KFBuild
{
	public const string OutputPath = "Builds/PSVita";

	public const string TitleId = "KFCL00001";

	// The Android build's quality level (no pixel lights: real-time lights per vertex, no shadows), so the Vita draws
	// what the Android game drew
	private const string QualityLevel = "android_default";

	public static void ConfigureVita()
	{
		PlayerSettings.colorSpace = ColorSpace.Gamma;
		PlayerSettings.SetScriptingBackend(BuildTargetGroup.PSP2, ScriptingImplementation.IL2CPP);
		// the game reaches engine code by reflection (SendMessage, AddComponent by name): keep it whole
		PlayerSettings.stripEngineCode = false;
		PlayerSettings.PSVita.shortTitle = "KF Calamity";
		PlayerSettings.PSVita.contentID = "IV0000-" + TitleId + "_00-0123456789ABCDEF";
		PlayerSettings.PSVita.masterVersion = "01.00";
		PlayerSettings.PSVita.appVersion = "01.00";
		PlayerSettings.PSVita.enterButtonAssignment = PlayerSettings.PSVita.PSVitaEnterButtonAssignment.CrossButton;
		PlayerSettings.PSVita.memoryExpansionMode = PlayerSettings.PSVita.PSVitaMemoryExpansionMode.ExpandBy109MB;
		PlayerSettings.PSVita.powerMode = PlayerSettings.PSVita.PSVitaPowerMode.ModeB;
		PlayerSettings.PSVita.acquireBGM = true;
		foreach (GraphicsTier tier in Enum.GetValues(typeof(GraphicsTier)))
		{
			TierSettings settings = EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.PSP2, tier);
			settings.renderingPath = RenderingPath.Forward;
			EditorGraphicsSettings.SetTierSettings(BuildTargetGroup.PSP2, tier, settings);
		}
		// The levels have no Unity lighting data (KFSceneLightmaps applies the Unity 4 maps at load), so automatic
		// stripping would drop every lightmapped shader variant and the levels would draw unlit: keep plain lightmaps.
		SerializedObject graphics = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
		graphics.FindProperty("m_LightmapStripping").intValue = 1;
		graphics.FindProperty("m_LightmapKeepPlain").boolValue = true;
		graphics.ApplyModifiedPropertiesWithoutUndo();
		int level = Array.IndexOf(QualitySettings.names, QualityLevel);
		if (level >= 0)
		{
			SerializedObject quality = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/QualitySettings.asset")[0]);
			SerializedProperty platforms = quality.FindProperty("m_PerPlatformDefaultQuality");
			for (int i = 0; i < platforms.arraySize; i++)
			{
				SerializedProperty entry = platforms.GetArrayElementAtIndex(i);
				if (entry.FindPropertyRelative("first").stringValue == "PSP2")
				{
					entry.FindPropertyRelative("second").intValue = level;
				}
			}
			quality.ApplyModifiedPropertiesWithoutUndo();
		}
		AssetDatabase.SaveAssets();
	}

	[MenuItem("KF/Build PS Vita")]
	public static void BuildVita()
	{
		Build(BuildOptions.None, OutputPath);
	}

	private static void Build(BuildOptions buildOptions, string outputPath)
	{
		ConfigureVita();
		string[] scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
		BuildPlayerOptions options = new BuildPlayerOptions
		{
			scenes = scenes,
			locationPathName = outputPath,
			target = BuildTarget.PSP2,
			options = buildOptions
		};
		// On a retail Vita every log stack trace fails ("backtrace failed") and the spam pushes the real messages out of
		// the console buffer kept in crash dumps; and a release player with "Use Player Log" off passes no Debug.Log on.
		LogType[] logTypes = (LogType[])Enum.GetValues(typeof(LogType));
		StackTraceLogType[] previousStackTraces = logTypes.Select(t => PlayerSettings.GetStackTraceLogType(t)).ToArray();
		foreach (LogType t in logTypes)
		{
			PlayerSettings.SetStackTraceLogType(t, StackTraceLogType.None);
		}
		bool previousPlayerLog = PlayerSettings.usePlayerLog;
		PlayerSettings.usePlayerLog = true;
		BuildReport report;
		try
		{
			report = BuildPipeline.BuildPlayer(options);
		}
		finally
		{
			PlayerSettings.usePlayerLog = previousPlayerLog;
			for (int i = 0; i < logTypes.Length; i++)
			{
				PlayerSettings.SetStackTraceLogType(logTypes[i], previousStackTraces[i]);
			}
		}
		BuildResult result = report.summary.result;
		Debug.Log(string.Format("[KFBUILD] result={0} errors={1} warnings={2} size={3} time={4}",
			result, report.summary.totalErrors, report.summary.totalWarnings, report.summary.totalSize, report.summary.totalTime));
		foreach (BuildStep step in report.steps)
		{
			foreach (BuildStepMessage message in step.messages)
			{
				if (message.type == LogType.Error || message.type == LogType.Exception)
				{
					Debug.Log("[KFBUILD] " + step.name + ": " + message.content);
				}
			}
		}
		if (Application.isBatchMode)
		{
			EditorApplication.Exit(result == BuildResult.Succeeded ? 0 : 1);
		}
	}
}
