using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

// Unity 4 light probes -> Unity 2018 (PS Vita port). Characters, zeds and pickups were lit by each level's baked light
// probes (Assets/Scenes/<level>/LightProbes.asset, Unity 4 format: bakedPositions, bakedCoefficients as 9 RGB triplets,
// tetrahedra). Unity 2018 cannot read that asset and the scenes lost their link to it, so probe-lit renderers got only
// the flat ambient (0.09 grey in the menu: NPCs nearly black on the Vita, 2026-10-26).
//
// Unity 2018 has no way to make a LightProbes object from data, so an empty scene holding only a LightProbeGroup at the
// Unity 4 positions is baked (no meshes or lights: Unity just tetrahedralizes the positions), the Unity 4 coefficients
// are written over the baked ones (matched to the nearest Unity 4 position, as Unity snaps probes to 1 cm; Unity 4 stores coefficient-major RGB triplets, Unity 2018
// SphericalHarmonicsL2 is [channel, coefficient], same basis order) and a copy is saved as LightProbesVita.asset next to
// the original. KFSceneLightmapsSetup stores it in the scene's KFSceneLightmaps, which assigns it at load.
public static class KFLightProbesConvert
{
	private const string BakeScene = "Assets/_KFProbeBake.unity";

	private const float MatchDistance = 0.05f;

	private static readonly Regex Vec = new Regex(@"\{x: ([^,]+), y: ([^,]+), z: ([^}]+)\}");

	private static readonly Regex Sh = new Regex(@"sh\[(\d+)\]: (\S+)");

