using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Port tool (adapted from Borderlands' BOLBuiltinShaderMap): the rip's Assets/Shader holds dummy copies of the Unity 4
// built-in shaders the game used (Diffuse, Mobile/Diffuse, Particles/..., GUI/Text Shader, ...). For every project
// shader this finds the Unity 2018 built-in of the same name, or its "Legacy Shaders/" rename, and writes
// Logs/shader_map.tsv: project path, project guid, name, built-in name, built-in guid, built-in file ID (empty when there
// is none: a custom shader). Tools: _Tools/remap_shaders.py points every reference at the built-ins.
// Batch: -executeMethod KFBuiltinShaderMap.Run
public static class KFBuiltinShaderMap
{
	// Unity 4 built-ins renamed in Unity 5 beyond the "Legacy Shaders/" prefix (same properties)
	private static readonly Dictionary<string, string> Renamed = new Dictionary<string, string>
	{
		{ "RenderFX/Skybox", "Skybox/6 Sided" }
	};

	public static void Run()
	{
		Dictionary<string, Shader> builtins = new Dictionary<string, Shader>();
		foreach (string path in new[] { "Resources/unity_builtin_extra", "Library/unity default resources" })
		{
			foreach (Object o in AssetDatabase.LoadAllAssetsAtPath(path))
			{
				Shader s = o as Shader;
				if (s != null && !builtins.ContainsKey(s.name))
				{
					builtins.Add(s.name, s);
				}
			}
		}
		List<string> lines = new List<string>();
		foreach (string guid in AssetDatabase.FindAssets("t:Shader", new[] { "Assets" }))
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
			string name = shader != null ? shader.name : ShaderNameFromFile(path);
			Shader builtin;
			string renamed;
			if (!builtins.TryGetValue(name, out builtin) && !builtins.TryGetValue("Legacy Shaders/" + name, out builtin) && Renamed.TryGetValue(name, out renamed))
			{
				builtins.TryGetValue(renamed, out builtin);
			}
			string builtinGuid = string.Empty;
			long builtinId = 0;
			if (builtin != null)
			{
				AssetDatabase.TryGetGUIDAndLocalFileIdentifier(builtin, out builtinGuid, out builtinId);
			}
			lines.Add(string.Join("\t", new[] { path, guid, name, builtin != null ? builtin.name : string.Empty, builtinGuid, builtin != null ? builtinId.ToString() : string.Empty }));
		}
		lines.Sort();
		Directory.CreateDirectory("Logs");
		File.WriteAllLines("Logs/shader_map.tsv", lines.ToArray());
		File.WriteAllLines("Logs/builtin_shaders.txt", builtins.Keys.OrderBy(k => k).ToArray());
		Debug.Log("[KFShaderMap] " + lines.Count + " project shaders, " + lines.Count(l => !l.EndsWith("\t")) + " with a built-in, " + builtins.Count + " built-ins");
	}

	private static string ShaderNameFromFile(string path)
	{
		string first = File.ReadAllLines(path).FirstOrDefault(l => l.TrimStart().StartsWith("Shader \"")) ?? string.Empty;
		int a = first.IndexOf('"'), b = first.IndexOf('"', a + 1);
		return a >= 0 && b > a ? first.Substring(a + 1, b - a - 1) : string.Empty;
	}
}
