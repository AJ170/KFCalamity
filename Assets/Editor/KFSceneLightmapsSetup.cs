using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Puts the Unity 4 lightmaps back into the level scenes (PS Vita port; adapted from Borderlands' BOLImportedSceneSetup).
//
// Unity 4 (Android, GLES2) stored the maps as double LDR: the light is twice the texel value. Each LightmapFar-N.png
// is converted to LightmapFar-N.exr holding that light as HDR and imported as a Lightmap, so Unity encodes it for the
// platform and the built-in shaders decode it as they would a map baked in 2018. Then a KFSceneLightmaps object is
// added to the scene holding the maps, every renderer's index and scale/offset and every light's Unity 4 lightmapping
// mode, all read from the scene file as exported (Unity 2018 drops them on load and a saved scene no longer has them,
// so they are read from the untouched rip, RipScenes; file IDs survive saving). Rerunning replaces it.
//
// Batch: -executeMethod KFSceneLightmapsSetup.Run          (every scene in the build settings with LightmapFar maps)
//        -executeMethod KFSceneLightmapsSetup.Verify       (renders a lightmap to check the conversion; Logs/)
public static class KFSceneLightmapsSetup
{
	private const string ObjectName = "KFSceneLightmaps";

	// the scenes as AssetRipper exported them (Unity 4 format, this project's GUIDs), relative to the project folder.
	// The rip folder they came from is gone and a new export has different GUIDs: keep this copy.
	private const string RipScenes = "_Baseline/Scenes/";

	// Unity 4 renderer "no lightmap" values (255 none, 254 temporary)
	private const int NoLightmap = 254;

	private static readonly Regex Document = new Regex(@"^--- !u!(\d+) &(-?\d+)", RegexOptions.Multiline);

	private static readonly Regex IndexField = new Regex(@"^\s*m_LightmapIndex: (\d+)", RegexOptions.Multiline);

	private static readonly Regex OffsetField = new Regex(@"^\s*m_LightmapTilingOffset: \{x: ([^,]+), y: ([^,]+), z: ([^,]+), w: ([^}]+)\}", RegexOptions.Multiline);

	private static readonly Regex LightmappingField = new Regex(@"^\s*m_Lightmapping: (\d+)", RegexOptions.Multiline);

	private static readonly Regex LightmapList = new Regex(@"m_Lightmaps:\s*\n((?:\s*- m_Lightmap: \{[^}]*\}\s*\n\s*m_IndirectLightmap: \{[^}]*\}\s*\n)*)");

	private static readonly Regex LightmapEntry = new Regex(@"- m_Lightmap: \{fileID: \d+, guid: ([0-9a-f]{32})");

	private static readonly PropertyInfo InspectorMode = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);

	[MenuItem("KF/Set up Unity 4 lightmaps")]
	public static void Run()
	{
		foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
		{
			Setup(scene.path);
		}
	}

	private static void Setup(string scenePath)
	{
		string exported = RipScenes + Path.GetFileName(scenePath);
		if (!File.Exists(exported))
		{
			Debug.LogWarning("[KFLightmaps] " + scenePath + ": no exported scene at " + exported);
			return;
		}
		string text = File.ReadAllText(exported);
		Match list = LightmapList.Match(text);
		if (!list.Success)
		{
			return;
		}
		List<Texture2D> lightmaps = new List<Texture2D>();
		foreach (Match m in LightmapEntry.Matches(list.Groups[1].Value))
		{
			string png = AssetDatabase.GUIDToAssetPath(m.Groups[1].Value);
			lightmaps.Add(string.IsNullOrEmpty(png) ? null : ConvertLightmap(png));
		}
		if (lightmaps.Count == 0)
		{
			return;
		}
		// file ID -> exported renderer lightmap data / light mode
		Dictionary<long, KeyValuePair<int, Vector4>> rendererData = new Dictionary<long, KeyValuePair<int, Vector4>>();
		Dictionary<long, int> lights = new Dictionary<long, int>();
		MatchCollection docs = Document.Matches(text);
		for (int i = 0; i < docs.Count; i++)
		{
			int start = docs[i].Index;
			int end = i + 1 < docs.Count ? docs[i + 1].Index : text.Length;
			string body = text.Substring(start, end - start);
			long id = long.Parse(docs[i].Groups[2].Value);
			if (docs[i].Groups[1].Value == "108")
			{
				Match mode = LightmappingField.Match(body);
				if (mode.Success)
				{
					lights[id] = int.Parse(mode.Groups[1].Value);
				}
				continue;
			}
			Match index = IndexField.Match(body);
			Match offset = OffsetField.Match(body);
			if (index.Success && offset.Success)
			{
				int lightmap = int.Parse(index.Groups[1].Value);
				if (lightmap < NoLightmap && lightmap < lightmaps.Count)
				{
					rendererData[id] = new KeyValuePair<int, Vector4>(lightmap, new Vector4(F(offset.Groups[1].Value), F(offset.Groups[2].Value), F(offset.Groups[3].Value), F(offset.Groups[4].Value)));
				}
			}
		}
		EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
		GameObject old = GameObject.Find(ObjectName);
		if (old != null)
		{
			Object.DestroyImmediate(old);
		}
		List<Renderer> found = new List<Renderer>();
		List<int> indices = new List<int>();
		List<Vector4> offsets = new List<Vector4>();
		foreach (Renderer r in Resources.FindObjectsOfTypeAll<Renderer>())
		{
			KeyValuePair<int, Vector4> data;
			if (r.gameObject.scene.IsValid() && rendererData.TryGetValue(LocalId(r), out data))
			{
				found.Add(r);
				indices.Add(data.Key);
				offsets.Add(data.Value);
			}
		}
		List<Light> foundLights = new List<Light>();
		List<int> modes = new List<int>();
		foreach (Light l in Resources.FindObjectsOfTypeAll<Light>())
		{
			int mode;
			if (l.gameObject.scene.IsValid() && lights.TryGetValue(LocalId(l), out mode))
			{
				foundLights.Add(l);
				modes.Add(mode);
			}
		}
		KFSceneLightmaps component = new GameObject(ObjectName).AddComponent<KFSceneLightmaps>();
		component.m_lightmaps = lightmaps.ToArray();
		component.m_renderers = found.ToArray();
		component.m_indices = indices.ToArray();
		component.m_scaleOffsets = offsets.ToArray();
		component.m_lights = foundLights.ToArray();
		component.m_lightModes = modes.ToArray();
		component.m_staticBatch = StaticBatchCandidates(found);
		Lightmapping.lightingDataAsset = null;
		EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
		Debug.Log(string.Format("[KFLightmaps] {0}: {1} lightmaps, {2} of {3} lightmapped renderers, {4} of {5} lights, {6} static batched", scenePath, lightmaps.Count, found.Count, rendererData.Count, foundLights.Count, lights.Count, component.m_staticBatch.Length));
	}

	// Lightmapped scenery that never moves: active mesh renderers with no Animation, Animator, Rigidbody, Joint or script
	// on them or above them, and not referenced (nor any of their parents) by a script in the scene, as scripts move,
	// hide or swap what they hold. KFSceneLightmaps static batches these at load.
	private static GameObject[] StaticBatchCandidates(List<Renderer> lightmapped)
	{
		HashSet<GameObject> referenced = new HashSet<GameObject>();
		foreach (MonoBehaviour behaviour in Object.FindObjectsOfType<MonoBehaviour>())
		{
			if (behaviour == null || behaviour is KFSceneLightmaps)
			{
				continue;
			}
			SerializedProperty p = new SerializedObject(behaviour).GetIterator();
			while (p.Next(true))
			{
				if (p.propertyType != SerializedPropertyType.ObjectReference || p.objectReferenceValue == null)
				{
					continue;
				}
				GameObject go = p.objectReferenceValue as GameObject;
				Component c = p.objectReferenceValue as Component;
				if (c != null)
				{
					go = c.gameObject;
				}
				if (go != null && go.scene.IsValid())
				{
					referenced.Add(go);
				}
			}
		}
		List<GameObject> result = new List<GameObject>();
		foreach (Renderer r in lightmapped)
		{
			MeshFilter mf = r.GetComponent<MeshFilter>();
			if (!(r is MeshRenderer) || mf == null || mf.sharedMesh == null || !mf.sharedMesh.isReadable || !r.gameObject.activeInHierarchy)
			{
				continue;
			}
			bool moves = false;
			for (Transform t = r.transform; t != null && !moves; t = t.parent)
			{
				moves = referenced.Contains(t.gameObject) || t.GetComponent<Animation>() != null || t.GetComponent<Animator>() != null ||
					t.GetComponent<Rigidbody>() != null || t.GetComponent<Joint>() != null || t.GetComponent<MonoBehaviour>() != null;
			}
			if (!moves)
			{
				result.Add(r.gameObject);
			}
		}
		return result.ToArray();
	}

	// LightmapFar-N.png (double LDR) -> LightmapFar-N.exr (the light itself), imported as a Lightmap
	public static Texture2D ConvertLightmap(string pngPath)
	{
		string exrPath = Path.ChangeExtension(pngPath, ".exr");
		if (!File.Exists(exrPath))
		{
			Texture2D png = new Texture2D(2, 2, TextureFormat.RGBA32, false, true);
			png.LoadImage(File.ReadAllBytes(pngPath));
			Color[] pixels = png.GetPixels();
			bool linear = PlayerSettings.colorSpace == ColorSpace.Linear;
			for (int i = 0; i < pixels.Length; i++)
			{
				Color c = pixels[i];
				Color light = new Color(c.r * 2f, c.g * 2f, c.b * 2f, 1f);
				// the importer treats the EXR as linear light; in a gamma project it stores it gamma encoded
				pixels[i] = linear ? light : new Color(Mathf.GammaToLinearSpace(light.r), Mathf.GammaToLinearSpace(light.g), Mathf.GammaToLinearSpace(light.b), 1f);
			}
			Texture2D hdr = new Texture2D(png.width, png.height, TextureFormat.RGBAHalf, false, true);
			hdr.SetPixels(pixels);
			File.WriteAllBytes(exrPath, hdr.EncodeToEXR(Texture2D.EXRFlags.CompressZIP));
			Object.DestroyImmediate(png);
			Object.DestroyImmediate(hdr);
			AssetDatabase.ImportAsset(exrPath);
		}
		TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(exrPath);
		if (importer.textureType != TextureImporterType.Lightmap)
		{
			importer.textureType = TextureImporterType.Lightmap;
			importer.wrapMode = TextureWrapMode.Clamp;
			importer.SaveAndReimport();
		}
		return AssetDatabase.LoadAssetAtPath<Texture2D>(exrPath);
	}

	// Renders a white Legacy Diffuse quad lit only by the first map of a scene and compares it with what Unity 4 drew
	// (2 x the PNG texel, clamped). -scene <path> (default asylum_scene). Logs/lightmap_verify.txt
	public static void Verify()
	{
		string[] args = System.Environment.GetCommandLineArgs();
		int a = System.Array.IndexOf(args, "-scene");
		string scenePath = a >= 0 && a + 1 < args.Length ? args[a + 1] : "Assets/Scenes/asylum_scene.unity";
		string png = Path.Combine(Path.ChangeExtension(scenePath, null), "LightmapFar-0.png").Replace('\\', '/');
		Texture2D map = ConvertLightmap(png);
		EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
		RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
		RenderSettings.ambientLight = Color.black;
		LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
		LightmapSettings.lightmaps = new[] { new LightmapData { lightmapColor = map } };
		GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
		MeshRenderer renderer = quad.GetComponent<MeshRenderer>();
		Material white = new Material(Shader.Find("Legacy Shaders/Diffuse"));
		white.color = Color.white;
		white.mainTexture = Texture2D.whiteTexture;
		renderer.sharedMaterial = white;
		renderer.lightmapIndex = 0;
		renderer.lightmapScaleOffset = new Vector4(1f, 1f, 0f, 0f);
		Mesh mesh = Object.Instantiate(quad.GetComponent<MeshFilter>().sharedMesh);
		mesh.uv2 = mesh.uv;
		quad.GetComponent<MeshFilter>().sharedMesh = mesh;
		Camera camera = new GameObject("Camera").AddComponent<Camera>();
		camera.orthographic = true;
		camera.orthographicSize = 0.5f;
		camera.transform.position = new Vector3(0f, 0f, -1f);
		camera.clearFlags = CameraClearFlags.SolidColor;
		camera.backgroundColor = Color.magenta;
		const int size = 256;
		RenderTexture rt = new RenderTexture(size, size, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
		camera.targetTexture = rt;
		camera.Render();
		RenderTexture.active = rt;
		Texture2D shot = new Texture2D(size, size, TextureFormat.RGB24, false);
		shot.ReadPixels(new Rect(0, 0, size, size), 0, 0);
		RenderTexture.active = null;
		Texture2D source = new Texture2D(2, 2, TextureFormat.RGBA32, false, true);
		source.LoadImage(File.ReadAllBytes(png));
		double sumGot = 0, sumWant = 0, sumAbs = 0;
		int n = 0;
		for (int y = 8; y < size - 8; y += 4)
		{
			for (int x = 8; x < size - 8; x += 4)
			{
				float u = (x + 0.5f) / size, v = (y + 0.5f) / size;
				float want = Mathf.Min(1f, source.GetPixelBilinear(u, v).g * 2f);
				float got = shot.GetPixel(x, y).g;
				sumGot += got;
				sumWant += want;
				sumAbs += Mathf.Abs(got - want);
				n++;
			}
		}
		Directory.CreateDirectory("Logs");
		File.WriteAllBytes("Logs/lightmap_verify.png", shot.EncodeToPNG());
		string result = string.Format(CultureInfo.InvariantCulture, "{0}: colour space {1}, mean rendered {2:0.000}, mean Unity 4 {3:0.000}, mean abs difference {4:0.000} over {5} samples ({6})",
			png, PlayerSettings.colorSpace, sumGot / n, sumWant / n, sumAbs / n, n, map.format);
		File.WriteAllText("Logs/lightmap_verify.txt", result + "\n");
		Debug.Log("[KFLightmaps] " + result);
	}

	private static long LocalId(Object o)
	{
		SerializedObject so = new SerializedObject(o);
		InspectorMode.SetValue(so, UnityEditor.InspectorMode.Debug, null);
		SerializedProperty p = so.FindProperty("m_LocalIdentfierInFile"); // sic
		return p == null ? 0 : p.longValue;
	}

	private static float F(string s)
	{
		return float.Parse(s.Trim(), CultureInfo.InvariantCulture);
	}
}