	// Unity 4 LightProbes.asset -> LightProbesVita.asset (made once; delete it to redo)
	public static LightProbes Convert(string unity4Path)
	{
		string outPath = Path.Combine(Path.GetDirectoryName(unity4Path), "LightProbesVita.asset").Replace('\\', '/');
		LightProbes existing = AssetDatabase.LoadAssetAtPath<LightProbes>(outPath);
		if (existing != null)
		{
			return existing;
		}
		string text = File.ReadAllText(unity4Path);
		int coeffStart = text.IndexOf("  bakedCoefficients:");
		int tetraStart = text.IndexOf("  tetrahedra:");
		if (coeffStart < 0 || tetraStart < 0)
		{
			Debug.LogWarning("[KFProbes] " + unity4Path + ": not a Unity 4 light probe asset");
			return null;
		}
		List<Vector3> positions = new List<Vector3>();
		foreach (Match m in Vec.Matches(text.Substring(0, coeffStart)))
		{
			positions.Add(new Vector3(F(m.Groups[1].Value), F(m.Groups[2].Value), F(m.Groups[3].Value)));
		}
		List<float> values = new List<float>();
		foreach (Match m in Sh.Matches(text.Substring(coeffStart, tetraStart - coeffStart)))
		{
			values.Add(F(m.Groups[2].Value));
		}
		if (values.Count != positions.Count * 27)
		{
			Debug.LogWarning(string.Format("[KFProbes] {0}: {1} positions but {2} coefficients", unity4Path, positions.Count, values.Count));
			return null;
		}
		List<SphericalHarmonicsL2> unity4 = new List<SphericalHarmonicsL2>();
		for (int p = 0; p < positions.Count; p++)
		{
			SphericalHarmonicsL2 sh = new SphericalHarmonicsL2();
			for (int c = 0; c < 9; c++)
			{
				for (int rgb = 0; rgb < 3; rgb++)
				{
					sh[rgb, c] = values[p * 27 + c * 3 + rgb];
				}
			}
			unity4.Add(sh);
		}

		EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
		LightProbeGroup group = new GameObject("Probes").AddComponent<LightProbeGroup>();
		group.probePositions = positions.ToArray();
		Lightmapping.bakedGI = true;
		Lightmapping.realtimeGI = false;
		Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;
		EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), BakeScene);
		LightProbes result = null;
		try
		{
			if (!Lightmapping.Bake() || LightmapSettings.lightProbes == null)
			{
				Debug.LogWarning("[KFProbes] " + unity4Path + ": probe bake failed");
				return null;
			}
			LightProbes baked = LightmapSettings.lightProbes;
			Vector3[] bakedPositions = baked.positions;
			SphericalHarmonicsL2[] coefficients = baked.bakedProbes;
			int matched = 0;
			for (int i = 0; i < bakedPositions.Length; i++)
			{
				// Unity snaps probe positions to 1 cm: take the nearest Unity 4 probe
				int nearest = -1;
				float best = MatchDistance * MatchDistance;
				for (int p = 0; p < positions.Count; p++)
				{
					float d = (positions[p] - bakedPositions[i]).sqrMagnitude;
					if (d < best)
					{
						best = d;
						nearest = p;
					}
				}
				if (nearest >= 0)
				{
					coefficients[i] = unity4[nearest];
					matched++;
				}
			}
			LightProbes copy = Object.Instantiate(baked);
			copy.name = "LightProbesVita";
			copy.bakedProbes = coefficients;
			AssetDatabase.CreateAsset(copy, outPath);
			result = AssetDatabase.LoadAssetAtPath<LightProbes>(outPath);
			Debug.Log(string.Format("[KFProbes] {0}: {1} Unity 4 probes, {2} baked, {3} matched -> {4}", unity4Path, positions.Count, bakedPositions.Length, matched, outPath));
		}
		finally
		{
			Lightmapping.Clear();
			Lightmapping.lightingDataAsset = null;
			EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
			AssetDatabase.DeleteAsset(BakeScene);
			AssetDatabase.DeleteAsset(Path.ChangeExtension(BakeScene, null));
		}
		return result;
	}

	// Compares Unity's interpolated probe at each Unity 4 probe position with the Unity 4 coefficients there, with the
	// converted asset assigned. -scene <path> (default menu). Logs/lightprobes_verify.txt
	public static void Verify()
	{
		string[] args = System.Environment.GetCommandLineArgs();
		int a = System.Array.IndexOf(args, "-scene");
		string scenePath = a >= 0 && a + 1 < args.Length ? args[a + 1] : "Assets/Scenes/menu.unity";
		string folder = Path.ChangeExtension(scenePath, null).Replace('\\', '/');
		LightProbes probes = Convert(folder + "/LightProbes.asset");
		EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
		LightmapSettings.lightProbes = probes;
		string text = File.ReadAllText(folder + "/LightProbes.asset");
		int coeffStart = text.IndexOf("  bakedCoefficients:");
		MatchCollection pos = Vec.Matches(text.Substring(0, coeffStart));
		MatchCollection sh = Sh.Matches(text.Substring(coeffStart));
		double worst = 0, sum = 0;
		int n = 0;
		for (int p = 0; p < pos.Count; p++)
		{
			Vector3 at = new Vector3(F(pos[p].Groups[1].Value), F(pos[p].Groups[2].Value), F(pos[p].Groups[3].Value));
			SphericalHarmonicsL2 got;
			LightProbes.GetInterpolatedProbe(at, null, out got);
			for (int c = 0; c < 9; c++)
			{
				for (int rgb = 0; rgb < 3; rgb++)
				{
					double d = System.Math.Abs(got[rgb, c] - F(sh[p * 27 + c * 3 + rgb].Groups[2].Value));
					worst = System.Math.Max(worst, d);
					sum += d;
					n++;
				}
			}
		}
		string result = string.Format(CultureInfo.InvariantCulture, "{0}: {1} probes assigned ({2}), {3} Unity 4 probes checked, mean abs difference {4:0.00000}, worst {5:0.00000}",
			scenePath, probes != null ? probes.count : 0, LightmapSettings.lightProbes != null ? "lightProbes set" : "lightProbes NULL", pos.Count, n > 0 ? sum / n : -1, worst);
		Directory.CreateDirectory("Logs");
		File.WriteAllText("Logs/lightprobes_verify.txt", result + "\n");
		Debug.Log("[KFProbes] " + result);
	}

	private static float F(string s)
	{
		return float.Parse(s.Trim(), CultureInfo.InvariantCulture);
	}
}
