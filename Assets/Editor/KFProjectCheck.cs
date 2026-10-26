using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Port check after the Unity 4.6 -> 2018.2 upgrade (adapted from BOL's BOLSceneCheck). For every scene in the build
// settings and every prefab: missing scripts, object references that point at nothing, renderers without a mesh or
// material, legacy Animation components with clips but no default clip, and per scene the renderer / lightmap /
// static counts and the shaders in use. Writes Logs/project_check.txt.
// Batch: -executeMethod KFProjectCheck.Run
public static class KFProjectCheck
{
	private class Report
	{
		public int Objects, MissingScripts, BrokenRefs, Renderers, Lightmapped, Static, NoMesh, NoMaterial, AnimNoClip;
		public readonly Dictionary<string, int> Shaders = new Dictionary<string, int>();
		public readonly List<string> Lines = new List<string>();
	}

	public static void Run()
	{
		StringBuilder sb = new StringBuilder();
		Report total = new Report();
		Dictionary<string, int> allShaders = new Dictionary<string, int>();
		foreach (EditorBuildSettingsScene s in EditorBuildSettings.scenes)
		{
			if (!File.Exists(s.path))
			{
				sb.AppendLine("== " + s.path + ": NOT FOUND");
				continue;
			}
			EditorSceneManager.OpenScene(s.path, OpenSceneMode.Single);
			Report r = new Report();
			foreach (GameObject root in EditorSceneManager.GetActiveScene().GetRootGameObjects())
			{
				foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
				{
					CheckObject(t.gameObject, r);
				}
			}
			Write(sb, s.path + (s.enabled ? "" : " (disabled in build settings)"), r, true);
			Add(total, r, allShaders);
		}
		Report prefabs = new Report();
		foreach (string guid in AssetDatabase.FindAssets("t:Prefab"))
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
			if (prefab == null)
			{
				prefabs.Lines.Add("cannot load: " + path);
				continue;
			}
			int before = prefabs.Lines.Count;
			foreach (Transform t in prefab.GetComponentsInChildren<Transform>(true))
			{
				CheckObject(t.gameObject, prefabs);
			}
			for (int i = before; i < prefabs.Lines.Count; i++)
			{
				prefabs.Lines[i] = path + ": " + prefabs.Lines[i];
			}
		}
		Write(sb, "all prefabs", prefabs, false);
		Add(total, prefabs, allShaders);
		sb.AppendLine("== shaders used by scene renderers (renderer slots)");
		foreach (KeyValuePair<string, int> kv in allShaders.OrderByDescending(kv => kv.Value))
		{
			sb.AppendLine(string.Format("  {0,6}  {1}", kv.Value, kv.Key));
		}
		sb.Insert(0, string.Format("TOTAL objects {0}, missing scripts {1}, broken refs {2}, renderers without mesh {3}, empty material slots {4}, Animation without default clip {5}\n\n",
			total.Objects, total.MissingScripts, total.BrokenRefs, total.NoMesh, total.NoMaterial, total.AnimNoClip));
		Directory.CreateDirectory("Logs");
		File.WriteAllText("Logs/project_check.txt", sb.ToString());
		Debug.Log("KFProjectCheck: wrote Logs/project_check.txt");
	}

	private static void CheckObject(GameObject go, Report r)
	{
		r.Objects++;
		if ((GameObjectUtility.GetStaticEditorFlags(go) & StaticEditorFlags.BatchingStatic) != 0)
		{
			r.Static++;
		}
		foreach (Component c in go.GetComponents<Component>())
		{
			if (c == null)
			{
				r.MissingScripts++;
				r.Lines.Add("missing script on " + PathOf(go.transform));
				continue;
			}
			SerializedProperty p = new SerializedObject(c).GetIterator();
			while (p.Next(true))
			{
				if (p.propertyType == SerializedPropertyType.ObjectReference && p.objectReferenceValue == null && p.objectReferenceInstanceIDValue != 0)
				{
					r.BrokenRefs++;
					r.Lines.Add("broken reference " + c.GetType().Name + "." + p.propertyPath + " on " + PathOf(go.transform));
				}
			}
			Renderer renderer = c as Renderer;
			if (renderer != null && !(renderer is ParticleSystemRenderer))
			{
				CheckRenderer(renderer, r);
			}
			Animation anim = c as Animation;
			if (anim != null && anim.clip == null && anim.GetClipCount() > 0)
			{
				r.AnimNoClip++;
				r.Lines.Add("Animation with " + anim.GetClipCount() + " clips but no default clip (playAutomatically " + anim.playAutomatically + ") on " + PathOf(go.transform));
			}
		}
	}

	private static void CheckRenderer(Renderer renderer, Report r)
	{
		r.Renderers++;
		if (renderer.lightmapIndex >= 0 && renderer.lightmapIndex < 0xFFFE)
		{
			r.Lightmapped++;
		}
		MeshFilter mf = renderer.GetComponent<MeshFilter>();
		if ((renderer is MeshRenderer && (mf == null || mf.sharedMesh == null)) || (renderer is SkinnedMeshRenderer && ((SkinnedMeshRenderer)renderer).sharedMesh == null))
		{
			r.NoMesh++;
			r.Lines.Add("no mesh: " + PathOf(renderer.transform));
		}
		foreach (Material m in renderer.sharedMaterials)
		{
			string key;
			if (m == null)
			{
				r.NoMaterial++;
				key = "(empty material slot)";
			}
			else if (m.shader == null)
			{
				key = "(material without shader)";
			}
			else
			{
				key = m.shader.name + "  [" + AssetDatabase.GetAssetPath(m.shader) + "]";
			}
			int n;
			r.Shaders.TryGetValue(key, out n);
			r.Shaders[key] = n + 1;
		}
	}

	private static void Add(Report total, Report r, Dictionary<string, int> allShaders)
	{
		total.Objects += r.Objects;
		total.MissingScripts += r.MissingScripts;
		total.BrokenRefs += r.BrokenRefs;
		total.NoMesh += r.NoMesh;
		total.NoMaterial += r.NoMaterial;
		total.AnimNoClip += r.AnimNoClip;
		foreach (KeyValuePair<string, int> kv in r.Shaders)
		{
			int n;
			allShaders.TryGetValue(kv.Key, out n);
			allShaders[kv.Key] = n + kv.Value;
		}
	}

	private static void Write(StringBuilder sb, string title, Report r, bool scene)
	{
		sb.AppendLine("== " + title);
		sb.AppendLine(string.Format("  objects {0}, renderers {1}, lightmapped {2}, batching static {3}, missing scripts {4}, broken refs {5}, no mesh {6}, empty material slots {7}, Animation without default clip {8}",
			r.Objects, r.Renderers, r.Lightmapped, r.Static, r.MissingScripts, r.BrokenRefs, r.NoMesh, r.NoMaterial, r.AnimNoClip));
		foreach (string line in r.Lines.Take(scene ? 40 : 200))
		{
			sb.AppendLine("  " + line);
		}
		if (r.Lines.Count > (scene ? 40 : 200))
		{
			sb.AppendLine("  ... " + (r.Lines.Count - (scene ? 40 : 200)) + " more");
		}
	}

	private static string PathOf(Transform t)
	{
		string path = t.name;
		while (t.parent != null)
		{
			t = t.parent;
			path = t.name + "/" + path;
		}
		return path;
	}
}
